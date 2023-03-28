using BigSchool.Models;
using BigSchool.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses

        public ActionResult Create()
        {
            var viewModel = new CourseViewModel
            {
                Categories = _dbcontext.Categories.ToList(),
                Heading = "Add Course"
            };
            return View(viewModel);
        }
        private readonly ApplicationDbContext _dbcontext;
        public CoursesController()
        {
            _dbcontext = new ApplicationDbContext();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Categories = _dbcontext.Categories.ToList();
                return View("Create", viewModel);
            }
            var course = new Course
            {
                LecturerId = User.Identity.GetUserId(),
                DateTime = viewModel.GetDateTime(),
                CategoryId = viewModel.Category,
                Place = viewModel.Place
            };
            _dbcontext.Courses.Add(course);
            _dbcontext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();
            var courses = _dbcontext.Attendances
                .Where(a=>a.AttendeeId== userId)
                .Select(a=>a.Course)
                .Include(l=>l.Lecturer)
                .Include(l=>l.Category)
                .ToList();
            var viewModel = new CourseViewModel
            {
                UpcommingCourses = courses,
                ShowAction = User.Identity.IsAuthenticated
            };
            return View(viewModel);
        }
        [Authorize]
        public ActionResult Mine()
        {
            var userId= User.Identity.GetUserId();
            var course = _dbcontext.Courses
                .Where(c=>c.LecturerId== userId && c.DateTime > DateTime.Now)
                .Include(l=>l.Lecturer)
                .Include(c=>c.Category)
                .ToList();
            return View(course);
        }
        [Authorize]
        public ActionResult Edit(int id)
        {
            var userId=User.Identity.GetUserId();
            var course=_dbcontext.Courses.Single(c=>c.Id== id && c.LecturerId==userId);
            var viewModel = new CourseViewModel
            {
                Categories = _dbcontext.Categories.ToList(),
                Date = course.DateTime.ToString("dd/M/yyyy"),
                Time = course.DateTime.ToString("HH:mm"),
                Category = course.CategoryId,
                Place = course.Place,
                Heading="Edit Course",
                Id=course.Id
            };
            return View("Create",viewModel);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(CourseViewModel viewModel)
        {
            
                if (!ModelState.IsValid)
                {
                    viewModel.Categories = _dbcontext.Categories.ToList();
                    return View("Create", viewModel);
                }
                var userId = User.Identity.GetUserId();
                var course = _dbcontext.Courses.Single(c => c.Id == viewModel.Id && c.LecturerId == userId);
                course.Place = viewModel.Place;
                course.DateTime = viewModel.GetDateTime();
                course.CategoryId = viewModel.Category;
                _dbcontext.SaveChanges();
                return RedirectToAction("Index", "Home");
        }
    }
}