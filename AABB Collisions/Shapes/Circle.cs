using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AABB_Collisions
{
    public class Circle : Rigidbody
    {
        public float radius;

        public Circle(Vector2 pos, float radius, MassData massData, float restitution, Color color, bool useGravity = true, float gravityScale = 1) : base(pos, massData, restitution, color, useGravity, gravityScale)
        {
            this.radius = radius;
            shape = ShapeType.Circle;
        }

        public override void Draw(Texture2D texture)
        {
            Game1.instance._spriteBatch.Draw(texture, new Rectangle((int)pos.X - (int)radius, (int)pos.Y - (int)radius, texture.Width, texture.Height), Color.White);
        }

        public override void RecalculateAABB()
        {
            aabb.min = new Vector2(pos.X - radius, pos.Y - radius);
            aabb.max = new Vector2(pos.X + radius, pos.Y + radius);
        }

        public override Texture2D CreateTexture(Color color)
        {
            return Util.GetColoredCircle(radius, color);
        }
    }
}
