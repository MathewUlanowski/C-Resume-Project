using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Exam.Models
{
    public class Attendees
    {
        [Key]
        public int AttendeesId {get;set;}

        public int UserId {get;set;}
        public int EventId {get;set;}

        public User User {get;set;}
        public Event Event {get;set;}
    }
}