namespace TitanExpress.Models
{
    public enum OrderType { Normal, Urgent, Dangerous }
    public enum OrderStatus { Pending, InProgress, Completed, Failed }

    public class Order
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Destination { get; set; }
        public OrderType Type { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public int Reward { get; set; }
        public int BandwidthRequired { get; set; }
        public int TimeRemaining { get; set; }

        public string TypeLabel => Type switch
        {
            OrderType.Normal => "[普通]",
            OrderType.Urgent => "[加急]",
            OrderType.Dangerous => "[危险]",
            _ => "[未知]"
        };

        public string StatusLabel => Status switch
        {
            OrderStatus.Pending => "[待处理]",
            OrderStatus.InProgress => "[配送中]",
            OrderStatus.Completed => "[已完成]",
            OrderStatus.Failed => "[失败]",
            _ => "[未知]"
        };
    }
}
