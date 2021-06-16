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
    class Disparo
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        //private const float DISTANCIAALCANZARENEMIGO = 5;

        //private Texture2D imagen;
        public Vector2 posicion;
        public Vector2 movimiento; //vector normalizado de movimiento del disparo

        public Vector2 direccion; //direccion del movimiento, es constante desde que empieza, expecto misiles

        public Vector2 previaPosicion; //para colisiones paso a paso necesito posicion previa

        //limites de la ventana
        //private Viewport ventana;

        private bool bAnimacion;
        private Animacion animacion;

        private eTipoDisparo tipoDisparo;

        public bool bEliminar;

        //caracteristicas
        private float velocidad;    //velocidad del desiparo
        private int daño;           //daño del disparo vs tierra
        private int dañoAire;       //daño del disparo vs aire
        public int radioMisiles;    //solo para misiles, radio de deteccion de enemigos
        private int apuntaEnemigo;  //solo para misiles, a quien apunta la torre, id enemigo


        ////////////////// PUBLICA //////////////////
        public int Daño
        {
            get { return daño; }
        }
        public int DañoAire
        {
            get { return dañoAire; }
        }

        public float RotateMovimiento
        {
            get { return (float)Math.Atan2(movimiento.Y, movimiento.X); }
        }

        public eTipoDisparo TipoDisparo
        {
            get { return tipoDisparo; }
        }

 
        ////////////////// EVENTOS //////////////////
        public event EventHandler FueraDePantalla;


        ////////////////// CONSTRUCTOR //////////////////
        public Disparo(GameTime gameTime, eTipoDisparo tipoDisparo, Vector2 direccion, Vector2 posicionDisparo, 
                       Vector2 posicionNave, Sprite sprite, CaracteristicasTipoDisparo caract)
        {
            this.bEliminar = false;

            this.direccion = direccion;
            this.velocidad = caract.velocidad;
            this.daño = caract.daño;
            this.dañoAire = caract.dañoAire;
            this.radioMisiles = caract.radioMisiles;
            this.apuntaEnemigo = -1;

            this.tipoDisparo = tipoDisparo;
            if (tipoDisparo == eTipoDisparo.Misiles)
            {
                bAnimacion = true;
                animacion = new Animacion(gameTime, 4, TimeSpan.FromSeconds(0.20f), true);
            }
            else
            {
                bAnimacion = false;
                animacion = null;
            }

            this.posicion = posicionNave + posicionDisparo;
            this.posicion += new Vector2(-sprite.center.Y, 0);
            if (tipoDisparo == eTipoDisparo.Misiles)
                this.posicion.X += 1; //ajustes para centrar disparo
            else
                this.posicion.X -= 1;
            this.previaPosicion = this.posicion;


            /*if ((direccion.X == 0) && (direccion.Y == -1)) //hacia arriba
            {
                posicion.X -= 2;
                posicion.Y -= dimensionesNave.Y / 2 + sprite.dimImagen.Y / 2;

                //movemos un poco la posicion del disparo, para que salga desde el centro de la nave y no desde una esquina
                //posicion.X += (dimensionesNave.X / 2);

                //lo que acabamos de centrar es la esquina superior izquierda del disparo. Así situaremos el centro alineado con el centro de la imagen
                //los 2 pixeles extra es por que la imagen del disparo no esta perfectamente centrada.
                //posicion.X -= (dimensiones.X / 2) + 2;
            }
            */
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// LOAD CONTENT //////////////////
        /*
        public static void LoadContent(ContentManager Content)
        {
        }
        */


        ////////////////// UPDATE ////////////////////////

        private void UpdateApuntarMisiles(Enemigos enemigos)
        {
            //detectar enemigos proximos
            if (apuntaEnemigo == -1)
            {
                EnemigoBase enemigo;
                float distancia;
                float menorDistancia = float.MaxValue;
                int enemigoCercano = -1;
                for (int iEnemigo = 0; iEnemigo < enemigos.enemigos.Count; iEnemigo++)
                {
                    enemigo = enemigos.enemigos[iEnemigo];
                    if (!enemigo.bEliminar)
                    {
                        distancia = Matematicas.Distancia(enemigo.posicion, posicion);
                        if (distancia < radioMisiles)
                            if (distancia < menorDistancia)
                            {
                                menorDistancia = distancia;
                                enemigoCercano = iEnemigo;
                            }
                    }
                }
                if (enemigoCercano != -1)
                {
                    apuntaEnemigo = enemigoCercano;
                }
            }
        }

        public void Update(GameTime gameTime, Sprite sprite, Viewport ventana, Enemigos enemigos)
        {
            if (tipoDisparo == eTipoDisparo.Misiles)
            {
                UpdateApuntarMisiles(enemigos);
            }


            if (apuntaEnemigo != -1) //sigue a un enemigo (misiles)
            {
                if (!enemigos.enemigos[apuntaEnemigo].bEliminar)
                {
                    Vector2 posicionEnemigo = enemigos.enemigos[apuntaEnemigo].posicion;
                    movimiento = Matematicas.Vector(posicion, posicionEnemigo);
                    movimiento.Normalize();
                }
                else
                    apuntaEnemigo = -1; //hay q buscar otro objetivo
            }
            if (apuntaEnemigo == -1) //no misiles o misiles sin objetivo
            {
                movimiento = direccion; //direccion inicial
            }


            previaPosicion = posicion; //para colision exhaustiva paso a paso

             //we'll use move Speed and elapsed Time to find the how far the tank moves
            posicion += movimiento * velocidad * (float)gameTime.ElapsedGameTime.TotalSeconds;


            //el disparo se sale de los limites pantalla

            int dim = Math.Max(sprite.dimImagen.X, sprite.dimImagen.Y);
            if ((posicion.Y + dim < 0) || (posicion.Y - dim > ventana.Height) ||
                (posicion.X + dim < 0) || (posicion.X - dim > ventana.Width))
            {
                FueraDePantalla(this, null);
            }


            if (bAnimacion)
            {
                animacion.Update(gameTime, sprite);
            }
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb, Sprite sprite)
        {
            if (bAnimacion)
            {
                animacion.Draw(spb, sprite, posicion, RotateMovimiento);
            }
            else
            {
                Rectangle rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, sprite.dimImagen.X, sprite.dimImagen.Y);

                spb.Draw(sprite.imagen, posicion, rectangulo, Color.White, RotateMovimiento, sprite.center, 1f, SpriteEffects.None, 0f);
            }

        }
    }
}
