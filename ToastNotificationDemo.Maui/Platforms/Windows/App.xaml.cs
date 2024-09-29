using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Serilog;
using System.Diagnostics;
using ToastNotificationDemo.Maui.Platforms.Windows;
using Windows.ApplicationModel.Activation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using static System.Runtime.InteropServices.JavaScript.JSType;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ToastNotificationDemo.Maui.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
    public Action<string> ShowPopup { get; set; }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        try
        {
            this.InitializeComponent();


            AppNotificationManager notificationManager = AppNotificationManager.Default;
            notificationManager.NotificationInvoked += NotificationManager_NotificationInvoked;
        }
        catch (Exception ex)
        {
            Log.Error($"Error in init {ex.StackTrace}");
        }
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);
        try
        {
            var activatedArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
            var activationKind = activatedArgs.Kind;
            if (activationKind != ExtendedActivationKind.AppNotification)
            {
                Log.Information($"not notification {activationKind}");
            }
            else
            {
                Log.Information($"notification umesh {activationKind}");
                var data = (AppNotificationActivatedEventArgs)activatedArgs.Data;
                Log.Information($"notification OnLaunched:  {data?.Argument} : {data?.Arguments.FirstOrDefault()}");
                foreach (var item in data?.Arguments)
                {
                    Log.Information($"Key:  {item.Key} : value  {item.Value}");
                }

                await Task.Delay(2000);
                ShowPopup?.Invoke(data?.Arguments.Values.FirstOrDefault());
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error:  {ex.StackTrace}");
        }
    }

    private void NotificationManager_NotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
    {
        Log.Information($"notification {args?.Argument} : {args?.UserInput.FirstOrDefault()}");
        foreach (var item in args?.Arguments)
        {
            Log.Information($"Key:  {item.Key} : value  {item.Value}");
        }
        ShowPopup?.Invoke(args?.Arguments.Values.FirstOrDefault());
    }

    /// <summary>
    /// Create's a ToastContentBuilder with all of the main elements we want.
    /// </summary>
    /// <returns></returns>
    public AppNotificationBuilder CreateBasicToast()
    {
        var builder = new AppNotificationBuilder()
            .AddArgument("action", "toastClicked")
            .AddText("My Toast Notification")
            .AddText("Check this out, this is a cool toast description!");

        return builder;
    }

    /// <summary>
    /// Create's a ToastContentBuilder with all of the elements we want and an Ok button.
    /// </summary>
    /// <returns></returns>
	public void ShowToastOkBtn()
    {
        var builder = CreateBasicToast()
            .AddButton(new AppNotificationButton("OK")
                .AddArgument("action", "ok"));

        //builder.(toast => toast.Activated += (sender, args) =>
        //{
        //    if (args is ToastActivatedEventArgs toastArgs && toastArgs.Arguments.Contains("action=ok"))
        //        ShowPopup?.Invoke("Ok Button Clicked!");
        //});
        var notificationManager = AppNotificationManager.Default;
        notificationManager.Show(builder.BuildNotification());
    }

    /// <summary>
    /// Create's a ToastContentBuilder with all of the elements we want and a Yes and No button.
    /// </summary>
    /// <returns></returns>
    public void ShowToastYesNoBtn()
    {
        var builder = CreateBasicToast()
            .AddButton(new AppNotificationButton("Yes")
                .AddArgument("action", "yes"))
            .AddButton(new AppNotificationButton("No")
                .AddArgument("action", "no"));

        //builder.Show(toast => toast.Activated += (sender, args) =>
        //{
        //    var toastArgs = args as ToastActivatedEventArgs;
        //    if (toastArgs.Arguments.Contains("action=yes"))
        //        ShowPopup?.Invoke("Yes Button Clicked!");
        //    else if (toastArgs.Arguments.Contains("action=no"))
        //        ShowPopup?.Invoke("No Button Clicked!");
        //});
        var notificationManager = AppNotificationManager.Default;
        notificationManager.Show(builder.BuildNotification());
    }

    /// <summary>
    /// Create's a ToastContentBuilder with all of the elements we want and an Image
    /// </summary>
    /// <returns></returns>
    public AppNotificationBuilder CreateToastWithImage(bool useHeroImage)
    {
        var builder = CreateBasicToast();

        var uri = new Uri("https://picsum.photos/360/202?image=1043");
        if (!useHeroImage)
            builder.SetInlineImage(uri);
        else
            builder.SetHeroImage(uri);

        return builder;
    }

    /// <summary>
    /// Create's a ToastContentBuilder with all of the elements we want, an Image, and thumbs up/down buttons
    /// </summary>
    /// <returns></returns>
    public void ShowToastWithImageAndThumbs()
    {
        var builder = CreateToastWithImage(true)
            .AddButton(new AppNotificationButton("Thumbs up")
                .AddArgument("action", "thumbsUp"))
            .AddButton(new AppNotificationButton("Thumbs Down")
                .AddArgument("action", "thumbsDown"));

        //builder.Show(toast => toast.Activated += (sender, args) =>
        //{
        //    var toastArgs = args as ToastActivatedEventArgs;
        //    if (toastArgs.Arguments.Contains("action=thumbsUp"))
        //        ShowPopup?.Invoke("Thumbs Up Button Clicked!");
        //    else if (toastArgs.Arguments.Contains("action=thumbsDown"))
        //        ShowPopup?.Invoke("Thumbs Down Button Clicked!");
        //});
        var notificationManager = AppNotificationManager.Default;
        notificationManager.Show(builder.BuildNotification());
    }

    /// <summary>
    /// Create's a ToastContentBuilder with all of the elements we want and a dropdown.
    /// </summary>
    /// <returns></returns>
	public void ShowToastWithDropdown()
    {
        var builder = CreateBasicToast()
            .AddButton(new AppNotificationButton("Select")
                .AddArgument("action", "select"))

            // the ID "options" becomes the key that we use to access the selected value.
            .AddComboBox(new AppNotificationComboBox("options")
            {
                // default item is based off of the ID, not the value. Value is just for display.
                SelectedItem = "lunch",
                Items = new Dictionary<string, string>()
                {
                    {"breakfast", "Breakfast" },
                    {"lunch", "Lunch" },
                    {"dinner", "Dinner" },
                }
            });

        //builder.Show(toast => toast.Activated += (sender, args) =>
        //{
        //    if (args is ToastActivatedEventArgs toastArgs && toastArgs.Arguments.Contains("action=select"))
        //    {
        //        // "options" is the ID of our ToastSelectionBox.
        //        // If we had more than one type of user input, you'd use the ID of the UserInput you wanted.
        //        string selectionId = "options";
        //        var selectedItemId = toastArgs.UserInput[selectionId];
        //        ShowPopup?.Invoke($"Select Button Clicked!\nDropdown Item Selected: {selectedItemId}");
        //    }
        //});
        var notificationManager = AppNotificationManager.Default;
        notificationManager.Show(builder.BuildNotification());
    }

    public void ShowToastWithProgressBar()
    {
        new ProgressBarToast().Show();
    }
}

