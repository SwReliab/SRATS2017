using System.ComponentModel;
using SRATS2017AddIn.Models;
using SRATS2017AddIn.Commons;
using SRATS2017AddIn.ViewModels;

namespace SRATS2017AddIn.ViewModels
{
    public class SRATSSRMViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private SRATSMainViewModel mainVm;
        private DataModel data;
        private SRMModel model;

        public DataModel Data {
            get
            {
                return data;
            }
        }

        public SRMModel Model
        {
            get
            {
                return model;
            }
        }

        public int XMax { get; set; }
        public int YMax { get; set; }

        public RelayCommand EstimateCommand { get; private set; }
        public RelayCommand InitializeCommand { get; private set; }
        public RelayCommand CloseWindowCommand { get; private set; }

        private void Estimate()
        {
            try
            {
                mainVm.SetData();
                model.Fit(data.SRMData);
                updateXYMax();
                mainVm.NotifyModel();
            } catch { }
        }

        private bool CanEstimate()
        {
            return true;
        }

        private void Initialize()
        {
            try
            {
                mainVm.SetData();
                model.Initialize(data.SRMData);
                updateXYMax();
                mainVm.NotifyModel();
            } catch { }
        }

        public void CloseWindow()
        {
            mainVm.RemoveModelEvent(UpdateModel);
            mainVm.RemoveDataEvent(UpdateData);
        }

        public void UpdateModel()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Model"));
                PropertyChanged(this, new PropertyChangedEventArgs("XMax"));
                PropertyChanged(this, new PropertyChangedEventArgs("YMax"));
            }
        }

        public void UpdateData()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Data"));
            }
        }

        private void updateXYMax()
        {
            if (model.MVF.Count != 0)
            {
                int i = model.MVF.Count;
                XMax = (int)model.MVF[i - 1].X + 1;
                YMax = (int)model.MVF[i - 1].Y + 1;
            }
            else
            {
                XMax = 1;
                YMax = 1;
            }
        }

        public SRATSSRMViewModel(SRMModel model, SRATSMainViewModel mainVm)
        {
            this.model = model;
            this.data = mainVm.Data;
            this.mainVm = mainVm;
            model.PlotMVF(data.SRMData);
            updateXYMax();

            mainVm.AddModelEvent(UpdateModel);
            mainVm.AddDataEvent(UpdateData);

            EstimateCommand = new RelayCommand(Estimate, CanEstimate);
            InitializeCommand = new RelayCommand(Initialize, CanEstimate);
            CloseWindowCommand = new RelayCommand(CloseWindow, CanEstimate);
        }
    }
}
