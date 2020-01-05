using System.Collections.Generic;

namespace FiboForm.Api.Models
{
    public class VisitedValuesModel
    {
        public List<int> Indexes { get; set; }
        public Dictionary<int, int> VisitedValues { get; set; }
    }
}
