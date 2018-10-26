using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportNews.Models
{
    public class SeasonInfo
    {
        public SeasonInfo(string name, int year, long yearid, int id)
        {
            this.Name = name;
            this.Year = year;
            this.yearId = yearid;
            this.Id = id;
        }
        public string Name { get; set; }
        public int Year { get; set; }
        public long yearId { get; set; }
        public int Id { get; set; }

    }
}