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
    public class ExchangGiftController : Controller
    {
        //
        // GET: /ExchangGift/
        private PointContext db = new PointContext();
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 查询所有礼品信息
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetExchangGiftPaged(int rows, int page)
        {
            Models.User u2 = (Models.User)Session["User"];
            string sql = "P_GetExchangGiftPaged";
            int r = 0;
            SqlParameter[] sp = { 
                                    new SqlParameter("PageSize",rows),
                                    new SqlParameter("PageIndex",page),
                                    new SqlParameter("Count",r),
                                    new SqlParameter("S_ID",u2.S_ID)
                                };
            sp[2].Direction = ParameterDirection.Output;
            DataTable dt = DBHelper.ExecuteSelect(sql, true, sp);
            List<object> ls = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.Shop s = dr.ToModel<Models.Shop>();
                Models.ExchangGift e = dr.ToModel<Models.ExchangGift>();
                ls.Add(new { Shop = s, ExchangGift = e });
            }
            var data = new { total = sp[2].Value, rows = ls };
            return Json(data, JsonRequestBehavior.AllowGet);
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
                var ex = db.ExchangGift.Find(id);
                db.ExchangGift.Remove(ex);
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
            return View();
        }
        /// <summary>
        /// 真正的新增
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Models.ExchangGift ex)
        {
            string s = "";
            try
            {
                Models.User user = (Models.User)Session["User"];
                ex.S_ID = user.S_ID;
                db.ExchangGift.Add(ex);
                db.SaveChanges();
                s = "新增成功";
            }
            catch (Exception)
            {
                s = "新增失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#ExchangGiftInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + s + "','info');</script>");
            return Content(js);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var ex = db.ExchangGift.Find(id);
            return View(ex);
        }

        [HttpPost]
        public ActionResult Edit(Models.ExchangGift ex)
        {
            string s = "";
            try
            {
                ex.S_ID = (Session["User"] as Models.User).S_ID;
                db.Entry(ex).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                s = "修改成功";
            }
            catch (Exception)
            {
                s = "修改失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#ExchangGiftInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + s + "','info');</script>");
            return Content(js);
        }
        /// <summary>
        /// 礼品兑换统计界面
        /// </summary>
        /// <returns></returns>
        public ActionResult GiftExchangCount()
        {
            return View();
        }
    }
}
