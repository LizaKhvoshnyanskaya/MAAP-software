using Fish_School_Search.New;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Fish_School_Search
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitMethodCombo();
            InitForecastGrid();
            InitDdsGrid();
            UpdateMethodUI();
            dataGridView1.RowCount = 2;
            dataGridView1.Rows[0].Cells[0].Value = "x";
            dataGridView1.Rows[1].Cells[0].Value = "y";
            InitDataGridView1();

            if ((File.Exists("protocol.dt")))
            {
                File.Delete("protocol.dt");
            }
            FileStream vipoln_test = new FileStream("protocol.dt", FileMode.OpenOrCreate);
            vipoln_test.Close();
            FileStream fs = new FileStream("protocol.dt", FileMode.Append, FileAccess.Write);

            StreamWriter r = new StreamWriter(fs);
            r.Write("*********************************************************************************************************" + "\r\n"
                   + "**                                                                                                     **" + "\r\n"
                   + "**                            Протокол работы метода, имтмирующего поведение стаи рыб                  **" + "\r\n"
                   + "**                                           " + DateTime.Now.ToString() + "                                       **" + "\r\n"
                   + "**                                                                                                     **" + "\r\n"
                   + "*********************************************************************************************************" + "\r\n");
            r.Close();
            fs.Close();
        }
        private enum OptimizationMethod
        {
            Beam = 0,
            Forecast = 1,
            Dds = 2
        }
        private void InitMethodCombo()
        {
            comboBoxMethod.Items.Clear();
            comboBoxMethod.Items.Add("Beam (генерация пучков)");
            comboBoxMethod.Items.Add("Прогноз + Леви (многошаговый)");
            comboBoxMethod.Items.Add("ДДС агенты (управление динамической системой)");
            comboBoxMethod.SelectedIndex = 0;
            comboBoxMethod.SelectedIndexChanged += (s, e) => UpdateMethodUI();
        }
        private void UpdateMethodUI()
        {
            bool isBeam = comboBoxMethod.SelectedIndex == (int)OptimizationMethod.Beam;
            bool isForecast = comboBoxMethod.SelectedIndex == (int)OptimizationMethod.Forecast;
            bool isDds = comboBoxMethod.SelectedIndex == (int)OptimizationMethod.Dds;

            dataGridView2.Visible = isBeam;             // Beam params
            dataGridViewForecast.Visible = isForecast;  // Forecast params
            dataGridViewDds.Visible = isDds;            // DDS params
        }
        private void InitDataGridView1()
        {
            // Beam-параметры (11 строк)
            dataGridView2.RowCount = 11;

            dataGridView2.Rows[0].Cells[0].Value = "NP (размер популяции)";
            dataGridView2.Rows[1].Cells[0].Value = "ITER (кол-во итераций)";
            dataGridView2.Rows[2].Cells[0].Value = "Smax (макс потомков)";
            dataGridView2.Rows[3].Cells[0].Value = "Smin (мин потомков)";
            dataGridView2.Rows[4].Cells[0].Value = "p (степень для sigma)";
            dataGridView2.Rows[5].Cells[0].Value = "sigmaInitial";
            dataGridView2.Rows[6].Cells[0].Value = "sigmaFinal";
            dataGridView2.Rows[7].Cells[0].Value = "omega";
            dataGridView2.Rows[8].Cells[0].Value = "NSTEP";
            dataGridView2.Rows[9].Cells[0].Value = "tau";
            dataGridView2.Rows[10].Cells[0].Value = "s";

            // Результаты
            dataGridView3.RowCount = 5;
            dataGridView3.Rows[0].Cells[0].Value = "Размер конечной популяции:";
            dataGridView3.Rows[1].Cells[0].Value = "(x*; y*)";
            dataGridView3.Rows[2].Cells[0].Value = "f* (максимум)";
            dataGridView3.Rows[3].Cells[0].Value = "Точное решение:";
            dataGridView3.Rows[4].Cells[0].Value = "Отклонение (f* - exact):";
        }
        private void InitForecastGrid()
        {
            // 8 строк: N,P,K,R,alpha,eta,beta,lambda
            dataGridViewForecast.RowCount = 8;

            dataGridViewForecast.Rows[0].Cells[0].Value = "N (попыток)";
            dataGridViewForecast.Rows[1].Cells[0].Value = "P (проходов)";
            dataGridViewForecast.Rows[2].Cells[0].Value = "K (итераций/проход)";
            dataGridViewForecast.Rows[3].Cells[0].Value = "R (мин шаг)";
            dataGridViewForecast.Rows[4].Cells[0].Value = "alpha";
            dataGridViewForecast.Rows[5].Cells[0].Value = "eta";
            dataGridViewForecast.Rows[6].Cells[0].Value = "beta";
            dataGridViewForecast.Rows[7].Cells[0].Value = "lambda";

            // дефолты (как в Python)
            dataGridViewForecast.Rows[0].Cells[1].Value = "5";
            dataGridViewForecast.Rows[1].Cells[1].Value = "10";
            dataGridViewForecast.Rows[2].Cells[1].Value = "20";
            dataGridViewForecast.Rows[3].Cells[1].Value = "1e-8";
            dataGridViewForecast.Rows[4].Cells[1].Value = "0,85";
            dataGridViewForecast.Rows[5].Cells[1].Value = "0,89";
            dataGridViewForecast.Rows[6].Cells[1].Value = "0,3";
            dataGridViewForecast.Rows[7].Cells[1].Value = "1,5";
        }
        private void InitDdsGrid()
        {
            dataGridViewDds.RowCount = 7;

            dataGridViewDds.Rows[0].Cells[0].Value = "NP";
            dataGridViewDds.Rows[1].Cells[0].Value = "P_max";
            dataGridViewDds.Rows[2].Cells[0].Value = "K";
            dataGridViewDds.Rows[3].Cells[0].Value = "h";
            dataGridViewDds.Rows[4].Cells[0].Value = "qWeight";
            dataGridViewDds.Rows[5].Cells[0].Value = "sWeight";
            dataGridViewDds.Rows[6].Cells[0].Value = "lambdaWeight";

            dataGridViewDds.Rows[0].Cells[1].Value = "40";
            dataGridViewDds.Rows[1].Cells[1].Value = "45";
            dataGridViewDds.Rows[2].Cells[1].Value = "30";
            dataGridViewDds.Rows[3].Cells[1].Value = "0,6";
            dataGridViewDds.Rows[4].Cells[1].Value = "10";
            dataGridViewDds.Rows[5].Cells[1].Value = "100";
            dataGridViewDds.Rows[6].Cells[1].Value = "100";
        }
 
        private Algoritm alg;
        double exact = 0;

        bool first = true;
        int numb = 0;
        bool[] flines = new bool[8];
        int n = 2;

        private BeamService beam = new BeamService();
        private readonly ForecastOptimizerService forecastService = new ForecastOptimizerService();
        private readonly AgentControlOptimizerService ddsService = new AgentControlOptimizerService();

        // то, что будем рисовать на pictureBox1
        private List<double[]> lastPopulation = new List<double[]>();
        private double[] bestPoint = null;
        private double bestValue = double.NaN;

        private bool TryReadForecastParams(out int N, out int P, out int K, out double R,
                                  out double alpha, out double eta, out double beta, out double lambda)
        {
            N = P = K = 0; R = alpha = eta = beta = lambda = 0;

            try
            {
                N = Convert.ToInt32(dataGridViewForecast.Rows[0].Cells[1].Value);
                P = Convert.ToInt32(dataGridViewForecast.Rows[1].Cells[1].Value);
                K = Convert.ToInt32(dataGridViewForecast.Rows[2].Cells[1].Value);
                R = Convert.ToDouble(dataGridViewForecast.Rows[3].Cells[1].Value);

                alpha = Convert.ToDouble(dataGridViewForecast.Rows[4].Cells[1].Value);
                eta = Convert.ToDouble(dataGridViewForecast.Rows[5].Cells[1].Value);
                beta = Convert.ToDouble(dataGridViewForecast.Rows[6].Cells[1].Value);
                lambda = Convert.ToDouble(dataGridViewForecast.Rows[7].Cells[1].Value);
            }
            catch
            {
                MessageBox.Show("Неверно введены параметры (Прогноз + Леви).");
                return false;
            }

            if (N <= 0 || P <= 0 || K <= 0 || R <= 0 || alpha <= 0 || alpha >= 1 || eta <= 0 || eta >= 1 || beta <= 0 || lambda <= 0)
            {
                MessageBox.Show("Параметры (Прогноз + Леви) вне допустимого диапазона.");
                return false;
            }

            return true;
        }
        private bool TryReadDdsParams(out int NP, out int Pmax, out int K, out double h,
                             out double qW, out double sW, out double lW)
        {
            NP = Pmax = K = 0; h = qW = sW = lW = 0;

            try
            {
                NP = Convert.ToInt32(dataGridViewDds.Rows[0].Cells[1].Value);
                Pmax = Convert.ToInt32(dataGridViewDds.Rows[1].Cells[1].Value);
                K = Convert.ToInt32(dataGridViewDds.Rows[2].Cells[1].Value);
                h = Convert.ToDouble(dataGridViewDds.Rows[3].Cells[1].Value);

                qW = Convert.ToDouble(dataGridViewDds.Rows[4].Cells[1].Value);
                sW = Convert.ToDouble(dataGridViewDds.Rows[5].Cells[1].Value);
                lW = Convert.ToDouble(dataGridViewDds.Rows[6].Cells[1].Value);
            }
            catch { MessageBox.Show("Неверные параметры (ДДС)"); return false; }

            if (NP < 3 || Pmax <= 0 || K <= 0 || h <= 0 || qW <= 0 || sW <= 0 || lW <= 0)
            { MessageBox.Show("Параметры (ДДС) вне диапазона"); return false; }

            return true;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TryBuildModelFromUi(out GenerationBeamsModel model, out int functionNumber))
                    return;

                OptimizationResult res = RunSelectedOptimizer(model);

                bestPoint = res.BestX;
                bestValue = res.BestValue;
                lastPopulation = res.FinalPopulation;

                dataGridView3.Rows[0].Cells[1].Value = (lastPopulation != null ? lastPopulation.Count.ToString() : "0");
                dataGridView3.Rows[1].Cells[1].Value = "(" + vectorText(bestPoint) + ")";
                dataGridView3.Rows[2].Cells[1].Value = Math.Round(bestValue, 6).ToString();
                dataGridView3.Rows[3].Cells[1].Value = exact.ToString();
                dataGridView3.Rows[4].Cells[1].Value = Math.Round(bestValue - exact, 6).ToString();

                flag2 = true;
                pictureBox1.Refresh();
                button8.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при выполнении оптимизации: " + ex.Message);
            }
        }


        private string vectorText(double[] x)
        {
            if (x == null || x.Length == 0)
                return "";

            var s = "";
            var k = x.Length <= 2 ? 3 : 2;
            if (x.Length >= 7) k = 1;

            for (int i = 0; i < x.Length; i++)
            {
                s += Math.Round(x[i], k).ToString();
                if (i < x.Length - 1) s += "; ";
            }
            return s;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            n = 2;
            if (comboBox1.SelectedIndex == 0) // швефель
            {
                dataGridView2.Rows[0].Cells[1].Value = "30";     // NP
                dataGridView2.Rows[1].Cells[1].Value = "100";  // ITER (как раньше K)
                dataGridView2.Rows[2].Cells[1].Value = "10";     // Smax
                dataGridView2.Rows[3].Cells[1].Value = "2";      // Smin
                dataGridView2.Rows[4].Cells[1].Value = "2";      // p
                dataGridView2.Rows[5].Cells[1].Value = "20";     // sigmaInitial (для больших областей можно больше)
                dataGridView2.Rows[6].Cells[1].Value = "0,5";    // sigmaFinal
                dataGridView2.Rows[7].Cells[1].Value = "0,7";    // omega
                dataGridView2.Rows[8].Cells[1].Value = "50";     // NSTEP
                dataGridView2.Rows[9].Cells[1].Value = "20";     // tau
                dataGridView2.Rows[10].Cells[1].Value = "0,2";

                dataGridView1.Rows[0].Cells[1].Value = "-500";
                dataGridView1.Rows[0].Cells[2].Value = "500";
                dataGridView1.Rows[1].Cells[1].Value = "-500";
                dataGridView1.Rows[1].Cells[2].Value = "500";

                exact = 837.9658;

                A[0] = -200;
                A[1] = -1;
                A[2] = 300;
                A[3] = 600;
                A[4] = 800;
                flag = true;
                pictureBox2.Image = Properties.Resources.швефель;

            }
            else if (comboBox1.SelectedIndex == 1) /*мульти*/
            {
                dataGridView2.Rows[0].Cells[1].Value = "30";     // NP
                dataGridView2.Rows[1].Cells[1].Value = "100";  // ITER (как раньше K)
                dataGridView2.Rows[2].Cells[1].Value = "10";     // Smax
                dataGridView2.Rows[3].Cells[1].Value = "2";      // Smin
                dataGridView2.Rows[4].Cells[1].Value = "2";      // p
                dataGridView2.Rows[5].Cells[1].Value = "20";     // sigmaInitial (для больших областей можно больше)
                dataGridView2.Rows[6].Cells[1].Value = "0,5";    // sigmaFinal
                dataGridView2.Rows[7].Cells[1].Value = "0,7";    // omega
                dataGridView2.Rows[8].Cells[1].Value = "50";     // NSTEP
                dataGridView2.Rows[9].Cells[1].Value = "20";     // tau
                dataGridView2.Rows[10].Cells[1].Value = "0,2";

                dataGridView1.Rows[0].Cells[1].Value = "-2";
                dataGridView1.Rows[0].Cells[2].Value = "2";
                dataGridView1.Rows[1].Cells[1].Value = "-2";
                dataGridView1.Rows[1].Cells[2].Value = "2";

                exact = 4.253888;

                A[0] = 0;
                A[1] = 1;
                A[2] = 2;
                A[3] = 3;
                A[4] = 4;
                flag = true;
                pictureBox2.Image = Properties.Resources.мульти;

            }
            else if (comboBox1.SelectedIndex == 2)
            {
                dataGridView2.Rows[0].Cells[1].Value = "30";     // NP
                dataGridView2.Rows[1].Cells[1].Value = "100";  // ITER (как раньше K)
                dataGridView2.Rows[2].Cells[1].Value = "10";     // Smax
                dataGridView2.Rows[3].Cells[1].Value = "2";      // Smin
                dataGridView2.Rows[4].Cells[1].Value = "2";      // p
                dataGridView2.Rows[5].Cells[1].Value = "20";     // sigmaInitial (для больших областей можно больше)
                dataGridView2.Rows[6].Cells[1].Value = "0,5";    // sigmaFinal
                dataGridView2.Rows[7].Cells[1].Value = "0,7";    // omega
                dataGridView2.Rows[8].Cells[1].Value = "50";     // NSTEP
                dataGridView2.Rows[9].Cells[1].Value = "20";     // tau
                dataGridView2.Rows[10].Cells[1].Value = "0,2";

                dataGridView1.Rows[0].Cells[1].Value = "-2";
                dataGridView1.Rows[0].Cells[2].Value = "2";
                dataGridView1.Rows[1].Cells[1].Value = "-2";
                dataGridView1.Rows[1].Cells[2].Value = "2";


                exact = 1;

                A[0] = 0.2F;
                A[1] = 0.45F;
                A[2] = 0.499999F;//0.5000001F;
                A[3] = 0.6F;
                A[4] = 0.9F;
                flag = true;
                pictureBox2.Image = Properties.Resources.рут;

            }
            else if (comboBox1.SelectedIndex == 3)
            {
                dataGridView2.Rows[0].Cells[1].Value = "30";     // NP
                dataGridView2.Rows[1].Cells[1].Value = "100";  // ITER (как раньше K)
                dataGridView2.Rows[2].Cells[1].Value = "10";     // Smax
                dataGridView2.Rows[3].Cells[1].Value = "2";      // Smin
                dataGridView2.Rows[4].Cells[1].Value = "2";      // p
                dataGridView2.Rows[5].Cells[1].Value = "20";     // sigmaInitial (для больших областей можно больше)
                dataGridView2.Rows[6].Cells[1].Value = "0,5";    // sigmaFinal
                dataGridView2.Rows[7].Cells[1].Value = "0,7";    // omega
                dataGridView2.Rows[8].Cells[1].Value = "50";     // NSTEP
                dataGridView2.Rows[9].Cells[1].Value = "20";     // tau
                dataGridView2.Rows[10].Cells[1].Value = "0,2";

                dataGridView1.Rows[0].Cells[1].Value = "-10";
                dataGridView1.Rows[0].Cells[2].Value = "10";
                dataGridView1.Rows[1].Cells[1].Value = "-10";
                dataGridView1.Rows[1].Cells[2].Value = "10";

                exact = 1;

                A[0] = 0.2F;
                A[1] = 0.4F;
                A[2] = 0.6F;//0.5000001F;
                A[3] = 0.8F;
                A[4] = 0.99F;
                flag = true;
                pictureBox2.Image = Properties.Resources.шафер;

            }
            else if (comboBox1.SelectedIndex == 4)
            {
                dataGridView2.Rows[0].Cells[1].Value = "30";     // NP
                dataGridView2.Rows[1].Cells[1].Value = "100";  // ITER (как раньше K)
                dataGridView2.Rows[2].Cells[1].Value = "10";     // Smax
                dataGridView2.Rows[3].Cells[1].Value = "2";      // Smin
                dataGridView2.Rows[4].Cells[1].Value = "2";      // p
                dataGridView2.Rows[5].Cells[1].Value = "20";     // sigmaInitial (для больших областей можно больше)
                dataGridView2.Rows[6].Cells[1].Value = "0,5";    // sigmaFinal
                dataGridView2.Rows[7].Cells[1].Value = "0,7";    // omega
                dataGridView2.Rows[8].Cells[1].Value = "50";     // NSTEP
                dataGridView2.Rows[9].Cells[1].Value = "20";     // tau
                dataGridView2.Rows[10].Cells[1].Value = "0,2";

                dataGridView1.Rows[0].Cells[1].Value = "-5";
                dataGridView1.Rows[0].Cells[2].Value = "5";
                dataGridView1.Rows[1].Cells[1].Value = "-5";
                dataGridView1.Rows[1].Cells[2].Value = "5";

                exact = 20;

                A[0] = -20F;
                A[1] = -10F;
                A[2] = 0F;//0.5000001F;
                A[3] = 10F;
                A[4] = 19F;
                flag = true;
                pictureBox2.Image = Properties.Resources.Растринг1;

            }
            else if (comboBox1.SelectedIndex == 5)
            {
                dataGridView2.Rows[0].Cells[1].Value = "30";     // NP
                dataGridView2.Rows[1].Cells[1].Value = "100";  // ITER (как раньше K)
                dataGridView2.Rows[2].Cells[1].Value = "10";     // Smax
                dataGridView2.Rows[3].Cells[1].Value = "2";      // Smin
                dataGridView2.Rows[4].Cells[1].Value = "2";      // p
                dataGridView2.Rows[5].Cells[1].Value = "20";     // sigmaInitial (для больших областей можно больше)
                dataGridView2.Rows[6].Cells[1].Value = "0,5";    // sigmaFinal
                dataGridView2.Rows[7].Cells[1].Value = "0,7";    // omega
                dataGridView2.Rows[8].Cells[1].Value = "50";     // NSTEP
                dataGridView2.Rows[9].Cells[1].Value = "20";     // tau
                dataGridView2.Rows[10].Cells[1].Value = "0,2";

                dataGridView1.Rows[0].Cells[1].Value = "-10";
                dataGridView1.Rows[0].Cells[2].Value = "10";
                dataGridView1.Rows[1].Cells[1].Value = "-10";
                dataGridView1.Rows[1].Cells[2].Value = "10";


                exact = 20;

                A[0] = 4F;
                A[1] = 7F;
                A[2] = 10F;//0.5000001F;
                A[3] = 14F;
                A[4] = 19F;
                flag = true;
                pictureBox2.Image = Properties.Resources.Аклей31;
            }
            else if (comboBox1.SelectedIndex == 6)
            {
                dataGridView2.Rows[0].Cells[1].Value = "30";     // NP
                dataGridView2.Rows[1].Cells[1].Value = "100";  // ITER (как раньше K)
                dataGridView2.Rows[2].Cells[1].Value = "10";     // Smax
                dataGridView2.Rows[3].Cells[1].Value = "2";      // Smin
                dataGridView2.Rows[4].Cells[1].Value = "2";      // p
                dataGridView2.Rows[5].Cells[1].Value = "20";     // sigmaInitial (для больших областей можно больше)
                dataGridView2.Rows[6].Cells[1].Value = "0,5";    // sigmaFinal
                dataGridView2.Rows[7].Cells[1].Value = "0,7";    // omega
                dataGridView2.Rows[8].Cells[1].Value = "50";     // NSTEP
                dataGridView2.Rows[9].Cells[1].Value = "20";     // tau
                dataGridView2.Rows[10].Cells[1].Value = "0,2";

                dataGridView1.Rows[0].Cells[1].Value = "-5";
                dataGridView1.Rows[0].Cells[2].Value = "5";
                dataGridView1.Rows[1].Cells[1].Value = "-5";
                dataGridView1.Rows[1].Cells[2].Value = "5";

                exact = 14.060606;

                A[0] = 2F;
                A[1] = 8F;
                A[2] = 10F;//0.5000001F;
                A[3] = 12F;
                A[4] = 14F;
                flag = true;
                pictureBox2.Image = Properties.Resources.седьмая1;
            }
            else if (comboBox1.SelectedIndex == 7)
            {
                dataGridView2.Rows[0].Cells[1].Value = "30";     // NP
                dataGridView2.Rows[1].Cells[1].Value = "100";  // ITER (как раньше K)
                dataGridView2.Rows[2].Cells[1].Value = "10";     // Smax
                dataGridView2.Rows[3].Cells[1].Value = "2";      // Smin
                dataGridView2.Rows[4].Cells[1].Value = "2";      // p
                dataGridView2.Rows[5].Cells[1].Value = "20";     // sigmaInitial (для больших областей можно больше)
                dataGridView2.Rows[6].Cells[1].Value = "0,5";    // sigmaFinal
                dataGridView2.Rows[7].Cells[1].Value = "0,7";    // omega
                dataGridView2.Rows[8].Cells[1].Value = "50";     // NSTEP
                dataGridView2.Rows[9].Cells[1].Value = "20";     // tau
                dataGridView2.Rows[10].Cells[1].Value = "0,2";

                dataGridView1.Rows[0].Cells[1].Value = "-5";
                dataGridView1.Rows[0].Cells[2].Value = "5";
                dataGridView1.Rows[1].Cells[1].Value = "-5";
                dataGridView1.Rows[1].Cells[2].Value = "5";

                exact = 1;

                A[0] = 0.1F;
                A[1] = 0.15F;
                A[2] = 0.2F;//0.5000001F;
                A[3] = 0.3F;
                A[4] = 0.5F;
                flag = true;
                pictureBox2.Image = Properties.Resources.trapfall;
            }
            else if (comboBox1.SelectedIndex == 8)
            {
                dataGridView2.Rows[0].Cells[1].Value = "30";     // NP
                dataGridView2.Rows[1].Cells[1].Value = "100";  // ITER (как раньше K)
                dataGridView2.Rows[2].Cells[1].Value = "10";     // Smax
                dataGridView2.Rows[3].Cells[1].Value = "2";      // Smin
                dataGridView2.Rows[4].Cells[1].Value = "2";      // p
                dataGridView2.Rows[5].Cells[1].Value = "20";     // sigmaInitial (для больших областей можно больше)
                dataGridView2.Rows[6].Cells[1].Value = "0,5";    // sigmaFinal
                dataGridView2.Rows[7].Cells[1].Value = "0,7";    // omega
                dataGridView2.Rows[8].Cells[1].Value = "50";     // NSTEP
                dataGridView2.Rows[9].Cells[1].Value = "20";     // tau
                dataGridView2.Rows[10].Cells[1].Value = "0,2";

                dataGridView1.Rows[0].Cells[1].Value = "-2";
                dataGridView1.Rows[0].Cells[2].Value = "2";
                dataGridView1.Rows[1].Cells[1].Value = "-2";
                dataGridView1.Rows[1].Cells[2].Value = "2";

                exact = 0;

                A[0] = -350F;
                A[1] = -180F;
                A[2] = -30F;//0.5000001F;
                A[3] = -4F;
                A[4] = -0.5F;
                flag = true;
                pictureBox2.Image = Properties.Resources.розенброк1;
            }
            else if (comboBox1.SelectedIndex == 9)
            {
                dataGridView2.Rows[0].Cells[1].Value = "30";     // NP
                dataGridView2.Rows[1].Cells[1].Value = "100";  // ITER (как раньше K)
                dataGridView2.Rows[2].Cells[1].Value = "10";     // Smax
                dataGridView2.Rows[3].Cells[1].Value = "2";      // Smin
                dataGridView2.Rows[4].Cells[1].Value = "2";      // p
                dataGridView2.Rows[5].Cells[1].Value = "20";     // sigmaInitial (для больших областей можно больше)
                dataGridView2.Rows[6].Cells[1].Value = "0,5";    // sigmaFinal
                dataGridView2.Rows[7].Cells[1].Value = "0,7";    // omega
                dataGridView2.Rows[8].Cells[1].Value = "50";     // NSTEP
                dataGridView2.Rows[9].Cells[1].Value = "20";     // tau
                dataGridView2.Rows[10].Cells[1].Value = "0,2";

                exact = 1;

                A[0] = -7F;
                A[1] = -4F;
                A[2] = -2F;//0.5000001F;
                A[3] = -0.8F;
                A[4] = -0.1F;
                flag = true;
                pictureBox2.Image = Properties.Resources.параболическая;
            }
            
            A[5] = 0;
            A[6] = 0;
            A[7] = 0;
            for (int i = 0; i < 5; i++)
                dataGridView3.Rows[i].Cells[1].Value = "";
            for (int i = 0; i < 8; i++)
            {
                flines[i] = true;
                Ar[i] = A[i];
            }
            flines[5] = false;
            flines[6] = false;
            flines[7] = false;

            forma3.flines = flines;
            flag2 = false;


            showobl[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
            showobl[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
            showobl[1, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
            showobl[1, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);

            showoblbase[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
            showoblbase[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
            showoblbase[1, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
            showoblbase[1, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);

            //numericUpDown5.Value = 1;
            button8.Enabled = false;
            forma3.Ar = Ar;
            forma3.obl = showobl;
            pictureBox1.Refresh();
            dataGridView1.Refresh();

        }

        float k = 1;
        float[] A = new float[8];
        float[] Ar = new float[8];
        double[,] showoblbase = new double[2, 2];
        double[,] showobl = new double[2, 2];
        bool flag = false;
        bool flag2 = false;

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            float w = pictureBox1.Width;
            float h = pictureBox1.Height;
            if (w <= 1 || h <= 1) return;
            float x0 = w/2;
            float y0 = h/2;
            float a = 30;
            //dataGridView1.Rows[0].Cells[1].Value = "-500";
            //dataGridView1.Rows[0].Cells[2].Value = "500";
            
            List<PointF> points = new List<PointF>();
            Pen p1 = new Pen(Color.PaleGreen, 1);
            Pen p2 = new Pen(Color.GreenYellow, 1);
            Pen p3 = new Pen(Color.YellowGreen, 1);
            Pen p4 = new Pen(Color.Yellow, 1);
            Pen p5 = new Pen(Color.Orange, 1);
            Pen p6 = new Pen(Color.OrangeRed, 1);
            Pen p7 = new Pen(Color.Red, 1);
            Pen p8 = new Pen(Color.Brown, 1);
            Pen p9 = new Pen(Color.Maroon, 1);
            Pen p10 = new Pen(Color.Black, 1);
            Pen p11 = new Pen(Color.Blue, 4);

            Font font1 = new Font("TimesNewRoman", 10, FontStyle.Bold);
            Font font2 = new Font("TimesNewRoman", 8);
            
            pictureBox1.BackColor = Color.White;
           // int a = Convert.ToInt32(pictureBox1.Height);
           // int b = Convert.ToInt32(pictureBox1.Width);
            if (flag == true)
            {
                double x1 = showobl[0, 0];//Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
                double x2 = showobl[0, 1];//Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
                double y1 = showobl[1, 0];//Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
                double y2 = showobl[1, 1];//Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);

                double eps = 1e-12;

                if (Math.Abs(x2 - x1) < eps)
                {
                    x2 = x1 + 1; // или +eps, но для рисования лучше заметный шаг
                }
                if (Math.Abs(y2 - y1) < eps)
                {
                    y2 = y1 + 1;
                }


                int z = comboBox1.SelectedIndex;
                flines = forma3.flines;
                Ar = forma3.Ar;
                double a1 = Ar[0];//5
                double a3 = Ar[1];//4
                double a5 = Ar[2];//3
                double a7 = Ar[3];//2
                double a9 = Ar[4];//1

                double a10 = Ar[5];//6
                double a11 = Ar[6];//7
                double a12 = Ar[7];//8

                double dx = x2 - x1;
                double dy = y2 - y1;
                double dxy = dx-dy;


                double bxy = Math.Max(dx, dy);
                double step =0;
                if (bxy < 1.1) step = 0.1;
                else if (bxy < 2.1) step = 0.2;
                else if (bxy < 5.1) step = 0.5;
                else if (bxy < 10.1) step = 1;
                else if (bxy < 20.1) step = 2;
                else if (bxy < 50.1) step = 5;
                else if (bxy < 100.1) step = 10;
                else if (bxy < 200.1) step = 20;
                else if (bxy < 500.1) step = 50;
                else if (bxy < 1000.1) step = 100;
                else if (bxy < 2000.1) step = 200;
                else step = 1000;

                float mw = k * (w) / ((float)(x2 - x1));
                float mh = k * (h) / ((float)(y2 - y1));

                int wi = pictureBox1.Width;
                int hi = pictureBox1.Height;


                for (int ii = 0; ii < wi; ii++)
                    for (int jj = 0; jj < hi; jj++)
                    {
                        double i = (ii * (x2 - x1) / w + x1) / k;
                        double j = (jj * (y2 - y1) / h + y1) / k;
                        double i1 = ((ii + 1) * (x2 - x1) / w + x1) / k;
                        double j1 = ((jj + 1) * (y2 - y1) / h + y1) / k;
                        double i0 = ((ii - 1) * (x2 - x1) / w + x1) / k;
                        double j0 = ((jj - 1) * (y2 - y1) / h + y1) / k;
                        double f = function2d(i, j, z);// j / k * Math.Sin(Math.Sqrt(Math.Abs(j / k))) + i / k * Math.Sin(Math.Sqrt(Math.Abs(i / k)));
                        double f2 = function2d(i0, j, z); //(j - 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j - 1) / k)) + i / k * Math.Sin(Math.Sqrt(Math.Abs(i / k)));
                        double f3 = function2d(i, j0, z); //(j / k) * Math.Sin(Math.Sqrt(Math.Abs(j / k))) + (i - 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i - 1) / k)));
                        double f4 = function2d(i1, j, z); //(j + 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j + 1) / k)) + (i / k) * Math.Sin(Math.Sqrt(Math.Abs(i / k)));
                        double f5 = function2d(i, j1, z); //(j / k) * Math.Sin(Math.Sqrt(Math.Abs(j / k))) + (i + 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i + 1) / k)));
                        double f6 = function2d(i1, j1, z); //(j + 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j + 1) / k)) + (i + 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i + 1) / k)));
                        double f7 = function2d(i0, j1, z); //(j - 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j - 1) / k)) + (i + 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i + 1) / k)));
                        double f8 = function2d(i1, j0, z); //(j + 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j + 1) / k)) + (i - 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i - 1) / k)));
                        double f9 = function2d(i0, j0, z); //(j - 1) / k * Math.Sin(Math.Sqrt(Math.Abs(j - 1) / k)) + (i - 1) / k * Math.Sin(Math.Sqrt(Math.Abs((i - 1) / k)));
                        
                        if (((f2 < a1) || (f3 < a1) || (f4 < a1) || (f5 < a1) || (f6 < a1) || (f7 < a1) || (f8 < a1) || (f9 < a1)) && (f > a1)&&(flines[4]==true)) e.Graphics.FillRectangle(Brushes.PaleGreen, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a3) || (f3 < a3) || (f4 < a3) || (f5 < a3) || (f6 < a3) || (f7 < a3) || (f8 < a3) || (f9 < a3)) && (f > a3)&&(flines[3]==true)) e.Graphics.FillRectangle(Brushes.YellowGreen, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a5) || (f3 < a5) || (f4 < a5) || (f5 < a5) || (f6 < a5) || (f7 < a5) || (f8 < a5) || (f9 < a5)) && (f > a5)&&(flines[2]==true)) e.Graphics.FillRectangle(Brushes.Orange, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a7) || (f3 < a7) || (f4 < a7) || (f5 < a7) || (f6 < a7) || (f7 < a7) || (f8 < a7) || (f9 < a7)) && (f > a7)&&(flines[1]==true)) e.Graphics.FillRectangle(Brushes.Red, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a9) || (f3 < a9) || (f4 < a9) || (f5 < a9) || (f6 < a9) || (f7 < a9) || (f8 < a9) || (f9 < a9)) && (f > a9)&&(flines[0]==true)) e.Graphics.FillRectangle(Brushes.Maroon, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a10) || (f3 < a10) || (f4 < a10) || (f5 < a10) || (f6 < a10) || (f7 < a10) || (f8 < a10) || (f9 < a10)) && (f > a10) && (flines[5] == true)) e.Graphics.FillRectangle(Brushes.Pink, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a11) || (f3 < a11) || (f4 < a11) || (f5 < a11) || (f6 < a11) || (f7 < a11) || (f8 < a11) || (f9 < a11)) && (f > a11) && (flines[6] == true)) e.Graphics.FillRectangle(Brushes.Violet, (float)(ii), (float)(h - jj), 1, 1);
                        else if (((f2 < a12) || (f3 < a12) || (f4 < a12) || (f5 < a12) || (f6 < a12) || (f7 < a12) || (f8 < a12) || (f9 < a12)) && (f > a12) && (flines[7] == true)) e.Graphics.FillRectangle(Brushes.MediumOrchid, (float)(ii), (float)(h - jj), 1, 1);

                    }

                if (flag2 == true && lastPopulation != null && lastPopulation.Count > 0)
                {
                    // чтобы точки не улетали за пределы (на всякий)
                    var oldClip = e.Graphics.Clip;
                    e.Graphics.SetClip(new RectangleF(0, 0, w, h));

                    for (int idx = 0; idx < lastPopulation.Count; idx++)
                    {
                        var v = lastPopulation[idx];
                        if (v == null || v.Length < 2) continue;

                        float px = SafeScreenX(v[0] * k, x1, x2, w);
                        float py = SafeScreenY(v[1] * k, y1, y2, h);
                        py = h - py;

                        e.Graphics.FillEllipse(Brushes.Blue, px - 3, py - 3, 5, 5);
                    }

                    if (bestPoint != null && bestPoint.Length >= 2)
                    {
                        float bx = SafeScreenX(bestPoint[0] * k, x1, x2, w);
                        float by = SafeScreenY(bestPoint[1] * k, y1, y2, h);
                        by = h - by;

                        e.Graphics.FillEllipse(Brushes.Red, bx - 4, by - 4, 7, 7);
                    }

                    e.Graphics.Clip = oldClip;
                }



                for (int i = -6; i < 12; i++)
                {
                    double gx = x1 - i * step;
                    double gy = y1 - i * step;

                    float sx = SafeScreenX(gx, x1, x2, w);
                    float sy = SafeScreenY(gy, y1, y2, h);

                    // вертикальные деления по X
                    if (!float.IsNaN(sx) && !float.IsInfinity(sx))
                    {
                        e.Graphics.DrawLine(p10, sx, h - a - 5, sx, h - a + 5);
                        e.Graphics.DrawString((i * step).ToString(), font2, Brushes.Black, sx, h - a + 5);
                    }

                    // горизонтальные деления по Y (в WinForms ось Y сверху вниз)
                    if (!float.IsNaN(sy) && !float.IsInfinity(sy))
                    {
                        float ypix = h - sy;
                        e.Graphics.DrawLine(p10, a - 5, ypix, a + 5, ypix);
                        e.Graphics.DrawString((i * step).ToString(), font2, Brushes.Black, a - 30, ypix - 5);
                    }
                }

            }
            e.Graphics.DrawLine(p10, 0, h - a, w, h - a);
            e.Graphics.DrawLine(p10, a, h, a, 0);
            e.Graphics.DrawLine(p10, a, 0, a-5, 10);
            e.Graphics.DrawLine(p10, a, 0, a+5, 10);
            e.Graphics.DrawLine(p10, w-5, h - a, w-15, h-a-5);
            e.Graphics.DrawLine(p10, w-5, h - a, w-15, h-a+5);
            e.Graphics.DrawString("x", font1, Brushes.Black, w - 20, h - a + 5);
            e.Graphics.DrawString("y", font1, Brushes.Black, a - 20, 1);

           
        }

        private float function2d(double x1, double x2, int f)
        {
            if (f == 13)
            {
                return function(x1, x2, 7.2, 0, f);
            }
            else if (f == 11)
            {
                return function(x1, x2, 42.3, 175.0, f);
            }
            else if (f == 10)
            {
                return function(x1, x2, 9.36, 0.2, f);
            }
            return function(x1, x2, 0, 0, f);
        }

        private float function(double x1, double x2, double x3, double x4, int f)
        { 
            float funct = 0;
            if (f == 0)
            {
                funct =(float) (x1 * Math.Sin(Math.Sqrt(Math.Abs(x1))) + x2 * Math.Sin(Math.Sqrt(Math.Abs(x2))));
            }
            else if (f == 1)
            {
                funct = (float)(x1 * Math.Sin(4 * Math.PI * x1) - x2 * Math.Sin(4 * Math.PI * x2 + Math.PI) + 1);
            }
            else if (f == 2)
            { 
                double[] c6 = Cpow(x1,x2,6);
                funct = (float)(1 / (1 + Math.Sqrt((c6[0] - 1) * (c6[0] - 1) + c6[1] * c6[1])));
            }
            else if (f == 3)
            {
                funct = (float)(0.5-(Math.Pow(Math.Sin(Math.Sqrt(x1*x1+x2*x2)),2)-0.5)/(1+0.001*(x1*x1+x2*x2)));
            }
            else if (f == 4)
            {
                funct = (float)(-x1 * x1 - x2 * x2 + 10 * Math.Cos(2 * Math.PI * x1) + 10 * Math.Cos(2 * Math.PI * x2));

                
                     //funct = (float)((-x1 * x1 + 10 * Math.Cos(2 * Math.PI * x1)) + (-x2 * x2 + 10 * Math.Cos(2*Math.PI * x2)));

            }
            else if (f == 5)
            {
                funct = (float)(-Math.E + 20 * Math.Exp(-0.2 * Math.Sqrt((x1 * x1 + x2 * x2) / 2)) + Math.Exp((Math.Cos(2 * Math.PI * x1) + Math.Cos(2 * Math.PI * x2)) / 2));
            }
            else if (f == 6)
            {
                funct = (float)(Math.Pow(Math.Cos(2 * x1 * x1) - 1.1, 2) + Math.Pow(Math.Sin(0.5 * x1) - 1.2, 2) - Math.Pow(Math.Cos(2 * x2 * x2) - 1.1, 2) + Math.Pow(Math.Sin(0.5 * x2) - 1.2, 2));
            }
            else if (f == 7)
            {
                funct = (float)(-Math.Sqrt(Math.Abs(Math.Sin(Math.Sin(Math.Sqrt(Math.Abs(Math.Sin(x1-1)))+Math.Sqrt(Math.Abs(Math.Sin(x2+2)))))))+1);
            }
            else if (f == 8)
            {
                funct = (float)(-(1 - x1) * (1 - x1) - 100 * (x2 - x1 * x1) * (x2 - x1 * x1));
            }
            else if (f == 9)
            {
                funct = (float)(1-x1 * x1 - x2 * x2);
            }


            
           
            return funct;
        }


        private double[] Cpow(double x, double y, int p)
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

       


        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

       // private void numericUpDown5_ValueChanged(object sender, EventArgs e)
       // {
       //     k = (float)numericUpDown5.Value;
       //     pictureBox1.Refresh();
       // }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        Form2 forma2 = new Form2();

        private void button8_Click(object sender, EventArgs e)
        {
            if (forma2.IsDisposed) forma2 = new Form2();
            //forma2 = new Form2();
            forma2.averageF = alg.AverF;
            forma2.bestF = alg.BestF;
            forma2.Population = alg.Population;
            forma2.FillList();
            forma2.Show();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Process.Start("fish.pdf");
            MessageBox.Show("Здесь будет Справка");
        }


        Form4 forma4 = new Form4();
        

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                int z = comboBox1.SelectedIndex;

                double[,] obl = new double[n, 2];

                int error = 0;

                if (z < 10)
                {
                    try
                    {
                        obl[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
                        obl[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
                        for (int k = 1; k < n; k++)
                        {
                            obl[k, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
                            obl[k, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);
                        }
                        obl[n - 1, 0] = obl[0, 0];
                        obl[n - 1, 1] = obl[0, 1];
                    }
                    catch
                    {
                        error = 1;
                    }
                    if ((obl[0, 1] - obl[0, 0] < 0) || (obl[1, 1] - obl[1, 0] < 0)) error = 1;
                }
                else
                {
                    if (z == 10)
                    {
                        obl[0, 0] = 0.1;
                        obl[0, 1] = 2;
                        obl[1, 0] = 0.1;
                        obl[1, 1] = 10;
                        obl[2, 0] = 0.1;
                        obl[2, 1] = 10;
                        obl[3, 0] = 0.1;
                        obl[3, 1] = 2;
                    }
                    else if (z == 11)
                    {
                        obl[0, 0] = 1;
                        obl[0, 1] = 99.99;
                        obl[1, 0] = 1;
                        obl[1, 1] = 99.99;
                        obl[2, 0] = 10;
                        obl[2, 1] = 200;
                        obl[3, 0] = 10;
                        obl[3, 1] = 200;
                    }
                    else if (z == 12)
                    {
                        obl[0, 0] = 2.6;
                        obl[0, 1] = 3.6;
                        obl[1, 0] = 0.7;
                        obl[1, 1] = 0.8;
                        obl[2, 0] = 17;
                        obl[2, 1] = 28.99;
                        obl[3, 0] = 7.3;
                        obl[3, 1] = 8.3;
                        obl[4, 0] = 7.8;
                        obl[4, 1] = 8.3;
                        obl[5, 0] = 2.9;
                        obl[5, 1] = 3.9;
                        obl[6, 0] = 5;
                        obl[6, 1] = 5.5;
                    }
                    else if (z == 13)
                    {
                        obl[0, 0] = 0.05;
                        obl[0, 1] = 0.2;
                        obl[1, 0] = 0.25;
                        obl[1, 1] = 1.3;
                        obl[2, 0] = 2;
                        obl[2, 1] = 15;

                    }
                }

                int Ip = 0;
                int K = 0;
                double thr = 0;
                double wScale = 1;
                double stepInd0 = 0.5;
                double stepInd1 = 0;
                //int Klon = 0;
                //double parkl = 0;
                //double g = 0;
                //double eps = 0;
                //double sigma = 0;
                //int percent = 0;
                try
                {
                    Ip = Convert.ToInt32(dataGridView2.Rows[0].Cells[1].Value);
                    K = Convert.ToInt32(dataGridView2.Rows[1].Cells[1].Value);
                    thr = Convert.ToDouble(dataGridView2.Rows[2].Cells[1].Value);
                    wScale = Convert.ToDouble(dataGridView2.Rows[3].Cells[1].Value);
                    stepInd0 = Convert.ToDouble(dataGridView2.Rows[4].Cells[1].Value);
                    stepInd1 = Convert.ToDouble(dataGridView2.Rows[5].Cells[1].Value);
                   
                }
                catch
                {
                    error = 2;
                }
                if ((Ip <= 0) || (K <= 0) ||  (thr <= 0) || (wScale <= 0)) error = 2;

                if (error == 0)
                {
                    flag2 = false;
                    if ((forma4.IsDisposed)||(first==true))
                    {
                        first = false;
                        forma4 = new Form4();
                        alg = new Algoritm();
                        alg.Init(Ip, K, obl, n, z, thr, wScale, stepInd0, stepInd1);
                        if (z == 11)
                        {
                            alg.optimalStart.v[0] = 20;
                            alg.optimalD.v[0] = 18;
                            alg.optimalStart.v[1] = 20;
                            alg.optimalD.v[1] = 18;
                            alg.optimalStart.v[2] = 42.5;
                            alg.optimalD.v[2] = 0.2;
                            alg.optimalStart.v[3] = 175;
                            alg.optimalD.v[3] = 1;
                        }
                        forma4.algst = alg;
                        forma4.showobl = showobl;
                        forma4.z = comboBox1.SelectedIndex;
                        forma4.Ar = Ar;
                        forma4.flines = flines;
                        forma4.exact = exact;
                        forma4.Ip = Ip;
                        forma4.K = K;
                        forma4.obl = obl;
                        forma4.thr = thr;
                        forma4.wScale = wScale;
                        forma4.stepInd0 = stepInd0;
                        forma4.stepInd1 = stepInd1;
                        forma4.n = n;
                      

                        forma4.A = A;
                        forma4.Ar = Ar;
                        forma4.flines = flines;
                        forma4.obl = obl;
                        forma4.showobl = showobl;
                        forma4.showoblbase = showoblbase;
                        forma4.Fname = comboBox1.Items[comboBox1.SelectedIndex].ToString().Split('.')[1];
                        forma4.Show();
                    }
                }
                else switch (error)
                    {
                        case 1:
                            MessageBox.Show("Неверно введена область определения функции!");
                            break;
                        case 2:
                            MessageBox.Show("Неверно введены параметры алгоритма!");
                            break;
                    }


                
            }
            else MessageBox.Show("Выберите целевую функцию!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Process.Start("protocol.dt");
        }

        Form3 forma3 = new Form3();

        private void button7_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                if (forma3.IsDisposed) forma3 = new Form3();
                forma3.A = A;
                forma3.Ar = Ar;
                forma3.flines = flines;
                forma3.obl = showobl;
                forma3.oblbase = showoblbase;
                forma3.Show();
                //MessageBox.Show("Здесь будет открываться окно с настройками: \r\n 1) объяснение, какому уровню соответствуют разные цвета и возможность поменять уровень \r\n 2) возможность добавить уровень \r\n 3) возможность поменять показываемый диапазон");
            }
            else MessageBox.Show("Выберите целевую функцию!");
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dataGridView1_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
           
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            showobl[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
            showobl[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
            showobl[1, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
            showobl[1, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);

            showoblbase[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
            showoblbase[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
            showoblbase[1, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
            showoblbase[1, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);

            pictureBox1.Refresh();
        }

        private float SafeScreenX(double x, double x1, double x2, float w)
        {
            double denom = (x2 - x1);
            if (Math.Abs(denom) < 1e-12) return 0f;
            double t = (x - x1) * w / denom;
            if (double.IsNaN(t) || double.IsInfinity(t)) return 0f;
            if (t > 1e7) t = 1e7;
            if (t < -1e7) t = -1e7;
            return (float)t;
        }

        private float SafeScreenY(double y, double y1, double y2, float h)
        {
            double denom = (y2 - y1);
            if (Math.Abs(denom) < 1e-12) return 0f;
            double t = (y - y1) * h / denom;
            if (double.IsNaN(t) || double.IsInfinity(t)) return 0f;
            if (t > 1e7) t = 1e7;
            if (t < -1e7) t = -1e7;
            return (float)t;
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private string GetMethodName()
        {
            if (comboBoxMethod.SelectedIndex == (int)OptimizationMethod.Beam)
                return "Beam";
            if (comboBoxMethod.SelectedIndex == (int)OptimizationMethod.Forecast)
                return "Forecast";
            if (comboBoxMethod.SelectedIndex == (int)OptimizationMethod.Dds)
                return "DDS";

            return "Unknown";
        }
        private List<double[]> GetIdealPoints(int z)
        {
            var result = new List<double[]>();

            if (z == 0) // швефель
            {
                result.Add(new double[] { 420.9687, 420.9687 });
            }
            else if (z == 1) // мульти
            {
                for (int k = 0; k < 4; k++)
                {
                    double a = k * Math.PI;
                    double x = 1.6288 * (Math.Cos(a) + Math.Pow(-1, k) * Math.Sin(a));
                    double y = 1.6288 * (Math.Cos(a) + Math.Sin(a));
                    result.Add(new double[] { x, y });
                }
            }
            else if (z == 2) // рут
            {
                for (int k = 0; k < 6; k++)
                {
                    double a = k * Math.PI / 3.0;
                    result.Add(new double[] { Math.Cos(a), Math.Sin(a) });
                }
            }
            else if (z == 3) // шафер
            {
                result.Add(new double[] { 0, 0 });
            }
            else if (z == 4) // растригин
            {
                result.Add(new double[] { 0, 0 });
            }
            else if (z == 5) // аклей
            {
                result.Add(new double[] { 0, 0 });
            }
            else if (z == 6) // кожа
            {
                result.Add(new double[] { -3.316, -3.073 });
            }
            else if (z == 7) // ловушка
            {
                result.Add(new double[] { 0, 0 });
            }
            else if (z == 8) // розенброк
            {
                result.Add(new double[] { 1, 1 });
            }
            else if (z == 9) // параболическая
            {
                result.Add(new double[] { 0, 0 });
            }

            return result;
        }

        private bool IsCloseToIdeal(double[] point, List<double[]> idealPoints, double[,] bounds)
        {
            if (point == null || point.Length < 2 || idealPoints == null || idealPoints.Count == 0)
                return false;

            double epsX = (bounds[0, 1] - bounds[0, 0]) / 1000.0;
            double epsY = (bounds[1, 1] - bounds[1, 0]) / 1000.0;
            double eps = Math.Max(epsX, epsY);

            foreach (var ideal in idealPoints)
            {
                double dx = point[0] - ideal[0];
                double dy = point[1] - ideal[1];
                double dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist <= eps)
                    return true;
            }

            return false;
        }

        private OptimizationResult RunSelectedOptimizer(GenerationBeamsModel model)
        {
            var method = (OptimizationMethod)comboBoxMethod.SelectedIndex;

            if (method == OptimizationMethod.Beam)
                return beam.RunOptimizer(model);

            if (method == OptimizationMethod.Dds)
                return ddsService.RunOptimizer(model);

            return forecastService.RunOptimizer(model);
        }
        private bool TryBuildModelFromUi(out GenerationBeamsModel model, out int functionNumber)
        {
            model = null;
            functionNumber = -1;

            if (comboBoxMethod.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите метод оптимизации!");
                return false;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите целевую функцию!");
                return false;
            }

            functionNumber = comboBox1.SelectedIndex;

            double[,] bounds = new double[2, 2];
            try
            {
                bounds[0, 0] = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
                bounds[0, 1] = Convert.ToDouble(dataGridView1.Rows[0].Cells[2].Value);
                bounds[1, 0] = Convert.ToDouble(dataGridView1.Rows[1].Cells[1].Value);
                bounds[1, 1] = Convert.ToDouble(dataGridView1.Rows[1].Cells[2].Value);
            }
            catch
            {
                MessageBox.Show("Неверно введена область определения функции!");
                return false;
            }

            if (bounds[0, 1] < bounds[0, 0] || bounds[1, 1] < bounds[1, 0])
            {
                MessageBox.Show("Неверно введена область определения функции!");
                return false;
            }

            model = new GenerationBeamsModel
            {
                nCount = 2,
                Bounds = bounds,
                FunctionNumber = functionNumber,
                knownMinimum = exact,
                x0 = 0
            };

            var method = (OptimizationMethod)comboBoxMethod.SelectedIndex;

            if (method == OptimizationMethod.Beam)
            {
                int NP, ITER, Smax, Smin, pBeam, NSTEP, tau;
                double sigmaInitial, sigmaFinal, omega, s;

                try
                {
                    NP = Convert.ToInt32(dataGridView2.Rows[0].Cells[1].Value);
                    ITER = Convert.ToInt32(dataGridView2.Rows[1].Cells[1].Value);
                    Smax = Convert.ToInt32(dataGridView2.Rows[2].Cells[1].Value);
                    Smin = Convert.ToInt32(dataGridView2.Rows[3].Cells[1].Value);
                    pBeam = Convert.ToInt32(dataGridView2.Rows[4].Cells[1].Value);

                    sigmaInitial = Convert.ToDouble(dataGridView2.Rows[5].Cells[1].Value);
                    sigmaFinal = Convert.ToDouble(dataGridView2.Rows[6].Cells[1].Value);
                    omega = Convert.ToDouble(dataGridView2.Rows[7].Cells[1].Value);

                    NSTEP = Convert.ToInt32(dataGridView2.Rows[8].Cells[1].Value);
                    tau = Convert.ToInt32(dataGridView2.Rows[9].Cells[1].Value);
                    s = Convert.ToDouble(dataGridView2.Rows[10].Cells[1].Value);
                }
                catch
                {
                    MessageBox.Show("Неверно введены параметры Beam!");
                    return false;
                }

                if (NP <= 0 || ITER <= 0 || Smax <= 0 || Smin <= 0 || Smin > Smax || NSTEP <= 0 || tau <= 0)
                {
                    MessageBox.Show("Неверно введены параметры Beam!");
                    return false;
                }

                model.NP = NP;
                model.ITER = ITER;
                model.Smax = Smax;
                model.Smin = Smin;
                model.p = pBeam;
                model.sigmaInitial = sigmaInitial;
                model.sigmaFinal = sigmaFinal;
                model.omega = omega;
                model.NSTEP = NSTEP;
                model.tau = tau;
                model.s = s;
            }
            else if (method == OptimizationMethod.Dds)
            {
                if (!TryReadDdsParams(out int ddsNP, out int ddsPmax, out int ddsK, out double ddsH,
                                      out double qW, out double sW, out double lW))
                    return false;

                model.Dds_NP = ddsNP;
                model.Dds_Pmax = ddsPmax;
                model.Dds_K = ddsK;
                model.Dds_h = ddsH;
                model.Dds_qWeight = qW;
                model.Dds_sWeight = sW;
                model.Dds_lambdaWeight = lW;
            }
            else
            {
                if (!TryReadForecastParams(out int fN, out int fP, out int fK, out double fR,
                                           out double fAlpha, out double fEta, out double fBeta, out double fLambda))
                    return false;

                model.Forecast_N = fN;
                model.Forecast_P = fP;
                model.Forecast_K = fK;
                model.Forecast_R = fR;
                model.Forecast_alpha = fAlpha;
                model.Forecast_eta = fEta;
                model.Forecast_beta = fBeta;
                model.Forecast_lambda = fLambda;
            }

            return true;
        }
        //private RunStatisticsResult CalculateStatistics(GenerationBeamsModel model, int repeatCount)
        //{
        //    var stat = new RunStatisticsResult();
        //    stat.RepeatCount = repeatCount;

        //    double sum = 0.0;
        //    double bestValue = double.MinValue;
        //    double[] bestPointOverall = null;

        //    var idealPoints = GetIdealPoints(model.FunctionNumber);

        //    for (int i = 0; i < repeatCount; i++)
        //    {
        //        OptimizationResult res = RunSelectedOptimizer(model);

        //        double value = res.BestValue;
        //        stat.Values.Add(value);
        //        sum += value;

        //        if (value > bestValue)
        //        {
        //            bestValue = value;
        //            bestPointOverall = res.BestX != null ? (double[])res.BestX.Clone() : null;
        //        }

        //        if (IsCloseToIdeal(res.BestX, idealPoints, model.Bounds))
        //            stat.GoodCount++;
        //    }

        //    stat.MeanValue = sum / repeatCount;
        //    stat.BestValue = bestValue;
        //    stat.BestPointOverall = bestPointOverall;

        //    double s2 = 0.0;
        //    for (int i = 0; i < stat.Values.Count; i++)
        //        s2 += Math.Pow(stat.Values[i] - stat.MeanValue, 2);

        //    stat.Sigma = Math.Sqrt(s2 / repeatCount);

        //    return stat;
        //}

        private RunStatisticsResult CalculateStatistics(GenerationBeamsModel model, int repeatCount)
        {
            var stat = new RunStatisticsResult();
            stat.RepeatCount = repeatCount;

            double sumDelta = 0.0;
            double bestDelta = double.MaxValue;
            double bestValueOverall = double.MinValue;
            double[] bestPointOverall = null;

            var idealPoints = GetIdealPoints(model.FunctionNumber);

            for (int i = 0; i < repeatCount; i++)
            {
                OptimizationResult res = RunSelectedOptimizer(model);

                double value = res.BestValue;
                double delta = Math.Abs(model.knownMinimum - value);

                stat.Values.Add(delta);
                sumDelta += delta;

                if (delta < bestDelta)
                    bestDelta = delta;

                if (value > bestValueOverall)
                {
                    bestValueOverall = value;
                    bestPointOverall = res.BestX != null ? (double[])res.BestX.Clone() : null;
                }

                if (IsCloseToIdeal(res.BestX, idealPoints, model.Bounds))
                    stat.GoodCount++;
            }

            stat.MeanValue = sumDelta / repeatCount;
            stat.BestValue = bestDelta;
            stat.BestPointOverall = bestPointOverall;

            double s2 = 0.0;
            for (int i = 0; i < stat.Values.Count; i++)
                s2 += Math.Pow(stat.Values[i] - stat.MeanValue, 2);

            stat.Sigma = Math.Sqrt(s2 / repeatCount);

            return stat;
        }

        private void SaveStatisticsToFile(RunStatisticsResult stat, int functionNumber, GenerationBeamsModel model)
        {
            string methodName = GetMethodName();
            string path = Path.Combine(Application.StartupPath, "statistics-" + functionNumber + "-" + methodName + ".txt");

            using (var sw = new StreamWriter(path, false, Encoding.ASCII))
            {
                sw.WriteLine("Run\tBestValue");

                for (int i = 0; i < stat.Values.Count; i++)
                {
                    sw.WriteLine((i + 1).ToString()
                        + "\t" + Math.Round(stat.Values[i], 6).ToString("0.000000"));
                }

                sw.WriteLine();
                sw.WriteLine("E f\tmax f\tsigma\tnum.good");
                sw.WriteLine(
                    Math.Round(stat.MeanValue, 6).ToString("0.000000") + "\t" +
                    Math.Round(stat.BestValue, 6).ToString("0.000000") + "\t" +
                    Math.Round(stat.Sigma, 6).ToString("0.000000") + "\t" +
                    stat.GoodCount.ToString());
            }

            MessageBox.Show("Файл сохранён:\n" + path);
        }

        private void buttonStat_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TryBuildModelFromUi(out GenerationBeamsModel model, out int functionNumber))
                    return;

                RunStatisticsResult stat = CalculateStatistics(model, 100);
                SaveStatisticsToFile(stat, functionNumber, model);

                dataGridView3.Rows[0].Cells[1].Value = "100";
                dataGridView3.Rows[1].Cells[1].Value = stat.BestPointOverall != null ? "(" + vectorText(stat.BestPointOverall) + ")" : "";
                dataGridView3.Rows[2].Cells[1].Value = Math.Round(stat.BestValue, 6).ToString();
                dataGridView3.Rows[3].Cells[1].Value = exact.ToString();
                dataGridView3.Rows[4].Cells[1].Value =
                    "Ef=" + Math.Round(stat.MeanValue, 6).ToString()
                    + "; σ=" + Math.Round(stat.Sigma, 6).ToString()
                    + "; good=" + stat.GoodCount.ToString();

                bestPoint = stat.BestPointOverall;
                bestValue = stat.BestValue;
                lastPopulation = new List<double[]>();
                if (stat.BestPointOverall != null)
                    lastPopulation.Add((double[])stat.BestPointOverall.Clone());

                flag2 = true;
                pictureBox1.Refresh();

                //MessageBox.Show("Статистика по 100 запускам посчитана.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при расчёте статистики: " + ex.Message);
            }
        }
    }
}
