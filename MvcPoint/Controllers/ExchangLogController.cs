using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using MvcPoint.Models;
using System.Web.Script.Serialization;
using System.Transactions;

namespace MvcPoint.Controllers
{
    [Authorize]
    public class ExchangLogController : Controller
    {
        //
        // GET: /ExchangLog/
        private PointContext db = new PointContext();
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 礼品兑换
        /// </summary>
        /// <param name="ExchangLogs"></param>
        /// <param name="CardOrMobile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string ExchangLogs, string CardOrMobile)
        {
            string m = "";
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //进行JSON的反序列化
            List<Models.ExchangLog> list = jss.Deserialize<List<Models.ExchangLog>>(ExchangLogs);
            try
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //根据卡号/手机得到单个会员对象
                    string sql = "select * from MemCards inner join CardLevels on MemCards.CL_ID=CardLevels.CL_ID where MC_CardID=@MC_CardID or MC_Mobile=@MC_Mobile";
                    SqlParameter[] para = { 
                                    new SqlParameter("MC_CardID",CardOrMobile),
                                    new SqlParameter("MC_Mobile",CardOrMobile)
                                  };
                    DataTable dt = DBHelper.ExecuteSelect(sql, para);
                    //得到dt里面的第一行
                    DataRow dr = dt.Rows[0];
                    Models.MemCard mc = dr.ToModel<Models.MemCard>();
                    Models.User u = (Models.User)Session["User"];
                    //循环遍历list
                    int totalPoint = 0;//这个变量专门用来存储本次兑换消费的总积分
                    foreach (Models.ExchangLog el in list)
                    {
                        el.EL_CreateTime = DateTime.Now;
                        el.MC_CardID = mc.MC_CardID;
                        el.MC_ID = mc.MC_ID;
                        el.MC_Name = mc.MC_Name;
                        el.S_ID = u.S_ID;
                        el.U_ID = u.U_ID;
                        db.ExchangLogs.Add(el);
                        totalPoint += el.EL_Point;
                        string sql3 = "update ExchangGifts set EG_ExchangNum=EG_ExchangNum+@EG_ExchangNum where EG_ID=@EG_ID";
                        SqlParameter[] para3 = { 
                                        new SqlParameter("EG_ExchangNum",el.EL_Number),
                                        new SqlParameter("EG_ID",el.EG_ID)
                                       };
                        DBHelper.ExecuteNonQuery(sql3, para3);
                    }
                    if (mc.MC_Point > totalPoint)
                    {
                        db.SaveChanges();
                        string sql2 = "update MemCards set MC_Point=MC_Point-@TotalPoint where MC_ID=@MC_ID";
                        SqlParameter[] para2 = { 
                                    new SqlParameter("TotalPoint",totalPoint),
                                    new SqlParameter("MC_ID",mc.MC_ID)
                                   };
                        DBHelper.ExecuteNonQuery(sql2, para2);
                        m = "兑换成功";
                        ts.Complete();
                    }
                    else
                    {                        
                        m = "当前用户积分不足！";
                    }
                }
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
        /// 查询会员的兑换记录
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectExchangeLogByMemCardIDOrMobile()
        {
            return View();
        }
        public ActionResult SelectExchangeLogByMemCardIDOrMobileTrue(int rows, int page, string MC_CardID, string MC_Mobile)
        {
            string sql = "P_GetExchangLogPaged";
            int r = 0;
            SqlParameter[] sp = { 
                                    new SqlParameter("PageSize",rows),
                                    new SqlParameter("PageIndex",page),
                                    new SqlParameter("Count",r),
                                    new SqlParameter("MC_CardID",MC_CardID),
                                    new SqlParameter("MC_Mobile",MC_Mobile)
                                 };
            sp[2].Direction = ParameterDirection.Output;
            DataTable dt = DBHelper.ExecuteSelect(sql, true, sp);
            List<Models.ExchangLog> ls = new List<Models.ExchangLog>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.ExchangLog cl = dr.ToModel<Models.ExchangLog>();
                ls.Add(cl);
            }
            var data = new { total = sp[2].Value, rows = ls };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult P_GetPagedMemCardAndGift(int rows, int page, string MC_IDOrName = "", string BeginTime = "2010-1-1", string EndTime = "2030-1-1")
        {
            Models.User u2 = (Models.User)Session["User"];
            if (BeginTime == "" && EndTime == "")
            {
                BeginTime = "2010-1-1";
                EndTime = "2030-1-1";
            }
            //调用存储过程[P_GetPagedMemCardsByCondition]
            string sql = "P_GetPagedMemCardAndGift";
            //定义变量接收输出参数
            int r = 0;
            SqlParameter[] sp = { 
                                    new SqlParameter("pageSize",rows),
                                    new SqlParameter("currentPageIndex",page),
                                    new SqlParameter("recordCount",r),
                                    new SqlParameter("MC_IDOrName",MC_IDOrName),
                                    new SqlParameter("BeginTime",BeginTime),
                                    new SqlParameter("EndTime",EndTime),
                                    new SqlParameter("S_ID",u2.S_ID)
                                };
            //指定第二个参数为输出参数
            sp[2].Direction = ParameterDirection.Output;
            //调用查询的方法,得到数据
            DataTable dt = DBHelper.ExecuteSelect(sql, true, sp);
            //实例化一个泛型集合
            List<object> list = new List<object>();
            //循环遍历dt
            foreach (DataRow dr in dt.Rows)
            {
                //实例化ExchangLog
                Models.ExchangLog el = dr.ToModel<Models.ExchangLog>();
                Models.MemCard m = dr.ToModel<Models.MemCard>();
                //新增到list
                list.Add(new { ExchangLog = el, MemCard = m });
            }
            var p = new { total = (int)sp[2].Value, rows = list };
            return Json(p, JsonRequestBehavior.AllowGet);
        }
    }
}
