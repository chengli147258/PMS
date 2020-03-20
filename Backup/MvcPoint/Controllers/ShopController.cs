using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using MvcPoint.Models;
using System.Transactions;
namespace MvcPoint.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        //
        // GET: /Shop/
        private PointContext db = new PointContext();
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 查询所有店铺信息
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="S_Name"></param>
        /// <param name="S_Address"></param>
        /// <returns></returns>
        public ActionResult GetShopPaged(int rows, int page, string S_Name = "", string S_ContactName = "", string S_Address = "")
        {
            string sql = "P_GetShopPaged";
            int r = 0;
            SqlParameter[] sp = { 
                                    new SqlParameter("PageSize",rows),
                                    new SqlParameter("PageIndex",page),
                                    new SqlParameter("Count",r),
                                    new SqlParameter("S_Name",S_Name),
                                    new SqlParameter("S_ContactName",S_ContactName),
                                    new SqlParameter("S_Address",S_Address)
                                };
            sp[2].Direction = ParameterDirection.Output;
            DataTable dt = DBHelper.ExecuteSelect(sql, true, sp);
            List<object> ls = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.Shop s = dr.ToModel<Models.Shop>();
                Models.CategoryItem c = dr.ToModel<Models.CategoryItem>();
                ls.Add(new { Shop = s, CategoryItem = c });
            }
            var data = new { total = sp[2].Value, rows = ls };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 删除店铺信息
        /// </summary>
        /// <param name="S_ID"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            try
            {
                var s = db.Shop.Find(id);
                db.Shop.Remove(s);
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
        /// 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            //string sql = "select CI_ID,CI_Name from CategoryItems where C_Category='S_Category'";
            //DataTable dt = DBHelper.ExecuteSelect(sql);
            //List<Models.CategoryItem> ls = new List<CategoryItem>();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    Models.CategoryItem c = dr.ToModel<Models.CategoryItem>();
            //    ls.Add(c);
            //}
            //ViewBag.S_Category = new SelectList(ls, "CI_ID", "CI_Name");
            var categoryItemQuery =
                from ci in db.CategoryItems
                where ci.C_Category == "S_Category"
                select new {CI_ID=ci.CI_ID,CI_Name=ci.CI_Name };
            ViewBag.S_Category = new SelectList(categoryItemQuery, "CI_ID", "CI_Name");
            return View();
            
        }
        /// <summary>
        /// 真正的新增
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Models.Shop s)
        {
            string m = "";
            try
            {
                s.S_CreateTime = DateTime.Now;
                s.S_IsHasSetAdmin = false;
                db.Shop.Add(s);
                db.SaveChanges();
                m = "新增成功";
            }
            catch (Exception)
            {
                m = "新增失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#ShopInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + m + "','info');</script>");
            return Content(js);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="S_ID"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var s = db.Shop.Find(id);
            string sql = "select CI_ID,CI_Name from CategoryItems where C_Category='S_Category'";
            DataTable dt = DBHelper.ExecuteSelect(sql);
            List<Models.CategoryItem> ls = new List<CategoryItem>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.CategoryItem c = dr.ToModel<Models.CategoryItem>();
                ls.Add(c);
            }
            ViewBag.S_Category = new SelectList(ls, "CI_ID", "CI_Name", s.S_Category);
            return View(s);
        }
        /// <summary>
        /// 真正的修改
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Models.Shop s)
        {
            string m = "";
            try
            {
                db.Entry(s).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                m = "修改成功";
            }
            catch (Exception)
            {
                m = "修改失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#ShopInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + m + "','info');</script>");
            return Content(js);
        }
        /// <summary>
        /// 分配管理员
        /// </summary>
        /// <returns></returns>
        public ActionResult AllotAdmin(int id)
        {
            ViewBag.S_ID = id;
            return View();
        }
        [HttpPost]
        public ActionResult AllotAdmin(Models.User u)
        {
            string s = "";
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    string sql = "update Shops set S_IsHasSetAdmin=1 where S_ID=@S_ID";
                    SqlParameter[] para = { 
                                        new SqlParameter("S_ID",u.S_ID)
                                      };
                    DBHelper.ExecuteNonQuery(sql, para);
                    u.U_Password = "123456";
                    u.U_Role = 2;
                    db.User.Add(u);
                    db.SaveChanges();
                    ts.Complete();
                }
                s = "分配成功！";
            }
            catch (Exception)
            {
                s = "分配失败！";
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#ShopInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + s + "','info');</script>");
            return Content(js);
        }
    }
}
