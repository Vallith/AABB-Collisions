using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace AABB_Collisions
{
    public class Game1 : Game
    {
        // TODO:
        // Positional Correction once Manifolds are implemented
        // Calculate mass off of density and area

        const float fps = 100;
        const float dt = 1 / fps;
        float accumulator = 0;

        bool canStep;
        bool frameSteppingActive;

        float frameStart;
        float currentTime;

        public static Game1 instance;

        [DllImport("kernel32")]
        static extern bool AllocConsole();

        private GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public SpriteFont defaultFont;

        public Circle circleA;
        public Circle circleB;

        public RigidRect square;

        public RigidRect ground;

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

            _graphics.SynchronizeWithVerticalRetrace = false;

            //circleA = RigidbodyStorage.Create(new Circle(new Vector2(200, 400), 30, 0f, 0.5f, Color.Red));
            circleB = RigidbodyStorage.Create(new Circle(new Vector2(400, 200), 90, new MassData(6), 0.9f, Color.Green));
            square = RigidbodyStorage.Create(new RigidRect(new Vector2(250, 50), 80, 40, new MassData(6), 0, 0.5f, Color.Blue));
            ground = RigidbodyStorage.Create(new RigidRect(new Vector2(400, 775), 800, 50, new MassData(0), 0, 0.5f, Color.Black));

            //circleA.SetVelocity(50, 0);
            //circleB.SetVelocity(0, 500);
            //circleC.SetVelocity(0, -600);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            defaultFont = Content.Load<SpriteFont>("DefaultFont");
            // TODO: use this.Content to load your game content here
        }
        bool spaceWasPressed;
        bool fWasPressed;
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

                if (!frameSteppingActive)
                {
                    canStep = true;
                }

                if (canStep)
                {
                    foreach (var rb in RigidbodyStorage.objectList.Keys)
                    {
                        rb.CalculateForce();
                        rb.RecalculateAABB();
                        rb.Update(dt);
                        rb.force = new Vector2(0, 0);
                    }
                    for (int i = 0; i < RigidbodyStorage.objectList.Count; i++)
                    {
                        Rigidbody A = RigidbodyStorage.objectList.ElementAt(i).Key;

                        for (int j = i + 1; j < RigidbodyStorage.objectList.Count; j++)
                        {
                            Rigidbody B = RigidbodyStorage.objectList.ElementAt(j).Key;
                            if (A.massData.inverseMass == 0 && B.massData.inverseMass == 0)
                                continue;

                            Manifold m = new Manifold(A, B);
                            m.Solve();
                            m.PositionalCorrection();
                        }
                    }
                    canStep = false;
                }
                accumulator -= dt;
            }

            float alpha = accumulator / dt;
            

            if (Keyboard.GetState().IsKeyDown(Keys.F) && fWasPressed == false)
            {
                fWasPressed = true;
                frameSteppingActive = !frameSteppingActive;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.F))
            {
                fWasPressed = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && spaceWasPressed == false)
            {
                spaceWasPressed = true;
                canStep = true;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                spaceWasPressed = false;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            DrawGame(gameTime, alpha);
            base.Update(gameTime);
        }
        protected void DrawGame(GameTime gameTime, float alpha)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(defaultFont, $"{Mouse.GetState().Position}", new Vector2(20, 20), Color.Black);
            foreach (var element in RigidbodyStorage.objectList)
            {
                
                element.Key.Draw(element.Value);
                element.Key.DrawVelocityVector();
                element.Key.DrawAABB();
            }
            _spriteBatch.End();
            // TODO: Add your drawing code here

            //base.Draw(gameTime);
        }


    }
}
