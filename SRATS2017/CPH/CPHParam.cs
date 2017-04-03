using System;
using System.Globalization;
using SRATS.Utils;

namespace SRATS.CPH
{
	public class CPHParam : IModelParam
	{
		private static double[] DefaultAlpha(int ndim)
		{
			double[] tmp = new double[ndim];
			tmp[0] = 1.0;
			for (int i = 1; i < tmp.Length; i++)
			{
				tmp[i] = 0.0;
			}
			return tmp;
		}

		private static double[] DefaultRate(int ndim)
		{
			double[] tmp = new double[ndim];
			for (int i = 0; i < tmp.Length; i++)
			{
				tmp[i] = 1.0;
			}
			return tmp;
		}

		private int ndim;
		private double omega;
		private double[] alpha;
		private double[] rate;

		private double lambda;
		private double[] scaledRate;
		private double factor;

		public int N
		{
			get
			{
				return ndim;
			}
		}

		public double Omega
		{
			get
			{
				return omega;
			}
		}

		public double[] Alpha
		{
			get
			{
				return alpha;
			}
		}

		public double[] Rate
		{
			get
			{
				return rate;
			}
		}

		public double Lambda
		{
			get
			{
				return lambda;
			}
		}

		public CPHParam(int ndim, double factor = 1.01)
		{
			this.ndim = ndim;
			alpha = new double[ndim];
			rate = new double[ndim];
			scaledRate = new double[ndim];
			this.factor = factor;
			Update(omega, DefaultAlpha(ndim), DefaultRate(ndim));
		}

		public CPHParam(int ndim, double omega, double[] alpha, double[] rate, double factor = 1.01)
		{
			this.ndim = ndim;
			this.alpha = new double[ndim];
			this.rate = new double[ndim];
			scaledRate = new double[ndim];
			this.factor = factor;
			Update(omega, alpha, rate);
		}

		public void Update(double omega, double[] alpha, double[] rate)
		{
			this.omega = omega;
			Blas.Dcopy(ndim, alpha, this.alpha);
			Blas.Dcopy(ndim, rate, this.rate);
			TransformCanonicalForm();
			SetScaledRate();
		}

		private void SetScaledRate()
		{
			lambda = Blas.Amax(ndim, rate) * factor;
			Blas.Dcopy(ndim, rate, scaledRate);
			Blas.Dscal(ndim, 1.0 / lambda, scaledRate);
		}

		public double Adiff(IModelParam param)
		{
			CPHParam v = param as CPHParam;
			double o = NMath.Abs(omega - v.omega);
			double a = Blas.Adiff(ndim, alpha, v.alpha);
			double b = Blas.Adiff(ndim, rate, v.rate);
			return NMath.Max(o, a, b);
		}

		public double Rdiff(IModelParam param)
		{
			CPHParam v = param as CPHParam;
			double o = NMath.Abs(omega - v.omega) / v.omega;
			double a = Blas.Rdiff(ndim, alpha, v.alpha);
			double b = Blas.Rdiff(ndim, rate, v.rate);
			return NMath.Max(o, a, b);
		}

		public void CopyTo(IModelParam p)
		{
			CPHParam v = p as CPHParam;
			if (ndim == v.ndim)
			{
				v.omega = omega;
				v.lambda = lambda;
				Blas.Dcopy(ndim, alpha, v.alpha);
				Blas.Dcopy(ndim, rate, v.rate);
				Blas.Dcopy(ndim, scaledRate, v.scaledRate);
			}
			else
			{
				throw new InvalidCastException();
			}
		}

		public IModelParam Create()
		{
			return new CPHParam(ndim, 1, new double[ndim], new double[ndim]);
		}

		public double GetOmega()
		{
			return Omega;
		}

		public double[] GetAlpha()
		{
			return Alpha;
		}

		public double[] GetRate()
		{
			return Rate;
		}

		public double GetLambda()
		{
			return lambda;
		}

		public double[] GetScaledRate()
		{
			return scaledRate;
		}

		private void TransformCanonicalForm()
		{
			int j;
			for (int i = 0; i < ndim - 1; i++)
			{
				if (rate[i] > rate[i + 1])
				{
					Swap(i, i + 1);
					j = i;
					while (j > 0 && rate[j - 1] > rate[j])
					{
						Swap(j - 1, j);
						j--;
					}
				}
			}
		}

		private void Swap(int i, int j)
		{
			double w, tmp;
			w = rate[j] / rate[i];
			alpha[i] += (1.0 - w) * alpha[j];
			alpha[j] *= w;
			tmp = rate[j];
			rate[j] = rate[i];
			rate[i] = tmp;
		}

		public string ToString(params string[] format)
		{
			string fomega;
			string falpha;
			string frate;

			if (format.Length == 0)
			{
				fomega = "G";
				falpha = "G";
				frate = "G";
			}
			else if (format.Length == 1)
			{
				fomega = format[0];
				falpha = format[0];
				frate = format[0];
			}
			else if (format.Length == 3)
			{
				fomega = format[0];
				falpha = format[1];
				frate = format[2];
			}
			else
			{
				throw new FormatException();
			}

			string[] s = new string[3];
			s[0] = omega.ToString(fomega);
			s[1] = Blas.ArrayToString(alpha, falpha, ",");
			s[2] = Blas.ArrayToString(rate, frate, ",");
			return string.Join(" | ", s);
		}

		public double[] ToArray()
		{
			int k, i;
			double[] tmp = new double[2*ndim+1];
			tmp[0] = omega;
			i = 1;
			for (k = 0; k < ndim; k++, i++)
			{
				tmp[i] = alpha[k];
			}
			for (k = 0; k < ndim; k++, i++)
			{
				tmp[i] = rate[k];
			}
			return tmp;
		}
	}
}
