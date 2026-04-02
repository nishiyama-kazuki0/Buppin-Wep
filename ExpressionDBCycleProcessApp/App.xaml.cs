using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExpressionDBCycleProcessApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// テキストボックスを全選択させるためのイベントハンドラ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            textBox.SelectAll();
        }
    }
}
