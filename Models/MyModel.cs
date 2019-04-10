using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeltExam.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required]
        [MinLength(2, ErrorMessage="Name should be at least 2 characters or longer!")]
        public string Name {get;set;}
        [Required]
        [EmailAddress]
        public string Email {get;set;}
        [Required]
        [DataType(DataType.Password)]
        [RegularExpression("(?-i)(?=^.{8,}$)((?!.*s)(?=.*[A-Z])(?=.*[a-z]))(?=(1)(?=.*d)|.*[^A-Za-z0-9])^.*$", ErrorMessage="Password should be a min length of 8 characters, contain at least 1 number, 1 letter, and a special character.")]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        public string Password {get;set;}
        [NotMapped]
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string Confirm {get;set;}
        public List<Activity> MyActivities {get;set;}
        public List<Attendee> Activities {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
    public class Activity
    {
        [Key]
        public int ActivityId {get;set;}
        [Required]
        public string Name {get;set;}
        [Required]
        public DateTime Date {get;set;}
        [Required]
        public int Duration {get;set;}
        [Required]
        public string Measure {get;set;}
        [Required]
        public string Description {get;set;}
        public int UserId {get;set;}
        public User Creator {get;set;}
        public List<Attendee> Attendees {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        
    }
    public class Attendee
    {
        [Key]
        public int AttendeeId {get;set;}
        public int UserId {get;set;}
        public int ActivityId {get;set;}
        public User User {get;set;}
        public Activity Activity {get;set;}
    }
    public class LoginClass
    {
        public User RegisterUser {get;set;}
        public LoginUser LoginUser {get;set;}
    }
    public class LoginUser
    {
        [Required]
        public string Email {get;set;}
        [Required]
        public string Password {get;set;}
    }
}