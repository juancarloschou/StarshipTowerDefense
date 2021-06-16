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
    class TorreBloque : TorreBase
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public static Sprite sprite = new Sprite(); //sprite de la base

        //public static int MAXNIVEL = 0;


        ////////////////// PUBLICAS //////////////////


        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public TorreBloque(Point coord, int id, int nivel = 0)
            : base(coord, eTipoTorre.Bloque, id, nivel)
        {
            direccion = new Vector2(0, 0); //siempre empieza mirando arriba

            maximoNivelUpgrade = MAXNIVELUPGRADE;
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public override CaracteristicasTorre GetCaracteristicas(int nivel)
        {
            CaracteristicasTorre caract = new CaracteristicasTorre();

            caract.alcance = 0; //no ataca
            caract.daño = 0;
            caract.ratio = 0;

            if (nivel <= MAXNIVELUPGRADE)
                caract.precio = 5 * (nivel + 1) + (nivel * 5);
            else
                caract.precio = -1;

            return caract;
        }

        protected override void SetCaracteristicasEspeciales(int nivel)
        {
            velocidadDisparo = 0;
            radioExplosion = 0;
            reduceVelocidad = 0;
            tiempoReduceVelocidad = TimeSpan.Zero;

            bDisparo = false;
        }
        

        ////////////////// LOAD CONTENT //////////////////
        public static void LoadContent(ContentManager Content)
        {
            sprite.imagen = Content.Load<Texture2D>("Sprites\\torre_01");
            sprite.dimImagen = new Point(50, 50);
            sprite.posImagen = new Point(0, 0);
        }


        ////////////////// UPDATE ////////////////////////
        protected override void UpdateDisparos(GameTime gameTime, Tablero tablero, Torres torres, Enemigos enemigos, Nave nave, Opciones opciones)
        {
        }


        ////////////////// DRAW //////////////////
        public override void Draw(SpriteBatch spb, Sprite sprite, Tablero tablero, SpriteFont font)
        {
            if (!bEliminar)
            {
                //base
                Rectangle rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, sprite.dimImagen.X, sprite.dimImagen.Y);
                spb.Draw(sprite.imagen, PosicionCoordenadas(tablero), rectangulo, Color.White);
            }
        }

    }
}
