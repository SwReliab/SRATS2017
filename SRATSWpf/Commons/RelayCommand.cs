using System;
using System.Windows.Input;

namespace SRATS2017AddIn.Commons
{
    public delegate bool CanExecuteFunc();
    public delegate void ExecuteFunc();

    public class RelayCommand : ICommand
    {
        private CanExecuteFunc check;
        private ExecuteFunc cmd;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(ExecuteFunc cmd, CanExecuteFunc check)
        {
            this.cmd = cmd;
            this.check = check;
        }

        public bool CanExecute(object parameter)
        {
            return check();
        }

        public void Execute(object parameter)
        {
            cmd();
        }
    }
}
