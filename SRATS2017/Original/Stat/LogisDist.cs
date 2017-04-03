using SRATS.Utils;

namespace SRATS.Stat
{
    public class LogisDist : SDist
    {
        const double PI = 3.14159265358979324;
        const double LOG_PI = 1.14472988584940017;

		GetParam muFunc;
		GetParam phiFunc;

		protected double Mu
		{
			get { return muFunc(); }
		}

		protected double Phi
		{
			get { return phiFunc(); }
		}

        public LogisDist(GetParam mu, GetParam phi)
        {
            muFunc = mu;
            phiFunc = phi;
        }

        public override double Pdf(double x)
        {
            double y = NMath.Exp(-(x - Mu) / Phi);
            return y / (1.0 + y) / (1.0 + y) / Phi;
        }

        public override double Cdf(double x)
        {
            double y = NMath.Exp(-(x - Mu) / Phi);
            return 1.0 / (1.0 + y);
        }

        public override double Ccdf(double x)
        {
            double y = NMath.Exp(-(x - Mu) / Phi);
            return y / (1.0 + y);
        }

        public override double Mean()
        {
            return Mu;
        }

        public override double Variance()
        {
            return Phi * Phi * PI * PI / 3.0;
        }

        public override double Quantile(double p, double eps)
        {
            return Mu - Phi * NMath.Log((1 - p) / p);
        }
    }

    public class TruncLogisDist : LogisDist
	{
		protected double P0
		{
			get { return base.Ccdf(0.0); }
		}

		public TruncLogisDist(GetParam mu, GetParam phi)
			: base(mu, phi)
		{ }

		public override double Pdf(double x)
		{
			return base.Pdf(x) / P0;
		}

		public override double Cdf(double x)
		{
			return 1.0 - base.Ccdf(x) / P0;
		}

		public override double Ccdf(double x)
		{
			return base.Ccdf(x) / P0;
		}

		public override double Mean()
		{
			//TODO: this is the mean of TruncNormal. It should be modified.
			double R = P0 / base.Pdf(0.0);
			return Mu + Phi / R;
		}

		public override double Variance()
		{
			//TODO: this is the variance of TruncNormal. It should be modified.
			double s = -Mu / Phi;
			double R = P0 / base.Pdf(0.0);
			return Phi * Phi * (1.0 + s / R - 1.0 / R / R);
		}

		public override double Quantile(double p, double eps)
		{
			return base.Quantile(1.0 - (1.0 - p) * P0, eps);
		}
	}

    public class LogLogisDist : LogisDist
	{
		public LogLogisDist(GetParam mu, GetParam sigma)
			: base(mu, sigma)
		{
		}

		public override double Pdf(double x)
		{
			return base.Pdf(NMath.Log(x)) / x;
		}

		public override double Cdf(double x)
		{
			if (x == 0.0)
			{
				return 0.0;
			}
			return base.Cdf(NMath.Log(x));
		}

		public override double Ccdf(double x)
		{
			if (x == 0.0)
			{
				return 1.0;
			}
			return base.Ccdf(NMath.Log(x));
		}

		public override double Mean()
		{
			//TODO: this is the mean of TruncNormal. It should be modified.
			return NMath.Exp(Mu + Phi * Phi / 2.0);
		}

		public override double Variance()
		{
			//TODO: this is the mean of TruncNormal. It should be modified.
			return NMath.Exp(2.0 * Mu + Phi * Phi)
				* (NMath.Exp(Phi * Phi) - 1.0);
		}

		public override double Quantile(double p, double eps)
		{
			return NMath.Exp(base.Quantile(p, eps));
		}
	}
}
