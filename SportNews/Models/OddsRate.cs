using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportNews.Models
{
    public class OddsRate
    {
        //public decimal FtHomeTeam { get; set; }
        //public decimal FtAwayTeam { get; set; }
        //public decimal FtDraw { get; set; }
        //public decimal DbChanceHome { get; set; }
        //public decimal DbChanceAway { get; set; }
        //public decimal DbChanceNoDraw { get; set; }
        //public decimal DrawNoBtHome { get; set; }
        //public decimal DrawNoBtAway { get; set; }
        //public decimal HtHomeTeam { get; set; }
        //public decimal HtAwayTeam { get; set; }
        //public decimal HtDraw { get; set; }
        //public decimal BothScYes { get; set; }
        //public decimal BothScNo { get; set; }
        //public decimal FirstScHome { get; set; }
        //public decimal FirstScAway { get; set; }
        //public decimal NoScore { get; set; }
        public string HomeWn { get; set; }
        public string AwayWn { get; set; }
        public string Draw { get; set; }
        public string HomeChoice { get; set; }
        public string AwayChoice { get; set; }
        public string DrawChoice { get; set; }
        public string AsaHome { get; set; }
        public string AsaAway { get; set; }
        public string AsaDraw { get; set; }
        public string OverChoice { get; set; }
        public string UnderChoice { get; set; }
        public string TotalChoice { get; set; }
        public string timeMt { get; set; }
        public string HomeTe { get; set; }
        public string AwayTe { get; set; }
        public string scale { get; set; }
        public string SubHomeTe { get; set; }
        public string SubAwayTe { get; set; }
        public string DpTime { get; set; }
        public string NaLig { get; set; }
        public int mark { get; set; }

        //1st
        public string St1HomeWn { get; set; }
        public string St1AwayWn { get; set; }
        public string St1Draw { get; set; }
        public string St1HomeChoice { get; set; }
        public string St1AwayChoice { get; set; }
        public string St1DrawChoice { get; set; }
        public string St1OverChoice { get; set; }
        public string St1UnderChoice { get; set; }
        public string St1TotalChoice { get; set; }

        public int cate { get; set; }
        public List<MatchGoals> GoaLst;
    }

    public class MatchGoals
    {
        public string goal { get; set; }
        public string OverGo { get; set; }
        public string UnderGo { get; set; }
    }

    public class CatOdds
    {
        public string name { get; set; }
        public int type { get; set; }
        public List<OddsRate> OddLst;
    }

    public class SumLst
    {
        public List<CatOdds> LstAll;
    }
}