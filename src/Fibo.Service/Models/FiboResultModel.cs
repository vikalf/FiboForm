using System.Collections.Generic;

namespace Fibo.Service.Models
{
    public class FiboResultModel
    {
        public int FiboNumeral { get; set; }
        public Dictionary<int, int> VisitedValues { get; set; }
    }
}
