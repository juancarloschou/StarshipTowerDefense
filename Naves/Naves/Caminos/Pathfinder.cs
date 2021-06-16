#region File Description
//-----------------------------------------------------------------------------
// PathFinder.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Naves
{
    #region Search Status Enum
    public enum eSearchStatus
    {
        Stopped,
        Searching,
        NoPath,
        PathFound,
    }
    #endregion

    #region Search Method Enum
    public enum eSearchMethod
    {
        BreadthFirst,
        BestFirst,
        AStar,
        Max,
    }
    #endregion

    public enum eSearchTipo
    {
        Terrestre,
        Aereo
    }

    class Pathfinder
    {
        #region Search Node Struct
        /// <summary>
        /// Reresents one node in the search space
        /// </summary>
        private struct SearchNode
        {
            /// <summary>
            /// Location on the map
            /// </summary>
            public Point Position;

            /// <summary>
            /// Distance to goal estimate
            /// </summary>
            public int DistanceToGoal;
            
            /// <summary>
            /// Distance traveled from the start
            /// </summary>
            public int DistanceTraveled;

            public SearchNode(
                Point mapPosition, int distanceToGoal, int distanceTraveled)
            {
                Position = mapPosition;
                DistanceToGoal = distanceToGoal;
                DistanceTraveled = distanceTraveled;
            }
        }
        #endregion


        #region Constants        

        /// <summary>
        /// Scales the draw size of the search nodes
        /// </summary>
        //const float searchNodeDrawScale = .75f;

        #endregion

        #region Fields

        private eSearchTipo searchTipo;

        //Draw data
        /*public Texture2D nodeTexture;
        public Vector2 nodeTextureCenter;*/

        /*private Color openColor = Color.Green;
        private Color closedColor = Color.Red;*/

        // How much time has passed since the last search step
        private float timeSinceLastSearchStep = 0f;
        // Holds search nodes that are avaliable to search
        private List<SearchNode> openList;
        // Holds the nodes that have already been searched
        private List<SearchNode> closedList;
        // Holds all the paths we've creted so far
        private Dictionary<Point, Point> paths;

        // Seconds per search step        
        //public float timeStep = .5f; //inicialmente, se cambia con barrita


        private Point entrada;
        private List<Point> salidas;
        private Point caminoSalida; //una vez el camino buscado llegó a una salida, cual era esta

        #endregion
        
        #region Properties

        // Tells us if the search is stopped, started, finished or failed
        public eSearchStatus SearchStatus
        {
            get { return searchStatus; }
        }
        private eSearchStatus searchStatus;

        // Tells us which search type we're using right now
        /*public SearchMethod SearchMethod
        {
            get { return searchMethod; }
        }*/
        private eSearchMethod searchMethod = eSearchMethod.BestFirst; //eSearchMethod.AStar;

        /*public float Scale
        {
            get { return scale; }
            set { scale = value * searchNodeDrawScale; }
        }
        private float scale = 1f;*/
        
        // Seconds per search step
        /*public float TimeStep
        {
            get { return timeStep; }
            set { timeStep = value; }
        }*/

        /// <summary>
        /// Toggles searching on and off
        /// </summary>
        public bool IsSearching
        {
            get { return searchStatus == eSearchStatus.Searching; }
            set 
            {
                if (searchStatus == eSearchStatus.Searching)
                {
                    searchStatus = eSearchStatus.Stopped;
                }
                else if (searchStatus == eSearchStatus.Stopped)
                {
                    searchStatus = eSearchStatus.Searching;
                }
            }
        }

        /// <summary>
        /// How many search steps have elapsed on this map
        /// </summary>
        public int TotalSearchSteps
        {
            get { return totalSearchSteps; }
        }
        private int totalSearchSteps = 0;

        #endregion


        #region Initialization

        public Pathfinder(eSearchTipo searchTipo)
        {
            this.searchTipo = searchTipo;
        }

        /// <summary>
        /// Setup search
        /// </summary>
        /// <param name="mazeMap">Map to search</param>
        public void Initialize(Tablero tablero, Point coordStart)
        {
            //searchStatus = SearchStatus.Stopped;
            searchStatus = eSearchStatus.Searching;
            openList = new List<SearchNode>();
            closedList = new List<SearchNode>();
            paths = new Dictionary<Point, Point>();

            entrada = coordStart;
            salidas = tablero.Salidas();

            openList.Add(new SearchNode(entrada, DistanciaASalidas(entrada), 0));

            while (searchStatus == eSearchStatus.Searching)
            {
                DoSearchStep(tablero); //LOGICA DE PATH FIND
            }
        }

        /// <summary>
        /// Load the Draw texture
        /// </summary>
        /*public void LoadContent(ContentManager content)
        {
            nodeTexture = content.Load<Texture2D>("Pathfinder\\dot");
            nodeTextureCenter = new Vector2(nodeTexture.Width / 2, nodeTexture.Height / 2);
        }
        */
        #endregion

        #region Update and Draw

        /// <summary>
        /// Search Update
        /// </summary>
        /*
        public void Update(GameTime gameTime, Tablero tablero)
        {
            if (searchStatus == SearchStatus.Searching)
            {
                timeSinceLastSearchStep += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timeSinceLastSearchStep >= timeStep)
                {
                    DoSearchStep(tablero); //LOGICA DE PATH FIND
                    timeSinceLastSearchStep = 0f;
                }
            }
        }
        */

        /// <summary>
        /// Draw the search space
        /// </summary>
        /*
        public void Draw(SpriteBatch spb, Viewport ventanaGame, Tablero tablero)
        {
            if (searchStatus != SearchStatus.PathFound)
            {
                Vector2 posicion;

                //spb.Begin();
                foreach (SearchNode node in openList)
                {
                    posicion = tablero.CoordToPosicion(node.Position);
                    //pasar a posicion absoluta (ventana total)
                    posicion = new Vector2(posicion.X + ventanaGame.X, posicion.Y + ventanaGame.Y);
                    //colocar en el centro casilla
                    posicion = new Vector2(posicion.X + tablero.tamCasillas.X / 2,
                                           posicion.Y + tablero.tamCasillas.Y / 2);

                    spb.Draw(nodeTexture,
                        posicion, null, openColor, 0f,
                        nodeTextureCenter, scale, SpriteEffects.None, 0f);
                }
                foreach (SearchNode node in closedList)
                {
                    posicion = tablero.CoordToPosicion(node.Position);
                    //pasar a posicion absoluta (ventana total)
                    posicion = new Vector2(posicion.X + ventanaGame.X, posicion.Y + ventanaGame.Y);
                    //colocar en el centro casilla
                    posicion = new Vector2(posicion.X + tablero.tamCasillas.X / 2,
                                           posicion.Y + tablero.tamCasillas.Y / 2);

                    spb.Draw(nodeTexture,
                        posicion, null, closedColor, 0f,
                        nodeTextureCenter, scale, SpriteEffects.None, 0f);
                }
                //spb.End();
            }


            //pinta casilla de inicio y final
            Color startColor = Color.Red;
            Color exitColor = Color.Blue;

            Vector2 tilePosition = tablero.CoordToPosicion(StartTile);
            //pasar a posicion absoluta (ventana total)
            tilePosition = new Vector2(tilePosition.X + ventanaGame.X, tilePosition.Y + ventanaGame.Y);
            //colocar en el centro casilla
            tilePosition = new Vector2(tilePosition.X + tablero.tamCasillas.X / 2,
                                       tilePosition.Y + tablero.tamCasillas.Y / 2);
            
            spb.Draw(nodeTexture, tilePosition, null, startColor, 0f, nodeTextureCenter, 1f, SpriteEffects.None, .25f);


            tilePosition = tablero.CoordToPosicion(EndTile);
            //pasar a posicion absoluta (ventana total)
            tilePosition = new Vector2(tilePosition.X + ventanaGame.X, tilePosition.Y + ventanaGame.Y);
            //colocar en el centro casilla
            tilePosition = new Vector2(tilePosition.X + tablero.tamCasillas.X / 2,
                                       tilePosition.Y + tablero.tamCasillas.Y / 2);

            spb.Draw(nodeTexture, tilePosition, null, exitColor, 0f, nodeTextureCenter, 1f, SpriteEffects.None, .25f);


        }
        */
        #endregion

        #region Methods

        /// <summary>
        /// This method find the next path node to visit, puts that node on the 
        /// closed list and adds any nodes adjacent to the visited node to the 
        /// open list.
        /// </summary>
        private void DoSearchStep(Tablero tablero)
        {
            SearchNode newOpenListNode;

            bool foundNewNode = SelectNodeToVisit(out newOpenListNode);
            if (foundNewNode)
            {
                Point currentPos = newOpenListNode.Position;
                foreach (Point point in OpenMapTiles(currentPos, tablero))
                {
                    SearchNode mapTile = new SearchNode(point, DistanciaASalidas(point), newOpenListNode.DistanceTraveled + 1);
                    if (!InList(openList,point) &&
                        !InList(closedList,point))
                    {
                        openList.Add(mapTile);
                        paths[point] = newOpenListNode.Position;
                    }
                }
                if (EsSalida(currentPos))
                {
                    caminoSalida = currentPos;
                    searchStatus = eSearchStatus.PathFound;
                }
                openList.Remove(newOpenListNode);
                closedList.Add(newOpenListNode);
            }
            else
            {
                searchStatus = eSearchStatus.NoPath;
            }
        }

        /// <summary>
        /// Determines if the given Point is inside the SearchNode list given
        /// </summary>
        private static bool InList(List<SearchNode> list, Point point)
        {
            bool inList = false;
            for (int i = 0; (i < list.Count) && !inList; i++)
            {
                if (list[i].Position == point)
                {
                    inList = true;
                }
            }
            return inList;
        }

        /// <summary>
        /// This Method looks at everything in the open list and chooses the next 
        /// path to visit based on which search type is currently selected.
        /// </summary>
        /// <param name="result">The node to be visited</param>
        /// <returns>Whether or not SelectNodeToVisit found a node to examine
        /// </returns>
        private bool SelectNodeToVisit(out SearchNode result)
        {
            result = new SearchNode();
            bool success = false;
            float smallestDistance = float.PositiveInfinity;
            float currentDistance = 0f;
            if (openList.Count > 0)
            {
                switch (searchMethod)
                {
                    // Breadth first search looks at every possible path in the 
                    // order that we see them in.
                    case eSearchMethod.BreadthFirst:
                        totalSearchSteps++;
                        result = openList[0];
                        success = true;
                        break;
                    // Best first search always looks at whatever path is closest to
                    // the goal regardless of how long that path is.
                    case eSearchMethod.BestFirst:
                        totalSearchSteps++;
                        foreach (SearchNode node in openList)
                        {
                            currentDistance = node.DistanceToGoal;
                            if(currentDistance < smallestDistance){
                                success = true;
                                result = node;
                                smallestDistance = currentDistance;
                            }
                        }
                        break;
                    // A* search uses a heuristic, an estimate, to try to find the 
                    // best path to take. As long as the heuristic is admissible, 
                    // meaning that it never over-estimates, it will always find 
                    // the best path.
                    case eSearchMethod.AStar:
                        totalSearchSteps++;
                        foreach (SearchNode node in openList)
                        {
                            currentDistance = Heuristic(node);
                            // The heuristic value gives us our optimistic estimate 
                            // for the path length, while any path with the same 
                            // heuristic value is equally ‘good’ in this case we’re 
                            // favoring paths that have the same heuristic value 
                            // but are longer.
                            if (currentDistance <= smallestDistance)
                            {
                                if (currentDistance < smallestDistance)
                                {
                                    success = true;
                                    result = node;
                                    smallestDistance = currentDistance;
                                }
                                else if (currentDistance == smallestDistance &&
                                    node.DistanceTraveled > result.DistanceTraveled)
                                {
                                    success = true;
                                    result = node;
                                    smallestDistance = currentDistance;
                                }
                            }
                        }
                        break;
                }
            }
            return success;
        }

        /// <summary>
        /// Generates an optimistic estimate of the total path length to the goal 
        /// from the given position.
        /// </summary>
        /// <param name="location">Location to examine</param>
        /// <returns>Path length estimate</returns>
        private static float Heuristic(SearchNode location)
        {
            return location.DistanceTraveled + location.DistanceToGoal;
        }


        /// <summary>
        /// Returns true if the given map location exists
        /// </summary>
        /// <param name="x">column position(x)</param>
        /// <param name="y">row position(y)</param>
        private bool InMap(int x, int y, Tablero tablero)
        {
            return (y >= 0 && y < tablero.dimTablero.Y &&
                    x >= 0 && x < tablero.dimTablero.X);
        }

        /// <summary>
        /// Returns true if the given map location exists and is not 
        /// blocked by a barrier
        /// </summary>
        /// <param name="column">column position(x)</param>
        /// <param name="row">row position(y)</param>
        private bool IsOpen(int x, int y, Tablero tablero)
        {
            if (searchTipo == eSearchTipo.Terrestre)
                return InMap(x, y, tablero) && (tablero.bTablero[x, y] == Tablero.BTABLERO_LIBRE); //pasa por libre solo
            else //aereo
                return InMap(x, y, tablero) && (tablero.bTablero[x, y] != Tablero.BTABLERO_MURO); //pasa por libre o torre
        }

        //version para diagonales
        //para q se pueda caminar en diagonal deben estar libres los 2 cuadrados adyacentes
        private bool IsOpenD(int x, int y, Tablero tablero, int xCoord, int yCoord)
        {
            if (IsOpen(x, y, tablero))
                if (IsOpen(x, yCoord, tablero))
                    if (IsOpen(xCoord, y, tablero))
                        return true;
            return false;
        }

        /// <summary>
        /// Enumerate all the map locations that can be entered from the given 
        /// map location
        /// </summary>
        public IEnumerable<Point> OpenMapTiles(Point coord, Tablero tablero)
        {

            if (IsOpen(coord.X, coord.Y + 1, tablero))
                yield return new Point(coord.X, coord.Y + 1);
            if (IsOpen(coord.X, coord.Y - 1, tablero))
                yield return new Point(coord.X, coord.Y - 1);
            if (IsOpen(coord.X + 1, coord.Y, tablero))
                yield return new Point(coord.X + 1, coord.Y);
            if (IsOpen(coord.X - 1, coord.Y, tablero))
                yield return new Point(coord.X - 1, coord.Y);

            //diagonales
            if (IsOpenD(coord.X + 1, coord.Y + 1, tablero, coord.X, coord.Y))
                yield return new Point(coord.X + 1, coord.Y + 1);

            if (IsOpenD(coord.X + 1, coord.Y - 1, tablero, coord.X, coord.Y))
                yield return new Point(coord.X + 1, coord.Y - 1);

            if (IsOpenD(coord.X - 1, coord.Y + 1, tablero, coord.X, coord.Y))
                yield return new Point(coord.X - 1, coord.Y + 1);

            if (IsOpenD(coord.X - 1, coord.Y - 1, tablero, coord.X, coord.Y))
                yield return new Point(coord.X - 1, coord.Y - 1);
        }

        /// <summary>
        /// Finds the minimum number of tiles it takes to move from Point A to 
        /// Point B if there are no barriers in the way
        /// </summary>
        /// <param name="pointA">Start position</param>
        /// <param name="pointB">End position</param>
        /// <returns>Distance in tiles</returns>
        public static int Distancia(Point pointA, Point pointB)
        {
            int distanceX = Math.Abs(pointA.X - pointB.X);
            int distanceY = Math.Abs(pointA.Y - pointB.Y);

            return distanceX + distanceY;
        }

        public static float Distancia(Vector2 pointA, Vector2 pointB)
        {
            float distanceX = Math.Abs(pointA.X - pointB.X);
            float distanceY = Math.Abs(pointA.Y - pointB.Y);

            return distanceX + distanceY;
        }

         /// <summary>
        /// Finds the minimum number of tiles it takes to move from the current 
        /// position to the end location on the Map if there are no barriers in 
        /// the way
        /// </summary>
        /// <param name="point">Current position</param>
        /// <returns>Distance to end in tiles</returns>
        public int DistanciaASalidas(Point point)
        {
            //busca menor distancia a las salidas
            int menorDistancia = -1;
            int distancia;
            foreach (Point salida in salidas)
            {
                distancia = Distancia(point, salida);
                if ((menorDistancia == -1) || (distancia < menorDistancia))
                {
                    menorDistancia = distancia;
                }
            }
            return menorDistancia;
        }

        public bool EsSalida(Point point)
        {
            //dice si el nodo coincide con alguna salida
            bool bEsSalida = false;
            int iSalida;
            for (iSalida = 0; (iSalida < salidas.Count) && !bEsSalida; iSalida++)
            {
                if (point.Equals(salidas[iSalida]))
                    bEsSalida = true;
            }
            return bEsSalida;
        }


        private bool AhorraMovimiento(Point p1, Point p2, Tablero tablero)
        {
            if ((Math.Abs(p1.X - p2.X) <= 1) && (Math.Abs(p1.Y - p2.Y) <= 1)) //se puede hacer en un solo movimiento
                if (IsOpenD(p1.X, p1.Y, tablero, p2.X, p2.Y)) //la diagonal es valida
                    return true;
            return false;
        }

        /// <summary>
        /// Generates the path from start to end.
        /// </summary>
        /// <returns>The path from start to end</returns>
        public LinkedList<Point> FinalPath(Tablero tablero)
        {
            LinkedList<Point> path = new LinkedList<Point>();
            if (searchStatus == eSearchStatus.PathFound)
            {
                Point curPrev = caminoSalida;
                path.AddFirst(curPrev);

                //optimizacion para diagonal
                while (paths.ContainsKey(curPrev))
                {
                    //si se puede ir en diagonal en vez de horiz + vert (y sin chocar al hacer el movimiento diagonal)
                    if ((paths.ContainsKey(paths[curPrev])) && AhorraMovimiento(curPrev, paths[paths[curPrev]], tablero)) 
                    {
                        //si puedo alcanzar en un movimiento la posic q hay dentro de dos pasos, entonces me salto uno
                        curPrev = paths[paths[curPrev]];
                    }
                    else
                    {
                        curPrev = paths[curPrev];
                    }
                    path.AddFirst(curPrev);
                }
            }
            return path;
        }

        #endregion
    }
}
