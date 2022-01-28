using System;
using System.Windows.Input;

namespace Revelator.io24.Wpf.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _executeAction;

        public DelegateCommand(Action<object> executeAction)
        {
            _executeAction = executeAction;
        }

        public void Execute(object parameter) => _executeAction(parameter);

        public bool CanExecute(object parameter) => true; //TODO: add connection open as indicator.

        public event EventHandler CanExecuteChanged;
    }
}
