using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class UserManagementController : Controller
    {
        #region Declaration
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        #endregion
        public ActionResult UserMaster()
        {
            return View();
        }
        #region RoleMaster

        [HttpGet]
        public ActionResult RoleMaster()
        {
            ViewBag.msg = "";
            ViewBag.error = "";
            Roles rl = new Roles();

            try
            {
                rl.RoleList = RestClient.RoleGetAll().Where(x => x.RoleName.ToLower() != "admin");
                return View(rl);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorPage", "PayTime");

            }

        }
        [HttpPost]

        public ActionResult RoleMaster(Roles rl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ViewBag.msg = "";
                    ViewBag.error = "";
                    if (rl.RoleId > 0)
                    {
                        rl.RoleList = RestClient.RoleGetAll().Where(c => c.RoleName.ToLower() == rl.RoleName.ToLower() && c.RoleId != rl.RoleId);
                        if (rl.RoleList.Count() > 0)
                        {
                            //Response.Write("<script>alert('Role Name already exists.')</script>");
                            ViewBag.error = rl.RoleName + " Role name already exists.";
                        }
                        else
                        {
                            if (!RestClient.RoleUpdate(rl.RoleId, rl))
                            {
                                //Response.Write("<script>alert('Role not updated due to service issue.')</script>");
                                ViewBag.error = "Role  not updated.";
                            }
                            else
                            {
                                //  Response.Write("<script>alert('Role is Updated.')</script>");
                                ViewBag.msg = "Role updated successfully.";
                            }
                        }
                    }
                    else
                    {
                        rl.RoleList = RestClient.RoleGetAll().Where(c => c.RoleName.ToLower() == rl.RoleName.ToLower());
                        if (Session["RoleId"].ToString() != "6805" && Session["RoleId"].ToString() != "6806")
                        {
                            rl.CompanyID = 0;
                            rl.BranchID = 0;
                        }
                        else if (Session["RoleId"].ToString() == "6805" || Session["RoleId"].ToString() == "6806")
                        {
                            var cmpid = Convert.ToInt32(Session["ClientCompanyId"].ToString());
                            var BranchID = Convert.ToInt32(Session["BranchId"].ToString());
                            rl.CompanyID = cmpid;
                            rl.BranchID = BranchID;
                        }
                        if (rl.RoleList.Count() > 0)
                        {
                            //Response.Write("<script>alert('Role Name already exists.')</script>");
                            ViewBag.error = rl.RoleName + " Role name already exists.";
                        }
                        RestClient.RoleAdd(rl);
                        // Response.Write("<script>alert('Role save Sucessfully.')</script>");
                        ViewBag.msg = "Role added successfully.";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Not Valid Entry.')</script>");
                    //ViewBag.error = "Not Valid Entry";
                }
                rl.RoleList = RestClient.RoleGetAll().Where(c => c.RoleName.ToLower() != "admin");
                return View(rl);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        [HttpPost]
        public string DeleteRoleMaster(string id)
        {
            string sts = "";
            if (id != null)
            {
                if (RestClient.RoleDelete(Convert.ToInt32(id.Trim())))
                {
                    sts = "OK";
                }
            }
            return new JavaScriptSerializer().Serialize(sts);
        }
        #endregion

        #region rolerights
        [HttpGet]

        public ActionResult RightDistribution()
        {
            try
            {
                RoleRightsMapping model = new RoleRightsMapping();
                var rolelist = RestClient.RoleGetAll();
                ViewBag.RoleList = rolelist.Where(x => x.RoleName.ToLower() != "admin");
                //model.RoleMenuData = null;
                return View(model);
            }
            catch (Exception)
            {

                return RedirectToAction("ErrorPage", "PayTime");
            }

        }
        public static class CommonUtility
        {
            public static bool ToBoolean(string _value)
            {
                bool _flag = false;
                if (_value == null)
                    return _flag;
                else if (_value.ToLower() == "on" || _value.ToLower() == "yes")
                {
                    return true;
                }
                else
                {
                    Boolean.TryParse(_value, out _flag);
                }
                return _flag;
            }
        }
        [HttpPost]

        public ActionResult RightDistribution(RoleRightsMapping model, String Command)
        {
            try
            {
                var rolelist = RestClient.RoleGetAll();
                ViewBag.RoleList = rolelist.Where(x => x.RoleName.ToLower() != "admin");
                ViewBag.msg = "";
                ViewBag.error = "";                
                if (model.RoleId > 0)
                {
                    model.RoleMenuData = RestClient.RoleRightsGetAll(model.RoleId);
                    model.RoleMenuData = model.RoleMenuData.Where(x => x.OperationId > 0 && x.ParentMenuId != 0);
                    int i = 0;
                    if (Command == "Set Permission")
                    {
                        try
                        {
                            bool flg;
                            flg = RestClient.RoleRightsDelete(model.RoleId);
                            if (flg)
                            {
                                foreach (var s in model.RoleMenuData)
                                {
                                    model.OperationId = s.OperationId;
                                    model.CanView = CommonUtility.ToBoolean(Request.Form["chkCanView_" + s.OperationId]);
                                    model.CanAdd = CommonUtility.ToBoolean(Request.Form["chkCanAdd_" + s.OperationId]);
                                    model.CanEdit = CommonUtility.ToBoolean(Request.Form["chkCanEdit_" + s.OperationId]);
                                    //model.CanDelete = CommonUtility.ToBoolean(Request.Form["chkCanDelete_" + s.OperationId]);
                                    model.CanDelete = false;
                                    model.CanImport = CommonUtility.ToBoolean(Request.Form["CanImport_" + s.OperationId]);
                                    model.CanExport = CommonUtility.ToBoolean(Request.Form["CanExport_" + s.OperationId]);
                                    if (model.CanView == true || model.CanAdd == true || model.CanEdit == true ||
                                        model.CanImport == true || model.CanExport == true)
                                    {
                                        RestClient.RoleRightsAdd(model);
                                    }
                                    i++;
                                }
                                ViewBag.msg = "Rights updated successfully.";
                                model.RoleMenuData = RestClient.RoleRightsGetAll(model.RoleId);

                            }
                            else
                            {
                                ViewBag.error = "Issue in service.";
                            }

                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                    {
                        ViewBag.error = "Please select role.";
                    }

                }
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "PayTime");
            }
        }
        #endregion
    }

}