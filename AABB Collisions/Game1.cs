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
        // TODO:
        // Positional Correction once Manifolds are implemented
        // Calculate mass off of density and area

        public Dictionary<string, Material> Mats
        {
            get { return MaterialStorage.materialTypes; }
        }

        Quadtree quad;

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
            Random rand = new Random();
            AllocConsole();
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();

            _graphics.SynchronizeWithVerticalRetrace = false;

            //circleA = RigidbodyStorage.Create(new Circle(new Vector2(200, 400), 30, 0f, 0.5f, Color.Red));
            circleB = RigidbodyStorage.Create(new Circle(new Vector2(400, 200), 90, Mats["SuperBall"], Color.Green), "Circle B");
            square = RigidbodyStorage.Create(new RigidRect(new Vector2(200, 0), 180, 180, 0, Mats["Metal"], Color.Blue), "Square");
            ground = RigidbodyStorage.Create(new RigidRect(new Vector2(400, 775), 800, 50, 0, Mats["Static"], Color.Black));


            for (int i = 0; i < 10; i++)
            {
                if (rand.Next(1, 100) > 50)
                {
                    Vector2 randPos = new Vector2(rand.Next(0, 800), rand.Next(0, 800));
                    RigidbodyStorage.Create(new RigidRect(randPos, rand.Next(1, 120), rand.Next(1, 120), 0, Mats.ElementAt(rand.Next(0, Mats.Count - 2)).Value, new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255))));
                }
                else
                {
                    Vector2 randPos = new Vector2(rand.Next(0, 800), rand.Next(0, 800));
                    RigidbodyStorage.Create(new Circle(randPos, rand.Next(1, 120), Mats.ElementAt(rand.Next(0, Mats.Count - 2)).Value, new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255))));
                }
            }
            quad = new Quadtree(0, new AABB(0,0,800, 800));

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
            quad.Clear();
            for (int i = 0; i < RigidbodyStorage.bodies.Count; i++)
            {
                quad.insert(RigidbodyStorage.bodies[i]);
            }

            List<Rigidbody> returnObjects = new List<Rigidbody>();
            for (int i = 0; i < RigidbodyStorage.bodies.Count; i++)
            {
                returnObjects.Clear();
                quad.Retrieve(returnObjects, RigidbodyStorage.bodies[i]);

                for (int x = 0; x < returnObjects.Count; x++)
                {
                    // Run collision detection algorithm between objects
                }
            }


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
                //element.Key.DrawVelocityVector();
                //element.Key.DrawAABB();
                if (string.IsNullOrEmpty(element.Key.name) == false)
                {
                    _spriteBatch.DrawString(defaultFont, element.Key.ToString(true), element.Key.pos, Color.Black);
                }
            }

            DrawQuadtree(quad);
            _spriteBatch.End();
            // TODO: Add your drawing code here

            //base.Draw(gameTime);
        }


        public void DrawQuadtree(Quadtree tree)
        {
            Util.DrawAABB(tree.bounds);
            foreach (var item in tree.nodes)
            {
                if (item != null)
                {
                    DrawQuadtree(item);
                }
            }
        }

    }
}
