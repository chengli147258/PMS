using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Security;
using System.Data.SqlClient;
using System.Data;

namespace MvcPoint.WebService
{
    /// <summary>
    /// UserLogin 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class UserLogin : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        [WebMethod]
        public Models.User Login(string LoginName,string Password)
        {
            string sql = "select * from Users  where U_LoginName=@U_LoginName and U_Password=@U_Password";
            SqlParameter[] para = { 
                                    new SqlParameter("U_LoginName",LoginName),
                                    new SqlParameter("U_Password",Password)
                                  };
            DataTable dt = DBHelper.ExecuteSelect(sql, para);
            //得到dt里面的第一行
            DataRow dr = dt.Rows[0];
            Models.User u = dr.ToModel<Models.User>();
            return u;
        }
    }
}
