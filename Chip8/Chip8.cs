using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Chip8
{
    public class Chip8 : Game
    {
        private Cpu _cpu;
        private Texture2D _texture;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Chip8(string rom)
        {
            _cpu = new Cpu();
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

            var kb = Keyboard.GetState();

            _cpu.keypad[0] = kb.IsKeyDown(Keys.NumPad0);
            _cpu.keypad[1] = kb.IsKeyDown(Keys.NumPad1);
            _cpu.keypad[2] = kb.IsKeyDown(Keys.NumPad2);
            _cpu.keypad[3] = kb.IsKeyDown(Keys.NumPad3);
            _cpu.keypad[4] = kb.IsKeyDown(Keys.NumPad4);
            _cpu.keypad[5] = kb.IsKeyDown(Keys.NumPad5);
            _cpu.keypad[6] = kb.IsKeyDown(Keys.NumPad6);
            _cpu.keypad[7] = kb.IsKeyDown(Keys.NumPad7);
            _cpu.keypad[8] = kb.IsKeyDown(Keys.NumPad8);
            _cpu.keypad[9] = kb.IsKeyDown(Keys.NumPad9);
            _cpu.keypad[0xA] = kb.IsKeyDown(Keys.A);
            _cpu.keypad[0xB] = kb.IsKeyDown(Keys.B);
            _cpu.keypad[0xC] = kb.IsKeyDown(Keys.C);
            _cpu.keypad[0xD] = kb.IsKeyDown(Keys.D);
            _cpu.keypad[0xE] = kb.IsKeyDown(Keys.E);
            _cpu.keypad[0xF] = kb.IsKeyDown(Keys.F);

            _cpu.cycle();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);

            _spriteBatch.Begin();
            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (_cpu.display[j * 64 + i])
                        _spriteBatch.Draw(_texture, new Microsoft.Xna.Framework.Rectangle(i * 10, j * 10, 10, 10), Microsoft.Xna.Framework.Color.White);
                }
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
