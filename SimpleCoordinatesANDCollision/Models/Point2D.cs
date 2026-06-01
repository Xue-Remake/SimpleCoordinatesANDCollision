using SimpleCoordinatesANDCollision.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCoordinatesANDCollision.Models
{
    public abstract class Point2D
    {
        public Double pointX;
        public Double pointY;
        public abstract void update();
        protected abstract void Move();
        protected abstract void accelerate();
    }
    public class PolarSpeedPoint2D : Point2D
    {
        public PolarVector Speed;
        public PolarVector Acceleration;

        public override void update()
        {
            accelerate();
            Move();
        }
        protected override void Move()
        {
            pointX += (Speed.R) * Math.Cos(Speed.Theta);
            pointY += (Speed.R) * Math.Sin(Speed.Theta);
        }
        protected override void accelerate()
        {
            Speed += Acceleration;
        }
    }
}
