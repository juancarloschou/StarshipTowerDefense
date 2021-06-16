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
    
    //para recalcular caminos de grupos de enemigos
    struct RecalcularEnemigo: IComparable
    {
        public int id;
        public float distancia; //distancia al objetivo

        // implement IComparable interface
        public int CompareTo(object obj)
        {
            if (obj is RecalcularEnemigo)
            {
                return this.distancia.CompareTo(((RecalcularEnemigo)obj).distancia); 
            }
            throw new ArgumentException("Object is not a RecalcularEnemigo");
        }

    }


    class Enemigos
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        //lista de enemigos
        public List<EnemigoBase> enemigos;

        //public int idEnemigoVida; //id del enemigo selecionado

        //contadores
        public int MaxEnemigos = 500; //numero maximo de enemigos en la lista

        //limites de la ventana
        //private Viewport ventanaGame; //para que???


        //dar la orden de reclacular los caminos de los enemigos
        private bool bRecalcularCaminos;
        private RecalcularEnemigo[] recalcularEnemigo; //trocear el problema de recalcular caminos enemigos
        private TimeSpan recalcularEnemigoRate = TimeSpan.FromMilliseconds(750); //cada cuanto tpo se procesa otro grupo
        private TimeSpan previoRecalcularEnemigo; //cuenta el tiempo para siguiente recalculo de grupo
        private Vector2 posicionRecalcularEnemigo; //empieza primero por los mas cercanos a la coord de la torre nueva/eliminada
        private const int TAMAÑOGRUPORECALCULAR = 15; //tamaño de los grupos a recalcular

        private Random random = new Random(System.DateTime.Now.Minute * System.DateTime.Now.Second);


        ////////////////// PUBLICAS //////////////////
        public bool RecalcularCaminos
        {
            get { return bRecalcularCaminos; }
        }

        public void SetRecalcularCaminos(Vector2 coord, GameTime gameTime)
        {
            if (bRecalcularCaminos)
            {
                //inicializa enemigos, q vuelvan a recalcularse con la nueva torre
                EnemigoBase enemigo;
                for (int iEnemigo = enemigos.Count - 1; iEnemigo >= 0; iEnemigo--)
                {
                    enemigo = enemigos[iEnemigo];
                    if(!enemigo.bEliminar)
                        if (enemigo.TipoMovimiento == eTipoMovimiento.Terrestre)
                        {
                            ((EnemigoTerrestre)enemigo).bMarcado = false;
                        }
                }
            }

            bRecalcularCaminos = true;
            posicionRecalcularEnemigo = coord; //posicion de la torre, en el centro de las 4 casillas
            previoRecalcularEnemigo = gameTime.TotalGameTime - recalcularEnemigoRate; //q empiece ya
        }


        ////////////////// CONSTRUCTOR //////////////////
        public Enemigos() //Viewport VentanaGame)
        {
            //this.ventanaGame = VentanaGame;

            //inicializar lista de enemigos
            enemigos = new List<EnemigoBase>();

            //idEnemigoVida = -1; //sin seleccionar

            bRecalcularCaminos = false;
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public void AddEnemigo(eTipoEnemigo tipoEnemigo, Puerta entrada, Tablero tablero, int nivel = 0)
        {
            Sprite sprite = EnemigoBase.GetSprite(tipoEnemigo);

            Vector2 posicion;
            // puertas horizontales
            if (entrada.coordTopLeft.Y == entrada.coordBottomRight.Y)
            {
                int ancho = (int)(tablero.CoordToPosicion(new Point(entrada.coordBottomRight.X + 1,entrada.coordBottomRight.Y)).X - sprite.dimImagen.X / 2.0
                            - (tablero.CoordToPosicion(entrada.coordTopLeft).X + sprite.dimImagen.X / 2.0));
                int rand = random.Next(ancho + 1);
                posicion = new Vector2((int)Math.Round(rand + tablero.CoordToPosicion(entrada.coordTopLeft).X + sprite.dimImagen.X / 2.0),
                                       -sprite.center.Y);
            }
            else
            {
                //puertas verticales
                int alto = (int)(tablero.CoordToPosicion(new Point(entrada.coordBottomRight.X, entrada.coordBottomRight.Y + 1)).X - sprite.dimImagen.Y / 2.0
                            - (tablero.CoordToPosicion(entrada.coordTopLeft).Y + sprite.dimImagen.Y / 2.0));
                int rand = random.Next(alto + 1);
                posicion = new Vector2(-sprite.center.X,
                                       (int)Math.Round(rand + tablero.CoordToPosicion(entrada.coordTopLeft).Y + sprite.dimImagen.Y / 2.0));
            }


            //###ENEMIGO###
            object[] ConstArgs = new object[3]; //argumentos del constructor
            ConstArgs[0] = posicion;
            ConstArgs[1] = tablero;
            ConstArgs[2] = nivel;

            EnemigoBase enemigo = (EnemigoBase)Activator.CreateInstance(EnemigoBase.GetTipo(tipoEnemigo), ConstArgs);
            enemigos.Add(enemigo);

            /*
            if (tipoEnemigo == eTipoEnemigo.Tanque)
            {
                EnemigoTanque enemigo = new EnemigoTanque(posicion, tablero, nivel);
                enemigos.Add(enemigo);
            }
            else if (tipoEnemigo == eTipoEnemigo.Garras)
            {
                EnemigoGarras enemigo = new EnemigoGarras(posicion, tablero, nivel);
                enemigos.Add(enemigo);
            }
            else if (tipoEnemigo == eTipoEnemigo.Robot)
            {
                EnemigoRobot enemigo = new EnemigoRobot(posicion, tablero, nivel);
                enemigos.Add(enemigo);
            }
            else if (tipoEnemigo == eTipoEnemigo.Helicoptero)
            {
                EnemigoHelicoptero enemigo = new EnemigoHelicoptero(posicion, tablero, nivel);
                enemigos.Add(enemigo);
            }
            else if (tipoEnemigo == eTipoEnemigo.CazaMercurio)
            {
                EnemigoCazaMercurio enemigo = new EnemigoCazaMercurio(posicion, tablero, nivel);
                enemigos.Add(enemigo);
            }
            */

        }


        private void ProcesaRecalcularCaminos(Tablero tablero)
        {
            //recalcular por trozos

            //crear array de uso auxiliar
            RecalcularEnemigo[] recalcular;
            int count = enemigos.Count;
            recalcular = new RecalcularEnemigo[count];

            //inicializa array
            for (int i = 0; i < count; i++)
            {
                recalcular[i].id = -1;
            }

            //llena array con los terrestres aun no marcados
            EnemigoBase enemigo;
            int iCount = 0;
            for (int i = 0; i < count; i++)
            {
                enemigo = enemigos[i];
                if (enemigo.TipoMovimiento == eTipoMovimiento.Terrestre)
                    if (!enemigo.bEliminar)
                        if (!((EnemigoTerrestre)enemigo).bMarcado)
                        {
                            recalcular[i].id = i;
                            //distancia a la torre nueva/borrada
                            recalcular[i].distancia = Vector2.Distance(enemigo.posicion, posicionRecalcularEnemigo);

                            iCount++; //contar elementos
                        }
            }

            //if iCount == 0 -> terminar (bRecalcularCaminos = false) y desmarcar todos los enemigos (foreach bMarcado = false)
            if (iCount == 0)
            {
                for (int iEnemigo = enemigos.Count - 1; iEnemigo >= 0; iEnemigo--)
                {
                    enemigo = enemigos[iEnemigo];
                    if (enemigo.TipoMovimiento == eTipoMovimiento.Terrestre)
                    {
                        ((EnemigoTerrestre)enemigo).bMarcado = false;
                    }
                }
                bRecalcularCaminos = false;
            }

            //crear array definitivo (ahora sabemos cuantos hay), sin los enemigos no terrestres o ya marcados
            recalcularEnemigo = new RecalcularEnemigo[iCount];
            int iRecalcular = 0;
            for (int i = 0; i < count; i++)
            {
                if (recalcular[i].id != -1)
                {
                    recalcularEnemigo[iRecalcular].id = recalcular[i].id;
                    recalcularEnemigo[iRecalcular].distancia = recalcular[i].distancia; //distancia a la torre
                    iRecalcular++;
                }
            }

            //ordenar array definitivo por distancia a la torre, de menor a mayor
            Array.Sort(recalcularEnemigo);

            //ir de arriba (mas cerca a torre) a abajo y coger los TamGrupoRecalcular enemigos
            int iMarcados = 0;
            for (int i = 0; (i < iCount) && (iMarcados < TAMAÑOGRUPORECALCULAR); i++)
            {
                enemigo = enemigos[recalcularEnemigo[i].id];
                if (enemigo.TipoMovimiento == eTipoMovimiento.Terrestre)
                {
                    ((EnemigoTerrestre)enemigo).bMarcado = true; //marcarlos para recalcular
                    iMarcados++;
                }
                else
                {
                    Console.WriteLine("error imposible RecalcularCaminos");
                }
            }
        }


        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager content)
        {
            //###ENEMIGO###
            //precarga el contenido de los enemigos
            EnemigoTanque.LoadContent(content);
            EnemigoGarras.LoadContent(content);
            EnemigoRobot.LoadContent(content);
            EnemigoHelicoptero.LoadContent(content);
            EnemigoCazaMercurio.LoadContent(content);
            //cargar todos los demas...

            EnemigoBase.LoadContent(content); //enemigo helado
            EnemigoVida.LoadContent(content); //barra de vida
        }


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, Tablero tablero, Nave nave, Torres torres, Animaciones animaciones, Opciones opciones)
        {
            //eliminar enemigos marcados para eliminar
            EnemigoBase enemigo;
            for (int iEnemigo = enemigos.Count - 1; iEnemigo >= 0; iEnemigo--)
            {
                enemigo = enemigos[iEnemigo];
                if (enemigo.bEliminar)
                {
                    //los disparos hacia este enemigo se eliminan
                    torres.disparos.EliminarEnemigo(iEnemigo);
                    
                    //elimina enemigo
                    //enemigos.Remove(enemigo);
                }
            }

            //al poner una torre se marca bRecalcularCaminos
            if (RecalcularCaminos)
            {
                //se van marcando los enemigos en grupos, poco a poco
                //luego en enemigo.Update se recalculan los caminos de los marcados
                if (gameTime.TotalGameTime - previoRecalcularEnemigo >= recalcularEnemigoRate)
                {
                    previoRecalcularEnemigo = gameTime.TotalGameTime;
                    ProcesaRecalcularCaminos(tablero);
                }
            }

            for (int iEnemigo = enemigos.Count - 1; iEnemigo >= 0; iEnemigo--)
            {
                //lógica de los enemigos
                enemigos[iEnemigo].Update(gameTime, tablero, nave, animaciones, opciones);

                //ver la vida de los enemigos
                enemigos[iEnemigo].enemigoVida.Update(enemigos[iEnemigo]);
            }

        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb)
        {
            foreach (EnemigoBase enemigo in enemigos)
            {
                if (!enemigo.bEliminar)
                {
                    //dibuja enemigo
                    //enemigo.Draw(spb, EnemigoBase.GetSprite(enemigo));
                    enemigo.Draw(spb, EnemigoBase.GetSprite(enemigo.TipoEnemigo), enemigo.GetAnimacion());
                }
            }

            foreach (EnemigoBase enemigo in enemigos)
            {
                if (!enemigo.bEliminar)
                {
                    //dibuja la vida de los enemigos
                    enemigo.enemigoVida.Draw(spb);
                }
            }

        }

    }
}
