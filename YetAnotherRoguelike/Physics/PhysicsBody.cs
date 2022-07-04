using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace YetAnotherRoguelike
{
    class PhysicsBody
    {
        public Vector2 velocity;
        public float mass, friction, maxVelocity;
        bool velocityClamped;

        public PhysicsBody(Vector2 _vel, float _mass = 1, float _friction = 0.5f, float maxVel = -1f)
        {
            velocity = _vel;
            mass = _mass;
            friction = _friction;

            maxVelocity = maxVel;
            velocityClamped = maxVel != -1f;
        }

        public void Update()
        {
            velocity *= friction / mass;
            if (velocityClamped)
            {
                velocity = new Vector2(
                    Math.Clamp(velocity.X, -maxVelocity, maxVelocity),
                    Math.Clamp(velocity.Y, -maxVelocity, maxVelocity)
                    );
            }
        }
    }
}
