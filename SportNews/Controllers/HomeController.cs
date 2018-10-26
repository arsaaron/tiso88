using SportNews.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SportNews.Constants;
using MySql.Data.MySqlClient;
using SportNews.Utility;
using System.Data.Common;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagedList;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace SportNews.Controllers
{
    public class HomeController : Controller
    {
        List<MatchInfoModel> matches = new List<MatchInfoModel>();
        //private bool connection_open;
        //private MySqlConnection connection;
        MySqlConnection connection = DbUtil.GetDBConnection();
        MySqlCommand cmd = new MySqlCommand();
        RoundInfo rig = new RoundInfo();
        RoundInfo rigIc = new RoundInfo();

        List<MatchInfoModel> matGlo = new List<MatchInfoModel>();

        //List<MatchInfoModel> rounds = new List<MatchInfoModel>();

        //private void Get_Connection()
        //{
        //    connection_open = false;
        //    connection = new MySqlConnection();
        //    connection.ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
        //    MySqlConnection conn = new MySqlConnection(connection.ConnectionString);
        //    if (Open_Local_Connection())
        //    {
        //        connection_open = true;
        //    }
        //    else
        //    {
        //        ViewBag.msg("No database connection connection made...\n Exiting now", "Database Connection Error");
        //    }

        //}

        //private bool Open_Local_Connection()
        //{
        //    try
        //    {
        //        connection.Open();
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        return false;
        //    }
        //}
        public HomeController()
        {

        }

        public ActionResult Index()
        {
            //await Task.Run(() => Parsing("https://mobile.bet365.com/?apptype=&appversion=&cb=1534440112#type=Coupon;key=1-1-13-37628398-2-1-0-0-1-0-0-4100-0-0-1-0-0-0-0-0-0-0-0;ip=0;lng=1;anim=1"));
            //getInfoSum(FlagsConst.ftLinkPrem);
            return View();
        }

        public List<MatchInfoModel> getInfoSum(string strLnk, string cst)
        {
            string cstAsa = "";
            var spath = TempData.Peek("InfoLst2");
            //Parsing();
            switch (cst)
            {
                case "Prm":
                    cstAsa = FlagsConst.AsaLinkPrem;
                    break;
                case "Llg":
                    cstAsa = FlagsConst.AsaLinkLlg;
                    break;
                case "Bdla":
                    cstAsa = FlagsConst.AsaLinkBdla;
                    break;
                case "Sra":
                    cstAsa = FlagsConst.AsaLinkSra;
                    break;
                case "Lg1":
                    cstAsa = FlagsConst.AsaLinkLg1;
                    break;
                case "C1":
                    cstAsa = FlagsConst.AsaLinkC1;
                    spath = TempData.Peek("InfoLst7");
                    break;
                case "Eup":
                    cstAsa = FlagsConst.AsaLinkEup;
                    spath = TempData.Peek("InfoLst8");
                    break;
                case "Unl":
                    cstAsa = FlagsConst.AsaLinkUnl;
                    break;
                //case "Fa":
                //    cstAsa = FlagsConst.AsaLinkFa;
                //    spath = TempData.Peek("InfoLst5");
                //    break;
                case "Efl":
                    cstAsa = FlagsConst.AsaLinkEfl;
                    spath = TempData.Peek("InfoLst6");
                    break;
            }

            var tmpLst = LoadParsing(cstAsa);
            string json = string.Empty;
            string jsonMa = string.Empty;
            string JsonText = strLnk;
            //var spath = TempData.Peek("InfoLst2");
            var tmpSS = TempData.Peek("season");
            var tmpCt = TempData.Peek("count");
            var sPrs = TempData.Peek("parsing");
            sPrs = tmpLst;
            TempData["parsing"] = sPrs;
            //RoundInfo ri = new RoundInfo();
            rig.Schelst = new List<MatchInfoModel>();

            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(JsonText);
            }

            NewObj m = JsonConvert.DeserializeObject<NewObj>(json);

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //var obj = jss.Deserialize<dynamic>(json);
            //int len = obj["tournaments"].Length;
            int len = m.tournaments.Count;
            tmpCt = m.tournaments.Count;
            TempData["count"] = tmpCt;
            int i = 0;
            if (cst == "C1" || cst == "Eup")
            {
                i = i + 2;
            }

            for (; i < len; i++)
            {
                tmpSS = m.tournaments[i].tournament.name;
                TempData["season"] = tmpSS;
                int lende = m.tournaments[i].events.Count;
                for (int j = 0; j < lende; j++)
                {
                    var nema = new NewMatchInfo();
                    nema = m.tournaments[i].events[j];
                    var match = new MatchInfoModel();
                    match.ore = new SumLst();
                    match.ore.LstAll = new List<CatOdds>();
                    CatOdds std = new CatOdds();
                    std.OddLst = new List<OddsRate>();
                    match.MatchId = nema.id;
                    //string JsonTextMa = FlagsConst.matchLnk + match.MatchId + "/details";
                    //using (WebClient wc = new WebClient())
                    //{
                    //    jsonMa = wc.DownloadString(JsonTextMa);
                    //}
                    //var objma = jss.Deserialize<dynamic>(jsonMa);
                    try
                    {
                        int hsc = nema.status.code;
                        match.status = hsc.ToString();
                        string asc = nema.statusDescription == null ? "?" : nema.statusDescription;
                        match.timeIn = asc.ToString();
                    }
                    catch (KeyNotFoundException ex)
                    {
                        match.status = "";
                        match.timeIn = "";
                    }

                    match.HomeTeam = nema.homeTeam.name;
                    match.AwayTeam = nema.awayTeam.name;
                    match.HomeId = nema.homeTeam.id;
                    match.AwayId = nema.awayTeam.id;
                    match.HomeFlag = FlagsConst.ptLink + match.HomeId.ToString() + ".png";
                    match.AwayFlag = FlagsConst.ptLink + match.AwayId.ToString() + ".png";

                    double time = nema.startTimestamp;
                    match.timeSt = unixConvert(time);
                    DateTime ustime = DateTime.ParseExact(match.timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                   CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);

                    //var datevr = ustime.ToString("dd/MMM");
                    var timevr = ustime.ToString("HH:mm");

                    var tmp = timeno.AddDays(1);
                    if (ustime > timeno)
                    {
                        if ((ustime >= timeno) && (ustime < timeno.AddDays(2)))
                        {
                            //match.ore = getEuOdd(match.MatchId);
                            foreach (var ma in tmpLst)
                            {
                                bool check1 = false, check3 = false, check4 = false;
                                DateTime dtime = DateTime.ParseExact(ma.timeMt, "HH:mm", CultureInfo.InvariantCulture);
                                //string easternZoneId = "GMT Standard Time";
                                //TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
                                //string name3 = TimeZoneInfo.ConvertTimeToUtc(ustime, easternZone).ToString("HH:mm");
                                string name3 = dtime.AddHours(-1).ToString("HH:mm");

                                if (timevr == name3)
                                {
                                    if(match.HomeTeam == "Beşiktaş")
                                    {
                                        match.HomeTeam = "Besiktas";
                                    }
                                    else if (match.HomeTeam == "Stade Rennais")
                                    {
                                        match.HomeTeam = "Rennes";
                                    }
                                    check1 = true;
                                    if (match.HomeTeam.ToLower().Contains(ma.HomeTe.ToLower()) || match.HomeTeam.ToLower().Contains(ma.SubHomeTe.ToLower()))
                                    {
                                        if (match.AwayTeam == "Beşiktaş")
                                        {
                                            match.AwayTeam = "Besiktas";
                                        }
                                        else if (match.AwayTeam == "Stade Rennais")
                                        {
                                            match.AwayTeam = "Rennes";
                                        }
                                        check3 = true;
                                        if (procEnc(match.AwayTeam).ToLower().Contains(ma.AwayTe.ToLower()) || procEnc(match.AwayTeam).ToLower().Contains(ma.SubAwayTe.ToLower()))
                                        {
                                            check4 = true;
                                            if (check1 == true && check3 == true && check4 == true)
                                            {
                                                std.OddLst.Add(ma);
                                                match.ore.LstAll.Add(std);
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
                            //match.ore = ParsingEng(timevr, match.HomeTeam, match.AwayTeam);
                            match.HomeScore = "?";
                            match.AwayScore = "?";
                        }
                        else if ((ustime >= timeno) && (ustime < timeno.AddDays(5)))
                        {
                            //match.ore = getEuOdd(match.MatchId);
                            foreach (var ma in tmpLst)
                            {
                                bool check1 = false, check3 = false, check4 = false;
                                DateTime dtime = DateTime.ParseExact(ma.timeMt, "HH:mm", CultureInfo.InvariantCulture);
                                //string easternZoneId = "GMT Standard Time";
                                //TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
                                //string name3 = TimeZoneInfo.ConvertTimeToUtc(ustime, easternZone).ToString("HH:mm");
                                string name3 = dtime.AddHours(-1).ToString("HH:mm");

                                if (timevr == name3)
                                {
                                    check1 = true;
                                    if (match.HomeTeam.ToLower().Contains(ma.HomeTe.ToLower()))
                                    {
                                        check3 = true;
                                        if (match.AwayTeam.ToLower().Contains(ma.AwayTe.ToLower()))
                                        {
                                            check4 = true;
                                            if (check1 == true && check3 == true && check4 == true)
                                            {
                                                std.OddLst.Add(ma);
                                                match.ore.LstAll.Add(std);
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
                            //match.ore = ParsingEng(timevr, match.HomeTeam, match.AwayTeam);
                            match.HomeScore = "?";
                            match.AwayScore = "?";
                        }
                    }
                    else
                    {
                        try
                        {
                            var tm = ustime.AddDays(0.125);
                            if (timeno > ustime && timeno < tm)
                            {
                                foreach (var ma in tmpLst)
                                {
                                    bool check1 = false, check3 = false, check4 = false;
                                    DateTime dtime = DateTime.ParseExact(ma.timeMt, "HH:mm", CultureInfo.InvariantCulture);
                                    //string easternZoneId = "GMT Standard Time";
                                    //TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
                                    //string name3 = TimeZoneInfo.ConvertTimeToUtc(ustime, easternZone).ToString("HH:mm");
                                    string name3 = dtime.AddHours(-1).ToString("HH:mm");

                                    if (timevr == name3)
                                    {
                                        check1 = true;
                                        if (match.HomeTeam.ToLower().Contains(ma.HomeTe.ToLower()))
                                        {
                                            check3 = true;
                                            if (match.AwayTeam.ToLower().Contains(ma.AwayTe.ToLower()))
                                            {
                                                check4 = true;
                                                if (check1 == true && check3 == true && check4 == true)
                                                {
                                                    std.OddLst.Add(ma);
                                                    match.ore.LstAll.Add(std);
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
                            }
                            //if ((ustime >= timeno) && (ustime < timeno.AddDays(0.125)))
                            //{
                            //    match.ore = ParsingEng(datevr, timevr, match.HomeTeam, match.AwayTeam);
                            //}
                            int hsc = nema.homeScore.current;
                            match.HomeScore = hsc.ToString();
                            int asc = nema.awayScore.current;
                            match.AwayScore = asc.ToString();
                        }
                        catch (KeyNotFoundException ex)
                        {
                            match.HomeScore = "?";
                            match.AwayScore = "?";
                        }
                    }

                    //var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                    //                   CultureInfo.InvariantCulture);
                    //DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    //var tmp = timeno.AddDays(1);

                    match.RoundInfo = nema.roundInfo.round;

                    rig.Schelst.Add(match);
                }
            }
            spath = rig.Schelst;
            //TempData["InfoLst2"] = spath;
            switch (cst)
            {
                //case "Fa":
                //    TempData["InfoLst5"] = spath;
                //    break;
                case "Efl":
                    TempData["InfoLst6"] = spath;
                    break;
                case "C1":
                    TempData["InfoLst7"] = spath;
                    break;
                case "Eup":
                    TempData["InfoLst8"] = spath;
                    break;
                default:
                    TempData["InfoLst2"] = spath;
                    break;
            }
            return rig.Schelst;
        }

        public List<MatchInfoModel> getInfoSumEng(string strLnk, string cst)
        {
            string cstAsa = "";
            //Parsing();
            switch (cst)
            {
                case "Prm":
                    cstAsa = FlagsConst.AsaLinkPrem;
                    break;
                case "Llg":
                    cstAsa = FlagsConst.AsaLinkLlg;
                    break;
                case "Bdla":
                    cstAsa = FlagsConst.AsaLinkBdla;
                    break;
                case "Sra":
                    cstAsa = FlagsConst.AsaLinkSra;
                    break;
                case "Lg1":
                    cstAsa = FlagsConst.AsaLinkLg1;
                    break;
            }

            var tmpLst = LoadParsing(cstAsa);
            string json = string.Empty;
            string jsonMa = string.Empty;
            string JsonText = strLnk;
            var spath = TempData.Peek("InfoLst2");
            var tmpSS = TempData.Peek("season");
            var tmpCt = TempData.Peek("count");

            //RoundInfo ri = new RoundInfo();
            rig.Schelst = new List<MatchInfoModel>();

            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(JsonText);
            }

            NewObj m = JsonConvert.DeserializeObject<NewObj>(json);

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //var obj = jss.Deserialize<dynamic>(json);
            //int len = obj["tournaments"].Length;
            int len = m.tournaments.Count;
            tmpCt = m.tournaments.Count;
            TempData["count"] = tmpCt;

            for (int i = 0; i < len; i++)
            {
                tmpSS = m.tournaments[i].tournament.name;
                TempData["season"] = tmpSS;
                int lende = m.tournaments[i].events.Count;
                for (int j = 0; j < lende; j++)
                {
                    var nema = new NewMatchInfo();
                    nema = m.tournaments[i].events[j];
                    var match = new MatchInfoModel();
                    match.ore = new SumLst();
                    match.ore.LstAll = new List<CatOdds>();
                    CatOdds std = new CatOdds();
                    std.OddLst = new List<OddsRate>();
                    match.MatchId = nema.id;
                    //string JsonTextMa = FlagsConst.matchLnk + match.MatchId + "/details";
                    //using (WebClient wc = new WebClient())
                    //{
                    //    jsonMa = wc.DownloadString(JsonTextMa);
                    //}
                    //var objma = jss.Deserialize<dynamic>(jsonMa);
                    try
                    {
                        int hsc = nema.status.code;
                        match.status = hsc.ToString();
                        string asc = nema.statusDescription == null ? "?" : nema.statusDescription;
                        match.timeIn = asc.ToString();
                    }
                    catch (KeyNotFoundException ex)
                    {
                        match.status = "";
                        match.timeIn = "";
                    }

                    match.HomeTeam = nema.homeTeam.name;
                    match.AwayTeam = nema.awayTeam.name;
                    match.HomeId = nema.homeTeam.id;
                    match.AwayId = nema.awayTeam.id;
                    match.HomeFlag = FlagsConst.ptLink + match.HomeId.ToString() + ".png";
                    match.AwayFlag = FlagsConst.ptLink + match.AwayId.ToString() + ".png";

                    double time = nema.startTimestamp;
                    match.timeSt = unixConvert(time);
                    DateTime ustime = DateTime.ParseExact(match.timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                   CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);

                    //var datevr = ustime.ToString("dd/MMM");
                    var timevr = ustime.ToString("HH:mm");

                    var tmp = timeno.AddDays(1);
                    if (ustime > timeno)
                    {
                        if ((ustime >= timeno) && (ustime < timeno.AddDays(2)))
                        {
                            //match.ore = getEuOdd(match.MatchId);
                            foreach (var ma in tmpLst)
                            {
                                bool check1 = false, check3 = false, check4 = false;
                                DateTime dtime = DateTime.ParseExact(ma.timeMt, "HH:mm", CultureInfo.InvariantCulture);
                                //string easternZoneId = "GMT Standard Time";
                                //TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
                                //string name3 = TimeZoneInfo.ConvertTimeToUtc(ustime, easternZone).ToString("HH:mm");
                                string name3 = dtime.AddHours(-1).ToString("HH:mm");

                                if (timevr == name3)
                                {
                                    check1 = true;
                                    if (match.HomeTeam.ToLower().Contains(ma.HomeTe.ToLower()) || match.HomeTeam.ToLower().Contains(ma.SubHomeTe.ToLower()))
                                    {
                                        check3 = true;
                                        if (match.AwayTeam.ToLower().Contains(ma.AwayTe.ToLower()) || match.AwayTeam.ToLower().Contains(ma.SubAwayTe.ToLower()))
                                        {
                                            check4 = true;
                                            if (check1 == true && check3 == true && check4 == true)
                                            {
                                                std.OddLst.Add(ma);
                                                match.ore.LstAll.Add(std);
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
                            //match.ore = ParsingEng(timevr, match.HomeTeam, match.AwayTeam);
                            match.HomeScore = "?";
                            match.AwayScore = "?";
                        }
                        else if ((ustime >= timeno) && (ustime < timeno.AddDays(5)))
                        {
                            //match.ore = getEuOdd(match.MatchId);
                            foreach (var ma in tmpLst)
                            {
                                bool check1 = false, check3 = false, check4 = false;
                                DateTime dtime = DateTime.ParseExact(ma.timeMt, "HH:mm", CultureInfo.InvariantCulture);
                                //string easternZoneId = "GMT Standard Time";
                                //TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
                                //string name3 = TimeZoneInfo.ConvertTimeToUtc(ustime, easternZone).ToString("HH:mm");
                                string name3 = dtime.AddHours(-1).ToString("HH:mm");

                                if (timevr == name3)
                                {
                                    check1 = true;
                                    if (match.HomeTeam.ToLower().Contains(ma.HomeTe.ToLower()))
                                    {
                                        check3 = true;
                                        if (match.AwayTeam.ToLower().Contains(ma.AwayTe.ToLower()))
                                        {
                                            check4 = true;
                                            if (check1 == true && check3 == true && check4 == true)
                                            {
                                                std.OddLst.Add(ma);
                                                match.ore.LstAll.Add(std);
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
                            //match.ore = ParsingEng(timevr, match.HomeTeam, match.AwayTeam);
                            match.HomeScore = "?";
                            match.AwayScore = "?";
                        }
                    }
                    else
                    {
                        try
                        {
                            var tm = ustime.AddDays(0.125);
                            if (timeno > ustime && timeno < tm)
                            {
                                foreach (var ma in tmpLst)
                                {
                                    bool check1 = false, check3 = false, check4 = false;
                                    DateTime dtime = DateTime.ParseExact(ma.timeMt, "HH:mm", CultureInfo.InvariantCulture);
                                    //string easternZoneId = "GMT Standard Time";
                                    //TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
                                    //string name3 = TimeZoneInfo.ConvertTimeToUtc(ustime, easternZone).ToString("HH:mm");
                                    string name3 = dtime.AddHours(-1).ToString("HH:mm");

                                    if (timevr == name3)
                                    {
                                        check1 = true;
                                        if (match.HomeTeam.ToLower().Contains(ma.HomeTe.ToLower()))
                                        {
                                            check3 = true;
                                            if (match.AwayTeam.ToLower().Contains(ma.AwayTe.ToLower()))
                                            {
                                                check4 = true;
                                                if (check1 == true && check3 == true && check4 == true)
                                                {
                                                    std.OddLst.Add(ma);
                                                    match.ore.LstAll.Add(std);
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
                            }
                            //if ((ustime >= timeno) && (ustime < timeno.AddDays(0.125)))
                            //{
                            //    match.ore = ParsingEng(datevr, timevr, match.HomeTeam, match.AwayTeam);
                            //}
                            int hsc = nema.homeScore.current;
                            match.HomeScore = hsc.ToString();
                            int asc = nema.awayScore.current;
                            match.AwayScore = asc.ToString();
                        }
                        catch (KeyNotFoundException ex)
                        {
                            match.HomeScore = "?";
                            match.AwayScore = "?";
                        }
                    }

                    //var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                    //                   CultureInfo.InvariantCulture);
                    //DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    //var tmp = timeno.AddDays(1);

                    match.RoundInfo = nema.roundInfo.round;

                    rig.Schelst.Add(match);
                }
            }
            spath = rig.Schelst;
            TempData["InfoLst2"] = spath;
            return rig.Schelst;
        }

        public Formation getMatch(long rid)
        {
            //int pos = match.IndexOf("^");
            //long rid = Int64.Parse(match.Substring(pos + 1));
            string json = string.Empty;
            string jsonGo = string.Empty;
            string jsonMa = string.Empty;
            string jsonSts = string.Empty;
            string jsonFoHm = string.Empty;
            string jsonFoAw = string.Empty;
            string jsonHead = string.Empty;

            string JsonText = FlagsConst.lineuplnk + rid + "/lineups";
            string JsonTextGo = FlagsConst.GoalLnk + rid + "/incidents";
            string JsonTextMa = FlagsConst.matchLnk + rid + "/details";
            string JsonTextSts = FlagsConst.statsLnk + rid + "/statistics";
            string JsonTextHead = FlagsConst.HeadLnk + rid + "/matches/json";

            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(JsonText);
                jsonGo = wc.DownloadString(JsonTextGo);
                jsonMa = wc.DownloadString(JsonTextMa);
                jsonSts = wc.DownloadString(JsonTextSts);
                jsonHead = wc.DownloadString(JsonTextHead);
            }

            lineUp m = JsonConvert.DeserializeObject<lineUp>(json);
            var objGo = JsonConvert.DeserializeObject<dynamic>(jsonGo);
            DetailEvent objMa = JsonConvert.DeserializeObject<DetailEvent>(jsonMa);
            staList objSts = JsonConvert.DeserializeObject<staList>(jsonSts);
            HeadStas objHead = JsonConvert.DeserializeObject<HeadStas>(jsonHead);

            Formation fm = new Formation();
            fm.homeLst = new List<PlayerInfo>();
            fm.awayLst = new List<PlayerInfo>();
            fm.HomeLineLst = new List<LineInfo>();
            fm.AwayLineLst = new List<LineInfo>();
            fm.subHomeLst = new List<PlayerInfo>();
            fm.subAwayLst = new List<PlayerInfo>();
            fm.goalHomeLst = new List<Incidents>();
            fm.goalAwayLst = new List<Incidents>();
            fm.goalLst = new List<Incidents>();
            fm.statsInfo = new List<lineStats>();
            fm.formHmLst = new List<formHmTeam>();
            fm.formAwLst = new List<formAwTeam>();

            fm.photoHome = FlagsConst.PhotoPlay + rid + "/jersey/home/player/image";
            fm.photoAway = FlagsConst.PhotoPlay + rid + "/jersey/away/player/image";
            int lenHome = m.home.Count;
            int lenAway = m.away.Count;
            int lenGo = objGo.Count;
            int lenSt = 0;
            if (objSts.periods != null)
            {
                lenSt = objSts.periods.Count;
            }

            fm.HomeName = objMa.game.tournaments[0].events[0].homeTeam.shortName;
            fm.AwayName = objMa.game.tournaments[0].events[0].awayTeam.shortName;
            long homeId = objMa.game.tournaments[0].events[0].homeTeam.id;
            long awayId = objMa.game.tournaments[0].events[0].awayTeam.id;

            string JsonTextSFoHm = FlagsConst.FormLnk + homeId + "/performance";
            string JsonTextFoAw = FlagsConst.FormLnk + awayId + "/performance";
            using (WebClient wc = new WebClient())
            {
                jsonFoHm = wc.DownloadString(JsonTextSFoHm);
                jsonFoAw = wc.DownloadString(JsonTextFoAw);
            }
            var objHm = JsonConvert.DeserializeObject<dynamic>(jsonFoHm);
            var objAw = JsonConvert.DeserializeObject<dynamic>(jsonFoAw);
            int lenHm = objHm.Count;
            int lenAw = objAw.Count;

            fm.HomeFlag = FlagsConst.ptLink + homeId.ToString() + ".png";
            fm.AwayFlag = FlagsConst.ptLink + awayId.ToString() + ".png";
            try
            {
                string hsc = objMa.game.tournaments[0].events[0].homeScore.current == null ? "?" : objMa.game.tournaments[0].events[0].homeScore.current;
                fm.HomeScore = hsc.ToString();
                string asc = objMa.game.tournaments[0].events[0].awayScore.current == null ? "?" : objMa.game.tournaments[0].events[0].awayScore.current;
                fm.AwayScore = asc.ToString();
            }
            catch (KeyNotFoundException ex)
            {
                fm.HomeScore = "?";
                fm.AwayScore = "?";
            }

            if (lenHome > 0 && lenAway > 0)
            {
                fm.homeFormation = m.homeFormation;
                fm.awayFormation = m.awayFormation;
                fm.lenghHome = chkleng(fm.homeFormation).Count;
                fm.lenghAway = chkleng(fm.awayFormation).Count;
                for (int i = 0; i < lenHome; i++)
                {
                    PlayerInfo pl = new PlayerInfo();
                    pl.PlayerName = m.home[i].player.name;
                    pl.shortName = m.home[i].player.shortName;
                    pl.PlayerId = m.home[i].player.id;
                    try
                    {
                        pl.position = m.home[i].position;
                    }
                    catch (Exception ex)
                    {
                        pl.position = i + 1;
                    }

                    try
                    {
                        string hsc = m.home[i].shirtNumber == null ? "" : m.home[i].shirtNumber;
                        pl.shirtNumber = hsc.ToString();
                    }
                    catch (KeyNotFoundException ex)
                    {
                        pl.shirtNumber = "";
                    }
                    pl.substitute = m.home[i].substitute;
                    pl.positionName = m.home[i].positionName;
                    pl.captain = m.home[i].captain;
                    fm.homeLst.Add(pl);
                }

                for (int j = 0; j < lenAway; j++)
                {
                    PlayerInfo pl = new PlayerInfo();
                    pl.PlayerName = m.away[j].player.name;
                    pl.shortName = m.away[j].player.shortName;
                    pl.PlayerId = m.away[j].player.id;
                    try
                    {
                        pl.position = m.away[j].position;
                    }
                    catch (Exception ex)
                    {
                        pl.position = j + 1;
                    }

                    try
                    {
                        string hsc = m.away[j].shirtNumber == null ? "" : m.away[j].shirtNumber;
                        pl.shirtNumber = hsc.ToString();
                    }
                    catch (KeyNotFoundException ex)
                    {
                        pl.shirtNumber = "";
                    }
                    pl.substitute = m.away[j].substitute;
                    pl.positionName = m.away[j].positionName;
                    pl.captain = m.away[j].captain;
                    fm.awayLst.Add(pl);
                }

                int k = 0, k2 = 0;
                for (int i = 0; i < fm.lenghHome; i++)
                {
                    LineInfo li = new LineInfo();
                    li.PlayerLine = new List<PlayerInfo>();
                    li.lineLengh = chkleng(fm.homeFormation)[i] + k;
                    for (int a = k; a < li.lineLengh; a++)
                    {
                        li.PlayerLine.Add(fm.homeLst[a]);
                    }
                    k = li.lineLengh;
                    fm.HomeLineLst.Add(li);
                }

                for (int i = 0; i < fm.lenghAway; i++)
                {
                    LineInfo li = new LineInfo();
                    li.PlayerLine = new List<PlayerInfo>();
                    li.lineLengh = chkleng(fm.awayFormation)[i] + k2;
                    for (int a = k2; a < li.lineLengh; a++)
                    {
                        li.PlayerLine.Add(fm.awayLst[a]);
                    }
                    k2 = li.lineLengh;
                    fm.AwayLineLst.Add(li);
                }

                for (int i = 11; i < lenHome; i++)
                {
                    fm.subHomeLst.Add(fm.homeLst[i]);
                }

                for (int j = 11; j < lenAway; j++)
                {
                    fm.subAwayLst.Add(fm.awayLst[j]);
                }
            }

            if (lenGo > 0)
            {
                //for (int i = 0; i < lenGo; i++)
                //{
                var neStd = new MatchIncident();

                //int lende = neStd.InciLst.Count;
                for (int a = lenGo - 1; a >= 0; a--)
                {
                    var mt = objGo[a];
                    var convOb = mt as JObject;
                    var tmp = new incide();
                    tmp = convOb.ToObject<incide>();
                    //tmp = neStd.InciLst[a];
                    Incidents idt = new Incidents();
                    idt.InciType = tmp.incidentType;
                    if (idt.InciType == "goal")
                    {
                        idt.time = tmp.time;
                        idt.scoringTeam = tmp.scoringTeam;
                        idt.Player = tmp.player.name;
                        try
                        {
                            string hsc = tmp.from == null ? "" : tmp.from;
                            idt.from = hsc.ToString();
                        }
                        catch (KeyNotFoundException ex)
                        {
                            idt.from = "";
                        }
                        fm.goalLst.Add(idt);
                        //if (idt.scoringTeam == 1)
                        //{
                        //    info.goalHomeLst.Add(idt);
                        //}else
                        //{
                        //    info.goalAwayLst.Add(idt);
                        //}
                    }
                    else if (idt.InciType == "card")
                    {
                        idt.time = tmp.time;
                        idt.scoringTeam = tmp.playerTeam;
                        idt.Player = tmp.player.name;
                        idt.type = tmp.type;
                        fm.goalLst.Add(idt);
                    }
                    else if (idt.InciType == "substitution")
                    {
                        idt.time = tmp.time;
                        idt.scoringTeam = tmp.playerTeam;
                        idt.PlayerIn = tmp.playerIn.name;
                        idt.PlayerOut = tmp.playerOut.name;
                        fm.goalLst.Add(idt);
                    }
                    else if (idt.InciType == "period")
                    {
                        idt.content = tmp.text;
                        fm.goalLst.Add(idt);
                    }
                }

                //}
            }

            if (lenSt > 0)
            {
                int realeng = objSts.periods[0].groups.Count;
                for (int i = 0; i < realeng; i++)
                {
                    lineStats ls = new lineStats();
                    ls.groupName = objSts.periods[0].groups[i].groupName;
                    ls.statItems = new List<Stats>();
                    int lengr = objSts.periods[0].groups[i].statisticsItems.Count;
                    for (int j = 0; j < lengr; j++)
                    {
                        Stats st = new Stats();
                        st.name = objSts.periods[0].groups[i].statisticsItems[j].name;
                        st.home = objSts.periods[0].groups[i].statisticsItems[j].home;
                        st.away = objSts.periods[0].groups[i].statisticsItems[j].away;
                        ls.statItems.Add(st);
                    }
                    fm.statsInfo.Add(ls);
                }
            }

            if (lenHm > 0)
            {
                for (int a = lenHm - 1; a >= lenHm - 10; a--)
                {
                    var mt = objHm[a];
                    var convOb = mt as JObject;
                    var tmp = new formHmTeam();
                    tmp = convOb.ToObject<formHmTeam>();
                    //tmp = neStd.InciLst[a];
                    //Incidents idt = new Incidents();
                    fm.formHmLst.Add(tmp);
                }
            }

            if (lenAw > 0)
            {
                for (int a = lenAw - 1; a >= lenAw - 10; a--)
                {
                    var mt = objAw[a];
                    var convOb = mt as JObject;
                    var tmp = new formAwTeam();
                    tmp = convOb.ToObject<formAwTeam>();
                    //tmp = neStd.InciLst[a];
                    //Incidents idt = new Incidents();
                    fm.formAwLst.Add(tmp);
                }
            }

            if (objHead != null)
            {
                fm.hs2 = new HeadStas();
                fm.hs2.h2h = new headLs();
                fm.hs2.home = new homeLs();
                fm.hs2.away = new awayLs();
                fm.hs2.h2h.events = new evtHead();
                fm.hs2.home.playedAtThisTournament = new evtHead();
                fm.hs2.away.playedAtThisTournament = new evtHead();

                fm.hs2.h2h.events.tournaments = new List<infoTour>();
                fm.hs2.home.playedAtThisTournament.tournaments = new List<infoTour>();
                fm.hs2.away.playedAtThisTournament.tournaments = new List<infoTour>();

                //fm.hs2.h2h = objHead.h2h;
                for (int i = 0; i < objHead.h2h.events.tournaments.Count; i++)
                {
                    infoTour it = new infoTour();
                    it.events = new List<tourMats>();
                    for (int j = 0; j < objHead.h2h.events.tournaments[i].events.Count; j++)
                    {
                        tourMats tm = new tourMats();
                        tm.homeTeam = objHead.h2h.events.tournaments[i].events[j].homeTeam;
                        tm.awayTeam = objHead.h2h.events.tournaments[i].events[j].awayTeam;
                        tm.homeScore = objHead.h2h.events.tournaments[i].events[j].homeScore;
                        tm.awayScore = objHead.h2h.events.tournaments[i].events[j].awayScore;
                        string tam = objHead.h2h.events.tournaments[i].events[j].startTimestamp;
                        tm.startTimestamp = unixConvert(Convert.ToDouble(tam));
                        DateTime ustime = DateTime.ParseExact(tm.startTimestamp, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                        var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                       CultureInfo.InvariantCulture);
                        DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);

                        if (ustime < timeno.AddDays(0.125))
                        {
                            it.events.Add(tm);
                        }
                    }
                    fm.hs2.h2h.events.tournaments.Add(it);
                }
                //fm.hs2.home = objHead.home;
                for (int i = 0; i < objHead.home.playedAtThisTournament.tournaments.Count; i++)
                {
                    infoTour it = new infoTour();
                    it.events = new List<tourMats>();
                    for (int j = 0; j < objHead.home.playedAtThisTournament.tournaments[i].events.Count; j++)
                    {
                        tourMats tm = new tourMats();
                        tm.homeTeam = objHead.home.playedAtThisTournament.tournaments[i].events[j].homeTeam;
                        tm.awayTeam = objHead.home.playedAtThisTournament.tournaments[i].events[j].awayTeam;
                        tm.homeScore = objHead.home.playedAtThisTournament.tournaments[i].events[j].homeScore;
                        tm.awayScore = objHead.home.playedAtThisTournament.tournaments[i].events[j].awayScore;
                        string tam = objHead.home.playedAtThisTournament.tournaments[i].events[j].startTimestamp;
                        tm.startTimestamp = unixConvert(Convert.ToDouble(tam));
                        DateTime ustime = DateTime.ParseExact(tm.startTimestamp, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                        var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                       CultureInfo.InvariantCulture);
                        DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);

                        if (ustime < timeno.AddDays(0.125))
                        {
                            it.events.Add(tm);
                        }
                    }
                    fm.hs2.home.playedAtThisTournament.tournaments.Add(it);
                }

                for (int i = 0; i < objHead.away.playedAtThisTournament.tournaments.Count; i++)
                {
                    infoTour it = new infoTour();
                    it.events = new List<tourMats>();
                    for (int j = 0; j < objHead.away.playedAtThisTournament.tournaments[i].events.Count; j++)
                    {
                        tourMats tm = new tourMats();
                        tm.homeTeam = objHead.away.playedAtThisTournament.tournaments[i].events[j].homeTeam;
                        tm.awayTeam = objHead.away.playedAtThisTournament.tournaments[i].events[j].awayTeam;
                        tm.homeScore = objHead.away.playedAtThisTournament.tournaments[i].events[j].homeScore;
                        tm.awayScore = objHead.away.playedAtThisTournament.tournaments[i].events[j].awayScore;
                        string tam = objHead.away.playedAtThisTournament.tournaments[i].events[j].startTimestamp;
                        tm.startTimestamp = unixConvert(Convert.ToDouble(tam));
                        DateTime ustime = DateTime.ParseExact(tm.startTimestamp, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                        var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                       CultureInfo.InvariantCulture);
                        DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);

                        if (ustime < timeno.AddDays(0.125))
                        {
                            it.events.Add(tm);
                        }
                    }
                    fm.hs2.away.playedAtThisTournament.tournaments.Add(it);
                }
                //fm.hs2.away = objHead.away;
            }
            return fm;
        }

        public List<MatchInfoModel> getInfoSumICC()
        {
            string json = string.Empty;
            string jsonMa = string.Empty;
            string JsonText = FlagsConst.ftLinkICC;
            var spath = TempData.Peek("InfoLst");

            //RoundInfo ri = new RoundInfo();
            rigIc.Schelst = new List<MatchInfoModel>();

            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(JsonText);
            }

            NewObj m = JsonConvert.DeserializeObject<NewObj>(json);

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //var obj = jss.Deserialize<dynamic>(json);
            //int len = obj["tournaments"].Length;
            int len = m.tournaments.Count;

            for (int i = 0; i < len; i++)
            {
                int lende = m.tournaments[i].events.Count;
                for (int j = 0; j < lende; j++)
                {
                    var nema = new NewMatchInfo();
                    nema = m.tournaments[i].events[j];
                    var match = new MatchInfoModel();
                    match.ore = new SumLst();
                    match.MatchId = nema.id;
                    //string JsonTextMa = FlagsConst.matchLnk + match.MatchId + "/details";
                    //using (WebClient wc = new WebClient())
                    //{
                    //    jsonMa = wc.DownloadString(JsonTextMa);
                    //}
                    //var objma = jss.Deserialize<dynamic>(jsonMa);
                    try
                    {
                        int hsc = nema.status.code;
                        match.status = hsc.ToString();
                        string asc = nema.statusDescription == null ? "?" : nema.statusDescription;
                        match.timeIn = asc.ToString();
                    }
                    catch (KeyNotFoundException ex)
                    {
                        match.status = "";
                        match.timeIn = "";
                    }

                    match.HomeTeam = nema.homeTeam.name;
                    match.AwayTeam = nema.awayTeam.name;
                    match.HomeId = nema.homeTeam.id;
                    match.AwayId = nema.awayTeam.id;
                    match.HomeFlag = FlagsConst.ptLink + match.HomeId.ToString() + ".png";
                    match.AwayFlag = FlagsConst.ptLink + match.AwayId.ToString() + ".png";

                    double time = nema.startTimestamp;
                    match.timeSt = unixConvert(time);
                    DateTime ustime = DateTime.ParseExact(match.timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                   CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmp = timeno.AddDays(1);
                    if (ustime > timeno)
                    {
                        if ((ustime >= timeno) && (ustime < timeno.AddDays(2)))
                        {
                            match.ore = getEuOdd(match.MatchId);
                            match.HomeScore = "?";
                            match.AwayScore = "?";
                        }
                    }
                    else
                    {
                        try
                        {
                            int hsc = nema.homeScore.current;
                            match.HomeScore = hsc.ToString();
                            int asc = nema.awayScore.current;
                            match.AwayScore = asc.ToString();
                        }
                        catch (KeyNotFoundException ex)
                        {
                            match.HomeScore = "?";
                            match.AwayScore = "?";
                        }
                    }

                    //var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                    //                   CultureInfo.InvariantCulture);
                    //DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    //var tmp = timeno.AddDays(1);

                    match.RoundInfo = nema.roundInfo.round;

                    rigIc.Schelst.Add(match);
                }
            }
            spath = rigIc.Schelst;
            TempData["InfoLst"] = spath;
            return rigIc.Schelst;
        }

        public List<MatchInfoModel> getInfoSumUnl()
        {
            string json = string.Empty;
            string jsonMa = string.Empty;
            string JsonText = FlagsConst.ftLinkUnl;
            var spath = TempData.Peek("InfoLst3");

            //RoundInfo ri = new RoundInfo();
            rigIc.Schelst = new List<MatchInfoModel>();

            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(JsonText);
            }

            NewObj m = JsonConvert.DeserializeObject<NewObj>(json);

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //var obj = jss.Deserialize<dynamic>(json);
            //int len = obj["tournaments"].Length;
            int len = m.tournaments.Count;

            for (int i = 0; i < len; i++)
            {
                int lende = m.tournaments[i].events.Count;
                for (int j = 0; j < lende; j++)
                {
                    var nema = new NewMatchInfo();
                    nema = m.tournaments[i].events[j];
                    var match = new MatchInfoModel();
                    match.ore = new SumLst();
                    match.MatchId = nema.id;
                    //string JsonTextMa = FlagsConst.matchLnk + match.MatchId + "/details";
                    //using (WebClient wc = new WebClient())
                    //{
                    //    jsonMa = wc.DownloadString(JsonTextMa);
                    //}
                    //var objma = jss.Deserialize<dynamic>(jsonMa);
                    try
                    {
                        int hsc = nema.status.code;
                        match.status = hsc.ToString();
                        string asc = nema.statusDescription == null ? "?" : nema.statusDescription;
                        match.timeIn = asc.ToString();
                    }
                    catch (KeyNotFoundException ex)
                    {
                        match.status = "";
                        match.timeIn = "";
                    }

                    match.HomeTeam = nema.homeTeam.name;
                    match.AwayTeam = nema.awayTeam.name;
                    match.HomeId = nema.homeTeam.id;
                    match.AwayId = nema.awayTeam.id;
                    match.HomeFlag = FlagsConst.ptLink + match.HomeId.ToString() + ".png";
                    match.AwayFlag = FlagsConst.ptLink + match.AwayId.ToString() + ".png";

                    double time = nema.startTimestamp;
                    match.timeSt = unixConvert(time);
                    DateTime ustime = DateTime.ParseExact(match.timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                   CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmp = timeno.AddDays(1);
                    if (ustime > timeno)
                    {
                        if ((ustime >= timeno) && (ustime < timeno.AddDays(2)))
                        {
                            match.ore = getEuOdd(match.MatchId);
                            match.HomeScore = "?";
                            match.AwayScore = "?";
                        }
                        else if ((ustime >= timeno) && (ustime < timeno.AddDays(5)))
                        {
                            match.ore = getEuOdd(match.MatchId);
                            match.HomeScore = "?";
                            match.AwayScore = "?";
                        }
                    }

                    else
                    {
                        try
                        {
                            var tm = ustime.AddDays(0.125);
                            if (timeno > ustime && timeno < tm)
                            {
                                match.ore = getEuOdd(match.MatchId);
                            }
                            int hsc = nema.homeScore.current;
                            match.HomeScore = hsc.ToString();
                            int asc = nema.awayScore.current;
                            match.AwayScore = asc.ToString();
                        }
                        catch (KeyNotFoundException ex)
                        {
                            match.HomeScore = "?";
                            match.AwayScore = "?";
                        }
                    }

                    //var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                    //                   CultureInfo.InvariantCulture);
                    //DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    //var tmp = timeno.AddDays(1);

                    match.RoundInfo = nema.roundInfo.round;

                    rigIc.Schelst.Add(match);
                }
            }
            spath = rigIc.Schelst;
            TempData["InfoLst3"] = spath;
            return rigIc.Schelst;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult Wc18Detail(string tourId)
        {
            var spa = TempData.Peek("Tour");
            spa = tourId;
            TempData["Tour"] = spa;
            ViewBag.Message = "Thông tin giảỉ đấu";
            return View("_Wc18InfoPartial", matches);
        }

        [HttpGet]
        public async Task<ActionResult> MatchInfo18()
        {
            string catTour = (string)TempData["Tour"];
            TempData.Keep();
            //if (TempData.ContainsKey("InfoLst"))
            //    matGlo = (List<MatchInfoModel>)TempData["InfoLst"];
            string json = string.Empty;
            string jsonMa = string.Empty;
            string JsonText = "";

            switch (catTour)
            {
                case "Prm":
                    JsonText = FlagsConst.ftLinkPrem;
                    break;
                case "Llg":
                    JsonText = FlagsConst.ftLinkLlg;
                    break;
                case "Bdla":
                    JsonText = FlagsConst.ftLinkBdla;
                    break;
                case "Sra":
                    JsonText = FlagsConst.ftLinkSra;
                    break;
                case "Lg1":
                    JsonText = FlagsConst.ftLinkLg1;
                    break;
                case "C1":
                    JsonText = FlagsConst.ftLinkCl;
                    break;
                case "Eup":
                    JsonText = FlagsConst.ftLinkEup;
                    break;
            }
            getInfoSum(JsonText, catTour);
            List<MatchInfoModel> matlst = new List<MatchInfoModel>();
            matlst = (List<MatchInfoModel>)TempData["InfoLst2"];
            matches = matlst;

            WcInfoModel info = new WcInfoModel();
            info.RoundLst = new List<RoundInfo>();

            //using (StreamReader reader = new StreamReader(JsonText))
            //{
            //    json = reader.ReadToEnd();
            //}
            int len2 = matches.Count;

            using (WebClient wc = new WebClient())
            {
                json = await wc.DownloadStringTaskAsync(JsonText);
            }

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //var obj = jss.Deserialize<dynamic>(json);
            //int len = obj["tournaments"].Length;
            int maxRnd = 1;
            int lenr = 0;
            var dict = new Dictionary<int, int>();
            info.name = (string)TempData["tmpSS"];
            int len = (int)TempData["count"];

            //for (int i = 0; i < len; i++)
            //{
            //int lende = obj["tournaments"][i]["events"].Length;
            for (int j = 0; j < len2; j++)
            {
                var match = new MatchInfoModel();
                match.MatchId = matches[j].MatchId;
                //string JsonTextMa = FlagsConst.matchLnk + match.MatchId + "/details";
                //using (WebClient wc = new WebClient())
                //{
                //    jsonMa = await wc.DownloadStringTaskAsync(JsonTextMa);
                //}
                //var objma = jss.Deserialize<dynamic>(jsonMa);
                //match.status = objma["game"]["tournaments"][0]["events"][0]["status"]["name"];
                //match.timeIn = objma["game"]["tournaments"][0]["events"][0]["statusDescription"];
                try
                {
                    string hsc = matches[j].status == null ? "?" : matches[j].status;
                    match.status = hsc.ToString();
                    string asc = matches[j].timeIn == null ? "?" : matches[j].timeIn;
                    match.timeIn = asc.ToString();
                }
                catch (KeyNotFoundException ex)
                {
                    match.status = "";
                    match.timeIn = "";
                }

                match.HomeTeam = matches[j].HomeTeam;
                match.AwayTeam = matches[j].AwayTeam;
                //HomeFlag = obj["object"]["tournaments"]["events"][i]["firstName"],
                //AwayFlag = obj["object"]["tournaments"]["events"][i]["firstName"],
                match.HomeId = matches[j].HomeId;
                match.AwayId = matches[j].AwayId;
                match.HomeFlag = FlagsConst.ptLink + match.HomeId.ToString() + ".png";
                match.AwayFlag = FlagsConst.ptLink + match.AwayId.ToString() + ".png";
                try
                {
                    string hsc = matches[j].HomeScore == null ? "?" : matches[j].HomeScore;
                    match.HomeScore = hsc.ToString();
                    string asc = matches[j].AwayScore == null ? "?" : matches[j].AwayScore;
                    match.AwayScore = asc.ToString();
                }
                catch (KeyNotFoundException ex)
                {
                    match.HomeScore = "?";
                    match.AwayScore = "?";
                }
                //double time = obj["tournaments"][i]["events"][j]["startTimestamp"];
                match.timeSt = matches[j].timeSt;
                //StatusMatch = obj["object"]["tournaments"]["events"][i]["firstName"],
                match.RoundInfo = matches[j].RoundInfo;
                //TimeMatch = obj["object"]["tournaments"]["events"][i]["firstName"],
                //DateMatch = obj["object"]["tournaments"]["events"][i]["firstName"]
                if (maxRnd >= match.RoundInfo && (j != len2 - 1))
                {
                    lenr++;
                }
                else if (j == len2 - 1)
                {
                    lenr++;
                    dict.Add(maxRnd, lenr);
                }
                else
                {
                    dict.Add(maxRnd, lenr);
                    maxRnd = match.RoundInfo;
                    lenr++;
                }
                matches.Add(match);
            }
            //i++;
            //}
            int k = 0;
            //int tm = 0;
            //int lenr = 10;
            foreach (var pa in dict)
            {
                int key = pa.Key;
                int val = pa.Value;
                //tm = tm + val;
                RoundInfo ri = new RoundInfo();
                ri.RoundId = key;
                ri.Schelst = new List<MatchInfoModel>();
                ri.RoundName = "Vòng " + key;
                for (int a = k; a < val; a++)
                {
                    ri.Schelst.Add(matches[a]);
                }
                k = val;
                info.RoundLst.Add(ri);
            }

            //for (int i = 1; i <= maxRnd; i++)
            //{
            //    RoundInfo ri = new RoundInfo();
            //    ri.RoundId = i;
            //    ri.Schelst = new List<MatchInfoModel>();
            //    ri.RoundName = "Vòng " + i;
            //    for (int a = k; a < lenr; a++)
            //    {
            //        ri.Schelst.Add(matches[a]);
            //    }
            //    k = lenr + 1;
            //    lenr = lenr + 10;
            //    info.RoundLst.Add(ri);
            //}
            return PartialView("_MatchInfo18", info);
        }

        public ActionResult Standing18()
        {
            string catTour = (string)TempData["Tour"];
            //ViewBag.Message = "World cup 2018 Info";
            string json = string.Empty;
            string JsonText = "";
            switch (catTour)
            {
                case "Prm":
                    JsonText = FlagsConst.stdLinkPrem;
                    break;
                case "Llg":
                    JsonText = FlagsConst.stdLinkLlg;
                    break;
                case "Bdla":
                    JsonText = FlagsConst.stdLinkBdla;
                    break;
                case "Sra":
                    JsonText = FlagsConst.stdLinkSra;
                    break;
                case "Lg1":
                    JsonText = FlagsConst.stdLinkLg1;
                    break;
                case "C1":
                    JsonText = FlagsConst.stdLinkC1;
                    break;
                case "Eup":
                    JsonText = FlagsConst.stdLinkEup;
                    break;
            }
            MatchInfoModel mim = new MatchInfoModel();
            mim.StdGroup = new List<StandingModel>();
            //using (StreamReader reader = new StreamReader(JsonText))
            //{
            //    json = reader.ReadToEnd();
            //}

            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(JsonText);
            }

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //var obj = jss.Deserialize<dynamic>(json);
            //int len = obj.Length;
            var StArr2 = JsonConvert.DeserializeObject<dynamic>(json);
            int len = StArr2.Count;
            for (int i = 0; i < len; i++)
            {
                var m = StArr2[i];
                var convOb = m as JObject;
                var neStd = new tableObj();
                neStd = convOb.ToObject<tableObj>();
                StandingModel std = new StandingModel();
                //std.Stt = i + 1;
                std.name = neStd.name;
                std.id = neStd.id;
                std.listStand = new List<GroupTeam>();
                //int lende = obj[i]["tableRows"].Length;
                int lende = neStd.tableRows.Count;
                for (int j = 0; j < lende; j++)
                {
                    var tmp = new DetailTeam();
                    tmp = neStd.tableRows[j];
                    var match = new GroupTeam();
                    match.TeamName = tmp.team.shortName;
                    match.TeamId = tmp.team.id;
                    match.TeamFlag = FlagsConst.ptLink + match.TeamId.ToString() + ".png";
                    match.Position = tmp.position;
                    match.matchesTotal = tmp.totalFields.matchesTotal;
                    match.winTotal = tmp.totalFields.winTotal;
                    match.drawTotal = tmp.totalFields.drawTotal;
                    match.lossTotal = tmp.totalFields.lossTotal;
                    match.goalsTotal = tmp.totalFields.goalsTotal;
                    match.goalDiffTotal = tmp.totalFields.goalDiffTotal;
                    match.pointsTotal = tmp.totalFields.pointsTotal;

                    std.listStand.Add(match);
                }
                mim.StdGroup.Add(std);
            }
            return PartialView("_Standing18", mim);
        }

        public string unixConvert(double x)
        {
            System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dateTime = dateTime.AddSeconds(x);
            string printDate = dateTime.ToLocalTime().ToString("dd/MM/yyyy", null) + " " + dateTime.ToLocalTime().ToString("HH:mm tt"); ;
            return printDate;
        }

        [HttpGet]
        public ActionResult MatchDetail(string match)
        {
            if (match.Contains("^"))
            {
                var tem = TempData.Peek("mat");
                tem = match;
                TempData["mat"] = tem;
            }
            else
            {
                match = (string)TempData["mat"];
            }
            int pos = match.IndexOf("^");
            long rid = Int64.Parse(match.Substring(pos + 1));
            Formation info = new Formation();
            info = getMatch(rid);

            return View("MatchDetail", info);
        }

        public List<int> chkleng(string str)
        {
            List<int> kq = new List<int>();
            kq.Add(1);
            //var x = str.ToArray();
            foreach (var i in str)
            {
                if (Char.IsNumber(i))
                {
                    kq.Add(int.Parse(i.ToString()));
                }
            }
            return kq;
        }

        //[HttpGet]
        //public ActionResult SearchBoard()
        //{
        //    //Get_Connection();
        //    newslist nl = new newslist();
        //    nl.ctLst = new List<ContentModel>();
        //    string sql = @"select a.news_id, a.title, a.description, a.image, a.hot, b.category_name 
        //                   from news a 
        //                   join category b on a.cat_id = b.category_id
        //                   where a.cat_id = 1
        //                   order by a.news_id desc  
        //                   limit 10 ";
        //    connection.Open();
        //    int count = 0;

        //    try
        //    {
        //        cmd.Connection = connection;
        //        cmd.CommandText = sql;
        //        using (DbDataReader reader = cmd.ExecuteReader())
        //        {
        //            if (reader.HasRows)
        //            {
        //                while (reader.Read())
        //                {
        //                    ++count;
        //                    ContentModel cm = new ContentModel();
        //                    cm.news_id = Convert.ToInt64(reader.GetValue(0));
        //                    cm.title = reader.GetString(1);
        //                    cm.descp = reader.GetString(2);
        //                    try
        //                    {
        //                        cm.imageLnk = reader.GetString(3);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        cm.imageLnk = "";
        //                    }
        //                    cm.hot = Int32.Parse(reader.GetValue(4).ToString());
        //                    cm.cat_name = reader.GetString(5);
        //                    cm.Stt = count;
        //                    //cm.category_id = Convert.ToInt64(reader.GetValue(0));
        //                    nl.ctLst.Add(cm);
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error: " + sql);
        //        Console.WriteLine(e.StackTrace);
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //    return PartialView("_NewsBoard", nl);
        //}

        [HttpGet]
        public ActionResult SearchBoard()
        {
            //Get_Connection();
            string json = string.Empty;
            string JsonText = FlagsConst.newsJs + "latest";

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
                if (x.image == "")
                {
                    x.image = "/Upload/brp411251.jpg";
                }
                cm.imageLnk = x.image;
                cm.origin_url = x.link;
                cm.source = x.siteName;
                cm.Stt = count;
                nl.ctLst.Add(cm);
            }

            return PartialView("_NewsBoard", nl);
        }

        public string procEnc(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str.ToString().Trim());
            string myString = Encoding.UTF8.GetString(bytes);
            return myString;
        }

        //[HttpGet]
        //public ActionResult NewsDetail(string news)
        //{
        //    int pos = news.IndexOf("^");
        //    long rid = Int64.Parse(news.Substring(pos + 1));
        //    ContentModel cm = new ContentModel();
        //    string sql = @"select a.news_id, a.title, a.description, a.image, a.content, b.category_name, a.source 
        //                   from news a 
        //                   join category b on a.cat_id = b.category_id
        //                   where a.news_id = " + rid;
        //    connection.Open();

        //    try
        //    {
        //        cmd.Connection = connection;
        //        cmd.CommandText = sql;
        //        using (DbDataReader reader = cmd.ExecuteReader())
        //        {
        //            if (reader.HasRows)
        //            {
        //                while (reader.Read())
        //                {
        //                    cm.news_id = Convert.ToInt64(reader.GetValue(0));
        //                    cm.title = reader.GetString(1);
        //                    cm.descp = reader.GetString(2);
        //                    cm.content = reader.GetString(4);

        //                    try
        //                    {
        //                        cm.imageLnk = reader.GetString(3);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        cm.imageLnk = "";
        //                    }
        //                    cm.cat_name = reader.GetString(5);
        //                    cm.source = reader.GetString(6);

        //                    //cm.category_id = Convert.ToInt64(reader.GetValue(0));
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Error: " + sql);
        //        Console.WriteLine(e.StackTrace);
        //    }
        //    finally
        //    {
        //        connection.Close();
        //        //connection.Dispose();
        //        //connection = null;
        //    }
        //    return View("NewsDetail", cm);
        //}
        [HttpGet]
        public ActionResult NewsDetail(string news)
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
            string json2 = string.Empty;

            string JsonText = FlagsConst.detailJs + rid;
            string JsonTe = FlagsConst.newsJs + "latest";

            using (WebClient wc = new WebClient())
            {
                wc.Headers["Content-Type"] = "application/json;charset=UTF-8";
                json = wc.DownloadString(JsonText);
                wc.Headers["Content-Type"] = "application/json;charset=UTF-8";
                json2 = wc.DownloadString(JsonTe);
            }

            detailData objGo = JsonConvert.DeserializeObject<detailData>(json);
            LatData objLa = JsonConvert.DeserializeObject<LatData>(json2);
            cm.relateDt = new List<memDt>();
            cm.relateDt = objLa.data.Where(x=>x.link != rid).ToList();

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
                        mp.linkVd = x.link.Split(',').ToList();
                        //int ps = x.link.IndexOf(",");
                        //var secWor = x.link.Substring(0, ps);
                        //var secWor2 = x.link.Substring(ps + 1, x.link.Length - ps - 1);
                        //string lnk1 = secWor.ToString();
                        //string lnk2 = secWor2.ToString();
                        //mp.linkVd.Add(lnk1);
                        //mp.linkVd.Add(lnk2);
                    }
                    else
                    {
                        mp.linkVd.Add(x.link);
                    }
                }
                cm.data.content.Add(mp);
            }

            //foreach (var x in objGo.data.content)
            //{
            //    cm.title = procEnc(x.title);
            //    cm.descp = procEnc(x.description);
            //    cm.hot = 0;
            //    cm.imageLnk = x.image;
            //    cm.origin_url = x.link;
            //    cm.source = x.siteName;
            //}
            return View("NewsDetail", cm);
        }

        //        [HttpGet]
        //        public ActionResult FullNews(int? page)
        //        {
        //            newslist nl = new newslist();
        //            nl.ctLst = new List<ContentModel>();
        //            string sql = @"select a.news_id, a.title, a.description, a.image, b.category_name 
        //                           from news a 
        //                           join category b on a.cat_id = b.category_id
        //                           where category_id = 1
        //                           order by a.news_id desc";
        //            connection.Open();
        //            int count = 0;

        //            try
        //            {
        //                cmd.Connection = connection;
        //                cmd.CommandText = sql;
        //                using (DbDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            ++count;
        //                            ContentModel cm = new ContentModel();
        //                            cm.news_id = Convert.ToInt64(reader.GetValue(0));
        //                            cm.title = reader.GetString(1);
        //                            cm.descp = reader.GetString(2);
        //                            try
        //                            {
        //                                cm.imageLnk = reader.GetString(3);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                cm.imageLnk = "";
        //                            }
        //                            cm.cat_name = reader.GetString(4);
        //                            cm.Stt = count;
        //                            //cm.category_id = Convert.ToInt64(reader.GetValue(0));
        //                            nl.ctLst.Add(cm);
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("Error: " + sql);
        //                Console.WriteLine(e.StackTrace);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //                //connection.Dispose();
        //                //connection = null;
        //            }
        //            int pageSize = 10;
        //            int pageNumber = (page ?? 1);
        //            return View("FullNews", nl.ctLst.ToPagedList(pageNumber, pageSize));
        //        }

        [HttpGet]
        public ActionResult FullNews(int? page)
        {
            string json = string.Empty;
            string JsonText = FlagsConst.newsJs + "latest";

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
                if (x.image == "")
                {
                    x.image = "/Upload/brp411251.jpg";
                }
                cm.imageLnk = x.image;
                cm.origin_url = x.link;
                cm.source = x.siteName;
                cm.Stt = count;
                nl.ctLst.Add(cm);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View("FullNews", nl.ctLst.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult SoiKeo(string mats)
        {
            //List<OddsRate> AsLst = (List<OddsRate>)TempData["parsing"];
            //TempData.Keep();
            //return RedirectToAction("Index", "SoiKeo", new { mats = mats, lst = AsLst });
            List<OddsRate> AsLst = (List<OddsRate>)TempData["parsing"];
            TempData.Keep();

            string time = "";
            string home = "";
            string away = "";
            if (mats != null && AsLst.Count > 0)
            {
                int p1 = mats.IndexOf("+");
                int p2 = mats.LastIndexOf("+");
                int p3 = mats.IndexOf("-");

                if (p1 == -1 || p2 == -1)
                {
                    var arp = (int[])TempData["pos"];
                    p1 = arp[0];
                    p2 = arp[1];
                }

                time = mats.Substring(0, p1);
                home = mats.Substring(p1 + 1, p2 - p1 - 1);
                away = mats.Substring(p2 + 1, p3 - p2 - 1);
                int[] arr1 = new int[] { p1, p2, p3 };

                var tem = TempData.Peek("match");
                tem = mats;
                TempData["match"] = tem;

                var temPs = TempData.Peek("parse");
                temPs = AsLst;
                TempData["parse"] = temPs;

                var Pos = TempData.Peek("pos");
                Pos = arr1;
                TempData["pos"] = Pos;
            }
            else
            {
                mats = (string)TempData["match"];
                AsLst = (List<OddsRate>)TempData["parse"];

                int p1 = mats.IndexOf("+");
                int p2 = mats.LastIndexOf("+");
                int p3 = mats.IndexOf("-");

                if (p1 == -1 || p2 == -1)
                {
                    var arp = (int[])TempData["pos"];
                    p1 = arp[0];
                    p2 = arp[1];
                }

                time = mats.Substring(0, p1);
                home = mats.Substring(p1 + 1, p2 - p1 - 1);
                away = mats.Substring(p2 + 1, p3 - p2 - 1);
                int[] arr1 = new int[] { p1, p2, p3 };

                var tem = TempData.Peek("match");
                tem = mats;
                TempData["match"] = tem;

                var temPs = TempData.Peek("parse");
                temPs = AsLst;
                TempData["parse"] = temPs;

                var Pos = TempData.Peek("pos");
                Pos = arr1;
                TempData["pos"] = Pos;
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
            int hsc = objma["game"]["tournaments"][0]["events"][0]["status"]["code"] == null ? "?" : objma["game"]["tournaments"][0]["events"][0]["status"]["code"];

            CatOdds stdAs = new CatOdds();
            stdAs.OddLst = new List<OddsRate>();

            foreach (var ma in AsLst)
            {
                bool check1 = false, check3 = false, check4 = false;
                DateTime detime = DateTime.ParseExact(ma.timeMt, "HH:mm", CultureInfo.InvariantCulture);
                string name3 = detime.AddHours(-1).ToString("HH:mm");

                if (timevr == name3)
                {
                    check1 = true;
                    home = convertToUnSign3(home);
                    if (home.ToLower().Contains(ma.HomeTe.ToLower()) || home.ToLower().Contains(ma.SubHomeTe.ToLower()))
                    {
                        check3 = true;
                        away = convertToUnSign3(away);
                        if (away.ToLower().Contains(ma.AwayTe.ToLower()) || away.ToLower().Contains(ma.SubAwayTe.ToLower()))
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

            //return Json(ct, JsonRequestBehavior.AllowGet);
            return View(ct);
        }

        //        [HttpGet]
        //        public ActionResult SearchIdea()
        //        {
        //            //Get_Connection();
        //            newslist nl = new newslist();
        //            nl.ctLst = new List<ContentModel>();
        //            string sql = @"select a.news_id, a.title, a.description, a.image, a.hot, b.category_name 
        //                           from news a 
        //                           join category b on a.cat_id = b.category_id
        //                           where a.cat_id = 3
        //                           order by a.news_id desc  
        //                           limit 10 ";
        //            connection.Open();
        //            int count = 0;

        //            try
        //            {
        //                cmd.Connection = connection;
        //                cmd.CommandText = sql;
        //                using (DbDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            ++count;
        //                            ContentModel cm = new ContentModel();
        //                            cm.news_id = Convert.ToInt64(reader.GetValue(0));
        //                            cm.title = reader.GetString(1);
        //                            cm.descp = reader.GetString(2);
        //                            try
        //                            {
        //                                cm.imageLnk = reader.GetString(3);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                cm.imageLnk = "";
        //                            }
        //                            cm.hot = Int32.Parse(reader.GetValue(4).ToString());
        //                            cm.cat_name = reader.GetString(5);
        //                            cm.Stt = count;
        //                            //cm.category_id = Convert.ToInt64(reader.GetValue(0));
        //                            nl.ctLst.Add(cm);
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("Error: " + sql);
        //                Console.WriteLine(e.StackTrace);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //                //connection.Dispose();
        //                //connection = null;
        //            }
        //            return PartialView("_IdeaBoard", nl);
        //        }

        [HttpGet]
        public ActionResult SearchIdea()
        {
            //Get_Connection();
            string json = string.Empty;
            string JsonText = FlagsConst.newsJs + "opinion";

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
                if (x.description == "")
                {
                    x.description = "Những nhận định, soi kèo sát nhất của các chuyên gia về trận đấu này";
                }
                cm.descp = x.description;
                cm.hot = 0;
                if (x.image == "")
                {
                    x.image = "/Upload/soikeo.jpg";
                }
                cm.imageLnk = x.image;
                cm.origin_url = x.link;
                cm.source = x.siteName;
                cm.Stt = count;
                nl.ctLst.Add(cm);
            }
            return PartialView("_IdeaBoard", nl);
        }

        //        [HttpGet]
        //        public ActionResult SearchVideo()
        //        {
        //            //Get_Connection();
        //            newslist nl = new newslist();
        //            nl.ctLst = new List<ContentModel>();
        //            string sql = @"select a.news_id, a.title, a.description, a.image, a.hot, b.category_name 
        //                           from news a 
        //                           join category b on a.cat_id = b.category_id
        //                           where a.cat_id = 2
        //                           order by a.news_id desc  
        //                           limit 10 ";
        //            connection.Open();
        //            int count = 0;

        //            try
        //            {
        //                cmd.Connection = connection;
        //                cmd.CommandText = sql;
        //                using (DbDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            ++count;
        //                            ContentModel cm = new ContentModel();
        //                            cm.news_id = Convert.ToInt64(reader.GetValue(0));
        //                            cm.title = reader.GetString(1);
        //                            cm.descp = reader.GetString(2);
        //                            try
        //                            {
        //                                cm.imageLnk = reader.GetString(3);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                cm.imageLnk = "";
        //                            }
        //                            cm.hot = Int32.Parse(reader.GetValue(4).ToString());
        //                            cm.cat_name = reader.GetString(5);
        //                            cm.Stt = count;
        //                            //cm.category_id = Convert.ToInt64(reader.GetValue(0));
        //                            nl.ctLst.Add(cm);
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("Error: " + sql);
        //                Console.WriteLine(e.StackTrace);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //                //connection.Dispose();
        //                //connection = null;
        //            }
        //            return PartialView("_VideoBoard", nl);
        //        }

        [HttpGet]
        public ActionResult SearchVideo()
        {
            //Get_Connection();
            string json = string.Empty;
            string JsonText = FlagsConst.newsJs + "video";

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
                if (x.link.Contains("highlights") && x.description == "")
                {
                    x.description = "Video Highlights Trận Đấu";
                }
                cm.descp = x.description;
                cm.hot = 0;
                if (x.image == "")
                {
                    x.image = "/Upload/football_soccer_highlight.jpg";
                }
                cm.imageLnk = x.image;
                cm.origin_url = x.link;
                cm.source = x.siteName;
                cm.Stt = count;
                nl.ctLst.Add(cm);
            }
            return PartialView("_VideoBoard", nl);
        }

        //        [HttpGet]
        //        public ActionResult SearchEng()
        //        {
        //            //Get_Connection();
        //            newslist nl = new newslist();
        //            nl.ctLst = new List<ContentModel>();
        //            string sql = @"select a.news_id, a.title, a.description, a.image, a.hot, b.category_name 
        //                           from news a 
        //                           join category b on a.cat_id = b.category_id
        //                           where lower(a.description) like lower(N'%Ngoại Hạng%') or lower(a.description) like lower(N'%giải Anh%')
        //                           or lower(a.description) like lower(N'%premier league%') or lower(a.description) like lower(N'%liverpool%') or lower(a.description) like lower(N'%arsenal%')
        //                           or lower(a.description) like lower(N'%chelsea%') or lower(a.description) like lower(N'%man city%') or lower(a.description) like lower(N'%manchester united%')
        //                           or lower(a.description) like lower(N'%man united%') or lower(a.description) like lower(N'%tottenham%') or lower(a.description) like lower(N'%aguero%') 
        //                           or lower(a.description) like lower(N'%guardiola%')  or lower(a.description) like lower(N'%emery%')  or lower(a.description) like lower(N'%klopp%') 
        //                           or lower(a.description) like lower(N'%mourinho%') or lower(a.description) like lower(N'%hazard%')  or lower(a.description) like lower(N'%morata%') 
        //                           or lower(a.description) like lower(N'%pogba%')  or lower(a.description) like lower(N'%sanchez%')  or lower(a.description) like lower(N'%ozil%')
        //                           or lower(a.description) like lower(N'%ramsey%') or lower(a.description) like lower(N'%auba%')  or lower(a.description) like lower(N'%salah%')
        //                           or lower(a.description) like lower(N'%kane%')  or lower(a.description) like lower(N'%son%')  or lower(a.description) like lower(N'%de bruyne%') 
        //                           or lower(a.description) like lower(N'%eriksen%')                           
        //                           order by a.news_id desc  
        //                           limit 10 ";
        //            connection.Open();
        //            int count = 0;

        //            try
        //            {
        //                cmd.Connection = connection;
        //                cmd.CommandText = sql;
        //                using (DbDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            ++count;
        //                            ContentModel cm = new ContentModel();
        //                            cm.news_id = Convert.ToInt64(reader.GetValue(0));
        //                            cm.title = reader.GetString(1);
        //                            cm.descp = reader.GetString(2);
        //                            try
        //                            {
        //                                cm.imageLnk = reader.GetString(3);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                cm.imageLnk = "";
        //                            }
        //                            cm.hot = Int32.Parse(reader.GetValue(4).ToString());
        //                            cm.cat_name = reader.GetString(5);
        //                            cm.Stt = count;
        //                            //cm.category_id = Convert.ToInt64(reader.GetValue(0));
        //                            nl.ctLst.Add(cm);
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("Error: " + sql);
        //                Console.WriteLine(e.StackTrace);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //                //connection.Dispose();
        //                //connection = null;
        //            }
        //            return PartialView("_England", nl);
        //        }

        [HttpGet]
        public ActionResult SearchEng()
        {
            //Get_Connection();
            string json = string.Empty;
            string JsonText = FlagsConst.newsJs + "epl";

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
            return PartialView("_England", nl);
        }

        //        [HttpGet]
        //        public ActionResult SearchCham()
        //        {
        //            //Get_Connection();
        //            newslist nl = new newslist();
        //            nl.ctLst = new List<ContentModel>();
        //            string sql = @"select a.news_id, a.title, a.description, a.image, a.hot, b.category_name 
        //                           from news a 
        //                           join category b on a.cat_id = b.category_id
        //                           where LOWER(a.description) like LOWER('%champions league%') or LOWER(a.description) like LOWER('%c1%')
        //                           or LOWER(a.title) like LOWER('%champions league%') or LOWER(a.title) like LOWER('%c1%')
        //                           order by a.news_id desc  
        //                           limit 10 ";
        //            connection.Open();
        //            int count = 0;

        //            try
        //            {
        //                cmd.Connection = connection;
        //                cmd.CommandText = sql;
        //                using (DbDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            ++count;
        //                            ContentModel cm = new ContentModel();
        //                            cm.news_id = Convert.ToInt64(reader.GetValue(0));
        //                            cm.title = reader.GetString(1);
        //                            cm.descp = reader.GetString(2);
        //                            try
        //                            {
        //                                cm.imageLnk = reader.GetString(3);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                cm.imageLnk = "";
        //                            }
        //                            cm.hot = Int32.Parse(reader.GetValue(4).ToString());
        //                            cm.cat_name = reader.GetString(5);
        //                            cm.Stt = count;
        //                            //cm.category_id = Convert.ToInt64(reader.GetValue(0));
        //                            nl.ctLst.Add(cm);
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("Error: " + sql);
        //                Console.WriteLine(e.StackTrace);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //                //connection.Dispose();
        //                //connection = null;
        //            }
        //            return PartialView("_Champion", nl);
        //        }

        [HttpGet]
        public ActionResult SearchCham()
        {
            //Get_Connection();
            string json = string.Empty;
            string JsonText = FlagsConst.newsJs + "c1";

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
            return PartialView("_Champion", nl);
        }

        //        [HttpGet]
        //        public ActionResult SearchErp()
        //        {
        //            //Get_Connection();
        //            newslist nl = new newslist();
        //            nl.ctLst = new List<ContentModel>();
        //            string sql = @"select a.news_id, a.title, a.description, a.image, a.hot, b.category_name 
        //                           from news a 
        //                           join category b on a.cat_id = b.category_id
        //                           where lower(a.description) like lower('%europa%') or LOWER(a.description) like LOWER('%c2%') or LOWER(a.description) like LOWER('%c3%')
        //                           or lower(a.title) like lower('%europa%') or LOWER(a.title) like LOWER('%c2%') or LOWER(a.title) like LOWER('%c3%')
        //                           order by a.news_id desc  
        //                           limit 10 ";
        //            connection.Open();
        //            int count = 0;

        //            try
        //            {
        //                cmd.Connection = connection;
        //                cmd.CommandText = sql;
        //                using (DbDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            ++count;
        //                            ContentModel cm = new ContentModel();
        //                            cm.news_id = Convert.ToInt64(reader.GetValue(0));
        //                            cm.title = reader.GetString(1);
        //                            cm.descp = reader.GetString(2);
        //                            try
        //                            {
        //                                cm.imageLnk = reader.GetString(3);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                cm.imageLnk = "";
        //                            }
        //                            cm.hot = Int32.Parse(reader.GetValue(4).ToString());
        //                            cm.cat_name = reader.GetString(5);
        //                            cm.Stt = count;
        //                            //cm.category_id = Convert.ToInt64(reader.GetValue(0));
        //                            nl.ctLst.Add(cm);
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("Error: " + sql);
        //                Console.WriteLine(e.StackTrace);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //                //connection.Dispose();
        //                //connection = null;
        //            }
        //            return PartialView("_Europa", nl);
        //        }

        [HttpGet]
        public ActionResult SearchErp()
        {
            //Get_Connection();
            string json = string.Empty;
            string JsonText = FlagsConst.newsJs + "c2";

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
            return PartialView("_Europa", nl);
        }

        //        [HttpGet]
        //        public ActionResult SearchFra()
        //        {
        //            //Get_Connection();
        //            newslist nl = new newslist();
        //            nl.ctLst = new List<ContentModel>();
        //            string sql = @"select a.news_id, a.title, a.description, a.image, a.hot, b.category_name 
        //                           from news a 
        //                           join category b on a.cat_id = b.category_id
        //                           where LOWER(a.description) like LOWER(N'%pháp%') or LOWER(a.description) like LOWER('%ligue 1%')
        //                           or LOWER(a.description) like LOWER('%psg%') or LOWER(a.description) like LOWER('%paris%') or LOWER(a.description) like LOWER('%monaco%')
        //                           or LOWER(a.description) like LOWER('%lyon%') or LOWER(a.description) like LOWER('%neymar%') or LOWER(a.description) like LOWER('%mbappe%')
        //                           or LOWER(a.description) like LOWER('%cavani%') or LOWER(a.description) like LOWER('%maria%')
        //                           order by a.news_id desc  
        //                           limit 10 ";
        //            connection.Open();
        //            int count = 0;

        //            try
        //            {
        //                cmd.Connection = connection;
        //                cmd.CommandText = sql;
        //                using (DbDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            ++count;
        //                            ContentModel cm = new ContentModel();
        //                            cm.news_id = Convert.ToInt64(reader.GetValue(0));
        //                            cm.title = reader.GetString(1);
        //                            cm.descp = reader.GetString(2);
        //                            try
        //                            {
        //                                cm.imageLnk = reader.GetString(3);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                cm.imageLnk = "";
        //                            }
        //                            cm.hot = Int32.Parse(reader.GetValue(4).ToString());
        //                            cm.cat_name = reader.GetString(5);
        //                            cm.Stt = count;
        //                            //cm.category_id = Convert.ToInt64(reader.GetValue(0));
        //                            nl.ctLst.Add(cm);
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("Error: " + sql);
        //                Console.WriteLine(e.StackTrace);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //                //connection.Dispose();
        //                //connection = null;
        //            }
        //            return PartialView("_France", nl);
        //        }

        [HttpGet]
        public ActionResult SearchFra()
        {
            //Get_Connection();
            string json = string.Empty;
            string JsonText = FlagsConst.newsJs + "france";

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
            return PartialView("_France", nl);
        }

        //        [HttpGet]
        //        public ActionResult SearchGer()
        //        {
        //            //Get_Connection();
        //            newslist nl = new newslist();
        //            nl.ctLst = new List<ContentModel>();
        //            string sql = @"select a.news_id, a.title, a.description, a.image, a.hot, b.category_name 
        //                           from news a 
        //                           join category b on a.cat_id = b.category_id
        //                           where LOWER(a.description) like LOWER(N'%đức%') or LOWER(a.description) like LOWER('%bundesliga%')
        //                           or LOWER(a.description) like LOWER('%bayern%') or LOWER(a.description) like LOWER('%dortmund%') or LOWER(a.description) like LOWER('%robben%')
        //                           or LOWER(a.description) like LOWER('%leipzig%') or LOWER(a.description) like LOWER('%schalke%') or LOWER(a.description) like LOWER('%ribery%')
        //                           or LOWER(a.description) like LOWER('%lewan%') or LOWER(a.description) like LOWER('%hummels%') or LOWER(a.description) like LOWER('%neuer%')
        //                           or LOWER(a.description) like LOWER('%reus%')
        //                           order by a.news_id desc  
        //                           limit 10 ";
        //            connection.Open();
        //            int count = 0;

        //            try
        //            {
        //                cmd.Connection = connection;
        //                cmd.CommandText = sql;
        //                using (DbDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            ++count;
        //                            ContentModel cm = new ContentModel();
        //                            cm.news_id = Convert.ToInt64(reader.GetValue(0));
        //                            cm.title = reader.GetString(1);
        //                            cm.descp = reader.GetString(2);
        //                            try
        //                            {
        //                                cm.imageLnk = reader.GetString(3);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                cm.imageLnk = "";
        //                            }
        //                            cm.hot = Int32.Parse(reader.GetValue(4).ToString());
        //                            cm.cat_name = reader.GetString(5);
        //                            cm.Stt = count;
        //                            //cm.category_id = Convert.ToInt64(reader.GetValue(0));
        //                            nl.ctLst.Add(cm);
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("Error: " + sql);
        //                Console.WriteLine(e.StackTrace);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //                //connection.Dispose();
        //                //connection = null;
        //            }
        //            return PartialView("_Germany", nl);
        //        }

        [HttpGet]
        public ActionResult SearchGer()
        {
            //Get_Connection();
            string json = string.Empty;
            string JsonText = FlagsConst.newsJs + "germany";

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
            return PartialView("_Germany", nl);
        }

        //        [HttpGet]
        //        public ActionResult SearchItaly()
        //        {
        //            //Get_Connection();
        //            newslist nl = new newslist();
        //            nl.ctLst = new List<ContentModel>();
        //            string sql = @"select a.news_id, a.title, a.description, a.image, a.hot, b.category_name 
        //                           from news a 
        //                           join category b on a.cat_id = b.category_id
        //                           where LOWER(a.description) like LOWER('%Italy%') or LOWER(a.description) like LOWER('%serie a%')
        //                           or LOWER(a.description) like LOWER('%juventus%') or LOWER(a.description) like LOWER('%inter%') or LOWER(a.description) like LOWER('%milan%')
        //                           or LOWER(a.description) like LOWER('%roma%') or LOWER(a.description) like LOWER('%lazio%') or LOWER(a.description) like LOWER('%napoli%')
        //                           or LOWER(a.description) like LOWER('%ronaldo%') or LOWER(a.description) like LOWER('%cr7%') or LOWER(a.description) like LOWER('%higuain%')
        //                           or LOWER(a.description) like LOWER('%dybala%') or LOWER(a.description) like LOWER('%mandzukic%') or LOWER(a.description) like LOWER('%ancelotti%')
        //                           or LOWER(a.description) like LOWER('%icardi%') or LOWER(a.description) like LOWER('%dzeko%') or LOWER(a.description) like LOWER('%hamsik%')
        //                           order by a.news_id desc  
        //                           limit 10 ";
        //            connection.Open();
        //            int count = 0;

        //            try
        //            {
        //                cmd.Connection = connection;
        //                cmd.CommandText = sql;
        //                using (DbDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            ++count;
        //                            ContentModel cm = new ContentModel();
        //                            cm.news_id = Convert.ToInt64(reader.GetValue(0));
        //                            cm.title = reader.GetString(1);
        //                            cm.descp = reader.GetString(2);
        //                            try
        //                            {
        //                                cm.imageLnk = reader.GetString(3);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                cm.imageLnk = "";
        //                            }
        //                            cm.hot = Int32.Parse(reader.GetValue(4).ToString());
        //                            cm.cat_name = reader.GetString(5);
        //                            cm.Stt = count;
        //                            //cm.category_id = Convert.ToInt64(reader.GetValue(0));
        //                            nl.ctLst.Add(cm);
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("Error: " + sql);
        //                Console.WriteLine(e.StackTrace);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //                //connection.Dispose();
        //                //connection = null;
        //            }
        //            return PartialView("_Italy", nl);
        //        }

        [HttpGet]
        public ActionResult SearchItaly()
        {
            //Get_Connection();
            string json = string.Empty;
            string JsonText = FlagsConst.newsJs + "italia";

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
            return PartialView("_Italy", nl);
        }

        //        [HttpGet]
        //        public ActionResult SearchSpain()
        //        {
        //            //Get_Connection();
        //            newslist nl = new newslist();
        //            nl.ctLst = new List<ContentModel>();
        //            string sql = @"select a.news_id, a.title, a.description, a.image, a.hot, b.category_name 
        //                           from news a 
        //                           join category b on a.cat_id = b.category_id
        //                           where LOWER(a.description) like LOWER(N'%tây ban nha%') or LOWER(a.description) like LOWER('%la liga%')
        //                           or LOWER(a.description) like LOWER('%barcelona%') or LOWER(a.description) like LOWER('%barca%') or LOWER(a.description) like LOWER('%real%')
        //                           or LOWER(a.description) like LOWER('%madrid%') or LOWER(a.description) like LOWER('%atletico%') or LOWER(a.description) like LOWER('%sevilla%')
        //                           or LOWER(a.description) like LOWER('%bale%') or LOWER(a.description) like LOWER('%modric%') or LOWER(a.description) like LOWER('%kroos%')
        //                           or LOWER(a.description) like LOWER('%isco%') or LOWER(a.description) like LOWER('%messi%') or LOWER(a.description) like LOWER('%suarez%')
        //                           or LOWER(a.description) like LOWER('%coutinho%') or LOWER(a.description) like LOWER('%rakitic%') or LOWER(a.description) like LOWER('%pique%')
        //                           or LOWER(a.description) like LOWER('%griezmann%') or LOWER(a.description) like LOWER('%courtois%')
        //                           order by a.news_id desc  
        //                           limit 10 ";
        //            connection.Open();
        //            int count = 0;

        //            try
        //            {
        //                cmd.Connection = connection;
        //                cmd.CommandText = sql;
        //                using (DbDataReader reader = cmd.ExecuteReader())
        //                {
        //                    if (reader.HasRows)
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            ++count;
        //                            ContentModel cm = new ContentModel();
        //                            cm.news_id = Convert.ToInt64(reader.GetValue(0));
        //                            cm.title = reader.GetString(1);
        //                            cm.descp = reader.GetString(2);
        //                            try
        //                            {
        //                                cm.imageLnk = reader.GetString(3);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                cm.imageLnk = "";
        //                            }
        //                            cm.hot = Int32.Parse(reader.GetValue(4).ToString());
        //                            cm.cat_name = reader.GetString(5);
        //                            cm.Stt = count;
        //                            //cm.category_id = Convert.ToInt64(reader.GetValue(0));
        //                            nl.ctLst.Add(cm);
        //                        }

        //                    }
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine("Error: " + sql);
        //                Console.WriteLine(e.StackTrace);
        //            }
        //            finally
        //            {
        //                connection.Close();
        //                //connection.Dispose();
        //                //connection = null;
        //            }
        //            return PartialView("_Spain", nl);
        //        }

        [HttpGet]
        public ActionResult SearchSpain()
        {
            //Get_Connection();
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
            return PartialView("_Spain", nl);
        }

        [HttpGet]
        public ActionResult MatchPre()
        {
            getInfoSumEng(FlagsConst.ftLinkPrem, "Prm");

            List<MatchInfoModel> matlst = new List<MatchInfoModel>();
            matlst = (List<MatchInfoModel>)TempData["InfoLst2"];
            TempData.Keep();
            string json = string.Empty;
            string jsonMa = string.Empty;
            //string JsonText = FlagsConst.ftLink;

            //WcInfoModel info = new WcInfoModel();
            //info.RoundLst = new List<RoundInfo>();
            RoundInfo ri = new RoundInfo();
            ri.Schelst = new List<MatchInfoModel>();

            //using (StreamReader reader = new StreamReader(JsonText))
            //{
            //    json = reader.ReadToEnd();
            //}

            for (int i = 0; i < matlst.Count; i++)
            {
                DateTime ustime = DateTime.ParseExact(matlst[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                   CultureInfo.InvariantCulture);
                DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);

                if (ustime <= timeno)
                {
                    if ((ustime > timeno.AddDays(-2)) && (ustime < timeno))
                    {
                        ri.Schelst.Add(matlst[i]);
                    }
                }
            }

            if (ri.Schelst.Count == 0)
            {
                //getInfoSum(FlagsConst.ftLinkFa, "Fa");
                List<MatchInfoModel> matlst2 = new List<MatchInfoModel>();
                matlst2 = getInfoSum(FlagsConst.ftLinkCl, "C1");

                //matlst2 = (List<MatchInfoModel>)TempData["InfoLst2"];
                TempData.Keep();
                for (int i = 0; i < matlst2.Count; i++)
                {
                    DateTime ustime = DateTime.ParseExact(matlst2[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                       CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmp = timeno.AddDays(1);
                    if (ustime <= timeno)
                    {
                        if ((ustime > timeno.AddDays(-1)) && (ustime < timeno))
                        {
                            ri.Schelst.Add(matlst2[i]);
                        }
                    }
                }
            }

            if (ri.Schelst.Count == 0)
            {
                //getInfoSum(FlagsConst.ftLinkFa, "Fa");
                List<MatchInfoModel> matlst2 = new List<MatchInfoModel>();
                matlst2 = getInfoSum(FlagsConst.ftLinkEup, "Eup");

                //matlst2 = (List<MatchInfoModel>)TempData["InfoLst2"];
                TempData.Keep();
                for (int i = 0; i < matlst2.Count; i++)
                {
                    DateTime ustime = DateTime.ParseExact(matlst2[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                       CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmp = timeno.AddDays(1);
                    if (ustime <= timeno)
                    {
                        if ((ustime > timeno.AddDays(-1)) && (ustime < timeno))
                        {
                            ri.Schelst.Add(matlst2[i]);
                        }
                    }
                }
            }

            if (ri.Schelst.Count == 0)
            {
                //getInfoSumUnl();
                List<MatchInfoModel> matlst2 = new List<MatchInfoModel>();
                matlst2 = getInfoSumUnl();

                //matlst2 = (List<MatchInfoModel>)TempData["InfoLst2"];
                TempData.Keep();
                for (int i = 0; i < matlst2.Count; i++)
                {
                    DateTime ustime = DateTime.ParseExact(matlst2[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                       CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmp = timeno.AddDays(1);
                    if (ustime <= timeno)
                    {
                        if ((ustime > timeno.AddDays(-3)) && (ustime < timeno))
                        {
                            ri.Schelst.Add(matlst2[i]);
                        }
                    }
                }
            }

            if (ri.Schelst.Count == 0)
            {
                //getInfoSum(FlagsConst.ftLinkEfl, "Efl");
                List<MatchInfoModel> matlst2 = new List<MatchInfoModel>();
                matlst2 = getInfoSum(FlagsConst.ftLinkEfl, "Efl");

                //matlst2 = (List<MatchInfoModel>)TempData["InfoLst2"];
                TempData.Keep();
                for (int i = 0; i < matlst2.Count; i++)
                {
                    DateTime ustime = DateTime.ParseExact(matlst2[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                       CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmp = timeno.AddDays(1);
                    if (ustime <= timeno)
                    {
                        if ((ustime > timeno.AddDays(-2)) && (ustime < timeno))
                        {
                            ri.Schelst.Add(matlst2[i]);
                        }
                    }
                }
                //return PartialView("_LiveMatchEu", ri);
            }

            if (ri.Schelst.Count == 0)
            {
                //getInfoSum(FlagsConst.ftLinkFa, "Fa");
                List<MatchInfoModel> matlst2 = new List<MatchInfoModel>();
                matlst2 = getInfoSum(FlagsConst.ftLinkFa, "Fa");

                //matlst2 = (List<MatchInfoModel>)TempData["InfoLst2"];
                TempData.Keep();
                for (int i = 0; i < matlst2.Count; i++)
                {
                    DateTime ustime = DateTime.ParseExact(matlst2[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                       CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmp = timeno.AddDays(1);
                    if (ustime <= timeno)
                    {
                        if ((ustime > timeno.AddDays(-2)) && (ustime < timeno))
                        {
                            ri.Schelst.Add(matlst2[i]);
                        }
                    }
                }
            }

            if (ri.Schelst.Count == 0)
            {
                for (int i = 0; i < matlst.Count; i++)
                {
                    DateTime ustime = DateTime.ParseExact(matlst[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                       CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);

                    if (ustime <= timeno)
                    {
                        if ((ustime > timeno.AddDays(-6)) && (ustime < timeno))
                        {
                            ri.Schelst.Add(matlst[i]);
                        }
                    }
                }
            }
            //if(ri.Schelst.Count == 0)
            //{
            //    getInfoSum(FlagsConst.ftLinkUnl, "Unl");
            //    List<MatchInfoModel> matlst2 = new List<MatchInfoModel>();
            //    matlst2 = (List<MatchInfoModel>)TempData["InfoLst2"];
            //    TempData.Keep();
            //}

            //if (ri.Schelst.Count == 0)
            //{
            //    getInfoSum(FlagsConst.ftLinkEfl, "Efl");

            //}

            //if (ri.Schelst.Count == 0)
            //{
            //    getInfoSum(FlagsConst.ftLinkFa, "Fa");

            //}

            return PartialView("_SchePre", ri);
        }

        public ActionResult StandPre()
        {
            //ViewBag.Message = "World cup 2018 Info";
            string json = string.Empty;
            string JsonText = "";
            MatchInfoModel mim = new MatchInfoModel();
            mim.StdGroup = new List<StandingModel>();
            //using (StreamReader reader = new StreamReader(JsonText))
            //{
            //    json = reader.ReadToEnd();
            //}
            string[] terms = new string[5];
            var listS = new List<dynamic>();

            for (int runs = 0; runs < 5; runs++)
            {
                switch (runs)
                {
                    case 0:
                        JsonText = FlagsConst.stdLinkPrem;
                        break;
                    case 1:
                        JsonText = FlagsConst.stdLinkLlg;
                        break;
                    case 2:
                        JsonText = FlagsConst.stdLinkSra;
                        break;
                    case 3:
                        JsonText = FlagsConst.stdLinkBdla;
                        break;
                    case 4:
                        JsonText = FlagsConst.stdLinkLg1;
                        break;
                }
                using (WebClient wc = new WebClient())
                {
                    json = wc.DownloadString(JsonText);
                }
                var StArr2 = JsonConvert.DeserializeObject<dynamic>(json);
                listS.Add(StArr2);
            }

            //using (WebClient wc = new WebClient())
            //{
            //    json = wc.DownloadString(JsonText);
            //}

            //var StArr = JsonConvert.DeserializeObject<dynamic>(json);

            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //var obj = jss.Deserialize<dynamic>(json);
            //int len = obj.Length;
            int len = listS.Count;

            for (int i = 0; i < len; i++)
            {
                var ls = listS[i];
                var m = ls[0];
                var convOb = m as JObject;
                var neStd = new tableObj();
                neStd = convOb.ToObject<tableObj>();
                StandingModel std = new StandingModel();
                std.Stt = i + 1;
                std.name = neStd.name;
                std.id = neStd.id;
                std.listStand = new List<GroupTeam>();
                //int lende = obj[i]["tableRows"].Length;
                int lende = neStd.tableRows.Count;

                for (int j = 0; j < lende; j++)
                {
                    var tmp = new DetailTeam();
                    tmp = neStd.tableRows[j];
                    var match = new GroupTeam();
                    match.TeamName = tmp.team.shortName;
                    match.TeamId = tmp.team.id;
                    match.TeamFlag = FlagsConst.ptLink + match.TeamId.ToString() + ".png";
                    match.Position = tmp.position;
                    match.matchesTotal = tmp.totalFields.matchesTotal;
                    match.winTotal = tmp.totalFields.winTotal;
                    match.drawTotal = tmp.totalFields.drawTotal;
                    match.lossTotal = tmp.totalFields.lossTotal;
                    match.goalsTotal = tmp.totalFields.goalsTotal;
                    match.goalDiffTotal = tmp.totalFields.goalDiffTotal;
                    match.pointsTotal = tmp.totalFields.pointsTotal;

                    std.listStand.Add(match);
                }
                mim.StdGroup.Add(std);
            }
            return PartialView("_StandPre", mim);
        }

        [HttpGet]
        public ActionResult StarPre()
        {
            //Get_Connection();
            newslist nl = new newslist();
            nl.ctLst = new List<ContentModel>();
            string sql = @"select a.news_id, a.title, a.description, a.image, a.hot, b.category_name 
                           from news a 
                           join category b on a.cat_id = b.category_id
                           where a.cat_id = 4
                           order by a.news_id desc  
                           limit 5 ";
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
                            cm.hot = Int32.Parse(reader.GetValue(4).ToString());
                            cm.cat_name = reader.GetString(5);
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
            }
            return PartialView("_StarsPre", nl);
        }

        [HttpGet]
        public ActionResult TransPre()
        {
            //Get_Connection();
            newslist nl = new newslist();
            nl.ctLst = new List<ContentModel>();
            string sql = @"select a.news_id, a.title, a.description, a.image, a.hot, b.category_name 
                           from news a 
                           join category b on a.cat_id = b.category_id
                           where a.cat_id = 5
                           order by a.news_id desc  
                           limit 5 ";
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
                            cm.hot = Int32.Parse(reader.GetValue(4).ToString());
                            cm.cat_name = reader.GetString(5);
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
            }
            return PartialView("_TransPre", nl);
        }

        public SumLst getEuOdd(long id)
        {
            string json = string.Empty;
            string jsonMa = string.Empty;

            string JsonText = FlagsConst.matchLnk + id + "/odds";
            string JsonTextMa = FlagsConst.matchLnk + id + "/details";

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
                    //return PartialView("_SoiKeo", ct);
                }
            }

            JavaScriptSerializer jss = new JavaScriptSerializer();
            var obj = jss.Deserialize<dynamic>(json);
            var objma = jss.Deserialize<dynamic>(jsonMa);

            try
            {
                int len = obj.Length;
                int hsc = objma["game"]["tournaments"][0]["events"][0]["status"]["code"] == null ? "?" : objma["game"]["tournaments"][0]["events"][0]["status"]["code"];

                for (int i = 0; i < 1; i++)
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
                    //else
                    //{
                    //    match.GoaLst = new List<MatchGoals>();
                    //    var lenfina = obj[len - 1][tmps].Length;
                    //    match.cate = 3;
                    //    for (int j = 0; j < lenfina; j++)
                    //    {
                    //        var mt = new MatchGoals();
                    //        mt.goal = obj[len - 1][tmps][j]["handicap"];
                    //        mt.OverGo = obj[len - 1][tmps][j]["odds"][0]["decimalValue"];
                    //        mt.UnderGo = obj[len - 1][tmps][j]["odds"][1]["decimalValue"];
                    //        match.GoaLst.Add(mt);
                    //    }
                    //}


                    std.OddLst.Add(match);
                    //}
                    ct.LstAll.Add(std);
                }
            }
            catch (Exception ex)
            {
                ct.LstAll = null;
            }

            return ct;
        }

        [HttpGet]
        public ActionResult LiveMatch()
        {
            //getInfoSum(FlagsConst.ftLinkPrem);
            List<MatchInfoModel> matlst = new List<MatchInfoModel>();
            matlst = (List<MatchInfoModel>)TempData["InfoLst2"];
            TempData.Keep();
            string json = string.Empty;
            string jsonMa = string.Empty;
            bool chkVi = false;
            //string JsonText = FlagsConst.ftLink;

            //WcInfoModel info = new WcInfoModel();
            //info.RoundLst = new List<RoundInfo>();
            RoundInfo ri = new RoundInfo();
            ri.Schelst = new List<MatchInfoModel>();

            //using (StreamReader reader = new StreamReader(JsonText))
            //{
            //    json = reader.ReadToEnd();
            //}


            for (int i = 0; i < matlst.Count; i++)
            {
                DateTime ustime = DateTime.ParseExact(matlst[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                   CultureInfo.InvariantCulture);
                DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                var tmp = timeno.AddDays(1);
                if (ustime > timeno)
                {
                    if ((ustime >= timeno) && (ustime < timeno.AddDays(2)))
                    {
                        ri.Schelst.Add(matlst[i]);
                    }
                }
                else if (ustime < timeno && timeno <= ustime.AddDays(0.125))
                {
                    ri.Schelst.Add(matlst[i]);
                }
            }

            if (ri.Schelst.Count == 0)
            {
                //getInfoSum(FlagsConst.ftLinkFa, "Fa");
                List<MatchInfoModel> matlst2 = new List<MatchInfoModel>();
                matlst2 = getInfoSum(FlagsConst.ftLinkCl, "C1");

                //matlst2 = (List<MatchInfoModel>)TempData["InfoLst2"];
                TempData.Keep();
                for (int i = 0; i < matlst2.Count; i++)
                {
                    DateTime ustime = DateTime.ParseExact(matlst2[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                       CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmp = timeno.AddDays(1);
                    if (ustime > timeno)
                    {
                        if ((ustime >= timeno) && (ustime < timeno.AddDays(1)))
                        {
                            ri.Schelst.Add(matlst2[i]);
                        }
                    }
                    else if (ustime < timeno && timeno <= ustime.AddDays(0.125))
                    {
                        ri.Schelst.Add(matlst2[i]);
                    }
                }
            }

            if (ri.Schelst.Count == 0)
            {
                //getInfoSum(FlagsConst.ftLinkFa, "Fa");
                List<MatchInfoModel> matlst2 = new List<MatchInfoModel>();
                matlst2 = getInfoSum(FlagsConst.ftLinkEup, "Eup");

                //matlst2 = (List<MatchInfoModel>)TempData["InfoLst2"];
                TempData.Keep();
                for (int i = 0; i < matlst2.Count; i++)
                {
                    DateTime ustime = DateTime.ParseExact(matlst2[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                       CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmp = timeno.AddDays(1);
                    if (ustime > timeno)
                    {
                        if ((ustime >= timeno) && (ustime < timeno.AddDays(1)))
                        {
                            ri.Schelst.Add(matlst2[i]);
                        }
                    }
                }
            }

            if (ri.Schelst.Count == 0)
            {
                //getInfoSum(FlagsConst.ftLinkEfl, "Efl");
                List<MatchInfoModel> matlst2 = new List<MatchInfoModel>();
                matlst2 = getInfoSum(FlagsConst.ftLinkEfl, "Efl");

                //matlst2 = (List<MatchInfoModel>)TempData["InfoLst2"];
                TempData.Keep();
                for (int i = 0; i < matlst2.Count; i++)
                {
                    DateTime ustime = DateTime.ParseExact(matlst2[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                       CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmp = timeno.AddDays(1);
                    if (ustime > timeno)
                    {
                        if ((ustime >= timeno) && (ustime < timeno.AddDays(3)))
                        {
                            ri.Schelst.Add(matlst2[i]);
                        }
                    }
                }
                //return PartialView("_LiveMatchEu", ri);
            }

            //if (ri.Schelst.Count == 0)
            //{
            //    //getInfoSum(FlagsConst.ftLinkFa, "Fa");
            //    List<MatchInfoModel> matlst2 = new List<MatchInfoModel>();
            //    matlst2 = getInfoSum(FlagsConst.ftLinkFa, "Fa");

            //    //matlst2 = (List<MatchInfoModel>)TempData["InfoLst2"];
            //    TempData.Keep();
            //    for (int i = 0; i < matlst2.Count; i++)
            //    {
            //        DateTime ustime = DateTime.ParseExact(matlst2[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
            //        var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
            //                           CultureInfo.InvariantCulture);
            //        DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
            //        var tmp = timeno.AddDays(1);
            //        if (ustime > timeno)
            //        {
            //            if ((ustime >= timeno) && (ustime < timeno.AddDays(3)))
            //            {
            //                ri.Schelst.Add(matlst2[i]);
            //            }
            //        }
            //    }
            //}

            if (ri.Schelst.Count == 0)
            {
                for (int i = 0; i < matlst.Count; i++)
                {
                    DateTime ustime = DateTime.ParseExact(matlst[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                       CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmp = timeno.AddDays(1);
                    if (ustime > timeno)
                    {
                        if ((ustime >= timeno) && (ustime < timeno.AddDays(5)))
                        {
                            ri.Schelst.Add(matlst[i]);
                        }
                    }
                }
            }

            if (ri.Schelst.Count == 0)
            {
                chkVi = true;
                //getInfoSumUnl();
                List<MatchInfoModel> matlst2 = new List<MatchInfoModel>();
                matlst2 = getInfoSumUnl();
                //matlst2 = (List<MatchInfoModel>)TempData["InfoLst2"];
                TempData.Keep();
                for (int i = 0; i < matlst2.Count; i++)
                {
                    DateTime ustime = DateTime.ParseExact(matlst2[i].timeSt, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("dd/MM/yyyy HH:mm tt",
                                       CultureInfo.InvariantCulture);
                    DateTime timeno = DateTime.ParseExact(tmpnow, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
                    var tmp = timeno.AddDays(1);
                    if (ustime > timeno)
                    {
                        if ((ustime >= timeno) && (ustime < timeno.AddDays(3)))
                        {
                            ri.Schelst.Add(matlst2[i]);
                        }
                    }
                }
                return PartialView("_LiveMatchEu", ri);
            }

            if (chkVi == true)
                return PartialView("_LiveMatchEu", ri);
            else
                return PartialView("_LiveMatch", ri);

            //return PartialView("_LiveMatch", ri);
        }

        private SumLst ParsingEng(string time, string home, string away)
        {
            SumLst ct = new SumLst();
            ct.LstAll = new List<CatOdds>();
            CatOdds std = new CatOdds();
            std.OddLst = new List<OddsRate>();
            try
            {
                //HttpClient http = new HttpClient();
                //var response = await http.GetByteArrayAsync(website);
                //String source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                //source = WebUtility.HtmlDecode(source);
                //WebClient webClient = new WebClient();
                //string page = webClient.DownloadString(website);

                string page = "";
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                using (WebClient webClient = new WebClient())
                {
                    page = webClient.DownloadString(FlagsConst.AsaLinkPrem);
                }
                //string page = "";
                //using (WebClient client = new WebClient())
                //{
                //    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                //    page = client.DownloadString(website);
                //}
                HtmlDocument resultat = new HtmlDocument();
                resultat.LoadHtml(page);

                AsLeague ai = new AsLeague();
                var root = resultat.DocumentNode;
                var anchors = root.Descendants("div");
                var handlingTimeNode = resultat.DocumentNode.SelectSingleNode("//*[@id=\"scn-HeaderNavigation_BreadcrumbLevel2\"]");

                //            var p = root.Descendants()
                //.Where(n => n.GetAttributeValue("class", "").Equals("scn-HeaderNavigation_BreadcrumbLevel2 scn-HeaderNavigation_HasMenu"))
                //.FirstOrDefault().InnerText;
                //ai.NaLeague = resultat.DocumentNode.SelectNodes("//td/input");
                //var div = resultat.DocumentNode.SelectSingleNode("//div[@id='scn-HeaderNavigation_BreadcrumbLevel2']").InnerText;
                //            string name = resultat.DocumentNode
                //.SelectSingleNode("//div")
                //.Attributes["value"].Value;

                //ai.NaLeague = (string)name;
                List<HtmlNode> table = resultat.DocumentNode.Descendants().Where
                (x => (x.Name == "div" && x.Attributes["id"] != null && x.Attributes["id"].Value.Contains("masterdiv"))).ToList();
                //     var table = resultat.DocumentNode
                //.Descendants("tr")
                //.Select(n => n.Elements("td").Select(e => e.InnerText).ToArray());

                //var li = toftitle[6].Descendants("li").ToList();
                //ai.AsianLst = new List<AsianInfo>();
                var _nodChinh = resultat.DocumentNode.SelectNodes(@"//div[@id='masterdiv']//table[1]//tr");
                //var tableTmp = _nodChinh.SelectNodes("//table[1]//tr//td//font");
                //var tds = _nodChinh.SelectNodes("//table //tr");
                int a = 0;
                foreach (HtmlNode tab in resultat.DocumentNode.SelectNodes(@"//div[@id='masterdiv']//table[1]"))
                {
                    foreach (HtmlNode row in tab.SelectNodes("tr"))
                    {
                        var ma = new OddsRate();
                        bool check1 = false, check3 = false, check4 = false;
                        a++;
                        string name = row.InnerText.Trim();
                        if (a >= 4)
                        {
                            int chk = 0;
                            foreach (HtmlNode cell in row.SelectNodes("td"))
                            {
                                string name2 = cell.InnerText.Trim();
                                chk++;
                                //if (a == 2)
                                //{
                                if (chk == 1)
                                {
                                    if (cell.SelectNodes("font") != null)
                                    {
                                        foreach (HtmlNode font in cell.SelectNodes("font"))
                                        {
                                            string name3 = font.InnerText.Trim();
                                        }
                                    }
                                    else
                                    {
                                        int count = 0;
                                        foreach (HtmlNode font in cell.SelectNodes("span"))
                                        {
                                            count++;
                                            if (count == 1)
                                            {
                                                string tmpname3 = font.SelectSingleNode("font").InnerText.Trim();
                                                DateTime ustime = DateTime.ParseExact(tmpname3, "HH:mm", CultureInfo.InvariantCulture);
                                                string easternZoneId = "GMT Standard Time";
                                                TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
                                                string name3 = TimeZoneInfo.ConvertTimeToUtc(ustime, easternZone).ToString("HH:mm");

                                                if (time == name3)
                                                {
                                                    check1 = true;
                                                    continue;
                                                }
                                                else
                                                {
                                                    check1 = false;
                                                    break;
                                                }
                                            }
                                            //else if (count == 2)
                                            //{
                                            //    string namedt = font.SelectSingleNode("font").InnerText.Trim();

                                            //    string tmp = namedt.Substring(0, 6);
                                            //    DateTime usnamedt = DateTime.ParseExact(tmp, "dd/MMM", CultureInfo.InvariantCulture);

                                            //    string chktime = namedt.Substring(18, 5);
                                            //    DateTime ustime = DateTime.ParseExact(chktime, "HH:mm", CultureInfo.InvariantCulture);
                                            //    DateTime timeEnd = DateTime.ParseExact("00:59", "HH:mm", CultureInfo.InvariantCulture);
                                            //    DateTime timeSta = DateTime.ParseExact("00:00", "HH:mm", CultureInfo.InvariantCulture);

                                            //    int cp1 = TimeSpan.Compare(ustime.TimeOfDay, timeEnd.TimeOfDay);
                                            //    int cp2 = TimeSpan.Compare(ustime.TimeOfDay, timeSta.TimeOfDay);

                                            //    if(cp1 == -1 && cp2 == 1)
                                            //    {
                                            //        usnamedt = usnamedt.AddDays(-1);
                                            //    }
                                            //    tmp = usnamedt.ToString("dd/MMM");

                                            //    if (date == tmp)
                                            //    {
                                            //        check2 = true;
                                            //        continue;
                                            //    }
                                            //    else
                                            //    {
                                            //        check2 = false;
                                            //        break;
                                            //    }
                                            //}
                                        }
                                    }
                                }
                                else if (chk == 2)
                                {
                                    int count = 0;
                                    foreach (HtmlNode font in cell.SelectNodes("span"))
                                    {
                                        count++;
                                        if (count == 3)
                                        {
                                            string nameho = font.InnerText.Trim();
                                            var firstWord = nameho.IndexOf(" ") > -1 ? nameho.Substring(0, nameho.IndexOf(" ")) : nameho;
                                            if (home.ToLower().Contains(firstWord.ToLower()))
                                            {
                                                check3 = true;
                                                continue;
                                            }
                                            else
                                            {
                                                check3 = false;
                                                break;
                                            }
                                        }
                                        string name5 = font.InnerText.Trim();
                                        if (count == 5)
                                        {
                                            string nameaw = font.InnerText.Trim();
                                            var firstWord = nameaw.IndexOf(" ") > -1 ? nameaw.Substring(0, nameaw.IndexOf(" ")) : nameaw;
                                            if (away.ToLower().Contains(firstWord.ToLower()))
                                            {
                                                check4 = true;
                                                continue;
                                            }
                                            else
                                            {
                                                check4 = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                else if ((chk == 3 || chk == 5) && check1 == true && check3 == true && check4 == true)
                                {
                                    foreach (HtmlNode font in cell.SelectNodes("b"))
                                    {
                                        string name6 = font.SelectSingleNode("span").InnerText.Trim();
                                        if (chk == 3)
                                        {
                                            ma.HomeWn = name6;
                                        }
                                        else
                                        {
                                            ma.AwayWn = name6;
                                        }
                                    }
                                }

                                else if (chk == 4 || chk == 7 && check1 == true && check3 == true && check4 == true)
                                {
                                    foreach (HtmlNode font in cell.SelectNodes("b"))
                                    {
                                        foreach (HtmlNode b in font.SelectNodes("font"))
                                        {
                                            foreach (HtmlNode lk in b.SelectNodes("a"))
                                            {
                                                string name8 = lk.SelectSingleNode("span").InnerText.Trim();
                                                if (chk == 4)
                                                {
                                                    ma.Draw = name8;
                                                }
                                                else
                                                {
                                                    ma.DrawChoice = name8;
                                                }
                                            }
                                        }
                                    }
                                }

                                else if (chk == 6 || chk == 8 && check1 == true && check3 == true && check4 == true)
                                {
                                    if (cell.SelectNodes("span") != null)
                                    {
                                        foreach (HtmlNode font in cell.SelectNodes("span"))
                                        {
                                            string name9 = font.InnerText.Trim();
                                            if (chk == 6)
                                            {
                                                ma.HomeChoice = name9;
                                            }
                                            else
                                            {
                                                ma.AwayChoice = name9;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (HtmlNode font in cell.SelectNodes("font"))
                                        {
                                            foreach (HtmlNode b in font.SelectNodes("b"))
                                            {
                                                string name3 = b.SelectSingleNode("span").InnerText.Trim();
                                                if (chk == 6)
                                                {
                                                    ma.HomeChoice = name3;
                                                }
                                                else
                                                {
                                                    ma.AwayChoice = name3;
                                                }
                                            }
                                        }
                                    }
                                }
                                //}
                            }
                        }
                        else
                        {
                            continue;
                        }
                        if (check1 == true && check3 == true && check4 == true)
                        {
                            std.OddLst.Add(ma);
                            ct.LstAll.Add(std);
                            break;
                        }
                    }
                }

                //for (int i = 0; i < _nodChinh.Count; i++)
                //{
                //    if (i == 2)
                //    {
                //        var _nod2 = resultat.DocumentNode.SelectNodes(@"//div[@id='masterdiv']//table[1]//tr//td//font");
                //        string name = _nod2[i].InnerText.Trim();
                //    }

                //var link = item.Descendants("td").ToList()[0].GetAttributeValue("font", null);
                //var img = item.Descendants("td").ToList()[0].GetAttributeValue("span", null);
                //string _tieude = item.SelectSingleNode(@"td/font").InnerText.Trim();
                //var title = item.Descendants("h5").ToList()[0].InnerText;

                //ai.AsianLst.Add(new AsianInfo()
                //{
                //HomeAsianHd = img,
                //AwayAsianHd = title,
                //RateHomeAsianHd = link
                //    });
                //}

                //listboxproduct.DataContext = listproduct;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //MessageBox.Show("Network Problem!");
            }
            return ct;
        }

        private List<OddsRate> LoadParsing(string cst)
        {
            //SumLst ct = new SumLst();
            //ct.LstAll = new List<CatOdds>();
            //CatOdds std = new CatOdds();
            List<OddsRate> lor = new List<OddsRate>();
            try
            {

                string page = "";
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                using (WebClient webClient = new WebClient())
                {
                    page = webClient.DownloadString(cst);
                }

                HtmlDocument resultat = new HtmlDocument();
                resultat.LoadHtml(page);

                AsLeague ai = new AsLeague();
                var root = resultat.DocumentNode;
                var anchors = root.Descendants("div");
                var handlingTimeNode = resultat.DocumentNode.SelectSingleNode("//*[@id=\"scn-HeaderNavigation_BreadcrumbLevel2\"]");

                List<HtmlNode> table = resultat.DocumentNode.Descendants().Where
                (x => (x.Name == "div" && x.Attributes["id"] != null && x.Attributes["id"].Value.Contains("masterdiv"))).ToList();

                var _nodChinh = resultat.DocumentNode.SelectNodes(@"//div[@id='masterdiv']//table[1]//tr");

                int a = 0;
                foreach (HtmlNode tab in resultat.DocumentNode.SelectNodes(@"//div[@id='masterdiv']//table[1]"))
                {
                    foreach (HtmlNode row in tab.SelectNodes("tr"))
                    {
                        var ma = new OddsRate();
                        bool check1 = false, check3 = false, check4 = false;
                        a++;
                        string name = row.InnerText.Trim();
                        if (a >= 4)
                        {
                            int chk = 0;
                            foreach (HtmlNode cell in row.SelectNodes("td"))
                            {
                                string name2 = cell.InnerText.Trim();
                                chk++;
                                //if (a == 2)
                                //{
                                if (chk == 1)
                                {
                                    if (cell.SelectNodes("font") != null)
                                    {
                                        foreach (HtmlNode font in cell.SelectNodes("font"))
                                        {
                                            string name3 = font.InnerText.Trim();
                                        }
                                    }
                                    else
                                    {
                                        int count = 0;
                                        foreach (HtmlNode font in cell.SelectNodes("span"))
                                        {
                                            count++;
                                            if (count == 1)
                                            {
                                                string tmpname3 = font.SelectSingleNode("font").InnerText.Trim();
                                                ma.timeMt = tmpname3;
                                                //DateTime ustime = DateTime.ParseExact(tmpname3, "HH:mm", CultureInfo.InvariantCulture);
                                                //string easternZoneId = "GMT Standard Time";
                                                //TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);
                                                //string name3 = TimeZoneInfo.ConvertTimeToUtc(ustime, easternZone).ToString("HH:mm");

                                                //if (time == name3)
                                                //{
                                                //    check1 = true;
                                                //    continue;
                                                //}
                                                //else
                                                //{
                                                //    check1 = false;
                                                //    break;
                                                //}
                                            }
                                        }
                                    }
                                }
                                else if (chk == 2)
                                {
                                    int count = 0;
                                    foreach (HtmlNode font in cell.SelectNodes("span"))
                                    {
                                        count++;
                                        if (count == 3)
                                        {
                                            string nameho = font.InnerText.Trim();
                                            int countbl = nameho.Count(Char.IsWhiteSpace);
                                            if (countbl == 1)
                                            {
                                                int ps = nameho.IndexOf(" ");
                                                var secWor = nameho.Substring(ps + 1, nameho.Length - ps - 1);
                                                ma.SubHomeTe = secWor;
                                            }
                                            else if (countbl > 1)
                                            {
                                                int index = nameho.IndexOf(" ", nameho.IndexOf(" ") + 1);
                                                int ps = nameho.IndexOf(" ");
                                                var secWor = nameho.Substring(ps + 1, index - ps - 1);
                                                ma.SubHomeTe = secWor;
                                            }
                                            else if (countbl == 0)
                                            {
                                                ma.SubHomeTe = nameho;
                                            }

                                            if (nameho == "QPR")
                                            {
                                                nameho = "Queens";
                                            }
                                            else if (nameho == "Fenerbahce")
                                            {
                                                nameho = "Fenerbah";
                                            }
                                            else if(nameho == "G.Rangers")
                                            {
                                                nameho = "Rangers";
                                            }
                                            else if (nameho == "Malmo")
                                            {
                                                nameho = "Malm";
                                            }
                                            else if (nameho == "Fehervar Videoton")
                                            {
                                                nameho = "Vidi";
                                            }

                                            //ma.SubHomeTe = nameho;
                                            var firstWord = nameho.IndexOf(" ") > -1 ? nameho.Substring(0, nameho.IndexOf(" ")) : nameho;
                                            if (firstWord == "Dinamo")
                                            {
                                                firstWord = "Dynamo";
                                            }
                                            ma.HomeTe = firstWord;
                                            //if (home.ToLower().Contains(firstWord.ToLower()))
                                            //{
                                            //    check3 = true;
                                            //    continue;
                                            //}
                                            //else
                                            //{
                                            //    check3 = false;
                                            //    break;
                                            //}
                                        }
                                        string name5 = font.InnerText.Trim();
                                        if (count == 5)
                                        {
                                            string nameaw = font.InnerText.Trim();
                                            int countbl = nameaw.Count(Char.IsWhiteSpace);
                                            if (countbl == 1)
                                            {
                                                int ps = nameaw.IndexOf(" ");
                                                var secWor = nameaw.Substring(ps + 1, nameaw.Length - ps - 1);
                                                ma.SubAwayTe = secWor;
                                            }
                                            else if (countbl > 1)
                                            {
                                                int index = nameaw.IndexOf(" ", nameaw.IndexOf(" ") + 1);
                                                int ps = nameaw.IndexOf(" ");
                                                var secWor = nameaw.Substring(ps + 1, index - ps - 1);
                                                ma.SubAwayTe = secWor;
                                            }
                                            else if (countbl == 0)
                                            {
                                                ma.SubAwayTe = nameaw;
                                            }

                                            if (nameaw == "QPR")
                                            {
                                                nameaw = "Queens";
                                            }

                                            else if (nameaw == "Fenerbahce")
                                            {
                                                nameaw = "Fenerbah";
                                            }
                                            else if (nameaw == "G.Rangers")
                                            {
                                                nameaw = "Rangers";
                                            }
                                            else if(nameaw == "Malmo")
                                            {
                                                nameaw = "Malm";
                                            }
                                            else if (nameaw == "Fehervar Videoton")
                                            {
                                                nameaw = "Vidi";
                                            }

                                            //ma.SubAwayTe = nameaw;
                                            var firstWord = nameaw.IndexOf(" ") > -1 ? nameaw.Substring(0, nameaw.IndexOf(" ")) : nameaw;
                                            if(firstWord == "Dinamo")
                                            {
                                                firstWord = "Dynamo";
                                            }
                                            ma.AwayTe = firstWord;
                                            //if (away.ToLower().Contains(firstWord.ToLower()))
                                            //{
                                            //    check4 = true;
                                            //    continue;
                                            //}
                                            //else
                                            //{
                                            //    check4 = false;
                                            //    break;
                                            //}
                                        }
                                    }
                                }

                                else if (chk == 3 || chk == 5)
                                {
                                    foreach (HtmlNode font in cell.SelectNodes("b"))
                                    {
                                        string name6 = font.SelectSingleNode("span").InnerText.Trim();
                                        if (chk == 3)
                                        {
                                            ma.AsaHome = name6;
                                        }
                                        else
                                        {
                                            ma.AsaAway = name6;
                                        }
                                    }
                                }

                                else if (chk == 4 || chk == 7)
                                {
                                    foreach (HtmlNode font in cell.SelectNodes("b"))
                                    {
                                        foreach (HtmlNode b in font.SelectNodes("font"))
                                        {
                                            foreach (HtmlNode lk in b.SelectNodes("a"))
                                            {
                                                string name8 = lk.SelectSingleNode("span").InnerText.Trim();
                                                if (chk == 4)
                                                {
                                                    ma.AsaDraw = name8;
                                                }
                                                else
                                                {
                                                    ma.TotalChoice = name8;
                                                }
                                            }
                                        }
                                    }
                                }

                                else if (chk == 6 || chk == 8)
                                {
                                    if (cell.SelectNodes("span") != null)
                                    {
                                        foreach (HtmlNode font in cell.SelectNodes("span"))
                                        {
                                            string name9 = font.InnerText.Trim();
                                            if (chk == 6)
                                            {
                                                ma.OverChoice = name9;
                                            }
                                            else
                                            {
                                                ma.UnderChoice = name9;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (HtmlNode font in cell.SelectNodes("font"))
                                        {
                                            foreach (HtmlNode b in font.SelectNodes("b"))
                                            {
                                                string name3 = b.SelectSingleNode("span").InnerText.Trim();
                                                if (chk == 6)
                                                {
                                                    ma.OverChoice = name3;
                                                }
                                                else
                                                {
                                                    ma.UnderChoice = name3;
                                                }
                                            }
                                        }
                                    }
                                }
                                //}
                            }
                        }
                        else
                        {
                            continue;
                        }
                        //if (check1 == true && check3 == true && check4 == true)
                        //{
                        lor.Add(ma);
                        //ct.LstAll.Add(std);
                        //break;
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //MessageBox.Show("Network Problem!");
            }
            return lor;
        }

        public static string convertToUnSign3(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
    }
}