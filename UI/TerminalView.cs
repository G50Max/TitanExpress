using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TitanExpress.Systems;

namespace TitanExpress.UI
{
    public class TerminalView
    {
        private readonly Terminal _terminal;
        private readonly SpriteFont _font;
        private readonly Rectangle _bounds;
        private readonly Color _bgColor = new Color(10, 10, 20);
        private readonly Color _textColor = new Color(0, 220, 150);
        private readonly Color _inputColor = new Color(0, 255, 180);
        private readonly Color _cursorColor = new Color(0, 255, 200);
        private float _cursorBlinkTimer;
        private bool _cursorVisible = true;
        private Texture2D _pixelTexture;

        public TerminalView(Terminal terminal, SpriteFont font, Rectangle bounds)
        {
            _terminal = terminal;
            _font = font;
            _bounds = bounds;
        }

        public void Initialize(GraphicsDevice graphicsDevice)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public void Update(GameTime gameTime)
        {
            _cursorBlinkTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_cursorBlinkTimer > 0.5f)
            {
                _cursorVisible = !_cursorVisible;
                _cursorBlinkTimer = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var bgRect = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, _bounds.Height);
            spriteBatch.Draw(_pixelTexture, bgRect, _bgColor);

            var borderRect = new Rectangle(_bounds.X, _bounds.Y, _bounds.Width, 2);
            spriteBatch.Draw(_pixelTexture, borderRect, new Color(0, 150, 100));

            int lineHeight = 20;
            int startY = _bounds.Y + 10;
            int x = _bounds.X + 10;

            var visibleLines = _terminal.GetVisibleLines();
            for (int i = 0; i < visibleLines.Count; i++)
            {
                var pos = new Vector2(x, startY + i * lineHeight);
                spriteBatch.DrawString(_font, visibleLines[i], pos, _textColor);
            }

            int inputY = startY + visibleLines.Count * lineHeight;
            if (inputY < _bounds.Bottom - 30)
            {
                var inputPrefix = "> ";
                var inputText = _terminal.CurrentInput;
                var prefixPos = new Vector2(x, inputY);
                spriteBatch.DrawString(_font, inputPrefix, prefixPos, _inputColor);

                var textPos = new Vector2(x + _font.MeasureString(inputPrefix).X, inputY);
                spriteBatch.DrawString(_font, inputText, textPos, _inputColor);

                if (_cursorVisible)
                {
                    var cursorX = textPos.X + _font.MeasureString(inputText).X;
                    var cursorRect = new Rectangle((int)cursorX, inputY, 2, 18);
                    spriteBatch.Draw(_pixelTexture, cursorRect, _cursorColor);
                }
            }
        }
    }
}
