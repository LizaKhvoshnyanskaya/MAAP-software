using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fish_School_Search
{
    public class Algoritm
    {
        public int Ip;
        public int K;
        public int N;
        public double[,] oblast;

        public int z;
        public double thr;
        public double WScale;

        public double Mfend = 0;

        public List<double> AverF = new List<double>();
        public List<double> BestF = new List<double>();

        Random R = new Random();

        public int Nm;
        int m;

        public int Nf = 0;

        public Vector optimalStart;
        public Vector optimalD;

        public void Init(int ip, int k, double[,] obl, int n, int z_, double thr_, double wScale, double stepInd0_ = 0.1, double stepInd1_ = 0.01)
        {
            K = k;
            N = n;
            oblast = obl;
            //----
            Ip = ip;
            z = z_;
            thr = thr_;
            thr = 0.8 * wScale; // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            WScale = wScale;
            //
            stepIndInitial = stepInd0_;
            stepIndFinal = stepInd1_;
            stepVolInitial = 0.001;
            stepVolFinal = 0.0001;
            //
            optimalStart = new Vector(n);
            optimalD = new Vector(n);
            for (int j = 0; j < n; j++) optimalStart.v[j] = -1;
        }





        private double Func(double[] x)
        {
            double func = 0;
            if (z == 0)
                func = x[0] * Math.Sin(Math.Sqrt(Math.Abs(x[0]))) + x[1] * Math.Sin(Math.Sqrt(Math.Abs(x[1])));
            else if (z == 1)
                func = x[0] * Math.Sin(4 * Math.PI * x[0]) - x[1] * Math.Sin(4 * Math.PI * x[1] + Math.PI) + 1;
            else if (z == 2)
            {
                double[] c6 = Cpow(x[0], x[1], 6);
                func = (float)(1 / (1 + Math.Sqrt((c6[0] - 1) * (c6[0] - 1) + c6[1] * c6[1])));
            }
            else if (z == 3)
            {
                func = (float)(0.5 - (Math.Pow(Math.Sin(Math.Sqrt(x[0] * x[0] + x[1] * x[1])), 2) - 0.5) / (1 + 0.001 * (x[0] * x[0] + x[1] * x[1])));
            }
            else if (z == 4)
            {
                func = (float)((-x[0] * x[0] + 10 * Math.Cos(2 * Math.PI * x[0])) + (-x[1] * x[1] + 10 * Math.Cos(Math.PI * x[1])));
            }
            else if (z == 5)
            {
                func = (float)(-Math.E + 20 * Math.Exp(-0.2 * Math.Sqrt((x[0] * x[0] + x[1] * x[1]) / 2)) + Math.Exp((Math.Cos(2 * Math.PI * x[0]) + Math.Cos(2 * Math.PI * x[1])) / 2));
            }
            else if (z == 6)
            {
                func = (float)(Math.Pow(Math.Cos(2 * x[0] * x[0]) - 1.1, 2) + Math.Pow(Math.Sin(0.5 * x[0]) - 1.2, 2) - Math.Pow(Math.Cos(2 * x[1] * x[1]) - 1.1, 2) + Math.Pow(Math.Sin(0.5 * x[1]) - 1.2, 2));
            }
            else if (z == 7)
            {
                func = (float)(-Math.Sqrt(Math.Abs(Math.Sin(Math.Sin(Math.Sqrt(Math.Abs(Math.Sin(x[0] - 1))) + Math.Sqrt(Math.Abs(Math.Sin(x[1] + 2))))))) + 1);
            }
            else if (z == 8)
            {
                func = (float)(-(1 - x[0]) * (1 - x[0]) - 100 * (x[1] - x[0] * x[0]) * (x[1] - x[0] * x[0]));
            }
            else if (z == 9)
            {
                func = (float)(1-x[0] * x[0] - x[1] * x[1]);
            }
      
            
       
            Nf++;
            return func;
        }

        private static double[] Cpow(double x, double y, int p)
        {
            double[] Cp = new double[2];
            Cp[0] = x; Cp[1] = y;
            double x0 = 0;
            double y0 = 0;
            for (int i = 1; i < p; i++)
            {
                x0 = Cp[0] * x - Cp[1] * y;
                y0 = Cp[1] * x + Cp[0] * y;
                Cp[0] = x0; Cp[1] = y0;
            }
            return Cp;
        }
        //--------------------------------------------------------------------------------------------------

        public struct Vector
        {
            public double[] v;

            public Vector(int n)
            {
                v = new double[n];
            }

            public double distance(Vector y)
            {
                var S = 0.0;
                for (int i = 0; i < v.Length; i++)
                {
                    var d = y.v[i] - v[i];
                    S += d * d;
                }
                return Math.Sqrt(S);
            }

            public void normalize()
            {
                var s = 0.0;
                for (int i = 0; i < v.Length; i++)
                {
                    s += v[i] * v[i];
                }
                if (s == 0) return;
                s = Math.Sqrt(s);
                for (int i = 0; i < v.Length; i++)
                {
                    v[i] /= s;
                }
            }

            public double this[int i]
            {
                get
                {
                    return v[i];
                }
            }
        }

        public class Fish
        {
            public Algoritm A;

            private Vector _x;
            public double[] dx;
            public double W;
            public double f;
            public double f2;
            public double normf;
            //public List<Fish> Kloni;

            public double df { get { return f2 - f; } }

            public Fish(Algoritm alg)
            {
                A = alg;
                dx = new double[A.N];
                x2 = new double[A.N];
                W = 0.5 * A.WScale;
            }

            public Vector x
            {
                get { return _x; }
                set
                {
                    _x = value;
                    for (int i = 0; i < A.N; i++) x2[i] = _x.v[i];
                    f = A.Func(_x.v);
                    f2 = f;
                }
            }

            public double[] X { get { return x.v; } }

            public double[] x2;

            //public double[] X2 
            //{ 
            //    get 
            //    {
            //        var x2 = new double[A.N];
            //        for (int i = 0; i < A.N; i++) x2[i] = x.v[i] + dx[i];
            //        return x2; 
            //    } 
            //}

            public bool X2inOblast
            {
                get
                {
                    //var x2 = X2;
                    for (int i = 0; i < A.N; i++) 
                        if (x2[i] < A.oblast[i, 0] || x2[i] > A.oblast[i, 1]) return false;
                    return true;
                }
            }

            public void CalcNextValueOfF()
            {
                f2 = A.Func(x2);
            }

            public void DropNexValueOfF()
            {
                for (int i = 0; i < A.N; i++)
                {
                    dx[i] = 0;
                    x2[i] = x.v[i];
                }
                f2 = f;
            }

            public void Move()
            {
                for (int i = 0; i < A.N; i++) x.v[i] = x.v[i] + dx[i];
                f = f2;
            }

            public int pairIndex;
        }

        public List<Fish> Population = new List<Fish>();

        private double maxOblSize
        {
            get
            {
                var m = oblast[0, 1] - oblast[0, 0];
                for (int i = 0; i < N; i++)
                {
                    if (oblast[i, 1] - oblast[i, 0] > m) m = oblast[i, 1] - oblast[i, 0];
                }
                return m;
            }
        }

        private double stepIndInitial;
        private double stepIndFinal;
        private double stepIndDelta { get { return (stepIndFinal - stepIndInitial) / K; } }
        private double stepInd { get { return stepIndInitial + stepIndDelta * k; } }

        private double stepVolInitial;
        private double stepVolFinal;
        private double stepVolDelta { get { return (stepVolFinal - stepVolInitial) / K; } }
        private double stepVol { get { return stepVolInitial + stepVolDelta * k; } }

        // Динамические параметры

        private int k;          // номер итерации
        private double dW;
        public double Mf;

        public int kpop
        {
            get
            {
                return k;
            }
            set
            {
                k = value;
            }
        }

      
        public void Create_Pop() 
        {
            for (int i = 0; i < Ip; i++)
            {
                Fish member = new Fish(this);
                //member.Kloni = new List<Fish>();

                Vector x0 = new Vector(N);
                for (int j = 0; j < N; j++)
                {
                    double u0 = oblast[j, 0];
                    double u1 = oblast[j, 1];
                    if (optimalStart.v[j] > 0)
                    {
                        u0 = optimalStart.v[j] - optimalD.v[j];
                        u1 = optimalStart.v[j] + optimalD.v[j];
                    }
                    x0.v[j] = R.NextDouble() * (u1 - u0) + u0;
                }
                member.x = x0;

                Population.Add(member);
                // Mf += member.f;     // ???
            }
            Analyze();
        }



        public void Work()//работа алгоритма
        {
            for (k = 0; k < K; k++)
            {
                //LogIteration();
                if (k > 0) Food();//питание 
                Swiming();//плавание
                Reproduction();//размножение
                Delete();//удаление
            }
            Analyze(true);
        }

        public void Analyze(bool final = false)
        {
            RecalcMaxf();
            //RecalcMaxDf();
            if (!final) return;

            Mfend = Math.Round(Mf, 6);
        }

        //операция плавания
        
        public void Swiming() 
        {
            var sumDf = 0.0;
            var sumDx = new double[N]; // все нули

            //  а)
            foreach (Fish member in Population)
            {
                // z)
                var __x2 = member.x2;
                for (int i = 0; i < N; i++)
                    member.x.v[i] = __x2[i];

                // ----------------------------------------------------------------------------------
                for (int i = 0; i < N; i++)
                {
                    double delta = -0.5 + R.NextDouble();
                    member.dx[i] = delta * stepInd;
                    if (i >= 2) member.dx[i] *= 0.3;
                    member.x2[i] = member.x.v[i] + member.dx[i];
                }
                if (!member.X2inOblast)
                {
                    for (int i = 0; i < N; i++)
                    {
                        member.dx[i] = 0;
                        member.x2[i] = member.x[i];
                    }
                }
                //
                member.CalcNextValueOfF();
                if (member.df < 0)
                    member.DropNexValueOfF();
                //
                if (member.df < 0)
                    Console.WriteLine("!");
                //
                if (member.df > 0)
                {
                    sumDf += member.df;
                    for (int i = 0; i < N; i++)
                        sumDx[i] += member.dx[i] * member.df;
                }
            }
            // b)
            if (sumDf > 0)
            {
                var dy = new double[N];
                for (int i = 0; i < N; i++)
                    dy[i] = sumDx[i] / sumDf;
                //
                foreach (Fish member in Population)
                {
                    for (int i = 0; i < N; i++)
                    {
                        ////// ОТКЛЮЧАЕМ! // 
                        member.dx[i] += dy[i];
                        member.x2[i] = member.x.v[i] + member.dx[i];
                    }
                    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    if (!member.X2inOblast)
                    {
                        for (int i = 0; i < N; i++)
                        {
                            member.dx[i] = 0;
                            member.x2[i] = member.x[i];
                        }
                    }

                    member.CalcNextValueOfF();
                    //if (member.df < 0)
                    //    member.DropNexValueOfF();
                    //if (member.df < 0)
                    //{
                    //    Console.WriteLine("!");
                    //}
                }
            }
            // c)
            var Bari = new double[N]; // нули?
            var sumW = 0.0;
            foreach (Fish member in Population)
            {
                for (int i = 0; i < N; i++)
                {
                    Bari[i] += member.x2[i] * member.W;
                }
                sumW += member.W;
            }
            if (sumW > 0)
                for (int i = 0; i < N; i++) Bari[i] /= sumW;
            var s = dW > 0 ? -1 : 1;
            // завершаем: 
            // if (false)
            foreach (Fish member in Population)
            {
                var u = new Vector(N);
                for (int i = 0; i < N; i++)
                {
                    u.v[i] = member.x2[i] - Bari[i];
                }
                u.normalize();
                var _r = R.NextDouble();
                for (int i = 0; i < N; i++)
                {
                    // ОТКЛЮЧАЕМ! // 
                    member.dx[i] += u.v[i] * stepVol * _r;
                    member.x2[i] = member.x.v[i] + member.dx[i];
                }


                //!!!!!!!!!!!!!!!
                if (!member.X2inOblast)
                {
                    for (int i = 0; i < N; i++)
                    {
                        member.dx[i] = 0;
                        member.x2[i] = member.x[i];
                    }
                }



                member.CalcNextValueOfF();
                if (member.df < 0)
                    member.DropNexValueOfF();
                if (member.df < 0)
                    Console.WriteLine("!");
                member.Move();
            }
            RecalcMaxf();
        }
        //операция питания
        public void Food() 
        {
            if (maxDf <= 0)
                return;

            dW = 0.0;
            foreach (Fish member in Population)
            {
                //if (member.df < 0)
                //    Console.WriteLine("!");

                int stop = 0;
                if (maxDf > 0.5)
                    stop = 1;

                double a0 = 0.1 * 0.5 * (maxDf + 0.33 * 0.5 * WScale / K);
                a0 = 2 * (WScale - Population[0].W) / WScale;
                if (a0 <= 0)
                    a0 = 1;
                //
                var newW = member.df > 0? member.W + member.df / maxDf : member.W;
                if (newW > WScale) newW = WScale;
                dW += newW - member.W;
                member.W = newW;
            }
        }
        //операция разножения
        public void Reproduction() 
        {
            var L = new List<Fish>();
            foreach (Fish member in Population)
            {
                if (member.W >= thr)
                {
                    member.pairIndex = -1;
                    L.Add(member);
                }
            }
            var n = L.Count;
            while (n > 1)
            {
                var k1 = R.Next(L.Count);
                var fish = L[k1];
                if (fish.pairIndex >= 0) continue;
                //
                double m = 0;
                int k2 = -1;
                for (int k = 0; k < L.Count; k++)
                {
                    var fish2_ = L[k];
                    if (fish2_.pairIndex >= 0) continue;
                    var m_ = fish2_.W / fish2_.x.distance(fish.x);
                    if (m_ > m)
                    {
                        m = m_;
                        k2 = k;
                    }
                }
                var fish2 = L[k2];
                //
                fish.pairIndex = k2;
                fish2.pairIndex = k1;
                //
                // размножение
                var child = new Fish(this);
                var y = new Vector(N);
                for (int i = 0; i < N; i++) y.v[i] = 0.5 * (fish.x.v[i] + fish2.x.v[i]);
                child.x = y;
                child.W = 0.5 * (fish.W + fish2.W);
                Population.Add(child);
                //
                n -= 2;
            }
        }
        //удаление рыб с наим весом
        public void Delete() 
        {
            Population.Sort(delegate(Fish p1, Fish p2) { return p2.W.CompareTo(p1.W); });
            //
            int r = -1;
            for (int j = 0; j < Population.Count; j++)
            {
                if (Population[j].W == WScale) r = j;
            }
            if (r >= 1)
            {
                var fish0 = Population[0];
                var j1 = 1 + (R.Next() % r);
                Population[0] = Population[j1];
                Population[j1] = fish0;
            }
            //
            int stop = 0;
            if (Population[0].W >= this.WScale)
                stop = 1;
            //
            if (Population.Count > Ip)
                Population.RemoveRange(Ip, Population.Count - Ip);

            Analyze();
        }

        double maxDf = 0;

        public void RecalcMaxDf()
        {
            maxDf = 0;
            var S = 0.0;
            for (int j = 0; j < Population.Count; j++)
            {
                Fish member = Population[j];
                if (member.df > maxDf) maxDf = member.df;
                S += member.f;
            }
            //BestF.Add(maxDf);
            Mf = S / Population.Count;
            //AverF.Add(Mf);
        }

        public void RecalcMaxf()
        {
            maxDf = 0;
            var S = 0.0;
            for (int j = 0; j < Population.Count; j++)
            {
                Fish member = Population[j];
                if (member.f > maxDf) maxDf = member.f;
                S += member.f;
            }
            BestF.Add(maxDf);
            Mf = S / Population.Count;
            AverF.Add(Mf);
        }


        private void LogIteration()
        {
            foreach (var fish in Population)
            {
                Console.Write(fish.W + "  ");
                Console.WriteLine();
            }
        }

    }
}
