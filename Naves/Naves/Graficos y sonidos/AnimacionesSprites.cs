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
    enum eAnimacionSprite
    {
        ExplosionMediana,
        ExplosionNave,
        ExplosionCanon
    }


    class AnimacionSprite
    {
        public Sprite sprite = new Sprite();
        public int numSprites; //numero de sprites en cada imagen
        public TimeSpan frameRate; //tiempo para cambiar imagen animacion por siguiente
    }


    static class AnimacionesSprites
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        //libreria de animaciones posibles
        public static Dictionary<eAnimacionSprite, AnimacionSprite> sprites = new Dictionary<eAnimacionSprite, AnimacionSprite>();


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// LOAD CONTENT //////////////////
        public static void LoadContent(ContentManager Content)
        {
            AnimacionSprite animacionSprite;

            /*
            AnimacionSprite animacionSprite = new AnimacionSprite();
            animacionSprite.sprite.imagen = Content.Load<Texture2D>("Sprites\\explosion_peq_01");
            animacionSprite.sprite.dimImagen = new Point(20, 20);
            animacionSprite.numSprites = 10;
            animacionSprite.frameRate = TimeSpan.FromMilliseconds(0.15f);
            sprites.Add(eAnimacion.ExplosionPequeña, animacionSprite);*/

            animacionSprite = new AnimacionSprite();
            animacionSprite.sprite.imagen = Content.Load<Texture2D>("Sprites\\explosion_media_01");
            animacionSprite.sprite.dimImagen = new Point(32, 32);
            animacionSprite.sprite.posImagen = new Point(0, 0);
            animacionSprite.numSprites = 15;
            animacionSprite.frameRate = TimeSpan.FromSeconds(0.075f);
            sprites.Add(eAnimacionSprite.ExplosionMediana, animacionSprite);

            animacionSprite = new AnimacionSprite();
            animacionSprite.sprite.imagen = Content.Load<Texture2D>("Sprites\\explosion_nave_01");
            animacionSprite.sprite.dimImagen = new Point(71, 100);
            animacionSprite.sprite.posImagen = new Point(0, 0);
            animacionSprite.numSprites = 15;
            animacionSprite.frameRate = TimeSpan.FromSeconds(0.10f);
            sprites.Add(eAnimacionSprite.ExplosionNave, animacionSprite);

            animacionSprite = new AnimacionSprite();
            animacionSprite.sprite.imagen = Content.Load<Texture2D>("Sprites\\explosion_canon_01");
            animacionSprite.sprite.dimImagen = new Point(58, 32);
            animacionSprite.sprite.posImagen = new Point(0, 0);
            animacionSprite.numSprites = 6;
            animacionSprite.frameRate = TimeSpan.FromSeconds(0.15f);
            sprites.Add(eAnimacionSprite.ExplosionCanon, animacionSprite);

        }


    }
}
