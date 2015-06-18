using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFTestApplcation
{
    /// <summary>
    /// Process start/stop command for manager
    /// </summary>
    class StartStopCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ProcessManager.I.ChangeState();
            // TODO: reflect interface on viewmodel
        }

        public event EventHandler CanExecuteChanged;
    }
}
