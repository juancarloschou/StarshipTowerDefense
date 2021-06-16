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

    /// <summary>
    /// Enemigo terrestre, el que camina por el tablero, rodeando las torres
    /// </summary>
    abstract class EnemigoTerrestre : EnemigoBase
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////


        protected PathfinderPointList camino; //esta clase ha quedado sin ninguna funcionalidad interna !!!

        //las posicion del nodo actual del camino (hacia donde va)
        private Vector2 destination;

        const float atDestinationLimit = 5f; //en q momento consideramos q ha llegado a su destino

        private Vector2 previoDestination = Matematicas.NOVECTOR;

        private bool bRecalcularCaminos;
        private bool bMarcadoRecalcularCaminos; //marca para recalcular


        ////////////////// PUBLICAS //////////////////
        public bool bMarcado
        {
            get { return bMarcadoRecalcularCaminos; }
            set
            {
                bMarcadoRecalcularCaminos = value;
                bRecalcularCaminos = value;
            }
        }

        public int TamañoCamino
        {
            get 
            {
                if (camino != null)
                    return camino.Count;
                else
                    return int.MaxValue;
            }
        }

        public Vector2 PosicionNodoCamino
        {
            get 
            {
                if (camino != null)
                    return camino.Peek();
                else
                    return new Vector2(int.MaxValue, int.MaxValue);
            }
        }


        ////////////////// EVENTOS //////////////////

        
        ////////////////// CONSTRUCTOR //////////////////
        public EnemigoTerrestre(Vector2 posicion, Tablero tablero, eTipoEnemigo tipoEnemigo, int nivel = 0, bool bAnimacion = true)
            : base(posicion, eTipoMovimiento.Terrestre, tipoEnemigo, nivel, bAnimacion)
        {
            bRecalcularCaminos = false;

            CalcularCamino(tablero, false);
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        /// <summary>
        /// Linear distance to the Tanks' current destination
        /// </summary>
        public float DistanceToDestination()
        {
            return Vector2.Distance(posicion, destination);
        }

        /// <summary>
        /// True when the tank is "close enough" to it's destination
        /// </summary>
        public bool AtDestination()
        {
            return DistanceToDestination() < atDestinationLimit;
        }


        public void CalcularCamino(Tablero tablero, bool bRecalcular)
        {
            camino = null;
            Pathfinder pathFinder = new Pathfinder(eSearchTipo.Terrestre);

            Point coordEnemigo = tablero.PosicionToCoord(posicion); //posicion inicial la actual del enemigo

            //DateTime timeIni = DateTime.Now;
            pathFinder.Initialize(tablero, coordEnemigo);
            //DateTime timeFin = DateTime.Now;
            //TimeSpan tiempo = timeFin - timeIni;

            if (pathFinder.SearchStatus == eSearchStatus.PathFound)
            {
                camino = new PathfinderPointList();
                Vector2 pos = new Vector2();
                bool bHayCamino = false;
                foreach (Point point in pathFinder.FinalPath(tablero))
                {
                    //guarda el camino final en posicion de pantalla game
                    pos = tablero.CoordToPosicion(point);
                    //colocar en el centro del las casillas tablero
                    pos = new Vector2(pos.X + tablero.tamCasillas.X / 2,
                                      pos.Y + tablero.tamCasillas.Y / 2);

                    camino.Enqueue(pos);
                    bHayCamino = true;
                }
                //añade la casilla debajo de la salida para q se marchen por ahi debajo
                if (bHayCamino)
                {
                    pos = new Vector2(pos.X, pos.Y + tablero.tamCasillas.Y * 2);
                    camino.Enqueue(pos);
                }

                if (bRecalcular)
                {
                    //comprueba si el siguiente nodo está detras del enemigo para saltarselo y q no regrese
                    //mira si el enemigo ya está entre el primer nodo y el siguiente
                    //mira si el primero nodo del camino es el previoNodo
                    if (camino.Peek().Equals(previoDestination))
                    {
                        camino.Dequeue();
                    }

                }

                camino.Num = camino.Count;
            }

        }


        ////////////////// LOAD CONTENT //////////////////
 

        ////////////////// UPDATE ////////////////////////
        public override void UpdateMovimiento(GameTime gameTime, Tablero tablero, Nave nave, Animaciones animaciones, Opciones opciones)
        {
            if (bRecalcularCaminos)
            {
                CalcularCamino(tablero, true);
                bRecalcularCaminos = false; //la marca bMarcado se mantiene para q no lo vuelva a marcar
            }

            if (camino == null)
            {
                Console.WriteLine("enemigo no se mueve");
            }
            else
            {
                // If we have any waypoints, the first one on the list is where we want to go
                if (camino.Count >= 1)
                {
                    destination = camino.Peek();
                }

                // If we’re at the destination and there is at least one waypoint in 
                // the list, get rid of the first one since we’re there now
                bool bAtDestination = AtDestination();
                if (bAtDestination)
                {
                    if (camino.Count >= 1)
                    {
                        previoDestination = destination;
                        camino.Dequeue();
                    }
                    else
                    {
                        //llega a la salida, eliminar enemigo
                        FinalCamino(gameTime, nave, animaciones, opciones);
                        bEliminar = true;
                    }
                }

                if (!bAtDestination)
                {
                    previaPosicion = posicion; //para colision exhaustiva paso a paso

                    movimiento = -(posicion - destination);
                    //This scales the vector to 1
                    movimiento.Normalize();

                    //we'll use move Speed and elapsed Time to find the how far the tank moves
                    posicion += movimiento * Velocidad * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

        }

        ////////////////// DRAW //////////////////
        /*
        public void Draw(SpriteBatch spb, Sprite sprite)
        {
            base.Draw(spb, sprite);

            //marcar enemigos q no se mueven, provisional
            //if (camino == null)
            //{
            //    //draw de dibujarlineas es MUY costoso

            //    DibujarLineas dp = new DibujarLineas(graphicsDevice);
            //    dp.Colour = Color.Green;
            //    dp.CreateCircle(posicion.X, posicion.Y, 30, 10);
            //    dp.Render(spb);
            //}

        }
        */
    }
}
