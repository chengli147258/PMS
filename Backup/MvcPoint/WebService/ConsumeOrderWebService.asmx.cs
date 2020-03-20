using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Data;

namespace MvcPoint.WebService
{
    /// <summary>
    /// ConsumeOrderWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class ConsumeOrderWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        //快速消费
        [WebMethod]
        public bool FastConsumeOrder(Models.ConsumeOrder co)
        {
            bool flag = false;
            string sql1 = "update MemCards set MC_Point=MC_Point+@MC_Point,MC_TotalMoney=MC_TotalMoney+@MC_TotalMoney,MC_TotalCount=MC_TotalCount+1 where MC_ID=@MC_ID";
            SqlParameter[] sp1 = { 
                                        new SqlParameter("MC_Point",co.CO_GavePoint),
                                        new SqlParameter("MC_TotalMoney",co.CO_DiscountMoney),
                                        new SqlParameter("MC_ID",co.MC_ID)
                                    };
            string sql2 = "insert into ConsumeOrders values(@S_ID, @U_ID, @CO_OrderCode, @CO_OrderType, @MC_ID, @MC_CardID, 0, @CO_TotalMoney, @CO_DiscountMoney, @CO_GavePoint,@CO_Recash , @CO_Remark, @CO_CreateTime)";
            SqlParameter[] sp2 = { 
                                    new SqlParameter("S_ID",co.S_ID),
                                    new SqlParameter("U_ID",co.U_ID),
                                    new SqlParameter("CO_OrderCode",DateTime.Now.ToString("yyyymmddhhmmss")),
                                    new SqlParameter("MC_ID",co.MC_ID),
                                    new SqlParameter("MC_CardID",co.MC_CardID),
                                    new SqlParameter("CO_TotalMoney",co.CO_TotalMoney),
                                    new SqlParameter("CO_OrderType",co.CO_OrderType),
                                    new SqlParameter("CO_DiscountMoney",co.CO_DiscountMoney),
                                    new SqlParameter("CO_GavePoint",co.CO_GavePoint),
                                    new SqlParameter("CO_Recash",co.CO_Recash),
                                    new SqlParameter("CO_Remark",co.CO_Remark),
                                    new SqlParameter("CO_CreateTime",DateTime.Now)
                                 };
            if (DBHelper.ExecuteNonQuery(sql1,sp1) && DBHelper.ExecuteNonQuery(sql2,sp2))
            {
                flag = true;
            }
            return flag;
        }
        /// <summary>
        /// 会员减积分
        /// </summary>
        /// <param name="co"></param>
        /// <returns></returns>
        [WebMethod]
        public bool SubstracPoint(Models.ConsumeOrder co)
        {
            bool flag = false;
            string sql1 = "update MemCards set MC_Point=MC_Point-@MC_Point where MC_ID=@MC_ID";
            SqlParameter[] sp1 = { 
                                        new SqlParameter("MC_Point",co.CO_GavePoint),
                                        new SqlParameter("MC_ID",co.MC_ID)
                                    };
            string sql2 = "insert into ConsumeOrders values(@S_ID, @U_ID, @CO_OrderCode, @CO_OrderType, @MC_ID, @MC_CardID, 0, @CO_TotalMoney, @CO_DiscountMoney, @CO_GavePoint,@CO_Recash , @CO_Remark, @CO_CreateTime)";
            SqlParameter[] sp2 = { 
                                    new SqlParameter("S_ID",co.S_ID),
                                    new SqlParameter("U_ID",co.U_ID),
                                    new SqlParameter("CO_OrderCode",DateTime.Now.ToString("yyyymmddhhmmss")),
                                    new SqlParameter("MC_ID",co.MC_ID),
                                    new SqlParameter("MC_CardID",co.MC_CardID),
                                    new SqlParameter("CO_TotalMoney",co.CO_TotalMoney),
                                    new SqlParameter("CO_OrderType",co.CO_OrderType),
                                    new SqlParameter("CO_DiscountMoney",co.CO_DiscountMoney),
                                    new SqlParameter("CO_GavePoint",co.CO_GavePoint),
                                    new SqlParameter("CO_Recash",co.CO_Recash),
                                    new SqlParameter("CO_Remark",co.CO_Remark),
                                    new SqlParameter("CO_CreateTime",DateTime.Now)
                                 };
            if (DBHelper.ExecuteNonQuery(sql1, sp1) && DBHelper.ExecuteNonQuery(sql2, sp2))
            {
                flag = true;
            }
            return flag;
        }
    }
}
