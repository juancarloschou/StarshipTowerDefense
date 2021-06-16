using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Naves
{
    //###TORRE###
    public enum eTipoTorre
    {
        Bloque,
        Tanque,
        Triple,
        Canon,
        Hielo,
        AntiAire
    }


    public class CaracteristicasTorre
    {
        public int nivel;
        public float alcance; //hasta donde alcanza a disparar
        public int daño; //daño de los disparos
        public float ratio; //disparos por segundo
        public int precio; //precio en dinero para construir torre

        public float MostrarAlcance //de cara al usuario se muestra en pixeles dividido entre 10
        {
            get { return (int)Math.Round(alcance / 10); }
        }
        public int MostrarRatio //de cara al usuario se muestra en disparos cada 10 segundos
        {
            get { return (int)Math.Round(ratio * 10); }
        }
    }


    abstract class TorreBase
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public static Point TAMAÑOMAXIMOTORRES = new Point (4, 4); //tamaño maximo en casillas

        protected int id;

        protected Vector2 direccion; //a donde apunta la torre, vector direccion
        protected int apuntaEnemigo; //a quien apunta la torre, id enemigo

        public Point coordenadas; //posicion en las coordenadas del tablero (evitas recorrer tablero para saberlas)

        //caracteristicas
        protected CaracteristicasTorre caracteristicas;
        /*protected int nivel;
        protected float alcance; //hasta donde alcanza a disparar
        protected int daño; //daño de los disparos
        protected float ratio; //disparos por segundo
        protected int precio; //precio en dinero para construir torre*/

        //caracteristicas especiales
        protected float velocidadDisparo; //velocidad en pixels por segundo de los disparos
        protected int radioExplosion; //radio de la explosion de armas tipo cañon
        protected float reduceVelocidad; //porcentaje de velocidad reducida por el hielo
        protected TimeSpan tiempoReduceVelocidad; //tiempo dura reduccion velocidad por el hielo

        protected bool bDisparo; //si la torre dispara
        public bool bVendiendose; //si se está vendiendo la torre
        public bool bEliminar;   //si true se borra

        protected eTipoTorre tipoTorre;

        protected int maximoNivelUpgrade;
        protected static int MAXNIVELUPGRADE = 5;
        

        ////////////////// PUBLICAS //////////////////
        //a donde apunta la torre en radianes
        public float RotateDireccion 
        {
            get { return (float)Math.Atan2(direccion.Y, direccion.X); }
        }

        public eTipoTorre TipoTorre
        {
            get { return tipoTorre; }
        }

        public int Nivel
        {
            get { return caracteristicas.nivel; }
        }

        public int Daño
        {
            get { return caracteristicas.daño; }
        }

        public float Alcance
        {
            get { return caracteristicas.alcance; }
        }
        public float MostrarAlcance //de cara al usuario se muestra en pixeles dividido entre 10
        {
            get { return (int)Math.Round(caracteristicas.alcance / 10); }
        }

        public float Ratio
        {
            get { return caracteristicas.ratio; }
        }
        public int MostrarRatio //de cara al usuario se muestra en disparos cada 10 segundos
        {
            get { return (int)Math.Round(caracteristicas.ratio * 10); }
        }

        public int Precio
        {
            get { return caracteristicas.precio; }
        }


        public static Type GetTipo(eTipoTorre tipoTorre)
        {
            //###TORRE###
            Type type = null;
            if (tipoTorre == eTipoTorre.Bloque)
                type = typeof(TorreBloque);
            else if (tipoTorre == eTipoTorre.Tanque)
                type = typeof(TorreTanque);
            else if (tipoTorre == eTipoTorre.Triple)
                type = typeof(TorreTriple);
            else if (tipoTorre == eTipoTorre.Canon)
                type = typeof(TorreCanon);
            else if (tipoTorre == eTipoTorre.Hielo)
                type = typeof(TorreHielo);
            else if (tipoTorre == eTipoTorre.AntiAire)
                type = typeof(TorreAntiAire);

            return type;
        }

        /*public static Sprite GetSprite(TorreBase torre)
        {
            return GetSprite(torre.tipoTorre);
        }*/

        public static Sprite GetSprite(eTipoTorre tipoTorre)
        {
            return (Sprite)(TorreBase.GetTipo(tipoTorre)).GetField("sprite").GetValue(0);

            /*
            Sprite sprite = null;
            if (tipoTorre == eTipoTorre.Bloque)
                sprite = TorreBloque.sprite;
            else if (tipoTorre == eTipoTorre.Tanque)
                sprite = TorreTanque.sprite;
            else if (tipoTorre == eTipoTorre.Triple)
                sprite = TorreTriple.sprite;
            else if (tipoTorre == eTipoTorre.Canon)
                sprite = TorreCanon.sprite;
            else if (tipoTorre == eTipoTorre.Hielo)
                sprite = TorreHielo.sprite;
            return sprite;
            */
        }

        public Sprite GetSprite()
        {
            return GetSprite(tipoTorre);
        }

        public static CaracteristicasTorre GetCaracteristicasOpcionTorre(eTipoTorre tipoTorre)
        {
            CaracteristicasTorre caract = new CaracteristicasTorre();

            object[] ConstArgs = new object[3];
            ConstArgs[0] = new Point(0, 0);
            ConstArgs[1] = -1;
            ConstArgs[2] = 0;

            object[] MethodArgs = new object[1];
            MethodArgs[0] = 0;

            TorreBase torre = (TorreBase)Activator.CreateInstance(TorreBase.GetTipo(tipoTorre), ConstArgs);
            caract = (CaracteristicasTorre)TorreBase.GetTipo(tipoTorre).GetMethod("GetCaracteristicas").Invoke(torre, MethodArgs);

            return caract;

            /*

            if (tipoTorre == eTipoTorre.Bloque)
            {
                TorreBloque torre = new TorreBloque(new Point(0, 0), -1);
                caract = torre.GetCaracteristicas(0);
            }
            else if (tipoTorre == eTipoTorre.Tanque)
            {
                TorreTanque torre = new TorreTanque(new Point(0, 0), -1);
                caract = torre.GetCaracteristicas(0);
            }
            else if (tipoTorre == eTipoTorre.Triple)
            {
                TorreTriple torre = new TorreTriple(new Point(0, 0), -1);
                caract = torre.GetCaracteristicas(0);
            }
            else if (tipoTorre == eTipoTorre.Canon)
            {
                TorreCanon torre = new TorreCanon(new Point(0, 0), -1);
                caract = torre.GetCaracteristicas(0);
            }
            else if (tipoTorre == eTipoTorre.Hielo)
            {
                TorreHielo torre = new TorreHielo(new Point(0, 0), -1);
                caract = torre.GetCaracteristicas(0);
            }

            return caract;
            */
        }


        public static string TipoTorreToString(eTipoTorre tipoTorre)
        {
            string str = string.Empty;

            //###TORRE###
            if (tipoTorre == eTipoTorre.Bloque)
                str = Recursos.Torre_Bloque;
            else if (tipoTorre == eTipoTorre.Tanque)
                str = Recursos.Torre_Tanque;
            else if (tipoTorre == eTipoTorre.Triple)
                str = Recursos.Torre_Triple;
            else if (tipoTorre == eTipoTorre.Canon)
                str = Recursos.Torre_Canon;
            else if (tipoTorre == eTipoTorre.Hielo)
                str = Recursos.Torre_Hielo;
            else if (tipoTorre == eTipoTorre.AntiAire)
                str = Recursos.Torre_AntiAire;

            return str;
        }


        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public TorreBase(Point coord, eTipoTorre tipoTorre, int id, int nivel = 0)
        {
            this.id = id;
            //this.nivel = nivel;
            this.coordenadas = coord;

            this.tipoTorre = tipoTorre;
            this.bEliminar = false;
            this.bVendiendose = false;
            this.apuntaEnemigo = -1;

            //CaracteristicasTorre caracteristicas = GetCaracteristicas(nivel);
            //SetCaracteristicas(caracteristicas);

            SetCaracteristicas(nivel);
            SetCaracteristicasEspeciales(nivel);
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        //se mejora nivel de la torre, hay comprobacion de nivel maximo, de si hay dinero suficiente. al mejorar se paga el precio
        public bool Upgrade(Nave nave)
        {
            bool bUpgrade = false; //se pudo o no pudo hacer le upgrade

            if (Nivel < maximoNivelUpgrade) //futuro nivel debajo del maximo
            {
                CaracteristicasTorre caractActual = GetCaracteristicas(Nivel);
                int nivelUpgrade = Nivel + 1;
                CaracteristicasTorre caractUpgrade = GetCaracteristicas(nivelUpgrade);
                int precioUpgrade = caractUpgrade.precio - caractActual.precio;

                if (nave.Dinero >= precioUpgrade) //hay dinero suficiente
                {
                    //hacer el upgrade
                    nave.PagarUpgradeTorre(precioUpgrade); //se paga el precio

                    SetCaracteristicas(caractUpgrade);
                    SetCaracteristicasEspeciales(nivelUpgrade);
                    //Nivel = nivelUpgrade;
                    bUpgrade = true;
                }
            }

            return bUpgrade;
        }

        //cada tipo de torre tiene sus caracteristicas
        public abstract CaracteristicasTorre GetCaracteristicas(int nivel);

        protected void SetCaracteristicas(CaracteristicasTorre caract)
        {
            caracteristicas = caract;

            //this.alcance = caracteristicas.alcance;
            //this.daño = caracteristicas.daño;
            //this.precio = caracteristicas.precio;
            //this.ratio = caracteristicas.ratio;
        }

        protected void SetCaracteristicas(int nivel)
        {
            caracteristicas = GetCaracteristicas(nivel);
        }

        protected abstract void SetCaracteristicasEspeciales(int nivel); 


        public Vector2 PosicionCoordenadas(Tablero tablero)
        {
            Vector2 posicion = tablero.CoordToPosicion(coordenadas);

            return posicion;
        }

        //casillas q ocupa la torre (tamTorre)
        public static Point CasillasTorre(eTipoTorre tipoTorre, Tablero tablero)
        {
            Point tamTorre = new Point((int)(TorreBase.GetSprite(tipoTorre).dimImagen.X / tablero.tamCasillas.X),
                                       (int)(TorreBase.GetSprite(tipoTorre).dimImagen.Y / tablero.tamCasillas.Y));
            return tamTorre;
        }

        //centro de la torre
        public static Vector2 CentroTorre(eTipoTorre tipoTorre, Tablero tablero)
        {
            Vector2 centro = TorreBase.GetSprite(tipoTorre).center;
            return centro;
        }


        ////////////////// LOAD CONTENT //////////////////
        //public abstract void LoadContent(ContentManager Content);


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, Tablero tablero, Torres torres, Enemigos enemigos, Nave nave, Opciones opciones)
        {
            if (!bEliminar)
            {
                if ((bDisparo)&& (!bVendiendose))
                {
                    //mueve direccion de la torre y gestiona los disparos
                    UpdateDisparos(gameTime, tablero, torres, enemigos, nave, opciones);
                }
            }
        }


        protected abstract void UpdateDisparos(GameTime gameTime, Tablero tablero, Torres torres, Enemigos enemigos, Nave nave, Opciones opciones);


        protected virtual void UpdateApuntar(Tablero tablero, Enemigos enemigos, bool bSoloAereo = false)
        {
            //busca enemigo mas avanzado dentro del alcance
            Vector2 centroTorre = TorreBase.CentroTorre(tipoTorre, tablero);
            centroTorre = tablero.CoordToPosicion(coordenadas) + centroTorre;

            float distancia;
            float distanciaNodo;
            float menorDistancia = float.MaxValue; //menor numero de nodos camino
            float menorDistanciaNodo = float.MaxValue; //menor distancia a proximo nodo camino
            Vector2 posicionEnemigoCercano = Vector2.Zero;
            int idEnemigo = -1;
            EnemigoBase enemigo;
            for (int iEnemigo = 0; iEnemigo < enemigos.enemigos.Count; iEnemigo++)
            {
                enemigo = enemigos.enemigos[iEnemigo];
                if (!enemigo.bEliminar)
                {
                    distancia = Matematicas.Distancia(centroTorre, enemigo.posicion);
                    if (distancia <= Alcance) //dentro del alcance
                    {
                        //distancia a la salida, el enemigo mas avanzado, le quedan menos nodos en su camino
                        if ((enemigo.TipoMovimiento == eTipoMovimiento.Terrestre) && !bSoloAereo) //bSoloAereo solo apunta a enemigos aereos
                        {
                            distancia = ((EnemigoTerrestre)enemigo).TamañoCamino;
                            distanciaNodo = Matematicas.Distancia(enemigo.posicion, ((EnemigoTerrestre)enemigo).PosicionNodoCamino);
                        }
                        else if (enemigo.TipoMovimiento == eTipoMovimiento.Aereo)
                        {
                            distancia = ((EnemigoAereo)enemigo).TamañoCamino;
                            distanciaNodo = Matematicas.Distancia(enemigo.posicion, ((EnemigoAereo)enemigo).PosicionNodoCamino);
                        }
                        else
                        {
                            distancia = float.MaxValue;
                            distanciaNodo = float.MaxValue;
                        }

                        if (distancia != float.MaxValue) //no camino nulo ni enemigo aereo
                        {
                            if (distancia == menorDistancia)
                            {
                                //comparar distancia enemigo a su proximo nodo
                                if (distanciaNodo < menorDistanciaNodo)
                                {
                                    posicionEnemigoCercano = enemigo.posicion;
                                    menorDistancia = distancia;
                                    menorDistanciaNodo = distanciaNodo;
                                    idEnemigo = iEnemigo;
                                }
                            }
                            else if (distancia < menorDistancia)
                            {
                                posicionEnemigoCercano = enemigo.posicion;
                                menorDistancia = distancia;
                                menorDistanciaNodo = distanciaNodo;
                                idEnemigo = iEnemigo;
                            }
                        }
                    }
                }
            }

            apuntaEnemigo = idEnemigo; //si no hay enemigos cerca, id = -1

            if (idEnemigo != -1)
            {
                //direccion es vector de centroTorre a posicionEnemigoCercano
                Vector2 Direccion = Matematicas.Vector(centroTorre, posicionEnemigoCercano);

                //normalizamos vector (tamaño 1)
                Direccion.Normalize();

                direccion = Direccion;
            }
        }


        ////////////////// DRAW //////////////////
        public abstract void Draw(SpriteBatch spb, Sprite sprite, Tablero tablero, SpriteFont font);

        /*
        public void Draw(SpriteBatch spb, Sprite sprite, Tablero tablero)
        {
            if (!bEliminar)
            {
                Rectangle rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, sprite.dimImagen.X, sprite.dimImagen.Y);

                spb.Draw(sprite.imagen, PosicionCoordenadas(tablero), rectangulo, Color.White, RotateDireccion, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
        */
    }
}
