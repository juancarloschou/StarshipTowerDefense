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
    class Fondo
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public Sprite sprite;

        //limites de la ventana
        private Viewport ventanaGame;


        ////////////////// CONSTRUCTOR //////////////////
        public Fondo(Viewport viewport)
        {
            ventanaGame = viewport;

            sprite = new Sprite();
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager Content)
        {
            sprite.imagen = Content.Load<Texture2D>("Fondo\\Fondo_01");
            sprite.dimImagen = new Point(700, 600);
            sprite.posImagen = new Point(0, 0);
        }


        ////////////////// UPDATE ////////////////////////
        public void Update()
        {
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb)
        {
            Rectangle rectangulo = new Rectangle(0, 0, (int)ventanaGame.Width - 50, (int)ventanaGame.Height - 50);
            spb.Draw(sprite.imagen, new Vector2(25, 25), rectangulo, Color.White);
        }

    }
}
