using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportNews.Models
{
    public class PlayerInfo
    {
        public string PlayerName { get; set; }
        public long PlayerId { get; set; }
        public int position { get; set; }
        public string shirtNumber { get; set; }
        public bool substitute { get; set; }
        public string positionName { get; set; }
        public bool captain { get; set; }
        public string shortName { get; set; }

    }
}