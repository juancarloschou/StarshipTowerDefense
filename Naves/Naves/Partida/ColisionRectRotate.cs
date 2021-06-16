using System;
using Microsoft.Xna.Framework;

namespace Naves
{
    struct RectRotate
    {
        public Vector2 UpperLeft;
        public Vector2 UpperRight;
        public Vector2 LowerLeft;
        public Vector2 LowerRight;

        public RectRotate(Rectangle rectangle)
        {
            UpperLeft = new Vector2(rectangle.Left, rectangle.Top);
            UpperRight = new Vector2(rectangle.Right, rectangle.Top);
            LowerLeft = new Vector2(rectangle.Left, rectangle.Bottom);
            LowerRight = new Vector2(rectangle.Right, rectangle.Bottom);
        }

        public void Rotate(Vector2 origin, float rotation)
        {
            UpperLeft = RecRotateUtil.RotateVector(UpperLeft, origin, rotation);
            UpperRight = RecRotateUtil.RotateVector(UpperRight, origin, rotation);
            LowerLeft = RecRotateUtil.RotateVector(LowerLeft, origin, rotation);
            LowerRight = RecRotateUtil.RotateVector(LowerRight, origin, rotation);
        }

        public void RotateInverse(Vector2 origin, float rotation)
        {
            UpperLeft = RecRotateUtil.RotateVectorInverse(UpperLeft, origin, rotation);
            UpperRight = RecRotateUtil.RotateVectorInverse(UpperRight, origin, rotation);
            LowerLeft = RecRotateUtil.RotateVectorInverse(LowerLeft, origin, rotation);
            LowerRight = RecRotateUtil.RotateVectorInverse(LowerRight, origin, rotation);
        }

        public void AddVector(Vector2 vector)
        {
            UpperLeft += vector;
            UpperRight += vector;
            LowerLeft += vector;
            LowerRight += vector;
        }

        public float MinX()
        {
            float[] temp = { UpperLeft.X, UpperRight.X, LowerLeft.X, LowerRight.X };
            return RecRotateUtil.Min(temp);
        }

        public float MaxX()
        {
            float[] temp = { UpperLeft.X, UpperRight.X, LowerLeft.X, LowerRight.X };
            return RecRotateUtil.Max(temp);
        }

        public float MinY()
        {
            float[] temp = { UpperLeft.Y, UpperRight.Y, LowerLeft.Y, LowerRight.Y };
            return RecRotateUtil.Min(temp);
        }

        public float MaxY()
        {
            float[] temp = { UpperLeft.Y, UpperRight.Y, LowerLeft.Y, LowerRight.Y };
            return RecRotateUtil.Max(temp);
        }
    }

    public static class ColisionRectRotate
    {
        public static bool Check(Rectangle theRectangleA, Vector2 theOriginA, float theRotationA, Rectangle theRectangleB, Vector2 theOriginB, float theRotationB)
        {
            RectRotate rectA = new RectRotate(theRectangleA);
            RectRotate rectB = new RectRotate(theRectangleB);

            theOriginA += rectA.UpperLeft;
            theOriginB += rectB.UpperLeft;

            rectB.Rotate(theOriginB, theRotationB);
            rectB.RotateInverse(theOriginA, theRotationA);

            rectA.AddVector(-theOriginA);
            rectB.AddVector(-theOriginA);

            if ((rectB.MinX() > rectA.MaxX()) || (rectB.MaxX() < rectA.MinX()) // x-axis of A
                || (rectB.MinY() > rectA.MaxY()) || (rectB.MaxY() < rectA.MinY()) // y-axis of A
                || (!CheckAxisCollision(rectA, rectB, rectB.UpperLeft - rectB.UpperRight)) // x-axis of B
                || (!CheckAxisCollision(rectA, rectB, rectB.UpperLeft - rectB.LowerLeft))) // y-axis of B
                return false;

            return true;
        }

        private static bool CheckAxisCollision(RectRotate rectA, RectRotate rectB, Vector2 aAxis)
        {
            int[] aRectangleAScalars = {
                GenerateScalar(rectB.UpperLeft, aAxis),
                GenerateScalar(rectB.UpperRight, aAxis),
                GenerateScalar(rectB.LowerLeft, aAxis),
                GenerateScalar(rectB.LowerRight, aAxis)
            };

            int[] aRectangleBScalars = {
                GenerateScalar(rectA.UpperLeft, aAxis),
                GenerateScalar(rectA.UpperRight, aAxis),
                GenerateScalar(rectA.LowerLeft, aAxis),
                GenerateScalar(rectA.LowerRight, aAxis)
            };

            int aRectangleAMinimum = RecRotateUtil.Min(aRectangleAScalars);
            int aRectangleAMaximum = RecRotateUtil.Max(aRectangleAScalars);
            int aRectangleBMinimum = RecRotateUtil.Min(aRectangleBScalars);
            int aRectangleBMaximum = RecRotateUtil.Max(aRectangleBScalars);

            //If we have overlaps between the Rectangles (i.e. Min of B is less than Max of A)
            //then we are detecting a collision between the rectangles on this Axis
            if ((aRectangleBMinimum <= aRectangleAMaximum
                    && aRectangleBMaximum >= aRectangleAMaximum)
                || (aRectangleAMinimum <= aRectangleBMaximum
                    && aRectangleAMaximum >= aRectangleBMaximum))
                return true;

            return false;
        }

        private static int GenerateScalar(Vector2 theRectangleCorner, Vector2 theAxis)
        {
            // create projection of corner onto axis
            float aDivisionResult = Vector2.Dot(theRectangleCorner, theAxis) / Vector2.Dot(theAxis, theAxis);
            Vector2 aCornerProjected = aDivisionResult * theAxis;

            // return scalar of projection
            return (int)Vector2.Dot(theAxis, aCornerProjected);
        }
    }

    class RecRotateUtil
    {
        public static Vector2 RotateVector(Vector2 vector, Vector2 origin, float angle)
        {
            return Vector2.Transform(vector, Matrix.CreateTranslation(-origin.X, -origin.Y, 0) * Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(origin.X, origin.Y, 0));
        }

        public static Vector2 RotateVectorInverse(Vector2 vector, Vector2 origin, float angle)
        {
            return Vector2.Transform(vector, Matrix.Invert(Matrix.CreateTranslation(-origin.X, -origin.Y, 0) * Matrix.CreateRotationZ(angle) * Matrix.CreateTranslation(origin.X, origin.Y, 0)));
        }

        public static float Min(float[] array)
        {
            float min = float.MaxValue;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < min)
                    min = array[i];
            }

            return min;
        }

        public static float Max(float[] array)
        {
            float max = float.MinValue;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > max)
                    max = array[i];
            }

            return max;
        }

        public static int Min(int[] array)
        {
            int min = int.MaxValue;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] < min)
                    min = array[i];
            }

            return min;
        }

        public static int Max(int[] array)
        {
            int max = int.MinValue;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > max)
                    max = array[i];
            }

            return max;
        }
    }
}
