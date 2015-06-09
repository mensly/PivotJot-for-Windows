using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
        private const string TOKEN_DEBUG = "__debug__";
        private const int PAGE_LIST = 0;
        private const int PAGE_JOT = 1;

        private Project selected;
        private string token = TOKEN_DEBUG;
        private bool loadingList;
        private bool loadingSubmit;

        public ObservableCollection<Project> Projects { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            Projects = new ObservableCollection<Project>();
            // TODO: Reload projects from cache
            LoadingList = Projects.Count == 0;
            LoadProjects();
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

        private async void LoadProjects()
        {
            if (token == TOKEN_DEBUG)
            {
                await Task.Delay(2000);
                Projects.Clear();
                Projects.Add(new Project(1) { Name = "Test 1" });
                Projects.Add(new Project(2) { Name = "Test 2" });
                LoadingList = false;
            }
        }

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

        private void LoadingChanged()
        {
            loadingIndicator.Visibility = (loadingList || loadingSubmit) ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool LoadingList
        {
            get { return loadingList; }
            set { loadingList = value; LoadingChanged(); }
        }
        public bool LoadingSubmit
        {
            get { return loadingSubmit; }
            set { loadingSubmit = value; LoadingChanged(); }
        }

        private async void Submit(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            LoadingSubmit = true;
            if (token == TOKEN_DEBUG)
            {
                await Task.Delay(4000);
            }
            titleEntry.Text = "";
            LoadingSubmit = false;
            IsEnabled = true;
            titleEntry.Focus(FocusState.Programmatic);
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Submit(sender, new RoutedEventArgs());
            }
        }
    }
}

