using System.Collections.ObjectModel;

using SRATS;
using SRATS.Utils;
using SRATS2017AddIn.Commons;
using SRATS.HErlang;

namespace SRATS2017AddIn.Models
{
    public class PL
    {
        public string PropertyName { get; set; }
        public object PropertyValue { get; set; }

        public PL(string name, object value)
        {
            PropertyName = name;
            PropertyValue = value;
        }
    }

    public enum ModelStatus
    {
        NotIntialized,
        Initialized,
        Processing,
        Convergence,
        MaxIteration,
        NumericalError,
        Unknown
    }

    public abstract class SRMModel
    {
        protected SRM srm;
        protected EMConf conf;
        protected Result result = null;

        protected ModelStatus status;

        protected MVFData mvf = null;

        public SRMModel(SRM srm, IMessage msg)
        {
            this.srm = srm;
            conf = new EMConf();
            conf.StopCond = StopCondition.PARAMETER;
            status = ModelStatus.NotIntialized;
        }

        public void Refresh()
        {
            status = ModelStatus.NotIntialized;
        }

        public abstract void Fit(SRMData data);
        public abstract void Initialize(SRMData data);

        public ObservableCollection<PL> PropertyList
        {
            get
            {
                ObservableCollection<PL> plist = new ObservableCollection<PL>();
                plist.Add(new PL("Model", Name));
                plist.Add(new PL("Iteration", Iter));
                plist.Add(new PL("Status", Status));
                plist.Add(new PL("LLF", Llf));
                for (int i = 0; i < Param.Length; i++)
                {
                    plist.Add(new PL("Parameter " + i, Param[i]));
                }
                plist.Add(new PL("DF", Df));
                plist.Add(new PL("AIC", Aic));
                plist.Add(new PL("BIC", Bic));
                plist.Add(new PL("MSE", Mse));
                return plist;
            }
        }

        public void PlotMVF(SRMData data, double tmax = 1.5)
        {
            if (data != null)
            {
                mvf = srm.MvfData(PlotPoint.MakeSeq(0, data.TotalTime * tmax));
            }
            else
            {
                mvf = null;
            }
        }

        public ObservableCollection<PlotPoint> MVF
        {
            get
            {
                if (mvf != null)
                {
                    return PlotPoint.makeSeries(mvf.MVFTime, mvf.MVF);
                }
                else
                {
                    return new ObservableCollection<PlotPoint>();
                }
            }
        }

        public SRM SRM
        {
            get
            {
                return srm;
            }
        }

        public double Llf
        {
            get
            {
                if (result != null)
                {
                    return result.Llf;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public int Df
        {
            get { return srm.NParam(); }
        }

        public double Aic
        {
            get
            {
                if (result != null)
                {
                    return result.Aic;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public double Bic
        {
            get
            {
                if (result != null)
                {
                    return result.Bic;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public double Mse
        {
            get
            {
                if (result != null)
                {
                    return result.CMsee;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public ModelStatus Status
        {
            get
            {
                return status;
            }
        }

        public int Iter
        {
            get
            {
                return conf.Cnt;
            }
        }

        public int MaxIter
        {
            get
            {
                return conf.MaxIter;
            }
        }

        public string IterString
        {
            get
            {
                return conf.Cnt.ToString() + "/" + conf.MaxIter.ToString();
            }
        }
        public double Total
        {
            get
            {
                if (result != null)
                {
                    return result.Total;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public double Residual
        {
            get
            {
                if (result != null)
                {
                    return result.Residual;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public double Ffp
        {
            get
            {
                if (result != null)
                {
                    return result.Ffp;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public double CondMttf
        {
            get
            {
                if (result != null)
                {
                    return result.ConMttf;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public double CumMttf
        {
            get
            {
                if (result != null)
                {
                    return result.CMttf;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public double InsMttf
        {
            get
            {
                if (result != null)
                {
                    return result.IMttf;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public double Median
        {
            get
            {
                if (result != null)
                {
                    return result.Median;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public double BeXLife
        {
            get
            {
                if (result != null)
                {
                    return result.Bexlife;
                }
                else
                {
                    return double.NaN;
                }
            }
        }

        public string Name
        {
            get
            {
                return srm.GetModelName();
            }
        }

        public double[] Param
        {
            get
            {
                return srm.GetParam().ToArray();
            }
        }

        public string ParamString
        {
            get
            {
                return srm.GetParam().ToString();
            }
        }
    }

    public class EMSRMModel : SRMModel
    {
        private EM em;

        public EMSRMModel(SRM srm, IMessage msg) : base(srm, msg)
        {
            em = new EM(srm, conf, msg);
        }

        public override void Initialize(SRMData data)
        {
            em.Initialize(data);
            status = ModelStatus.Initialized;
            result = srm.Calc(data);
            PlotMVF(data);
        }

        public override void Fit(SRMData data)
        {
            if (status == ModelStatus.NotIntialized)
            {
                em.Initialize(data);
                status = ModelStatus.Initialized;
            }
            status = ModelStatus.Processing;
            em.Fit(data);
            switch (conf.Status)
            {
                case SRATS.Status.CONVERGENCE:
                    status = ModelStatus.Convergence;
                    break;
                case SRATS.Status.MAXITERATION:
                    status = ModelStatus.MaxIteration;
                    break;
                case SRATS.Status.NUMERICALERROR:
                    status = ModelStatus.NumericalError;
                    break;
                default:
                    status = ModelStatus.Unknown;
                    break;
            }
            result = srm.Calc(data);
            PlotMVF(data);
        }
    }

    public class HErSRMModel : SRMModel
    {
        private IMessage msg;
        private HErlangAll hsrm;

        public HErSRMModel(SRM srm, IMessage msg) : base(srm, msg)
        {
            this.msg = msg;
            hsrm = srm as HErlangAll;
        }

        public override void Initialize(SRMData data)
        {
            hsrm.InitAll(data, conf, msg);
            status = ModelStatus.Initialized;
            result = srm.Calc(data);
            PlotMVF(data);
        }

        public override void Fit(SRMData data)
        {
            if (status == ModelStatus.NotIntialized)
            {
                hsrm.InitAll(data, conf, msg);
                status = ModelStatus.Initialized;
            }
            status = ModelStatus.Processing;
            hsrm.FitAll(data, conf, msg);
            switch (conf.Status)
            {
                case SRATS.Status.CONVERGENCE:
                    status = ModelStatus.Convergence;
                    break;
                case SRATS.Status.MAXITERATION:
                    status = ModelStatus.MaxIteration;
                    break;
                case SRATS.Status.NUMERICALERROR:
                    status = ModelStatus.NumericalError;
                    break;
                default:
                    status = ModelStatus.Unknown;
                    break;
            }
            result = srm.Calc(data);
            PlotMVF(data);
        }
    }
}
