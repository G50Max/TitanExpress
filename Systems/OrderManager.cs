using System;
using System.Collections.Generic;
using System.Linq;
using TitanExpress.Models;

namespace TitanExpress.Systems
{
    public class OrderManager
    {
        private readonly List<Order> _orders = new List<Order>();
        private int _nextId = 1;
        private readonly Random _random = new Random();
        private readonly string[] _destinations = { "旧城区", "商业中心", "工业港口", "科技园区", "地下避难所" };
        private readonly string[] _normalDescriptions = {
            "配送医疗物资到指定区域",
            "运送食品补给包",
            "递送通信设备零件",
            "运输净水设备滤芯",
            "配送基础生活物资"
        };
        private readonly string[] _urgentDescriptions = {
            "紧急医疗物资配送 - 生命垂危!",
            "加急运送应急电源",
            "快速递送关键通信模块",
            "紧急运送防护装备"
        };
        private readonly string[] _dangerousDescriptions = {
            "穿越辐射区运送物资 - 高风险!",
            "通过敌对区域配送 - 需武装护送",
            "危险区域紧急补给任务",
            "穿越电磁干扰区运送设备"
        };

        public IReadOnlyList<Order> Orders => _orders.AsReadOnly();

        public void GenerateOrders(int count)
        {
            for (int i = 0; i < count; i++)
            {
                _orders.Add(GenerateRandomOrder());
            }
        }

        public List<Order> GetPendingOrders()
        {
            return _orders.Where(o => o.Status == OrderStatus.Pending).ToList();
        }

        public List<Order> GetActiveOrders()
        {
            return _orders.Where(o => o.Status == OrderStatus.InProgress).ToList();
        }

        public bool AcceptOrder(int orderId, int availableBandwidth)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null || order.Status != OrderStatus.Pending)
                return false;

            if (availableBandwidth < order.BandwidthRequired)
                return false;

            order.Status = OrderStatus.InProgress;
            order.TimeRemaining = order.Type switch
            {
                OrderType.Normal => 30,
                OrderType.Urgent => 15,
                OrderType.Dangerous => 20,
                _ => 30
            };
            return true;
        }

        public void UpdateOrders(float deltaTime)
        {
            foreach (var order in _orders.Where(o => o.Status == OrderStatus.InProgress))
            {
                order.TimeRemaining -= (int)deltaTime;
                if (order.TimeRemaining <= 0)
                {
                    order.Status = OrderStatus.Completed;
                }
            }
        }

        private Order GenerateRandomOrder()
        {
            int roll = _random.Next(100);
            OrderType type;
            string[] descriptions;

            if (roll < 60)
            {
                type = OrderType.Normal;
                descriptions = _normalDescriptions;
            }
            else if (roll < 85)
            {
                type = OrderType.Urgent;
                descriptions = _urgentDescriptions;
            }
            else
            {
                type = OrderType.Dangerous;
                descriptions = _dangerousDescriptions;
            }

            return new Order
            {
                Id = _nextId++,
                Description = descriptions[_random.Next(descriptions.Length)],
                Destination = _destinations[_random.Next(_destinations.Length)],
                Type = type,
                Reward = type switch
                {
                    OrderType.Normal => _random.Next(20, 50),
                    OrderType.Urgent => _random.Next(50, 100),
                    OrderType.Dangerous => _random.Next(80, 150),
                    _ => 30
                },
                BandwidthRequired = type switch
                {
                    OrderType.Normal => 1,
                    OrderType.Urgent => 2,
                    OrderType.Dangerous => 3,
                    _ => 1
                }
            };
        }
    }
}
