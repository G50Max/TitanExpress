using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using TitanExpress.Models;
using TitanExpress.Systems;

namespace TitanExpress
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        private SpriteFont _font;

        private GameResources _resources;
        private OrderManager _orderManager;
        private StationManager _stationManager;

        private MouseState _currentMouse;
        private MouseState _previousMouse;

        private List<string> _messages;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;

            _messages = new List<string>();
            _resources = new GameResources();
            _orderManager = new OrderManager();
            _stationManager = new StationManager();
        }

        protected override void Initialize()
        {
            _orderManager.GenerateOrders(3);

            _messages.Add("系统启动成功");
            _messages.Add("欢迎使用泰坦快递");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
            
            _font = Content.Load<SpriteFont>("TerminalFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _currentMouse = Mouse.GetState();
            HandleInput();
            _previousMouse = _currentMouse;

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            if (_currentMouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
            {
                int screenWidth = GraphicsDevice.Viewport.Width;
                int screenHeight = GraphicsDevice.Viewport.Height;
                
                int btnY = screenHeight - 80;
                float scale = screenWidth / 1200f;
                
                var deployButton = new Rectangle((int)(80 * scale), btnY, (int)(100 * scale), 25);
                if (deployButton.Contains(_currentMouse.Position) && _resources.Battery >= 10)
                {
                    _stationManager.DeployStation("基站" + (_stationManager.Stations.Count + 1), 50 + _stationManager.Stations.Count * 80, 100);
                    _resources.Battery -= 10;
                    _messages.Add("部署基站成功");
                }

                var acceptButton = new Rectangle((int)(200 * scale), btnY, (int)(100 * scale), 25);
                if (acceptButton.Contains(_currentMouse.Position))
                {
                    var pendingOrders = _orderManager.GetPendingOrders();
                    if (pendingOrders.Count > 0)
                    {
                        var order = pendingOrders[0];
                        int availableBandwidth = _stationManager.TotalBandwidth - _orderManager.GetActiveOrders().Sum(o => o.BandwidthRequired);
                        if (_orderManager.AcceptOrder(order.Id, availableBandwidth))
                        {
                            _messages.Add("接受订单 #" + order.Id + " 成功");
                        }
                        else
                        {
                            _messages.Add("带宽不足，无法接受订单");
                        }
                    }
                }

                var statusButton = new Rectangle((int)(320 * scale), btnY, (int)(100 * scale), 25);
                if (statusButton.Contains(_currentMouse.Position))
                {
                    _messages.Add("=== 系统状态 ===");
                    _messages.Add("带宽: " + _resources.Bandwidth + "/" + _resources.MaxBandwidth);
                    _messages.Add("电池: " + _resources.Battery);
                    _messages.Add("信誉: " + _resources.Reputation);
                    _messages.Add("资金: $" + _resources.Money);
                    _messages.Add("基站数量: " + _stationManager.Stations.Count);
                    _messages.Add("总带宽: " + _stationManager.TotalBandwidth);
                }

                var clearButton = new Rectangle((int)(440 * scale), btnY, (int)(80 * scale), 25);
                if (clearButton.Contains(_currentMouse.Position))
                {
                    _messages.Clear();
                    _messages.Add("消息已清空");
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(15, 20, 25));
            
            _spriteBatch.Begin();
            
            DrawUI();
            
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private void DrawUI()
        {
            int screenWidth = GraphicsDevice.Viewport.Width;
            int screenHeight = GraphicsDevice.Viewport.Height;
            
            float scale = screenWidth / 1200f;
            
            int headerHeight = (int)(45 * scale);
            int resourceBarHeight = (int)(35 * scale);
            int panelHeight = (int)(280 * scale);
            int topPanelHeight = screenHeight - headerHeight - resourceBarHeight - panelHeight - 40;
            
            int margin = (int)(20 * scale);
            int gap = (int)(10 * scale);
            
            DrawPanel(0, 0, screenWidth, headerHeight, new Color(25, 30, 40));
            _spriteBatch.DrawString(_font, "泰坦快递 - Titan Express", new Vector2(margin, headerHeight / 2 - 10), Color.White);
            
            DrawResourceBars(scale, headerHeight);

            int leftWidth = (screenWidth - margin * 2 - gap * 2) / 3;
            int contentY = headerHeight + resourceBarHeight + 10;
            int contentHeight = topPanelHeight;
            
            DrawPanel(margin, contentY, leftWidth, contentHeight, new Color(30, 35, 45));
            _spriteBatch.DrawString(_font, "地图", new Vector2(margin + 10, contentY + 10), Color.White);
            DrawStations(margin, contentY, leftWidth);

            DrawPanel(margin + leftWidth + gap, contentY, leftWidth, contentHeight, new Color(35, 30, 45));
            _spriteBatch.DrawString(_font, "订单", new Vector2(margin + leftWidth + gap + 10, contentY + 10), Color.White);
            DrawOrders(margin + leftWidth + gap, contentY, leftWidth);

            DrawPanel(margin + (leftWidth + gap) * 2, contentY, leftWidth, contentHeight, new Color(45, 30, 35));
            _spriteBatch.DrawString(_font, "基站", new Vector2(margin + (leftWidth + gap) * 2 + 10, contentY + 10), Color.White);
            DrawStationInfo(margin + (leftWidth + gap) * 2, contentY, leftWidth);

            int bottomY = contentY + contentHeight + 10;
            DrawPanel(margin, bottomY, screenWidth - margin * 2, panelHeight, new Color(20, 25, 30));
            DrawButtons(scale, screenHeight);
            DrawMessages(bottomY, panelHeight);
        }

        private void DrawPanel(int x, int y, int width, int height, Color color)
        {
            _spriteBatch.Draw(_pixel, new Rectangle(x, y, width, height), color);
            DrawBorder(x, y, width, height, 2, new Color(color.R + 20, color.G + 20, color.B + 20));
        }

        private void DrawResourceBars(float scale, int headerHeight)
        {
            int baseY = headerHeight + 8;
            int x1 = (int)(20 * scale);
            int x2 = (int)(70 * scale);
            int x3 = (int)(150 * scale);
            int x4 = (int)(240 * scale);
            int x5 = (int)(290 * scale);
            int x6 = (int)(120 * scale);
            int x7 = (int)(430 * scale);
            int x8 = (int)(480 * scale);
            int x9 = (int)(620 * scale);
            
            _spriteBatch.DrawString(_font, "带宽:", new Vector2(x1, baseY), Color.White);
            DrawProgressBar(x2, baseY, x3, (int)(18 * scale), (float)_resources.Bandwidth / _resources.MaxBandwidth, Color.Blue);
            
            _spriteBatch.DrawString(_font, "电池:", new Vector2(x4, baseY), Color.White);
            DrawProgressBar(x5, baseY, x6, (int)(18 * scale), (float)_resources.Battery / 100, Color.Green);
            
            _spriteBatch.DrawString(_font, "信誉:", new Vector2(x7, baseY), Color.White);
            DrawProgressBar(x8, baseY, x6, (int)(18 * scale), (float)_resources.Reputation / 100, Color.Yellow);
            
            _spriteBatch.DrawString(_font, "$" + _resources.Money, new Vector2(x9, baseY), Color.Gold);
        }

        private void DrawProgressBar(int x, int y, int width, int height, float progress, Color color)
        {
            _spriteBatch.Draw(_pixel, new Rectangle(x, y, width, height), new Color(40, 40, 40));
            
            int progressWidth = (int)(width * Math.Min(1, Math.Max(0, progress)));
            if (progressWidth > 0)
            {
                _spriteBatch.Draw(_pixel, new Rectangle(x, y, progressWidth, height), color);
            }
            
            DrawBorder(x, y, width, height, 2, Color.White);
        }

        private void DrawStations(int panelX, int panelY, int panelWidth)
        {
            foreach (var station in _stationManager.Stations)
            {
                Color color = station.Health > 50 ? Color.Green : station.Health > 25 ? Color.Yellow : Color.Red;
                _spriteBatch.Draw(_pixel, new Rectangle(panelX + 20 + station.X, panelY + 30 + station.Y, 12, 12), color);
            }
        }

        private void DrawOrders(int panelX, int panelY, int panelWidth)
        {
            int y = panelY + 40;
            foreach (var order in _orderManager.GetPendingOrders())
            {
                _spriteBatch.Draw(_pixel, new Rectangle(panelX + 10, y, 20, 20), Color.Yellow);
                _spriteBatch.DrawString(_font, "#" + order.Id + " " + order.TypeLabel, new Vector2(panelX + 35, y), Color.White);
                _spriteBatch.DrawString(_font, "需要带宽:" + order.BandwidthRequired, new Vector2(panelX + 10, y + 28), Color.Gray);
                _spriteBatch.DrawString(_font, "奖励:$" + order.Reward, new Vector2(panelX + 10, y + 50), Color.Gold);
                y += 80;
            }
        }

        private void DrawStationInfo(int panelX, int panelY, int panelWidth)
        {
            int y = panelY + 40;
            foreach (var station in _stationManager.Stations)
            {
                Color healthColor = station.Health > 50 ? Color.Green : station.Health > 25 ? Color.Yellow : Color.Red;
                _spriteBatch.Draw(_pixel, new Rectangle(panelX + 10, y, 20, 20), healthColor);
                _spriteBatch.DrawString(_font, station.Name, new Vector2(panelX + 35, y), Color.White);
                _spriteBatch.DrawString(_font, (station.IsActive ? "在线" : "离线"), new Vector2(panelX + 35, y + 22), station.IsActive ? Color.Green : Color.Red);
                _spriteBatch.DrawString(_font, "带宽:" + station.BandwidthProvided + " 生命:" + station.Health + "%", new Vector2(panelX + 10, y + 48), Color.Gray);
                y += 75;
            }
        }

        private void DrawButtons(float scale, int screenHeight)
        {
            int btnY = screenHeight - 80;
            
            Color deployColor = _resources.Battery >= 10 ? Color.Green : Color.Red;
            _spriteBatch.Draw(_pixel, new Rectangle((int)(80 * scale), btnY, (int)(100 * scale), 25), deployColor);
            DrawBorder((int)(80 * scale), btnY, (int)(100 * scale), 25, 2, Color.White);
            _spriteBatch.DrawString(_font, "部署基站", new Vector2(85 * scale, btnY + 4), Color.White);

            bool hasPendingOrder = _orderManager.GetPendingOrders().Count > 0;
            Color acceptColor = hasPendingOrder ? Color.Green : Color.Gray;
            _spriteBatch.Draw(_pixel, new Rectangle((int)(200 * scale), btnY, (int)(100 * scale), 25), acceptColor);
            DrawBorder((int)(200 * scale), btnY, (int)(100 * scale), 25, 2, Color.White);
            _spriteBatch.DrawString(_font, "接受订单", new Vector2(205 * scale, btnY + 4), Color.White);

            _spriteBatch.Draw(_pixel, new Rectangle((int)(320 * scale), btnY, (int)(100 * scale), 25), Color.Blue);
            DrawBorder((int)(320 * scale), btnY, (int)(100 * scale), 25, 2, Color.White);
            _spriteBatch.DrawString(_font, "系统状态", new Vector2(325 * scale, btnY + 4), Color.White);

            _spriteBatch.Draw(_pixel, new Rectangle((int)(440 * scale), btnY, (int)(80 * scale), 25), Color.Purple);
            DrawBorder((int)(440 * scale), btnY, (int)(80 * scale), 25, 2, Color.White);
            _spriteBatch.DrawString(_font, "清空", new Vector2(450 * scale, btnY + 4), Color.White);
        }

        private void DrawMessages(int panelY, int panelHeight)
        {
            int y = panelY + 20;
            for (int i = 0; i < _messages.Count && i < 8; i++)
            {
                Color messageColor = i == _messages.Count - 1 ? Color.LightGreen : Color.Gray;
                _spriteBatch.DrawString(_font, _messages[i], new Vector2(40, y), messageColor);
                y += 28;
            }
        }

        private void DrawBorder(int x, int y, int width, int height, int borderWidth, Color color)
        {
            _spriteBatch.Draw(_pixel, new Rectangle(x, y, width, borderWidth), color);
            _spriteBatch.Draw(_pixel, new Rectangle(x, y + height - borderWidth, width, borderWidth), color);
            _spriteBatch.Draw(_pixel, new Rectangle(x, y, borderWidth, height), color);
            _spriteBatch.Draw(_pixel, new Rectangle(x + width - borderWidth, y, borderWidth, height), color);
        }
    }
}
