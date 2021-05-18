using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace AABB_Collisions
{
    public class Game1 : Game
    {

        public static Game1 instance;

        [DllImport("kernel32")]
        static extern bool AllocConsole();

        private GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;

        public Circle circleA;
        public Circle circleB;

        public Texture2D circleATexture;
        public Texture2D circleBTexture;

        public RigidRect square;

        public Texture2D squareTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            instance = this;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            AllocConsole();
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();
            
            circleA = RigidbodyStorage.Create(new Circle(new Vector2(200, 400), 30, 3, 0.5f, Color.Red));
            circleB = RigidbodyStorage.Create(new Circle(new Vector2(600 - 60, 400), 90, 9, 0.5f, Color.Green));
            square = RigidbodyStorage.Create(new RigidRect(new Vector2(400, 200), 80, 40, 8, 0, 0.5f, Color.Black));

            circleA.SetVelocity(50, 0);
            circleB.SetVelocity(-50, 0);

            System.Console.WriteLine(circleA.vel);
            System.Console.WriteLine(circleB.vel);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var rb in RigidbodyStorage.objectList.Keys)
            {
                rb.Update(deltaTime);
            }

            if (CollisionUtil.CirclevsCircleUnoptimized(circleA, circleB))
            {
                System.Console.WriteLine("collided");
                CollisionUtil.ResolveCollision(circleA, circleB);
                System.Console.WriteLine(circleA.vel);
                System.Console.WriteLine(circleB.vel);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            foreach (var element in RigidbodyStorage.objectList)
            {
                element.Key.Draw(element.Value);
                element.Key.DrawVelocityVector();
            }
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }


    }
}
