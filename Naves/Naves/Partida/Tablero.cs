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
    //por donde entran y salen los bichos
    class Puerta
    {
        public Point coordTopLeft; //posicion arriba izquierda puerta (en casillas)
        public Point coordBottomRight; //posicion abajo derecha puerta (en casillas)
        public bool bEntrada; //true entrada, false salida
        public int idPuerta; //posicion en la lista de puertas
    }


    class Tablero
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public Sprite sprite = new Sprite(); //sprite del muro q rodea el tablero

        public int[,] tablero; //tiene utilidad al buscar las torres en el tablero
        public byte[,] bTablero; //indica para cada casilla si hay una torre (2), está libre (0), o hay muro (1)

        public const byte BTABLERO_LIBRE = 0;
        public const byte BTABLERO_MURO = 1;
        public const byte BTABLERO_TORRE = 2;

        public Point dimTablero; //elimina GetUpperBound, son las dimesiones del tablero
        public Point tamCasillas = new Point(25, 25); //tamaño de las casillas del tablero

        public List<Puerta> puertas; //entradas y salidas de enemigos

        //limites de la ventana (usa la ventana total)
        //private Viewport ventana;
        //private Viewport ventanaGame; //para saber donde colocar las torres


        ////////////////// PUBLICAS //////////////////

        public Vector2 TamCasillas
        {
            get { return new Vector2(tamCasillas.X, tamCasillas.Y); }
        }

        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public Tablero(Point dimensionesTablero) //, Viewport VentanaGame)
        {
            //this.ventana = Ventana;
            //this.ventanaGame = VentanaGame;

       
            this.dimTablero = dimensionesTablero; //28*24 + muro
            tablero = new int[dimTablero.X, dimTablero.Y];

            for (int x = 0; x < dimTablero.X; x++)
            {
                for (int y = 0; y < dimTablero.Y; y++)
                {
                    tablero[x, y] = -1;
                }
            }

            bTablero = new byte[dimTablero.X, dimTablero.Y];
            //TableroBool(null); //calcula bTablero
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        //transforma coordenadas de ventana en casillas del tablero
        //para ver en q casilla se pulsa el raton
        public Vector2 CoordToPosicion(Point coordenadas)
        {
            Vector2 posicion;

            //posicion = new Vector2(coordenadas.X * tamCasillas.X + tamCasillas.X, coordenadas.Y * tamCasillas.Y + tamCasillas.X);
            posicion = new Vector2(coordenadas.X * tamCasillas.X, coordenadas.Y * tamCasillas.Y);

            return posicion;
        }


        //transforma casillas del tablero en coordenadas de ventana
        //para ver donde se dibujan las torres
        public Point PosicionToCoord(Vector2 posicion)
        {
            Point coord;

            //coord = new Point((int)(posicion.X / tamCasillas.X) - tamCasillas.X, (int)(posicion.Y / tamCasillas.Y) - tamCasillas.Y);
            coord = new Point((int)(posicion.X / tamCasillas.X), (int)(posicion.Y / tamCasillas.Y));

            return coord;
        }

        public Point PosicionToCoord(Point posicion)
        {
            Point coord;

           // coord = new Point((int)(posicion.X / tamCasillas.X) - tamCasillas.X, (int)(posicion.Y / tamCasillas.Y) - tamCasillas.Y);
            coord = new Point((int)(posicion.X / tamCasillas.X), (int)(posicion.Y / tamCasillas.Y));

            return coord;
        }


        /// <summary>
        /// Inserta torre en el tablero
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="torre"></param>
        /// <returns>Devuelve 0 si OK, 1 si fuera limites, 2 si colisiona con otra torre, 3 si cierra los caminos posibles, 
        /// 4 si hay un enemigo en esa posicion, 5 si esta recalculando caminos, 6 si no hay dinero</returns>
        public int AddTorre(GameTime gameTime, Point coord, eTipoTorre tipoTorre, Torres torres, Enemigos enemigos, Nave nave)
        {
            //casillas q ocupa la torre
            Point tamTorre = TorreBase.CasillasTorre(tipoTorre, this);

            //ver si esta en los limites del tablero
            if ((coord.X < 1) || (coord.X + tamTorre.X >= dimTablero.X) ||
                (coord.Y < 1) || (coord.Y + tamTorre.Y >= dimTablero.Y))
                return 1;

            //ver si hay colisiona con otra torre del tablero
            bool bCabe = true;
            int x, y;
            for (x = coord.X; (x < coord.X + tamTorre.X) && bCabe; x++)
            {
                for (y = coord.Y; (y < coord.Y + tamTorre.Y) && bCabe; y++)
                {
                    //tenemos los 4 puntos de la nueva torre, hay q recorrer TableroBool a ver si esta libre
                    if (bTablero[x, y] != BTABLERO_LIBRE)
                        bCabe = false;
                }
            }
            if (!bCabe)
                return 2;


            //copiar tablero y torres para añadirle la nueva torre
            Tablero futuroTablero = new Tablero(dimTablero);
            for (x = 0; x < dimTablero.X; x++)
                for (y = 0; y < dimTablero.Y; y++)
                    futuroTablero.tablero[x, y] = tablero[x, y];
            futuroTablero.puertas = puertas;
            Torres futuroTorres = new Torres();
            foreach (TorreBase torre in torres.torres)
                futuroTorres._TableroAddTorre(torre.TipoTorre, torre.coordenadas);
            int idFuturo = futuroTorres._TableroAddTorre(tipoTorre, coord);
            futuroTablero.tablero[coord.X, coord.Y] = idFuturo;
            futuroTablero.TableroBool(futuroTorres);
            //ver si cierra los caminos posibles
            bool bCierra = false;
            List<Point> entradas = Entradas();
            Point point;
            for (int iPoint = 0; (iPoint < entradas.Count) && !bCierra; iPoint++)
            {
                point = entradas[iPoint];
                Pathfinder pathFinder = new Pathfinder(eSearchTipo.Terrestre);
                pathFinder.Initialize(futuroTablero, point);

                if (pathFinder.SearchStatus != eSearchStatus.PathFound)
                    bCierra = true;
            }
            if (bCierra)
                return 3;


            //if (enemigos.bRecalcularCaminos)
            //    return 5; //desactivado, q calcule caminos todo el tiempo

            CaracteristicasTorre caracteristicas = TorreBase.GetCaracteristicasOpcionTorre(tipoTorre);
            int precio = caracteristicas.precio;

            if (nave.Dinero < precio)
                return 6;

            //todo OK, creamos torre en la lista y en tablero
            int id = torres._TableroAddTorre(tipoTorre, coord); //añade torre a lista de torres
            tablero[coord.X, coord.Y] = id;
            TableroBool(torres); //calcula bTablero
            nave.PagarCrearTorre(precio); //paga el precio

            Vector2 centroTorre = TorreBase.CentroTorre(tipoTorre, this);
            enemigos.SetRecalcularCaminos(CoordToPosicion(coord) + centroTorre, gameTime);
            //da orden a enemigos de q empiece a recalcular caminos, le pasa la posicion de la torre, en el centro de las 4 casillas
            return 0;
        }


         /// <summary>
        /// Borra torre del tablero, al final de una venta
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="torre"></param>
        public void DeleteTorre(GameTime gameTime, int id, Nave nave, Torres torres, Enemigos enemigos)
        {
            nave.Dinero += nave.DineroVentaTorre(torres.torres[id].Precio); //aumentar el dinero con la venta

            //borramos torre en la lista y en tablero
            eTipoTorre tipoTorre = torres.torres[id].TipoTorre;
            Point coord = torres._TableroDeleteTorre(id); //no cambia lista de torres, pone bExiste a false
            tablero[coord.X, coord.Y] = -1;
            TableroBool(torres); //calcula bTablero

            Vector2 centroTorre = TorreBase.CentroTorre(tipoTorre, this);
            enemigos.SetRecalcularCaminos(CoordToPosicion(coord) + centroTorre, gameTime);
            //da orden a enemigos de q empiece a recalcular caminos, le pasa la posicion de la torre, en el centro de las 4 casillas
        }


        private byte BoolMuro(int x, int y)
        {
            Point coord = new Point(x, y);

            if (ExistePuerta(coord))
                return BTABLERO_LIBRE;
            else
                return BTABLERO_MURO;
        }

        /// <summary>
        ///pone 2 si una casilla esta ocupada por una torre, 0 si esta libre, 1 si hay muro
        /// </summary>
        /// <returns></returns>
        public void TableroBool(Torres torres)
        {
            //vacia tablero
            int x, y;
            for (x = 0; x < dimTablero.X; x++)
            {
                for (y = 0; y < dimTablero.Y; y++)
                {
                    bTablero[x, y] = BTABLERO_LIBRE;
                }
            }


            //poner muro

            //linea de arriba
            y = 0;
            for (x = 1; x < dimTablero.X - 1; x++)
            {
                bTablero[x, y] = BoolMuro(x, y);
            }

            //linea de derecha
            x = dimTablero.X - 1;
            for (y = 0; y < dimTablero.Y; y++)
            {
                bTablero[x, y] = BoolMuro(x, y);
            }

            //linea de abajo
            y = dimTablero.Y - 1;
            for (x = 1; x < dimTablero.X - 1; x++)
            {
                bTablero[x, y] = BoolMuro(x, y);
            }

            //linea de izquierda
            x = 0;
            for (y = 0; y < dimTablero.Y; y++)
            {
                bTablero[x, y] = BoolMuro(x, y);
            }


            //lena tablero con torres
            if (torres != null)
            {
                TorreBase torre;
                for (x = 0; x < dimTablero.X; x++)
                {
                    for (y = 0; y < dimTablero.Y; y++)
                    {
                        if (tablero[x, y] != -1) //id de la torre
                        {
                            //hay torre, llenar las casillas con true
                            torre = torres.torres[tablero[x, y]];

                            //casillas q ocupa la torre
                            Point tamTorre = TorreBase.CasillasTorre(torre.TipoTorre, this);

                            if (!torre.bEliminar)
                            {
                                //recorre las 4 casillas de la torre;
                                for (int tx = x; tx < x + tamTorre.X; tx++)
                                {
                                    for (int ty = y; ty < y + tamTorre.Y; ty++)
                                    {
                                        bTablero[tx, ty] = BTABLERO_TORRE;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //return bTablero;
            /*string s;
            for (x = 0; x < dimensiones.X; x++)
            {
                for (y = 0; y < dimensiones.Y; y++)
                {
                    if(bTablero[x, y])
                        s = "1";
                    else
                        s = "0";
                    Console.Write(s);
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");*/
        }


        //indica si hay una puerta en las coordenadas
        private bool ExistePuerta(Point coord)
        {
            bool bExistePuerta = false;
            Puerta puerta;
            int x, y;
            for (int iPuerta = 0; (iPuerta < puertas.Count) && !bExistePuerta; iPuerta++)
            {
                puerta = puertas[iPuerta];
                if (puerta.coordTopLeft.Y == puerta.coordBottomRight.Y) //horizontal
                {
                    y = puerta.coordTopLeft.Y;
                    for (x = puerta.coordTopLeft.X; (x <= puerta.coordBottomRight.X) && !bExistePuerta; x++)
                    {
                        if ((coord.X == x) && (coord.Y == y))
                            bExistePuerta = true;
                    }
                }
                else
                {
                    if (puerta.coordTopLeft.X == puerta.coordBottomRight.X) //vertical
                    {
                        x = puerta.coordTopLeft.X;
                        for (y = puerta.coordTopLeft.Y; (y <= puerta.coordBottomRight.Y) && !bExistePuerta; y++)
                        {
                            if ((coord.X == x) && (coord.Y == y))
                                bExistePuerta = true;
                        }
                    }
                }

            }

            return bExistePuerta;
        }


        //devuelve lista de puntos que son una salida
        public List<Point> Salidas()
        {
            List<Point> salidas = new List<Point>();
            Puerta puerta;
            int x, y;
            for (int iPuerta = 0; (iPuerta < puertas.Count); iPuerta++)
            {
                puerta = puertas[iPuerta];
                if (puerta.bEntrada == false) //es puerta de salida
                {
                    if (puerta.coordTopLeft.Y == puerta.coordBottomRight.Y) //horizontal
                    {
                        y = puerta.coordTopLeft.Y;

                        for (x = puerta.coordTopLeft.X; (x <= puerta.coordBottomRight.X); x++)
                        {
                            salidas.Add(new Point(x, y));
                        }
                    }
                    else
                    {
                        if (puerta.coordTopLeft.X == puerta.coordBottomRight.X) //vertical
                        {
                            x = puerta.coordTopLeft.X;

                            for (y = puerta.coordTopLeft.Y; (y <= puerta.coordBottomRight.Y); y++)
                            {
                                salidas.Add(new Point(x, y));
                            }
                        }
                    }
                }
            }

            return salidas;
        }

        //devuelve lista de puntos que son una entrada
        public List<Point> Entradas()
        {
            List<Point> entradas = new List<Point>();
            Puerta puerta;
            int x, y;
            for (int iPuerta = 0; (iPuerta < puertas.Count); iPuerta++)
            {
                puerta = puertas[iPuerta];
                if (puerta.bEntrada == true) //es puerta de entrada
                {
                    if (puerta.coordTopLeft.Y == puerta.coordBottomRight.Y) //horizontal
                    {
                        y = puerta.coordTopLeft.Y;

                        for (x = puerta.coordTopLeft.X; (x <= puerta.coordBottomRight.X); x++)
                        {
                            entradas.Add(new Point(x, y));
                        }
                    }
                    else
                    {
                        if (puerta.coordTopLeft.X == puerta.coordBottomRight.X) //vertical
                        {
                            x = puerta.coordTopLeft.X;

                            for (y = puerta.coordTopLeft.Y; (y <= puerta.coordBottomRight.Y); y++)
                            {
                                entradas.Add(new Point(x, y));
                            }
                        }
                    }
                }
            }

            return entradas;
        }


        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager content)
        {
            sprite.imagen = content.Load<Texture2D>("Menu\\muro_01");
            sprite.dimImagen = new Point(25, 25);
            sprite.posImagen = new Point(0, 0);
        }


        ////////////////// DRAW //////////////////
        private void DibujaMuro(SpriteBatch spb, int x, int y, Rectangle rectangulo)
        {
            Vector2 posicion; //, posMuro;
            Point coord;

            coord = new Point(x, y);
            if (ExistePuerta(coord))
            {
                //no dibuja, aunque la de salida deberia marcarla...
            }
            else
            {
                //dibuja muro
                posicion = CoordToPosicion(coord);
                //pasar a posicion absoluta (ventana total)
                //posMuro = new Vector2(posicion.X + ventanaGame.X, posicion.Y + ventanaGame.Y);
                spb.Draw(sprite.imagen, posicion, rectangulo, Color.White);
            }
        }

        public void Draw(SpriteBatch spb)
        {
            int x, y;
            Rectangle rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, sprite.dimImagen.X, sprite.dimImagen.Y);

            //linea de arriba
            y = 0;
            for (x = 1; x < dimTablero.X - 1; x++)
            {
                DibujaMuro(spb, x, y, rectangulo);
            }

            //linea de derecha
            x = dimTablero.X - 1;
            for (y = 0; y < dimTablero.Y; y++)
            {
                DibujaMuro(spb, x, y, rectangulo);
            }

            //linea de abajo
            y = dimTablero.Y - 1;
            for (x = 1; x < dimTablero.X - 1; x++)
            {
                DibujaMuro(spb, x, y, rectangulo);
            }

            //linea de izquierda
            x = 0;
            for (y = 0; y < dimTablero.Y; y++)
            {
                DibujaMuro(spb, x, y, rectangulo);
            }


        }

    }
}
