using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TitanExpress.Models;

namespace TitanExpress.UI
{
    public class ResourceDisplay
    {
        private readonly GameResources _resources;
        private readonly Rectangle _bounds;
        private readonly List<ResourceProgressBar> _bars;

        public ResourceDisplay(GameResources resources, Rectangle bounds)
        {
            _resources = resources;
            _bounds = bounds;
            _bars = new List<ResourceProgressBar>();
        }

        public void Initialize()
        {
            int barWidth = 150;
            int barHeight = 25;
            int spacing = 10;
            int x = _bounds.X;
            int y = _bounds.Y;

            _bars.Add(new ResourceProgressBar("带宽", _resources.Bandwidth, _resources.MaxBandwidth, new Rectangle(x, y, barWidth, barHeight), new Color(0, 150, 200)));
            x += barWidth + spacing;

            _bars.Add(new ResourceProgressBar("电池", _resources.Battery, 100, new Rectangle(x, y, barWidth, barHeight), new Color(0, 200, 0)));
            x += barWidth + spacing;

            _bars.Add(new ResourceProgressBar("信誉", _resources.Reputation, 100, new Rectangle(x, y, barWidth, barHeight), new Color(200, 150, 0)));
            x += barWidth + spacing;

            _bars.Add(new ResourceProgressBar("资金", _resources.Money, 1000, new Rectangle(x, y, barWidth, barHeight), new Color(0, 200, 0)));
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            foreach (var bar in _bars)
            {
                bar.Draw(spriteBatch, font);
            }
        }
    }

    public class ResourceProgressBar
    {
        private readonly string _label;
        private readonly int _current;
        private readonly int _max;
        private readonly Rectangle _bounds;
        private readonly Color _color;

        public ResourceProgressBar(string label, int current, int max, Rectangle bounds, Color color)
        {
            _label = label;
            _current = current;
            _max = max;
            _bounds = bounds;
            _color = color;
        }

        public virtual void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            var bgRect = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, _bounds.Height);
            spriteBatch.Draw(pixel, bgRect, new Color(30, 30, 40));

            float percentage = (float)_current / _max;
            int fillWidth = (int)(_bounds.Width * percentage);
            var fillRect = new Rectangle(_bounds.X, _bounds.Y, fillWidth, _bounds.Height);
            spriteBatch.Draw(pixel, fillRect, _color);

            var borderRect = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, 1);
            spriteBatch.Draw(pixel, borderRect, new Color(60, 60, 80));
            var borderRect2 = new Rectangle(_bounds.X, _bounds.Y + _bounds.Height - 1, _bounds.Width, 1);
            spriteBatch.Draw(pixel, borderRect2, new Color(60, 60, 80));
            var borderRect3 = new Rectangle(_bounds.X, _bounds.Y, 1, _bounds.Height);
            spriteBatch.Draw(pixel, borderRect3, new Color(60, 60, 80));
            var borderRect4 = new Rectangle(_bounds.X + _bounds.Width - 1, _bounds.Y, 1, _bounds.Height);
            spriteBatch.Draw(pixel, borderRect4, new Color(60, 60, 80));

            string text = $"{_label}: {_current}/{_max}";
            var textSize = font.MeasureString(text);
            var textPos = new Vector2(_bounds.X + (_bounds.Width - textSize.X) / 2, _bounds.Y + (_bounds.Height - textSize.Y) / 2);
            spriteBatch.DrawString(font, text, textPos, Color.White);
        }
    }
}
