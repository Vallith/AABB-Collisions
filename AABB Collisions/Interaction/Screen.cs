using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
    class Screen
    {
        // Screen data
        public static int height;
        public static int width;
        public static int HalfHeight { get => height / 2; }
        public static int HalfWidth { get => width / 2; }

        // MonoGame crap
        public static GraphicsDeviceManager _graphics;
        public static SpriteBatch Draw { get { return Game1.instance._spriteBatch; } }

        public static bool isUIElement;

        /// <summary>
        /// Calculates if the mouse position is over a UI Element
        /// </summary>
        public static void TestUIElement()
        {
            isUIElement = false;
            foreach (var item in Game1.instance.uiElements)
            {
                foreach (var aabb in item.aabbs)
                {
                    if (aabb.IsInside(Input.mouse.Position.ToVector2()))
                    {
                        isUIElement = true;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Handles screen setup
        /// </summary>
        public static void Initialise()
        {
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            //_graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

    }
}
