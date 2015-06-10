using Refit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private const string TOKEN_DEBUG = "__debug__";
        private const int PAGE_LIST = 0;
        private const int PAGE_JOT = 1;

        private IPivotalTrackerApi pivotalApi = RestService.For<IPivotalTrackerApi>("https://www.pivotaltracker.com");
        private Project selected;
        private readonly UserData userData = new UserData();
        private bool loadingList;
        private bool loadingSubmit;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Project> Projects { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            storyType.ItemsSource = Enum.GetValues(typeof(Story.Type)).Cast<Story.Type>();
            storyType.SelectedIndex = 0;
            Projects = new ObservableCollection<Project>();
            var cachedProjects = userData.Projects;
            PopulateProjectList(cachedProjects);
            if (cachedProjects.Count == 0)
            {
                LoadingList = true;
            }
            else
            {
                int selected = userData.SelectedId;
                foreach (var p in cachedProjects)
                {
                    if (p.ProjectId == selected)
                    {
                        projectsList.SelectedItem = p;
                        Selected = p;
                        ShowJotPage();
                        break;
                    }
                }
            }
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

        private void ProjectSelected(object sender, SelectionChangedEventArgs e)
        {
            Selected = e.AddedItems.FirstOrDefault() as Project;
            if (Selected == Project.PLACEHOLDER_LOGOUT)
            {
                projectsList.SelectedItem = null;
                Projects.Clear();
                Selected = null;
                userData.SetToken(null);
                LoadingList = true;
                LoadProjects();
            }
            else if (Selected == Project.PLACEHOLDER_EMPTY)
            {
                projectsList.SelectedItem = null;
            }
            else if (Selected != null)
            {
                ShowJotPage();
            }
        }

        private async void ShowJotPage()
        {
            pivotView.SelectedIndex = PAGE_JOT;
            await Task.Delay(250);
            titleEntry.Focus(FocusState.Programmatic);
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            submitBtn.IsEnabled = Selected != null && titleEntry.Text.Length > 0;
        }

        private async Task<string> GetToken()
        {
            string token = await userData.GetToken();
#if DEBUG
            if (token == null) { return TOKEN_DEBUG; }
            // if (token == null) { token = "REDACTED"; userData.SetToken(token); }
#endif
            return token;
        }

        private async void LoadProjects()
        {
            Projects.Clear();
            string token = await GetToken();
            if (token == null)
            {
                // TODO: Show login UI
            }
            else
            {
                if (token == TOKEN_DEBUG)
                {
                    await Task.Delay(2000);
                    PopulateProjectList(new List<Project>()
                    {
                        new Project(1) { Name = "Test 1" },
                        new Project(2) { Name = "Test 2" },
                        new Project(3) { Name = "Test 3" },
                    });
                }
                else
                {
                    var projects = await pivotalApi.GetProjects(token);
                    userData.Projects = projects;
                    PopulateProjectList(projects);
                }
            }
            LoadingList = false;
        }

        private void PopulateProjectList(ICollection<Project> projects)
        {
            foreach (Project p in projects)
            {
                Projects.Add(p);
            }
            if (Projects.Count == 0)
            {
                Projects.Add(Project.PLACEHOLDER_EMPTY);
            }
            Projects.Add(Project.PLACEHOLDER_LOGOUT);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Project Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                userData.SelectedId = selected.ProjectId;
                titleEntry.IsEnabled = selected != null;
                TextChanged(titleEntry, null);
                NotifyPropertyChanged();
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
            string token = await GetToken();
            if (token == TOKEN_DEBUG)
            {
                await Task.Delay(4000);
            }
            else
            {
                var story = new Story(titleEntry.Text, (Story.Type)storyType.SelectedItem);
                await pivotalApi.PostStory(token, Selected.ProjectId, story);
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

        private async void Login()
        {
            // TODO: UI
            string username = "TODO";
            string password = "TODO";
            string authInfo = username + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
            var user = await pivotalApi.Authorize("Basic " + authInfo);
            // TODO: Handle errors
            userData.SetToken(user.Token);
            LoadingList = true;
            LoadProjects();
        }
    }
}

