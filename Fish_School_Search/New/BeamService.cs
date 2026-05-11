using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fish_School_Search
{
    public class BeamService
    {
        public OptimizationResult RunOptimizer(GenerationBeamsModel model)
        {
            int NP = model.NP;
            int ITER = model.ITER;
            int Smax = model.Smax;
            int Smin = model.Smin;
            int pBeam = model.p;
            double sigmaInitial = model.sigmaInitial;
            double sigmaFinal = model.sigmaFinal;
            int nCount = model.nCount;
            double omega = model.omega;
            int NSTEP = model.NSTEP;
            int tau = model.tau;
            double s = model.s;
            double[,] bounds = model.Bounds;

            // ВАЖНО: Старый алгоритм искал максимум f(x).
            // Beam написан как минимизация => минимизируем (-f(x)).
            Func<double[], double> targetFunction = vec =>
            {
                double f = TargetFunctions.Evaluate(
                    vec[0],
                    vec[1],
                    vec.Length > 2 ? vec[2] : 0,
                    vec.Length > 3 ? vec[3] : 0,
                    model.FunctionNumber
                );
                return -f;
            };

            // init population
            var population = BeamHelper.EvaluatePopulation(
                BeamHelper.GetInitialPopulation(NP, bounds, nCount),
                targetFunction);

            double bestInternal = double.MaxValue;
            double[] bestX = null;

            for (int k = 0; k < ITER; k++)
            {
                population.Sort((x, y) => x.Item1.CompareTo(y.Item1)); // min (-f) => max f

                double f_min = population[0].Item1;
                double f_max = population[population.Count - 1].Item1;
                double[] leader = population[0].Item2;

                // descendants count per individual
                var descendantCounts = new List<int>();
                for (int i = 0; i < population.Count; i++)
                {
                    descendantCounts.Add(BeamHelper.GetDescendantNumber(Smin, Smax, f_max, f_min, population[i].Item1));
                }

                double sigma = BeamHelper.GetMeanSquareDeviation(k, ITER, pBeam, sigmaInitial, sigmaFinal);

                // workPop: (parentValue, parentVector, descendants)
                var workPop = new List<System.Tuple<double, double[], List<double[]>>>();
                for (int i = 0; i < NP; i++)
                {
                    var descendants = BeamHelper.GetDescendants(population[i].Item2, sigma, descendantCounts[i], bounds);
                    workPop.Add(System.Tuple.Create(population[i].Item1, population[i].Item2, descendants));
                }

                double ksi = BeamHelper.GetRandom(-1, 1);
                BeamHelper.SearchTowardsLeader(workPop, leader, ksi, k, ITER, omega, NSTEP, tau, s, targetFunction, bounds);

                var newPopulation = BeamHelper.SelectBestDescendants(workPop, targetFunction);

                // best tracking
                newPopulation.Sort((x, y) => x.Item1.CompareTo(y.Item1));
                if (newPopulation[0].Item1 < bestInternal)
                {
                    bestInternal = newPopulation[0].Item1;
                    bestX = (double[])newPopulation[0].Item2.Clone();
                }

                // survivors + newcomers
                int half = NP / 2;
                var survivors = newPopulation.Take(half).ToList();
                var newcomers = BeamHelper.EvaluatePopulation(
                    BeamHelper.GetInitialPopulation(NP - half, bounds, nCount),
                    targetFunction);

                population = survivors.Concat(newcomers).ToList();
            }

            // final sort
            population.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            double[] finalBest = (bestX != null) ? bestX : population[0].Item2;
            double maxF = TargetFunctions.Evaluate(finalBest[0], finalBest[1], 0, 0, model.FunctionNumber);

            var res = new OptimizationResult();
            res.BestX = (double[])finalBest.Clone();
            res.BestValue = maxF;

            // финальная популяция (координаты)
            var finalPop = new List<double[]>();
            for (int i = 0; i < population.Count; i++)
                finalPop.Add(population[i].Item2);

            res.FinalPopulation = finalPop;
            return res;
        }
    }

}
