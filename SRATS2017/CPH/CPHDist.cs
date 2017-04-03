using System;
using SRATS.Utils;

namespace SRATS.CPH
{
	public class CPHDist : Stat.SDist
	{
		int ndim;
		GetParamVec alphaFunc;
		GetParamVec rateFunc;
		GetParam lambdaFunc;

		double cache_t;
		double[] cache_x;

		int max_right;
		double[] prob;

		double epsi;

		CPHUniformization unif;

		private double Lambda
		{
			get
			{
				return lambdaFunc();
			}
		}

		private double[] Alpha
		{
			get
			{
				return alphaFunc();
			}
		}

		private double[] Rate
		{
			get
			{
				return rateFunc();
			}
		}

		public CPHDist(int ndim, GetParamVec alphaFunc, GetParamVec rateFunc,
		               GetParam lambdaFunc, GetParamVec scaledRateFunc, double epsi)
		{
			this.ndim = ndim;
			this.alphaFunc = alphaFunc;
			this.rateFunc = rateFunc;
			this.lambdaFunc = lambdaFunc;
			this.epsi = epsi;

			cache_x = new double[ndim];
			cache_t = 0.0;
			Blas.Dcopy(ndim, Alpha, cache_x);

			max_right = 10;
			prob = new double[max_right + 1];
			unif = new CPHUniformization(ndim, lambdaFunc, scaledRateFunc);
		}

		private int AllocProb(double t)
		{
			int right = PoiDist.GetRightBound(Lambda * t, epsi);
			if (right > max_right)
			{
				max_right = right;
				prob = new double[right + 1];
			}
			return right;
		}

		public void ClearCache()
		{
//			System.Console.WriteLine("clear cache");
			cache_t = 0.0;
			Blas.Dcopy(ndim, Alpha, cache_x);
		}

		private void CalcProb(double t)
		{
			double dt;
			if (t < cache_t)
			{
				dt = t;
				Blas.Dcopy(ndim, Alpha, cache_x);
			}
			else
			{
				dt = t - cache_t;
			}
			int right = AllocProb(dt);
			unif.DoForward(dt, cache_x, right, prob);
			cache_t = t;
		}

		public override double Ccdf(double x)
		{
			CalcProb(x);
			return Blas.Asum(ndim, cache_x);
		}

		public override double Cdf(double x)
		{
			double tmp = 1.0 - Ccdf(x);
			if (tmp < 0 && tmp > -NMath.ZERO)
			{
				return 0;
			}
			return tmp;
		}

		public override double Pdf(double x)
		{
			CalcProb(x);
			return cache_x[ndim - 1] * Rate[ndim - 1];
		}

		public override double Mean()
		{
			double sum = 0.0;
			double tmpv = 0.0;
			for (int i = 0; i < ndim; i++)
			{
				tmpv += Alpha[i];
				sum += tmpv / Rate[i];
			}
			return sum;
		}

		public override double Variance()
		{
			throw new NotImplementedException();
		}
	}
}
