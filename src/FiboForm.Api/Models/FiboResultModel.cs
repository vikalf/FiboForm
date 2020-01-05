using System.Collections.Generic;

namespace FiboForm.Api.Models
{
    public class FiboResultModel
    {
        public int FiboNumeral { get; set; }
        public Dictionary<int, int> VisitedValues { get; set; }
    }
}
