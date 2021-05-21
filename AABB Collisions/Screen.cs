using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
    class Screen
    {
        public int height;
        public int width;

        public static SpriteBatch Draw { get { return Game1.instance._spriteBatch; } }
    }
}
