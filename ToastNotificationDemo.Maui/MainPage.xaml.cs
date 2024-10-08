﻿#if WINDOWS
using Microsoft.Windows.AppNotifications;
#endif
using System.Diagnostics;
namespace ToastNotificationDemo.Maui;

public partial class MainPage : ContentPage
{
    // Using this to track whether or not the example image has been downloaded.
    // If not it will show a popup letting the user know it needs to download and may take time.
    public bool HasDownloadedImage
    {
        get { return Preferences.Default.Get<bool>(nameof(HasDownloadedImage), false); }
        set { Preferences.Default.Set(nameof(HasDownloadedImage), value); }
    }

    public MainPage()
    {
        InitializeComponent();


#if WINDOWS

        // Using this to communicate between the Windows specific project and MainPage.
        // This is needed for the Demo project since I'm using DisplayAlert to show Popups, 
        // leting the user know what happened with the Toast.
        GetWinUIApp().ShowPopup += Popup;
        GetWinUIApp().NavigationToAnotherPage += async (message) =>
        {
            await Application.Current.Dispatcher.DispatchAsync(async () =>
             {

                 await Shell.Current.GoToAsync($"{nameof(DetailPage)}?Message=User input {message}");
             });
        };

        BasicToastBtn.Clicked += (sender, args) =>
        {
            var builder = GetWinUIApp().CreateBasicToast();
            var notificationManager = AppNotificationManager.Default;
            notificationManager.Show(builder.BuildNotification());
        };

        ToastOkBtn.Clicked += (sender, args) => GetWinUIApp().ShowToastOkBtn();

        ToastYesNoBtn.Clicked += (sender, args) => GetWinUIApp().ShowToastYesNoBtn();


        ToastWithImageBtn.Clicked += (sender, args) =>
        {
            ShowImageWarning();
            var builder = GetWinUIApp().CreateToastWithImage(useHeroImage: false);
            var notificationManager = AppNotificationManager.Default;
            notificationManager.Show(builder.BuildNotification());
        };

        ToastWithHeroImageBtn.Clicked += (sender, args) =>
        {
            ShowImageWarning();
            var builder = GetWinUIApp().CreateToastWithImage(useHeroImage: true);
            var notificationManager = AppNotificationManager.Default;
            notificationManager.Show(builder.BuildNotification());
        };

        ToastWithHeroImageAndThumbsBtn.Clicked += (sender, args) =>
        {
            ShowImageWarning();
            GetWinUIApp().ShowToastWithImageAndThumbs();
        };

        ToastWithDropDown.Clicked += (sender, args) => GetWinUIApp().ShowToastWithDropdown();
        ToastWithProgressBar.Clicked += (sender, args) => GetWinUIApp().ShowToastWithProgressBar();
#endif
    }

    public void Popup(string message)
    {
        Dispatcher.Dispatch(async () => await DisplayAlert("Notice", message, "Ok"));
    }

    private void ShowImageWarning()
    {
        if (!HasDownloadedImage)
        {
            Popup("Image needs to download, may take a moment to load");
            HasDownloadedImage = true;
        }
    }


#if WINDOWS

    // Moved most of the Windows specific code to the Windows project. 
    // It's cleaner than putting it all here, however you can put it all in MainPage if you want by using #if WINDOWS.
    public ToastNotificationDemo.Maui.WinUI.App GetWinUIApp()
    {
        return App.Current.Handler.PlatformView as ToastNotificationDemo.Maui.WinUI.App;
    }
#endif
}

