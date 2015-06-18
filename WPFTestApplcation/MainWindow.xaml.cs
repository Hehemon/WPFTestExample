using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFTestApplcation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Start/stop event handling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStartStop_OnClick(object sender, RoutedEventArgs e)
        {
            ProcessManager.I.ChangeState();
            BtnStartStop.Content = ProcessManager.I.CurrentState ? "Stop" : "Start";
        }

        /// <summary>
        /// Details event handling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDetails_OnClick(object sender, RoutedEventArgs e)
        {
            var item = LvProcesses.SelectedItem;
            if (item == null)
            {
                MessageBox.Show("No selected item in ListView");
                return;
            }
            // TODO: process defining id of porcess and showing message box
        }

        /// <summary>
        /// Subscribe to events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            BtnStartStop.Content = ProcessManager.I.CurrentState ? "Stop" : "Start";
            ProcessManager.I.OnProcessUpdate += UpdateListEvent;
        }

        /// <summary>
        /// Process income messages with new processes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateListEvent(object sender, ProcessUpdateEventArgs e)
        {
            
        }

        /// <summary>
        /// Unsubscribe from updating
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ProcessManager.I.OnProcessUpdate -= UpdateListEvent;
        }
    }
}
