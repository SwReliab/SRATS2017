using SRATS.Utils;

namespace SRATS.HErlang
{
	public class HErlangDist : Stat.SDist
	{
		int m;
		GetParamVec alphaFunc;
		GetParamIntVec shapeFunc;
		GetParamVec rateFunc;

		private double[] Alpha
		{
			get
			{
				return alphaFunc();
			}
		}

		private int[] Shape
		{
			get
			{
				return shapeFunc();
			}
		}

		private double[] Rate
		{
			get
			{
				return rateFunc();
			}
		}

		public HErlangDist(int m, GetParamVec alphaFunc, GetParamIntVec shapeFunc, GetParamVec rateFunc)
		{
			this.m = m;
			this.alphaFunc = alphaFunc;
			this.shapeFunc = shapeFunc;
			this.rateFunc = rateFunc;
		}

		public double Pdf(int i, double x)
		{
			double y = Rate[i] * x;
			return Rate[i] * NMath.Pow(y, Shape[i] - 1) * NMath.Exp(-y - NMath.Lfact(Shape[i] - 1));
		}

		public double Cdf(int i, double x)
		{
			double y = Rate[i] * x;
			return NMath.P_gamma(Shape[i], y, NMath.Lfact(Shape[i] - 1));
		}

		public double Ccdf(int i, double x)
		{
			double y = Rate[i] * x;
			return NMath.Q_gamma(Shape[i], y, NMath.Lfact(Shape[i] - 1));
		}

		public override double Ccdf(double x)
		{
			double sum = 0.0;
			for (int i = 0; i < m; i++)
			{
				sum += Alpha[i] * Ccdf(i, x);
			}
			return sum;
		}

		public override double Cdf(double x)
		{
			double sum = 0.0;
			for (int i = 0; i < m; i++)
			{
				sum += Alpha[i] * Cdf(i, x);
			}
			return sum;
		}

		public override double Pdf(double x)
		{
			double sum = 0.0;
			for (int i = 0; i < m; i++)
			{
				sum += Alpha[i] * Pdf(i, x);
			}
			return sum;
		}

		public override double Mean()
		{
			double sum = 0.0;
			for (int i = 0; i < m; i++)
			{
				sum += Alpha[i] * Shape[i] / Rate[i];
			}
			return sum;
		}

		public override double Variance()
		{
			double me = Mean();
			double sum = 0.0;
			for (int i = 0; i < m; i++)
			{
				sum += Alpha[i] * (Shape[i] + Shape[i] * Shape[i]) / (Rate[i] * Rate[i]);
			}
			return sum - me * me;
		}
	}
}
