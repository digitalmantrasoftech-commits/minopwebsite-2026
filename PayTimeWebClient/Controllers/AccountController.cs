using PayTimeWebClient.Helper;
using PayTimeWebClient.Models;
using System;
using System.Web;
using System.Web.Mvc;
namespace PayTimeWebClient.Controllers
{
    public class AccountController : Controller
    {
       static readonly IAccountRestClint RestAccout = new AccountRestClint();
       MRespo resp = new MRespo();


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(LoginModel model)
        {

            if (ModelState.IsValid)
            {
                resp = RestAccout.Register(model);
            }
            else
            {
                ModelState.AddModelError(resp.MegSts, resp.Meg);
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                resp = RestAccout.Login(model);
                if (resp.MegSts == "jwt_token")
                {
                    HttpCookie ck = new HttpCookie("jwt_token");
                    ck.Value = resp.Meg;
                    ck.Expires = DateTime.Now.AddMinutes(60);
                    Response.Cookies.Add(ck);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(resp.MegSts, resp.Meg);
                }

            }
            else
            {
                ModelState.AddModelError(resp.MegSts, resp.Meg);
            }

            return View(model);
        }

        public ActionResult Logoff()
        {
            Session.RemoveAll();
            Session.Abandon();
            Session["tokan"] = null;
            return RedirectToAction("Index", "PayTime", false);
        }

        public ActionResult Logoffdeveloper()
        {
            Session.RemoveAll();
            Session.Abandon();
            Session["tokan"] = null;
            return RedirectToAction("DevelopersAccount", "PayTime", false);
        }



    }
}