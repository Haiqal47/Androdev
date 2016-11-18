namespace Androdev.Core.Args
{
    public class DownloadStartedEventArgs : System.EventArgs
    {
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string Queue { get; set; }
    }
}
