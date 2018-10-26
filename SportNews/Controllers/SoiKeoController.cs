using SportNews.Constants;
using SportNews.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SportNews.Controllers
{
    public class SoiKeoController : Controller
    {
        // GET: SoiKeo
        public ActionResult Index(string mats, List<OddsRate> lst)
        {
            //List<OddsRate> AsLst = (List<OddsRate>)TempData["parsing"];
            //TempData.Keep();

            string time = "";
            string home = "";
            string away = "";
            if (mats != null)
            {
                int p1 = mats.IndexOf("+");
                int p2 = mats.LastIndexOf("+");
                int p3 = mats.IndexOf("-");

                time = mats.Substring(0, p1);
                home = mats.Substring(p1 + 1, p2 - p1 - 1);
                away = mats.Substring(p2 + 1, p3 - p2 - 1);

                var tem = TempData.Peek("match");
                tem = mats;
                TempData["match"] = tem;
            }
            else
            {
                mats = (string)TempData["match"];
                int p1 = mats.IndexOf("+");
                int p2 = mats.LastIndexOf("+");
                int p3 = mats.IndexOf("-");

                time = mats.Substring(0, p1);
                home = mats.Substring(p1 + 1, p2 - p1 - 1);
                away = mats.Substring(p2 + 1, p3 - p2 - 1);

                var tem = TempData.Peek("match");
                tem = mats;
                TempData["match"] = tem;
            }

            //ViewBag.Message = "World cup 2018 Info";
            int pos = mats.IndexOf("-");
            long rid = Int64.Parse(mats.Substring(pos + 1));
            string json = string.Empty;
            string jsonMa = string.Empty;

            string JsonText = FlagsConst.matchLnk + rid + "/odds";
            string JsonTextMa = FlagsConst.matchLnk + rid + "/details";

            SumLst ct = new SumLst();
            ct.LstAll = new List<CatOdds>();
            //using (StreamReader reader = new StreamReader(JsonText))
            //{
            //    json = reader.ReadToEnd();
            //}

            using (WebClient wc = new WebClient())
            {
                try
                {
                    json = wc.DownloadString(JsonText);
                    jsonMa = wc.DownloadString(JsonTextMa);
                }
                catch (Exception ex)
                {
                    ViewBag.msg = ex.Message;
                    return View("SoiKeo", ct);
                    //return Json(ct, JsonRequestBehavior.AllowGet);
                }
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            var obj = jss.Deserialize<dynamic>(json);
            var objma = jss.Deserialize<dynamic>(jsonMa);

            DateTime ustime = DateTime.ParseExact(time, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
            var timevr = ustime.ToString("HH:mm");

            int len = obj.Length;
            int hsc = objma["game"]["tournaments"][0]["events"][0]["status"]["code"] == null ? "?" : objma["game"]["tournaments"][0]["events"][0]["status"]["code"]; ;

            for (int i = 0; i < len; i++)
            {
                CatOdds std = new CatOdds();

                std.OddLst = new List<OddsRate>();
                std.name = obj[i]["name"];
                int lende;
                string tmps;
                if (hsc >= 6 && hsc <= 7)
                {
                    lende = obj[i]["live"][0]["odds"].Length;
                    tmps = "live";
                }
                else
                {
                    lende = obj[i]["regular"][0]["odds"].Length;
                    tmps = "regular";
                }
                //for (int j = 0; j < lende; j++)
                //{
                var match = new OddsRate();
                if (i != len - 1)
                {
                    if (lende == 3)
                    {
                        match.HomeWn = obj[i][tmps][0]["odds"][lende - 3]["decimalValue"];
                        match.Draw = obj[i][tmps][0]["odds"][lende - 2]["decimalValue"];
                        match.AwayWn = obj[i][tmps][0]["odds"][lende - 1]["decimalValue"];
                        match.HomeChoice = obj[i][tmps][0]["odds"][lende - 3]["choice"];
                        match.DrawChoice = obj[i][tmps][0]["odds"][lende - 2]["choice"];
                        match.AwayChoice = obj[i][tmps][0]["odds"][lende - 1]["choice"];
                        match.cate = 1;
                    }
                    else
                    {
                        match.HomeWn = obj[i][tmps][0]["odds"][lende - 2]["decimalValue"];
                        match.AwayWn = obj[i][tmps][0]["odds"][lende - 1]["decimalValue"];
                        match.HomeChoice = obj[i][tmps][0]["odds"][lende - 2]["choice"];
                        match.AwayChoice = obj[i][tmps][0]["odds"][lende - 1]["choice"];
                        match.cate = 2;
                    }
                }
                else
                {
                    match.GoaLst = new List<MatchGoals>();
                    var lenfina = obj[len - 1][tmps].Length;
                    match.cate = 3;
                    for (int j = 0; j < lenfina; j++)
                    {
                        var mt = new MatchGoals();
                        mt.goal = obj[len - 1][tmps][j]["handicap"];
                        mt.OverGo = obj[len - 1][tmps][j]["odds"][0]["decimalValue"];
                        mt.UnderGo = obj[len - 1][tmps][j]["odds"][1]["decimalValue"];
                        match.GoaLst.Add(mt);
                    }
                }


                std.OddLst.Add(match);
                //}
                ct.LstAll.Add(std);
            }

            CatOdds stdAs = new CatOdds();
            stdAs.OddLst = new List<OddsRate>();

            foreach (var ma in lst)
            {
                bool check1 = false, check3 = false, check4 = false;
                DateTime detime = DateTime.ParseExact(ma.timeMt, "HH:mm", CultureInfo.InvariantCulture);
                string name3 = detime.AddHours(-1).ToString("HH:mm");

                if (timevr == name3)
                {
                    check1 = true;
                    home = convertToUnSign3(home);
                    if (home.ToLower().Contains(ma.HomeTe.ToLower()))
                    {
                        check3 = true;
                        away = convertToUnSign3(away);
                        if (away.ToLower().Contains(ma.AwayTe.ToLower()))
                        {
                            check4 = true;
                            if (check1 == true && check3 == true && check4 == true)
                            {
                                ma.cate = 6;
                                stdAs.OddLst.Add(ma);
                                stdAs.name = "Kèo Châu Á";
                                //match.ore.LstAll.Add(std);
                                ct.LstAll.Add(stdAs);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    check1 = false;
                }

            }

            //return Json(ct, JsonRequestBehavior.AllowGet);
            return View(ct);
        }

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}