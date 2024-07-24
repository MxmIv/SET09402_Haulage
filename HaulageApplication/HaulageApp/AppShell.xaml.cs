namespace HaulageApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Console.WriteLine("AppShell constructor");
        InitializeComponent();
        Console.WriteLine("AppShell initialized");
        Routing.RegisterRoute(nameof(Views.NotePage), typeof(Views.NotePage));

    }
}