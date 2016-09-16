using System;

namespace Androdev.Core
{
    public class InstallProgressChangedEventArgs : EventArgs
    {
        public int OverallProgressPercentage { get; set; }
        public int CurrentProgressPercentage { get; set; }
        public string StatusText { get; set; }
        public string ExtraStatusText { get; set; }
    }
}
