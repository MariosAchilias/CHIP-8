using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Chip8
{
    public class Game1 : Game
    {
        private Cpu _cpu;
        private Texture2D _texture;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1(string rom)
        {
            _cpu = new Cpu();
            // Load rom
            _cpu.load(rom);
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 640;
            _graphics.PreferredBackBufferHeight = 320;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _texture = new Texture2D(GraphicsDevice, 10, 10);
            Microsoft.Xna.Framework.Color[] color = new Microsoft.Xna.Framework.Color[100];
            for (int i = 0; i < _texture.Width; i++)
                for (int j = 0; j < _texture.Height; j++)
                    color[i * _texture.Width + j] = Microsoft.Xna.Framework.Color.White;
            _texture.SetData(color);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            _spriteBatch.Begin();
            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    if (_cpu._display[j * 32 + i])
                        _spriteBatch.Draw(_texture, new Microsoft.Xna.Framework.Rectangle(i * 10, j * 10, 10, 10), Microsoft.Xna.Framework.Color.White);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
