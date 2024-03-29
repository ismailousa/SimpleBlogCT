﻿using SimpleBlog.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleBlog.Areas.Admin.ViewModels;
using SimpleBlog.Models;
using NHibernate.Linq;

namespace SimpleBlog.Areas.Admin.Controllers
{
    [Authorize(Roles ="admin")]
    [SelectedTabAttribute("users")]
    public class UsersController : Controller
    {
        // GET: Admin/Users
        public ActionResult Index()
        {
            var users = Database.Session.Query<User>().ToList(); //select * from users
            return View(new UsersIndex() { Users = users });
        }

        private void SyncRoles(IList<RoleCheckBox> checkBoxes, IList<Role> roles)
        {
            var selectedRoles = new List<Role>();


            foreach (var role in Database.Session.Query<Role>())
            {
                var checkBox = checkBoxes.Single(c => c.Id == role.Id);
                checkBox.Name = role.Name;

                if (checkBox.IsChecked)
                    selectedRoles.Add(role);
            }


            foreach (var toAdd in selectedRoles.Where(p => !roles.Contains(p)))
            {
                roles.Add(toAdd);
            }

            foreach (var toRemove in roles.Where(p => !selectedRoles.Contains(p)).ToList())
            {
                roles.Remove(toRemove);
            }


        }

        public ActionResult New()
        {
            return View(new UsersNew
            {
                Roles = Database.Session.Query<Role>().Select(
                    role => new RoleCheckBox()
                    {
                        Id = role.Id,
                        Name = role.Name
                    }).ToList()
            });
        }
        [HttpPost]
        public ActionResult New(UsersNew form)
        {
            if (Database.Session.Query<User>().Any(u => u.Username == form.Username))
                ModelState.AddModelError("Username", "Username must be unique");
            if (!ModelState.IsValid)
                return View(form);
            var user = new User
            {
                Email = form.Email,
                Username = form.Username
            };

            SyncRoles(form.Roles, user.Roles);
            user.SetPassword(form.Password);
            Database.Session.Save(user);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var user = Database.Session.Load<User>(id);
            if (User == null)
                return HttpNotFound();

            return View(new UsersEdit
            {
                Username = user.Username,
                Email = user.Email,
                Roles = Database.Session.Query<Role>().Select(
                        role => new RoleCheckBox()
                        {
                            Id = role.Id,
                            Name = role.Name,
                            IsChecked = user.Roles.Contains(role)

                        }).ToList()
            });
        }

        [HttpPost]
        public ActionResult Edit(int id, UsersEdit form)
        {
            var user = Database.Session.Load<User>(id);
            if (User == null)
                return HttpNotFound();
            SyncRoles(form.Roles, user.Roles);

            if (Database.Session.Query<User>().Any(u => u.Username == form.Username && u.Id != id))
                ModelState.AddModelError("Username", "Username must be unique");

            if (!ModelState.IsValid)
                return View(form);

            user.Username = form.Username;
            user.Email = form.Email;
            Database.Session.Update(user);

            return RedirectToAction("index");
        }

        public ActionResult ResetPassword(int id)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();

            return View(new UsersResetPassword
            {
                Username = user.Username
            });
        }

        [HttpPost]
        public ActionResult ResetPassword(int id, UsersResetPassword form)
        {
            var user = Database.Session.Load<User>(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            form.Username = user.Username;

            if (!ModelState.IsValid)
                return View(form);

            user.SetPassword(form.Password);


            Database.Session.Update(user);

            return RedirectToAction("index");
        }

        public ActionResult Delete(int id)
        {
            var user = Database.Session.Load<User>(id);

            if (user == null)
                return HttpNotFound();

            Database.Session.Delete(user);
            return RedirectToAction("index");
        }

        //[HttpPost]
        //public ActionResult Delete(int id)
        //{
        //    var user = Database.Session.Load<User>(id);

        //    if (user == null)
        //        return HttpNotFound();

        //    Database.Session.Delete(user);
        //    return RedirectToAction("index");
        //}
    }
}