using HaulageApp.ViewModels;
using HaulageApp.Views;

namespace HaulageApp;

public partial class AppShell : Shell
{
    public AppShell(PermissionsViewModel viewmodel)
    {
        BindingContext = viewmodel;
        InitializeComponent();

        Routing.RegisterRoute("notes", typeof(AllNotesPage));
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("vehicles", typeof(AllVehiclesPage));
        Routing.RegisterRoute(nameof(NotePage), typeof(NotePage));
        Routing.RegisterRoute("expenses", typeof(ExpensesPage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
        Routing.RegisterRoute(nameof(VehiclePage), typeof(VehiclePage));
        Routing.RegisterRoute(nameof(EditExpensePage), typeof(EditExpensePage));
        Routing.RegisterRoute(nameof(AllBillsPage), typeof(AllBillsPage));
    }
}