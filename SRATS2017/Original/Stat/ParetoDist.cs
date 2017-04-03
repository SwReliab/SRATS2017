using SRATS.Utils;

namespace SRATS.Stat
{
    public class ParetoDist : SDist
    {
		GetParam aFunc;
		GetParam cFunc;

		protected double A
		{
			get { return aFunc(); }
		}

		protected double C
		{
			get { return cFunc(); }
		}

        public ParetoDist(GetParam a, GetParam c)
        {
			aFunc = a;
			cFunc = c;
        }

        public override double Pdf(double x)
        {
            return A * NMath.Pow(C / (x + C), A) / (x + C);
        }

        public override double Cdf(double x)
        {
            return 1.0 - NMath.Pow(C / (x + C), A);
        }

        public override double Ccdf(double x)
        {
            return NMath.Pow(C / (x + C), A);
        }

        public override double Mean()
        {
            return C / (A - 1.0);
        }

        public override double Variance()
        {
            return C * C / (A - 1.0) / (A - 2.0);
        }

        public override double Quantile(double p, double eps)
        {
            return -C * (1.0 - 1.0 / NMath.Pow(1.0 - p, 1.0 / A));
        }
    }
}
