﻿using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SimpleBlog.Areas.Admin.ViewModels
{
    public class RoleCheckBox
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Boolean IsChecked { get; set; }
    }

    public class UsersIndex
    {
        public IEnumerable<User> Users { get; set; }
    }

    public class UsersNew
    {
        [Required, MaxLength(128)]
        public string Username { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, MaxLength(256), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public IList<RoleCheckBox> Roles { get; set; }
    }

    public class UsersEdit
    {
        [Required, MaxLength(128)]
        public string Username { get; set; }
        [Required, MaxLength(256), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public IList<RoleCheckBox> Roles { get; set; }
    }

    public class UsersResetPassword
    {
        public string Username { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}