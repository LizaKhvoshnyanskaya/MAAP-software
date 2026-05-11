using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fish_School_Search
{
    public class OptimizationResult
    {
        public double[] BestX { get; set; }
        public double BestValue { get; set; }
        public List<double[]> FinalPopulation { get; set; }

        public OptimizationResult()
        {
            FinalPopulation = new List<double[]>();
        }
    }
}
