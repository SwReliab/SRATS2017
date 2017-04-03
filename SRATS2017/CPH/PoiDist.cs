using System;
using SRATS.Utils;

namespace SRATS.CPH
{
	public static class PoiDist
	{
		static double log2piOver2 = 0.9189385332046727417803297364056176398;

		private static double GetNormalQuantile(double eps)
		{
			double leps = NMath.Log(eps);
			if (leps > -6.6 || leps < -689.0)
			{
				throw new InvalidOperationException();
			}
			double l = 3.0;
			double u = 37.0;
			double m = (l + u) / 2.0;
			double fm = GetApproximateNormalTail(m) - leps;
			while (NMath.Abs(fm) > 1.0e-8)
			{
				if (fm > 0)
				{
					l = m;
				}
				else
				{
					u = m;
				}
				m = (l + u) / 2.0;
				fm = GetApproximateNormalTail(m) - leps;
			}
			return m;
		}

		private static double GetApproximateNormalTail(double x)
		{
			double x2 = x * x;
			double tmp = x;
			double sum;
			sum = 1.0 / tmp;
			tmp *= x2;
			sum += -1.0 / tmp;
			tmp *= x2;
			sum += 3.0 / tmp;
			tmp *= x2;
			sum += -15.0 / tmp;
			tmp *= x2;
			sum += 105.0 / tmp;
			return NMath.Log(sum) - x2 / 2.0 - log2piOver2;
		}

		public static int GetRightBound(double lambda, double epsi)
		{
			int right;
			if (lambda < 3.0)
			{
				int cnt = 0;
				double ll = NMath.Exp(-lambda);
				double total = ll;
				do
				{
					cnt++;
					ll *= lambda / cnt;
					total += ll;
				} while (total + epsi < 1.0);
				right = cnt;
			}
			else
			{
				double z = GetNormalQuantile(epsi);
				right = (int)((z + NMath.Sqrt(4.0 * lambda - 1.0))
						* (z + NMath.Sqrt(4.0 * lambda - 1.0)) / 4.0) + 1;
			}
			return right;
		}

		/*
		public int initializeLeftBound()
		{
			if (lambda <= 30.0)
			{
				left = 0;
			}
			else
			{
				left = (int)(((-z + NMath.sqrt(4.0 * lambda - 1.0))
						* (-z + NMath.sqrt(4.0 * lambda - 1.0)) / 4.0));
				left = (left <= 10) ? 0 : (left - 10);
			}
			return left;
		}
		*/

		public static double CompProb(double lambda, int left, int right, double[] prob)
		{
			if (lambda == 0.0)
			{
				prob[0 - left] = 1.0;
				for (int x = 1; x <= right; x++)
				{
					prob[x - left] = 0.0;
				}
				return 1.0;
			}

			int mode = (int)lambda;

			if (mode >= 1)
			{
				prob[mode - left] = NMath.Exp(-lambda + mode * NMath.Log(lambda)
						- log2piOver2 - (mode + 1.0 / 2.0) * NMath.Log(mode) + mode);
			}
			else
			{
				prob[mode - left] = NMath.Exp(-lambda);
			}

			/* Down */
			for (int j = mode; j > left; j--)
			{
				prob[j - 1 - left] = (j / lambda) * prob[j - left];
			}
			/* Up */
			for (int j = mode; j < right; j++)
			{
				prob[j + 1 - left] = (lambda / (j + 1)) * prob[j - left];
			}

			/* compute W */
			double w = 0.0;
			int s = left;
			int t = right;
			while (s < t)
			{
				if (prob[s - left] <= prob[t - left])
				{
					w += prob[s - left];
					s++;
				}
				else
				{
					w += prob[t - left];
					t--;
				}
			}
			w += prob[s - left];
			return w;
		}

		//public int getLeftBound()
		//{
		//	return left;
		//}

		//public int getRightBound()
		//{
		//	return right;
		//}

		//public double pmf(int i)
		//{
		//	return prob[i - left] / w;
		//}
	}
}
