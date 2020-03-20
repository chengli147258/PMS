using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPoint.Models;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;
using System.Transactions;

namespace MvcPoint.Controllers
{
    [Authorize]
    public class ConsumeOrderController : Controller
    {
        //
        // GET: /ConsumeOrder/
        private PointContext db = new PointContext();
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 快速消费
        /// </summary>
        /// <returns></returns>
        public ActionResult FastConsume()
        {
            sssssss.MyYunSuanService f = new sssssss.MyYunSuanService();

            return View();
        }
        /// <summary>
        /// 会员减积分
        /// </summary>
        /// <returns></returns>
        public ActionResult SubstracPoint()
        {
            return View();
        }
        /// <summary>
        /// 真正的快速消费
        /// </summary>
        /// <param name="ConsumeOrder"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FastConsume(string ConsumeOrder)
        {
            string m = "";
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    Models.ConsumeOrder co = jss.Deserialize<Models.ConsumeOrder>(ConsumeOrder);
                    Models.User user = (Models.User)Session["User"];
                    co.CO_CreateTime = DateTime.Now;
                    co.CO_OrderCode = DateTime.Now.ToString("yyyymmddhhmmss");
                    co.CO_OrderType = 5;
                    co.CO_Recash = 0;
                    co.CO_Remark = "快速消费";
                    co.EG_ID = 0;
                    co.S_ID = user.S_ID;
                    co.U_ID = user.U_ID;
                    db.ConsumeOrder.Add(co);
                    string sql = "update MemCards set MC_Point=MC_Point+@MC_Point,MC_TotalMoney=MC_TotalMoney+@MC_TotalMoney,MC_TotalCount=MC_TotalCount+1 where MC_ID=@MC_ID";
                    SqlParameter[] sp = { 
                                        new SqlParameter("MC_Point",co.CO_GavePoint),
                                        new SqlParameter("MC_TotalMoney",co.CO_DiscountMoney),
                                        new SqlParameter("MC_ID",co.MC_ID)
                                    };
                    DBHelper.ExecuteNonQuery(sql, sp);
                    db.SaveChanges();
                    ts.Complete();
                    m = "消费成功";
                }
            }
            catch (Exception)
            {
                m = "消费失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$.messager.alert('温馨提示','" + m + "','info');location=location;</script>");
            return Content(js);
        }
        /// <summary>
        /// 真正的会员减积分
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubstracPoint(string SubstracPoint)
        {
            string m = "";
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    JavaScriptSerializer jss = new JavaScriptSerializer();
                    Models.ConsumeOrder co = jss.Deserialize<Models.ConsumeOrder>(SubstracPoint);
                    Models.User user = (Models.User)Session["User"];
                    co.CO_CreateTime = DateTime.Now;
                    co.CO_OrderCode = DateTime.Now.ToString("yyyymmddhhmmss");
                    co.CO_OrderType = 3;
                    co.CO_Recash = 0;
                    co.EG_ID = 0;
                    co.S_ID = user.S_ID;
                    co.U_ID = user.U_ID;
                    co.CO_DiscountMoney = 0;
                    co.CO_TotalMoney = 0;
                    db.ConsumeOrder.Add(co);
                    string sql = "update MemCards set MC_Point=MC_Point-@MC_Point where MC_ID=@MC_ID";
                    SqlParameter[] sp = { 
                                        new SqlParameter("MC_Point",co.CO_GavePoint),
                                        new SqlParameter("MC_ID",co.MC_ID)
                                    };
                    DBHelper.ExecuteNonQuery(sql, sp);
                    db.SaveChanges();
                    ts.Complete();
                    m = "扣除成功";
                }
            }
            catch (Exception)
            {
                m = "扣除失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$.messager.alert('温馨提示','" + m + "','info');location=location;</script>");
            return Content(js);
        }
        /// <summary>
        /// 查询用户消费记录
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectConsumeOrderInfo()
        {
            return View();
        }
        /// <summary>
        /// 真正的查询用户消费记录
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectConsumeOrderInfoTure(int rows, int page, string MC_CardID, string MC_Mobile, string BeginDate, string EndDate, int CO_OrderType)
        {
            Models.User u2 = (Models.User)Session["User"];
            string sql = "P_GetConsumeOrderPaged";
            int r = 0;
            SqlParameter[] sp = { 
                                    new SqlParameter("PageSize",rows),
                                    new SqlParameter("PageIndex",page),
                                    new SqlParameter("Count",r),
                                    new SqlParameter("MC_CradID",MC_CardID),
                                    new SqlParameter("MC_Mobile",MC_Mobile),
                                    new SqlParameter("BeginDate",BeginDate),
                                    new SqlParameter("EndDate",EndDate),
                                    new SqlParameter("CO_OrderType",CO_OrderType),
                                    new SqlParameter("S_ID",u2.S_ID)
                                 };
            sp[2].Direction = ParameterDirection.Output;
            DataTable dt = DBHelper.ExecuteSelect(sql, true, sp);
            List<object> ls = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.ConsumeOrder co = dr.ToModel<Models.ConsumeOrder>();
                Models.MemCard m = dr.ToModel<Models.MemCard>();
                Models.CategoryItem ci = dr.ToModel<Models.CategoryItem>();
                ls.Add(new { ConsumOrder = co, MemCard = m, CategoryItem = ci });
            }
            var data = new { total = sp[2].Value, rows = ls };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 积分返现
        /// </summary>
        /// <returns></returns>
        public ActionResult PointExchangMoney()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PointExchangMoney(string ConsumeOrder)
        {
            string m = "";
            try
            {
                Models.User user = (Models.User)Session["User"];
                JavaScriptSerializer jss = new JavaScriptSerializer();
                Models.ConsumeOrder co = jss.Deserialize<Models.ConsumeOrder>(ConsumeOrder);
                co.CO_CreateTime = DateTime.Now;
                co.CO_DiscountMoney = 0;
                co.CO_OrderCode = DateTime.Now.ToString("yyyymmddhhmmss");
                co.CO_OrderType = 2;
                co.CO_Remark = "";
                co.CO_TotalMoney = 0;
                co.EG_ID = 0;
                co.S_ID = user.S_ID;
                co.U_ID = user.U_ID;
                db.ConsumeOrder.Add(co);
                db.SaveChanges();
                m = "兑换成功";
            }
            catch (Exception)
            {
                m = "兑换失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$.messager.alert('温馨提示','" + m + "','info');location=location;</script>");
            return Content(js);
        }
        /// <summary>
        /// 快速消费统计
        /// </summary>
        /// <returns></returns>
        public ActionResult FastConsumeStatistic()
        {
            ViewBag.CL_ID = new SelectList(db.CardLevels, "CL_ID", "CL_LevelName");
            return View();
        }
        /// <summary>
        /// 快速消费的查询
        /// </summary>
        /// <param name="rows">每页显示的条数</param>
        /// <param name="page">页码</param>
        /// <param name="BeginDate">起始时间</param>
        /// <param name="EndDate">结束时间</param>
        /// <param name="CardID">会员卡号/手机号</param>
        /// <param name="CL_ID">会员等级编号</param>
        /// <param name="Operation">运算符</param>
        /// <param name="CO_DiscountMoney">金额</param>
        /// <param name="CO_OrderCode">订单号</param>
        /// <returns></returns>
        public ActionResult SelectFastConsumeStatistic(int rows, int page, string BeginDate="", string EndDate = "", string CardID = "", string CL_ID = "", string Operation = ">=", string CO_DiscountMoney = "0", string CO_OrderCode = "")
        {
            Models.User u2 = (Models.User)Session["User"];
            string sql = "P_GetFastConsumPaged";
            int r = 0;
            SqlParameter[] sp = { 
                                    new SqlParameter("PageSize",rows),
                                    new SqlParameter("PageIndex",page),
                                    new SqlParameter("Count",r),
                                    new SqlParameter("BeginDate",BeginDate),
                                    new SqlParameter("EndDate",EndDate),
                                    new SqlParameter("CardID",CardID),
                                    new SqlParameter("CL_ID",CL_ID),
                                    new SqlParameter("Operation",Operation),
                                    new SqlParameter("CO_DiscountMoney",CO_DiscountMoney),
                                    new SqlParameter("CO_OrderCode",CO_OrderCode),
                                    new SqlParameter("S_ID",u2.S_ID)
                                };
            sp[2].Direction = ParameterDirection.Output;
            DataTable dt = DBHelper.ExecuteSelect(sql, true, sp);
            List<object> ls = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.CardLevel cl = dr.ToModel<Models.CardLevel>();
                Models.ConsumeOrder co = dr.ToModel<Models.ConsumeOrder>();
                Models.MemCard mc = dr.ToModel<Models.MemCard>();
                ls.Add(new { CardLevel = cl, ConsumeOrder = co, MemCard = mc });
            }
            var data = new { total = sp[2].Value, rows = ls };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 查询会员的消费情况
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectMemConsumeStatistic()
        {
            var categoryItemQuery =
                from ci in db.CategoryItems
                where ci.C_Category == "CO_OrderType"
                select new {CI_ID=ci.CI_ID,CI_Name=ci.CI_Name };
            ViewBag.CO_OrderType = new SelectList(categoryItemQuery, "CI_ID", "CI_Name");
            return View();
        }
        /// <summary>
        /// 会员减积分统计
        /// </summary>
        /// <returns></returns>
        public ActionResult MemSubstracPoint()
        {
            ViewBag.CL_ID = new SelectList(db.CardLevels, "CL_ID", "CL_LevelName");
            return View();
        }
        /// <summary>
        /// 真正的会员减积分统计      
        public ActionResult SelectMemSubstracPoint(int rows, int page, string BeginDate = "", string EndDate = "", string CardID = "", string CL_ID = "", string Operation = ">=", string CO_GavePoint = "0", string CO_OrderCode = "")
        {
            Models.User u2 = (Models.User)Session["User"];
            string sql = "P_GetExchangePointPaged";
            int r = 0;
            SqlParameter[] sp = { 
                                    new SqlParameter("PageSize",rows),
                                    new SqlParameter("PageIndex",page),
                                    new SqlParameter("Count",r),
                                    new SqlParameter("BeginDate",BeginDate),
                                    new SqlParameter("EndDate",EndDate),
                                    new SqlParameter("CardID",CardID),
                                    new SqlParameter("CL_ID",CL_ID),
                                    new SqlParameter("Operation",Operation),
                                    new SqlParameter("CO_GavePoint",CO_GavePoint),
                                    new SqlParameter("CO_OrderCode",CO_OrderCode),
                                    new SqlParameter("S_ID",u2.S_ID)
                                };
            sp[2].Direction = ParameterDirection.Output;
            DataTable dt = DBHelper.ExecuteSelect(sql, true, sp);
            List<object> ls = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.CardLevel cl = dr.ToModel<Models.CardLevel>();
                Models.ConsumeOrder co = dr.ToModel<Models.ConsumeOrder>();
                Models.MemCard mc = dr.ToModel<Models.MemCard>();
                ls.Add(new { CardLevel = cl, ConsumeOrder = co, MemCard = mc });
            }
            var data = new { total = sp[2].Value, rows = ls };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///积分返现统计
        /// </summary>
        /// <returns></returns>
        public ActionResult MemExchangePointToMoney()
        {
            ViewBag.CL_ID = new SelectList(db.CardLevels, "CL_ID", "CL_LevelName");
            return View();
        }
        /// <summary>
        /// 真正的会员积分返现      
        public ActionResult SelectMemExchangePointToMoney(int rows, int page, string BeginDate = "", string EndDate = "", string CardID = "", string CL_ID = "", string Operation = ">=", string CO_Recash = "0", string CO_OrderCode = "")
        {
            Models.User u2 = (Models.User)Session["User"];
            string sql = "P_GetExchangePointToMoneyPaged";
            int r = 0;
            SqlParameter[] sp = { 
                                    new SqlParameter("PageSize",rows),
                                    new SqlParameter("PageIndex",page),
                                    new SqlParameter("Count",r),
                                    new SqlParameter("BeginDate",BeginDate),
                                    new SqlParameter("EndDate",EndDate),
                                    new SqlParameter("CardID",CardID),
                                    new SqlParameter("CL_ID",CL_ID),
                                    new SqlParameter("Operation",Operation),
                                    new SqlParameter("CO_Recash",CO_Recash),
                                    new SqlParameter("CO_OrderCode",CO_OrderCode),
                                    new SqlParameter("S_ID",u2.S_ID)
                                };
            sp[2].Direction = ParameterDirection.Output;
            DataTable dt = DBHelper.ExecuteSelect(sql, true, sp);
            List<object> ls = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.CardLevel cl = dr.ToModel<Models.CardLevel>();
                Models.ConsumeOrder co = dr.ToModel<Models.ConsumeOrder>();
                Models.MemCard mc = dr.ToModel<Models.MemCard>();
                ls.Add(new { CardLevel = cl, ConsumeOrder = co, MemCard = mc });
            }
            var data = new { total = sp[2].Value, rows = ls };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
