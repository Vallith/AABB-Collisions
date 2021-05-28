using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AABB_Collisions
{
    public class RigidRect : Rigidbody
    {

        public float width;
        public float height;
        float angle;

        public RigidRect(Vector2 pos, float width, float height, float angle, Material material, Color color, bool useGravity = true, float gravityScale = 1) : base(pos, material, color, useGravity, gravityScale)
        {
            this.width = width;
            this.height = height;
            this.angle = angle;
            this.color = color;
            shape = ShapeType.AABB;
            CalculateMass(material.density);
            RecalculateAABB();
        }

        public override Texture2D CreateTexture(Color color)
        {
            return Util.GetColoredSquare(width, height, color);
        }

        public override void Draw(Texture2D texture)
        {
            Game1.instance._spriteBatch.Draw(texture, new Rectangle((int)pos.X - texture.Width / 2, (int)pos.Y - texture.Height / 2, texture.Width, texture.Height), Color.White);
        }

        public override void RecalculateAABB()
        {
            aabb.min = new Vector2(pos.X - width / 2, pos.Y - height / 2);
            aabb.max = new Vector2(pos.X + width / 2, pos.Y + height / 2);
        }

        public override void CalculateMass(float density)
        {
            float area = width * height;
            massData = new MassData(density * area);
        }

        public override void DrawOutline()
        {
            // Top line
            Game1.instance._spriteBatch.DrawLine(aabb.min, new Vector2(aabb.max.X - outlineWidth, aabb.min.Y), InvertedColor, outlineWidth);
            // Right line
            Game1.instance._spriteBatch.DrawLine(new Vector2(aabb.max.X, aabb.min.Y), aabb.max, InvertedColor, outlineWidth);
            // Left line
            Game1.instance._spriteBatch.DrawLine(aabb.min, new Vector2(aabb.min.X, aabb.max.Y), InvertedColor, outlineWidth);
            // Bottom line
            Game1.instance._spriteBatch.DrawLine(new Vector2(aabb.min.X, aabb.max.Y), aabb.max - new Vector2(outlineWidth, 0), InvertedColor, outlineWidth);
        }

    }
}
