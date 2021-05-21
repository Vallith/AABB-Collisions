using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System;

namespace AABB_Collisions
{
    public class Game1 : Game
    {

        public Dictionary<string, Material> Mats
        {
            get { return MaterialStorage.materialTypes; }
        }

        public Random rand = new Random();

        Point mousePoint;

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

        public Circle circleTest1;
        public Circle circleTest2;

        public RigidRect square;

        public RigidRect leftWall;
        public RigidRect rightWall;
        public RigidRect ground;

        public Texture2D squareTexture;

        public Rigidbody selectedShape;

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

            //circleTest1 = RigidbodyStorage.Create(new Circle(new Vector2(200, 400), 30, Mats["BouncyBall"], Color.Black));
            //circleTest2 = RigidbodyStorage.Create(new Circle(new Vector2(600, 400), 30, Mats["BouncyBall"], Color.White));
            //
            //circleTest1.SetVelocity(100, 0);

            leftWall = RigidbodyStorage.Create(new RigidRect(new Vector2(25, 400), 50, 800, 0, Mats["Static"], new Color(145, 136, 129)), "Left Wall");
            rightWall = RigidbodyStorage.Create(new RigidRect(new Vector2(775, 400), 50, 800, 0, Mats["Static"], new Color(145, 136, 129)), "Right Wall");
            ground = RigidbodyStorage.Create(new RigidRect(new Vector2(400, 775), 800, 50, 0, Mats["Static"], new Color(145, 136, 129)), "Ground");
            circleA = RigidbodyStorage.Create(new Circle(new Vector2(300, 100), 30, Mats["BouncyBall"], Color.Red), "Circle A");
            circleB = RigidbodyStorage.Create(new Circle(new Vector2(489, 254), 90, Mats["'Aerogel'"], new Color(85, 158, 89)), "Circle B");
            square = RigidbodyStorage.Create(new RigidRect(new Vector2(200, 0), 90, 90, 0, Mats["Metal"], new Color(41, 110, 143)), "Square");

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

        protected override void Update(GameTime gameTime)
        {
            _spriteBatch.Begin();
            Input.Process();

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

            DrawGame(gameTime);

            while (accumulator > dt)
            {

                if (!frameSteppingActive)
                {
                    canStep = true;
                }

                if (canStep)
                {

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

                        }
                    }
                    foreach (var rb in RigidbodyStorage.objectList.Keys)
                    {
                        rb.CalculateForce();
                        rb.RecalculateAABB();
                        rb.Update(dt);
                        rb.force = new Vector2(0, 0);
                    }
                    canStep = false;
                }
                accumulator -= dt;
            }


            if (Input.IsPressed(Keys.F))
            {
                frameSteppingActive = !frameSteppingActive;
            }

            if (Input.IsPressed(Keys.Space))
            {
                canStep = true;
            }

            if (Input.IsPressed(MouseButton.LeftButton) || Input.IsPressed(MouseButton.RightButton))
            {
                bool isInside = true;
                foreach (var item in RigidbodyStorage.objectList.Keys)
                {
                    if (item.aabb.IsInside(Util.PointToVector2(mousePoint)))
                    {
                        isInside = true;
                        break;
                    }
                    else
                    {
                        isInside = false;
                    }
                }
                if (!isInside && Input.IsPressed(MouseButton.LeftButton))
                {
                    RigidbodyStorage.Create(new Circle(Util.PointToVector2(mousePoint), rand.Next(10, 30), Mats["SuperBall"], new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255))));
                }
                else if (!isInside && Input.IsPressed(MouseButton.RightButton))
                {
                    RigidbodyStorage.Create(new RigidRect(Util.PointToVector2(mousePoint), rand.Next(10, 60), rand.Next(10, 60), 0, Mats["Wood"], new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255))));
                }
            }


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _spriteBatch.End();

            base.Update(gameTime);
        }
        protected void DrawGame(GameTime gameTime)
        {
            mousePoint = Mouse.GetState().Position;
            GraphicsDevice.Clear(new Color(111, 177, 199));

            

            _spriteBatch.DrawString(defaultFont, $"{Mouse.GetState().Position}", new Vector2(20, 20), Color.Black);
            foreach (var element in RigidbodyStorage.objectList)
            {
                   
                element.Key.Draw(element.Value);
                element.Key.DrawVelocityVector();
                //element.Key.DrawAABB();
                if (selectedShape != null)
                {
                    _spriteBatch.DrawString(defaultFont, selectedShape.ToString(true), new Vector2(20, 50), Color.Black);
                    selectedShape.DrawOutline();
                }
                if (Input.IsPressed(MouseButton.LeftButton))
                {
                    if (element.Key.aabb.IsInside(Util.PointToVector2(mousePoint)))
                    {
                        selectedShape = element.Key;
                    }
                }

            }
        }


    }
}
