﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AABB_Collisions
{
    public abstract class Rigidbody
    {

        public AABB aabb;

        public MassData massData;

        public float restitution;

        static float gravity = 80;
        public float gravityScale = 1;
        public bool useGravity = true;

        public Color color;

        public Vector2 pos;
        public Vector2 vel;
        public Vector2 force;


        public Rigidbody(Vector2 pos, MassData massData, float restitution, Color color, bool useGravity = true, float gravityScale = 1)
        {
            this.pos = pos;
            this.massData = massData;
            this.restitution = restitution;
            this.gravityScale = useGravity == true ? gravityScale : 0;
            this.color = color;
        }

        public abstract void RecalculateAABB();

        public abstract void Draw(Texture2D texture);

        public abstract Texture2D CreateTexture(Color color);

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

        public void DrawVelocityVector()
        {
            Game1.instance._spriteBatch.DrawLine(pos, pos + vel, Color.White, 1);
        }

        public void CalculateForce(params Vector2[] forces)
        {
            force += Vector2.UnitY * (massData.mass * gravity);
            foreach (var item in forces)
            {
                force += item;
            }
        }

    }
}