using PayTimeWebClient.Helper;
using PayTimeWebClient.Infrastructure;
using PayTimeWebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayTimeWebClient.Controllers
{
    [CustAuthFilter]
    public class SchoolUserManagementController : Controller
    {
        //static readonly SchoolMasterDataRestClient RestClient = new SchoolMasterDataRestClient();
        static readonly MasterDataRestClient RestClient = new MasterDataRestClient();
        #region Roles
        public ActionResult Roles()
        {
            return View();
        }
        #endregion

        #region Rights
        public ActionResult Rights()
        {
            try
            {
                //RoleRightsMapping model = new RoleRightsMapping();
                RoleRightsMapping model = new RoleRightsMapping();
                //var rolelist = RestClient.SchoolRoleGetAll();
                ViewBag.RoleList = RestClient.RoleGetAll().Where(x => x.RoleName.ToLower() != "admin");

                //ViewBag.RoleList = RestClient.RoleGetAll().Where(x => x.RoleName.ToLower() != "admin");
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
        public ActionResult Rights(RoleRightsMapping model, String Command)
        {
            try
            {
                //var rolelist = RestClient.SchoolRoleGetAll();
                //ViewBag.RoleList = rolelist.Where(x => x.RoleName.ToLower() != "admin");
                ViewBag.RoleList = RestClient.RoleGetAll().Where(x => x.RoleName.ToLower() != "admin");
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
                                ViewBag.msg = "Rights Updated Successfully.";
                                model.RoleMenuData = RestClient.RoleRightsGetAll(model.RoleId);
                                model.RoleMenuData = model.RoleMenuData.Where(x => x.OperationId > 0 && x.ParentMenuId != 0);

                            }
                            else
                            {
                                ViewBag.error = "Issue in Updating Rights.";
                            }

                        }
                        catch (Exception)
                        {

                        }
                    }
                    else
                    {
                        ViewBag.error = "Please Select Role.";
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