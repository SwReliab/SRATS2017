using SRATS.Utils;

namespace SRATS.CPH
{
	public class CPHUniformization
	{
		GetParamVec scaledRateFunc;
		GetParam lambdaFunc;

		private double Lambda
		{
			get { return lambdaFunc(); }
		}

		private double[] ScaledRate
		{
			get { return scaledRateFunc(); }
		}

		// rate
		int ndim;
		//double maxt0, maxt;
		double[] tmp;
		double[] xi;
//		double[] scaledRate;
		//double[][] vc;
		//double[] prob;
		//double lambda;

		public CPHUniformization(int ndim, GetParam lambdaFunc, GetParamVec scaledRateFunc)
		{
			this.ndim = ndim;
			tmp = new double[ndim];
			xi = new double[ndim];
			this.lambdaFunc = lambdaFunc;
			this.scaledRateFunc = scaledRateFunc;
		}

		//public void setRate(double[] rate)
		//{
		//	lambda = NMath.max(rate) * factor;
		//	Blas.dcopy(ndim, rate, scaledRate);
		//	Blas.dscal(ndim, 1.0 / lambda, scaledRate);
		//	setMaxT(maxt0);
		//}

		//private int setMaxT(double t)
		//{
		//	System.Console.WriteLine("set Max t: " + t);
		//	maxt = t;
		//	int right = PoiDist.getRightBound(lambda * maxt, epsi);
		//	prob = new double[right + 2];
		//	vc = new double[right + 2][];
		//	for (int i = 0; i < vc.Length; i++)
		//	{
		//		vc[i] = new double[ndim];
		//	}
		//	return right;
		//}

		// private vector matrix operations
		private void DgemvNoTrans(double a, double[] x, double b, double[] y)
		{
			for (int i = 0; i < ndim - 1; i++)
			{
				y[i] = b * y[i] + (1.0 - ScaledRate[i]) * x[i] + ScaledRate[i] * x[i + 1];
			}
			y[ndim - 1] = b * y[ndim - 1] + (1.0 - ScaledRate[ndim - 1]) * x[ndim - 1];
		}

		private void DgemvTrans(double a, double[] x, double b, double[] y)
		{
			y[0] = b * y[0] + (1.0 - ScaledRate[0]) * x[0];
			for (int i = 1; i < ndim; i++)
			{
				y[i] = b * y[i] + ScaledRate[i - 1] * x[i - 1] + (1.0 - ScaledRate[i]) * x[i];
			}
		}

		private void Dger(double a, double[] x, double[] y, double[] h)
		{
			for (int i = 0; i < ndim - 1; i++)
			{
				h[2 * i] += a * x[i] * y[i];
				h[2 * i + 1] += a * x[i] * y[i + 1];
			}
			h[2 * (ndim - 1)] += a * x[ndim - 1] * y[ndim - 1];
			h[2 * (ndim - 1) + 1] += 0.0;
		}

		// markov operation
		public void DoBackward(double t, double[] x, int right, double[] prob)
		{
			//int right;
			//if (t > maxt)
			//{
			//	right = setMaxT(t);
			//}
			//else
			//{
			//	right = PoiDist.getRightBound(lambda * t, epsi);
			//}
			double weight = PoiDist.CompProb(Lambda * t, 0, right, prob);

			Blas.Dcopy(ndim, x, xi);
			Blas.Fill(ndim, x, 0.0);
			Blas.Daxpy(ndim, prob[0], xi, x);
			for (int l = 1; l <= right; l++)
			{
				Blas.Dcopy(ndim, xi, tmp);
				DgemvNoTrans(1.0, tmp, 0.0, xi);
				Blas.Daxpy(ndim, prob[l], xi, x);
			}
			Blas.Dscal(ndim, 1.0 / weight, x);
		}

		public void DoForward(double t, double[] x, int right, double[] prob)
		{
			//int right;
			//if (t > maxt)
			//{
			//	right = setMaxT(t);
			//}
			//else
			//{
			//	right = PoiDist.getRightBound(lambda * t, epsi);
			//}
			double weight = PoiDist.CompProb(Lambda * t, 0, right, prob);

			Blas.Dcopy(ndim, x, xi);
			Blas.Fill(ndim, x, 0.0);
			Blas.Daxpy(ndim, prob[0], xi, x);
			for (int l = 1; l <= right; l++)
			{
				Blas.Dcopy(ndim, xi, tmp);
				DgemvTrans(1.0, tmp, 0.0, xi);
				Blas.Daxpy(ndim, prob[l], xi, x);
			}
			Blas.Dscal(ndim, 1.0 / weight, x);
		}

		public void DoSojournForward(double t, double[] f, double[] b, double[] h, int right, double[] prob, double[][] vc)
		{
			//int right;
			//if (t > maxt)
			//{
			//	right = setMaxT(t);
			//}
			//else
			//{
			//	right = PoiDist.getRightBound(lambda * t, epsi);
			//}
			double weight = PoiDist.CompProb(Lambda * t, 0, right + 1, prob);

			// forward and backward
			Blas.Fill(ndim, vc[right + 1], 0.0);
			Blas.Daxpy(ndim, prob[right + 1], b, vc[right + 1]);
			for (int l = right; l >= 1; l--)
			{
				DgemvNoTrans(1.0, vc[l + 1], 0.0, vc[l]);
				Blas.Daxpy(ndim, prob[l], b, vc[l]);
			}
			Blas.Dcopy(ndim, f, xi);
			Blas.Fill(ndim, f, 0.0);
			Blas.Daxpy(ndim, prob[0], xi, f);
			Blas.Fill(ndim * 2, h, 0.0);
			Dger(1.0, xi, vc[1], h);
			for (int l = 1; l <= right; l++)
			{
				Blas.Dcopy(ndim, xi, tmp);
				DgemvTrans(1.0, tmp, 0.0, xi);
				Blas.Daxpy(ndim, prob[l], xi, f);
				Dger(1.0, xi, vc[l + 1], h);
			}
			Blas.Dscal(ndim, 1.0 / weight, f);
			Blas.Dscal(ndim * 2, 1.0 / Lambda / weight, h);
		}
	}
}
