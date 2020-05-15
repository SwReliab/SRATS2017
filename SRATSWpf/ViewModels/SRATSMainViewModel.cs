using SRATS;
using SRATS.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SRATS2017AddIn.Models;
using SRATS2017AddIn.Commons;
using System.Windows;

namespace SRATS2017AddIn.ViewModels
{
    public delegate void UpdateEvent();

    public class SRATSMainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private List<SRMModel> models;
        private DataModel data;

        private UpdateEvent ulist;
        private UpdateEvent dlist;

        public DataModel Data
        {
            get
            {
                return data;
            }

        }

        public bool Cumulative
        {
            get
            {
                return data.Cumulative;
            }

            set
            {
                data.Cumulative = value;
                SetData();
            }
        }

        public bool TimeInterval
        {
            get
            {
                return data.TimeInterval;
            }

            set
            {
                data.TimeInterval = value;
                SetData();
            }
        }

        public List<SRMModel> Models { get; private set; }

        public SRMList SRMList { get; set; }

        public void SetData()
        {
            if (data.DataReaded)
            {
                return;
            }
            try
            {
                data.SetData();
                foreach (SRMModel model in models)
                {
                    model.Refresh();
                }
                NotifyData();
                NotifyModel();
            }
            catch
            {
                MessageBox.Show("Invalid Data");
                throw new InvalidDataRange();
            }
        }

        public ObservableCollection<SRMModel> ModelList
        {
            get
            {
                return new ObservableCollection<SRMModel>(models);
            }
        }

        public RelayCommand EstimateCommand { get; set; }
        public RelayCommand InitializeCommand { get; set; }

        private void Estimate()
        {
            try
            {
                SetData();
                foreach (SRMModel model in models)
                {
                    model.Fit(data.SRMData);
                    NotifyModel();
                }
            }
            catch { }
        }

        private bool CanEstimate()
        {
            return true;
        }

        private void Initialize()
        {
            try
            {
                SetData();
                foreach (SRMModel model in models)
                {
                    model.Initialize(data.SRMData);
                    NotifyModel();
                }
            }
            catch { }
        }

        public SRATSMainViewModel(string range)
        {
            EstimateCommand = new RelayCommand(Estimate, CanEstimate);
            InitializeCommand = new RelayCommand(Initialize, CanEstimate);

            data = new DataModel();
            data.DataRange = range;

            SRMList = new SRMList();
            SRMList.IsBasic = true;
            SRMList.IsCPH = false;
            SRMList.CPHPhase = "2-5";
            SRMList.IsHEr = false;
            SRMList.HErPhase = "2-5";

            ulist = ModelNotifyPropertyChanged;
            dlist = DataNotifyPropertyChanged;
            AddSRM();
        }

        public void AddSRM()
        {
            IMessage msg = new NullMessage();
            SRMFactory factory = SRMFactory.GetInstance();
            models = new List<SRMModel>();

            if (SRMList.IsBasic)
            {
                models.Add(new EMSRMModel(factory.CreateSRM(OriginalSRMModel.EXP), msg));
                models.Add(new EMSRMModel(factory.CreateSRM(OriginalSRMModel.GAMMA), msg));
                models.Add(new EMSRMModel(factory.CreateSRM(OriginalSRMModel.PARETO), msg));
                models.Add(new EMSRMModel(factory.CreateSRM(OriginalSRMModel.TNORM), msg));
                models.Add(new EMSRMModel(factory.CreateSRM(OriginalSRMModel.LNORM), msg));
                models.Add(new EMSRMModel(factory.CreateSRM(OriginalSRMModel.TLOGIS), msg));
                models.Add(new EMSRMModel(factory.CreateSRM(OriginalSRMModel.LLOGIS), msg));
                models.Add(new EMSRMModel(factory.CreateSRM(OriginalSRMModel.TXVMAX), msg));
                models.Add(new EMSRMModel(factory.CreateSRM(OriginalSRMModel.TXVMIN), msg));
                models.Add(new EMSRMModel(factory.CreateSRM(OriginalSRMModel.LXVMAX), msg));
                models.Add(new EMSRMModel(factory.CreateSRM(OriginalSRMModel.LXVMIN), msg));
            }

            if (SRMList.IsCPH)
            {
                foreach (int k in SRMList.CPH)
                {
                    models.Add(new EMSRMModel(factory.CreateCPHSRM(k), msg));
                }
            }

            if (SRMList.IsHEr)
            {
                foreach (int k in SRMList.HEr)
                {
                    models.Add(new HErSRMModel(factory.CreateHErSRM(k), msg));
                }
            }

            NotifyModel();
        }

        // model notify

        private void ModelNotifyPropertyChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ModelList"));
            }
        }

        public void NotifyModel()
        {
            ulist();
        }

        public void AddModelEvent(UpdateEvent func)
        {
            ulist += func;
        }

        public void RemoveModelEvent(UpdateEvent func)
        {
            ulist -= func;
        }

        // data notify

        private void DataNotifyPropertyChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Data"));
                PropertyChanged(this, new PropertyChangedEventArgs("Cumulative"));
                PropertyChanged(this, new PropertyChangedEventArgs("TimeInterval"));
            }
        }

        public void NotifyData()
        {
            dlist();
        }

        public void AddDataEvent(UpdateEvent func)
        {
            dlist += func;
        }

        public void RemoveDataEvent(UpdateEvent func)
        {
            dlist -= func;
        }
    }
}
