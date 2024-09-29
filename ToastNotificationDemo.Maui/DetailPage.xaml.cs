namespace ToastNotificationDemo.Maui;

[QueryProperty(nameof(Message), "Message")]
public partial class DetailPage : ContentPage
{
    string message;
    public string Message
    {
        get => message;
        set
        {
            message= value;
            OnPropertyChanged();
        }
    }
    public DetailPage()
	{
		InitializeComponent();
        BindingContext = this;
	}
}