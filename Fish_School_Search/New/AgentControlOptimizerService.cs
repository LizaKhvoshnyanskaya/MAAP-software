using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fish_School_Search.New
{
    public class AgentControlOptimizerService
    {
        private readonly Random _rand = new Random();

        public OptimizationResult RunOptimizer(GenerationBeamsModel model)
        {
            int n = model.nCount; // 2
            double[,] bounds = model.Bounds;

            // внутренне минимизируем (-f) => максимизируем f
            Func<double[], double> objMin = v =>
            {
                double f = TargetFunctions.Evaluate(v[0], v[1], 0, 0, model.FunctionNumber);
                return -f;
            };

            int NP = model.Dds_NP;
            int Pmax = model.Dds_Pmax;
            int K = model.Dds_K;
            double h = model.Dds_h;

            int sigma = (NP - 1) / 2;

            // матрицы весов (диагональные)
            double qW = model.Dds_qWeight;
            double sW = model.Dds_sWeight;
            double lW = model.Dds_lambdaWeight;

            double[,] Q = Diag(n, qW);          // n x n
            double[,] S = Diag(2 * n, sW);      // 2n x 2n
            double[,] lmd = Diag(2 * n, lW);    // 2n x 2n

            // init population (позиции)
            var population = GetInitialPopulation(NP, bounds, n);

            var errorHistory = new List<double>(); // если понадобится

            for (int pIter = 0; pIter < Pmax; pIter++)
            {
                if (pIter > 0)
                {
                    population = population.OrderBy(v => objMin(v)).ToList();
                    population = population.Take(NP - sigma).ToList();
                    population.AddRange(GetInitialPopulation(sigma, bounds, n));
                }

                double[,] A = A_matrix(n, h);  // 2n x 2n
                double[,] B = B_matrix(n, h);  // 2n x n

                var (P_arr, L_arr) = GetPL(A, B, Q, S, lmd, K); // L_arr: K матриц

                population = population.OrderBy(v => objMin(v)).ToList();
                double[] leader = population[0];

                errorHistory.Add(objMin(leader)); // внутренний “минимум”

                var newPop = new List<double[]>();
                newPop.Add((double[])leader.Clone());

                foreach (var agent in population.Skip(1))
                {
                    double[] dX = ZeroConcat(Sub(agent, leader), n); // 2n
                    double bestVal = double.MaxValue;
                    double[] bestVec2n = null;

                    for (int j = 0; j <= K; j++)
                    {
                        double[] fullX = Add(ZeroConcat(leader, n), dX); // 2n
                        double[] pos = fullX.Take(n).ToArray();

                        // clamp, чтобы не улетать из области
                        ClampInPlace(pos, bounds);

                        double val = objMin(pos);
                        if (val < bestVal)
                        {
                            bestVal = val;
                            bestVec2n = (double[])fullX.Clone();
                        }

                        if (j < K)
                            dX = GetDifferenceEquation(L_arr[j], dX, A, B);
                    }

                    double[] bestPos = bestVec2n.Take(n).ToArray();
                    ClampInPlace(bestPos, bounds);
                    newPop.Add(bestPos);
                }

                population = newPop;
            }

            population = population.OrderBy(v => objMin(v)).ToList();
            double[] bestX = population[0];
            double bestMaxF = -objMin(bestX);

            return new OptimizationResult
            {
                BestX = (double[])bestX.Clone(),
                BestValue = bestMaxF,
                FinalPopulation = population
            };
        }

        // ---------- Dynamics ----------
        private static double[,] A_matrix(int n, double h)
        {
            // [[I, I*h],[0,I]] size 2n x 2n
            var A = new double[2 * n, 2 * n];
            for (int i = 0; i < n; i++)
            {
                A[i, i] = 1.0;
                A[i, i + n] = h;
                A[i + n, i + n] = 1.0;
            }
            return A;
        }

        private static double[,] B_matrix(int n, double h)
        {
            // [[I*h^2/2],[I*h]] size 2n x n
            var B = new double[2 * n, n];
            double a = h * h / 2.0;
            for (int i = 0; i < n; i++)
            {
                B[i, i] = a;
                B[i + n, i] = h;
            }
            return B;
        }

        // ---------- Riccati (порт getPL) ----------
        private static (List<double[,]>, List<double[,]>) GetPL(double[,] A, double[,] B, double[,] Q, double[,] S, double[,] lmd, int K)
        {
            var P_list = new List<double[,]>();
            var L_list = new List<double[,]>();

            P_list.Add(lmd);

            for (int t = 0; t < K; t++)
            {
                var Pprev = P_list[P_list.Count - 1];

                var Bt = Transpose(B);
                var At = Transpose(A);

                // L = inv(Q + B^T P B) * (B^T P A)
                var BtPB = Mul(Mul(Bt, Pprev), B);             // n x n
                var M = Add(Q, BtPB);                          // n x n
                var invM = Inv(M);                             // n x n
                var BtPA = Mul(Mul(Bt, Pprev), A);             // n x (2n)
                var L = Mul(invM, BtPA);                       // n x (2n)

                // P = S + L^T Q L + (A - B L)^T Pprev (A - B L)
                var Lt = Transpose(L);                         // (2n) x n
                var LtQL = Mul(Mul(Lt, Q), L);                 // (2n) x (2n)

                var BL = Mul(B, L);                            // (2n) x (2n)
                var AminusBL = Sub(A, BL);                     // (2n) x (2n)
                var term = Mul(Mul(Transpose(AminusBL), Pprev), AminusBL);

                var P = Add(Add(S, LtQL), term);

                P_list.Add(P);
                L_list.Add(L);
            }

            // reverse как в Python
            P_list.Reverse();
            L_list.Reverse();
            return (P_list, L_list);
        }

        private static double[] GetDifferenceEquation(double[,] L, double[] dX, double[,] A, double[,] B)
        {
            // u = -L dX
            var u = MulVec(L, dX);
            for (int i = 0; i < u.Length; i++) u[i] = -u[i];

            // A dX + B u
            var Adx = MulVec(A, dX);
            var Bu = MulVec(B, u);
            return Add(Adx, Bu);
        }

        // ---------- Population / helpers ----------
        private List<double[]> GetInitialPopulation(int count, double[,] bounds, int n)
        {
            var list = new List<double[]>();
            for (int i = 0; i < count; i++)
            {
                var v = new double[n];
                for (int j = 0; j < n; j++)
                    v[j] = bounds[j, 0] + _rand.NextDouble() * (bounds[j, 1] - bounds[j, 0]);
                list.Add(v);
            }
            return list;
        }

        private static double[] ZeroConcat(double[] vec, int n)
        {
            var r = new double[2 * n];
            Array.Copy(vec, 0, r, 0, n);
            // остальное нули
            return r;
        }

        private static void ClampInPlace(double[] x, double[,] bounds)
        {
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < bounds[i, 0]) x[i] = bounds[i, 0];
                if (x[i] > bounds[i, 1]) x[i] = bounds[i, 1];
            }
        }

        // ---------- Vector ops ----------
        private static double[] Add(double[] a, double[] b)
        {
            var r = new double[a.Length];
            for (int i = 0; i < a.Length; i++) r[i] = a[i] + b[i];
            return r;
        }

        private static double[] Sub(double[] a, double[] b)
        {
            var r = new double[a.Length];
            for (int i = 0; i < a.Length; i++) r[i] = a[i] - b[i];
            return r;
        }

        // ---------- Matrix ops ----------
        private static double[,] Diag(int n, double val)
        {
            var m = new double[n, n];
            for (int i = 0; i < n; i++) m[i, i] = val;
            return m;
        }

        private static double[,] Add(double[,] A, double[,] B)
        {
            int r = A.GetLength(0), c = A.GetLength(1);
            var M = new double[r, c];
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    M[i, j] = A[i, j] + B[i, j];
            return M;
        }

        private static double[,] Sub(double[,] A, double[,] B)
        {
            int r = A.GetLength(0), c = A.GetLength(1);
            var M = new double[r, c];
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    M[i, j] = A[i, j] - B[i, j];
            return M;
        }

        private static double[,] Transpose(double[,] A)
        {
            int r = A.GetLength(0), c = A.GetLength(1);
            var T = new double[c, r];
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    T[j, i] = A[i, j];
            return T;
        }

        private static double[,] Mul(double[,] A, double[,] B)
        {
            int r = A.GetLength(0), m = A.GetLength(1), c = B.GetLength(1);
            var R = new double[r, c];
            for (int i = 0; i < r; i++)
                for (int k = 0; k < m; k++)
                {
                    double aik = A[i, k];
                    for (int j = 0; j < c; j++)
                        R[i, j] += aik * B[k, j];
                }
            return R;
        }

        private static double[] MulVec(double[,] A, double[] x)
        {
            int r = A.GetLength(0), c = A.GetLength(1);
            var y = new double[r];
            for (int i = 0; i < r; i++)
            {
                double s = 0;
                for (int j = 0; j < c; j++) s += A[i, j] * x[j];
                y[i] = s;
            }
            return y;
        }

        // Gauss-Jordan inverse
        private static double[,] Inv(double[,] A)
        {
            int n = A.GetLength(0);
            var aug = new double[n, 2 * n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++) aug[i, j] = A[i, j];
                aug[i, n + i] = 1.0;
            }

            for (int col = 0; col < n; col++)
            {
                int pivot = col;
                double max = Math.Abs(aug[pivot, col]);
                for (int r = col + 1; r < n; r++)
                {
                    double v = Math.Abs(aug[r, col]);
                    if (v > max) { max = v; pivot = r; }
                }
                if (max < 1e-14) throw new Exception("Matrix is singular");

                if (pivot != col)
                    SwapRows(aug, pivot, col);

                double div = aug[col, col];
                for (int j = 0; j < 2 * n; j++) aug[col, j] /= div;

                for (int r = 0; r < n; r++)
                {
                    if (r == col) continue;
                    double factor = aug[r, col];
                    if (Math.Abs(factor) < 1e-18) continue;
                    for (int j = 0; j < 2 * n; j++)
                        aug[r, j] -= factor * aug[col, j];
                }
            }

            var inv = new double[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    inv[i, j] = aug[i, n + j];
            return inv;
        }

        private static void SwapRows(double[,] M, int r1, int r2)
        {
            int c = M.GetLength(1);
            for (int j = 0; j < c; j++)
            {
                double tmp = M[r1, j];
                M[r1, j] = M[r2, j];
                M[r2, j] = tmp;
            }
        }
    }
}
