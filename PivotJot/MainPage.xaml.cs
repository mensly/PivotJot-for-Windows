using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PivotJot
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const int PAGE_LIST = 0;
        private const int PAGE_JOT = 1;
        public ObservableCollection<Project> Projects { get; private set; }
        private Project selected;
        public Project Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                titleEntry.IsEnabled = selected != null;
                TextChanged(titleEntry, null);
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            Projects = new ObservableCollection<Project>();
            Projects.Add(new Project(1) { Name = "Test 1" });
            Projects.Add(new Project(2) { Name = "Test 2" });

            this.DataContext = this;

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += (o, args) => args.Handled = OnBackPressed();
            }

        }

        private bool OnBackPressed()
        {
            if (pivotView.SelectedIndex != PAGE_LIST)
            {
                pivotView.SelectedIndex = PAGE_LIST;
                return true;
            }
            else
            {
                return false;
            }
        }

        private async void ProjectSelected(object sender, SelectionChangedEventArgs e)
        {
            Selected = e.AddedItems.FirstOrDefault() as Project;
            if (Selected != null)
            {
                pivotView.SelectedIndex = PAGE_JOT;
                await Task.Delay(250);
                titleEntry.Focus(FocusState.Programmatic);
            }
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            submitBtn.IsEnabled = Selected != null && titleEntry.Text.Length > 0;
        }
    }
}

