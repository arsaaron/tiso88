using MySql.Data.MySqlClient;
using PagedList;
using SportNews.Models;
using SportNews.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportNews.Controllers
{
    public class DataWhController : Controller
    {
        MySqlConnection connection = DbUtil.GetDBConnection();
        MySqlCommand cmd = new MySqlCommand();
        List<Category> lstCa = new List<Category>();

        // GET: DataWh
        public ActionResult Index(int? page)
        {
            getCate();
            newslist nl = new newslist();
            nl.ctLst = new List<ContentModel>();
            string sql = @"select a.title, a.description, b.category_name, a.news_id, a.origin_url 
                           from news a 
                           left join category b on a.cat_id = b.category_id
                           where a.cat_id = -1";
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

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
                            //cm.cat_name = reader.GetString(2);
                            cm.news_id = Int64.Parse(reader.GetString(3));
                            cm.origin_url = reader.GetString(4);
                            cm.catList = new List<Category>();
                            cm.catList = lstCa;
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
            return View(nl.ctLst.ToPagedList(pageNumber, pageSize));
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
                        while (reader.Read())
                        {
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

        [HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public JsonResult SaveCt(long catId, long id)
        {
            string kq = "";
            //if (ModelState.IsValid)
            //{
                //MySqlConnection connection = DbUtil.GetDBConnection();
                connection.Open();
                string sql = "update news set ";

                try
                {

                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;
                    DateTime theDate = DateTime.Now;
                    //string s = Encoding.UTF8.GetBytes(model.title.ToString())[0].ToString();

                    sql = sql + "cat_id = " + catId + "";

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
            //}

            return Json(kq, JsonRequestBehavior.AllowGet);
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
                            //if (reader.GetString(2) != null)
                            //{
                            //    cm.cat_name = reader.GetString(2);
                            //}
                            cm.content = reader.GetString(3);
                            cm.imageLnk = reader.GetString(4);
                            cm.source = reader.GetString(5);
                            cm.category_id = Int64.Parse(reader.GetString(6));
                            cm.news_id = Int64.Parse(reader.GetString(7));
                            //cm.hot = Int32.Parse(reader.GetString(8));
                            //if (reader.GetString(8) != null)
                            //{
                            //    cm.hot = Int32.Parse(reader.GetString(8));
                            //}
                            cm.origin_url = reader.GetString(9);

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
                    sql = sql + ",origin_url = '" + model.origin_url + "'";
                    sql = sql + ",hot = " + model.hot;

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

    }
}