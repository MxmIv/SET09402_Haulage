using HaulageApp.ViewModels;

namespace HaulageApp.Views;

public partial class AllNotesPage : ContentPage
{
    public AllNotesPage(AllNotesViewModel viewModel)
    {
        this.BindingContext = viewModel;   
        InitializeComponent();
        Console.WriteLine("AllNotesPage initialized");

    }
    
    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        notesCollection.SelectedItem = null;
    }
}