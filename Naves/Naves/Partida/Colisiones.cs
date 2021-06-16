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
    class Colisiones
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////


        ////////////////// CONSTRUCTOR //////////////////


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////


        ////////////////// UPDATE ////////////////////////
        public static void Update(GameTime gameTime, Enemigos enemigos, Nave nave, Opciones opciones, Animaciones animaciones)
        {
            UpdateColisionesEnemigosDisparos(enemigos, nave, opciones);  //colision enemigo * disparo
            UpdateColisionesEnemigosNave(gameTime, enemigos, nave, opciones, animaciones);      //colision enemigo * nave
        }


        private static void UpdateColisionesEnemigosDisparos(Enemigos enemigos, Nave nave, Opciones opciones)
        {
            EnemigoBase enemigo;
            Sprite eneSprite = null;
            Disparo disparo;
            Sprite disSprite;

            //colision por pasos
            bool bColPaso;
            int tamanoPaso = 4; //ver colision punto a punto (tamanoPaso a tamanoPaso)
            Vector2 posIni;
            Vector2 posFin;
            Vector2 movim;
            int numPasos;
            Vector2 movPasoDis;
            Vector2 posPasoDis;
            Vector2 movPasoEne;
            Vector2 posPasoEne;

            int dNum;
            int eNum;
            for (dNum = nave.disparos.disparos.Count - 1; dNum >= 0; dNum--)
            {
                disparo = nave.disparos.disparos[dNum];
                disSprite = Disparos.sprite[(int)disparo.TipoDisparo];

                //colision por pasos, se recorre punto a punto el movimiento del disparo

                bColPaso = false;
                //disparo paso a paso
                posFin = disparo.posicion;
                posIni = disparo.previaPosicion;
                movim = Matematicas.Vector(posIni, posFin);
                numPasos = (int)(Math.Max(Math.Abs(movim.X), Math.Abs(movim.Y)) / tamanoPaso);
                if (numPasos == 0)
                {
                    numPasos = 1;
                    movPasoDis = movim;
                }
                else
                    movPasoDis = new Vector2(movim.X / numPasos, movim.Y / numPasos);
                posPasoDis = posIni;

                //bucle de disparos por pasos
                for (int iPaso = 0; (iPaso < numPasos) && !bColPaso; iPaso++)
                {
                    posPasoDis += movPasoDis; //posicion del disparo

                    //bucle de enemigos
                    for (eNum = 0; (eNum < enemigos.enemigos.Count) && !bColPaso; eNum++) 
                    {
                        enemigo = enemigos.enemigos[eNum];
                        if (!enemigo.bEliminar)
                        {
                            eneSprite = EnemigoBase.GetSprite(enemigo.TipoEnemigo);

                            //enemigo paso a paso
                            posFin = enemigo.posicion;
                            posIni = enemigo.previaPosicion;
                            movim = Matematicas.Vector(posIni, posFin);
                            if (numPasos == 0)
                            {
                                numPasos = 1;
                                movPasoEne = movim;
                            }
                            else
                                movPasoEne = new Vector2(movim.X / numPasos, movim.Y / numPasos);
                            posPasoEne = posIni;

                            posPasoEne += movPasoEne; //posicion del enemigo


                            //colision con rotate sin pixel a pixel, necesito rapidez pero los disparos rotan

                            if (ColisionRectangulo(eneSprite.imagen, eneSprite.posImagen, posPasoEne, eneSprite.dimImagen, eneSprite.center,
                                                   disSprite.imagen, disSprite.posImagen, posPasoDis, new Point(disSprite.dimImagen.X * 2, disSprite.dimImagen.Y * 2), new Vector2(disSprite.dimImagen.X, disSprite.dimImagen.Y)))
                            {
                                Rectangle rectEne = new Rectangle((int)(posPasoEne.X - eneSprite.center.X), (int)(posPasoEne.Y - eneSprite.center.Y),
                                                                   eneSprite.dimImagen.X, eneSprite.dimImagen.Y);
                                Rectangle rectDis = new Rectangle((int)(posPasoDis.X - disSprite.center.X), (int)(posPasoDis.Y - disSprite.center.Y),
                                                                   disSprite.dimImagen.X, disSprite.dimImagen.Y);
                                if (ColisionRectRotate.Check(rectEne, eneSprite.center, 0f, //enemigo.rotateMovimiento
                                                            rectDis, disSprite.center, disparo.RotateMovimiento))
                                {
                                    bColPaso = true;

                                    enemigo.RecibeAtaque(disparo.Daño, disparo.DañoAire, nave, opciones);

                                    //nave.disparos.disparos.RemoveAt(dNum); //eliminamos disparo
                                    disparo.bEliminar = true;
                                }
                            }

                        }


                        /*
            int dNum;
            int eNum;
            for(eNum = enemigos.enemigos.Count - 1; eNum >= 0; eNum--)
            {
                enemigo = enemigos.enemigos[eNum];
                if (!enemigo.bEliminar)
                {
                    eneSprite = EnemigoBase.GetSprite(enemigo);

                    for (dNum = nave.disparos.disparos.Count - 1; dNum >= 0; dNum--)
                    {
                        disparo = nave.disparos.disparos[dNum];
                        disSprite = Disparos.sprite;

                        //colision con rotate sin pixel a pixel, necesito rapidez pero los disparos rotan

                        if (ColisionRectangulo(eneSprite.imagen, eneSprite.posImagen, posPasoEne, eneSprite.dimImagen, eneSprite.center,
                                               disSprite.imagen, disSprite.posImagen, posPasoDis, new Point(disSprite.dimImagen.X * 2, disSprite.dimImagen.Y * 2), new Vector2(disSprite.dimImagen.X, disSprite.dimImagen.Y)))
                        {
                            Rectangle rectEne = new Rectangle((int)(enemigo.posicion.X - eneSprite.center.X), (int)(enemigo.posicion.Y - eneSprite.center.Y),
                                                               eneSprite.dimImagen.X, eneSprite.dimImagen.Y);
                            Rectangle rectDis = new Rectangle((int)(disparo.posicion.X - disSprite.center.X), (int)(disparo.posicion.Y - disSprite.center.Y),
                                                               disSprite.dimImagen.X, disSprite.dimImagen.Y);
                            if (ColisionRectRotate.Check(rectEne, eneSprite.center, 0f, //enemigo.rotateMovimiento
                                                        rectDis, disSprite.center, disparo.RotateMovimiento))
                            {
                                enemigo.RecibeAtaque(disparo.Daño, nave, opciones);

                                nave.disparos.disparos.RemoveAt(dNum); //eliminamos disparo
                            }
                        }

                        //colision sin rotate y sin pixel a pizel, necesito rapidez
                        //if (ColisionRectangulo(eneSprite.imagen, eneSprite.posImagen, enemigo.posicion, eneSprite.dimImagen, eneSprite.center,
                        //                       disSprite.imagen, disSprite.posImagen, disparo.posicion, disSprite.dimImagen, disSprite.center))
                        //{
                        //    enemigo.RecibeAtaque(disparo.Daño, nave, opciones);

                        //    nave.disparos.disparos.RemoveAt(dNum); //eliminamos disparo
                        //}
                        */
                    }
                }
            }
        }


        private static void UpdateColisionesEnemigosNave(GameTime gameTime, Enemigos enemigos, Nave nave, Opciones opciones, Animaciones animaciones)
        {
            EnemigoBase enemigo;
            Sprite eneSprite = null;
            Sprite naveSprite = nave.sprite;
            bool bExplosionNave = false;
            int eNum;
            for (eNum = enemigos.enemigos.Count - 1; eNum >= 0; eNum--)
            {
                enemigo = enemigos.enemigos[eNum];
                if (!enemigo.bEliminar)
                {
                    eneSprite = EnemigoBase.GetSprite(enemigo.TipoEnemigo);

                    //colision con rotate y pixel a pixel, necesito precisión

                    if (ColisionRotatePixel(naveSprite.imagen, naveSprite.posImagen, nave.posicion, naveSprite.dimImagen, naveSprite.center,
                                            eneSprite.imagen, eneSprite.posImagen, enemigo.posicion, eneSprite.dimImagen, eneSprite.center, enemigo.RotateMovimiento))
                    {
                        bExplosionNave = nave.RecibeAtaque(enemigo.Daño, gameTime, animaciones, opciones);

                        //enemigos.enemigos.RemoveAt(eNum); //eliminamos enemigo
                        enemigo.Explosion(gameTime, opciones, animaciones, !bExplosionNave);
                    }

                    /*
                    if (ColisionRectangulo(eneSprite.imagen, eneSprite.posImagen, enemigo.posicion, new Point(eneSprite.dimImagen.X * 2, eneSprite.dimImagen.Y * 2), eneSprite.center,
                                           naveSprite.imagen, naveSprite.posImagen, nave.posicion, naveSprite.dimImagen, naveSprite.center))
                    {
                        Rectangle rectEne = new Rectangle((int)(enemigo.posicion.X - eneSprite.center.X), (int)(enemigo.posicion.Y - eneSprite.center.Y),
                                                           eneSprite.dimImagen.X, eneSprite.dimImagen.Y);
                        Rectangle rectNave = new Rectangle((int)(nave.posicion.X - naveSprite.center.X), (int)(nave.posicion.Y - naveSprite.center.Y),
                                                           naveSprite.dimImagen.X, naveSprite.dimImagen.Y);
                        if (ColisionRectRotate.Check(rectNave, naveSprite.center, 0f,
                                                     rectEne, eneSprite.center, enemigo.rotateMovimiento))
                    */

                }
            }

        }


        private static bool ColisionRectangulo(Texture2D imagenA, Point posImagenA, Vector2 posicionA, Point dimImagenA, Vector2 centerA,
                                               Texture2D imagenB, Point posImagenB, Vector2 posicionB, Point dimImagenB, Vector2 centerB)
        {
            bool bColision = false;

            //Si los rectangle de A y de B se intersectan comprobamos la colision.
            Rectangle rectanguloA = new Rectangle((int)(posicionA.X - centerA.X), (int)(posicionA.Y - centerA.Y), dimImagenA.X, dimImagenA.Y);
            Rectangle rectanguloB = new Rectangle((int)(posicionB.X - centerB.X), (int)(posicionB.Y - centerB.Y), dimImagenB.X, dimImagenB.Y);

            if (rectanguloA.Intersects(rectanguloB))
            {
                //los disparos y enemigos son muchos y es suficiente con la intersecion rectangulos sin rotar
                //el disparo es sprite muy preciso, los enemigos son cuadrados (no influye que estén rotados)
                //cuando los enemigos sean rectangulares habra q revisarlo y usar ColisionRectRotate:

                bColision = true;
            }

            return bColision;

            //!!! este metodo se puede hacer comun a todos los objetos, hacer interface o abstracto 
            ///IColisionable q tenga posicion, dimensiones y posImagen !!!
        }


        private static bool ColisionPixel(Texture2D imagenA, Vector2 posImagenA, Rectangle rectanguloA,
                                         Texture2D imagenB, Vector2 posImagenB, Rectangle rectanguloB)
        {
            //rectangulo es la posicion en la pantalla y el ancho de la imagen dibujada
            //rectangulo = new Rectangle((int)(posicion.X - center.X), (int)(posicion.Y - center.Y), anchoImagen, altoImagen);

            //posImagen es la posicion de la imagen dibujada dentro del sprite
            //posImagen = new Vector2(posImagen.X, posImagen.Y);

            bool colisionPxAPx = false;

            uint[] bitsA = new uint[imagenA.Width * imagenA.Height];
            uint[] bitsB = new uint[imagenB.Width * imagenB.Height];

            //almacenamos los datos de los pixeles en las variables locales bitsA y bitsB
            imagenA.GetData<uint>(bitsA);
            imagenB.GetData<uint>(bitsB);
            
            //almacenamos las coordenadas que delimitaran la zona en la que trabajaremos
            int x1 = Math.Max(rectanguloA.X, rectanguloB.X);
            int x2 = Math.Min(rectanguloA.X + rectanguloA.Width, rectanguloB.X + rectanguloB.Width);

            int y1 = Math.Max(rectanguloA.Y, rectanguloB.Y);
            int y2 = Math.Min(rectanguloA.Y + rectanguloA.Height, rectanguloB.Y + rectanguloB.Height);

            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    if (((bitsA[(x - rectanguloA.X + (int)posImagenA.X) + (y - rectanguloA.Y + (int)posImagenA.Y) * imagenA.Width] & 0xFF000000) >> 24) > 20 &&
                        ((bitsB[(x - rectanguloB.X + (int)posImagenB.X) + (y - rectanguloB.Y + (int)posImagenB.Y) * imagenB.Width] & 0xFF000000) >> 24) > 20)
                    {
                        //Se comprueba el canal alpha de las dos imagenes en el mismo pixel. Si los dos son visibles hay colision.
                        colisionPxAPx = true;
                        break;
                    }
                }

                // Rompe el bucle si la condicion ya se ha cumplido.
                if (colisionPxAPx)
                {
                    break;
                }
            }

            return colisionPxAPx;
        }


        //colision con rotate y pixel a pixel, primero comprueba rectangulos rotados
        //elementoA no está rotado, elementoB si está rotado
        //Fuente: TransformedCollisionSample_4_0 (AdHub)
        private static bool ColisionRotatePixel(Texture2D imagenA, Point posImagenA, Vector2 posicionA, Point dimImagenA, Vector2 centerA,
                                                Texture2D imagenB, Point posImagenB, Vector2 posicionB, Point dimImagenB, Vector2 centerB, float rotationB)
        {
            bool bColision = false;

            // Update the transform of nave
            //Matrix naveTransform =
            //    Matrix.CreateTranslation(new Vector3(posicionA, 0.0f));
            Matrix ATransform =
                Matrix.CreateTranslation(new Vector3(-centerA, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(posicionA, 0.0f));

            // Get the bounding rectangle of the nave
            Rectangle ARectangle = new Rectangle((int)(posicionA.X - centerA.X), (int)(posicionA.Y - centerA.Y),
                                                 dimImagenA.X, dimImagenA.Y);

            // Build the transform of enemigo
            Matrix BTransform =
                Matrix.CreateTranslation(new Vector3(-centerB, 0.0f)) *
                // Matrix.CreateScale(block.Scale) *  would go here
                Matrix.CreateRotationZ(rotationB) *
                Matrix.CreateTranslation(new Vector3(posicionB, 0.0f));

            // Calculate the bounding rectangle of the enemigo in world space
            Rectangle BRectangle = CalculateBoundingRectangle(
                     new Rectangle(posImagenB.X, posImagenB.Y, dimImagenB.X, dimImagenB.Y),
                //new Rectangle(0, 0, blockTexture.Width, blockTexture.Height), // el 0,0 es posImagen o position o siempre cero?
                     BTransform);


            // The per-pixel check is expensive, so check the bounding rectangles
            // first to prevent testing pixels when collisions are impossible
            if (ARectangle.Intersects(BRectangle))
            {
                // Check collision pixel to pixel
                Color[] ATextureData;
                Color[] BTextureData;

                // Extract collision data
                ATextureData = new Color[imagenA.Width * imagenA.Height];
                imagenA.GetData(ATextureData);
                BTextureData = new Color[imagenB.Width * imagenB.Height];
                imagenB.GetData(BTextureData);

                if (IntersectPixels(ATransform, dimImagenA.X, dimImagenA.Y, ATextureData, posImagenA.X, posImagenA.Y, imagenA.Width,
                                    BTransform, dimImagenB.X, dimImagenB.Y, BTextureData, posImagenB.X, posImagenB.Y, imagenB.Width))
                {
                    bColision = true;
                }
            }

            return bColision;
        }


        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites. CON ROTACION
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool IntersectPixels(
                            Matrix transformA, int widthA, int heightA, Color[] dataA, int posImagenXA, int posImagenYA, int imagenWidthA,
                            Matrix transformB, int widthB, int heightB, Color[] dataB, int posImagenXB, int posImagenYB, int imagenWidthB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + posImagenXA + (yA + posImagenYA) * imagenWidthA];
                        Color colorB = dataB[xB + posImagenXB + (yB + posImagenYB) * imagenWidthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }


        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle,
                                                           Matrix transform)
        {
            // Get all four corners in local space
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }


        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites. SIN ROTACION
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the first sprite</param>
        /// <param name="dataA">Pixel data of the first sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the second sprite</param>
        /// <param name="dataB">Pixel data of the second sprite</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        /*
        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                           Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }
        */



    }
}
