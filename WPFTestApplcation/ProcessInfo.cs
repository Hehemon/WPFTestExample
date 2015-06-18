using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace WPFTestApplcation
{
    /// <summary>
    /// Class for storing default info of process
    /// </summary>
    public class ProcessInfo : IEquatable<ProcessInfo>, ICloneable
    {
        public readonly int Id;
        public readonly string FriendlyName;
        public bool Removed { get; protected set; }

        public ProcessInfo(int id, string friendlyName)
        {
            Id = id;
            FriendlyName = friendlyName;
        }

        /// <summary>
        /// Remove this process from list
        /// </summary>
        public void Remove()
        {
            Removed = true;
        }

        public bool Equals(ProcessInfo other)
        {
            if (other == null) return false;
            return Id == other.Id &&
                   FriendlyName == other.FriendlyName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProcessInfo);
        }

        public override int GetHashCode()
        {
            // It's unique for each process
            return Id;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", FriendlyName, Id);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Get details of process
        /// </summary>
        /// <param name="id">Process id</param>
        /// <returns>String with complete information</returns>
        public static string GetProcessDetails(int id)
        {
            try
            {
                var info = Process.GetProcessById(id);
                return string.Format("{0}, started at {1}, total processor time: {2}", info.ProcessName, info.StartTime, info.TotalProcessorTime);
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error("Couldn't find details of process id {0} by reason {1}", id, e.Message);
                return string.Format("There are no such process in system: {0}", id);
            }
        }

    }
}
