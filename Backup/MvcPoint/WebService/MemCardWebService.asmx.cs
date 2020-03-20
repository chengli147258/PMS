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
    /// MemCardWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class MemCardWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        /// <summary>
        /// //根据卡号/手机号得到单个会员的信息
        /// </summary>
        /// <param name="CardIDOrMobile"></param>
        /// <returns></returns>
        [WebMethod]
        public Models.MemCard GetMemCardByM_CardIDOrM_Mobile(string CardIDOrMobile)
        {
            string sql = "select * from MemCards inner join CardLevels on MemCards.CL_ID=CardLevels.CL_ID where MC_CardID=@MC_CardID or MC_Mobile=@MC_Mobile";
            SqlParameter[] para = { 
                                    new SqlParameter("MC_CardID",CardIDOrMobile),
                                    new SqlParameter("MC_Mobile",CardIDOrMobile)
                                  };
            DataTable dt = DBHelper.ExecuteSelect(sql, para);
            //得到dt里面的第一行
            DataRow dr = dt.Rows[0];
            Models.MemCard mc = dr.ToModel<Models.MemCard>();
            Models.CardLevel cl = dr.ToModel<Models.CardLevel>();
            return mc;
        }
    }
}
