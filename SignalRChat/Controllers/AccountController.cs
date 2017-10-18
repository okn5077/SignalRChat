using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using SignalRChat.DAL;
using SignalRChat.Models;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SignalRChat.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private RoleManager<ApplicationRole> customerRoleManager;
        public AccountController()
        {
            IdentityContext db = new IdentityContext();
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
            userManager = new UserManager<ApplicationUser>(userStore);
            RoleStore<ApplicationRole> roleStore = new RoleStore<ApplicationRole>(db);
            customerRoleManager = new RoleManager<ApplicationRole>(roleStore);
        }

        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Register newMember)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser rep = userManager.FindByName("representative");

                ApplicationUser member = new ApplicationUser();
                member.Name = newMember.Name + " " + newMember.Surname;
                member.UserName = newMember.Username;

                string pass = newMember.Password;
                IdentityResult iResult = userManager.Create(member, pass);
                if (iResult.Succeeded)
                {
                    if (newMember.IsRep)
                        userManager.AddToRole(member.Id, "Representative");
                    else
                    {
                        member.Representative = rep;
                        member.RepId = new Guid(rep.Id);
                        iResult = userManager.Update(member);
                        userManager.AddToRole(member.Id, "Customer");
                    }
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("RegisterUser", "Kullanıcı ekleme işleminde hata!");
                }
            }
            return View(newMember);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = userManager.Find(model.Username, model.Password);
                if (user != null)
                {
                    IAuthenticationManager authManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                    ClaimsIdentity identity = userManager.CreateIdentity(user, "ApplicationCookie");
                    AuthenticationProperties authProps = new AuthenticationProperties();
                    authProps.IsPersistent = model.RememberMe;
                    authManager.SignIn(authProps, identity);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("LoginUser", "Böyle bir kullanıcı bulunamadı");
                    return View("Login");
                }
            }
            return View(model);
        }
    }
}