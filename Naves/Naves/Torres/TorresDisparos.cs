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
    enum eTipoTorreDisparo
    {
        Tanque,
        Triple,
        Canon,
        Misil
    }


    class TorresDisparos
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public List<TorreDisparo> disparos;

        //imagenes del disparo, precargadas para reutilizarlas
        public static Sprite[] sprite = new Sprite[Enum.GetValues(typeof(eTipoDisparo)).Length]; 


        ////////////////// CONSTRUCTOR //////////////////
        public TorresDisparos() //Viewport viewport)
        {
            //ventana = viewport;

            //inicializar lista de disparos
            disparos = new List<TorreDisparo>();

            //establece q el disparo sale cada x segundos
            //fireRate = TimeSpan.FromSeconds(0.33f);
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public void EliminarEnemigo(int idEnemigo)
        {
            foreach (TorreDisparo torreDisparo in disparos)
            {
                torreDisparo.EliminarEnemigo(idEnemigo);
            }
        }

        public void AddDisparo(GameTime gameTime, Vector2 posicionTorre, Point dimensionesTorre, int idTorre, int daño, 
                               float velocidad, int radioExplosion, Vector2 posicionEnemigo, int idEnemigo, eTipoTorreDisparo tipoDisparo)
        {
            TorreDisparo torreDisparo = new TorreDisparo(gameTime, posicionTorre, dimensionesTorre, idTorre, daño, 
                                                         velocidad, radioExplosion, posicionEnemigo, idEnemigo, tipoDisparo);
            disparos.Add(torreDisparo);
        }


        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager Content)
        {
            //precarga el contenido de los disparos
            sprite[(int)eTipoTorreDisparo.Tanque] = new Sprite();
            sprite[(int)eTipoTorreDisparo.Tanque].imagen = Content.Load<Texture2D>("Sprites\\torre_disparos_01");
            sprite[(int)eTipoTorreDisparo.Tanque].dimImagen = new Point(6, 6);
            sprite[(int)eTipoTorreDisparo.Tanque].posImagen = new Point(0, 0);

            sprite[(int)eTipoTorreDisparo.Triple] = new Sprite();
            sprite[(int)eTipoTorreDisparo.Triple].imagen = sprite[(int)eTipoTorreDisparo.Tanque].imagen;
            sprite[(int)eTipoTorreDisparo.Triple].dimImagen = new Point(5, 5);
            sprite[(int)eTipoTorreDisparo.Triple].posImagen = new Point(6, 0);

            //animacion disparo cañon
            sprite[(int)eTipoTorreDisparo.Canon] = new Sprite();
            sprite[(int)eTipoTorreDisparo.Canon].imagen = Content.Load<Texture2D>("Sprites\\torre_disparos_canon_01");
            sprite[(int)eTipoTorreDisparo.Canon].dimImagen = new Point(14, 4);
            sprite[(int)eTipoTorreDisparo.Canon].posImagen = new Point(0, 0);

            //animacion disparo misil
            sprite[(int)eTipoTorreDisparo.Misil] = new Sprite();
            sprite[(int)eTipoTorreDisparo.Misil].imagen = Content.Load<Texture2D>("Sprites\\torre_disparos_misil_01");
            sprite[(int)eTipoTorreDisparo.Misil].dimImagen = new Point(27, 9);
            sprite[(int)eTipoTorreDisparo.Misil].posImagen = new Point(0, 0);

        }


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, Nave nave, Enemigos enemigos, Animaciones animaciones, Opciones opciones)
        {
            TorreDisparo torreDisparo;
            for (int iTorreDisparo = disparos.Count - 1; iTorreDisparo >= 0; iTorreDisparo--)
            {
                torreDisparo = disparos[iTorreDisparo];

                if (torreDisparo.bEliminar)
                {
                    //elimina disparo
                    disparos.RemoveAt(iTorreDisparo);
                }
                else
                {
                    torreDisparo.Update(gameTime, nave, enemigos, animaciones, sprite[(int)torreDisparo.tipoDisparo], opciones);
                }
            }
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb, Torres torres, Tablero tablero)
        {
            //disparo de hielo
            TorreHielo torreHielo;
            foreach (TorreBase torre in torres.torres)
            {
                if (!torre.bEliminar)
                {
                    if (torre.TipoTorre == eTipoTorre.Hielo)
                    {
                        torreHielo = (TorreHielo)torre;
                        if (torreHielo.bDisparando)
                        {
                            torreHielo.DrawDisparo(spb, tablero);
                        }
                    }
                }
            }


            //disparos de la coleccion TorresDisparos
            foreach (TorreDisparo torreDisparo in disparos)
            {
                if (!torreDisparo.bEliminar)
                {
                    torreDisparo.Draw(spb, sprite[(int)torreDisparo.tipoDisparo]);
                }
            }

        }

    }
}
