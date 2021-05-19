using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace AABB_Collisions
{
    public class Game1 : Game
    {
        // TODO:
        // Positional Correction once Manifolds are implemented

        const float fps = 100;
        const float dt = 1 / fps;
        float accumulator = 0;

        float frameStart;
        float currentTime;

        public static Game1 instance;

        [DllImport("kernel32")]
        static extern bool AllocConsole();

        private GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;

        public Circle circleA;
        public Circle circleB;
        public Circle circleC;

        public Texture2D circleATexture;
        public Texture2D circleBTexture;
        public Texture2D circleCTexture;

        //public RigidRect square;

        //public Texture2D squareTexture;

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

            _graphics.SynchronizeWithVerticalRetrace = false;

            //circleA = RigidbodyStorage.Create(new Circle(new Vector2(200, 400), 30, 0f, 0.5f, Color.Red));
            circleB = RigidbodyStorage.Create(new Circle(new Vector2(400, 200), 90, new MassData(3), 0.9f, Color.Green));
            circleC = RigidbodyStorage.Create(new Circle(new Vector2(400, 700), 100, new MassData(0), 0.7f, Color.Beige));
            //square = RigidbodyStorage.Create(new RigidRect(new Vector2(400, 200), 80, 40, 8, 0, 0.5f, Color.Black));

            //circleA.SetVelocity(50, 0);
            //circleB.SetVelocity(0, 100);

            //System.Console.WriteLine(circleA.vel);
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

            currentTime = (float)gameTime.TotalGameTime.TotalSeconds;


            // Store the time elapsed since the last frame began
            accumulator += currentTime - frameStart;

            // Record the starting of this frame
            frameStart = currentTime;

            // Avoid spiral of death and clamp dt, thus clamping
            // how many times the UpdatePhysics can be
            // called in a single game loop.
            if (accumulator > 0.2f)
                accumulator = 0.2f;


            while (accumulator > dt)
            {
                foreach (var rb in RigidbodyStorage.objectList.Keys)
                {
                    rb.CalculateForce();
                    rb.Update(dt);
                    rb.force = new Vector2(0, 0);
                }
                if (CollisionUtil.CirclevsCircleUnoptimized(circleC, circleB))
                {
                    System.Console.WriteLine("collided");
                    CollisionUtil.ResolveCollision(circleC, circleB);
                    //System.Console.WriteLine(circleA.vel);
                    System.Console.WriteLine(circleB.vel);
                }

                accumulator -= dt;
            }

            float alpha = accumulator / dt;



            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            DrawGame(gameTime, alpha);
            base.Update(gameTime);
        }

        protected void DrawGame(GameTime gameTime, float alpha)
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

            //base.Draw(gameTime);
        }


    }
}
