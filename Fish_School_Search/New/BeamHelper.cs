using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fish_School_Search
{
    public static class BeamHelper
    {
        private static Random rand = new Random();

        public static List<double[]> GetInitialPopulation(int count, double[,] bounds, int n)
        {
            var list = new List<double[]>();
            for (int i = 0; i < count; i++)
            {
                var v = new double[n];
                for (int j = 0; j < n; j++)
                {
                    double min = bounds[j, 0];
                    double max = bounds[j, 1];
                    v[j] = min + rand.NextDouble() * (max - min);
                }
                list.Add(v);
            }
            return list;
        }

        public static List<System.Tuple<double, double[]>> EvaluatePopulation(List<double[]> pop, Func<double[], double> func)
        {
            var res = new List<System.Tuple<double, double[]>>();
            for (int i = 0; i < pop.Count; i++)
            {
                res.Add(System.Tuple.Create(func(pop[i]), pop[i]));
            }
            return res;
        }

        public static int GetDescendantNumber(int Smin, int Smax, double fMax, double fMin, double fCurrent)
        {
            if (Math.Abs(fMax - fMin) < 1e-12) return Smin;
            double val = Smin + ((double)(Smax - Smin) / (fMax - fMin)) * (fMax - fCurrent);
            int r = (int)val;
            if (r < Smin) r = Smin;
            if (r > Smax) r = Smax;
            return r;
        }

        public static double GetMeanSquareDeviation(int k, int ITER, int p, double sigmaInitial, double sigmaFinal)
        {
            return Math.Pow((ITER - k) / (double)ITER, p) * (sigmaInitial - sigmaFinal) + sigmaFinal;
        }

        public static List<double[]> GetDescendants(double[] parent, double sigma, int count, double[,] bounds)
        {
            var list = new List<double[]>();
            for (int i = 0; i < count; i++)
            {
                var d = new double[parent.Length];
                for (int j = 0; j < parent.Length; j++)
                {
                    d[j] = parent[j] + sigma * NextGaussian();
                    // clamp to bounds
                    double min = bounds[j, 0];
                    double max = bounds[j, 1];
                    if (d[j] < min) d[j] = min;
                    if (d[j] > max) d[j] = max;
                }
                list.Add(d);
            }
            return list;
        }

        // pop: list of (parentValue, parentVector, descendantsList)
        public static void SearchTowardsLeader(
            List<System.Tuple<double, double[], List<double[]>>> pop,
            double[] leader,
            double ksi,
            int k,
            int ITER,
            double omega,
            int NSTEP,
            int tau,
            double s,
            Func<double[], double> targetFunc,
            double[,] bounds)
        {
            int step = (int)(omega * NSTEP);
            if (step < 1) step = 1;

            double r = (k == 1) ? -1 : -1 - k / (double)ITER;

            for (int idx = 0; idx < pop.Count; idx++)
            {
                var tuple = pop[idx];
                double[] parent = tuple.Item2;
                List<double[]> descendants = tuple.Item3;

                for (int m = 0; m < descendants.Count; m++)
                {
                    double[] current = descendants[m];
                    double best = targetFunc(current);

                    if (ksi <= 0.4)
                    {
                        for (int i = 0; i < step; i++)
                        {
                            double[] delta = Sub(leader, current);
                            double[] next = Add(current, Scale(delta, i / (double)NSTEP));
                            ClampInPlace(next, bounds);
                            double f = targetFunc(next);
                            if (f < best)
                            {
                                best = f;
                                descendants[m] = next;
                            }
                        }
                    }
                    else if (ksi < 0.6)
                    {
                        for (int i = 0; i < step; i++)
                        {
                            double[] delta = Sub(leader, parent);
                            double[] next = Add(current, Scale(delta, i / (double)NSTEP));
                            ClampInPlace(next, bounds);
                            double f = targetFunc(next);
                            if (f < best)
                            {
                                best = f;
                                descendants[m] = next;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < tau; i++)
                        {
                            double t = r - 1 + rand.NextDouble();
                            double[] direction = Abs(Sub(leader, current));
                            double[] spiralMove = Scale(direction, Math.Exp(s * t) * Math.Cos(2 * Math.PI * t));
                            double[] next = Add(current, spiralMove);
                            ClampInPlace(next, bounds);
                            double f = targetFunc(next);
                            if (f < best)
                            {
                                best = f;
                                descendants[m] = next;
                            }
                        }
                    }
                }
            }
        }

        public static List<System.Tuple<double, double[]>> SelectBestDescendants(
            List<System.Tuple<double, double[], List<double[]>>> pop,
            Func<double[], double> func)
        {
            var result = new List<System.Tuple<double, double[]>>();

            for (int i = 0; i < pop.Count; i++)
            {
                var tuple = pop[i];
                double[] parent = tuple.Item2;
                List<double[]> descendants = tuple.Item3;

                double bestVal = func(parent);
                double[] bestVec = parent;

                for (int j = 0; j < descendants.Count; j++)
                {
                    double f = func(descendants[j]);
                    if (f < bestVal)
                    {
                        bestVal = f;
                        bestVec = descendants[j];
                    }
                }

                result.Add(System.Tuple.Create(bestVal, bestVec));
            }

            return result;
        }

        public static double[] Add(double[] a, double[] b)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++) r[i] = a[i] + b[i];
            return r;
        }

        public static double[] Sub(double[] a, double[] b)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++) r[i] = a[i] - b[i];
            return r;
        }

        public static double[] Scale(double[] a, double f)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++) r[i] = a[i] * f;
            return r;
        }

        public static double[] Abs(double[] a)
        {
            double[] r = new double[a.Length];
            for (int i = 0; i < a.Length; i++) r[i] = Math.Abs(a[i]);
            return r;
        }

        private static void ClampInPlace(double[] v, double[,] bounds)
        {
            for (int i = 0; i < v.Length; i++)
            {
                double min = bounds[i, 0];
                double max = bounds[i, 1];
                if (v[i] < min) v[i] = min;
                if (v[i] > max) v[i] = max;
            }
        }

        public static double NextGaussian()
        {
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        }

        public static double GetRandom(double min, double max)
        {
            return min + rand.NextDouble() * (max - min);
        }
    }
}
