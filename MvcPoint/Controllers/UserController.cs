using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using MvcPoint.Models;

namespace MvcPoint.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        
        //
        // GET: /User/
        private PointContext db = new PointContext();
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 查询所有用户信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="U_LoginName"></param>
        /// <param name="U_RealName"></param>
        /// <param name="U_Telephone"></param>
        /// <returns></returns>
        public ActionResult GetUserPaged(int page, int rows, string U_LoginName = "", string U_RealName = "", string U_Telephone = "")
        {
            Models.User u2 = (Models.User)Session["User"];
            string sql = "P_GetUserPaged";
            int r = 0;
            SqlParameter[] sp = {
                                    new SqlParameter("PageSize",rows),
                                    new SqlParameter("PageIndex",page),
                                    new SqlParameter("Count",r),
                                    new SqlParameter("U_LoginName",U_LoginName),
                                    new SqlParameter("U_RealName",U_RealName),
                                    new SqlParameter("U_Telephone",U_Telephone),
                                    new SqlParameter("S_ID",u2.S_ID)
                                };
            sp[2].Direction = ParameterDirection.Output;
            DataTable dt = DBHelper.ExecuteSelect(sql, true, sp);
            List<object> ls = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.CategoryItem c = dr.ToModel<Models.CategoryItem>();
                Models.Shop s = dr.ToModel<Models.Shop>();
                Models.User u = dr.ToModel<Models.User>();
                ls.Add(new { CategoryItem = c, Shop = s, User = u });
            }
            var data = new { total = sp[2].Value, rows = ls };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            try
            {
                var u = db.User.Find(id);
                db.User.Remove(u);
                db.SaveChanges();
                return Content("1");
            }
            catch (Exception)
            {
                return Content("0");
                throw;
            }
        }
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var categoryItemQuery =
                from ci in db.CategoryItems
                where ci.C_Category == "U_Role"
                select new { CI_ID = ci.CI_ID, CI_Name = ci.CI_Name };
            ViewBag.U_Role = new SelectList(categoryItemQuery, "CI_ID", "CI_Name");
            return View();
        }
        /// <summary>
        /// 真正的新增用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Models.User u)
        {
            string m = "";
            try
            {
                Models.User user =Session["User"] as Models.User;
                u.U_Password = "123456";
                u.S_ID = user.S_ID;
                db.User.Add(u);
                db.SaveChanges();
                m = "新增成功";
            }
            catch (Exception)
            {
                m = "新增失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#UserInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + m + "','info');</script>");
            return Content(js);
        }
        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var u=db.User.Find(id);
            var categoryItemQuery =
                from ci in db.CategoryItems
                where ci.C_Category == "U_Role"
                select new { CI_ID = ci.CI_ID, CI_Name = ci.CI_Name };
            ViewBag.U_Role = new SelectList(categoryItemQuery, "CI_ID", "CI_Name",u.U_Role);
            return View(u);
        }
        /// <summary>
        /// 真正的修改
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Models.User u)
        {
            string m = "";
            try
            {
                db.Entry(u).State = System.Data.EntityState.Modified;
                u.S_ID = (Session["User"] as Models.User).S_ID;
                db.SaveChanges();
                m = "修改成功";
            }
            catch (Exception)
            {
                m = "修改失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#UserInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + m + "','info');</script>");
            return Content(js);
        }
    }
}
