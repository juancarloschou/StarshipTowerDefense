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

    class TorreDisparo
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        private const float DISTANCIAALCANZARENEMIGO = 5;
        //private const float DISTANCIAEXPLOSIONCANON = 30;

        private Vector2 posicion;
        public Vector2 movimiento; //vector de longitud 1, representa la direccion

        private Vector2 posicionEnemigo;
        private int idEnemigo;
        
        private Vector2 posicionTorre;
        private int idTorre;
        //private float alcanceTorre;
        
        //caracteristicas
        private float velocidad;    //velocidad del desiparo
        private int daño;           //daño del disparo
        private int radioExplosion; //radio de la explosion de armas tipo cañon

        public bool bEliminar;

        public bool bRotacion;      //disparo tiene rotacion
        public bool bAnimacion;     //disparo es animado
        public Animacion animacion; //animacion del disparo

        public eTipoTorreDisparo tipoDisparo;


        ////////////////// PUBLICA //////////////////
        /*public int Daño
        {
            get { return daño; }
            //set { daño = value; }
        }*/

        public float RotateMovimiento
        {
            get 
            {
                if (bRotacion)
                    return (float)Math.Atan2(movimiento.Y, movimiento.X);
                else
                    return 0f;
            }
        }


        ////////////////// EVENTOS //////////////////
        //public event EventHandler FueraDeAlcance;


        ////////////////// CONSTRUCTOR //////////////////
        public TorreDisparo(GameTime gameTime, Vector2 posicionTorre, Point dimensionesTorre, int idTorre, int daño, 
                            float velocidad, int radioExplosion, Vector2 posicionEnemigo, int idEnemigo, eTipoTorreDisparo tipoDisparo)
        {
            //ventana = viewport;

            this.bEliminar = false;
            this.velocidad = velocidad;
            this.radioExplosion = radioExplosion;
            this.daño = daño;

            this.posicionEnemigo = posicionEnemigo;
            this.idEnemigo = idEnemigo;
            this.posicionTorre = posicionTorre;
            //this.alcanceTorre = alcanceTorre;
            this.idTorre = idTorre;

            this.tipoDisparo = tipoDisparo;

            if ((tipoDisparo == eTipoTorreDisparo.Canon) || (tipoDisparo == eTipoTorreDisparo.Misil))
            {
                bRotacion = true;
                bAnimacion = true;
                animacion = new Animacion(gameTime, 4, TimeSpan.FromSeconds(0.20f), true);
            }
            else
            {
                bRotacion = false;
                bAnimacion = false;
                animacion = null;
            }

            //Vector2 direccion = Matematicas.Vector(posicionTorre, posicionEnemigo);

            posicion = posicionTorre; //centro torre... // + direccion + dimensionesTorre //???
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public void EliminarEnemigo(int idEnemigo)
        {
            if (this.idEnemigo == idEnemigo)
            {
                bEliminar = true;
            }
        }


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, Nave nave, Enemigos enemigos, Animaciones animaciones, Sprite sprite, Opciones opciones)
        {
            Vector2 previaPosicion = posicionEnemigo;

            if (!bEliminar)
            {
                try
                {
                    if (enemigos.enemigos[idEnemigo].bEliminar)
                    {
                        bEliminar = true;
                    }
                    else
                    {
                        posicionEnemigo = enemigos.enemigos[idEnemigo].posicion; //nueva posicion enemigo, siempre lo alcanza
                    }
                }
                catch
                {
                    bEliminar = true;
                }
            }

            if (!bEliminar)
            {
                /*if (Matematicas.Distancia(previaPosicion, posicionEnemigo) > 100)
                {
                    string s = "el disparo se va a por otro enemigo q no es el mismo";
                    Console.WriteLine(s);
                }*/

                movimiento = Matematicas.Vector(posicion, posicionEnemigo);
                movimiento.Normalize();

                //we'll use move Speed and elapsed Time to find the how far the tank moves
                posicion += movimiento * velocidad * (float)gameTime.ElapsedGameTime.TotalSeconds;

                //alcanzar al enemigo
                float distancia = Matematicas.Distancia(posicion, posicionEnemigo);
                if (distancia <= DISTANCIAALCANZARENEMIGO)
                {
                    bEliminar = true;
                    
                    //restar vida enemigo - daño
                    if(radioExplosion == 0)
                    {
                        enemigos.enemigos[idEnemigo].RecibeAtaqueTorre(daño, nave, opciones);
                    }
                    else
                    {
                        //buscar enemigos dentro del radio de explosion
                        EnemigoBase enemigo;
                        float distExplosion;
                        for (int iEnemigo = 0; iEnemigo < enemigos.enemigos.Count; iEnemigo++)
                        {
                            enemigo = enemigos.enemigos[iEnemigo];
                            if (!enemigo.bEliminar)
                            {
                                distExplosion = Matematicas.Distancia(posicion, enemigo.posicion);
                                if (distExplosion <= radioExplosion)
                                {
                                    enemigo.RecibeAtaqueTorre(daño, nave, opciones);
                                }
                            }
                        }
                        //explosion cañon
                        animaciones.AddAnimacion(gameTime, eAnimacionSprite.ExplosionCanon, posicion);
                        Sonidos.PlayFX(eSonido.ExplosionCanon, opciones);
                    }
                }

                /*
                float distancia = Matematicas.Distancia(posicionTorre, posicion);
                if (distancia > alcanceTorre) //el disparo se sale de los limites
                {
                    //FueraDeAlcance(this, null);
                    bEliminar = true;
                }
                */

                if (bAnimacion)
                {
                    animacion.Update(gameTime, sprite);
                }
            }
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb, Sprite sprite)
        {
            if (!bEliminar)
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
}
