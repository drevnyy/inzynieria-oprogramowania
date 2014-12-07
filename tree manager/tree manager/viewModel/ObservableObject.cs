using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace tree_manager.viewModel
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void RaisePropertyChangedEvent(PropertyChangedEventArgs property)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, property);
        }
        protected void ChangePropertyAndNotify<T>(Action<T> setter, T value)
        {
            setter(value);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(setter.Method.Name));
            }
        }
    }
}
