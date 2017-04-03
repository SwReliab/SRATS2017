using System;
using System.Collections.Generic;
using System.Linq;
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
using SRATS2017AddIn.Models;
using SRATS2017AddIn.ViewModels;
using SRATS2017AddIn.Views;

namespace SRATS2017AddIn.Views
{
    /// <summary>
    /// SRATSSRMView.xaml の相互作用ロジック
    /// </summary>
    public partial class SRATSSRMView : Window
    {
        public SRATSSRMView(SRATSSRMViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SRATSSRMViewModel vm = DataContext as SRATSSRMViewModel;
            vm.CloseWindowCommand.Execute(null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SRATSSRMViewModel vm = DataContext as SRATSSRMViewModel;
            SRATSReportView window = new SRATSReportView(new SRATSReportViewModel(vm.Model, vm.Data));
            window.Owner = this;
            window.ShowDialog();
        }
    }
}
