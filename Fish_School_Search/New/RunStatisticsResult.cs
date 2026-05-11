using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fish_School_Search.New
{
    public class RunStatisticsResult
    {
        public int RepeatCount { get; set; }
        public double MeanValue { get; set; }
        public double BestValue { get; set; }
        public double MeanError { get; set; }
        public double BestError { get; set; }
        public double Sigma { get; set; }
        public int GoodCount { get; set; }

        public double BestValueOverall { get; set; }
        public double[] BestPointOverall { get; set; }

        public List<double> Values { get; set; } = new List<double>();
        public List<double> Errors { get; set; } = new List<double>();
    }
}
