using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ASP.MVC.Scratch.App_Start;
using ASP.MVC.Scratch.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ASP.MVC.Scratch.Controllers
{
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private ApplicationDbContext db = new ApplicationDbContext();

        public UserController()
        {
        }

        public enum ManageMessageId
        {
            UpdateDetailInformation,
            ErrorUpdateDetailInformation,
            Error
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
        }

        public ActionResult Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.UpdateDetailInformation ? "Your details have been updated."
               :message == ManageMessageId.ErrorUpdateDetailInformation ? "Error during update."
               : "";

            return View();
        }

        [Authorize]
        public ActionResult MoreUserDetail(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MoreUserDetail(tb_UserDetails model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            var id = user.Id;
            if (user != null)
            {
                var record = new tb_UserDetails {UserId = id,  Age = 30, Address ="35 boundary St.", City = "Brisbane", Phone = 0480254785 };
                db.UsersDetails.Add(record);
                db.SaveChanges();
                return RedirectToAction("Index", new { Message = ManageMessageId.UpdateDetailInformation });
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.ErrorUpdateDetailInformation });
        }

    }
}