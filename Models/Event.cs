using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Exam.Models
{
    public class Event
    {
        [Key]
        public int EventId {get;set;}
        
        [Required]
        public string Title {get;set;}

        [Required]
        public DateTime Date {get;set;}

        [Required]
        public int Duration {get;set;}
        
        [Required]
        public string DurationIndicator {get;set;}

        [Required]
        public string Description {get;set;}

        public int UserId {get;set;}
        public User Coordinator {get;set;}

        public IList<Attendees> Attendees {get;set;}
    }
}