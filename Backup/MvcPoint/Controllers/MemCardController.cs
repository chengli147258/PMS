using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcPoint.Models;
using System.Data;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Data;
using System.IO;
using System.Data.Entity;
namespace MvcPoint.Controllers
{
    [Authorize]
    public class MemCardController : Controller
    {
        //
        // GET: /Point/
        private PointContext db = new PointContext();
        /// <summary>
        /// 会员信息的主页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            string sql = "select CI_ID,CI_Name from CategoryItems where C_Category='MC_State'";
            DataTable dt = DBHelper.ExecuteSelect(sql);
            List<Models.CategoryItem> ls = new List<CategoryItem>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.CategoryItem c = dr.ToModel<Models.CategoryItem>();
                ls.Add(c);
            }
            ViewBag.CI_ID = new SelectList(ls, "CI_ID", "CI_Name");
            ViewBag.CL_ID = new SelectList(db.CardLevels, "CL_ID", "CL_LevelName");
            return View();
        }
        /// <summary>
        /// 会员信息查询
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetMemCardPaged(int rows, int page, string MC_CardID = "", string MC_Mobile = "", string MC_Name = "", int CL_ID = 0, int CI_ID = 0)
        {
            Models.User u2 = (Models.User)Session["User"];
            string sql = "P_GetMemCardPaged";
            int r = 0;
            SqlParameter[] sp = {
                                    new SqlParameter("PageSize",rows),
                                    new SqlParameter("PageIndex",page),
                                    new SqlParameter("Count",r),
                                    new SqlParameter("MC_CardID",MC_CardID),
                                    new SqlParameter("MC_Name",MC_Name),
                                    new SqlParameter("MC_Mobile",MC_Mobile),
                                    new SqlParameter("CL_ID",CL_ID),
                                    new SqlParameter("CI_ID",CI_ID),
                                    new SqlParameter("S_ID",u2.S_ID)
                                };
            sp[2].Direction = ParameterDirection.Output;
            DataTable dt = DBHelper.ExecuteSelect(sql, true, sp);
            List<object> ls = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.CardLevel c = dr.ToModel<Models.CardLevel>();
                Models.MemCard m = dr.ToModel<Models.MemCard>();
                Models.CategoryItem ci = dr.ToModel<Models.CategoryItem>();
                ls.Add(new { CardLevel = c, MemCard = m, CategoryItem = ci });
            }
            var p = new { total = sp[2].Value, rows = ls };
            return Json(p, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 创建会员信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            string sql = "select CI_ID,CI_Name from CategoryItems where C_Category='MC_State'";
            DataTable dt = DBHelper.ExecuteSelect(sql);
            List<Models.CategoryItem> ls = new List<CategoryItem>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.CategoryItem c = dr.ToModel<Models.CategoryItem>();
                ls.Add(c);
            }
            ViewBag.MC_State = new SelectList(ls, "CI_ID", "CI_Name");
            ViewBag.CL_ID = new SelectList(db.CardLevels, "CL_ID", "CL_LevelName");
            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem { Text = "未知", Value = "2" });
            item.Add(new SelectListItem { Text = "男", Value = "1" });
            item.Add(new SelectListItem { Text = "女", Value = "0" });
            ViewBag.MC_Sex = item;
            return View();
        }
        /// <summary>
        /// 查询会员姓名
        /// </summary>
        /// <param name="MC_CardID"></param>
        /// <returns></returns>
        public ActionResult GetNamebyId(string MC_CardID)
        {
            string sql = "select MC_Name from MemCards where MC_CardID=@MC_CardID";
            SqlParameter[] sp = { 
                                    new SqlParameter("MC_CardID",MC_CardID)
                                };
            DataTable dt = DBHelper.ExecuteSelect(sql, sp);
            string name = dt.Rows[0][0].ToString();
            return Content(name);
        }
        /// <summary>
        /// 实现真正的新增
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(Models.MemCard m)
        {
            string s = "";
            try
            {
                Models.User user = (Models.User)Session["User"];
                m.S_ID = user.S_ID;
                m.MC_CreateTime = DateTime.Parse(DateTime.Now.ToLongDateString());
                db.MemCards.Add(m);
                db.SaveChanges();
                s = "新增成功";
            }
            catch (Exception)
            {
                s = "新增失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#memCardInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + s + "','info');</script>");
            return Content(js);
        }
        /// <summary>
        /// 删除会员信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult Delete(int ID)
        {
            try
            {
                var MemCard = db.MemCards.Find(ID);
                db.MemCards.Remove(MemCard);
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
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var m = db.MemCards.Find(id);
            Session["MemCard"] = m;
            string sql = "select CI_ID,CI_Name from CategoryItems where C_Category='MC_State'";
            DataTable dt = DBHelper.ExecuteSelect(sql);
            List<Models.CategoryItem> ls = new List<CategoryItem>();
            foreach (DataRow dr in dt.Rows)
            {
                Models.CategoryItem c = dr.ToModel<Models.CategoryItem>();
                ls.Add(c);
            }
            ViewBag.MC_State = new SelectList(ls, "CI_ID", "CI_Name", m.MC_State);
            ViewBag.CL_ID = new SelectList(db.CardLevels, "CL_ID", "CL_LevelName", m.CL_ID);
            List<SelectListItem> item = new List<SelectListItem>();
            item.Add(new SelectListItem { Text = "未知", Value = "2" });
            item.Add(new SelectListItem { Text = "男", Value = "1" });
            item.Add(new SelectListItem { Text = "女", Value = "0" });
            ViewBag.MC_Sex = new SelectList(item, "Value", "Text", m.MC_Sex);
            return View(m);
        }
        /// <summary>
        /// 实现真正的修改
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Models.MemCard m)
        {
            string s = "";
            try
            {
                Models.MemCard sm = (Models.MemCard)Session["MemCard"];
                m.S_ID = sm.S_ID;
                m.MC_CreateTime = sm.MC_CreateTime;
                db.Entry(m).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                s = "修改成功";
            }
            catch (Exception)
            {
                s = "修改失败";
                throw;
            }
            string js = string.Format("<script>window.parent.$('#dlg').dialog('close');window.parent.$('#memCardInfo').datagrid('reload');window.parent.$.messager.alert('温馨提示','" + s + "','info');</script>");
            return Content(js);
        }
        /// <summary>
        /// 修改会员卡状态
        /// </summary>
        /// <returns></returns>
        public ActionResult EditMCState(int id)
        {
            var m = db.MemCards.Find(id);
            var categoryItemQuery =
                from ci in db.CategoryItems
                where ci.C_Category == "MC_State"
                select new { CI_ID = ci.CI_ID, CI_Name = ci.CI_Name };
            ViewBag.MC_State = new SelectList(categoryItemQuery, "CI_ID", "CI_Name", m.MC_State);
            return View(m);
        }
        /// <summary>
        /// 修改会员卡状态
        /// </summary>
        /// <returns></returns>
        public ActionResult EditMCStateTrue(string id, int state)
        {
            string sql = "update MemCards set MC_State=@MC_State where MC_CardID=@MC_CardID";
            SqlParameter[] sp = { 
                                    new SqlParameter("MC_State",state),
                                    new SqlParameter("MC_CardID",id)
                                };
            if (DBHelper.ExecuteNonQuery(sql, sp))
            {
                return Content("1");
            }
            else
            {
                return Content("0");
            }
        }
        /// <summary>
        ///会员换卡
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ExchangeCard(int id)
        {
            Models.MemCard card= db.MemCards.Find(id);
            Session["MemCard"] = card;
            return View();
        }
        /// <summary>
        /// 实现真正的会员换卡
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pwd"></param>
        /// <param name="cardno"></param>
        /// <param name="newpwd"></param>
        /// <returns></returns>
        public ActionResult TrueExchangeCard(int type,string pwd,string cardno,string newpwd)
        {
            Models.MemCard card = (MvcPoint.Models.MemCard)Session["MemCard"];
            int MC_ID = card.MC_ID;
            if (pwd==card.MC_Password)//输入密码是否正确
            {
                string sql = "update MemCards set MC_CardID=@MC_CardID,MC_Password=@MC_Password where MC_ID=@MC_ID";
                if (type == 1)//表示新卡密码与原卡一致
                {
                    SqlParameter[] sp = { 
                                            new SqlParameter("MC_CardID",cardno),
                                            new SqlParameter("MC_Password",card.MC_Password),
                                            new SqlParameter("MC_ID",MC_ID)
                                        };
                    if (DBHelper.ExecuteNonQuery(sql,sp))
                    {
                        return Content("1"); //表示成功
                    }
                    else
                    {
                        return Content("0");//表示失败
                    }
                }
                else
                {
                    SqlParameter[] sp = { 
                                            new SqlParameter("MC_CardID",cardno),
                                            new SqlParameter("MC_Password",newpwd),
                                            new SqlParameter("MC_ID",MC_ID)
                                        };
                    if (DBHelper.ExecuteNonQuery(sql, sp))
                    {
                        return Content("1"); //表示成功
                    }
                    else
                    {
                        return Content("0");//表示失败
                    }
                }
            }
            else
            {
                return Content("2"); //就密码输入错误
            }
        }
        //根据卡号/手机号得到单个会员的信息
        public ActionResult GetMemCardByM_CardIDOrM_Mobile(string CardIDOrMobile)
        {
            string sql = "select * from MemCards inner join CardLevels on MemCards.CL_ID=CardLevels.CL_ID where MC_CardID=@MC_CardID or MC_Mobile=@MC_Mobile";
            SqlParameter[] para = { 
                                    new SqlParameter("MC_CardID",CardIDOrMobile),
                                    new SqlParameter("MC_Mobile",CardIDOrMobile)
                                  };
            DataTable dt = DBHelper.ExecuteSelect(sql,para);
            //得到dt里面的第一行
            DataRow dr=dt.Rows[0];
            Models.MemCard mc=dr.ToModel<Models.MemCard>();
            Models.CardLevel cl = dr.ToModel<Models.CardLevel>();
            return Json(new{MemCard=mc,CardLevel=cl}, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Excel导出
        /// </summary>
        /// <returns></returns>
        public ActionResult Excelout()
        {
            Models.User user = Session["User"] as Models.User;
            DateTime time = DateTime.Now;
            //表格的顶层
            Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            //创建Excel工作薄
            Excel.Workbook workbook = excel.Workbooks.Add(Server.MapPath("~/ExcelTemplate/会员列表模板.xlsx"));
            //引用Excel工作簿
            excel.Visible = true;
            //创建Excel工作表
            Excel.Worksheet sheet = workbook.Worksheets[1] as Excel.Worksheet;
            //创建Excel工作表名称
            sheet.Name = "员工信息表";
            var list = db.MemCards.Where(e=>e.S_ID==user.S_ID).Include(e=>e.CardLevel).ToList();
            sheet.Cells[1, 2] = time.ToShortDateString() + "_会员列表";
            //标头单位信息
            sheet.Cells[3, 3] = list.Count;
            sheet.Cells[3, 5] = list.Where(e => e.MC_Sex == 1).Count();
            sheet.Cells[3, 7] = list.Where(e => e.MC_Sex == 0).Count();
            int i = 6;
            //遍历泛型集合中的所有记录，把数据填充到单元格里面去
            foreach (var item in list)
            {
                sheet.Cells[i, 2] = item.MC_CardID;
                sheet.Cells[i, 3] = item.MC_Name;
                sheet.Cells[i, 4] = (item.MC_Sex==1)?"男":"女";
                sheet.Cells[i, 5] = item.MC_Mobile;
                sheet.Cells[i, 6] = item.MC_CreateTime.ToString();
                sheet.Cells[i, 7] = item.CardLevel.CL_LevelName;
                i += 1;
            }
            string path = "~/ExcelTemplate/会员列表_" + time.ToString("yyyy-MM-dd_hhmmss") + ".xlsx";
            //先把文件保存到网站的Excel文件夹里面，然后再下载到客户端来
            workbook.SaveAs(Server.MapPath(path), Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlExclusive, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            workbook.Close(Missing.Value, Missing.Value, Missing.Value);
            excel.Quit();
            //保存好后实现文件的下载
            FileInfo loadFile = new FileInfo(Server.MapPath(path));
            Response.Clear();
            Response.ClearHeaders();
            Response.Buffer = false;
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(loadFile.Name, System.Text.Encoding.UTF8));
            Response.AppendHeader("Content-Length", loadFile.Length.ToString());
            Response.WriteFile(loadFile.FullName);
            Response.Flush();
            //下载完文件后将excel文件夹中的临时文件删除
            System.IO.File.Delete(Server.MapPath(path));
            //释放进程
            GC.Collect();
            return RedirectToAction("Index");
        }

    }
}
