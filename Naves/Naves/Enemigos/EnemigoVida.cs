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
    class EnemigoVida
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        private const float SEPARACION = 3;
        private const float ALTURA = 5;

        private static Sprite sprite = new Sprite();

        private bool bEliminar;
        private Vector2 posicion;
        //private int idEnemigo;
        private int maxVida;
        private int vida;
        private int ancho;


        ////////////////// PUBLICAS //////////////////


        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        /*public EnemigoVida()
        {
            this.bEliminar = true;
        }*/

        public EnemigoVida(EnemigoBase enemigo)
        {
            this.bEliminar = false;
            //this.idEnemigo = idEnemigo;
            this.maxVida = enemigo.MaxVida; //enemigos.enemigos[idEnemigo].MaxVida;

            ObtieneDatos(enemigo);
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        private void ObtieneDatos(EnemigoBase enemigo)
        {
            if (enemigo.bEliminar)
                bEliminar = true;
            else
            {
                vida = enemigo.Vida;
                Vector2 posicionEnemigo = enemigo.posicion;
                Point dimImagen = enemigo.GetSprite().dimImagen;
                ancho = dimImagen.X;
                posicion = posicionEnemigo - new Vector2(dimImagen.X / 2, dimImagen.Y / 2 + SEPARACION);
            }
        }


        ////////////////// LOAD CONTENT //////////////////
        public static void LoadContent(ContentManager content)
        {
            sprite.imagen = content.Load<Texture2D>("Sprites\\blank");
            sprite.dimImagen = new Point(4, 4);
            sprite.posImagen = new Point(0, 0);
        }


        ////////////////// UPDATE ////////////////////////
        public void Update(EnemigoBase enemigo)
        {
            if(!bEliminar)
                ObtieneDatos(enemigo);
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb) //, GraphicsDevice graphiscsDevice)
        {
            if (!bEliminar)
            {

                // This creates a gradient between 0 for the first waypoint on the 
                // list and 1 for the last, 0 creates a color that's completely red 
                // and 1 creates a color that's completely green
                float porcentajeVida;
                Color drawColor;

                porcentajeVida = (float)vida / (float)maxVida;
                /*
                if (porcentajeVida > 0.5)
                    drawColor = new Color(Vector4.Lerp(Color.Yellow.ToVector4(), Color.Green.ToVector4(), (porcentajeVida - 0.50f) * 2));
                else
                    drawColor = new Color(Vector4.Lerp(Color.Red.ToVector4(), Color.Yellow.ToVector4(), porcentajeVida * 2));
                */

                if(porcentajeVida > 0.525)
                    drawColor = new Color(Vector4.Lerp(Color.FromNonPremultiplied(195, 255, 0, 255).ToVector4(), Color.Green.ToVector4(), (porcentajeVida - 0.525f) * (1 / 0.475f)));
                else if (porcentajeVida < 0.475)
                    drawColor = new Color(Vector4.Lerp(Color.Red.ToVector4(), Color.FromNonPremultiplied(255, 195, 0, 255).ToVector4(), porcentajeVida * (1 / 0.475f)));
                else
                    drawColor = Color.Yellow;

                int x = (int) Math.Round(porcentajeVida * ancho);

                Rectangle rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, x, (int)ALTURA);
                spb.Draw(sprite.imagen, posicion + new Vector2(0, -ALTURA), rectangulo, drawColor);

                if (x < ancho)
                {
                    rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, (int)ancho - x, (int)ALTURA);
                    spb.Draw(sprite.imagen, posicion + new Vector2(x, -ALTURA), rectangulo, Color.Black);
                }

            }
        }

    }
}
