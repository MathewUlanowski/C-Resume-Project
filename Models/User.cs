using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Exam.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required]
        public string UserName {get;set;}

        [Required]
        public string Email {get;set;}

        public IList<Event> Cordinated {get;set;}

        [Required]
        public string Password {get;set;}

        // this is the one to many key
        public IList<Attendees> Attending {get;set;}


        [NotMapped]
        [Compare("Password", ErrorMessage="Your passwords did not match please make sure that they are the same.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword {get;set;}
    }
}