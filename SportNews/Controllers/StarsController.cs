using MySql.Data.MySqlClient;
using SportNews.Models;
using SportNews.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace SportNews.Controllers
{
    public class StarsController : Controller
    {
        MySqlConnection connection = DbUtil.GetDBConnection();
        MySqlCommand cmd = new MySqlCommand();
        // GET: Stars
        public ActionResult Index(int? page)
        {
            newslist nl = new newslist();
            nl.ctLst = new List<ContentModel>();
            string sql = @"select a.news_id, a.title, a.description, a.image, b.category_name 
                           from news a 
                           join category b on a.cat_id = b.category_id
                           where category_id = 4";
            connection.Open();
            int count = 0;

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
                            ++count;
                            ContentModel cm = new ContentModel();
                            cm.news_id = Convert.ToInt64(reader.GetValue(0));
                            cm.title = reader.GetString(1);
                            cm.descp = reader.GetString(2);
                            try
                            {
                                cm.imageLnk = reader.GetString(3);
                            }
                            catch (Exception ex)
                            {
                                cm.imageLnk = "";
                            }
                            cm.cat_name = reader.GetString(4);
                            cm.Stt = count;
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

        [HttpGet]
        public ActionResult StarDetail(string news)
        {
            int pos = news.IndexOf("^");
            long rid = Int64.Parse(news.Substring(pos + 1));
            ContentModel cm = new ContentModel();
            string sql = @"select a.news_id, a.title, a.description, a.image, a.content, b.category_name, a.source 
                           from news a 
                           join category b on a.cat_id = b.category_id
                           where a.news_id = " + rid;
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
                            cm.news_id = Convert.ToInt64(reader.GetValue(0));
                            cm.title = reader.GetString(1);
                            cm.descp = reader.GetString(2);
                            cm.content = reader.GetString(4);

                            try
                            {
                                cm.imageLnk = reader.GetString(3);
                            }
                            catch (Exception ex)
                            {
                                cm.imageLnk = "";
                            }
                            cm.cat_name = reader.GetString(5);
                            cm.source = reader.GetString(6);

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
                connection.Dispose();
                connection = null;
            }
            return PartialView("StarDetail", cm);
        }
    }
}