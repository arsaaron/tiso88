using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportNews.Models
{
    public class AsianInfo
    {
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string TImeMatch { get; set; }
        public string HomeAsianHd { get; set; }
        public string AwayAsianHd { get; set; }
        public string RateHomeAsianHd { get; set; }
        public string RateAwayAsianHd { get; set; }
        public string GoalLineAway { get; set; }
        public string GoalLineHome { get; set; }
        public string GoalLine { get; set; }
        public string HomeHalfAs { get; set; }
        public string AwayHalfAs { get; set; }
        public string RateHomeHalfAs { get; set; }
        public string RateAwayHalfAs { get; set; }
        public string HalfGl { get; set; }
        public string HomeHalfGl { get; set; }
        public string AwayHalfGl { get; set; }
        public string totalCorner { get; set; }
        public string totalHomeCorner { get; set; }
        public string totalAwayCorner { get; set; }
        public string halfAsCorner { get; set; }
        public string halfHomeAsCorner { get; set; }
        public string halfAwayAsCorner { get; set; }
        public string AsRtHomeCorner { get; set; }
        public string AsHomeCorner { get; set; }
        public string AsAwayCorner { get; set; }
        public string AsRtAwayCorner { get; set; }
        public string totalCard { get; set; }
        public string totalHomeCard { get; set; }
        public string totalAwayCard { get; set; }
        public string AsHomeCard { get; set; }
        public string AsAwayCard { get; set; }
        public string AsRateHomeCard { get; set; }
        public string AsRateAwayCard { get; set; }

    }

    public class AsLeague
    {
        public string NaLeague { get; set; }
        public List<AsianInfo> AsianLst { get; set; }
    }
}