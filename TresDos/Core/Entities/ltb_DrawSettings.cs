namespace TresDos.Core.Entities
{
    public class ltb_DrawSettings
    {
        public int id { get; set; }
        public string DrawType { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan CutOffTime { get; set; }
        public TimeSpan DrawTime { get; set; }
    }
}
