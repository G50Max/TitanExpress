namespace TitanExpress.Models
{
    public class BaseStation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Range { get; set; } = 100;
        public int BandwidthProvided { get; set; } = 5;
        public bool IsActive { get; set; } = true;
        public int Health { get; set; } = 100;

        public override string ToString()
        {
            return $"基站 #{Id}: {Name} | 范围: {Range} | 带宽: {BandwidthProvided} | 状态: {(IsActive ? "在线" : "离线")} | 生命: {Health}%";
        }
    }
}
