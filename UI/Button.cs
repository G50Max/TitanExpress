using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TitanExpress.UI
{
    public class Button
    {
        public Rectangle Bounds { get; set; }
        public string Text { get; set; }
        public bool Enabled { get; set; } = true;
        public bool IsHovered { get; private set; }
        public bool IsPressed { get; private set; }
        public event Action OnClick;

        private readonly Color _normalColor = new Color(0, 120, 80);
        private readonly Color _hoverColor = new Color(0, 150, 100);
        private readonly Color _pressedColor = new Color(0, 180, 120);
        private readonly Color _disabledColor = new Color(60, 60, 60);
        private readonly Color _textColor = Color.White;
        private readonly Color _disabledTextColor = new Color(120, 120, 120);

        public Button(Rectangle bounds, string text)
        {
            Bounds = bounds;
            Text = text;
        }

        public void Update(MouseState mouseState)
        {
            if (!Enabled)
            {
                IsHovered = false;
                IsPressed = false;
                return;
            }

            IsHovered = Bounds.Contains(mouseState.Position);

            if (IsHovered && mouseState.LeftButton == ButtonState.Pressed)
            {
                IsPressed = true;
            }
            else if (IsPressed && mouseState.LeftButton == ButtonState.Released)
            {
                if (IsHovered)
                {
                    OnClick?.Invoke();
                }
                IsPressed = false;
            }
            else if (mouseState.LeftButton == ButtonState.Released)
            {
                IsPressed = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            Color bgColor;
            Color textColor;

            if (!Enabled)
            {
                bgColor = _disabledColor;
                textColor = _disabledTextColor;
            }
            else if (IsPressed)
            {
                bgColor = _pressedColor;
                textColor = _textColor;
            }
            else if (IsHovered)
            {
                bgColor = _hoverColor;
                textColor = _textColor;
            }
            else
            {
                bgColor = _normalColor;
                textColor = _textColor;
            }

            var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            spriteBatch.Draw(pixel, Bounds, bgColor);

            var textSize = font.MeasureString(Text);
            var textPos = new Vector2(
                Bounds.X + (Bounds.Width - textSize.X) / 2,
                Bounds.Y + (Bounds.Height - textSize.Y) / 2
            );

            spriteBatch.DrawString(font, Text, textPos, textColor);

            var borderRect = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, 1);
            spriteBatch.Draw(pixel, borderRect, new Color(0, 200, 150));
            var borderRect2 = new Rectangle(Bounds.X, Bounds.Y + Bounds.Height - 1, Bounds.Width, 1);
            spriteBatch.Draw(pixel, borderRect2, new Color(0, 200, 150));
            var borderRect3 = new Rectangle(Bounds.X, Bounds.Y, 1, Bounds.Height);
            spriteBatch.Draw(pixel, borderRect3, new Color(0, 200, 150));
            var borderRect4 = new Rectangle(Bounds.X + Bounds.Width - 1, Bounds.Y, 1, Bounds.Height);
            spriteBatch.Draw(pixel, borderRect4, new Color(0, 200, 150));
        }
    }
}
