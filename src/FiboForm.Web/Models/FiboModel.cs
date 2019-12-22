using System.Collections.Generic;
using System.Linq;

namespace FiboForm.Web.Models
{
    public class FiboModel : FiboInputModel
    {
        public List<int> VisitedIndexes { get; set; }
        public List<VisitedValue> VisitedValues { get; set; }
        public int? FiboNumber { get 
            {
                if (Index.HasValue && VisitedValues.Any(e => e.Index == Index.Value))
                    return VisitedValues.First(e => e.Index == Index.Value).Value;
                else
                    return null;
            } 
        }

    }

    public class VisitedValue
    {
        public int Index { get; set; }
        public int Value { get; set; }

    }

    public class FiboInputModel
    {
        public int? Index { get; set; }
        public string ErrorMessage { get; set; }
    }
}
