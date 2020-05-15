using SRATS;
using SRATS2017AddIn.Commons;
using System.Collections.Generic;

namespace SRATS2017AddIn.Models
{
    public class PlotModel
    {
        private SRMModel srm;
        private DataModel data;

        private List<string> labels = null;
        private List<object> values = null;
        private GraphData graph = null;

        public string Name
        {
            get
            {
                return srm.Name;
            }
        }

        public string SheetName { get; set; }

        public bool IsMVF { get; set; }
        public bool IsIntensity { get; set; }
        public bool IsReliability { get; set; }

        public double PlotRange { get; set; }

        public string[] ResultLabels
        {
            get
            {
                return labels.ToArray();
            }
        }

        public object[] ResultValues
        {
            get
            {
                return values.ToArray();
            }
        }

        public SRM SRM
        {
            get
            {
                return srm.SRM;
            }
        }

        public SRMData Data
        {
            get
            {
                return data.SRMData;
            }
        }

        public double[] MVFTime
        {
            get { return graph.MVFTime; }
        }

        public double[] MVF
        {
            get
            {
                return graph.MVF;
            }
        }

        public double[] IntensityTime
        {
            get {
                double[] tmp = new double[graph.MVFTime.Length - 1];
                for (int i=1; i<graph.MVFTime.Length; i++)
                {
                    tmp[i - 1] = graph.MVFTime[i];
                }
                return tmp;
            }
        }

        public double[] Intensity
        {
            get
            {
                double[] tmp = new double[graph.Intensity.Length - 1];
                for (int i = 1; i < graph.Intensity.Length; i++)
                {
                    tmp[i - 1] = graph.Intensity[i];
                }
                return tmp;
            }
        }

        public double[] ReliTime
        {
            get
            {
                double[] tmp = new double[graph.ReliTime.Length];
                for (int i = 0; i < graph.ReliTime.Length; i++)
                {
                    tmp[i] = graph.ReliTime[i] + Data.TotalTime;
                }
                return tmp;
            }
        }

        public double[] Reli
        {
            get
            {
                return graph.Reli;
            }
        }

        public PlotModel(SRMModel srm, DataModel data)
        {
            this.srm = srm;
            this.data = data;

            SheetName = "";
            IsMVF = true;
            IsIntensity = true;
            IsReliability = true;

            PlotRange = Data.TotalTime * 1.5;
        }

        public void MakeResult()
        {
            labels = new List<string>();
            labels.Add("Selected Model");
            labels.Add("Total Experienced Failures");
            labels.Add("Minimum Failure Time");
            labels.Add("Maximum Failure Time");
            labels.Add("Mean Failure Time");
            labels.Add("-------------------------------");
            labels.Add("The Number of Parameters");
            labels.Add("Degrees of freedom");
            for (int i = 0; i < srm.Param.Length; i++)
            {
                labels.Add("Parameter " + i);
            }
            labels.Add("-------------------------------");
            labels.Add("Status");
            labels.Add("Iteration");
            labels.Add("Maximum Log-Likelihood");
            labels.Add("AIC");
            labels.Add("BIC");
            labels.Add("MSE");
            labels.Add("-------------------------------");
            labels.Add("Predictive Total Faults");
            labels.Add("Predictive Residual Faults");
            labels.Add("Fault-Free Probability");
            labels.Add("Conditional MTTF");
            labels.Add("Cumulative MTTF");
            labels.Add("Instantaneous MTTF");
            labels.Add("Median");
            labels.Add("Be X Life");

            values = new List<object>();
            values.Add(srm.Name);
            values.Add(data.Total);
            values.Add(data.Min);
            values.Add(data.Max);
            values.Add(data.Mean);
            values.Add("");
            values.Add(srm.Param.Length);
            values.Add(srm.Df);
            for (int i=0; i<srm.Param.Length; i++)
            {
                values.Add(srm.Param[i]);
            }
            values.Add("");
            values.Add(srm.Status);
            values.Add(srm.Iter);
            values.Add(srm.Llf);
            values.Add(srm.Aic);
            values.Add(srm.Bic);
            values.Add(srm.Mse);
            values.Add("");
            values.Add(srm.Total);
            values.Add(srm.Residual);
            values.Add(srm.Ffp);
            values.Add(srm.CondMttf);
            values.Add(srm.CumMttf);
            values.Add(srm.InsMttf);
            values.Add(srm.Median);
            values.Add(srm.BeXLife);

            graph = srm.SRM.GraphData(PlotPoint.MakeSeq(0, PlotRange), PlotPoint.MakeSeq(0, PlotRange - Data.TotalTime));
        }
    }
}
