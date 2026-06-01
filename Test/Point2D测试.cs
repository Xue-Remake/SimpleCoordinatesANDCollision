using ConsoleUILib.UILib;
using SimpleCoordinatesANDCollision.Models;
using SimpleCoordinatesANDCollision.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Ship : PolarSpeedPoint2D
    {
        public Ship(Double x, Double y)
        {
            pointX = Math.Round(x, 4, MidpointRounding.AwayFromZero);
            pointY = Math.Round(y, 4, MidpointRounding.AwayFromZero);
            Speed = new PolarVector(0,0);
            Acceleration = new PolarVector(0, 0);
        }
        protected override void Move()
        {
            base.Move();
            pointX = Math.Round(pointX, 4, MidpointRounding.AwayFromZero);
            pointY = Math.Round(pointY, 4, MidpointRounding.AwayFromZero);
        }
        protected override void accelerate()
        {
            base.accelerate();
            Speed.R = Math.Round(Speed.R, 4, MidpointRounding.AwayFromZero);
            Speed.Theta = Math.Round(Speed.Theta, 4, MidpointRounding.AwayFromZero);
        }
    }
    public class Program
    {
        static void Main()
        {
            Ship myShip = new Ship(5, 7);

            var ShipX = new StaticText();
            var ShipY = new StaticText();

            var addAcc = new PolarVector(0.01, 0);
            var addRid = new PolarVector(0, 0.01);
            var Acc = new StaticText();
            var shipSpeed = new StaticText();

            ShipX.Bind(() => "飞船X坐标: " + myShip.pointX);
            ShipY.Bind(() => "飞船Y坐标: " + myShip.pointY);
            Acc.Bind(() => "飞船加速度: " + myShip.Acceleration);
            shipSpeed.Bind(() => "飞船速度: " + myShip.Speed);


            var inputHandler = new InputHandler() { PollingIntervalMs = (1000 / 30) };
            var dispatcher = new CommandDispatcher();
            dispatcher.Separators = new[] { ' ' };
            dispatcher.Register("w", args => { myShip.Acceleration += addAcc; return true; });
            dispatcher.Register("r", args => { myShip.Acceleration.Theta += 0.01 ; return true; });

            inputHandler.OnCommandSubmitted += cmd => dispatcher.Dispatch(cmd);

            var session = new Session(1000 / 30);
            session.Add(ShipX);
            session.Add(ShipY);
            session.Add(shipSpeed);
            session.Add(Acc);

            inputHandler.Start();
            session.Start();

            while (session.IsRunning)
            {
                myShip.update();
                Thread.Sleep(1000 / 30);
            }
        }
    }
}
