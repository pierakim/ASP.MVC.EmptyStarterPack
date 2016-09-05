using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        //OwinContext is shared across your entire application. :)
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private readonly ApplicationDbContext _db = new ApplicationDbContext();

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

        public ActionResult Index(ManageMessageId? message, tb_UserDetails model)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.UpdateDetailInformation ? "Your details have been updated."
               :message == ManageMessageId.ErrorUpdateDetailInformation ? "Error during update."
               : "";

            //return 2 models with Partial/.. instead of this
            var manager = new UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(new ApplicationDbContext()));
            var currentUser = manager.FindById(User.Identity.GetUserId());

            model = (from s in _db.UsersDetails
                     where s.UserId == currentUser.Id
                     select s).FirstOrDefault();

            return View(model);
        }

        [Authorize]
        public ActionResult MoreUserDetail(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /User/MoreUserDetail
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MoreUserDetail(tb_UserDetails model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //is there an user?
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            //user detail already provided?
            var userDetails = _db.UsersDetails.Any(w => w.UserId == user.Id);

            if (user != null)
            {
                if(!userDetails)
                { 
                    //New UserDetails
                    var newUserDetail = new tb_UserDetails {UserId = user.Id,  Age = model.Age, Address = model.Address, City = model.City, Phone = model.Phone };
                    _db.UsersDetails.Add(newUserDetail);
                    _db.SaveChanges();
                    return RedirectToAction("Index", new { Message = ManageMessageId.UpdateDetailInformation });
                }
                else
                {
                    //Existing userDetails
                    var existingUserDetails = (from s in _db.UsersDetails
                                              where s.UserId == user.Id
                                              select s).FirstOrDefault();
                    //New data
                    if (model.Age != null)
                        if (existingUserDetails != null) existingUserDetails.Age = model.Age;
                    if (model.Address != null)
                        if (existingUserDetails != null) existingUserDetails.Address = model.Address;
                    if (model.City != null)
                        if (existingUserDetails != null) existingUserDetails.City = model.City;
                    if (model.Phone != null)
                        if (existingUserDetails != null) existingUserDetails.Phone = model.Phone;

                    //Update
                    _db.UsersDetails.Attach(existingUserDetails);
                    _db.Entry(existingUserDetails).State=EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index", new { Message = ManageMessageId.UpdateDetailInformation });
                }
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.ErrorUpdateDetailInformation });
        }

    }
}