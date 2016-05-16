using System;
using System.Windows.Input;

namespace DealOrNoDeal.Support
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> action;

        public DelegateCommand(Action<object> action)
        {
            this.action = action;
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

    }
}
