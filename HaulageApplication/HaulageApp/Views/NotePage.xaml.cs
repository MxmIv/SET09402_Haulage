using HaulageApp.ViewModels;

namespace HaulageApp.Views;

public partial class NotePage : ContentPage
{
    public NotePage(NoteViewModel viewModel)
    {
        this.BindingContext = viewModel;   
        InitializeComponent();
        Console.WriteLine("NotePage initialized");

    }
}