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
    class Clase
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        private Texture2D imagen;
        public Vector2 posicion;
        public Vector2 movimiento;
        //ancho y alto de la imagen
        public Vector2 dimensiones = new Vector2(42, 44);
        //posicion inicial dentro del sprite
        public Vector2 posImagen;
        //limites de la ventana
        private Viewport ventana;


        ////////////////// PUBLICAS //////////////////


        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public Clase()
        {

        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager Content)
        {
            //imagen = Content.Load<Texture2D>("Fondo\\space_background_01");
        }


        ////////////////// UPDATE ////////////////////////
        public void Update()
        {

        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb)
        {

        }

    }
}
