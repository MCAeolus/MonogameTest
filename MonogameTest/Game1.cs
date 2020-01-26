using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonogameTest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameBoard board;
        string rlativeLocation = "null";
        SpriteFont defaultFont;

        public static bool gameover
        {
            private set;
            get;
        }

        public static Random random = new Random();


        //DrawingUtility drawUtil;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            random = new Random();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //drawUtil = new DrawingUtility(Content);
            board = new GameBoard(Content);
            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            defaultFont = Content.Load<SpriteFont>("DefaultFont");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState mouseState = Mouse.GetState();
            Vector2 relPosition = board.getApproximateRelativePosition(mouseState.Position.ToVector2());

            rlativeLocation = relPosition.ToString();

            board.MoveTrayPiece(mouseState);

            //board.DoGameUpdate();


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        public static void setGameOver()
        {
            gameover = true;
            //goto home screen or smthing
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(DrawingUtility.ColorFromHex("f0e3ff")) ;

            spriteBatch.Begin();

            board.DrawBoard(spriteBatch);
            //other drawing

            //spriteBatch.DrawString(defaultFont, rlativeLocation, new Vector2(0, 0), Color.Red);

            spriteBatch.End();
            /*
            spriteBatch.Begin();

            Vector2 spriteOrigin = new Vector2(50, 50); //top left of sprite
            Color tint = Color.Yellow;

            spriteBatch.Draw(blockTexture, spriteOrigin, tint);

            spriteBatch.End();
            */

            base.Draw(gameTime);
        }
    }
}
