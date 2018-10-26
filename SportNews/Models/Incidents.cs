using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportNews.Models
{
    public class Incidents
    {
        public string type { get; set; }
        public string InciType { get; set; }
        public int time { get; set; }
        public string from { get; set; }
        public string PlayerIn { get; set; }
        public string PlayerOut { get; set; }
        public string Player { get; set; }
        public int scoringTeam { get; set; }
        public string content { get; set; }
    }
}