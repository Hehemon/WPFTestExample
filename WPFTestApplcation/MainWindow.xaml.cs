using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// <summary>
        /// For updating from not UI thread
        /// </summary>
        /// <param name="current"></param>
        private delegate void UpdateListViewDelegate(IEnumerable<ProcessInfo> current);
        private readonly UpdateListViewDelegate _updater;

        private ObservableCollection<ProcessInfo> _processes = new ObservableCollection<ProcessInfo>();
  
        public MainWindow()
        {
            InitializeComponent();
            _updater = UpdateListViewInUiThread;
        }

        /// <summary>
        /// Data binding for list view
        /// </summary>
        public ObservableCollection<ProcessInfo> Processes { get { return _processes; } }

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
        private void ShowDetails(object sender, RoutedEventArgs e)
        {
            var item = LvProcesses.SelectedItem as ProcessInfo;
            if (item == null)
            {
                MessageBox.Show("No selected item in ListView", "Details", MessageBoxButton.OK);
                return;
            }
            var message = ProcessInfo.GetProcessDetails(item.Id);
            MessageBox.Show(message, "Details", MessageBoxButton.OK);
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
            LvProcesses.Dispatcher.Invoke(_updater, e.ProcessData);
        }

        /// <summary>
        /// Updating data in listview
        /// </summary>
        /// <param name="e"></param>
        private void UpdateListViewInUiThread(IEnumerable<ProcessInfo> data)
        {
            _processes.Clear();
            foreach (var datum in data)
            {
                _processes.Add(datum);
            }
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

        /// <summary>
        /// Reflect Details button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LvProcesses_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnDetails.IsEnabled = LvProcesses.SelectedIndex != -1;
        }
    }
}
