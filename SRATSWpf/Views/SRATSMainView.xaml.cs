using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SRATS2017AddIn.Models;
using SRATS2017AddIn.ViewModels;
using System.ComponentModel;

namespace SRATS2017AddIn.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SRATSMainView : Window
    {
        public SRATSMainView(string range)
        {
            InitializeComponent();
            DataContext = new SRATSMainViewModel(range);
            modellist.Items.SortDescriptions.Clear();
        }

        private void ListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem targetItem = sender as ListViewItem;
            SRMModel model = targetItem.DataContext as SRMModel;
            SRATSMainViewModel vm = DataContext as SRATSMainViewModel;
            SRATSSRMView window = new SRATSSRMView(new SRATSSRMViewModel(model, vm));
            window.Owner = this;
            window.Title = model.Name;
            window.Show();
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader header = sender as GridViewColumnHeader;
            string tag = header.Tag as string;
            switch (tag)
            {
                case "Llf":
                case "Df":
                case "Aic":
                case "Bic":
                case "Mse":
                    SortListView(modellist, tag, false);
                    break;
                default:
                    modellist.Items.SortDescriptions.Clear();
                    break;
            }
        }

        private void SortListView(ListView listView, string tag, bool v)
        {
            if (listView == null || listView.Items.Count == 0)
            {
                return;
            }

            var r = listView.Items.SortDescriptions.Where(x => x.PropertyName == tag);
            ListSortDirection sort;
            if (r.Count() == 0)
            {
                sort = ListSortDirection.Ascending;
            }
            else
            {
                sort = r.First().Direction == ListSortDirection.Descending ?
                    ListSortDirection.Ascending : ListSortDirection.Descending;
                listView.Items.SortDescriptions.Remove(r.First());
            }

            if (v == false)
            {
                listView.Items.SortDescriptions.Clear();
            }
            listView.Items.SortDescriptions.Add(new SortDescription(tag, sort));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SRATSMainViewModel vm = DataContext as SRATSMainViewModel;
            SRATSSRMSelectorView window = new SRATSSRMSelectorView(vm);
            window.Owner = this;
            window.ShowDialog();
        }
    }
}
