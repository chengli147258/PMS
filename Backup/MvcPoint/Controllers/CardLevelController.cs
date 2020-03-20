using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPoint.Models;
using System.Data;
using System.Data.SqlClient;

namespace MvcPoint.Controllers
{
    [Authorize]
    public class CardLevelController : Controller
    {
        //
        // GET: /CardLevel/
        private PointContext db = new PointContext();
        /// <summary>
        /// 会员卡类型的主页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 会员卡类型的查询
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetCardLevelPaged(int rows, int page,string CL_Name="")
        {
            string sql = "P_GetCardLevelsPaged";
            int r=0;
            SqlParameter[] sp = { 
                                    new SqlParameter("PageSize",rows),
                                    new SqlParameter("PageIndex",page),
                                    new SqlParameter("Count",r),
                                    new SqlParameter("LevelName",CL_Name)
                                };
            sp[2].Direction = ParameterDirection.Output;
            DataTable dt = DBHelper.ExecuteSelect(sql, true,sp);
            List<object> ls = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.CardLevel c = dr.ToModel<Models.CardLevel>();
                ls.Add(c);
            }
            var data = new { total = (int)sp[2].Value, rows = ls};
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 新增会员卡类别
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// 实现真正的新增
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Models.CardLevel c)
        {
            string s = "";
            try
            {
                db.CardLevels.Add(c);
                db.SaveChanges();
                s = "新增成功";
            }
            catch (Exception)
            {
                s = "新增失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#cardLevelInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + s + "','info');</script>");
            return Content(js);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            try
            {
                var c = db.CardLevels.Find(id);
                db.CardLevels.Remove(c);
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
        /// 修改
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var c = db.CardLevels.Find(id);
            return View(c);
        }
        /// <summary>
        /// 实现真正的修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Models.CardLevel c)
        {
            string s = "";
            try
            {
                db.Entry(c).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                s = "修改成功";
            }
            catch (Exception)
            {
                s = "修改失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#cardLevelInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + s + "','info');</script>");
            return Content(js);
        }
    }
}
