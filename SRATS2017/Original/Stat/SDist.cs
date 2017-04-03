using SRATS.Utils;

namespace SRATS.Stat
{
	public abstract class SDist
    {
        //private int npara;

		protected SDist()
        {
            //npara = n;
        }

        public abstract double Pdf(double x);
        public abstract double Cdf(double x);
        public abstract double Ccdf(double x);
        public abstract double Mean();
        public abstract double Variance();

        protected virtual double QuantileInit()
        {
            return Mean();
        }

        public virtual double Quantile(double p, double eps)
        {
            int cnt = 1;
            double x, xn;
            x = QuantileInit(); // init x

            do
            {
                xn = x - 0.1 * (Cdf(x) - p) / Pdf(x);
                if (double.IsInfinity(x) || double.IsNaN(x))
                {
					return double.NaN;
                }
                if (NMath.Abs(xn - x) < eps * NMath.Abs(x))
                {
                    break;
                }
                x = xn;
                cnt++;
                if (cnt >= 10000)
                {
                    break;
                }
            } while (true);
            return xn;
        }
    }
}
