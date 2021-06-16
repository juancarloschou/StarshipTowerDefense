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
    class Animacion
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public bool bEliminar; //para ver cuando acaba y eliminarlo de la lista
        private bool bCiclico; //si es ciclico no termina al acabar, vuelve a empezar
        public TimeSpan frameRate; //tiempo para cambiar imagen animacion por siguiente
        private TimeSpan previoTime; //contador de tiempo
        private int contadorSprite;
        private int numSprites;
        private Point posImagen;


        ////////////////// PUBLICAS //////////////////

        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public Animacion(GameTime gameTime, int numSprites, TimeSpan frameRate, bool ciclico)
        {
            this.previoTime = gameTime.TotalGameTime;
            this.frameRate = frameRate;
            this.contadorSprite = 0;
            this.numSprites = numSprites;
            this.posImagen = new Point(0, 0);
            this.bEliminar = false;
            this.bCiclico = ciclico;
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// LOAD CONTENT //////////////////
        /*public void LoadContent(ContentManager Content)
        {
            //sprite.imagen = Content.Load<Texture2D>(nombreImagen);
        }*/


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, Sprite sprite)
        {
            if (gameTime.TotalGameTime - previoTime >= frameRate)
            {
                previoTime = gameTime.TotalGameTime;

                posImagen.X += sprite.dimImagen.X;

                contadorSprite++;
                if (contadorSprite >= numSprites)
                {
                    contadorSprite = 0;
                    posImagen.X = 0;

                    if (!bCiclico)
                        bEliminar = true; //se termina animacion
                }
            }
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb, Sprite sprite, Vector2 posicion, float rotateMovimiento = 0f, float scale = 1f)
        {
            if (!bEliminar)
            {
                Rectangle rectangulo = new Rectangle(posImagen.X, posImagen.Y, sprite.dimImagen.X, sprite.dimImagen.Y);

                spb.Draw(sprite.imagen, posicion, rectangulo, Color.White, rotateMovimiento, sprite.center, scale, SpriteEffects.None, 0f);
            }
        }
       
    }
}
