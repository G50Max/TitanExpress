using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TitanExpress.Models;
using TitanExpress.Systems;

namespace TitanExpress.UI
{
    public class MapView
    {
        private readonly StationManager _stationManager;
        private readonly Rectangle _bounds;
        private readonly Color _bgColor = new Color(15, 20, 30);
        private readonly Color _gridColor = new Color(30, 40, 50);
        private readonly Color _stationColor = new Color(0, 200, 150);
        private readonly Color _stationInactiveColor = new Color(150, 50, 50);
        private readonly Color _regionColor = new Color(40, 60, 80);
        private Texture2D _pixelTexture;

        private readonly Dictionary<string, Rectangle> _regions = new Dictionary<string, Rectangle>
        {
            { "旧城区", new Rectangle(50, 50, 120, 80) },
            { "商业中心", new Rectangle(200, 40, 130, 90) },
            { "工业港口", new Rectangle(360, 60, 120, 100) },
            { "科技园区", new Rectangle(80, 180, 140, 90) },
            { "地下避难所", new Rectangle(250, 190, 130, 80) }
        };

        public MapView(StationManager stationManager, Rectangle bounds)
        {
            _stationManager = stationManager;
            _bounds = bounds;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var bgRect = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, _bounds.Height);
            spriteBatch.Draw(_pixelTexture, bgRect, _bgColor);

            var borderRect = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, 2);
            spriteBatch.Draw(_pixelTexture, borderRect, new Color(0, 150, 100));

            int offsetX = _bounds.X + 10;
            int offsetY = _bounds.Y + 25;

            foreach (var region in _regions)
            {
                var rect = new Rectangle(offsetX + region.Value.X, offsetY + region.Value.Y, region.Value.Width, region.Value.Height);
                spriteBatch.Draw(_pixelTexture, rect, _regionColor);

                var border = new Rectangle(rect.X, rect.Y, rect.Width, 1);
                spriteBatch.Draw(_pixelTexture, border, new Color(60, 90, 120));
                var border2 = new Rectangle(rect.X, rect.Y + rect.Height - 1, rect.Width, 1);
                spriteBatch.Draw(_pixelTexture, border2, new Color(60, 90, 120));
                var border3 = new Rectangle(rect.X, rect.Y, 1, rect.Height);
                spriteBatch.Draw(_pixelTexture, border3, new Color(60, 90, 120));
                var border4 = new Rectangle(rect.X + rect.Width - 1, rect.Y, 1, rect.Height);
                spriteBatch.Draw(_pixelTexture, border4, new Color(60, 90, 120));
            }

            foreach (var station in _stationManager.Stations)
            {
                int sx = offsetX + station.X;
                int sy = offsetY + station.Y;
                int radius = 8;

                var rangeRect = new Rectangle(sx - station.Range / 10, sy - station.Range / 10, station.Range / 5, station.Range / 5);
                spriteBatch.Draw(_pixelTexture, rangeRect, new Color(0, 100, 80, 30));

                var stationRect = new Rectangle(sx - radius, sy - radius, radius * 2, radius * 2);
                spriteBatch.Draw(_pixelTexture, stationRect, station.IsActive ? _stationColor : _stationInactiveColor);
            }
        }
    }
}
