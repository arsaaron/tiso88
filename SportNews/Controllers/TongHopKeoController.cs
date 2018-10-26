using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SportNews.Constants;
using SportNews.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SportNews.Controllers
{
    public class TongHopKeoController : Controller
    {
        // GET: TongHopKeo
        public ActionResult Index()
        {
            SumLst ct = new SumLst();
            //ct.LstAll = new List<CatOdds>();

            //CatOdds stdAs = new CatOdds();
            //stdAs.OddLst = new List<OddsRate>();
            //stdAs.OddLst = LoadParsing(FlagsConst.btLnk);
            //stdAs = LoadParsing(FlagsConst.btLnk);
            ct = LoadParsing(FlagsConst.btLnkMi);

            return View(ct);
        }

        private SumLst LoadParsing(string cst)
        {
            //SumLst ct = new SumLst();
            //ct.LstAll = new List<CatOdds>();
            //CatOdds std = new CatOdds();
            SumLst sl = new SumLst();
            sl.LstAll = new List<CatOdds>();
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

                var _nodChinh = resultat.DocumentNode.SelectNodes(@"//div[@class='coming']//table//tbody//tr");

                int a = 0; int check = 0;
                string nameLe = "", tmTime = "", nameho = "", nameaw = "", tyle = "";
                var dict = new Dictionary<string, int>();
                var tmpOd = new List<OddsRate>();

                foreach (HtmlNode tab in resultat.DocumentNode.SelectNodes("//div[contains(@class,'xboxcontent')]//div"))
                {
                    check++;
                    if (check != 5)
                    {
                        continue;
                    }
                    CatOdds lor = null; int mark = 0, max = 0;

                    foreach (HtmlNode dv in tab.SelectNodes("table"))
                    {
                        //lor = new CatOdds();
                        //lor.OddLst = new List<OddsRate>();

                        foreach (HtmlNode row in dv.SelectNodes("tr"))
                        {
                            /*continue*/
                            ;
                            int count = 0; int ctfo = 0; int dem = 0;
                            var ma = new OddsRate();

                            //List<OddsRate> rate;
                            bool check1 = false, check3 = false, check4 = false;
                            a++;
                            string name = row.InnerText.Trim();
                            //if (row.SelectSingleNode("./td[1]/strong")!= null)
                            //{
                            //    lor = new CatOdds();
                            //    lor.OddLst = new List<OddsRate>();
                            //    lor.name = name;
                            //    continue;
                            //}
                            //mark++;
                            //max = mark;
                            if (row == dv.SelectSingleNode("tr[last()]"))
                            {
                                dict.Add(nameaw, max);
                            }

                            foreach (HtmlNode cell in row.SelectNodes("td"))
                            {
                                count++;
                                string name2 = cell.InnerText.Trim();

                                if (cell.SelectNodes("a") != null)
                                {
                                    //count++;
                                    if (a != 1)
                                    {
                                        dict.Add(nameaw, max);
                                    }
                                    mark = 0;

                                    foreach (HtmlNode font in cell.SelectNodes("a"))
                                    {
                                        byte[] bytes = Encoding.Default.GetBytes(font.InnerText.Trim());
                                        string myString = Encoding.UTF8.GetString(bytes);
                                        //nameLe = font.InnerText.Trim();
                                        ma.NaLig = myString;
                                        nameaw = myString;
                                    }
                                    continue;
                                }

                                if (count == 1)
                                {
                                    byte[] bytes = Encoding.Default.GetBytes(cell.InnerText.Trim());
                                    string myString = Encoding.UTF8.GetString(bytes);
                                    ma.timeMt = myString;
                                    ma.mark = mark;

                                }
                                else if (count == 2)
                                {
                                    byte[] bytes = Encoding.Default.GetBytes(cell.InnerText.Trim());
                                    string myString = Encoding.UTF8.GetString(bytes);
                                    ma.HomeTe = myString;
                                    mark++;
                                    max = mark;
                                    ma.mark = mark;

                                }
                                else if (count == 5)
                                {
                                    byte[] bytes = Encoding.Default.GetBytes(cell.InnerText.Trim());
                                    string myString = Encoding.UTF8.GetString(bytes);
                                    ma.AwayWn = myString;
                                    ma.mark = mark;

                                }
                                else if (count == 6)
                                {
                                    byte[] bytes = Encoding.Default.GetBytes(cell.InnerText.Trim());
                                    string myString = Encoding.UTF8.GetString(bytes);
                                    ma.TotalChoice = myString;
                                    ma.mark = mark;

                                }
                                else if (count == 7)
                                {
                                    byte[] bytes = Encoding.Default.GetBytes(cell.InnerText.Trim());
                                    string myString = Encoding.UTF8.GetString(bytes);
                                    ma.OverChoice = myString;
                                    ma.mark = mark;

                                }
                                else if (count == 10)
                                {
                                    byte[] bytes = Encoding.Default.GetBytes(cell.InnerText.Trim());
                                    string myString = Encoding.UTF8.GetString(bytes);
                                    ma.DrawChoice = myString;
                                    ma.mark = mark;

                                }

                                //if (cell.SelectNodes("a") != null)
                                //{
                                //    continue;
                                //}                             

                                else if (count == 3)
                                {
                                    byte[] bytes = Encoding.Default.GetBytes(cell.InnerText.Trim());
                                    string myString = Encoding.UTF8.GetString(bytes);
                                    ma.HomeWn = myString;
                                    ma.mark = mark;

                                }
                                else if (count == 4)
                                {
                                    byte[] bytes = Encoding.Default.GetBytes(cell.InnerText.Trim());
                                    string myString = Encoding.UTF8.GetString(bytes);
                                    ma.Draw = myString;
                                    ma.mark = mark;

                                }
                                else if (count == 8)
                                {
                                    byte[] bytes = Encoding.Default.GetBytes(cell.InnerText.Trim());
                                    string myString = Encoding.UTF8.GetString(bytes);
                                    ma.UnderChoice = myString;
                                    ma.mark = mark;

                                }
                                else if (count == 9)
                                {
                                    byte[] bytes = Encoding.Default.GetBytes(cell.InnerText.Trim());
                                    string myString = Encoding.UTF8.GetString(bytes);
                                    ma.HomeChoice = myString;
                                    ma.mark = mark;

                                }
                                else if (count == 11)
                                {
                                    byte[] bytes = Encoding.Default.GetBytes(cell.InnerText.Trim());
                                    string myString = Encoding.UTF8.GetString(bytes);
                                    ma.AwayChoice = myString;
                                    ma.mark = mark;

                                }
                            }
                            if (!string.IsNullOrEmpty(ma.HomeTe))
                            {
                                tmpOd.Add(ma);
                            }
                            //lor.OddLst.Add(ma);
                            //ls.Add(lor);
                        }

                        //ls.Add(lor);
                        break;
                    }
                }

                int k = 0;
                foreach (var pa in dict)
                {
                    string key = pa.Key;
                    int val = pa.Value;
                    //tm = tm + val;
                    CatOdds ri = new CatOdds();
                    ri.name = key;
                    ri.OddLst = new List<OddsRate>();
                    for (int aq = k; aq < val + k; aq++)
                    {
                        ri.OddLst.Add(tmpOd[aq]);
                    }
                    k += val;
                    sl.LstAll.Add(ri);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //MessageBox.Show("Network Problem!");
            }
            return sl;
        }

        [HttpGet]
        public ActionResult KeoDetail(string mts)
        {
            HomeController hc = new HomeController();
            Formation info = new Formation();
            long rid = 0;
            if (mts.Contains("-") == false)
                return View("Index");
            else
                rid = getIdm(mts);
            info = hc.getMatch(rid);
            return View("KeoDetail", info);
        }

        public long getIdm(string mts)
        {
            int pos = mts.IndexOf("^");
            string timeTp = mts.Substring(0, pos);
            int p2 = mts.IndexOf("-");
            string homeTmp = mts.Substring(pos + 1, p2 - pos - 2);
            string awayTmp = mts.Substring(p2 + 1, mts.Length - p2 - 1);
            if (homeTmp == "Rennes")
            {
                homeTmp = "Stade Rennais";
            }
            if (awayTmp == "Rennes")
            {
                awayTmp = "Stade Rennais";
            }
            var tmpnow = DateTime.UtcNow.ToLocalTime().ToString("yyyy",
                   CultureInfo.InvariantCulture);
            int posy = timeTp.IndexOf(" ");
            string tmpDate = timeTp.Substring(0, posy + 1);
            int p3 = tmpDate.IndexOf("/");
            string dTmp = tmpDate.Substring(0, p3);
            string mTmp = tmpDate.Substring(p3 + 1, tmpDate.Length - p3 - 2);
            //string fulltime = tmpDate.Replace("/", "-");

            string neDate = string.Concat(mTmp, string.Concat("-", dTmp));
            neDate.Replace(" ", string.Empty);
            string ustime = string.Concat(tmpnow, string.Concat("-", neDate));

            //string ustime = tmpnow.Insert(5, "-" + neDate).ToString();
            //DateTime ustime = DateTime.ParseExact(timeTp, "dd/MM/yyyy HH:mm tt", CultureInfo.InvariantCulture);
            //DateTime ustime = DateTime.ParseExact(fulltime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            //DateTime ustime = Convert.ToDateTime("dd/MM/yyyy HH:mm");

            //var timevr = ustime.ToString("yyyy-MM-dd");

            long id = 0;
            string date1 = ustime;
            string json = string.Empty;
            string JsonText = FlagsConst.mergeLnk + date1 + "/json";

            using (WebClient wc = new WebClient())
            {
                json = wc.DownloadString(JsonText);
            }
            //objMatch m = JsonConvert.DeserializeObject<objMatch>(json);
            objMatch objGo = JsonConvert.DeserializeObject<objMatch>(json);

            //int lenGo = objGo.Count;
            //if(lenGo > 0)
            //{
            //    for (int a = 0; a < lenGo; a ++)
            //    {
            //        var mt = objGo[a];
            //        var convOb = mt as JObject;
            //        var tmp = new spItem();
            //        tmp = convOb.ToObject<spItem>();
            //    }
            //}
            //objMatch om = new objMatch();
            int count = 0;
            //objGo.sportItem = new spItem();
            //objGo.sportItem.tournaments = new List<tourna>();
            //om.sportItem = objGo;
            //objMatch info = ((JArray)objGo)[0].ToObject<objMatch>();

            //objGo.sportItem = new spItem();
            //var convOb = objGo as JObject;

            //var tmp = new objMatch();
            //tmp.sportItem = new spItem();
            //tmp.sportItem.tournaments = new List<tourna>();
            //tmp.sportItem = convOb.ToObject<spItem>();
            //tmp.sportItem.tournaments = new List<tourna>();
            //tmp.sportItem.tournaments = objGo.sportItem.tournaments;

            foreach (var x in objGo.sportItem.tournaments)
            {
                //x.events = new List<smEvent>();
                foreach (var y in x.events)
                {
                    string SubHomeTe = "";
                    string HomeTe = "";
                    string nameho = y.homeTeam.name;
                    int countbl = nameho.Count(Char.IsWhiteSpace);
                    if (countbl == 1)
                    {
                        int ps = nameho.IndexOf(" ");
                        var secWor = nameho.Substring(ps + 1, nameho.Length - ps - 1);
                        SubHomeTe = secWor;
                    }
                    else if (countbl > 1)
                    {
                        int index = nameho.IndexOf(" ", nameho.IndexOf(" ") + 1);
                        int ps = nameho.IndexOf(" ");
                        var secWor = nameho.Substring(ps + 1, index - ps - 1);
                        SubHomeTe = secWor;
                    }
                    else if (countbl == 0)
                    {
                        SubHomeTe = nameho;
                    }

                    if (nameho == "QPR")
                    {
                        nameho = "Queens";
                    }

                    //if (nameho == "Stade Rennais")
                    //{
                    //    nameho = "Rennes";
                    //}

                    var firstWord = nameho.IndexOf(" ") > -1 ? nameho.Substring(0, nameho.IndexOf(" ")) : nameho;
                    HomeTe = firstWord;

                    if (homeTmp.ToLower().Contains(HomeTe.ToLower()) || homeTmp.ToLower().Contains(SubHomeTe.ToLower()))
                    {
                        string SubAwayTe = "";
                        string AwayTe = "";
                        string nameaw = y.awayTeam.name;
                        int countbl2 = nameaw.Count(Char.IsWhiteSpace);
                        if (countbl2 == 1)
                        {
                            int ps = nameaw.IndexOf(" ");
                            var secWor = nameaw.Substring(ps + 1, nameaw.Length - ps - 1);
                            SubAwayTe = secWor;
                        }
                        else if (countbl2 > 1)
                        {
                            int index = nameaw.IndexOf(" ", nameaw.IndexOf(" ") + 1);
                            int ps = nameaw.IndexOf(" ");
                            var secWor = nameaw.Substring(ps + 1, index - ps - 1);
                            SubAwayTe = secWor;
                        }
                        else if (countbl2 == 0)
                        {
                            SubAwayTe = nameaw;
                        }

                        if (nameaw == "QPR")
                        {
                            nameaw = "Queens";
                        }
                        //if (awayTmp == "Rennes")
                        //{
                        //    awayTmp = "Stade Rennais";
                        //}

                        var firstWord2 = nameaw.IndexOf(" ") > -1 ? nameaw.Substring(0, nameaw.IndexOf(" ")) : nameaw;
                        AwayTe = firstWord2;
                        if (awayTmp.ToLower().Contains(AwayTe.ToLower()) || awayTmp.ToLower().Contains(SubAwayTe.ToLower()))
                        {
                            id = y.id;
                            return id;
                            break;
                        }
                    }
                    //else
                    //{
                    //    break;
                    //}
                }
            }
            return id;
        }

        public static string ReverseWords(string sentence)
        {
            string res = "";
            int temp;
            string[] a = sentence.Split('-');
            int k = a.Length - 1;
            temp = k;
            for (int i = k; temp >= 0; k--)
            {
                res += a[temp] + '-';
                --temp;
            }
            return res;
        }
    }
}