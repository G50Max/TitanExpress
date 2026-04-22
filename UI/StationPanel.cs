using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TitanExpress.Models;
using TitanExpress.Systems;

namespace TitanExpress.UI
{
    public class StationPanel : Panel
    {
        private readonly StationManager _stationManager;
        private readonly GameResources _resources;
        private Button _deployButton;
        private Button _repairButton;
        private int _selectedStationId = -1;

        public StationPanel(Rectangle bounds, StationManager stationManager, GameResources resources) 
            : base(bounds, "基站管理")
        {
            _stationManager = stationManager;
            _resources = resources;
        }

        public void Initialize()
        {
            Buttons.Clear();

            _deployButton = AddButton(new Rectangle(Bounds.X + 20, Bounds.Y + 40, 120, 30), "部署基站");
            _deployButton.OnClick += DeployStation;

            _repairButton = AddButton(new Rectangle(Bounds.X + 20, Bounds.Y + 80, 120, 30), "修复基站");
            _repairButton.OnClick += RepairStation;

            UpdateButtonStates();
        }

        public void UpdateStationSelection(int stationId)
        {
            _selectedStationId = stationId;
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            _deployButton.Enabled = _resources.Battery >= 10;
            _repairButton.Enabled = _selectedStationId != -1 && _resources.Battery >= 5;
        }

        private void DeployStation()
        {
            if (_resources.Battery < 10)
                return;

            var station = _stationManager.DeployStation($"基站{_stationManager.Stations.Count + 1}", 100, 100);
            if (station != null)
            {
                _resources.Battery -= 10;
                _resources.MaxBandwidth += 5;
                _resources.Bandwidth += 5;
                UpdateButtonStates();
            }
        }

        private void RepairStation()
        {
            if (_selectedStationId == -1 || _resources.Battery < 5)
                return;

            var station = _stationManager.GetStation(_selectedStationId);
            if (station != null && station.Health < 100)
            {
                _stationManager.RepairStation(_selectedStationId, 25);
                _resources.Battery -= 5;
                UpdateButtonStates();
            }
        }

        public override void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            base.Draw(spriteBatch, font);

            var infoY = Bounds.Y + 120;
            var infoRect = new Rectangle(Bounds.X + 10, infoY, Bounds.Width - 20, Bounds.Height - infoY + Bounds.Y - 10);
            
            string stationInfo = "基站信息:\n";
            if (_selectedStationId != -1)
            {
                var station = _stationManager.GetStation(_selectedStationId);
                if (station != null)
                {
                    stationInfo += $"名称: {station.Name}\n";
                    stationInfo += $"位置: ({station.X}, {station.Y})\n";
                    stationInfo += $"状态: {(station.IsActive ? "在线" : "离线")}\n";
                    stationInfo += $"生命值: {station.Health}%\n";
                    stationInfo += $"带宽: {station.BandwidthProvided}\n";
                    stationInfo += $"范围: {station.Range}";
                }
            }
            else
            {
                stationInfo += "点击地图上的基站选择\n";
                stationInfo += $"总基站数: {_stationManager.Stations.Count}\n";
                stationInfo += $"总带宽: {_stationManager.TotalBandwidth}";
            }

            var label = new Label(infoRect, stationInfo);
            label.Draw(spriteBatch, font);
        }
    }
}