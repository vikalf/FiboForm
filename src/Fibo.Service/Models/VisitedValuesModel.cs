using System.Collections.Generic;

namespace Fibo.Service.Models
{
    public class VisitedValuesModel
    {
        public List<int> Indexes { get; set; }
        public Dictionary<int, int> VisitedValues { get; set; }
    }
}
