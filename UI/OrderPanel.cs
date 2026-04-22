using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TitanExpress.Models;
using TitanExpress.Systems;

namespace TitanExpress.UI
{
    public class OrderPanel : Panel
    {
        private readonly OrderManager _orderManager;
        private readonly StationManager _stationManager;
        private readonly GameResources _resources;
        private readonly List<Label> _orderLabels;

        public OrderPanel(Rectangle bounds, OrderManager orderManager, StationManager stationManager, GameResources resources) 
            : base(bounds, "订单管理")
        {
            _orderManager = orderManager;
            _stationManager = stationManager;
            _resources = resources;
            _orderLabels = new List<Label>();
        }

        public void UpdateOrders()
        {
            Buttons.Clear();
            _orderLabels.Clear();

            var pendingOrders = _orderManager.GetPendingOrders();
            var activeOrders = _orderManager.GetActiveOrders();

            int y = 40;
            int buttonWidth = 80;
            int buttonHeight = 25;

            if (pendingOrders.Count > 0)
            {
                AddLabel(new Rectangle(Bounds.X + 10, Bounds.Y + y, Bounds.Width - 20, 20), "待处理订单:");
                y += 25;

                foreach (var order in pendingOrders)
                {
                    string orderText = $"#{order.Id} {order.TypeLabel} {order.Description}";
                    string details = $"目的地: {order.Destination} | 奖励: ${order.Reward} | 需要带宽: {order.BandwidthRequired}";

                    AddLabel(new Rectangle(Bounds.X + 10, Bounds.Y + y, Bounds.Width - 20, 40), orderText + "\n" + details);
                    
                    int availableBandwidth = _stationManager.TotalBandwidth - _orderManager.GetActiveOrders().Sum(o => o.BandwidthRequired);
                    bool canAccept = availableBandwidth >= order.BandwidthRequired;

                    var acceptButton = AddButton(new Rectangle(Bounds.X + Bounds.Width - buttonWidth - 10, Bounds.Y + y, buttonWidth, buttonHeight), "接受");
                    acceptButton.Enabled = canAccept;
                    acceptButton.OnClick += () => AcceptOrder(order.Id);

                    y += 50;
                }
            }

            if (activeOrders.Count > 0)
            {
                y += 10;
                AddLabel(new Rectangle(Bounds.X + 10, Bounds.Y + y, Bounds.Width - 20, 20), "进行中订单:");
                y += 25;

                foreach (var order in activeOrders)
                {
                    string statusText = $"#{order.Id} {order.StatusLabel} 剩余时间: {order.TimeRemaining}s";
                    AddLabel(new Rectangle(Bounds.X + 10, Bounds.Y + y, Bounds.Width - 20, 20), statusText);
                    y += 30;
                }
            }

            if (pendingOrders.Count == 0 && activeOrders.Count == 0)
            {
                AddLabel(new Rectangle(Bounds.X + 10, Bounds.Y + 40, Bounds.Width - 20, 20), "暂无订单");
            }
        }

        private void AcceptOrder(int orderId)
        {
            int availableBandwidth = _stationManager.TotalBandwidth - _orderManager.GetActiveOrders().Sum(o => o.BandwidthRequired);
            if (_orderManager.AcceptOrder(orderId, availableBandwidth))
            {
                UpdateOrders();
            }
        }

        private void AddLabel(Rectangle bounds, string text)
        {
            var label = new Label(bounds, text);
            _orderLabels.Add(label);
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            base.Draw(spriteBatch, font);

            foreach (var label in _orderLabels)
            {
                label.Draw(spriteBatch, font);
            }
        }
    }

    public class Label
    {
        public Rectangle Bounds { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; } = Color.White;

        public Label(Rectangle bounds, string text)
        {
            Bounds = bounds;
            Text = text;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            var lines = Text.Split('\n');
            float y = Bounds.Y;
            
            foreach (var line in lines)
            {
                var pos = new Vector2(Bounds.X, y);
                spriteBatch.DrawString(font, line, pos, Color);
                y += font.MeasureString(line).Y;
            }
        }
    }
}
