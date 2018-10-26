using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using PagedList;
using SportNews.Constants;
using SportNews.Models;
using SportNews.Utility;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SportNews.Controllers
{
    public class SpainController : Controller
    {
        MySqlConnection connection = DbUtil.GetDBConnection();
        MySqlCommand cmd = new MySqlCommand();
        // GET: Spain
        public ActionResult Index(int? page)
        {
            string json = string.Empty;
            string JsonText = FlagsConst.newsJs + "laliga";

            using (WebClient wc = new WebClient())
            {
                wc.Headers["Content-Type"] = "application/json;charset=UTF-8";
                json = wc.DownloadString(JsonText);
            }
            newslist nl = new newslist();
            nl.ctLst = new List<ContentModel>();
            int count = 0;
            LatData objGo = JsonConvert.DeserializeObject<LatData>(json);
            foreach (var x in objGo.data)
            {
                ++count;
                ContentModel cm = new ContentModel();
                cm.title = x.title;
                cm.descp = x.description;
                cm.hot = 0;
                cm.imageLnk = x.image;
                cm.origin_url = x.link;
                cm.source = x.siteName;
                cm.Stt = count;
                nl.ctLst.Add(cm);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(nl.ctLst.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult SpainDetail(string news)
        {
            if (news.Contains("^"))
            {
                var tem = TempData.Peek("mlink");
                tem = news;
                TempData["mlink"] = tem;
            }
            else
            {
                news = (string)TempData["mlink"];
            }
            int pos = news.IndexOf("^");
            string rid = news.Substring(pos + 1);
            detailDataPr cm = new detailDataPr();
            string json = string.Empty;
            string JsonText = FlagsConst.detailJs + rid;

            using (WebClient wc = new WebClient())
            {
                wc.Headers["Content-Type"] = "application/json;charset=UTF-8";
                json = wc.DownloadString(JsonText);
            }

            detailData objGo = JsonConvert.DeserializeObject<detailData>(json);
            cm.data = new ctinfoPr();
            cm.data.meta = new memDt();
            cm.data.meta = objGo.data.meta;
            cm.data.content = new List<memContentPr>();

            foreach (var x in objGo.data.content)
            {
                memContentPr mp = new memContentPr();
                mp.head = x.head;
                mp.text = x.text;
                mp.type = x.type;
                if (mp.type != "video")
                {
                    mp.link = x.link;
                }
                else
                {
                    mp.linkVd = new List<string>();
                    int countbl = x.link.Count(f => f == ',');
                    if (countbl >= 1)
                    {
                        int ps = x.link.IndexOf(",");
                        var secWor = x.link.Substring(0, ps);
                        var secWor2 = x.link.Substring(ps + 1, x.link.Length - ps - 1);
                        string lnk1 = secWor.ToString();
                        string lnk2 = secWor2.ToString();
                        mp.linkVd.Add(lnk1);
                        mp.linkVd.Add(lnk2);
                    }
                    else
                    {
                        mp.linkVd.Add(x.link);
                    }
                }
                cm.data.content.Add(mp);
            }
            return PartialView("SpainDetail", cm);
        }
    }
}