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
    class AnimacionesAnimacion : Animacion
    {
        public Vector2 posicion;
        public eAnimacionSprite tipoAnimacion; //tipo de animacion

        public AnimacionesAnimacion(GameTime gameTime, int numSprites, TimeSpan frameRate, eAnimacionSprite tipoAnimacion, Vector2 posicion) 
            : base (gameTime, numSprites, frameRate, false)
        {
            this.tipoAnimacion = tipoAnimacion;
            this.posicion = posicion;
        }
    }


    class Animaciones
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        //lista de animaciones en pantalla
        public List<AnimacionesAnimacion> animaciones = new List<AnimacionesAnimacion>();

        //libreria de animaciones posibles
        //public static Dictionary<eAnimacion, AnimacionSprite> sprites = new Dictionary<eAnimacion, AnimacionSprite>();


        ////////////////// PUBLICAS //////////////////

        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public Animaciones()
        {
        }

 
        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public void AddAnimacion(GameTime gameTime, eAnimacionSprite tipoAnimacion, Vector2 posicion)
        {
            AnimacionesAnimacion animacion = new AnimacionesAnimacion(gameTime, AnimacionesSprites.sprites[tipoAnimacion].numSprites,
                                                                      AnimacionesSprites.sprites[tipoAnimacion].frameRate, tipoAnimacion, posicion);
            animaciones.Add(animacion);
        }

        ////////////////// LOAD CONTENT //////////////////
        /*
        public void LoadContent(ContentManager Content)
        {
            AnimacionSprite animacionSprite = new AnimacionSprite();

            animacionSprite.sprite.imagen = Content.Load<Texture2D>("Sprites\\explosion_med_01");
            animacionSprite.sprite.dimImagen = new Point(20, 21);
            animacionSprite.numSprites = 9;
            animacionSprite.frameRate = TimeSpan.FromSeconds(0.15f);
            sprites.Add(eAnimacion.ExplosionMediana, animacionSprite);

        }*/


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime)
        {
            AnimacionesAnimacion animacion;
            for (int iAnimacion = animaciones.Count - 1; iAnimacion >= 0; iAnimacion--)
            {
                animacion = animaciones[iAnimacion];

                //logica de cada animacion
                animacion.Update(gameTime, AnimacionesSprites.sprites[animacion.tipoAnimacion].sprite);

                //borrar las eliminadas
                if (animacion.bEliminar)
                {
                    animaciones.RemoveAt(iAnimacion);
                }
            }
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb)
        {
            AnimacionesAnimacion animacion;
            for (int iAnimacion = animaciones.Count - 1; iAnimacion >= 0; iAnimacion--)
            {
                animacion = animaciones[iAnimacion];

                animacion.Draw(spb, AnimacionesSprites.sprites[animacion.tipoAnimacion].sprite, animacion.posicion, 0f);
            }
        }
       
    }
}
