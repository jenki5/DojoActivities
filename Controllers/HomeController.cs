using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BeltExam.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {
        private BeltExamContext dbContext;
        public HomeController(BeltExamContext context)
        {
            dbContext = context;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("CreateUser")]
        public IActionResult CreateUser(LoginClass user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.RegisterUser.Email))
                {
                    ModelState.AddModelError("RegisterUser.Email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.RegisterUser.Password = Hasher.HashPassword(user.RegisterUser, user.RegisterUser.Password);
                dbContext.Add(user.RegisterUser);
                dbContext.SaveChanges();
                Console.WriteLine("I'm in the right controller");
                User CurrentUser = dbContext.Users.FirstOrDefault(u => u.Email == user.RegisterUser.Email);
                HttpContext.Session.SetInt32("UserId", CurrentUser.UserId);
                return Redirect("/home");
            }
            else
            {
                return View("Index");
            }
        }
        [HttpPost("LoginUser")]
        public IActionResult LoginUser(LoginClass user)
        {
            var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == user.LoginUser.Email);
            if(userInDb == null)
            {
                ModelState.AddModelError("LoginUser.Email", "Invalid Email/Password");
                return View("Index");
            }
            var hasher = new PasswordHasher<LoginUser>();
            var result = hasher.VerifyHashedPassword(user.LoginUser, userInDb.Password, user.LoginUser.Password);
            if(result == 0)
            {
                ModelState.AddModelError("LoginUser.Email", "Invalid Email/Password");
                return View("Index");
            }
            else
            {
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return Redirect("/home");
            }
        }
        [HttpGet("Home")]
        public IActionResult Home()
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                ModelState.AddModelError("LoginUser.Email", "You are not logged in");
                return View("Index");
            }
            else
            {
                ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
                List<Models.Activity> AllActivities = dbContext.Activities
                .Where(a => a.Date > DateTime.Now)
                .OrderBy(a => a.Date)
                .Include(a => a.Creator)
                .Include(a => a.Attendees)
                .ThenInclude(att => att.User)
                .ToList();
                return View(AllActivities);
            }
        }
        [HttpGet("NewActivity")]
        public IActionResult NewActivity()
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                ModelState.AddModelError("LoginUser.Email", "You are not logged in");
                return View("Index");
            }
            else
            {
                return View();
            }
        }
        [HttpPost("CreateActivity")]
        public IActionResult CreateActivity(Models.Activity activity)
        {
            if(activity.Date < DateTime.Now)
            {
                ModelState.AddModelError("Date", "Activity must be in the future!");
                return View("NewActivity");
            }
            activity.UserId = (int)HttpContext.Session.GetInt32("UserId");
            dbContext.Add(activity);
            dbContext.SaveChanges();
            Models.Activity curactivity = dbContext.Activities.FirstOrDefault(a => a.Name == activity.Name && a.Description == activity.Description);

            return Redirect($"/activity/{curactivity.ActivityId}");
        }
        [HttpGet("activity/{id}")]
        public IActionResult Activity(int id)
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                ModelState.AddModelError("LoginUser.Email", "You are not logged in");
                return View("Index");
            }
            else
            {
                var curactivity = dbContext.Activities
                .Include(a => a.Creator)
                .Include(a => a.Attendees)
                .ThenInclude(u => u.User)
                .FirstOrDefault(a => a.ActivityId == id);
                return View("Activity", curactivity);
            }
        }
        [HttpGet("JoinActivity/{id}")]
        public IActionResult JoinActivity(int id)
        {
            Attendee attendee = new Attendee();
            attendee.UserId = (int)HttpContext.Session.GetInt32("UserId");
            attendee.ActivityId = id;
            dbContext.Add(attendee);
            dbContext.SaveChanges();
            return Redirect("/home");
        }
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
        }
        [HttpGet("DeleteActivity/{id}")]
        public IActionResult DeleteActivity(int id)
        {
            Models.Activity RetrievedActivity = dbContext.Activities.SingleOrDefault(a => a.ActivityId == id);
            dbContext.Activities.Remove(RetrievedActivity);
            dbContext.SaveChanges();
            return Redirect("/home");
        }
        [HttpGet("LeaveActivity/{id}")]
        public IActionResult LeaveActivity(int id)
        {
            Attendee RetrievedAttendee = dbContext.Attendees.SingleOrDefault(a => a.ActivityId == id && a.UserId == HttpContext.Session.GetInt32("UserId"));
            dbContext.Attendees.Remove(RetrievedAttendee);
            dbContext.SaveChanges();
            return Redirect("/home");
        }
    }
}
