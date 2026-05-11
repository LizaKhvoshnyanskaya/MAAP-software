using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fish_School_Search
{
    public class GenerationBeamsModel
    {
        public int NP { get; set; }
        public int ITER { get; set; }
        public int Smax { get; set; }
        public int Smin { get; set; }
        public int p { get; set; }
        public double sigmaInitial { get; set; }
        public double sigmaFinal { get; set; }
        public int nCount { get; set; } // для нас будет 2
        public double omega { get; set; }
        public int NSTEP { get; set; }
        public int tau { get; set; }
        public double s { get; set; }
        public double knownMinimum { get; set; } // у тебя это exact (но это максимум f*)
        public double? x0 { get; set; }
        public int FunctionNumber { get; set; }

        // bounds[j,0]=min, bounds[j,1]=max; для 2D: bounds[0,*] для x, bounds[1,*] для y
        public double[,] Bounds { get; set; }

        // === Forecast / Multi-step params (из Python) ===
        public int Forecast_N { get; set; } = 5;          // N
        public int Forecast_P { get; set; } = 10;         // P
        public int Forecast_K { get; set; } = 20;         // K
        public double Forecast_R { get; set; } = 1e-8;    // R
        public double Forecast_alpha { get; set; } = 0.85;
        public double Forecast_eta { get; set; } = 0.89;
        public double Forecast_beta { get; set; } = 0.3;
        public double Forecast_lambda { get; set; } = 1.5;


        // === дискрет дин модели движения агентов ===
        public int Dds_NP { get; set; }
        public int Dds_Pmax { get; set; }
        public int Dds_K { get; set; }
        public double Dds_h { get; set; }
        public double Dds_qWeight { get; set; }
        public double Dds_sWeight { get; set; }
        public double Dds_lambdaWeight { get; set; }

    }
}
