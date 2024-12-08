using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Multiplayer_Games_Programming_Framework.Core.Utilities;
using Multiplayer_Games_Programming_Framework.GameCode;

namespace Multiplayer_Games_Programming_Framework.Core
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager m_Graphics;
        public SpriteBatch m_SpriteBatch { get; private set; }

        SceneManager m_SceneManager;

        public MainGame()
        {
            m_Graphics = new GraphicsDeviceManager(this);
            m_Graphics.PreferredBackBufferWidth = Graphics.InitScreenWidth;
            m_Graphics.PreferredBackBufferHeight = Graphics.InitScreenHeight;
            IsFixedTimeStep = true;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Graphics.GraphicsDevice = m_Graphics.GraphicsDevice;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);
            m_SceneManager = new SceneManager(this);
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            m_SceneManager.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            //changed the sampler so that pixel art actually looks good
            m_SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: Camera.m_Matrix, samplerState: SamplerState.PointClamp);

            m_SceneManager.Draw(deltaTime);

            m_SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}




