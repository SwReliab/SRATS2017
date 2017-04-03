using System;
using SRATS.Utils;

namespace SRATS
{
	public enum StopCondition
	{
		LLF = 0,
		PARAMETER = 1
	}

	public enum Status
	{
		PROCESSING = 0,
		CONVERGENCE = 1,
		MAXITERATION = 2,
		NUMERICALERROR = 3
	}

	public class EMConf
	{
		public double Atol { get; set; }
		public double Rtol { get; set; }
		public int MaxIter { get; set; }
		public bool PrintFlag { get; set; }
		public int Progress { get; set; }
		public StopCondition StopCond { get; set; }
		public int Cnt { get; set; }
		public double Aerror { get; set; }
		public double Rerror { get; set; }
		public Status Status { get; set; }
		public bool InitFlag { get; set; }
		public double Llf { get; set; }

		public EMConf()
		{
			Initialize();
		}

		public void Set(EMConf conf)
		{
			Atol = conf.Atol;
			Rtol = conf.Rtol;
			MaxIter = conf.MaxIter;
			PrintFlag = conf.PrintFlag;
			Progress = conf.Progress;
			StopCond = conf.StopCond;
			Cnt = conf.Cnt;
			Aerror = conf.Aerror;
			Rerror = conf.Rerror;
			Status = conf.Status;
			InitFlag = conf.InitFlag;
			Llf = conf.Llf;
		}

		public void Initialize()
		{
			Atol = 1.0e-3;
			Rtol = 1.0e-6;
			MaxIter = 2000;
			PrintFlag = true;
			Progress = 50;
			StopCond = StopCondition.LLF;
			Cnt = 0;
			Aerror = 0.0;
			Rerror = 0.0;
			Status = Status.PROCESSING;
			InitFlag = true;
		}

	}

	public interface IEMbase
	{
		IModelParam GetParam();
		void Initialize(SRMData data);
		double Emstep(SRMData data);
		void Pre_em(SRMData data);
		void Post_em(SRMData data);
	}

	public class EM
	{
		IEMbase model;
		IModelParam prev_param;
		IMessage msg;
		EMConf emparam;

		//public EM(EMbase model, EMConf emparam, Message msg)
		//{
		//	this.model = model;
		//	this.msg = msg;
		//	this.emparam = emparam;
		//	init();
		//}

		public EM(SRM model, EMConf emparam, IMessage msg)
		{
			this.model = model as IEMbase;
			this.msg = msg;
			this.emparam = emparam;
			Init();
		}

		private void Init()
		{
			prev_param = model.GetParam().Create();
		}

		public void Initialize(SRMData data)
		{
			model.Initialize(data);
		}

		public void Fit(SRMData data)
		{
			emparam.Status = Status.PROCESSING;
			//if (emparam.initflag)
			//{
			//	initialize(data);
			//}

			double prev_llf;
			emparam.Llf = -double.MaxValue;

			model.Pre_em(data);
			emparam.Cnt = 0;
			while (true)
			{
				prev_llf = emparam.Llf;
				model.GetParam().CopyTo(prev_param);
				emparam.Llf = model.Emstep(data);

				if (emparam.Llf < prev_llf)
				{
					msg.Warning();
				}

				switch (emparam.StopCond)
				{
					case StopCondition.LLF:
						emparam.Aerror = NMath.Abs(emparam.Llf - prev_llf);
						emparam.Rerror = emparam.Aerror / NMath.Abs(prev_llf);
						break;
					case StopCondition.PARAMETER:
						emparam.Aerror = model.GetParam().Adiff(prev_param);
						emparam.Rerror = model.GetParam().Rdiff(prev_param);
						break;
				}

				emparam.Cnt++;

				if (double.IsNaN(emparam.Llf))
				{
					emparam.Status = Status.NUMERICALERROR;
					break;
				}

				if (emparam.PrintFlag && emparam.Cnt % emparam.Progress == 0)
				{
					msg.Show();
				}

				if (emparam.Aerror < emparam.Atol && emparam.Rerror < emparam.Rtol)
				{
					emparam.Status = Status.CONVERGENCE;
					break;
				}

				if (emparam.Cnt >= emparam.MaxIter)
				{
					emparam.Status = Status.MAXITERATION;
					break;
				}
			}
			msg.Final();
			model.Post_em(data);
		}
	}
}
