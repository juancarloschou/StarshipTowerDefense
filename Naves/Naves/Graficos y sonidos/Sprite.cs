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
    class Sprite
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public Texture2D imagen;
        //ancho y alto del sprite
        public Point dimImagen;
        //posicion del sprite dentro de la imagen (puede haber varios sprites en una imagen)
        public Point posImagen;


        ////////////////// PUBLICAS //////////////////
        //centro del sprite (para dibujar su centro en las coordenadas especificadas)
        public Vector2 center
        {
            get { return new Vector2((int)(dimImagen.X / 2), (int)(dimImagen.Y / 2)); }
        }

        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public Sprite()
        {
        }

        /*public Sprite(Texture2D Imagen, Point DimImagen, Point PosImagen)
        {
            this.imagen = Imagen;
            this.dimImagen = DimImagen;
            this.posImagen = PosImagen;
        }*/

        /*public Sprite(Texture2D Imagen, Vector2 DimImagen, Vector2 PosImagen, Vector2 Center)
        {
            this.imagen = Imagen;
            this.dimImagen = DimImagen;
            this.posImagen = PosImagen;
            this.center = Center;
        }*/


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// LOAD CONTENT //////////////////
        /*public void LoadContent(ContentManager Content)
        {
        }
        */


        ////////////////// UPDATE ////////////////////////
        /*
        public void Update()
        {

        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb)
        {

        }
        */
    }
}
