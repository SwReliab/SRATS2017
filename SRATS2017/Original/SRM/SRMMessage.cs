using System;
using SRATS.Utils;

namespace SRATS.MODELS
{
    public class SRMConsoleMessage : DefaultMessage
    {
		private SRM srm;
        private EMConf emparam;

        public SRMConsoleMessage(SRM srm, EMConf emparam)
        {
            this.srm = srm;
            this.emparam = emparam;
        }

        public override void Show()
        {
			Console.Write(srm.GetModelName() + " iter=" + emparam.Cnt + " ");
            Console.Write("(aerror=" + emparam.Aerror + " ");
            Console.Write("rerror=" + emparam.Rerror + "): ");
            Console.Write("llf=" + emparam.Llf + " ");
            Console.Write("param=(");
			Console.Write(srm.GetParam().ToString());
            Console.WriteLine(" )");
        }

        public override void Error()
        {
			base.Error();
            throw new NotFiniteNumberException();
        }

    }
}
