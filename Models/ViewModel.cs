using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Exam.Models
{
    public class IndexModel
    {
        public LoginModel LoginModel {get;set;}
        public User IndexUser {get;set;}
    }

    public class ViewModel
    {
        public User CurrentUser;
        public IList<Event> AllEvents;
    }

    public class EventCreate
    {
        public User CurrentUser;
        public Event Event {get;set;}
    }

    public class LoginModel
    {
        [Required]
        public string Email {get;set;}
        [Required]
        public string Password {get;set;}
    }
}