using System;
using SRATS.Utils;

namespace SRATS.HErlang
{
	public class HErlangParam : IModelParam
	{
		private int m;
		private double omega;
		private double[] alpha;
		private int[] shape;
		private double[] rate;

		public int N
		{
			get
			{
				int sum = 0;
				for (int i = 0; i < m; i++)
				{
					sum += shape[i];
				}
				return sum;
			}
		}

		public int M
		{
			get
			{
				return m;
			}
		}

		public double Omega
		{
			get
			{
				return omega;
			}

			set
			{
				omega = value;
			}
		}

		public double[] Alpha
		{
			get
			{
				return alpha;
			}
		}

		public int[] Shape
		{
			get
			{
				return shape;
			}
		}

		public double[] Rate
		{
			get
			{
				return rate;
			}
		}

		public HErlangParam(int[] shape) : this(shape.Length, shape) { }

		public HErlangParam(int m, int[] shape)
		{
			this.m = m;
			this.omega = 1.0;
			this.alpha = new double[m];
			this.shape = new int[m];
			Array.Copy(shape, this.shape, m);
			this.rate = new double [m];
		}

		public HErlangParam(int m, double omega, double[] alpha, int[] shape, double[] rate)
		{
			this.m = m;
			this.alpha = new double[m];
			this.shape = new int[m];
			Array.Copy(shape, this.shape, m);
			this.rate = new double[m];
			Blas.Dcopy(m, alpha, this.alpha);
			Blas.Dcopy(m, rate, this.rate);
		}

		public double Adiff(IModelParam param)
		{
			HErlangParam v = param as HErlangParam;
			double o = NMath.Abs(omega - v.omega);
			double a = Blas.Adiff(m, alpha, v.alpha);
			double b = Blas.Adiff(m, rate, v.rate);
			return NMath.Max(o, a, b);
		}

		public double Rdiff(IModelParam param)
		{
			HErlangParam v = param as HErlangParam;
			double o = NMath.Abs(omega - v.omega) / v.omega;
			double a = Blas.Rdiff(m, alpha, v.alpha);
			double b = Blas.Rdiff(m, rate, v.rate);
			return NMath.Max(o, a, b);
		}

		public void CopyTo(IModelParam p)
		{
			HErlangParam v = p as HErlangParam;
			if (m == v.m)
			{
				v.omega = omega;
				Blas.Dcopy(m, alpha, v.alpha);
				Array.Copy(shape, v.shape, m);
				Blas.Dcopy(m, rate, v.rate);
			}
			else
			{
				throw new InvalidCastException();
			}
		}

		public IModelParam Create()
		{
			return new HErlangParam(m, 1, new double[m], new int[m], new double[m]);
		}

		//public void print()
		//{
		//	Console.Write(omega);
		//	Console.Write(" | ");
		//	Console.Write(Blas.intToString(shape));
		//	Console.Write(" | ");
		//	Console.Write(Blas.doubleToString(alpha));
		//	Console.Write(" | ");
		//	Console.Write(Blas.doubleToString(rate));
		//}

		public string ToString(params string[] format)
		{
			string fomega;
			string falpha;
			string fshape;
			string frate;

			if (format.Length == 0)
			{
				fomega = "G";
				falpha = "G";
				fshape = "G";
				frate = "G";
			}
			else if (format.Length == 1)
			{
				fomega = format[0];
				falpha = format[0];
				fshape = format[0];
				frate = format[0];
			}
			else if (format.Length == 4)
			{
				fomega = format[0];
				falpha = format[1];
				fshape = format[2];
				frate = format[3];
			}
			else
			{
				throw new FormatException();
			}
				
			string[] s = new string[4];
			s[0] = omega.ToString(fomega);
			s[1] = Blas.ArrayToString(alpha, falpha, ",");
			s[2] = Blas.ArrayToString(shape, fshape, ",");
			s[3] = Blas.ArrayToString(rate, frate, ",");
			return string.Join(" | ", s);
		}

		public double GetOmega()
		{
			return Omega;
		}

		public int GetM()
		{
			return m;
		}

		public double[] GetAlpha()
		{
			return Alpha;
		}

		public int[] GetShape()
		{
			return Shape;
		}

		public double[] GetRate()
		{
			return Rate;
		}

		public double[] ToArray()
		{
			int k, i;
			double[] tmp = new double[3 * m + 1];
			tmp[0] = omega;
			i = 1;
			for (k = 0; k < m; k++, i++)
			{
				tmp[i] = alpha[k];
			}
			for (k = 0; k < m; k++, i++)
			{
				tmp[i] = shape[k];
			}
			for (k = 0; k < m; k++, i++)
			{
				tmp[i] = rate[k];
			}
			return tmp;
		}
	}
}
