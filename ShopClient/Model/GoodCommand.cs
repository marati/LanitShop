using System;
using System.Windows.Input;

namespace ShopClient.Model
{
    public class GoodCommand<T> : ICommand
    {
        Action<object> _action;

        public GoodCommand(Action<object> action)
        {
            _action = action;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
