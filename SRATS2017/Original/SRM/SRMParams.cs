using System;
using SRATS.Utils;

namespace SRATS.MODELS
{
	abstract public class OriginalParam : IModelParam
	{
		protected readonly int n;
		protected double[] param;

		protected OriginalParam(int n)
		{
			this.n = n;
			param = new double[n];
		}

		public void SetParam(params double[] v)
		{
			if (v.Length == n)
			{
				Array.Copy(v, param, n);
			}
			else
			{
				throw new InvalidCastException();
			}
		}

		public double Adiff(IModelParam other)
		{
			OriginalParam v = other as OriginalParam;
			return Blas.Adiff(n, param, v.param);
		}

		public double Rdiff(IModelParam other)
		{
			OriginalParam v = other as OriginalParam;
			return Blas.Rdiff(n, param, v.param);
		}

		public void CopyTo(IModelParam other)
		{
			OriginalParam v = other as OriginalParam;
			if (n == v.n)
			{
				Blas.Dcopy(n, param, v.param);
			}
			else
			{
				throw new InvalidCastException();
			}
		}

		abstract public IModelParam Create();

		public string ToString(params string[] format)
		{
			if (format.Length == 0)
			{
				return Blas.ArrayToString(param, "G", ",");
			}
			else if (format.Length == 1)
			{
				return Blas.ArrayToString(param, format[0], ",");
			}
			else if (format.Length == param.Length)
			{
				return Blas.ArrayToString(param, format, ",");
			}
			else
			{
				throw new FormatException();
			}
		}

		public double[] ToArray()
		{
			return param;
		}

		//public int size()
		//{
		//	return n;
		//}
	}


}
