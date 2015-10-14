using System;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.DirectX;
using System.Text;

namespace founded2sharp
{
    public  struct GameDefinition //game definition
    {
        public static int width;
        public static int height;
        public static bool window;
        public static IntPtr hwnd;
        public static bool music;
        public static bool sounds;
        public static bool isometric;

        public  GameDefinition(int width, int height, bool window, IntPtr hwnd, bool music, bool sounds, bool isometric)
        {
            GameDefinition.width = width;
            GameDefinition.height = height;
            GameDefinition.window = window;
            GameDefinition.hwnd = hwnd;
            GameDefinition.music = true;
            GameDefinition.sounds = sounds;
            GameDefinition.isometric = isometric;
        }
               

    }

    public struct PolygonCollisionResult
    {
        public bool WillIntersect; // Are the polygons going to intersect forward in time?
        public bool Intersect; // Are the polygons currently intersecting
        public Vector MinimumTranslationVector; // The translation to apply to polygon A to push the polygons appart.
     
    }

    public class c_engine
    {
        /// <summary>
        /// //////debug stuff
        /// </summary>
        public float a;
        public float b;

        public c_engine() { }

        // Check if polygon A is going to collide with polygon B for the given velocity
        public PolygonCollisionResult PolygonCollision(Polygon polygonA, Polygon polygonB, Vector velocity)
        {
            PolygonCollisionResult result   = new PolygonCollisionResult();
            result.Intersect                = false;
            result.WillIntersect            = true;
            
            int edgeCountA                  = polygonA.Edges.Count;
            int edgeCountB                  = polygonB.Edges.Count;
            float minIntervalDistance       = float.PositiveInfinity;
            Vector translationAxis          = new Vector();
            Vector edge;

            // Loop through all the edges of both polygons
            for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++)
            {
                if (edgeIndex < edgeCountA)
                {
                    edge = polygonA.Edges[edgeIndex];
                }
                else
                {
                    edge = polygonB.Edges[edgeIndex - edgeCountA];
                }

                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                Vector axis = new Vector(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjectPolygon(axis, polygonA, ref minA, ref maxA);
                ProjectPolygon(axis, polygonB, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                //if (IntervalDistance(minA, maxA, minB, maxB) > 0) result.Intersect = false;
             
               

                // ===== 2. Now find if the polygons *will* intersect =====

                // Project the velocity on the current axis
                float velocityProjection = axis.DotProduct(velocity);

                // Get the projection of polygon A during the movement
                if (velocityProjection < 0)
                {
                    minA += velocityProjection;
                }
                else
                {
                    maxA += velocityProjection;
                }

                // Do the same test as above for the new projection
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);

               

                if (intervalDistance > 0) result.WillIntersect = false;

                // If the polygons are not intersecting and won't intersect, exit the loop
                if (!result.Intersect && !result.WillIntersect) break;

                // Check if the current interval distance is the minimum one. If so store
                // the interval distance and the current distance.
                // This will be used to calculate the minimum translation vector
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector d = polygonA.Center - polygonB.Center;
                    if (d.DotProduct(translationAxis) < 0) translationAxis = -translationAxis;
                }
            }

            // The minimum translation vector can be used to push the polygons appart.
            // First moves the polygons by their velocity
            // then move polygonA by MinimumTranslationVector.
            if (result.WillIntersect) result.MinimumTranslationVector = translationAxis * minIntervalDistance;

            return result;
        }

        // Calculate the distance between [minA, maxA] and [minB, maxB]
        // The distance will be negative if the intervals overlap
        public float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
            {
                return minB - maxA;
            }
            else
            {
                return minA - maxB;
            }
        }

        // Calculate the projection of a polygon on an axis and returns it as a [min, max] interval
        public void ProjectPolygon(Vector axis, Polygon polygon, ref float min, ref float max)
        {
            // To project a point on an axis use the dot product
            float d = axis.DotProduct(polygon.Points[0]);
            min = d;
            max = d;
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                d = polygon.Points[i].DotProduct(axis);
                if (d < min)
                {
                    min = d;
                }
                else
                {
                    if (d > max)
                    {
                        max = d;
                    }
                }
            }
        }

        public bool m_rect_Collision(ref RectangleF rectA, ref RectangleF rectB)
        {
            if (rectA.X > rectB.X && rectA.X < rectB.X + rectB.Width || rectA.X < rectB.X && rectA.X + rectA.Width > rectB.X)
            {

                if (rectA.Y > rectB.Y && rectA.Y < rectB.Y + rectB.Height || rectA.Y < rectB.Y && rectA.Y + rectA.Height > rectB.Y)
                {
                    return true;
                }
            }
            return false;
        }

        public bool punkt_rect_Collision(ref RectangleF rect, PointF point)
        {
            if (point.X >= rect.X && point.X <= rect.X + rect.Width)
            {
                if (point.Y >= rect.Y && point.Y <= rect.Y + rect.Height)
                {
                    return true;
                }
            }
            return false;
        }

        public double m_rect_Distanz(ref RectangleF rectA, ref RectangleF rectB)
        {
            double difX = (rectA.X + rectA.Width/2) - (rectB.X + rectB.Width/2);
            if (difX < 0) difX = -difX;
            double difY = (rectB.Y + rectB.Height / 2) - (rectA.Y + rectA.Height / 2);
            if (difY < 0) difY = -difY;
            return Math.Sqrt((difX * difX ) + (difY * difY));
        }

        public double PointToPoint(PointF p1, PointF p2)
        {
            double a = p2.X - p1.X;
            double b = p2.Y - p1.Y;
            return Math.Sqrt(a * a + b * b);
        }

        public PointF PointFToPointF(PointF p1, PointF p2)
        {
            float a = p2.X - p1.X;
            float b = p2.Y - p1.Y;
            return new PointF(a, b);
        }

        public Point PointToPoint(Point p1, Point p2)
        {
            int a = p2.X - p1.X;
            int b = p2.Y - p1.Y;
            return new Point(a,b);
        }

        public Point vectorToPoint(Vector3 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }

        public bool behind(PointF zentrum, PointF A, PointF B)
        {
            PointF P = new PointF(zentrum.X  / 2, zentrum.Y  / 2);

            double AB = this.PointToPoint(A, B);
            double AP = this.PointToPoint(A, P);
            double BP = this.PointToPoint(B, P);
            double ABPA = (AB + AP + BP) / 2;

            float first = (float)(2 * Math.Sqrt(ABPA * (ABPA - AB) * (ABPA - AP) * (ABPA - BP)) / AB);

            P = new PointF(zentrum.X  / 2, 2 + zentrum.Y  / 2);

            AP = this.PointToPoint(A, P);
            BP = this.PointToPoint(B, P);
            ABPA = (AB + AP + BP) / 2;

            float second = (float)(2 * Math.Sqrt(ABPA * (ABPA - AB) * (ABPA - AP) * (ABPA - BP)) / AB);

            if (second < first)
                return true;
            else
                return false;
        }

        public PointF m_inner_bounce(ref RectangleF rectA, float spdX, float spdY, ref RectangleF rectB)
        {
            if (rectA.X <= rectB.X || (rectA.X + rectA.Width) >= (rectB.X + rectB.Width))
            {
                spdX = -spdX;
            }
            if (rectA.Y <= rectB.Y || (rectA.Y + rectA.Height) >= (rectB.Y + rectB.Height))
            {
                spdY = -spdY;
            }
            return (new PointF(spdX, spdY));
        }

        public RectangleF RectF_Scale(RectangleF rect, float scalex, float scaley)
        {
            return new RectangleF(rect.X * scalex, rect.Y * scaley, rect.Width * scalex,rect.Height*scaley);
        }

        public Rectangle RectF_Convert(RectangleF rect)
        {
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }

    }

    public class Polygon
    {

        private List<Vector> points = new List<Vector>();
        private List<Vector> edges = new List<Vector>();

        public bool intersect(List<Vector> poly)
        {
            RectangleF box = new RectangleF(points[0].X, points[0].Y, points[1].X - points[0].X, points[2].Y - points[0].Y);
            RectangleF box2 = new RectangleF(poly[0].X, poly[0].Y, poly[1].X - poly[0].X, poly[2].Y - poly[0].Y);
            
            return box.IntersectsWith(box2);
        }

        public void BuildEdges()
        {
            Vector p1;
            Vector p2;
            edges.Clear();
            for (int i = 0; i < points.Count; i++)
            {
                p1 = points[i];
                if (i + 1 >= points.Count)
                {
                    p2 = points[0];
                }
                else
                {
                    p2 = points[i + 1];
                }
                edges.Add(p2 - p1);
            }
        }

        public List<Vector> Edges
        {
            get { return edges; }
        }

        public List<Vector> Points
        {
            get { return points; }
        }

        public Vector Center
        {
            get
            {
                float totalX = 0;
                float totalY = 0;
                for (int i = 0; i < points.Count; i++)
                {
                    totalX += points[i].X;
                    totalY += points[i].Y;
                }

                return new Vector(totalX / (float)points.Count, totalY / (float)points.Count);
            }
        }

        public void Offset(Vector v)
        {
            Offset(v.X, v.Y);
        }

        public void Offset(float x, float y)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Vector p = points[i];
                points[i] = new Vector(p.X + x, p.Y + y);
            }
        }

        

    }

    public struct Vector
    {

        public float X;
        public float Y;

       

        static public Vector FromPoint(Point p)
        {
            return Vector.FromPoint(p.X, p.Y);
        }

        static public Vector FromPoint(int x, int y)
        {
            return new Vector((float)x, (float)y);
        }

        public Vector(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

     

        public float Magnitude
        {
            get { return (float)Math.Sqrt(X * X + Y * Y); }
        }

        public void Normalize()
        {
            float magnitude = Magnitude;
            X = X / magnitude;
            Y = Y / magnitude;
        }

        public Vector GetNormalized()
        {
            float magnitude = Magnitude;

            return new Vector(X / magnitude, Y / magnitude);
        }

        public float DotProduct(Vector vector)
        {
            return this.X * vector.X + this.Y * vector.Y;
        }

        public float DistanceTo(Vector vector)
        {
            return (float)Math.Sqrt(Math.Pow(vector.X - this.X, 2) + Math.Pow(vector.Y - this.Y, 2));
        }

        public static implicit operator Point(Vector p)
        {
            return new Point((int)p.X, (int)p.Y);
        }

        public static implicit operator PointF(Vector p)
        {
            return new PointF(p.X, p.Y);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a)
        {
            return new Vector(-a.X, -a.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator *(Vector a, float b)
        {
            return new Vector(a.X * b, a.Y * b);
        }

        public static Vector operator *(Vector a, int b)
        {
            return new Vector(a.X * b, a.Y * b);
        }

        public static Vector operator *(Vector a, double b)
        {
            return new Vector((float)(a.X * b), (float)(a.Y * b));
        }

        public override bool Equals(object obj)
        {
            Vector v = (Vector)obj;

            return X == v.X && Y == v.Y;
        }

        public bool Equals(Vector v)
        {
            return X == v.X && Y == v.Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Vector a, Vector b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector a, Vector b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

      

    }


}
