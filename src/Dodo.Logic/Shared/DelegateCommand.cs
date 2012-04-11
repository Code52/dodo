using System;
using System.Windows.Input;

namespace Dodo.Logic.Shared
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecute;

        public DelegateCommand(Action action)
            : this(action, () => true) { }

        public DelegateCommand(Action action, Func<bool> canExecute )
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action();
        }
    }

    public class DelegateCommand<T> : ICommand where T : class
    {
        private readonly Action<T> _action;
        private readonly Func<T, bool> _canExecute;

        public DelegateCommand(Action<T> action)
            : this(action, a => true) { }

        public DelegateCommand(Action<T> action, Func<T,bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            var val = parameter as T;
            return _canExecute(val);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var val = parameter as T;
            _action(val);
        }
    }
}
