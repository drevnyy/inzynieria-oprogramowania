using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace tree_manager
{
    /**
     * klasa wirtualizuj¹ca grid
     */
    public class DataGridColumnsBehavior
    {
        public static readonly DependencyProperty BindableColumnsProperty =
            DependencyProperty.RegisterAttached("BindableColumns",
                                                typeof(ObservableCollection<DataGridColumn>),
                                                typeof(DataGridColumnsBehavior),
                                                new UIPropertyMetadata(null, BindableColumnsPropertyChanged));
        private static void BindableColumnsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = source as DataGrid;
            var columns = e.NewValue as ObservableCollection<DataGridColumn>;
            if (dataGrid != null)
            {
                dataGrid.Columns.Clear();
                if (columns == null)
                {
                    return;
                }
                foreach (DataGridColumn column in columns)
                {
                    dataGrid.Columns.Add(column);
                }
                columns.CollectionChanged += (sender, e2) =>
                {
                    NotifyCollectionChangedEventArgs changedEventArgs = e2;
                    if (changedEventArgs.Action == NotifyCollectionChangedAction.Reset)
                    {
                        dataGrid.Columns.Clear();
                        if (changedEventArgs.NewItems != null)
                        {
                            foreach (DataGridColumn column in changedEventArgs.NewItems)
                            {
                                dataGrid.Columns.Add(column);
                            }
                        }
                    }
                    else if (changedEventArgs.Action == NotifyCollectionChangedAction.Add)
                    {
                        if (changedEventArgs.NewItems != null)
                        {
                            foreach (DataGridColumn column in changedEventArgs.NewItems)
                            {
                                dataGrid.Columns.Add(column);
                            }
                        }
                    }
                    else if (changedEventArgs.Action == NotifyCollectionChangedAction.Move)
                    {
                        dataGrid.Columns.Move(changedEventArgs.OldStartingIndex, changedEventArgs.NewStartingIndex);
                    }
                    else if (changedEventArgs.Action == NotifyCollectionChangedAction.Remove)
                    {
                        if (changedEventArgs.OldItems != null)
                        {
                            foreach (DataGridColumn column in changedEventArgs.OldItems)
                            {
                                dataGrid.Columns.Remove(column);
                            }
                        }
                    }
                    else if (changedEventArgs.Action == NotifyCollectionChangedAction.Replace)
                    {
                        dataGrid.Columns[changedEventArgs.NewStartingIndex] = changedEventArgs.NewItems[0] as DataGridColumn;
                    }
                };
            }
        }
        public static void SetBindableColumns(DependencyObject element, ObservableCollection<DataGridColumn> value)
        {
            element.SetValue(BindableColumnsProperty, value);
        }
        public static ObservableCollection<DataGridColumn> GetBindableColumns(DependencyObject element)
        {
            return (ObservableCollection<DataGridColumn>)element.GetValue(BindableColumnsProperty);
        }
    }
}