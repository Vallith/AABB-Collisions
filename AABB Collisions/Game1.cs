﻿using Microsoft.Xna.Framework;
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

        public Dictionary<string, Material> Mats
        {
            get { return MaterialStorage.materialTypes; }
        }


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

            //circleA = RigidbodyStorage.Create(new Circle(new Vector2(200, 400), 30, 0f, 0.5f, Color.Red));
            circleB = RigidbodyStorage.Create(new Circle(new Vector2(400, 200), 90, Mats["SuperBall"], new Color(85, 158, 89)), "Circle B");
            square = RigidbodyStorage.Create(new RigidRect(new Vector2(200, 0), 180, 180, 0, Mats["Metal"], new Color(41, 110, 143)), "Square");
            ground = RigidbodyStorage.Create(new RigidRect(new Vector2(400, 775), 800, 50, 0, Mats["Static"], new Color(145, 136, 129)));

            //circleA.SetVelocity(50, 0);
            //circleB.SetVelocity(0, 1000);
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
            

            if (Input.IsPressed(Keys.F))
            {
                frameSteppingActive = !frameSteppingActive;
            }

            if (Input.IsPressed(Keys.Space))
            {
                canStep = true;
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            DrawGame(gameTime, alpha);
            base.Update(gameTime);
        }
        protected void DrawGame(GameTime gameTime, float alpha)
        {
            GraphicsDevice.Clear(new Color(111, 177, 199));

            _spriteBatch.Begin();

            _spriteBatch.DrawString(defaultFont, $"{Mouse.GetState().Position}", new Vector2(20, 20), Color.Black);
            foreach (var element in RigidbodyStorage.objectList)
            {
                   
                element.Key.Draw(element.Value);
                element.Key.DrawVelocityVector();
                //element.Key.DrawAABB();
                if (Input.IsPressed(MouseButton.LeftButton))
                {
                    Point mousePoint = Mouse.GetState().Position;
                    foreach (var item in RigidbodyStorage.objectList.Keys)
                    {
                        if (item.aabb.IsInside(Util.PointToVector2(mousePoint)))
                        {
                            selectedShape = item;
                        }
                    }
                }
                if (selectedShape != null)
                {
                    _spriteBatch.DrawString(defaultFont, selectedShape.ToString(true), new Vector2(20, 50), Color.Black);
                    selectedShape.DrawOutline();
                }
            }
            _spriteBatch.End();
        }


    }
}
