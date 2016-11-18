namespace Androdev.Core.Args
{
    public class UpdateProgressChangedEventArgs : System.EventArgs
    {
        public int ProgressPercentage { get; set; }
        public string Downloaded { get; set; }
    }
}
