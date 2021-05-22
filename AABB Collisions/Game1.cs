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
        // Continuous Collision Detection
        // Quad Trees

        // Game1 instance
        public static Game1 instance;

        public Dictionary<string, Material> Mats
        {
            get { return MaterialStorage.materialTypes; }
        }

        public List<UIElement> uiElements = new List<UIElement>();

        public Random rand = new Random();

        // Fixed Time Step Variables
        const float fps = 100;
        const float dt = 1 / fps;
        float accumulator = 0;

        bool canStep;
        bool frameSteppingActive;

        float frameStart;
        float currentTime;

        // Creates console
        [DllImport("kernel32")]
        static extern bool AllocConsole();

        // Handles texture changes
        public SpriteBatch _spriteBatch;
        // Base font
        public SpriteFont defaultFont;

        // Screen Bounds
        public int wallWidth = 50;
        public int halfWallWidth => wallWidth / 2;

        public QuadTree quadTree;

        public RigidRect leftWall;
        public RigidRect rightWall;
        public RigidRect ground;
        public RigidRect roof;

        // Currently selected object
        public Rigidbody selectedShape;

        // UI elements
        RadioButton drawAABBs;
        RadioButton drawVelocityVectors;
        Slider gravitySlider;
        Slider boxHeight;
        Slider boxWidth;

        public Game1()
        {
            Screen._graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            instance = this;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            AllocConsole();
            //Screen.width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            //Screen.height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Screen.width = 800;
            Screen.height = 800;
            Screen.Initialise();

            quadTree = new QuadTree(new AABB(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(Screen.width, Screen.height)), 4);

            drawAABBs = new RadioButton(new Vector2(26, 30), 15, 2);
            drawVelocityVectors = new RadioButton(new Vector2(26, 80), 15, 2);

            gravitySlider = new Slider(new Vector2(90, 120), 10, 100, -500, 500, true);
            gravitySlider.onSliderChanged += (slider, value) => { Rigidbody.gravity = value; };

            boxHeight = new Slider(new Vector2(90, 180), 10, 100, 10, 200, true);
            boxWidth = new Slider(new Vector2(90, 240), 10, 100, 10, 200, true);

            RigidbodyStorage.Create(new Circle(new Vector2(400, 400), 20, Mats["Metal"], Color.White));
            RigidbodyStorage.Create(new Circle(new Vector2(300, 400), 20, Mats["Metal"], Color.White));
            RigidbodyStorage.Create(new Circle(new Vector2(200, 400), 20, Mats["Metal"], Color.White));
            RigidbodyStorage.Create(new Circle(new Vector2(500, 400), 20, Mats["Metal"], Color.White));

            leftWall = RigidbodyStorage.Create(new RigidRect(new Vector2(halfWallWidth, Screen.HalfHeight), wallWidth, Screen.height, 0, Mats["Static"], new Color(145, 136, 129)), "Left Wall");
            rightWall = RigidbodyStorage.Create(new RigidRect(new Vector2(Screen.width - halfWallWidth, Screen.HalfHeight), wallWidth, Screen.height, 0, Mats["Static"], new Color(145, 136, 129)), "Right Wall");
            ground = RigidbodyStorage.Create(new RigidRect(new Vector2(Screen.HalfWidth, Screen.height - halfWallWidth), Screen.width, wallWidth, 0, Mats["Static"], new Color(145, 136, 129)), "Ground");
            roof = RigidbodyStorage.Create(new RigidRect(new Vector2(Screen.HalfWidth, halfWallWidth), Screen.width, wallWidth, 0, Mats["Static"], new Color(145, 136, 129)), "Roof");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            defaultFont = Content.Load<SpriteFont>("DefaultFont");
        }

        public int collisionCount;
        protected override void Update(GameTime gameTime)
        {
            _spriteBatch.Begin();
            Input.Process();
            Screen.TestUIElement();

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
                    quadTree.ClearTree();
                    foreach (var item in RigidbodyStorage.objectList.Keys)
                    {
                        quadTree.Insert(item);
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
                            collisionCount++;


                        }
                        A.CalculateForce();
                        A.RecalculateAABB();
                        A.Update(dt);
                        A.force = new Vector2(0, 0);

                    }
                    canStep = false;
                }
                accumulator -= dt;
            }

            // Controls
            if (Input.IsPressed(Keys.F))
            {
                frameSteppingActive = !frameSteppingActive;
            }

            if (Input.IsPressed(Keys.Space))
            {
                canStep = true;
            }

            if (Input.IsPressed(Keys.Delete))
            {
                if (selectedShape != null)
                {
                    RigidbodyStorage.objectList.Remove(selectedShape);
                    selectedShape = null;
                }
            }
            DrawGame(gameTime);
            if (Input.IsPressed(MouseButton.LeftButton) || Input.IsPressed(MouseButton.RightButton))
            {
                bool isInside = Screen.isUIElement;
                foreach (var item in RigidbodyStorage.objectList.Keys)
                {
                    if (item.aabb.IsInside(Input.MousePos))
                    {
                        isInside = true;
                        break;
                    }
                }

                if (!isInside && Input.IsPressed(MouseButton.LeftButton))
                {
                    RigidbodyStorage.Create(new Circle(Input.MousePos, rand.Next(10, 30), Mats["SuperBall"], new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255))));
                }
                else if (!isInside && Input.IsPressed(MouseButton.RightButton))
                {
                    RigidbodyStorage.Create(new RigidRect(Input.MousePos, boxHeight.Value, boxWidth.Value, 0, Mats["Wood"], new Color(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255))));
                }
            }

            

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            _spriteBatch.End();



            base.Update(gameTime);
        }
        protected void DrawGame(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(111, 177, 199));
            quadTree.DrawTree();
            foreach (var element in RigidbodyStorage.objectList)
            {
                   
                element.Key.Draw(element.Value);
                if (drawVelocityVectors.isTicked)
                {
                    element.Key.DrawVelocityVector();
                }
                if (drawAABBs.isTicked)
                {
                    element.Key.DrawAABB();
                }
                if (Input.IsPressed(MouseButton.LeftButton))
                {
                    if (element.Key.aabb.IsInside(Input.MousePos) && !Screen.isUIElement)
                    {
                        selectedShape = element.Key;
                    }
                }

            }

            foreach (var item in uiElements)
            {
                item.InputControl();
                item.DrawGUI();
            }

            if (selectedShape != null)
            {
                _spriteBatch.DrawString(defaultFont, selectedShape.ToString(true), new Vector2(600, 20), Color.Black);
                selectedShape.DrawOutline();
            }
            _spriteBatch.DrawString(defaultFont, $"{Mouse.GetState().Position}", new Vector2(75, 20), Color.Black);
            _spriteBatch.DrawString(defaultFont, $"RB Count: {RigidbodyStorage.objectList.Count}", new Vector2(75, 40), Color.Black);
            _spriteBatch.DrawString(defaultFont, $"Collision Count: {collisionCount}", new Vector2(75, 60), Color.Black);
            collisionCount = 0;
        }


    }
}
