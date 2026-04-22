using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayTimeWebClient.Models;
using PayTimeWebClient.Helper;

namespace PayTimeWebClient.Controllers
{
    public class DealerRegistrationController : Controller
    {
        //
        // GET: /DealerRegistration/
        static readonly IDealerRegistration DealerAcco = new DealerRegistration();
        MRespo resp = new MRespo();

        public ActionResult DealerRegistration()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DealerRegistration(DealerRegisterModel  model)
        {

            resp = DealerAcco.Register(model);
            return View();
        }

        public ActionResult DealerLogin() {
            
            return View();
        }

        [HttpPost]
        public ActionResult DealerLogin(DealerRegisterModel model)
        {

            if (ModelState.IsValid)
            {
                resp = DealerAcco.Login(model);
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


	}
}