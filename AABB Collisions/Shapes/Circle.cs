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

        public Circle(Vector2 pos, float radius, Material material, Color color, bool useGravity = true, float gravityScale = 1) : base(pos, material, color, useGravity, gravityScale)
        {
            this.radius = radius;
            shape = ShapeType.Circle;
            CalculateMass(material.density);
        }

        public override void Draw(Texture2D texture)
        {
            Game1.instance._spriteBatch.Draw(texture, new Rectangle((int)pos.X - (int)radius, (int)pos.Y - (int)radius, texture.Width, texture.Height), Color.White);
            //Game1.instance._spriteBatch.DrawString(Game1.instance.defaultFont, name, pos, Color.Red);
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

        public override void CalculateMass(float density)
        {
            float area = (float)Math.PI * (radius * radius);
            massData = new MassData(density * area);
        }

        public override void DrawOutline()
        {
            Game1.instance._spriteBatch.DrawCircle(pos, radius, 30, InvertedColor, outlineWidth);
        }

    }
}
