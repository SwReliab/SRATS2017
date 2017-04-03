using System;
using SRATS.MODELS;

namespace SRATS
{
	public enum OriginalSRMModel
	{
		EXP = 0,
		GAMMA = 1,
		PARETO = 2,
		TNORM = 3,
		LNORM = 4,
		TLOGIS = 5,
		LLOGIS = 6,
		TXVMAX = 7,
		LXVMAX = 8,
		TXVMIN = 9,
		LXVMIN = 10
	}

	public class SRMFactory
	{
		private static SRMFactory factory = new SRMFactory();

		private SRMFactory() { }

		public static SRMFactory GetInstance()
		{
			return factory;
		}

		private OriginalSRMModel GetEnumModel(int i)
		{
			return (OriginalSRMModel)Enum.ToObject(typeof(OriginalSRMModel), i);
		}

		public SRM CreateSRM(OriginalSRMModel model)
		{
			switch (model)
			{
				case OriginalSRMModel.EXP:
					return new ExpEMSRM(new ExpParam());
				case OriginalSRMModel.GAMMA:
					return new GammaEMSRM(new GammaParam());
				case OriginalSRMModel.PARETO:
					return new ParetoEMSRM(new ParetoParam());
				case OriginalSRMModel.TNORM:
					return new TNormEMSRM(new NormalParam());
				case OriginalSRMModel.LNORM:
					return new LNormEMSRM(new NormalParam());
				case OriginalSRMModel.TLOGIS:
					return new TLogisEMSRM(new LogisParam());
				case OriginalSRMModel.LLOGIS:
					return new LLogisEMSRM(new LogisParam());
				case OriginalSRMModel.TXVMAX:
					return new TXvmaxEMSRM(new ExtremeParam());
				case OriginalSRMModel.LXVMAX:
					return new LXvmaxEMSRM(new ExtremeParam());
				case OriginalSRMModel.TXVMIN:
					return new TXvminEMSRM(new ExtremeParam());
				case OriginalSRMModel.LXVMIN:
					return new LXvminEMSRM(new ExtremeParam());
				default:
					throw new InvalidOperationException();
			}
		}

		public SRM CreateSRM(int i)
		{
			return CreateSRM(GetEnumModel(i));
		}

		public SRM CreateCPHSRM(int n)
		{
			return new CPH.CPHEMSRM(new CPH.CPHParam(n));
		}

		public SRM CreateHErSRM(int n)
		{
			return new HErlang.HErlangAll(n);
		}
	}
}
