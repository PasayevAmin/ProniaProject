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


        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }



        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM,Gender gender)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            string pattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b";
            Regex regex = new Regex(pattern);
            bool resultemail = regex.Match(registerVM.Email).Success;
            if (!resultemail)
            {
                ModelState.AddModelError("Email", "Your Email isn't true pleasse try again");
                return View();
            }

            AppUser user = new AppUser
            {
                Email = registerVM.Email,
                Name = registerVM.Name.CapitalizeName(),
                Surname = registerVM.Surname.CapitalizeName(),
                UserName = registerVM.UserName,
                Gender=gender
            };

           
            

            IdentityResult result= await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var eror in result.Errors)
                {
                    ModelState.AddModelError(String.Empty,eror.Description);
                }
                return View();
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            


            return RedirectToAction("Index","Home");
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }
    }
}
