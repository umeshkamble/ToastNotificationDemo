using CommunityToolkit.Maui;
using Microsoft.Maui.LifecycleEvents;
#if WINDOWS
using Microsoft.Windows.AppLifecycle;
#endif
using Serilog;
using Serilog.Events;
using System.Diagnostics;

namespace ToastNotificationDemo.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        SetupSerilog();
        builder
            .UseMauiApp<App>()
             .UseMauiCommunityToolkit(options =>
             {
                 options.SetShouldEnableSnackbarOnWindows(true);
             })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
            //.ConfigureLifeCycleEvent();

        return builder.Build();
    }

    private static void SetupSerilog()
    {
        try
        {

            var flushInterval = new TimeSpan(0, 0, 1);
            var file = Path.Combine(FileSystem.AppDataDirectory, "Umesh.txt");

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(file, flushToDiskInterval: flushInterval, encoding: System.Text.Encoding.UTF8, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 22)
            .CreateLogger();
        }
        catch (Exception  er)
        {

           
        }
    }
    private static MauiAppBuilder ConfigureLifeCycleEvent(this MauiAppBuilder builder)
    {
        builder.ConfigureLifecycleEvents(events =>
        {
#if WINDOWS
            events.AddWindows(windowsLifecycleBuilder =>
            {
                //windowsLifecycleBuilder.OnWindowCreated(window =>
                //{
                //    window.Title = "Angel Chip Validation & Counting System";
                //    window.ExtendsContentIntoTitleBar = false;
                //    var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                //    var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);

                //});
                //windowsLifecycleBuilder.OnActivated((window, args) =>
                //{
                //    Debug.WriteLine(nameof(WindowsLifecycle.OnActivated));

                //});
                windowsLifecycleBuilder.OnActivated(async (window, act) =>
                {
                    try
                    {
                        AppActivationArguments activatedEventArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
                        Debug.WriteLine($"Umesh--: {activatedEventArgs.Kind}");
                        Log.Information($"Umesh--: {activatedEventArgs.Kind}");
                        if (activatedEventArgs.Kind == ExtendedActivationKind.AppNotification)
                        {
                            Log.Information($"Umesh OKKK: {activatedEventArgs.Kind}");
                            //await App.Current.MainPage.DisplayAlert("Alert", "Hello Umesh", "Ok");
                        }
                    }
                    catch (Exception er)
                    {
                        Log.Error($"Error OnActivated --{er.StackTrace}");
                    }
                });
                windowsLifecycleBuilder.OnLaunched(async (window, act) =>
                {
                    try
                    {
                        var activatedEventArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();

                        Log.Information($"Umesh On Launched--: {activatedEventArgs.Kind}");
                      
                    }
                    catch (Exception
                    er)
                    {
                        Log.Error($"Error maui program -- {er.StackTrace}");

                    }
                   

                });
            });

#endif
        });
        return builder;
    }

}
