using System.ComponentModel.DataAnnotations;

namespace gbsitsec.ViewModel
{
    public class PagingListViewModel
    {
        [Key]
        public string first { get; set; }

        public string last { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
    }
}