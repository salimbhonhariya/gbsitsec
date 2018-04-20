using System.ComponentModel.DataAnnotations;

namespace gbsitsec.ViewModel
{
    public class ItemListViewModel
    {
        public string access_level { get; set; }
        public string address { get; set; }
        public string created_at { get; set; }
        public string description { get; set; }
        public int members_count { get; set; }
        public bool subscribed { get; set; }


        [Key]
        public string name { get; set; }
    }
}