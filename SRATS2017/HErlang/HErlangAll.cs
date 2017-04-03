using System;
using System.Collections.Generic;
using SRATS.Utils;

namespace SRATS.HErlang
{
	public class HErlangAll : SRM
	{
		int ndim;
		double maxllf;
		HErlangEMSRM maxmodel;
		EMConf maxconf;

		List<HErlangEMSRM> allmodels;

		public HErlangAll(int ndim) : base(ndim + "-HyperErlang SRGM", 2*ndim)
		{
			this.ndim = ndim;
			maxconf = new EMConf();

			allmodels = new List<HErlangEMSRM>();
			int[] list = new int[ndim];
			MakeShape(0, list, 1, ndim);
			maxmodel = allmodels[0];
		}

		public override string GetModelName()
		{
			return base.GetModelName() + " [" + maxmodel.GetModelName() + "]";
		}

		public void InitAll(SRMData data, EMConf emconf, IMessage msg)
		{
			foreach (HErlangEMSRM model in allmodels)
			{
				EM em = new EM(model, emconf, msg);
				em.Initialize(data);
				maxmodel = model;
			}
		}

		public void FitAll(SRMData data, EMConf emconf, IMessage msg)
		{
			maxllf = double.MinValue;
			foreach (HErlangEMSRM model in allmodels)
			{
				EM em = new EM(model, emconf, msg);
				em.Fit(data);
				if (emconf.Llf > maxllf)
				{
					maxllf = emconf.Llf;
					maxmodel = model;
					maxconf.Set(emconf);
				}
			}
			emconf.Set(maxconf);
		}

		private void MakeShape(int m, int[] list, int u, int res)
		{
			if (u > res)
			{
				return;
			}
			for (int i = u; i < res; i++)
			{
				list[m] = i;
				MakeShape(m + 1, list, i, res - i);
			}
			list[m] = res;

			int[] shape = new int[m + 1];
			Array.Copy(list, shape, m + 1);
			HErlangEMSRM model = new HErlangEMSRM(new HErlangParam(shape));
			allmodels.Add(model);
			list[m] = 0;
		}

		public override Result Calc(SRMData data)
		{
			return maxmodel.Calc(data);
		}

		public override IModelParam GetParam()
		{
			return maxmodel.GetParam();
		}

		public override GraphData GraphData(double[] mvftime, double[] relitime)
		{
			return maxmodel.GraphData(mvftime, relitime);
		}

		public override MVFData MvfData(double[] mvftime)
		{
			return maxmodel.MvfData(mvftime);
		}
	}
}
