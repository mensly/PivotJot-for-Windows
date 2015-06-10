using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PivotJot
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
            var brush = mainGrid.Background as SolidColorBrush;
            if (brush != null)
            {
                var colour = brush.Color;
                colour.A = 0xcc;
                mainGrid.Background = new SolidColorBrush(colour);
            }
            ShowKeyboard();
        }

        private async void ShowKeyboard()
        {
            await Task.Delay(250);
            usernameEntry.Focus(FocusState.Programmatic);
        }

        private async void Login(object sender, RoutedEventArgs e)
        {
            loadingIndicator.Visibility = Visibility.Visible;
            IsEnabled = false;
            MainPage parentPage = this.FindParent<MainPage>();
            string authInfo = usernameEntry.Text + ":" + passwordEntry.Password;
            authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
            try
            {
                var user = await MainPage.PIVOTAL.Authorize("Basic " + authInfo);
                Frame.Visibility = Visibility.Collapsed;
                if (parentPage != null)
                {
                    parentPage.LoginComplete(user.Token);
                }
            }
            catch (ApiException)
            {
                // TODO: Handle incorrect password
            }
            catch (HttpRequestException)
            {
                // TODO: Handle errors
            }
            IsEnabled = true;
            loadingIndicator.Visibility = Visibility.Collapsed;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                if (submitBtn.IsEnabled)
                {
                    Login(sender, e);
                }
                else if (sender != passwordEntry)
                {
                    passwordEntry.Focus(FocusState.Keyboard);
                }
            }
        }


        private void TextChanged(object sender, RoutedEventArgs e)
        {
            submitBtn.IsEnabled = usernameEntry.Text.Length > 0 &&
                passwordEntry.Password.Length > 0;
        }
    }
}
