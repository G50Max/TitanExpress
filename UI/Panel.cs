using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TitanExpress.UI
{
    public class Panel
    {
        public Rectangle Bounds { get; set; }
        public string Title { get; set; }
        public List<Button> Buttons { get; private set; }
        public bool Visible { get; set; } = true;

        private readonly Color _bgColor = new Color(20, 25, 35, 220);
        private readonly Color _borderColor = new Color(0, 150, 100);
        private readonly Color _titleColor = new Color(0, 200, 150);

        public Panel(Rectangle bounds, string title)
        {
            Bounds = bounds;
            Title = title;
            Buttons = new List<Button>();
        }

        public void Update(MouseState mouseState)
        {
            if (!Visible) return;

            foreach (var button in Buttons)
            {
                button.Update(mouseState);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (!Visible) return;

            var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(pixel, Bounds, _bgColor);

            var borderRect = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 2);
            spriteBatch.Draw(pixel, borderRect, _borderColor);

            if (!string.IsNullOrEmpty(Title))
            {
                var titleSize = font.MeasureString(Title);
                var titlePos = new Vector2(Bounds.X + 10, Bounds.Y + 5);
                spriteBatch.DrawString(font, Title, titlePos, _titleColor);
            }

            foreach (var button in Buttons)
            {
                button.Draw(spriteBatch, font);
            }
        }

        public Button AddButton(Rectangle bounds, string text)
        {
            var button = new Button(bounds, text);
            Buttons.Add(button);
            return button;
        }
    }
}
