using MySql.Data.MySqlClient;
using PagedList;
using SportNews.Models;
using SportNews.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SportNews.Controllers
{
    public class ConteController : Controller
    {
        MySqlConnection connection = DbUtil.GetDBConnection();
        MySqlCommand cmd = new MySqlCommand();
        List<Category> lstCa = new List<Category>();
        //
        // GET: /Conte/
        public ActionResult Index(int? page)
        {
            var spath = TempData.Peek("page");
            spath = page;
            TempData["page"] = spath;
            ContentModel cm = new ContentModel();
            getCate();
            cm.catList = new List<Category>();
            cm.catList = lstCa;
            return View(cm);
        }

        [HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Index(ContentModel model)
        {
            if (model.category_id == 0)
            {
                model.category_id = -1;
                connection.Open();
                string sql = "Insert into news (title, description, cat_id, created_date, content, image, source, hot) values ( ";

                try
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    DateTime theDate = DateTime.Now;
                    theDate.ToString("dd/MM/yyyy");
                    //string s = Encoding.UTF8.GetBytes(model.title.ToString())[0].ToString();

                    sql = sql + "'" + model.title.ToString() + "'";
                    sql = sql + ", '" + model.descp.ToString() + "'";
                    sql = sql + ", " + model.category_id + "";
                    sql = sql + ", '" + theDate + "'";
                    sql = sql + ", '" + model.content + "'";
                    sql = sql + ", '" + model.imageLnk + "'";
                    sql = sql + ", '" + model.source + "'";
                    sql = sql + ", '" + model.hot + "'";

                    sql = sql + ");";

                    cmd.CommandText = sql;
                    int rowCount = cmd.ExecuteNonQuery();

                    Console.WriteLine("Row Count affected = " + rowCount);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + sql);
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
            }
            else if(ModelState.IsValid)
            {                
                //MySqlConnection connection = DbUtil.GetDBConnection();
                connection.Open();
                string sql = "Insert into news (title, description, cat_id, created_date, content, image, source, hot) values ( ";

                try
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    DateTime theDate = DateTime.Now;
                    theDate.ToString("dd/MM/yyyy");
                    //string s = Encoding.UTF8.GetBytes(model.title.ToString())[0].ToString();

                    sql = sql + "'" + model.title.ToString() + "'";
                    sql = sql + ", '" + model.descp.ToString() + "'";
                    sql = sql + ", " + model.category_id + "";
                    sql = sql + ", '" + theDate + "'";
                    sql = sql + ", '" + model.content + "'";
                    sql = sql + ", '" + model.imageLnk + "'";
                    sql = sql + ", '" + model.source + "'";
                    sql = sql + ", '" + model.hot + "'";

                    sql = sql + ");";

                    cmd.CommandText = sql;
                    int rowCount = cmd.ExecuteNonQuery();

                    Console.WriteLine("Row Count affected = " + rowCount);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + sql);
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                    connection = null;
                }
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult Search(int? page)
        {
            var tmp = TempData["page"];
            if (tmp != null)
            {
                page = (int)TempData["page"];
            }
            newslist nl = new newslist();
            nl.ctLst = new List<ContentModel>();
            string sql = @"select a.title, a.description, b.category_name, a.news_id, a.origin_url 
                           from news a 
                           join category b on a.cat_id = b.category_id
                           order by news_id desc";
            connection.Open();

            try
            {
                cmd.Connection = connection;
                cmd.CommandText = sql;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ContentModel cm = new ContentModel();
                            cm.title = reader.GetString(0);
                            cm.descp = reader.GetString(1);
                            cm.cat_name = reader.GetString(2);
                            cm.news_id = Int64.Parse(reader.GetString(3));
                            if (reader.GetString(4) != null || reader.GetString(4) != "")
                            {
                                cm.origin_url = reader.GetString(4);
                            }
                            //cm.category_id = Convert.ToInt64(reader.GetValue(0));
                            nl.ctLst.Add(cm);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + sql);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                connection.Close();
                //connection.Dispose();
                //connection = null;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return PartialView("_NewsLst", nl.ctLst.ToPagedList(pageNumber, pageSize));
        }

        public List<Category> getCate()
        {
            string sql = "select category_id, category_name from category ";
            connection.Open();

            try
            {
                cmd.Connection = connection;
                cmd.CommandText = sql;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while(reader.Read()){
                        Category um = new Category();
                        um.category_name = reader.GetString(1);
                        um.category_id = Convert.ToInt64(reader.GetValue(0));
                        lstCa.Add(um);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + sql);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                connection.Close();
                //connection.Dispose();
                //connection = null;
            }
            return lstCa;
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            getCate();
            ContentModel cm = new ContentModel();
            cm.catList = new List<Category>();
            cm.catList = lstCa;
            string sql = @"select a.title, a.description, b.category_name,
                           a.content, a.image, a.source, a.cat_id , a.news_id, a.hot, a.origin_url 
                           from news a 
                           left join category b on a.cat_id = b.category_id
                           where a.news_id = " + id;
            connection.Open();

            try
            {
                cmd.Connection = connection;
                cmd.CommandText = sql;
                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            cm.title = reader.GetString(0);
                            cm.descp = reader.GetString(1);
                            if(cm.cat_name != null)
                            {
                                cm.cat_name = reader.GetString(2);
                            }
                            cm.content = reader.GetString(3);
                            cm.imageLnk = reader.GetString(4);
                            cm.source = reader.GetString(5);
                            cm.category_id = Int64.Parse(reader.GetString(6));
                            cm.news_id = Int64.Parse(reader.GetString(7));
                            cm.hot = Int32.Parse(reader.GetString(8));
                            if (reader.GetString(9) != null || reader.GetString(9) != "")
                            {
                                cm.origin_url = reader.GetString(9);
                            }
                            //cm.category_id = Convert.ToInt64(reader.GetValue(0));
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + sql);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                connection.Close();
                //connection.Dispose();
                //connection = null;
            }
            return View(cm);
        }

        [HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult Edit(ContentModel model)
        {
            string kq = "";
            if (ModelState.IsValid)
            {
                //MySqlConnection connection = DbUtil.GetDBConnection();
                connection.Open();
                string sql = "update news set ";

                try
                {

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    DateTime theDate = DateTime.Now;
                    theDate.ToString("dd/MM/yyyy");
                    //string s = Encoding.UTF8.GetBytes(model.title.ToString())[0].ToString();

                    sql = sql + "title = '" + model.title.ToString() + "'";
                    sql = sql + ",description = '" + model.descp.ToString() + "'";
                    sql = sql + ",cat_id = " + model.category_id + "";
                    //sql = sql + ", '" + theDate + "'";
                    sql = sql + ",content = '" + model.content + "'";
                    sql = sql + ",image = '" + model.imageLnk + "'";
                    sql = sql + ",source = '" + model.source + "'";
                    sql = sql + ",hot = " + model.hot;
                    sql = sql + ",origin_url = '" + model.origin_url + "'";

                    sql = sql + " where news_id = " + model.news_id + ";";

                    cmd.CommandText = sql;
                    int rowCount = cmd.ExecuteNonQuery();

                    Console.WriteLine("Row Count affected = " + rowCount);
                    kq = "OK";
                    //return RedirectToAction("Index", "Home");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + sql);
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    connection.Close();
                    //connection.Dispose();
                    //connection = null;
                }
            }

            return Json(kq, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult Delete(long id)
        {
            string kq = "";
            if (ModelState.IsValid)
            {
                //MySqlConnection connection = DbUtil.GetDBConnection();
                connection.Open();
                string sql = "delete from news ";

                try
                {

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;

                    sql = sql + " where news_id = " + id + ";";

                    cmd.CommandText = sql;
                    int rowCount = cmd.ExecuteNonQuery();

                    Console.WriteLine("Row Count affected = " + rowCount);
                    kq = "OK";
                    //return RedirectToAction("Index", "Home");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + sql);
                    Console.WriteLine(e.StackTrace);
                }
                finally
                {
                    connection.Close();
                    //connection.Dispose();
                    //connection = null;
                }
            }

            return Json(kq, JsonRequestBehavior.AllowGet);
        }
    }
}