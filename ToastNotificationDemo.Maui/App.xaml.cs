using Serilog;

namespace ToastNotificationDemo.Maui;

public partial class App : Application
{
	public App()
	{
		try
		{
            InitializeComponent();

            MainPage = new AppShell();
        }
		catch (Exception ex)
		{
			Log.Error($" Error in App : {ex.StackTrace}");
			
		}
		
	}
}
