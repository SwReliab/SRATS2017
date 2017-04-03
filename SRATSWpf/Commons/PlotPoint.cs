using System.Collections.ObjectModel;

namespace SRATS2017AddIn.Commons
{
    public class PlotPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PlotPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static ObservableCollection<PlotPoint> makeSeries(double[] t, double[] x)
        {
            ObservableCollection<PlotPoint> s = new ObservableCollection<PlotPoint>();
            for (int i = 0; i < t.Length; i++)
            {
                s.Add(new PlotPoint(t[i], x[i]));
            }
            return s;
        }

        public static double[] MakeSeq(double ts, double te, int d = 100)
        {
            double dx = (te - ts) / (d + 1);
            double[] t = new double[d + 2];
            t[0] = ts;
            t[d + 1] = te;
            for (int i = 1; i <= d; i++)
            {
                t[i] = t[i - 1] + dx;
            }
            return t;
        }

    }
}
