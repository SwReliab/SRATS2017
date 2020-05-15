using SRATS2017AddIn.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SRATS2017AddIn.Views
{
    /// <summary>
    /// SRATSReportView.xaml の相互作用ロジック
    /// </summary>
    public partial class SRATSReportView : Window
    {
        public SRATSReportView(SRATSReportViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SRATSReportViewModel vm = DataContext as SRATSReportViewModel;
            vm.ReportCommand.Execute(null);
            Close();
        }
    }
}
