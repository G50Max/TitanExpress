using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TitanExpress.Models;
using TitanExpress.Systems;

namespace TitanExpress.UI
{
    public class ResourceBar
    {
        private readonly GameResources _resources;
        private readonly SpriteFont _font;
        private readonly Rectangle _bounds;

        public ResourceBar(GameResources resources, SpriteFont font, Rectangle bounds)
        {
            _resources = resources;
            _font = font;
            _bounds = bounds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var bgRect = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, 30);
            spriteBatch.Draw(CreateTexture(spriteBatch.GraphicsDevice, new Color(30, 30, 40)), bgRect, Color.White);

            string text = _resources.ToString();
            var textSize = _font.MeasureString(text);
            var pos = new Vector2(_bounds.X + 10, _bounds.Y + 5);
            spriteBatch.DrawString(_font, text, pos, new Color(0, 200, 150));
        }

        private Texture2D CreateTexture(GraphicsDevice device, Color color)
        {
            var texture = new Texture2D(device, 1, 1);
            texture.SetData(new[] { color });
            return texture;
        }
    }
}
