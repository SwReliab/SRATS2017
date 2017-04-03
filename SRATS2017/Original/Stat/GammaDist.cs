using SRATS.Utils;

namespace SRATS.Stat
{
    public class GammaDist : SDist
    {
		GetParam alphaFunc;
		GetParam betaFunc;

		protected double Alpha
		{
			get { return alphaFunc(); }
		}

		protected double Beta
		{
			get { return betaFunc(); }
		}

		private double Loggamma_a
		{
			get { return NMath.Lgamma(Alpha); }
		}

        public GammaDist(GetParam alpha, GetParam beta)
        {
			alphaFunc = alpha;
			betaFunc = beta;
        }

        public override double Pdf(double x)
        {
            double y = Beta * x;
            return Beta * NMath.Pow(y, Alpha - 1.0) * NMath.Exp(-y - Loggamma_a);
        }

        public override double Cdf(double x)
        {
            return NMath.P_gamma(Alpha, Beta * x, Loggamma_a);
        }

        public override double Ccdf(double x)
        {
            return NMath.Q_gamma(Alpha, Beta * x, Loggamma_a);
        }

        public override double Mean()
        {
            return Alpha / Beta;
        }

        public override double Variance()
        {
            return Alpha / (Beta * Beta);
        }
    }
}
