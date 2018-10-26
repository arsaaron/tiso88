using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportNews.Models
{
    public class MatchInfoModel
    {
        public long MatchId { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string DateMatch { get; set; }
        public string TimeMatch { get; set; }
        public string HomeFlag { get; set; }
        public string AwayFlag { get; set; }
        public string StatusMatch { get; set; }
        public string HomeScore { get; set; }
        public string AwayScore { get; set; }
        public int RoundInfo { get; set; }
        public long HomeId { get; set; }
        public long AwayId { get; set; }
        public string timeSt { get; set; }
        public string status { get; set; }
        public string timeIn { get; set; }
        public string ssName { get; set; }
        public List<StandingModel> StdGroup;
        public SumLst ore;

    }

    public class NewMatchInfo
    {
        public long id { get; set; }
        public roundin roundInfo { get; set; }
        public statusin status { get; set; }
        public hmTeamin homeTeam { get; set; }
        public awTeamin awayTeam { get; set; }
        public hmScore homeScore { get; set; }
        public AwScore awayScore { get; set; }
        public double startTimestamp { get; set; }
        public string statusDescription { get; set; }

        public SumLst ore;

    }

    public class roundin
    {
        public int round { get; set; }
    }
    public class statusin
    {
        public int code { get; set; }
    }
    public class hmTeamin
    {
        public long id { get; set; }
        public string name { get; set; }

    }
    public class awTeamin
    {
        public long id { get; set; }
        public string name { get; set; }
    }
    public class hmScore
    {
        public int current { get; set; }
        public int period1 { get; set; }
        public int normaltime { get; set; }

    }
    public class AwScore
    {
        public int current { get; set; }
        public int period1 { get; set; }
        public int normaltime { get; set; }
    }

    public class seasonName
    {
        public string name { get; set; }
    }

    public class eventObj
    {
        public List<NewMatchInfo> events;
        public seasonName tournament;
    }
}