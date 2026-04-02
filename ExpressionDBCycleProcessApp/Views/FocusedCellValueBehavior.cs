using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ExpressionDBCycleProcessApp.Views;

public class FocusedCellValueBehavior : Behavior<DataGrid>
{
    public static readonly DependencyProperty FocusedCellValueProperty =
        DependencyProperty.Register(nameof(FocusedCellValue), typeof(string), typeof(FocusedCellValueBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string FocusedCellValue
    {
        get => (string)GetValue(FocusedCellValueProperty);
        set => SetValue(FocusedCellValueProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.GotKeyboardFocus += AssociatedObject_GotKeyboardFocus;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.GotKeyboardFocus -= AssociatedObject_GotKeyboardFocus;
    }

    private void AssociatedObject_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        var cell = e.OriginalSource as FrameworkElement;
        var dataGridCell = cell?.ParentOfType<DataGridCell>();
        if (dataGridCell?.Column is DataGridBoundColumn boundColumn &&
            boundColumn.Binding is Binding binding &&
            dataGridCell.DataContext is object dataItem)
        {
            var prop = dataItem.GetType().GetProperty(binding.Path.Path);
            var value = prop?.GetValue(dataItem)?.ToString();
            FocusedCellValue = value;
        }
    }
}

public static class VisualTreeHelperExtensions
{
    public static T? ParentOfType<T>(this DependencyObject child) where T : DependencyObject
    {
        DependencyObject? parentObject = child;

        while (parentObject != null)
        {
            if (parentObject is T typed)
                return typed;

            parentObject = VisualTreeHelper.GetParent(parentObject);
        }

        return null;
    }
}