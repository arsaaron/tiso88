using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportNews.Models
{
    public class Formation
    {
        public long MatchId { get; set; }
        public string homeFormation { get; set; }
        public string awayFormation { get; set; }
        public string homeManager { get; set; }
        public string awayManager { get; set; }
        public List<PlayerInfo> homeLst;
        public List<PlayerInfo> awayLst;
        public string homePlayerColNum { get; set; }
        public string homePlayerColLine { get; set; }
        public string homeGkColNum { get; set; }
        public string homeGkColLine { get; set; }
        public string awayPlayerColNum { get; set; }
        public string awayPlayerColLine { get; set; }
        public string awayGkColNum { get; set; }
        public string awayGkColLine { get; set; }
        public List<Incidents> homeIncidents;
        public List<Incidents> awayIncidents;
        public int lenghHome { get; set; }
        public int lenghAway { get; set; }
        public List<LineInfo> HomeLineLst;
        public List<LineInfo> AwayLineLst;
        public string photoHome { get; set; }
        public string photoAway { get; set; }

        public List<PlayerInfo> subHomeLst;
        public List<PlayerInfo> subAwayLst;
        public string HomeName { get; set; }
        public string AwayName { get; set; }
        public string HomeFlag { get; set; }
        public string AwayFlag { get; set; }

        public List<Incidents> goalHomeLst;
        public List<Incidents> goalAwayLst;
        public List<Incidents> goalLst;
        public string FtResult { get; set; }
        public string HomeScore { get; set; }
        public string AwayScore { get; set; }
        public List<lineStats> statsInfo;
        public List<formHmTeam> formHmLst;
        public List<formAwTeam> formAwLst;
        public HeadStas hs2 { get; set; }
    }

    public class LineInfo
    {
        public int lineLengh { get; set; }
        public List<PlayerInfo> PlayerLine;
    }

    public class lineUp
    {
        public string homeFormation { get; set; }
        public string awayFormation { get; set; }
        public List<playerDetail> home { get; set; }
        public List<playerDetail> away { get; set; }
        public homeMan homeManager { get; set; }
        public awayMan awayManager { get; set; }
        //public List<incideHome> homeIncidents { get; set; }
        //public List<incideAway> awayIncidents { get; set; }

    }



    public class playerDetail
    {
        public playerName player { get; set; }
        public int position { get; set; }
        public string shirtNumber { get; set; }
        public bool substitute { get; set; }
        public string positionName { get; set; }
        public bool captain { get; set; }
    }

    public class playerName
    {
        public string name { get; set; }
        public long id { get; set; }
        public string shortName { get; set; }
    }

    public class incideHome
    {
        public string type { get; set; }
        public string incidentType { get; set; }
        public int time { get; set; }
        public string from { get; set; }
        public inPlayer playerIn { get; set; }
        public outPlayer playerOut { get; set; }
        public cardPlayer player { get; set; }
        public asPlayer assist1 { get; set; }
        public int scoringTeam { get; set; }
        public int playerTeam { get; set; }
        public int homeScore { get; set; }
        public int awayScore { get; set; }
        public string text { get; set; }
    }

    public class incideAway
    {
        public string type { get; set; }
        public string incidentType { get; set; }
        public int time { get; set; }
        public string from { get; set; }
        public inPlayer playerIn { get; set; }
        public outPlayer playerOut { get; set; }
        public cardPlayer player { get; set; }
        public asPlayer assist1 { get; set; }
        public int scoringTeam { get; set; }
        public int playerTeam { get; set; }
        public int homeScore { get; set; }
        public int awayScore { get; set; }
        public string text { get; set; }
    }

    public class cardPlayer
    {
        public string name { get; set; }
        public string shortName { get; set; }
        public long id { get; set; }
    }

    public class inPlayer
    {
        public string name { get; set; }
        public string shortName { get; set; }
        public long id { get; set; }
    }

    public class outPlayer
    {
        public string name { get; set; }
        public string shortName { get; set; }
        public long id { get; set; }
    }

    public class asPlayer
    {
        public string name { get; set; }
        public string shortName { get; set; }
        public long id { get; set; }
    }

    public class MatchIncident
    {
        public List<incide> InciLst { get; set; }
    }

    public class homeMan
    {
        public string name { get; set; }
        public long id { get; set; }
    }

    public class awayMan
    {
        public string name { get; set; }
        public long id { get; set; }
    }

    public class incide
    {
        public string type { get; set; }
        public string incidentType { get; set; }
        public int time { get; set; }
        public string from { get; set; }
        public inPlayer playerIn { get; set; }
        public outPlayer playerOut { get; set; }
        public cardPlayer player { get; set; }
        public asPlayer assist1 { get; set; }
        public int scoringTeam { get; set; }
        public int playerTeam { get; set; }
        public int homeScore { get; set; }
        public int awayScore { get; set; }
        public string text { get; set; }
        public int length { get; set; }
    }

    public class oppo
    {
        public string name { get; set; }
        public long id { get; set; }
    }

    public class formHmTeam
    {
        public string winFlag { get; set; }
        public oppo opponent { get; set; }
    }

    public class formAwTeam
    {
        public string winFlag { get; set; }
        public oppo opponent { get; set; }
    }

    public class objMatch
    {
        public spItem sportItem { get; set; }
    }

    public class spItem
    {
        public List<tourna> tournaments { get; set; }
    }

    public class tourna
    {
        public List<smEvent> events { get; set; }
    }

    public class groupEvent
    {
        public smEvent sme { get; set; }
    }

    public class smEvent
    {
        public homeMan homeTeam { get; set; }
        public awayMan awayTeam { get; set; }
        public long id { get;set;}
    }
}