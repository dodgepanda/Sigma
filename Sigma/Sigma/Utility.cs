using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sigma
{
    class Circle
    {
        public int xPos, yPos, radius;
        public bool centered = true;

        public Circle(int x, int y, int r)
        {
            xPos = x;
            yPos = y;
            radius = r;
        }

        public Microsoft.Xna.Framework.Vector2 getCenter()
        {
            if(!centered)
                return new Microsoft.Xna.Framework.Vector2(xPos + radius, yPos + radius);
            return new Microsoft.Xna.Framework.Vector2(xPos, yPos);
        }

        public bool intersects(Circle c)
        {
            double distance = Math.Sqrt(Math.Pow(getCenter().X - c.getCenter().X, 2) + Math.Pow(getCenter().Y - c.getCenter().Y, 2));
            return radius + c.radius > distance;
        }
    }
    class Utility
    {
        //public static bool Intersect(Circle c, Rectangle r)
        //{
        //    return PointInRectangle(c.getCenter(), r)||
        //           LineIntersectCircle(new Vector2(r.X, r.Y), new Vector2(r.X+r.Width, r.Y), c) ||   //a,b
        //           LineIntersectCircle(new Vector2(r.X + r.Width, r.Y), new Vector2(r.X + r.Width, r.Y+r.Height), c) ||  //b,c
        //           LineIntersectCircle(new Vector2(r.X + r.Width, r.Y + r.Height), new Vector2(r.X, r.Y+r.Height), c) ||  //c,d
        //           LineIntersectCircle(new Vector2(r.X, r.Y + r.Height), new Vector2(r.X, r.Y), c);    //d,a
        //}
        //public static bool PointInRectangle(Vector2 point, Rectangle r)
        //{
        //    Rectangle pointRect = new Rectangle((int)point.X, (int)point.Y, 1, 1);
        //    return pointRect.Intersects(r);
        //}
        //public static bool LineIntersectCircle(Vector2 pointA, Vector2 pointB, Circle c)
        //{
        //    float dx, dy, dr, D;
        //    dx = pointB.X - pointA.X;
        //    dy = pointB.Y - pointA.Y;
        //    dr = (float)Math.Sqrt(dx * dx + dy * dy);
        //    D = (pointA.X * pointB.Y) - (pointB.X * pointA.Y);
        //    float discriminant = (float)(Math.Pow(c.radius, 2) * Math.Pow(dr, 2) - Math.Pow(D, 2));
        //    return discriminant >= 0;
        //}
    }
}
