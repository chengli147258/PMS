using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using MvcPoint.Models;
using System.Web.Security;


namespace MvcPoint.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        private PointContext db = new PointContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="LoginName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public ActionResult Index(Models.User u)
        {
            var userQuery =
               from u2 in db.User
               where u2.U_LoginName == u.U_LoginName && u2.U_Password == u.U_Password
               select u2;
            if (userQuery.Count() > 0)
            {
                //表示用户名和密码正确
                Models.User user = userQuery.FirstOrDefault();
                //把单个用户对象存储到Session里面去
                Session["User"] = user;
                FormsAuthentication.SetAuthCookie(user.U_LoginName, false);//实现Forms身份验证
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                //表示用户名或密码有误
                ModelState.AddModelError("", "用户名或密码有误！");
                return View();//继续显示当前视图
            }
        }
        /// <summary>
        /// 退出系统
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginOff()
        {
            //清空Session里面的数据
            Session.Clear();
            //清空身份验证票据
            FormsAuthentication.SignOut();
            //实现页面跳转
            return RedirectToAction("Index");
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ExchangPassword()
        {
            return View();
        }
        /// <summary>
        /// 修改个人资料
        /// </summary>
        /// <returns></returns>
        public ActionResult ExchangeMessage()
        {
            Models.User user= Session["User"] as Models.User;
            int id = user.U_ID;
            var u = db.User.Find(id);
            return View(u);
        }
        /// <summary>
        /// 个人资料的真正修改
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExchangeMessage(Models.User u)
        {
            string m = "";
            try
            {
                Models.User user= Session["User"] as Models.User;
                db.Entry(u).State = System.Data.EntityState.Modified;
                u.S_ID = user.S_ID;
                u.U_Password = user.U_Password;
                u.U_Role = user.U_Role;
                u.U_CanDelete = user.U_CanDelete;
                db.SaveChanges();
                m = "修改成功";
            }
            catch (Exception)
            {
                m = "修改失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#UserInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + m + "','info');</script>");
            return Content(js);
        }
        /// <summary>
        /// 修改个人密码真正的修改
        /// </summary>
        /// <returns></returns>
        public ActionResult TrueExchangPassword(string OldPwd,string NewPwd)
        {
            Models.User user=(Models.User)Session["User"];
            if (OldPwd!=user.U_Password)
            {
                return Content("1");
            }
            else
            {
                string sql = "update Users set U_Password=@U_Password where U_ID=@U_ID";
                SqlParameter[] sp = { 
                                    new SqlParameter("U_Password",NewPwd),
                                    new SqlParameter("U_ID",user.U_ID)
                                };
                if (DBHelper.ExecuteNonQuery(sql,sp))
                {
                    return Content("2");
                }
                else
                {
                    return Content("3");
                }
            }
        }
    }
}
