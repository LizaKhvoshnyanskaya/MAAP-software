using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fish_School_Search
{
    public static class TargetFunctions
    {
        public static double Evaluate(double x1, double x2, double x3, double x4, int f)
        {
            switch (f)
            {
                case 0:
                    return x1 * Math.Sin(Math.Sqrt(Math.Abs(x1))) + x2 * Math.Sin(Math.Sqrt(Math.Abs(x2)));
                case 1:
                    return x1 * Math.Sin(4 * Math.PI * x1) - x2 * Math.Sin(4 * Math.PI * x2 + Math.PI) + 1;
                case 2:
                    {
                        var c6 = Cpow(x1, x2, 6);
                        return 1.0 / (1.0 + Math.Sqrt((c6[0] - 1) * (c6[0] - 1) + c6[1] * c6[1]));
                    }
                case 3:
                    return 0.5 - (Math.Pow(Math.Sin(Math.Sqrt(x1 * x1 + x2 * x2)), 2) - 0.5) /
                                 (1 + 0.001 * (x1 * x1 + x2 * x2));
                case 4:
                    return -x1 * x1 - x2 * x2 + 10 * Math.Cos(2 * Math.PI * x1) + 10 * Math.Cos(2 * Math.PI * x2);
                case 5:
                    return -Math.E + 20 * Math.Exp(-0.2 * Math.Sqrt((x1 * x1 + x2 * x2) / 2.0)) +
                           Math.Exp((Math.Cos(2 * Math.PI * x1) + Math.Cos(2 * Math.PI * x2)) / 2.0);
                case 6:
                    return Math.Pow(Math.Cos(2 * x1 * x1) - 1.1, 2) + Math.Pow(Math.Sin(0.5 * x1) - 1.2, 2)
                         - Math.Pow(Math.Cos(2 * x2 * x2) - 1.1, 2) + Math.Pow(Math.Sin(0.5 * x2) - 1.2, 2);
                case 7:
                    return -Math.Sqrt(Math.Abs(Math.Sin(Math.Sin(
                               Math.Sqrt(Math.Abs(Math.Sin(x1 - 1))) + Math.Sqrt(Math.Abs(Math.Sin(x2 + 2))))))) + 1;
                case 8:
                    return -(1 - x1) * (1 - x1) - 100 * (x2 - x1 * x1) * (x2 - x1 * x1);
                case 9:
                    return 1 - x1 * x1 - x2 * x2;
                default:
                    throw new ArgumentOutOfRangeException(nameof(f));
            }
        }

        private static double[] Cpow(double x, double y, int p)
        {
            double[] Cp = { x, y };
            for (int i = 1; i < p; i++)
            {
                double x0 = Cp[0] * x - Cp[1] * y;
                double y0 = Cp[1] * x + Cp[0] * y;
                Cp[0] = x0; Cp[1] = y0;
            }
            return Cp;
        }
    }
}
