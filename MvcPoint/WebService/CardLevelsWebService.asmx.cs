using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;

namespace MvcPoint.WebService
{
    /// <summary>
    /// CardLevelsWebService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class CardLevelsWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public Models.CardLevel GetCL_NamebyCL_ID(int CL_ID)
        {
            string sql = "select * from CardLevels where CL_ID=@CL_ID";
            SqlParameter[] sp = {
                                    new SqlParameter("CL_ID",CL_ID)
                                };
            DataTable dt = DBHelper.ExecuteSelect(sql, sp);
            DataRow dr = dt.Rows[0];
            Models.CardLevel cl = dr.ToModel<Models.CardLevel>();
            return cl;
        }
    }
}
