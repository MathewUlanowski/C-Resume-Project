using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Exam.Models;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Exam.Controllers
{
    public class HomeController : Controller
    {
        private Context dbContext;
        public HomeController(Context context)
        {
            dbContext = context;
        }



        // auto redirect to sign in so URL looks clean
        public IActionResult Index()
        {
            return RedirectToAction("SignInPage");
        }


        // this is the view for the initial login/registration page
        [HttpGet("signin")]
        public IActionResult SignInPage()
        {
            return View("index");
        }

        
        // handler for registraion of the account creates database entries when user model conditions are met
        [HttpPost("RegistrationHandler")]
        public IActionResult RegistrationHandler(IndexModel modelData)
        {
            Console.WriteLine($"\n\n\n{modelData}\n{modelData.IndexUser.UserName}\n{modelData.IndexUser.Email}\n{modelData.IndexUser.Password}\n\n\n");
            if(ModelState.IsValid)
            {
                PasswordHasher<User> HasherSlasher = new PasswordHasher<User>();
                modelData.IndexUser.Password = HasherSlasher.HashPassword(modelData.IndexUser, modelData.IndexUser.Password);
                dbContext.Users.Add(modelData.IndexUser);
                dbContext.SaveChanges();
                return RedirectToAction("SignInPage");
            }
            return View("Index", modelData);
        }

        // handler for logging in validates password and weather theres a user in the database with the entered email handles the second form on the index page
        [HttpPost("LoginHandler")]
        public IActionResult LoginHandler(IndexModel modelData)
        {
            Console.WriteLine($"\n\n\n{modelData.LoginModel}\n{modelData.LoginModel.Password}\n{modelData.LoginModel.Email}\n\n\n");
            if(ModelState.IsValid)
            {
                Console.WriteLine("\n\n\nModel state is valid\n\n\n");
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == modelData.LoginModel.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index", modelData);
                }

                var hashItAndSlashIt = new PasswordHasher<LoginModel>();
                var result = hashItAndSlashIt.VerifyHashedPassword(modelData.LoginModel, userInDb.Password, modelData.LoginModel.Password);

                if(result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index", modelData);
                }
                HttpContext.Session.SetInt32("Validator", userInDb.UserId);
                return RedirectToAction("HomePage");
            }
            return View("Index", modelData);
        }

        // this is the home page where it lists activities
        [HttpGet("home")]
        public IActionResult HomePage()
        {
            ViewModel HomeView = new ViewModel();
            HomeView.CurrentUser = dbContext.Users.FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("Validator"));
            HomeView.AllEvents = dbContext.Events.Include(attn => attn.Attendees).Include(cord => cord.Coordinator).ToList();
            return View("Home", HomeView);
        }
        
        [HttpGet("new")]
        public IActionResult NewEvent()
        {
            EventCreate creatingEvent = new EventCreate();
            creatingEvent.CurrentUser = dbContext.Users.FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("Validator"));
            return View("New", creatingEvent);
        }

        //this handles the creation of the event from the form in NewEvent View
        [HttpPost("new")]
        public IActionResult EventHandler(EventCreate modelData, TimeSpan ETime)
        {
            // Console.WriteLine(ModelState.IsValid);
            modelData.CurrentUser = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("Validator"));
            // Console.WriteLine($"\n\n\n{modelData.Event.Date}");
            // Console.WriteLine($"{ETime}\n\n\n");
            // Console.WriteLine($"\n\n\nPostEvent: {modelData.Event.Title}\n\n\n");
            // Console.WriteLine($"\n\n\nuserID: {HttpContext.Session.GetInt32("Validator")}\nUsername: {dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("Validator")).UserName}\n\n\n");
            modelData.Event.Coordinator = dbContext.Users.FirstOrDefault(u => u.UserId == (int) HttpContext.Session.GetInt32("Validator"));
            modelData.Event.Date = modelData.Event.Date.Add(ETime);
            // Console.WriteLine($"\n\n\n{ModelState.IsValid}\n\n\n");
            if(ModelState.IsValid)
            {
                //applies the one to many key for coordiantor

                dbContext.Events.Add(modelData.Event);
                dbContext.SaveChanges();
                return RedirectToAction("HomePage");
            }
            return View("New");
        }

        //this shows one event and the details for it
        [HttpGet("event/{eventId}")]
        public IActionResult ViewEvent(Event SpecificEvent, int eventId)
        {
            SpecificEvent = dbContext.Events
            .Include(c => c.Coordinator)
            .Include(a => a.Attendees).ThenInclude(at => at.User)
            .FirstOrDefault(e => e.EventId == eventId);
            return View("Event", SpecificEvent);
        }
        [HttpGet("/join/{evid}")]
        public IActionResult Join(Attendees NewLink,int evid)
        {
            NewLink.User = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("Validator"));
            NewLink.Event = dbContext.Events
                .Include(attn => attn.Attendees).ThenInclude(at => at.User)
                .FirstOrDefault(e => e.EventId == evid);
            Console.WriteLine(ModelState.IsValid);
            if(ModelState.IsValid)
            {
                if(
                    !NewLink.Event.Attendees
                        .Any(u => u.UserId == NewLink.User.UserId)
                )
                {
                    Attendees somenewAttendee = new Attendees ()
                    {
                        UserId = NewLink.User.UserId,
                        EventId = evid,
                        User = NewLink.User,
                        Event = NewLink.Event
                    };
                    dbContext.UserEventLinks.Add(somenewAttendee);
                    dbContext.SaveChanges();
                }
            }
            return RedirectToAction("ViewEvent", new{SpecificEvent = NewLink.Event, eventId = evid});
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
