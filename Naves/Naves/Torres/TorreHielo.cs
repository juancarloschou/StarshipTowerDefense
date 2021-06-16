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
    class TorreHielo : TorreBase
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public static Sprite sprite = new Sprite(); //sprite de la base
        public static Sprite spriteTorre = new Sprite(); //sprite de la torre
        public static Sprite spriteDisparo = new Sprite(); //sprite del disparo de hielo

        private TimeSpan previoDisparo; //tiempo en q se hizo anterior disparo
        //private int disparoCount; //durante el disparo, contador
        //private const int DISPAROMAX = 120; //acaba el disparo
        public bool bDisparando; //si el disparo está en marcha
        public static TimeSpan tiempoTotalDisparo = TimeSpan.FromSeconds(1.5f); //tiempo duracion total del disparo
        public TimeSpan tiempoPasadoDisparo; //tiempo transcurrido de disparo

        //public static int MAXNIVEL = 5;


        ////////////////// PUBLICAS //////////////////


        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public TorreHielo(Point coord, int id, int nivel = 0)
            : base(coord, eTipoTorre.Hielo, id, nivel)
        {
            direccion = new Vector2(0, -1); //siempre empieza mirando arriba
            previoDisparo = TimeSpan.Zero; //no se ha disparado nunca

            maximoNivelUpgrade = MAXNIVELUPGRADE;
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
                    caract.precio = 50;
                    caract.daño = 10;
                    caract.alcance = 50;
                    caract.ratio = 1f;
                    break;
                case 1:
                    caract.precio = 75;
                    caract.daño = 15;
                    caract.alcance = 50;
                    caract.ratio = 1f;
                    break;
                case 2:
                    caract.precio = 100;
                    caract.daño = 20;
                    caract.alcance = 50;
                    caract.ratio = 1f;
                    break;
                case 3:
                    caract.precio = 125;
                    caract.daño = 25;
                    caract.alcance = 50;
                    caract.ratio = 1f;
                    break;
                case 4:
                    caract.precio = 150;
                    caract.daño = 30;
                    caract.alcance = 50;
                    caract.ratio = 1f;
                    break;
                case 5:
                    caract.precio = 250;
                    caract.daño = 50;
                    caract.alcance = 75;
                    caract.ratio = 1.2f;
                    break;
            }

            caract.nivel = nivel;

            float cDaño = 0.5f;
            float cRatio = 0.3f;
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
            velocidadDisparo = 0;
            radioExplosion = 0;
            reduceVelocidad = 0.8f - nivel * 0.1f;
            tiempoReduceVelocidad = TimeSpan.FromSeconds(2f + nivel * 0.33f);

            //disparoCount = -1; //no esta disparando
            bDisparando = false;

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
            spriteTorre.imagen = Content.Load<Texture2D>("Sprites\\torre_hielo_01");
            spriteTorre.dimImagen = new Point(50, 50);
            spriteTorre.posImagen = new Point(0, 0);

            //disparo hielo
            spriteDisparo.imagen = Content.Load<Texture2D>("Sprites\\torre_disparos_hielo_01");
            spriteDisparo.dimImagen = new Point(332, 332);
            spriteDisparo.posImagen = new Point(0, 0);
        }


        ////////////////// UPDATE ////////////////////////
        protected override void UpdateDisparos(GameTime gameTime, Tablero tablero, Torres torres, Enemigos enemigos, Nave nave, Opciones opciones)
        {
            //cambia direccion apunta la torre
            //UpdateApuntar(tablero, enemigos);

            //dispara
            UpdateDisparar(gameTime, tablero, torres, enemigos, nave, opciones);
        }


        private void UpdateDisparar(GameTime gameTime, Tablero tablero, Torres torres, Enemigos enemigos, Nave nave, Opciones opciones)
        {
            bool bHayEnemigos = false;

            Vector2 centroTorre = TorreBase.CentroTorre(tipoTorre, tablero);
            centroTorre = tablero.CoordToPosicion(coordenadas) + centroTorre;
            EnemigoBase enemigo;
            float distancia;
            for (int iEnemigo = 0; iEnemigo < enemigos.enemigos.Count; iEnemigo++)
            {
                enemigo = enemigos.enemigos[iEnemigo];
                if (!enemigo.bEliminar)
                {
                    distancia = Matematicas.Distancia(centroTorre, enemigo.posicion);
                    if (distancia <= Alcance) //dentro del alcance
                    {
                        bHayEnemigos = true;
                    }
                }
            }

            if (bHayEnemigos) //hay enemigos dentro del alcance
            {
                if (gameTime.TotalGameTime - previoDisparo >= TimeSpan.FromSeconds(1 / Ratio))
                {
                    bDisparando = true;
                    //disparoCount = 0;
                    tiempoPasadoDisparo = TimeSpan.Zero;

                    //Sonidos.PlayFX(eSonido.TankFire, opciones);
                    previoDisparo = gameTime.TotalGameTime;
                }
            }

            if (bDisparando)
            {
                //disparocount debe basarse en timeelapsed para hacer la animacion en un tiempo definido
                //disparoCount++;
                tiempoPasadoDisparo += gameTime.ElapsedGameTime;

                if (tiempoPasadoDisparo > tiempoTotalDisparo)
                {
                    bDisparando = false;

                    //congelar enemigos
                    for (int iEnemigo = 0; iEnemigo < enemigos.enemigos.Count; iEnemigo++)
                    {
                        enemigo = enemigos.enemigos[iEnemigo];
                        if (!enemigo.bEliminar)
                        {
                            distancia = Matematicas.Distancia(centroTorre, enemigo.posicion);
                            if (distancia <= Alcance) //dentro del alcance
                            {
                                //helar enemigo
                                enemigo.RecibeAtaqueTorreHelado(gameTime, Daño, reduceVelocidad, tiempoReduceVelocidad, nave, opciones);
                            }
                        }
                    }

                }
            }
        }

 
        ////////////////// DRAW //////////////////
        public void DrawDisparo(SpriteBatch spb, Tablero tablero)
        {
            if (bDisparando)
            {
                //float percent = ((float)disparoCount / DISPAROMAX);
                float percent = (float)tiempoPasadoDisparo.TotalMilliseconds / (float)tiempoTotalDisparo.TotalMilliseconds;

                float scale = percent * ((Alcance * 2) / spriteDisparo.dimImagen.X);

                float giro;
                if (percent < 0.5f)
                    giro = -(0.5f - percent) * 2;
                else
                    giro = (percent - 0.5f) * 2;

                Color drawColor = new Color(Vector4.Lerp(Color.White.ToVector4(), Color.LightSkyBlue.ToVector4(), percent));

                float alpha = 0.7f;

                Vector2 posicion = PosicionCoordenadas(tablero);
                //Vector2 posCentroTorre = TorreBase.CentroTorre(tipoTorre, tablero);
                Rectangle rectangulo = new Rectangle(spriteDisparo.posImagen.X, spriteDisparo.posImagen.Y,
                                           spriteDisparo.dimImagen.X, spriteDisparo.dimImagen.Y);

                //spb.Draw(spriteDisparo.imagen, posicion + posCentroTorre, rectangulo, Color.White * alpha, giro, spriteDisparo.center, scale, SpriteEffects.None, 0f);
                spb.Draw(spriteDisparo.imagen, posicion + spriteTorre.center, rectangulo, drawColor * alpha, giro, spriteDisparo.center, scale, SpriteEffects.None, 0f);
            }
        }

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
                spb.Draw(spriteTorre.imagen, posicion + spriteTorre.center, rectangulo, Color.White, 0f, spriteTorre.center, 1f, SpriteEffects.None, 0f);

                //nivel
                Vector2 posNivel = posicion + new Vector2(sprite.dimImagen.X, sprite.dimImagen.Y) - 
                                   new Vector2(3, 1) - font.MeasureString(Nivel.ToString());
                spb.DrawString(font, Nivel.ToString(), posNivel, Color.Black);

                /*
                if (bDisparando)
                {
                    //float percent = ((float)disparoCount / DISPAROMAX);
                    float percent = (float)tiempoPasadoDisparo.TotalMilliseconds / (float)tiempoTotalDisparo.TotalMilliseconds;

                    float scale = percent * ((alcance * 2) / spriteDisparo.dimImagen.X);

                    float giro;
                    if (percent < 0.5f)
                        giro = -(0.5f - percent) * 2;
                    else
                        giro = (percent - 0.5f) * 2;

                    float alpha = 0.7f;

                    Vector2 posCentroTorre = TorreBase.CentroTorre(tipoTorre, tablero);
                    rectangulo = new Rectangle(spriteDisparo.posImagen.X, spriteDisparo.posImagen.Y,
                                               spriteDisparo.dimImagen.X, spriteDisparo.dimImagen.Y);

                    spb.Draw(spriteDisparo.imagen, posicion + posCentroTorre, rectangulo, Color.White * alpha, giro, spriteDisparo.center, scale, SpriteEffects.None, 0f);
                    //spb.Draw(spriteDisparo.imagen, posicion + spriteTorre.center, rectangulo, Color.White * alpha, giro, spriteDisparo.center, scale, SpriteEffects.None, 0f);
                }
                */
            }
        }

    }
}
