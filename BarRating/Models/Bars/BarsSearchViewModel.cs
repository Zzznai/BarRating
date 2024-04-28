using BarRating.Data.Entities;

namespace BarRating.Models.Bars
{
    public class BarsSearchViewModel
    {
        public string Name { get; set; }
        public List<Bar> Bars { get; set; }
    }
}
