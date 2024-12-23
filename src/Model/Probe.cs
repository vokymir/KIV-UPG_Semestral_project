﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ElectricFieldVis.Model
{
    /// <summary>
    /// Probe has it's current Position,
    /// Radius on which it orbit and
    /// "speed" angularVelocity.
    /// </summary>
    public class Probe
    {
        public Vector2 position { get; set; }
        public float radius {  get; private set; }
        public float angularVelocity { get;  set; }  

        // doesn't really belongs here for the data, but for the visual representation
        public Color color { get; set; }
        // for other than main probes - to distinguish
        public int ID { get; set; }

        public Probe()
        {
            // setting default values specified in assignment
            radius = 1;
            position = new Vector2(radius,0f);
            angularVelocity = (float)(Math.PI / 6); 
            color = Color.Black;
        }

        public Probe(float X, float Y, float radius, Color clr, float angularVelocity = 0)
        {
            this.radius = radius;
            this.position = new Vector2(X, Y);
            this.color = clr;
            this.angularVelocity = angularVelocity;
        }
        public Probe(Vector2 pos, float radius, Color clr, float angularVelocity = 0)
        {
            this.radius = radius;
            this.position = pos;
            this.color = clr;
            this.angularVelocity = angularVelocity;
        }

        /// <summary>
        /// Updates it's position over time, orbiting around the origin.
        /// </summary>
        /// <param name="time">Time elapsed since the beginning.</param>
        public void UpdatePosition(float time)
        {
            float angle = angularVelocity * time;
            position = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
        }

    }
}
