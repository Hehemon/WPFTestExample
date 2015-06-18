using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using NLog;
using Timer = System.Timers.Timer;

namespace WPFTestApplcation
{
    /// <summary>
    /// Class for deliviring arguments of ProcessManager Event
    /// </summary>
    public class ProcessUpdateEventArgs : EventArgs
    {
        public IEnumerable<ProcessInfo> ProcessData { get; protected set; }

        public ProcessUpdateEventArgs(IEnumerable<ProcessInfo> data)
        {
            ProcessData = data;
        }
    }

    /// <summary>
    /// Managing process data class
    /// </summary>
    public sealed class ProcessManager
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static readonly ProcessManager I = new ProcessManager();

        public event EventHandler<ProcessUpdateEventArgs> OnProcessUpdate;

        /// <summary>
        /// Previous process data
        /// </summary>
        private List<ProcessInfo> Processes { get; set; }

        /// <summary>
        /// Timer for updating in different thread
        /// </summary>
        private Timer UpdatingTimer { get; set; }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="startUpdating">if start updating process at the begging</param>
        /// <param name="interval">update interval</param>
        public ProcessManager(bool startUpdating = true, int interval = 2000)
        {
            Processes = new List<ProcessInfo>(0);
            UpdatingTimer = new Timer();
            UpdatingTimer.Elapsed += OnTimedEvent;
            UpdatingTimer.Interval = interval;

            if (startUpdating)
            {
                ChangeState();
            }
        }

        /// <summary>
        /// Start/Stop updating processes
        /// </summary>
        public void ChangeState()
        {
            UpdatingTimer.Enabled = !UpdatingTimer.Enabled;
            LogManager.GetCurrentClassLogger()
                .Info(UpdatingTimer.Enabled ? "Process Updating has been started" : "Process Updating has been stopped");
        }

        /// <summary>
        /// Return current state of internal timer
        /// </summary>
        public bool CurrentState
        {
            get { return UpdatingTimer.Enabled; }
        }

        /// <summary>
        /// Notificate about event subscribers
        /// </summary>
        /// <param name="e">Arguments</param>
        private void OnUpdate(ProcessUpdateEventArgs e)
        {
            EventHandler<ProcessUpdateEventArgs> temp = Volatile.Read(ref OnProcessUpdate); // Thread safety

            if (temp != null) temp(this, e);
        }

        /// <summary>
        /// Asynchronouos way
        /// </summary>
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                var newProcesses = Process.GetProcesses();
                ProcessUpdateEventArgs args;
                lock (this)
                {
                    RemoveAndLogOldProcesses(newProcesses);
                    AddAndLogNewProcesses(newProcesses);
                    args = new ProcessUpdateEventArgs(Processes.Select(item => (ProcessInfo)item.Clone()));
                }
                OnUpdate(args);
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Error("OnTimedEvent exception: {0}, {1}", exception.Message, exception.StackTrace);
            }
        }

        /// <summary>
        /// Add new found processes and log it
        /// </summary>
        /// <param name="newProcesses">Collection of current proccesses</param>
        private void AddAndLogNewProcesses(IEnumerable<Process> newProcesses)
        {
            foreach (var process in newProcesses)
            {
                var found = Processes.SingleOrDefault(item => item.Id == process.Id);
                if (found == null)
                {
                    var toAdd = new ProcessInfo(process.Id, process.ProcessName);
                    Processes.Add(toAdd);
                    LogManager.GetCurrentClassLogger().Info("Found new process: {0}", toAdd);
                }
            }
        }

        /// <summary>
        /// Remove processes which have been finished and log it
        /// </summary>
        /// <param name="newProcesses">Collection of current processes</param>
        private void RemoveAndLogOldProcesses(IEnumerable<Process> newProcesses)
        {
            foreach (var process in Processes)
            {
                var found = newProcesses.SingleOrDefault(item => item.Id == process.Id);
                if (found == null)
                {
                    process.Remove(); // Mark process
                    LogManager.GetCurrentClassLogger().Info("Process has been removed: {0}", process);
                }
            }
            // Remove marked data
            Processes.RemoveAll(item => item.Removed);
        }
    }
}
