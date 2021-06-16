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
    class TorreCanon : TorreBase
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public static Sprite sprite = new Sprite(); //sprite de la base
        public static Sprite spriteTorre = new Sprite(); //sprite de la torre

        private TimeSpan previoDisparo; //tiempo en q se hizo anterior disparo

        //public static int MAXNIVEL = 5;


        ////////////////// PUBLICAS //////////////////


        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public TorreCanon(Point coord, int id, int nivel = 0)
            : base(coord, eTipoTorre.Tanque, id, nivel)
        {
            direccion = new Vector2(0, -1); //siempre empieza mirando arriba
            previoDisparo = TimeSpan.Zero; //no se ha disparado nunca

            maximoNivelUpgrade = 5;
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public override CaracteristicasTorre GetCaracteristicas(int nivel)
        {
            CaracteristicasTorre caract = new CaracteristicasTorre();

            switch (nivel)
            {
                case 0:
                    caract.precio = 20;
                    caract.daño = 8;
                    caract.alcance = 90;
                    caract.ratio = 1f;
                    break;
                case 1:
                    caract.precio = 35;
                    caract.daño = 16;
                    caract.alcance = 100;
                    caract.ratio = 1f;
                    break;
                case 2:
                    caract.precio = 70;
                    caract.daño = 32;
                    caract.alcance = 110;
                    caract.ratio = 1f;
                    break;
                case 3:
                    caract.precio = 130;
                    caract.daño = 64;
                    caract.alcance = 120;
                    caract.ratio = 1f;
                    break;
                case 4:
                    caract.precio = 240;
                    caract.daño = 128;
                    caract.alcance = 130;
                    caract.ratio = 1f;
                    break;
                case 5:
                    caract.precio = 400;
                    caract.daño = 256;
                    caract.alcance = 150;
                    caract.ratio = 1.2f;
                    break;
            }

            caract.nivel = nivel;

            float cDaño = 0.5f;
            float cRatio = 0.5f;
            float cAlcance = 20;
            caract.daño = (int)(caract.daño * cDaño);
            caract.ratio *= cRatio;
            caract.alcance += cAlcance;

            if (nivel > MAXNIVELUPGRADE)
                caract.precio = -1;

            return caract;
        }

        protected override void SetCaracteristicasEspeciales(int nivel)
        {
            velocidadDisparo = 110f;
            radioExplosion = 30 + nivel * 4;
            reduceVelocidad = 0;
            tiempoReduceVelocidad = TimeSpan.Zero;

            bDisparo = true;
        }
        

        ////////////////// LOAD CONTENT //////////////////
        public static void LoadContent(ContentManager Content)
        {
            //base
            sprite.imagen = Content.Load<Texture2D>("Sprites\\torre_01");
            sprite.dimImagen = new Point(50, 50);
            sprite.posImagen = new Point(0, 0);

            //torre
            spriteTorre.imagen = Content.Load<Texture2D>("Sprites\\torre_canon_01");
            spriteTorre.dimImagen = new Point(50, 50);
            spriteTorre.posImagen = new Point(0, 0);
        }


        ////////////////// UPDATE ////////////////////////
        protected override void UpdateDisparos(GameTime gameTime, Tablero tablero, Torres torres, Enemigos enemigos, Nave nave, Opciones opciones)
        {
            //cambia direccion apunta la torre
            UpdateApuntar(tablero, enemigos);

            //dispara
            UpdateDisparar(gameTime, tablero, torres, enemigos, nave, opciones);
        }


        private void UpdateDisparar(GameTime gameTime, Tablero tablero, Torres torres, Enemigos enemigos, Nave nave, Opciones opciones)
        {
            if(apuntaEnemigo != -1)
            {
                if (!enemigos.enemigos[apuntaEnemigo].bEliminar)
                {
                    if (gameTime.TotalGameTime - previoDisparo >= TimeSpan.FromSeconds(1 / Ratio))
                    {
                        Vector2 posicionEnemigo = enemigos.enemigos[apuntaEnemigo].posicion;
                        Vector2 posicionTorre = PosicionCoordenadas(tablero) + CentroTorre(tipoTorre, tablero);
                        torres.disparos.AddDisparo(gameTime, posicionTorre, CasillasTorre(tipoTorre, tablero), id, Daño,
                                                   velocidadDisparo, radioExplosion, posicionEnemigo, apuntaEnemigo, eTipoTorreDisparo.Canon);

                        //Sonidos.PlayFX(eSonido.TankFire, opciones);
                        previoDisparo = gameTime.TotalGameTime;
                    }
                }
            }
        }


        ////////////////// DRAW //////////////////
        public override void Draw(SpriteBatch spb, Sprite sprite, Tablero tablero, SpriteFont font)
        {
            if (!bEliminar)
            {
                Vector2 posicion = PosicionCoordenadas(tablero);

                //base
                Rectangle rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, sprite.dimImagen.X, sprite.dimImagen.Y);
                spb.Draw(sprite.imagen, posicion, rectangulo, Color.White);
                
                //torre
                rectangulo = new Rectangle(spriteTorre.posImagen.X, spriteTorre.posImagen.Y, spriteTorre.dimImagen.X, spriteTorre.dimImagen.Y);
                spb.Draw(spriteTorre.imagen, posicion + spriteTorre.center, rectangulo, Color.White, RotateDireccion, spriteTorre.center, 1f, SpriteEffects.None, 0f);

                //nivel
                Vector2 posNivel = posicion + new Vector2(sprite.dimImagen.X, sprite.dimImagen.Y) - 
                                   new Vector2(3, 1) - font.MeasureString(Nivel.ToString());
                spb.DrawString(font, Nivel.ToString(), posNivel, Color.Black);

            }
        }

    }
}
