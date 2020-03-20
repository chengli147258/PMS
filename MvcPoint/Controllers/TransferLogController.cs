using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPoint.Models;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;

namespace MvcPoint.Controllers
{
    [Authorize]
    public class TransferLogController : Controller
    {
        //
        // GET: /TransferLog/
        private PointContext db = new PointContext();
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 新增转账记录
        /// </summary>
        /// <returns></returns>
        public ActionResult Create(int id)
        {
            Models.MemCard card = db.MemCards.Find(id);
            Session["MemCard"] = card;
            return View();
        }
        /// <summary>
        /// 实现转账记录表的新增
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Models.TransferLog t)
        {
            string m = "";
            Models.MemCard card = (Models.MemCard)Session["MemCard"];
            int point = int.Parse(t.TL_TransferMoney.ToString());
            int FromID = t.TL_FromMC_ID;
            int ToID = t.TL_ToMC_ID;
            if (t.TL_TransferMoney > card.MC_Point)
            {
                m = "账户余额不足！";//当前错误为余额不足
            }
            else
            {
                try
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        string sqlOne = "update MemCards set MC_Point=MC_Point+@Point where MC_ID=@MC_ID";
                        SqlParameter[] spOne = { 
                                            new SqlParameter("Point",point),
                                            new SqlParameter("MC_ID",ToID)
                                       };
                        string sqlTwo = "update MemCards set MC_Point=MC_Point-@Point where MC_ID=@MC_ID";
                        SqlParameter[] spTwo = { 
                                            new SqlParameter("Point",point),
                                            new SqlParameter("MC_ID",FromID)
                                       };
                        if (DBHelper.ExecuteNonQuery(sqlOne, spOne) && DBHelper.ExecuteNonQuery(sqlTwo, spTwo))
                        {
                            Models.User user = (Models.User)Session["User"];
                            t.TL_CreateTime = DateTime.Now;
                            t.S_ID = user.S_ID;
                            t.U_ID = user.U_ID;
                            db.TransferLog.Add(t);
                            db.SaveChanges();
                            m = "转账成功";
                            ts.Complete();
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    m = "转账失败";
                }
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#memCardInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + m + "','info');</script>");
            return Content(js);
        }
        /// <summary>
        /// 通过输入的会员卡号获取会员信息
        /// </summary>
        /// <param name="MC_CardID"></param>
        /// <returns></returns>
        public ActionResult GetNamebyId(string MC_CardID)
        {
            var cardQuery =
               from card in db.MemCards
               where card.MC_CardID == MC_CardID
               select card;
            if (cardQuery.Count() > 0)
            {
                //表示用户名和密码正确
                Models.MemCard card = cardQuery.FirstOrDefault();
                return Json(card, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Content("0");//继续显示当前视图
            }
        }
    }
}
