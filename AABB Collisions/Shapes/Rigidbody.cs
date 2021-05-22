using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AABB_Collisions
{
    public abstract class Rigidbody
    {
        public Color InvertedColor
        {
            get { return new Color(255 - (int)color.R, 255 - (int)color.G, 255 - (int)color.B); }
        }


        public enum ShapeType
        {
            Circle = 0,
            AABB = 1
        };

        public string name;

        public AABB aabb;

        public MassData massData;

        public Material material;

        public ShapeType shape;

        public static float gravity = 80;
        public float gravityScale = 1;
        public bool useGravity = true;

        public static int outlineWidth = 3;
        public Color color;

        public Vector2 pos;
        public Vector2 vel;
        public Vector2 force;


        public Rigidbody(Vector2 pos, Material material, Color color, bool useGravity = true, float gravityScale = 1)
        {
            aabb = new AABB();
            this.pos = pos;
            this.material = material;
            this.gravityScale = useGravity == true ? gravityScale : 0;
            this.color = color;
            RecalculateAABB();
        }

        /// <summary>
        /// Recalculates the AABB upon a change of position
        /// </summary>
        public abstract void RecalculateAABB();

        /// <summary>
        /// Draws the object
        /// </summary>
        public abstract void Draw(Texture2D texture);

        /// <summary>
        /// Creates a texture for the object
        /// </summary>
        public abstract Texture2D CreateTexture(Color color);
        /// <summary>
        /// Calculates the mass of the object based on it's area and density
        /// </summary>
        public abstract void CalculateMass(float density);
        /// <summary>
        /// Draws the outline of the object
        /// </summary>
        public abstract void DrawOutline();

        /// <summary>
        /// Updates the position of the object
        /// </summary>
        public void Update(float dt)
        {
            Vector2 acceleration = new Vector2(force.X * massData.inverseMass, force.Y * massData.inverseMass);
            vel += acceleration * dt;
            pos += vel * dt;

        }

        public void SetVelocity(float x, float y)
        {
            vel = new Vector2(x, y);
        }

        /// <summary>
        /// Draws the Velocity Vector
        /// </summary>
        public void DrawVelocityVector()
        {
            Game1.instance._spriteBatch.DrawLine(pos, pos + vel, Color.White, 1);
        }

        public void DrawAABB()
        {
            Game1.instance._spriteBatch.DrawPoint(aabb.max, Color.Red, 10);
            Game1.instance._spriteBatch.DrawPoint(aabb.min, Color.Blue, 10);
            Game1.instance._spriteBatch.DrawLine(aabb.min, new Vector2(aabb.max.X, aabb.min.Y), Color.Red);
            Game1.instance._spriteBatch.DrawLine(new Vector2(aabb.max.X, aabb.min.Y), aabb.max, Color.Red);
            Game1.instance._spriteBatch.DrawLine(aabb.min, new Vector2(aabb.min.X, aabb.max.Y), Color.Red);
            Game1.instance._spriteBatch.DrawLine(new Vector2(aabb.min.X, aabb.max.Y), aabb.max, Color.Red);
        }

        /// <summary>
        /// Applies an array of Vector2 forces to an object, including gravity
        /// </summary>
        public void CalculateForce(params Vector2[] forces)
        {
            force += Vector2.UnitY * (massData.mass * (gravity * gravityScale));
            foreach (var item in forces)
            {
                force += item;
            }
        }

        public string ToString(bool full)
        {
            return $"{name}:\n{shape}\n{pos}\n{vel}\n{massData.mass}kg\n";
        }
    }
}
