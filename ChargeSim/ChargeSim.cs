using System;
using System.Collections.Generic;

namespace ChargeSim
{
    public class ChargeSystem
    {
        public const double deltaT = 0.00000001;
        public const double k = 8.9875517873681764E9;

        Vec distance;
        Vec acceleration1;
        Vec acceleration2;
        Coordinates accel1;
        Coordinates accel2;
        double c = 2000;
        double f;

        public double t = 0;
        public List<Charge> charges = new List<Charge>();
        public List<Boundary> bounderies = new List<Boundary>();

        public List<Charge> GetSystemState()
        {
            return charges;
        }

        /*Charge Object Section*/

        public class Charge
        {
            public Vec acceleration = new Vec();

            public double x;
            public double y;
            public double q;
            public double m;
            public double vx = 0;
            public double vy = 0;

            public Charge(double x, double y, double q, double m)
            {
                this.x = x;
                this.y = y;
                this.q = q;
                this.m = m;
            }
        }

        public void NewCharge(double x, double y, double q, double m)
        {
            Charge charge = new Charge(x, y, q, m);
            charges.Add(charge);
        }

        /*Boundary Object Section*/

        public class Boundary
        {
            public int leftX;
            public int rightX;
            public int topY;
            public int bottomY;

            public Boundary(int leftX, int rightX, int bottomY, int topY)
            {
                this.leftX = leftX;
                this.rightX = rightX;
                this.bottomY = bottomY;
                this.topY = topY;
            }
        }

        public void NewBoundary(int leftX, int rightX, int bottomY, int topY)
        {
            Boundary boundary = new Boundary(leftX, rightX, bottomY, topY);
            bounderies.Add(boundary);
        }

        private void RunBoundaries(Charge charge)
        {
            for (int i = 0; i < bounderies.Count; i++) {
                Boundary boundary = bounderies[i];
                if (charge.x > boundary.rightX) {
                    charge.x = boundary.rightX;
                }
                if (charge.x < boundary.leftX) {
                    charge.x = boundary.leftX;
                }
                if (charge.y > boundary.topY) {
                    charge.y = boundary.topY;
                }
                if (charge.y < boundary.bottomY) {
                    charge.y = boundary.bottomY;
                }
            }
        }

        /*Vector Structs*/

        public struct Coordinates
        {
            public double x;
            public double y;
        }

        private Coordinates NewCoordinates(double x, double y)
        {
            Coordinates coords;
            coords.x = x;
            coords.y = y;
            return coords;
        }

        public struct Vec
        {
            public double Magnitude;
            public double Direction;
        }

        private Vec NewVector(double magnitude, double direction)
        {
            Vec vector;
            vector.Magnitude = magnitude;
            vector.Direction = direction;
            return vector;
        }

        /*
        * Method Section
        */

        public void UpdateSystem()
        {
            t = t + deltaT;
            // Calculate forces on charges

            for (int i = 0; i < charges.Count; i++) {
                for (int j = i + 1; j < charges.Count; j++) {

                    distance = GetDistVec(charges[i], charges[j]);
                    f = charges[i].q * charges[j].q * k / (distance.Magnitude * distance.Magnitude);

                    acceleration1.Magnitude = Math.Abs(f / charges[i].m);
                    acceleration2.Magnitude = Math.Abs(f / charges[j].m);
                    acceleration1.Direction = distance.Direction;
                    acceleration2.Direction = distance.Direction += Math.PI;

                    // DAMPENING
                    accel1 = PolarToComponent(acceleration1);
                    accel2 = PolarToComponent(acceleration2);

                    accel1.x -= c * charges[i].vx;
                    accel1.y -= c * charges[i].vy;
                    accel2.x -= c * charges[j].vx;
                    accel2.y -= c * charges[j].vy;
                    // DAMPENING

                    SetVelVec(charges[i], accel1);
                    SetVelVec(charges[j], accel2);
                }
            }
            // Update positions on charges

            for (int i = 0; i < charges.Count; i++)
            {
                //Console.WriteLine((i + 1) + ": (" + charge.x + ", " + charge.y + ")");
                charges[i].x = charges[i].x + charges[i].vx * deltaT;
                charges[i].y = charges[i].y + charges[i].vy * deltaT;
                RunBoundaries(charges[i]);
            }
            Console.Clear();
            Console.Write("t = " + Math.Round((decimal) t, 5));
        }

        /*Sets the velocity vector of a given charge*/

        private void SetVelVec(Charge p1, Vec acceleration)
        {
            Coordinates accel = PolarToComponent(acceleration);
            p1.vx = p1.vx + accel.x * deltaT;
            p1.vy = p1.vy + accel.y * deltaT;
        }

        private void SetVelVec(Charge p1, Coordinates accel)
        {
            p1.vx = p1.vx + accel.x * deltaT;
            p1.vy = p1.vy + accel.y * deltaT;
        }

        /*Returns the distance between two charges*/

        private Vec GetDistVec(Charge p1, Charge p2)
        {
            Coordinates pos1 = NewCoordinates(p1.x, p1.y);
            Coordinates pos2 = NewCoordinates(p2.x, p2.y);
            Vec distance = ComponentToPolar(pos1, pos2);
            if (SameSign(p1.q, p2.q)) {
                distance.Direction += Math.PI;
            }
            return distance;
        }

        /*Sets the velocity vector of a given charge*/

        private bool SameSign(double a, double b)
        {
            if (a > 0 && b > 0) {
                return true;
            }
            if (a < 0 && b < 0) {
                return true;
            }
            return false;
        }

        /*Vector Conversions*/

        private Coordinates PolarToComponent(Vec velVec)
        {
            Coordinates coords;
            coords.x = velVec.Magnitude * Math.Cos(velVec.Direction);
            coords.y = velVec.Magnitude * Math.Sin(velVec.Direction);
            return coords;
        }

        /*Calculates polar vector from coords1 to coords2*/

        private Vec ComponentToPolar(Coordinates coords1, Coordinates coords2)
        {
            Vec vector = new Vec();
            double tan;

            vector.Magnitude = Math.Sqrt(Math.Pow(coords1.x - coords2.x, 2) + Math.Pow(coords1.y - coords2.y, 2));
            // Special case, deltaX == 0
            if (coords1.x == coords2.x)
            {
                if (coords1.y > coords2.y)
                {
                    vector.Direction = -Math.PI / 2;
                }
                else
                {
                    vector.Direction = Math.PI / 2;
                }
            }
            else {
                tan = (coords1.y - coords2.y) / (coords1.x - coords2.x);
                vector.Direction = Math.Atan(tan);
                if (coords1.x > coords2.x)
                {
                    vector.Direction += Math.PI;
                }
            }
            return vector;
        }

        /*Calculates */

        Vec ComponentToPolar(Coordinates coords)
        {
            Coordinates nullCoords = NewCoordinates(0, 0);
            return ComponentToPolar(coords, nullCoords);
        }

        public void LogData() {
            string data = "";
            for (int i = 0; i < charges.Count - 1; i++) {
                Vec dist = GetDistVec(charges[i], charges[i+1]);
                double lambda = 1 / dist.Magnitude;
                data = data + (charges[i].x + ", " + lambda.ToString() + ", " + "\"color\", \"k\", ");
            }
            System.IO.File.WriteAllText(@"C:\Users\Public\TestFolder\chargedata.txt", data);
        }

    }
}