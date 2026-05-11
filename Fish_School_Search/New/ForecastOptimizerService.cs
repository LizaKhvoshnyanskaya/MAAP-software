using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fish_School_Search.New
{
    public class ForecastOptimizerService
    {
        private readonly Random _rand = new Random();

        public OptimizationResult RunOptimizer(GenerationBeamsModel model)
        {
            int n = model.nCount; // у тебя 2
            double[,] bounds = model.Bounds;

            // Внутренне минимизируем (-f) => максимизируем f
            Func<double[], double> fMin = v =>
            {
                double fx = TargetFunctions.Evaluate(
                    v[0],
                    v[1],
                    v.Length > 2 ? v[2] : 0,
                    v.Length > 3 ? v[3] : 0,
                    model.FunctionNumber
                );
                return -fx;
            };

            int N = model.Forecast_N;
            int P = model.Forecast_P;
            int K = model.Forecast_K;
            double R = model.Forecast_R;
            double alpha = model.Forecast_alpha;
            double eta = model.Forecast_eta;
            double beta = model.Forecast_beta;
            double lambda = model.Forecast_lambda;

            // старт
            double[] x0 = GetInitialApproximation(n, bounds);
            double t0 = GetBaseT0(bounds);

            var pool = new List<Tuple<double[], double>>();   // (x, fMin(x))
            var trajectory = new List<double[]>();            // точки (для отрисовки)

            double bestVal = double.MaxValue;
            double[] bestX = null;

            for (int p = 1; p <= P; p++)
            {
                // собираем путь текущего прохода
                var passPath = new List<double[]>();
                double[] xBestPass = RunPass(x0, t0, fMin, K, N, R, alpha, bounds, passPath);

                double fxMin = fMin(xBestPass);
                pool.Add(Tuple.Create((double[])xBestPass.Clone(), fxMin));

                // добавим весь путь в общую траекторию
                trajectory.AddRange(passPath);

                if (fxMin < bestVal)
                {
                    bestVal = fxMin;
                    bestX = (double[])xBestPass.Clone();
                }

                if (p < P)
                {
                    t0 = GenerateNewT0(p, eta, bounds);
                    x0 = GenerateNewStartPoint(xBestPass, bounds, beta, lambda, p);
                }
            }

            if (bestX == null)
            {
                // на всякий случай
                var best = pool.OrderBy(t => t.Item2).First();
                bestX = best.Item1;
                bestVal = best.Item2;
            }

            // наружу вернём max f
            double bestMaxF = -bestVal;

            return new OptimizationResult
            {
                BestX = (double[])bestX.Clone(),
                BestValue = bestMaxF,
                FinalPopulation = trajectory.Count > 0 ? trajectory : new List<double[]> { (double[])bestX.Clone() }
            };
        }

        // ====== PASS ======
        private double[] RunPass(
            double[] x0,
            double t0,
            Func<double[], double> f,
            int K,
            int N,
            double R,
            double alpha,
            double[,] bounds,
            List<double[]> passPath)
        {
            int k = 0;
            double[] xkPrev = (double[])x0.Clone();
            double[] xk = (double[])x0.Clone();
            double t = t0;

            passPath.Add((double[])xk.Clone());

            while (k < K)
            {
                double[] z = GetForecastedPoint(xk, xkPrev, k + 1, bounds);
                var (next, newT) = SearchNearby(z, t, f, bounds, N, R, alpha);

                t = newT;

                if (next == null)
                    return xk;

                xkPrev = xk;
                xk = next;
                passPath.Add((double[])xk.Clone());

                k++;
            }

            return xk;
        }

        // ====== FORECAST ======
        private double[] GetForecastedPoint(double[] xk, double[] xkPrev, int k, double[,] bounds)
        {
            int n = xk.Length;
            double[] z = new double[n];

            double factor = 1.0 - Math.Exp(-k / 5.0);
            double randVal = NextUniform(0.0, 1.0);

            for (int i = 0; i < n; i++)
            {
                double delta = xk[i] - xkPrev[i];
                double zi = xk[i] + factor * delta * randVal;

                zi = Clamp(zi, bounds[i, 0], bounds[i, 1]);
                z[i] = zi;
            }

            return z;
        }

        // ====== LOCAL SEARCH ======
        private (double[] next, double t) SearchNearby(
            double[] z,
            double step,
            Func<double[], double> f,
            double[,] bounds,
            int N,
            double R,
            double alpha)
        {
            int n = z.Length;
            double t = step;

            while (true)
            {
                var successful = new List<Tuple<double[], double>>();
                double fz = f(z);

                for (int attempt = 0; attempt < N; attempt++)
                {
                    // случайное направление [-1;1], нормируем
                    double[] dir = new double[n];
                    double norm2 = 0.0;
                    for (int i = 0; i < n; i++)
                    {
                        double d = 2.0 * NextUniform(0.0, 1.0) - 1.0;
                        dir[i] = d;
                        norm2 += d * d;
                    }

                    if (norm2 <= 1e-18) continue;
                    double norm = Math.Sqrt(norm2);
                    for (int i = 0; i < n; i++) dir[i] /= norm;

                    // y = z + t*dir
                    double[] y = new double[n];
                    bool inside = true;
                    for (int i = 0; i < n; i++)
                    {
                        y[i] = z[i] + t * dir[i];
                        if (y[i] < bounds[i, 0] || y[i] > bounds[i, 1])
                        {
                            inside = false;
                            break;
                        }
                    }

                    if (!inside) continue;

                    double fy = f(y);
                    if (fy < fz)
                        successful.Add(Tuple.Create(y, fy));
                }

                if (successful.Count > 0)
                {
                    // сортируем по лучшему fy (минимум)
                    successful.Sort((a, b) => a.Item2.CompareTo(b.Item2));

                    int s = successful.Count;

                    // weights = (s+1-j)/s, j=1..s
                    double[] delta = new double[n];

                    for (int j = 0; j < s; j++)
                    {
                        double w = (s - j) / (double)s; // (s+1-(j+1))/s = (s-j)/s
                        double[] yj = successful[j].Item1;
                        for (int i = 0; i < n; i++)
                            delta[i] += w * (yj[i] - z[i]);
                    }

                    double scale = 2.0 / (s + 1.0);
                    double[] xNext = new double[n];
                    for (int i = 0; i < n; i++)
                    {
                        xNext[i] = z[i] + scale * delta[i];
                        xNext[i] = Clamp(xNext[i], bounds[i, 0], bounds[i, 1]);
                    }

                    return (xNext, t);
                }
                else
                {
                    if (t <= R)
                        return (null, t);

                    t *= alpha;
                }
            }
        }

        // ====== LEVY ======
        private double GenerateNewT0(int p, double eta, double[,] bounds)
        {
            double range = GetMaxRange(bounds);
            return (range / 2.0) * Math.Pow(eta, p);
        }

        private double[] GenerateNewStartPoint(double[] xStar, double[,] bounds, double beta, double lambda, int p)
        {
            int n = xStar.Length;
            const double eps = 1e-7;

            while (true)
            {
                var x0 = new double[n];

                for (int i = 0; i < n; i++)
                {
                    while (true)
                    {
                        double range = bounds[i, 1] - bounds[i, 0];
                        if (range <= 0) range = 1.0;

                        // Python: Q ~ U(eps, b-a), theta = 2*pi*Q, L = Q^(-1/lambda)
                        double Q = NextUniform(eps, range);
                        double theta = 2.0 * Math.PI * Q;
                        double L = Math.Pow(Q, -1.0 / lambda);

                        double levy = L * (i < n / 2 ? Math.Sin(theta) : Math.Cos(theta));
                        double xi = xStar[i] + (beta / p) * levy;

                        if (xi >= bounds[i, 0] && xi <= bounds[i, 1])
                        {
                            x0[i] = xi;
                            break;
                        }
                    }
                }

                // если дошли сюда — все координаты внутри
                return x0;
            }
        }

        // ====== INIT ======
        private double[] GetInitialApproximation(int n, double[,] bounds)
        {
            var x = new double[n];
            for (int i = 0; i < n; i++)
                x[i] = NextUniform(bounds[i, 0], bounds[i, 1]);
            return x;
        }

        private double GetBaseT0(double[,] bounds)
        {
            // Python: t0 = (b-a)/2
            return GetMaxRange(bounds) / 2.0;
        }

        private double GetMaxRange(double[,] bounds)
        {
            int n = bounds.GetLength(0);
            double maxRange = 0.0;
            for (int i = 0; i < n; i++)
            {
                double r = bounds[i, 1] - bounds[i, 0];
                if (r > maxRange) maxRange = r;
            }
            if (maxRange <= 0) maxRange = 1.0;
            return maxRange;
        }

        // ====== RNG / MATH ======
        private double NextUniform(double min, double max)
        {
            return min + _rand.NextDouble() * (max - min);
        }

        private static double Clamp(double x, double min, double max)
        {
            if (x < min) return min;
            if (x > max) return max;
            return x;
        }
    }
}
