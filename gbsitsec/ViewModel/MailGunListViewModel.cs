using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gbsitsec.ViewModel
{
    public class MailGunItemViewModel
    {
        [Key]
        public int Id { get; set; }

        public List<ItemListViewModel> items { get; set; }

        public string first { get; set; }
        public string last { get; set; }
        public string next { get; set; }

        public string previous { get; set; }
    }
}