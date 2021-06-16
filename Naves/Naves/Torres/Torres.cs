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
    /// dibuja las torres y las procesa (sus disparos)
    /// </summary>
    class Torres
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        //lista de torres
        public List<TorreBase> torres;


        //lista de disparos
        public TorresDisparos disparos;

        private static SpriteFont font; //fuente para poner nivel de la torre


        //vender torre
        private bool bVenderTorre; //si hay alguna torre vendiendose
        private int venderIdTorre; //cual se esta vendiendo
        private TimeSpan venderInicio; //tiempo inicial
        private TimeSpan venderTiempo; //tiempo transcurrido
        private TimeSpan venderTotal; //tiempo total
        private static int margen = 3;
        private static int ALTURA = 10;

        private static Sprite sprite = new Sprite();



        ////////////////// PUBLICAS //////////////////


        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public Torres() //Viewport Ventana)
        {
            //this.ventana = Ventana; 

            torres = new List<TorreBase>();

            disparos = new TorresDisparos();

            bVenderTorre = false;
            venderTotal = TimeSpan.FromSeconds(2);
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ///NO USAR DIRECTAMENTE, SOLO DESDE TABLERO ADDTORRE (ahi se crean las torres)
        /// devuelve el id de la torre en la lista
        public int _TableroAddTorre(eTipoTorre tipoTorre, Point coord, int nivel = 0)
        {
            int id = torres.Count;

            object[] ConstArgs = new object[3]; //argumentos del constructor
            ConstArgs[0] = coord;
            ConstArgs[1] = id;
            ConstArgs[2] = nivel;

            TorreBase torre = (TorreBase)Activator.CreateInstance(TorreBase.GetTipo(tipoTorre), ConstArgs);
            torres.Add(torre);


            /*
            if (tipoTorre == eTipoTorre.Bloque)
            {
                TorreBloque torre = new TorreBloque(coord, id, nivel);
                torres.Add(torre);
            }
            else if (tipoTorre == eTipoTorre.Tanque)
            {
                TorreTanque torre = new TorreTanque(coord, id, nivel);
                torres.Add(torre);
            }
            else if (tipoTorre == eTipoTorre.Triple)
            {
                TorreTriple torre = new TorreTriple(coord, id, nivel);
                torres.Add(torre);
            }
            else if (tipoTorre == eTipoTorre.Canon)
            {
                TorreCanon torre = new TorreCanon(coord, id, nivel);
                torres.Add(torre);
            }
            else if (tipoTorre == eTipoTorre.Hielo)
            {
                TorreHielo torre = new TorreHielo(coord, id, nivel);
                torres.Add(torre);
            }
            */

            return id;
        }

        ///NO USAR DIRECTAMENTE, SOLO DESDE TABLERO DELETETORRE (ahi se borran las torres)
        /// devuelve coord de la torre en el tablero
        public Point _TableroDeleteTorre(int id)
        {
            torres[id].bEliminar = true;

            return torres[id].coordenadas;
        }


        //devuelve true si se puede vender, false sino
        public bool VenderTorre(GameTime gameTime, int idTorre)
        {
            if(bVenderTorre)
                return false;
            else
            {
                torres[idTorre].bVendiendose = true; //esa torre ya no dispara
                bVenderTorre = true;
                venderIdTorre = idTorre;
                venderInicio = gameTime.TotalGameTime;
                return true;
            }
        }


        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager content)
        {
            //###TORRE###
            //precarga el contenido de las torres
            TorreBloque.LoadContent(content);
            TorreTanque.LoadContent(content);
            TorreTriple.LoadContent(content);
            TorreCanon.LoadContent(content);
            TorreHielo.LoadContent(content);
            TorreAntiAire.LoadContent(content);
            //cargar todos los demas...

            disparos.LoadContent(content); //sprites de disparos torres

            //fuente del nivel de las torres
            font = content.Load<SpriteFont>("Fuentes\\Torres");

            //barra progreso vender torre
            sprite.imagen = content.Load<Texture2D>("Sprites\\blank");
            sprite.dimImagen = new Point(4, 4);
            sprite.posImagen = new Point(0, 0);
        }


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, Tablero tablero, Nave nave, Enemigos enemigos, Animaciones animaciones, Opciones opciones)
        {
            DateTime timeIni = DateTime.Now;

            TorreBase torre;
            for (int iTorre = torres.Count - 1; iTorre >= 0; iTorre--)
            {
                //eliminar torres marcadas para eliminar
                torre = torres[iTorre];
                /*
                if (torre.bEliminar)
                {
                    torres.Remove(torre);
                }
                */

                //update de torres (direccion y disparo)
                torre.Update(gameTime, tablero, this, enemigos, nave, opciones);
            }

            disparos.Update(gameTime, nave, enemigos, animaciones, opciones);


            //vender torre
            if(bVenderTorre)
            {
                venderTiempo = gameTime.TotalGameTime - venderInicio;
                if (venderTiempo > venderTotal)
                {
                    //destruir torre
                    bVenderTorre = false;
                    Sonidos.PlayFX(eSonido.Vender, opciones);
                    tablero.DeleteTorre(gameTime, venderIdTorre, nave, this, enemigos);
                }
            }
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb, Tablero tablero)
        {
            TorreBase torre;
            for (int iTorre = torres.Count - 1; iTorre >= 0; iTorre--)
            {
                //dibuja torre
                torre = torres[iTorre];
                if (!torre.bEliminar)
                {
                    if (!bVenderTorre || (iTorre != venderIdTorre)) //si no la estamos vediendo
                        torre.Draw(spb, TorreBase.GetSprite(torre.TipoTorre), tablero, font);
                }
            }

            //torresDisparos.Draw(spb); //se debe hacer despues de enemigos, va por separado en Game


            //vender torre
            if (bVenderTorre)
            {
                torre = torres[venderIdTorre];
                Vector2 posicion = tablero.CoordToPosicion(torre.coordenadas);

                //base
                Rectangle rectangulo = new Rectangle(TorreBloque.sprite.posImagen.X, TorreBloque.sprite.posImagen.Y,
                                                     TorreBloque.sprite.dimImagen.X, TorreBloque.sprite.dimImagen.Y);
                spb.Draw(TorreBloque.sprite.imagen, posicion, rectangulo, Color.White);

                //barra de progreso
                posicion += new Vector2(margen, TorreBloque.sprite.center.Y - ALTURA / 2);

                float porcentaje = (float)venderTiempo.TotalMilliseconds / (float)venderTotal.TotalMilliseconds;
                int ancho = TorreBloque.sprite.dimImagen.X - margen * 2;
                int x = (int)Math.Round(porcentaje * ancho);

                rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, x, (int)ALTURA);
                spb.Draw(sprite.imagen, posicion, rectangulo, Color.Black);

                if (x < ancho)
                {
                    rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, (int)ancho - x, (int)ALTURA);
                    spb.Draw(sprite.imagen, posicion + new Vector2(x, 0), rectangulo, Color.Gray);
                }
            }
        }

    }
}
