using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Naves
{
    class Matematicas
    {

        public static Vector2 NOVECTOR = new Vector2(-999, -999);


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////


        //distancia entre dos puntos
        public static float Distancia(Point pointA, Point pointB)
        {
            double distanceX = Math.Pow(pointB.X - pointA.X, 2);
            double distanceY = Math.Pow(pointB.Y - pointA.Y, 2);

            return (float)Math.Sqrt(distanceX + distanceY);
        }

        public static float Distancia(Vector2 pointA, Vector2 pointB)
        {
            double distanceX = Math.Pow(pointB.X - pointA.X, 2);
            double distanceY = Math.Pow(pointB.Y - pointA.Y, 2);

            return (float)Math.Sqrt(distanceX + distanceY);
        }

        //vector entre dos puntos
        public static Vector2 Vector(Vector2 pointIni, Vector2 pointFin)
        {
            return new Vector2(pointFin.X - pointIni.X, pointFin.Y - pointIni.Y);
        }

        public static Vector2 Vector(Point pointIni, Point pointFin)
        {
            return new Vector2(pointFin.X - pointIni.X, pointFin.Y - pointIni.Y);
        }

        //sprite con rotacion, para saber dimensiones horizontal/vertical necesito hacer calculos
        public static Rectangle dimImagen(Sprite sprite, float rotation)
        {
            // Build the transform of sprite
            Matrix transform =
                Matrix.CreateTranslation(new Vector3(-sprite.center, 0.0f)) *
                Matrix.CreateRotationZ(rotation);
                // * Matrix.CreateTranslation(new Vector3(posicion, 0.0f));

            // Calculate the bounding rectangle of the sprite in world space
            Rectangle rectangle = Colisiones.CalculateBoundingRectangle(
                     new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, sprite.dimImagen.X, sprite.dimImagen.Y),
                     transform);

            return rectangle;
        }


    }
}
