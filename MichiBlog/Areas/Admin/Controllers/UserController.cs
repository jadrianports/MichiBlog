﻿using AspNetCoreHero.ToastNotification.Abstractions;
using MichiBlog.Models;
using MichiBlog.WebApp.Utilities;
using MichiBlog.WebApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MichiBlog.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notification;
        public UserController(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                INotyfService notification) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notification = notification;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userVMs = users.Select(x => new UserVM() 
            { 
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                Email = x.Email
            }).ToList();

            foreach (var user in userVMs) { 
                var singleUser = await _userManager.FindByIdAsync(user.Id);
                var role = await _userManager.GetRolesAsync(singleUser);
                user.Role = role.FirstOrDefault();
            }

            return View(userVMs);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddUser() {
            return View(new RegisterVM());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddUser(RegisterVM registerVM)
        {
            if (!ModelState.IsValid) { return View(registerVM); }
            var checkUserByEmail = await _userManager.FindByEmailAsync(registerVM.Email);
            if (checkUserByEmail != null) { 
                _notification.Error("Email already exists");
                return View(registerVM);
            }
            var checkUserByUserName = await _userManager.FindByNameAsync(registerVM.UserName);
            if(checkUserByUserName != null) {
                _notification.Error("Username already exists");
                return View(registerVM);
            }
            var applicationUser = new ApplicationUser()
            {
                Email = registerVM.Email,
                UserName = registerVM.UserName,
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName
            };

            var result = await _userManager.CreateAsync(applicationUser, registerVM.Password);
            if (result.Succeeded) {
                if (registerVM.IsAdmin) {
                    await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteAdmin);
                }
                else {
                    await _userManager.AddToRoleAsync(applicationUser, WebsiteRoles.WebsiteUser);
                }
                _notification.Success("Registration Successful");
                RedirectToAction("Index", "User", new {area = "Admin"});
            }
            return View(registerVM);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditUser(string id) {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _notification.Error("User not found");
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, WebsiteRoles.WebsiteAdmin);
            var editUserVM = new EditUserVM
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                IsAdmin = isAdmin
            };

            return View(editUserVM);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserVM editUserVM)
        {
            if (!ModelState.IsValid)
            {
                return View(editUserVM);
            }

            var user = await _userManager.FindByIdAsync(editUserVM.Id);
            if (user == null)
            {
                _notification.Error("User not found");
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }

            user.FirstName = editUserVM.FirstName;
            user.LastName = editUserVM.LastName;
            user.Email = editUserVM.Email;
            user.UserName = editUserVM.UserName;

            var roles = await _userManager.GetRolesAsync(user);
            if (editUserVM.IsAdmin)
            {
                await _userManager.AddToRoleAsync(user, WebsiteRoles.WebsiteAdmin);
                await _userManager.RemoveFromRoleAsync(user, WebsiteRoles.WebsiteUser);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, WebsiteRoles.WebsiteUser);
                await _userManager.RemoveFromRoleAsync(user, WebsiteRoles.WebsiteAdmin);
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _notification.Success("User updated successfully");
            }
            else
            {
                _notification.Error("Failed to update user");
            }

            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id) 
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null) { 
                _notification.Error("User not found");
                return View();
            }
            return View(new ResetPassword { Id = existingUser.Id, UserName = existingUser.UserName });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        { 
            if (!ModelState.IsValid) { 
                return View(resetPassword);
            }
            var existingUser = await _userManager.FindByIdAsync(resetPassword.Id);
            if (existingUser == null) { 
                _notification.Error("User not found");
                return View(resetPassword);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
            var result = await _userManager.ResetPasswordAsync(existingUser, token, resetPassword.NewPassword);
            if (result.Succeeded) { 
                _notification.Success("Password reset successful");
                return RedirectToAction(nameof(Index));
            }
            return View(resetPassword);
        }
    }
}
