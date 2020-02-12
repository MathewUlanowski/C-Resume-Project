using Microsoft.EntityFrameworkCore;
 
namespace Exam.Models
{
    public class Context : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public Context(DbContextOptions options) : base(options) { }

        public DbSet<User> Users {get;set;}
        public DbSet<Event> Events {get;set;}
        public DbSet<Attendees> UserEventLinks {get;set;}
    }
}