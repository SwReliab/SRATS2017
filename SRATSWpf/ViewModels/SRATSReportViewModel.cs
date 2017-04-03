using SRATS2017AddIn.Commons;
using SRATS2017AddIn.Models;
using System.Windows;

namespace SRATS2017AddIn.ViewModels
{
    public class SRATSReportViewModel
    {
        public PlotModel Model { get; private set; }

        public RelayCommand ReportCommand { get; set; }

        public SRATSReportViewModel(SRMModel srm, DataModel data)
        {
            Model = new PlotModel(srm, data);
            ReportCommand = new RelayCommand(Report, CanReport);
        }

        private bool CanReport()
        {
            return true;
        }

        private void Report()
        {
            try
            {
                Model.MakeResult();
                IOOperation.GetInstance().MakeReport(Model);
            }
            catch (AlreadyExistSheetName)
            {
                MessageBox.Show("This sheet name has already been used.");
            }
        }
    }
}
