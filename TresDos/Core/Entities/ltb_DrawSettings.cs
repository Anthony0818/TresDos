namespace TresDos.Core.Entities
{
    public class ltb_DrawSettings
    {
        public int id { get; set; }
        public string DrawType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CutOffTime { get; set; }
        public DateTime DrawTime { get; set; }
    }
}
