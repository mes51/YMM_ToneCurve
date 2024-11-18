using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace YMM_ToneCurve.View.Command
{
    class DelegateCommand : ICommand
    {
        Action ExecuteBody { get; }

        Func<bool> CanExecuteBody { get; }

        public event EventHandler? CanExecuteChanged;

        public DelegateCommand(Action executeBody) : this(executeBody, () => true) { }

        public DelegateCommand(Action executeBody, Func<bool> canExecuteBody)
        {
            ExecuteBody = executeBody;
            CanExecuteBody = canExecuteBody;
        }

        public bool CanExecute(object? parameter)
        {
            return CanExecuteBody();
        }

        public void Execute(object? parameter)
        {
            ExecuteBody();
        }
    }
}
