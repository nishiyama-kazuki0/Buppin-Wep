using ExpressionDBCycleProcessApp.ViewModels;
using System.Windows;

namespace ExpressionDBCycleProcessApp.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}