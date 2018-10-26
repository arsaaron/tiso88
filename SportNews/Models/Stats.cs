using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportNews.Models
{
    public class Stats
    {
        //public int homeBallPossession { get; set; }
        //public int awayBallPossession { get; set; }
        //public int homeCornerKicks { get; set; }
        //public int awayCornerKicks { get; set; }
        //public int homeShotsOnGoal { get; set; }
        //public int awayShotsOnGoal { get; set; }
        //public int homeShotsOffGoal { get; set; }
        //public int awayShotsOffGoal { get; set; }
        //public int homeOffsides { get; set; }
        //public int awayOffsides { get; set; }
        //public int homeGoalkeeperSaves { get; set; }
        //public int awayGoalkeeperSaves { get; set; }
        //public int homeFouls { get; set; }
        //public int awayFouls { get; set; }
        //public int homeYellowCards { get; set; }
        //public int awayYellowCards { get; set; }
        //public int homeRedCards { get; set; }
        //public int awayRedCards { get; set; }
        public string home { get; set; }
        public string away { get; set; }
        public string name { get; set; }
    }

    public class lineStats
    {
        public string groupName { get; set; }
        public List<Stats> statItems;
    }

    public class staList
    {
        public List<statis> periods;
    }

    public class statis
    {
        public string period { get; set; }
        public List<StatDetail> groups;
    }

    public class StatDetail
    {
        public string groupName { get; set; }
        public List<Stats> statisticsItems;
    }

    public class DetailEvent
    {
        public tour game { get; set; }
    }

    public class tour
    {
        public List<tourEvt> tournaments { get; set; }
    }

    public class tourEvt
    {
        public List<Evts> events { get; set; }
    }

    public class Evts
    {
        public homeT homeTeam { get; set; }
        public awayT awayTeam { get; set; }
        public homeSc homeScore { get; set; }
        public awaySc awayScore { get; set; }

    }

    public class homeT
    {
        public string name { get; set; }
        public string shortName { get; set; }
        public long id { get; set; }
    }

    public class awayT
    {
        public string name { get; set; }
        public string shortName { get; set; }
        public long id { get; set; }
    }

    public class homeSc
    {
        public string current { get; set; }
        public string period1 { get; set; }
        public string normaltime { get; set; }
    }

    public class awaySc
    {
        public string current { get; set; }
        public string period1 { get; set; }
        public string normaltime { get; set; }
    }

    public class HeadStas
    {
        public headLs h2h { get; set; }
        public homeLs home { get; set; }
        public awayLs away { get; set; }
    }

    public class headLs
    {
        public evtHead events { get; set; }
    }

    public class evtHead
    {
        public List<infoTour> tournaments { get; set; }
    }

    public class infoTour
    {
        public List<tourMats> events { get; set; }
    }

    public class tourMats
    {
        public homeT homeTeam { get; set; }
        public awayT awayTeam { get; set; }
        public homeSc homeScore { get; set; }
        public awaySc awayScore { get; set; }
        public string startTimestamp { get; set; }
    }

    public class homeLs
    {
        public evtHead playedAtThisTournament { get; set; }
    }

    public class awayLs
    {
        public evtHead playedAtThisTournament { get; set; }
    }
}