using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace movie
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(movie.firstage));
            var view = ApplicationView.GetForCurrentView();
            view.TitleBar.BackgroundColor = Color.FromArgb(255, 77, 77, 77);
            view.TitleBar.ButtonBackgroundColor = Color.FromArgb(255, 77, 77, 77);
            view.TitleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 77, 77, 77);
            view.TitleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 66, 66, 66);
            NavigationCacheMode = NavigationCacheMode.Enabled;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size { Width = 450, Height = 480 });
            load();
        }
        private async void load()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent(typeof(StatusBar).FullName))
            {
                StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                await statusBar.HideAsync();
            }
        }
        private void mainButton_Click(object sender, RoutedEventArgs e)
        {
            mySplit.IsPaneOpen = !mySplit.IsPaneOpen;
        }

        private void todayButton_Click(object sender, RoutedEventArgs e)
        {
            if (mySplit.IsPaneOpen == true)
                mySplit.IsPaneOpen = !mySplit.IsPaneOpen;
            ContentFrame.Navigate(typeof(movie.nowPage));
            back();
        }

        private void soonButton_Click(object sender, RoutedEventArgs e)
        {
            if (mySplit.IsPaneOpen == true)
                mySplit.IsPaneOpen = !mySplit.IsPaneOpen;
            ContentFrame.Navigate(typeof(movie.soonPage));
            back();
        }

        private void amButton_Click(object sender, RoutedEventArgs e)
        {
            if (mySplit.IsPaneOpen == true)
                mySplit.IsPaneOpen = !mySplit.IsPaneOpen;
            ContentFrame.Navigate(typeof(movie.amPage));
            back();
        }

        private void topButton_Click(object sender, RoutedEventArgs e)
        {
            if (mySplit.IsPaneOpen == true)
                mySplit.IsPaneOpen = !mySplit.IsPaneOpen;
            ContentFrame.Navigate(typeof(movie.topPage));
            back();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (mySplit.IsPaneOpen == true)
                mySplit.IsPaneOpen = !mySplit.IsPaneOpen;
            ContentFrame.Navigate(typeof(movie.searchPage));
            back();
        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
            if (mySplit.IsPaneOpen == true)
                mySplit.IsPaneOpen = !mySplit.IsPaneOpen;
            ContentFrame.Navigate(typeof(movie.aboutPage));
            back();
        }

        private void likeButton_Click(object sender, RoutedEventArgs e)
        {
            if (mySplit.IsPaneOpen == true)
                mySplit.IsPaneOpen = !mySplit.IsPaneOpen;
            ContentFrame.Navigate(typeof(movie.lovePage));
            back();
        }
        private void back()
        {
            service.status = 0;
            if (ContentFrame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                if (service.time == 0)
                    SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
                service.time++;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }
        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            service.status = 0;
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
                e.Handled = true;
            }
            if (!ContentFrame.CanGoBack)
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }
    }
}
