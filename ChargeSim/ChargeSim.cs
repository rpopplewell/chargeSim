using System;
using System.Collections.Generic;

namespace ChargeSim
{
    public class ChargeSystem
    {
        /*
        * Object Section
        */

        public const double deltaT = 0.00001;
        public const double e = 1.6021766208E-19;
        public const double k = 8.9875517873681764E9;
        public const double c = 299792458;

        public double t = 0;
        public List<Charge> charges = new List<Charge>();
        public List<Boundary> bounderies = new List<Boundary>();

        // Charge

        public class Charge
        {
            public Vec acceleration = new Vec();

            public double x;
            public double y;
            public double q;
            public double m;
            public double vx = 0;
            public double vy = 0;

            public Charge(double x, double y, double q, double m) {
                this.x = x;
                this.y = y;
                this.q = q;
                this.m = m;
            }
        }

        public void NewCharge(double x, double y, double q, double m) {
            Charge charge = new Charge(x, y, q, m);
            charges.Add(charge);
        }

        // Boundary

        public class Boundary
        {
            public int leftX;
            public int rightX;
            public int topY;
            public int bottomY;

            public Boundary(int leftX, int rightX, int topY, int bottomY) {
                this.leftX = leftX;
                this.rightX = rightX;
                this.topY = topY;
                this.bottomY = bottomY;
            }
        }

        public void NewBoundary(int leftX, int rightX, int topY, int bottomY) {
            Boundary boundary = new Boundary(leftX, rightX, topY,bottomY);
            bounderies.Add(boundary);
        }

        private void RunBoundaries(Charge charge) {
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

        // Structs

        public struct Coordinates
        {
            public double x;
            public double y;
        }

        public struct Vec
        {
            public double Magnitude;
            public double Direction;
        }

        /*
        * Method Section
        */

        public List<Charge> GetSystemState() {
            return charges;
        }

        /*Vector Calculation*/

        public void UpdateSystem() {
            t = t + deltaT;
            Console.Clear();
            Vec distance = new Vec();
            Vec acceleration = new Vec();
            for (int i = 0; i < charges.Count; i++) {
                for (int j = 0; j < charges.Count; j++) {
                    if (i != j) {
                        distance = GetDistVec(charges[i], charges[j]);
                        acceleration.Magnitude = Math.Abs(((charges[i].q * charges[j].q * k) / Math.Pow(distance.Magnitude,2)) / charges[i].m);
                        acceleration.Direction = distance.Direction;
                        GetVelVec(charges[i], acceleration);
                    }
                }
            }
            for (int i = 0; i < charges.Count; i++)
            {
                Charge charge = charges[i];

                Console.WriteLine((i + 1) + ": (" + charge.x + ", " + charge.y + ")");

                // Update positions
                charge.x = charge.x + charge.vx * deltaT;
                charge.y = charge.y + charge.vy * deltaT;
                RunBoundaries(charge);
            }
            Console.WriteLine("\nt = " + Math.Round((decimal) t, 5));
        }

        private void GetVelVec(Charge p1, Vec acceleration) {
            double oldVel = Math.Sqrt(Math.Pow(p1.vx, 2) + Math.Pow(p1.vy, 2));
            double addVel = acceleration.Magnitude * deltaT;
            
            p1.vx = p1.vx + addVel * Math.Cos(acceleration.Direction);
            p1.vy = p1.vy + addVel * Math.Sin(acceleration.Direction);
        }

        private Vec GetDistVec(Charge p1, Charge p2) {
            Vec distance = new Vec();
            double tan;

            distance.Magnitude = Math.Sqrt(Math.Pow(p1.x - p2.x, 2) + Math.Pow(p1.y - p2.y, 2));

            //Special case
            if (p1.x == p2.x) {
                if (p1.y > p2.y) {
                    distance.Direction = -Math.PI / 2;
                } else {
                    distance.Direction = Math.PI / 2;
                }
            }

            else {
                tan = (p1.y - p2.y) / (p1.x - p2.x);
                distance.Direction = Math.Atan(tan);
                if (p1.x > p2.x) {
                    distance.Direction += Math.PI;
                }
            }

            if (SameSign(p1, p2)) {
                distance.Direction += Math.PI;
            }

            return distance;
        }

        private bool SameSign(Charge p1, Charge p2) {
            if (p1.q > 0 && p2.q > 0) {
                return true;
            }
            if (p1.q < 0 && p2.q < 0) {
                return true;
            }
            return false;
        }
    }
}