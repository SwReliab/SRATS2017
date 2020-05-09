---
layout: default
title: 設計概要
---

## 設計概要

SRATS2017ではC#によってモデル推定部分が再設計されています．

[https://github.com/SwReliab/SRATS2017/tree/master/SRATS2017](https://github.com/SwReliab/SRATS2017/tree/master/SRATS2017)がモデル推定（および評価尺度計算）モジュールになります．推定モジュールの概要を以下に記載します．

- CPH: 標準形位相型モデルの推定および計算に関するモジュール．計算ではマルコル連鎖の一様化を用いている．
- Commons: 全てのモデルに共通するモジュール．トップレベルでの抽象クラスなど
- Data: バグデータ，グラフ描画データなどのデータに対するモジュール
- HErlang: 超アーラン形位相型モデルの推定および計算に関するモジュール
- Original: SRATS2010で扱っていた各種モデルの推定および計算に関するモジュール
- Properties: VisualStudioによる自動生成
- Utils: ベクトル演算，数値計算（特殊関数）などの汎用的な共通モジュール

C#からの直接利用する場合の例は以下の `SRATS2017/TestProgram.cs` を参考にしてください．

```c#
using System;

namespace SRATS
{
	class MainClass
	{
		public static void Test_srm()
		{
			double[] time = { 2, 1, 2, 3, 2, 1 };
			int[] num = { 0, 1, 1, 0, 2, 0 };
			int[] type = { 0, 0, 0, 0, 0, 0 };
			SRMData data = new SRMData();
			data.SetData(time, num, type);

			SRM[] models = new SRM[11];
			models[0] = SRMFactory.GetInstance().CreateSRM(OriginalSRMModel.EXP);
			models[1] = SRMFactory.GetInstance().CreateSRM(OriginalSRMModel.GAMMA);
			models[2] = SRMFactory.GetInstance().CreateSRM(OriginalSRMModel.PARETO);
			models[3] = SRMFactory.GetInstance().CreateSRM(OriginalSRMModel.TNORM);
			models[4] = SRMFactory.GetInstance().CreateSRM(OriginalSRMModel.LNORM);
			models[5] = SRMFactory.GetInstance().CreateSRM(OriginalSRMModel.TLOGIS);
			models[6] = SRMFactory.GetInstance().CreateSRM(OriginalSRMModel.LLOGIS);
			models[7] = SRMFactory.GetInstance().CreateSRM(OriginalSRMModel.TXVMAX);
			models[8] = SRMFactory.GetInstance().CreateSRM(OriginalSRMModel.TXVMIN);
			models[9] = SRMFactory.GetInstance().CreateSRM(OriginalSRMModel.LXVMAX);
			models[10] = SRMFactory.GetInstance().CreateSRM(OriginalSRMModel.LXVMIN);

			EMConf[] emconf = new EMConf[11];
			EM[] em = new EM[11];
			for (int i = 0; i < 11; i++)
			{
				emconf[i] = new EMConf();
				em[i] = new EM(models[i], emconf[i], new MODELS.SRMConsoleMessage(models[i], emconf[i]));
			}

			for (int i = 0; i < 11; i++)
			{
				emconf[i].Progress = 1;
				emconf[i].StopCond = StopCondition.PARAMETER;
				em[i].Initialize(data);
				em[i].Fit(data);
				Console.WriteLine(emconf[i].Status);
				Result result = models[i].Calc(data);
				print_result(result);
			}
		}

		public static void Test_cph1()
		{
			double[] alpha = { 1 };
			double[] rate = { 100.0 };
			CPH.CPHParam p = new CPH.CPHParam(1, 1.0, alpha, rate);
			CPH.CPHDist dist = new CPH.CPHDist(1, p.GetAlpha, p.GetRate, p.GetLambda, p.GetScaledRate, 1.0e-8);

			double x = 0.0;
			for (int i = 0; i < 100; i++)
			{
				Console.WriteLine(dist.Pdf(x));
				x += 0.01;
			}
		}

		public static void Test_cph()
		{
			double[] alpha = { 1, 0 };
			double[] rate = { 100.0, 100.0 };
			CPH.CPHParam p = new CPH.CPHParam(2, 1.0, alpha, rate);
			CPH.CPHDist dist = new CPH.CPHDist(2, p.GetAlpha, p.GetRate, p.GetLambda, p.GetScaledRate, 1.0e-8);

			double x = 0.0;
			for (int i = 0; i < 100; i++)
			{
				Console.WriteLine(dist.Pdf(x));
				x += 0.01;
			}
		}

		public static void Test_cphem()
		{
			double[] time = { 2, 1, 2, 3, 2, 1 };
			int[] num = { 0, 1, 1, 0, 2, 0 };
			int[] type = { 0, 0, 0, 0, 0, 0 };
			SRMData data = new SRMData();
			data.SetData(time, num, type);

			int nm = 4;
			SRM[] models = new SRM[nm];
			models[0] = new CPH.CPHEMSRM(new CPH.CPHParam(1));
			models[1] = new CPH.CPHEMSRM(new CPH.CPHParam(2));
			models[2] = new CPH.CPHEMSRM(new CPH.CPHParam(3));
			models[3] = new CPH.CPHEMSRM(new CPH.CPHParam(4));

			EMConf[] emconf = new EMConf[nm];
			EM[] em = new EM[nm];
			for (int i = 0; i < nm; i++)
			{
				emconf[i] = new EMConf();
				em[i] = new EM(models[i], emconf[i], new MODELS.SRMConsoleMessage(models[i], emconf[i]));
			}

			for (int i = 0; i < nm; i++)
			{
				emconf[i].Progress = 1;
				emconf[i].StopCond = StopCondition.LLF;
				em[i].Initialize(data);
				em[i].Fit(data);
				Console.WriteLine(emconf[i].Status);
				Result result = models[i].Calc(data);
				print_result(result);
			}
		}

		public static void Test_herl()
		{
			double[] time = { 2, 1, 2, 3, 2, 1 };
			int[] num = { 0, 1, 1, 0, 2, 0 };
			int[] type = { 0, 0, 0, 0, 0, 0 };
			SRMData data = new SRMData();
			data.SetData(time, num, type);

			int nm = 4;
			SRM[] models = new SRMBase[nm];
			models[0] = new HErlang.HErlangEMSRM(new HErlang.HErlangParam(new int[] { 1, 1 }));
			models[1] = new HErlang.HErlangEMSRM(new HErlang.HErlangParam(new int[] { 1, 2 }));
			models[2] = new HErlang.HErlangEMSRM(new HErlang.HErlangParam(new int[] { 1, 1, 1 }));
			models[3] = new HErlang.HErlangEMSRM(new HErlang.HErlangParam(new int[] { 1, 2, 5 }));

			EMConf[] emconf = new EMConf[nm];
			EM[] em = new EM[nm];
			for (int i = 0; i < nm; i++)
			{
				emconf[i] = new EMConf();
				em[i] = new EM(models[i], emconf[i], new MODELS.SRMConsoleMessage(models[i], emconf[i]));
			}

			for (int i = 0; i < nm; i++)
			{
				emconf[i].Progress = 1;
				emconf[i].StopCond = StopCondition.LLF;
				em[i].Initialize(data);
				em[i].Fit(data);
				Console.WriteLine(emconf[i].Status);
				Result result = models[i].Calc(data);
				print_result(result);
			}
		}

		public static void Test_herlall()
		{
			double[] time = { 2, 1, 2, 3, 2, 1 };
			int[] num = { 0, 1, 1, 0, 2, 0 };
			int[] type = { 0, 0, 0, 0, 0, 0 };
			SRMData data = new SRMData();
			data.SetData(time, num, type);

			EMConf emconf = new EMConf();
			emconf.Progress = 10000;
			emconf.MaxIter = 1000000;
			emconf.StopCond = StopCondition.PARAMETER; //.LLF;

			HErlang.HErlangAll all = new HErlang.HErlangAll(5);
			all.FitAll(data, emconf, null);
			Console.WriteLine(all.GetModelName());
			Console.WriteLine(all.GetParam().ToString());
			Console.WriteLine();

			Result result = all.Calc(data);
			print_result(result);
		}

		public static void print_result(Result result)
		{
			Console.WriteLine("result");
			Console.WriteLine("llf=" + result.Llf);
			Console.WriteLine("AIC=" + result.Aic);
			Console.WriteLine("BIC=" + result.Bic);
			Console.WriteLine("MSE1=" + result.CMse);
			Console.WriteLine("MSE2=" + result.CMsee);
			Console.WriteLine("MSE3=" + result.DMse);
			Console.WriteLine("MSE4=" + result.DMsee);
			Console.WriteLine("TOTAL=" + result.Total);
			Console.WriteLine("RESIDUAL=" + result.Residual);
			Console.WriteLine("FFP=" + result.Ffp);
			Console.WriteLine("CMTTF=" + result.CMttf);
			Console.WriteLine("IMTTF=" + result.IMttf);
			Console.WriteLine("ConMTTF=" + result.ConMttf);
			Console.WriteLine("MEDIAN=" + result.Median);
			Console.WriteLine("BeXLife=" + result.Bexlife);
			Console.WriteLine("ConMEDIAN=" + result.ConMedian);
			Console.WriteLine("ConBeXLife=" + result.ConBexlife);
		}

		public static void Main(string[] args)
		{
			Test_srm();
			//test_cph1();
			//test_cph();
			Test_cphem();
			//test_herl();
			//test_herlall();
		}
	}
}
```