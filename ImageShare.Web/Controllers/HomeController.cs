using ImageShare.Data;
using ImageShare.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ImageShare.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Manager manager = new Manager(Properties.Settings.Default.ConStr);
            IndexViewModel vm = new IndexViewModel();
            vm.Top5Date = manager.Top5Date();
            vm.Top5Liked = manager.Top5Liked();
            vm.Top5Viewed = manager.Top5Viewed();
            vm.LoggedIn = User.Identity.IsAuthenticated;
            if(vm.LoggedIn)
            {
                string email = User.Identity.Name;
                vm.User = manager.GetByEmail(email);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult addImage(string firstName, string lastName, HttpPostedFileBase image)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            image.SaveAs(Server.MapPath("~/Images") + "/" + fileName);
            Image i = new Image
            {
                FirstName = firstName,
                LastName = lastName,
                FileName = fileName,
                DateUploaded = DateTime.Now,
                ViewCount = 0
            };
            Manager manager = new Manager(Properties.Settings.Default.ConStr);
            int Id = manager.AddImage(i);
            var vm = new NewImageViewModel();
            vm.ImageId = Id;
            return View(vm);
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult addUser(User user, string password)
        {
            Manager manager = new Manager(Properties.Settings.Default.ConStr);
            manager.AddUser(user, password);
            FormsAuthentication.SetAuthCookie(user.Email, true);
            return Redirect("/home/index");
        }
        public ActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            Manager manager = new Manager(Properties.Settings.Default.ConStr);
            User user =manager.LogIn(email, password);
            if(user == null)
            {
                return Redirect("/home/signIn");
            }
            FormsAuthentication.SetAuthCookie(email, true);
            return Redirect("/home/index");
        }
        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/home/index");
        }
        [Authorize]
        public ActionResult Likes()
        {
            Manager manager = new Manager(Properties.Settings.Default.ConStr);
            string email = User.Identity.Name;
            User user = manager.GetByEmail(email);
            LikesViewModel vm = new LikesViewModel();
            vm.Images = manager.GetLikesForUser(user.Id);
            vm.User = user;
            return View(vm);
        }
        public ActionResult Details(int ImageId)
        {
            var manager = new Manager(Properties.Settings.Default.ConStr);
            var vm = new DetailsViewModel();
            vm.Image = manager.GetImageById(ImageId);
            manager.AddViewCount(ImageId);
            if (User.Identity.IsAuthenticated)
            {
                string email = User.Identity.Name;
                vm.User = manager.GetByEmail(email);
            }
            return View(vm);
        }
        [Authorize]
        [HttpPost]
        public void AddLike(Like like)
        {
            var manager = new Manager(Properties.Settings.Default.ConStr);
            manager.AddLike(like);
        }
        public ActionResult LikedAlready(Like like)
        {
            var manager = new Manager(Properties.Settings.Default.ConStr);
            bool liked = manager.LikedAlready(like);
            return Json(new {likedAlready = liked }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetViewCount(int ImageId)
        {
            var manager = new Manager(Properties.Settings.Default.ConStr);
            Image image = manager.GetImageById(ImageId);
            return Json(new { VC = image.ViewCount }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLikes(int ImageId)
        {
            var manager = new Manager(Properties.Settings.Default.ConStr);
            Image image = manager.GetImageById(ImageId);
            return Json(new { Likes = image.Likes }, JsonRequestBehavior.AllowGet);
        }
    }
}