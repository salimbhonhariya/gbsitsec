using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gbsitsec.Models
{
    public class MailGunList
    {
        [Key]
        public int Id { get; set; }

        public List<Item> items { get; set; }
        public Paging paging { get; set; }

        public class Item
        {
            public string access_level { get; set; }
            public string address { get; set; }
            public string created_at { get; set; }
            public string description { get; set; }
            public int members_count { get; set; }

            [Key]
            public string name { get; set; }
        }

        public class Paging
        {
            [Key]
            public string first { get; set; }

            public string last { get; set; }
            public string next { get; set; }
            public string previous { get; set; }
        }
    }
}