using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    public class HomeController : Controller
    {
        private ToDoContext context;

        public HomeController(ToDoContext ctx) => context = ctx;

        public IActionResult Index(string id)
        {
            var filters = new Filters(id);
            ViewBag.Filters = filters;

            ViewBag.Cartegories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            ViewBag.DueFilters = Filters.DueFilterValues;

            IQueryable<ToDo> query = context.ToDos
                .Include(t => t.Category)
                .Include(t => t.Status);

            if (filters.HasCategory)
            {
                query = query.Where(t => t.CategoryId == filters.CategoryId);
            }

            if (filters.HasStatus)
            {
                query = query.Where(t => t.StatusId == filters.StatusId);
            }

            if (filters.HasDue)
            {
                var today = DateTime.Today;
                if (filters.IsPast)
                {
                    query = query.Where(t => t.DueDate < today);
                }
                else if (filters.IsFuture)
                {
                    query = query.Where(t => t.DueDate < today);
                }
                else if (filters.IsToday)
                {
                    query = query.Where(t => t.DueDate < today);
                }

            }
            var tasks = query.OrderBy(t => t.DueDate).ToList();

            return View(tasks);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Categories = context.Categories.ToList();
            ViewBag.Statuses = context.Statuses.ToList();
            var task = new ToDo { StatusId = "open" };
            return ViewBag(task);

        }

        [HttpPost]
        public IActionResult Add(ToDo task)
        {
            if (ModelState.IsValid) //data validation 
            {
                context.ToDos.Add(task); //adds it to the database 
                context.SaveChanges(); //saves those changes
                return RedirectToAction("Index"); //redirects to home action 
            }
            else //otherwise reload the view bag
            {
                ViewBag.Categories = context.Categories.ToList();
                ViewBag.Statuses = context.Statuses.ToList();
                return View(task);
            }
        }

        [HttpPost]
        public IActionResult Filter(string[] filter)
        {
            string id = string.Join('-', filter);
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult MarkComplete([FromRoute]string id, ToDo selected) //hard codes to get id from the URL, then bind ToDo obj called "selected"
        {
            selected = context.ToDos.Find(selected.Id)!; //hit database using that objects id

            if(selected != null) //if not null, close it
            {
                selected.StatusId = "closed";
                context.SaveChanges();
            }
            return RedirectToAction("Index", new { ID = id });
        }

        [HttpPost]
        public IActionResult DeleteComplete(string id)
        {
            var toDelete = context.ToDos.Where(t => t.StatusId == "closed").ToList(); //hit up all todos that have a "closed" status id

            foreach(var task in toDelete) //loop through them 
            {
                context.ToDos.Remove(task); //remove each individual one
            }

            context.SaveChanges(); //save changes

            return RedirectToAction("Index", new { ID = id });
        }
    }
}