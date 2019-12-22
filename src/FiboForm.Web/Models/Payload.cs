using System.Collections.Generic;

namespace FiboForm.Web.Models
{
    public class Payload
    {
        public List<int> VisitedIndexes { get; set; }
        public List<VisitedValues> VisitedValues { get; set; }
    }

    public class VisitedValues
    {
        public int Index { get; set; }
        public int Value { get; set; }
    }
}
