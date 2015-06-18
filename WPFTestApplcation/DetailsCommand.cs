using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using NLog;

namespace WPFTestApplcation
{
    /// <summary>
    /// Process "Get Process details" command
    /// </summary>
    class DetailsCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            // TODO: check if there is selected item
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                var id = Convert.ToInt32(parameter);
                var message = ProcessInfo.GetProcessDetails(id);
                MessageBox.Show(message, "Details", MessageBoxButton.OK);
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error("Coudln't execute details command: {0}, {1}", e.Message, e.StackTrace);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
