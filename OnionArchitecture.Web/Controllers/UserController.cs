using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OA.Service;
using OA.Web.Models;
using OA.Data;
using Microsoft.AspNetCore.Http;

namespace OA.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<UserViewModel> model = new List<UserViewModel>();
            userService.GetUsers().ToList().ForEach(u =>
            {
                User user_fromdb = userService.GetUser(u.Id);

                UserViewModel user = new UserViewModel
                {
                    Id = u.Id,
                    Name = $"{user_fromdb.FirstName} {user_fromdb.LastName}",
                    Email = u.Email,
                    Address = user_fromdb.Address
                };
                model.Add(user);
            });

            return View(model);
        }

        [HttpGet]
        public ActionResult AddUser()
        {
            UserViewModel model = new UserViewModel();

            return PartialView("_AddUser", model);
        }

        [HttpPost]
        public ActionResult AddUser(UserViewModel model)
        {
            User userEntity = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                AddedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
            };
            userService.InsertUser(userEntity);
            if (userEntity.Id > 0)
            {
                return RedirectToAction("index");
            }
            return View(model);
        }

        public ActionResult EditUser(int? id)
        {
            UserViewModel model = new UserViewModel();
            if (id.HasValue && id != 0)
            {
                User userEntity = userService.GetUser(id.Value);
                model.FirstName = userEntity.FirstName;
                model.LastName = userEntity.LastName;
                model.Address = userEntity.Address;
                model.Email = userEntity.Email;
            }
            return PartialView("_EditUser", model);
        }

        [HttpPost]
        public ActionResult EditUser(UserViewModel model)
        {
            User userEntity = userService.GetUser(model.Id);
            userEntity.Email = model.Email;
            userEntity.ModifiedDate = DateTime.UtcNow;
            userEntity.IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            //UserProfile userProfileEntity = userProfileService.GetUserProfile(model.Id);
            userEntity.FirstName = model.FirstName;
            userEntity.LastName = model.LastName;
            userEntity.Address = model.Address;
            userEntity.ModifiedDate = DateTime.UtcNow;
            userEntity.IPAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            //userEntity.UserProfile = userProfileEntity;
            userService.UpdateUser(userEntity);
            if (userEntity.Id > 0)
            {
                return RedirectToAction("index");
            }
            return View(model);
        }



    }
}