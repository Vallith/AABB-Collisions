using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AABB_Collisions
{
    public abstract class Rigidbody
    {

        public float inverseMass;
        public float mass;

        public AABB aabb;

        public float restitution;

        float gravity = 80;
        public bool useGravity = true;

        public Color color;

        public Vector2 pos;
        public Vector2 vel;

        public Rigidbody(Vector2 pos, float mass, float restitution, Color color, bool useGravity = true)
        {
            this.pos = pos;
            this.restitution = restitution;
            inverseMass = mass == 0 ? 0 : 1 / mass;
            gravity = useGravity == true ? gravity : 0;
            this.mass = mass;
            this.color = color;
        }

        public abstract void RecalculateAABB();

        public abstract void Draw(Texture2D texture);

        public abstract Texture2D CreateTexture(Color color);

        public void Update(float dt)
        {
            Vector2 force = CalculateForce();
            Vector2 acceleration = new Vector2(force.X * inverseMass, force.Y * inverseMass);

            vel += acceleration * dt;
            pos += vel * dt;
        }

        public void SetVelocity(float x, float y)
        {
            vel = new Vector2(x, y);
        }

        public void DrawVelocityVector()
        {
            Game1.instance._spriteBatch.DrawLine(pos, pos + vel, Color.White, 1);
        }

        public Vector2 CalculateForce()
        {
            return new Vector2(0, mass * gravity);
        }

    }
}
