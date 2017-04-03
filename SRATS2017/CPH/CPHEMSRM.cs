using System;
using SRATS.Utils;

namespace SRATS.CPH
{
	public static class Array2 {

		public static double[][] Create(int m, int n)
		{
			double[][] tmp = new double[m][];
			for (int i = 0; i < m; i++)
			{
				tmp[i] = new double[n];
			}
			return tmp;
		}
	}

	public class CPHEMSRM : SRMBase, IEMbase
	{
		private CPHParam param;
		private CPHUniformization cuni;

		int ndim;
		double eno;
		double[] eb, eb2, ey;
		double[] tmp, pi2;
		double[] h0, en;

		double barblf;
		double[] blf, blf2;

		double[][] vb, vb2;
//		double[] vf, vf2;

		double epsi;

		public CPHEMSRM(CPHParam param, double epsi = 1.0e-8)
			: base(param.N + "-CanonicalPH SRGM", 2 * param.N, param.GetOmega,
			       new CPHDist(param.N, param.GetAlpha, param.GetRate, param.GetLambda, param.GetScaledRate, epsi))
 		{
			ndim = param.N;
			this.param = param;
			this.epsi = epsi;
			cuni = new CPHUniformization(ndim, param.GetLambda, param.GetScaledRate);

			tmp = new double[ndim];
			h0 = new double[ndim * 2];
			pi2 = new double[ndim];

			//vf = new double[ndim];
			//vf2 = new double[ndim];

			eb = new double[ndim];
			eb2 = new double[ndim];
			en = new double[ndim * 2];
			ey = new double[ndim];
		}

		private void SetPowerStructuredParameter(double scale, double shape, double[] alpha, double[] rate)
		{
			if (ndim == 1)
			{
				alpha[0] = 1.0;
				rate[0] = 1.0 / scale;
				return;
			}
			double tmpvalue, totalvalue, basev;
			double p = NMath.Exp(1.0 / (ndim - 1.0) * NMath.Log(shape));
			totalvalue = tmpvalue = 1.0;
			for (int i = 1; i < ndim; i++)
			{
				tmpvalue *= (i + 1.0) / (i * p);
				totalvalue += tmpvalue;
			}
			basev = totalvalue / (ndim * scale);
			tmpvalue = basev;
			for (int i = 0; i < ndim; i++)
			{
				alpha[i] = 1.0 / ndim;
				rate[i] = tmpvalue;
				tmpvalue *= p;
			}
		}

		private void SetLinearStructuredParameter(double scale, double shape, double[] alpha, double[] rate)
		{
			if (ndim == 1)
			{
				alpha[0] = 1.0;
				rate[0] = 1.0 / scale;
				return;
			}
			double totalvalue, basev;
			double al = (shape - 1.0) / (ndim - 1.0);
			totalvalue = 1.0;
			for (int i = 1; i < ndim; i++)
			{
				totalvalue += (i + 1) / (al * i + 1.0);
			}
			basev = totalvalue / (ndim * scale);
			for (int i = 0; i < ndim; i++)
			{
				alpha[i] = 1.0 / ndim;
				rate[i] = basev * (al * i + 1.0);
			}
		}

		public void Initialize(SRMData data)
		{
			Alloc(data);
			double[] p = { 2.0, 4.0, 8.0, 16.0, 32.0, 64.0 };
			SetPowerStructuredParameter(data, p[0]);
			double maxllf = DoEstimation(data, 5);
			CPHParam maxparam = param.Create() as CPHParam;
			param.CopyTo(maxparam);
			for (int i = 1; i < p.Length; i++)
			{
				SetPowerStructuredParameter(data, p[i]);
				double llf = DoEstimation(data, 5);
				if (maxllf < llf)
				{
					maxllf = llf;
					param.CopyTo(maxparam);
				}
			}
			for (int i = 0; i < p.Length; i++)
			{
				SetLinearStructuredParameter(data, p[i]);
				double llf = DoEstimation(data, 5);
				if (maxllf < llf)
				{
					maxllf = llf;
					param.CopyTo(maxparam);
				}
			}
			maxparam.CopyTo(param);
			CPHDist cph = dist as CPHDist;
			cph.ClearCache();
		}

		private double DoEstimation(SRMData data, int u)
		{
			double llf = 0.0;
			for (int k = 0; k < u; k++)
			{
				llf = Emstep(data);
			}
			return llf;
		}

		private void SetPowerStructuredParameter(SRMData data, double shape)
		{
			double[] alpha = new double[ndim];
			double[] rate = new double[ndim];
			SetPowerStructuredParameter(data.TotalTime, shape, alpha, rate);
			param.Update(data.TotalFaults, alpha, rate);
		}

		private void SetLinearStructuredParameter(SRMData data, double shape)
		{
			double[] alpha = new double[ndim];
			double[] rate = new double[ndim];
			SetLinearStructuredParameter(data.MaxTime / 2.0, shape, alpha, rate);
			param.Update(data.TotalFaults, alpha, rate);
		}

		private void Alloc(SRMData data)
		{
			int dsize = data.Size;

			//vb = new double[dsize + 2][ndim];
			//vb2 = new double[dsize + 2][ndim];
			vb = Array2.Create(dsize + 2, ndim);
			vb2 = Array2.Create(dsize + 2, ndim);

			blf = new double[dsize + 2];
			blf2 = new double[dsize + 2];
		}

		public void Pre_em(SRMData data)
		{
			Alloc(data);
		}

		public void Post_em(SRMData data)
		{
			CPHDist cph = dist as CPHDist;
			cph.ClearCache();
		}

		public double Emstep(SRMData data)
		{
			int dsize = data.Size;
			double[] time = data.Time;
			int[] num = data.Fault;
			int[] type = data.Type;

			int x;
			double t, llf, tmpv;

			int right = PoiDist.GetRightBound(param.Lambda * NMath.Max(time), epsi);
			double[] prob = new double[right + 2];
			double[][] vc = Array2.Create(right + 2, ndim);

			// initialize for estep
			eno = 0.0;
			Blas.Fill(ndim, eb, 0.0);
			Blas.Fill(ndim, eb2, 0.0);
			Blas.Fill(ndim * 2, en, 0.0);

			// backward: compute eb
			Blas.Fill(ndim, vb[0], 1.0);
			Blas.Fill(ndim, vb2[0], 0.0);
			vb2[0][ndim - 1] = param.Rate[ndim - 1];

			llf = 0.0;
			for (int k = 1; k <= dsize; k++)
			{
				t = time[k-1]; // dat.getTime(k);
				x = num[k-1]; // dat.getNumber(k);

				Blas.Dcopy(ndim, vb[k - 1], vb[k]);
				cuni.DoBackward(t, vb[k], right, prob);
				if (x != 0)
				{
					Blas.Dcopy(ndim, vb[k - 1], tmp);
					Blas.Daxpy(ndim, -1.0, vb[k], tmp);
					blf[k] = Blas.Ddot(ndim, param.Alpha, tmp);
					llf += x * NMath.Log(param.Omega * blf[k]) - NMath.Lgamma(x + 1);

					eno += x;
					Blas.Daxpy(ndim, x / blf[k], tmp, eb);
				}
				else
				{
					blf[k] = 1.0; // to avoid NaN
				}

				Blas.Dcopy(ndim, vb2[k - 1], vb2[k]);
				cuni.DoBackward(t, vb2[k], right, prob);
				if (type[k-1] == 1) // (dat.getType(k) == 1)
				{
					blf2[k] = Blas.Ddot(ndim, param.Alpha, vb2[k]);
					llf += NMath.Log(param.Omega * blf2[k]);
					eno += 1.0;
					Blas.Daxpy(ndim, 1.0 / blf2[k], vb2[k], eb2);
				}
			}
			barblf = Blas.Ddot(ndim, param.Alpha, vb[dsize]);
			llf += -param.Omega * (1.0 - barblf);
			Blas.Daxpy(ndim, param.Omega, vb[dsize], eb);

			// compute pi2
			tmpv = 0.0;
			for (int i = 0; i < ndim - 1; i++)
			{
				tmpv += param.Alpha[i];
				pi2[i] = tmpv / param.Rate[i];
			}
			pi2[ndim - 1] = 1.0 / param.Rate[ndim - 1];

			// sojourn:
			Blas.Fill(ndim, tmp, 0.0);
			Blas.Daxpy(ndim, -num[dsize-1] / blf[dsize] + param.Omega, pi2, tmp);
			if (type[dsize-1] == 1)
			{
				Blas.Daxpy(ndim, 1.0 / blf2[dsize], param.Alpha, tmp);
			}
			cuni.DoSojournForward(time[dsize-1], tmp, vb2[dsize - 1], h0, right, prob, vc);
			Blas.Daxpy(ndim * 2, 1.0, h0, en);
			for (int k = dsize - 1; k >= 1; k--)
			{
				Blas.Daxpy(ndim, num[k] / blf[k + 1] - num[k-1] / blf[k], pi2, tmp);
				if (type[k-1] == 1)
				{
					Blas.Daxpy(ndim, 1.0 / blf2[k], param.Alpha, tmp);
				}
				cuni.DoSojournForward(time[k-1], tmp, vb2[k - 1], h0, right, prob, vc);
				Blas.Daxpy(ndim * 2, 1.0, h0, en);
			}

			/* concrete algorithm: M-step */
			for (int i = 0; i < ndim - 1; i++)
			{ // <-- not <=ndim!
				ey[i] = param.Rate[i]
						* (en[2 * i + 1] + eb[i + 1] * pi2[i])
						/ (en[2 * i] + eb[i] * pi2[i]);
			}
			tmpv = en[2 * (ndim - 1)] + eb[ndim - 1] * pi2[ndim - 1];
			double sum = 0.0;
			for (int i = 0; i < ndim; i++)
			{
				eb[i] = param.Alpha[i] * (eb[i] + eb2[i]);
				sum += eb[i];
			}
			ey[ndim - 1] = sum / tmpv;
			for (int i = 0; i < ndim; i++)
			{
				eb[i] /= sum;
			}
			param.Update(eno + param.Omega * barblf, eb, ey);
			return llf;
		}

		public override IModelParam GetParam()
		{
			return param;
		}

	}
}
