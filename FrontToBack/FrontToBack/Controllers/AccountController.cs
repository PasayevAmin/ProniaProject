using FrontToBack.Models;
using FrontToBack.Utilities.Enums;
using FrontToBack.Utilities.Extension;
using FrontToBack.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;
namespace FrontToBack.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }



        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM)
        {
            if (!ModelState.IsValid) return View();
            //if (!userVM.CheeckWords(userVM.Name) || !userVM.IsSymbol(userVM.Name) ||! userVM.IsDigit(userVM.Name))
            //{
            //    ModelState.AddModelError("Name", "your name mustn't contain space,numbers and symbol");
            //    return View();
            //}
            //if (!userVM.CheeckWords(userVM.Surname) || !userVM.IsSymbol(userVM.Surname) || !userVM.IsDigit(userVM.Surname))
            //{
            //    ModelState.AddModelError("Surname", "your Surname mustn't contain space,numbers and symbol");
            //    return View();
            //}

            if (!userVM.CheeckEmail(userVM.Email))
            {
                ModelState.AddModelError("Email", "your email type isn't true please try again");
                return View();
            }
            
            AppUser appUser = new AppUser
            {
                Name = userVM.Name.CapitalizeName(),
                Surname = userVM.Surname.CapitalizeName(),
                Email = userVM.Email,
                UserName = userVM.Username,
                Gender = userVM.Gender,
            };
            IdentityResult result = await _userManager.CreateAsync(appUser, userVM.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(appUser,UserRole.Member.ToString());
            await _signInManager.SignInAsync(appUser, false);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult SignIn( )
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(LoginVM loginvm,string? returnurl)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.FindByNameAsync(loginvm.UserNameOrEmail);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(loginvm.UserNameOrEmail);
                if (user == null)
                {
                    ModelState.AddModelError(String.Empty, "Username,Email or Password is incorrect");
                    return View();
                }
            }

            var value = await _signInManager.PasswordSignInAsync(user, loginvm.Password, loginvm.IsRemembered,true);
            if (value.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "You are Bloced");
                return View();
            }
            if (!value.Succeeded)
            {
                ModelState.AddModelError(String.Empty, "Username,Email or Password is incorrect");
                return View();
            }
            if (returnurl is null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(returnurl);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> CreateRole()
        {
            foreach (UserRole item in Enum.GetValues(typeof(UserRole)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = item.ToString(),
                    });
                }
               
            }
            return RedirectToAction("Index", "Home");   
        }
    }
}
