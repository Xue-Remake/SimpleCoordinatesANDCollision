using ConsoleUILib.UILib;
using SimpleCoordinatesANDCollision.Models;
using SimpleCoordinatesANDCollision.Vector;

namespace Test
{
    public struct ShipData
    {
        int _tic;
        Double X;
        Double Y;
        Double SpeedR;
        Double SpeedD;
        Double AccR;
        Double AccD;
        public ShipData(int tic, Double x, Double y, Double sr, Double sd, Double ar, Double ad)
        {
            _tic = tic;
            X = x;
            Y = y;
            SpeedR = sr;
            SpeedD = sd;
            AccR = ar;
            AccD = ad;
        }
        public override string ToString()
        {
            return $"tic: {_tic}\r\nShipX: {X}\r\nShipY: {Y}\r\nSpeedR: {SpeedR}\r\nSpeedD: {SpeedD}" + Environment.NewLine + $"AccR: {AccR}\r\nAccD: {AccD}" + Environment.NewLine;
        }
    }
    public class 线性阻尼模型测试
    {
        static void Main(string[] args)
        {
            LinearDamping ship = new LinearDamping(new PolarVector(0, 0), new PolarVector(0, 0), 0.5, 0.02);

            int[] tics = { 1, 200, 400 };
            PolarVector[] Accelerations = { new(0.008, 0), new(0.007, (0.5) * Math.PI), new(0.01, (1) * Math.PI) };
            int tic = 0;


            Session session = new Session(1000 / 30);
            var ShipX = new StaticText();
            var ShipY = new StaticText();
            var ShipSpeedR = new StaticText();
            var ShipSpeedDirection = new StaticText();
            var ShipAccelerationR = new StaticText();
            var ShipAccelerationDirection = new StaticText();
            var Tick = new StaticText();

            ShipX.Bind(() => ship.pointX);
            ShipY.Bind(() => ship.pointY);
            ShipSpeedR.Bind(() => ship.Speed.R);
            ShipSpeedDirection.Bind(() => ship.Speed.Theta);
            ShipAccelerationR.Bind(() => ship.Acceleration.R);
            ShipAccelerationDirection.Bind(() => ship.Acceleration.Theta);
            Tick.Bind(() => tic);

            session.Add(ShipX);
            session.Add(ShipY);
            session.Add(ShipSpeedR);
            session.Add(ShipSpeedDirection);
            session.Add(ShipAccelerationR);
            session.Add(ShipAccelerationDirection);
            session.Add(Tick);

            session.Start();

            List<ShipData> DataList = new();

            while (session.IsRunning)
            {
                tic++;

                for (int i = 0; i < 3; i++)
                {
                    if (tic == tics[i])
                        ship.Acceleration = Accelerations[i];
                }
                ship.Update();
                if (tic % 10 == 0)
                {
                    DataList.Add(new ShipData(tic, ship.pointX, ship.pointY, ship.Speed.R, ship.Speed.Theta, ship.Acceleration.R, ship.Acceleration.Theta));
                }
                if (tic == 600)
                {
                    break;
                }
                Thread.Sleep(1000 / 30);
            }
            session.Stop();
            Console.Clear();

            Console.WriteLine($"实验参数：最大速度：{ship.MaxSpeed},阻尼系数：{ship.DampingCoefficient}");
            Console.WriteLine();
            int M = DataList.Count();
            for (int i = 0; i < M; i++)
            {
                Console.WriteLine(DataList[i].ToString());
            }
        }
    }
}
