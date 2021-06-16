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
    public enum eTipoDisparo
    {
        Simple,
        Multiple,
        Laser,
        Misiles,
        Rayo
    }


    public class CaracteristicasTipoDisparo
    {
        public bool bActivo; //arma disponible o no disponible (si se ha comprado)
        public int nivel;
        public int precio;

        public float velocidad; //velocidad del disparo
        public int daño; //daño del disparo contra enemigos terrestres
        public int dañoAire; //daño del disparo contra enemigos aereos
        public int radioMisiles; //alcance de los misiles
        public float ratio; //disparos por segundo

        public int numDisparos;
        public Vector2[] direcciones; //vectores normalizados
        public Vector2[] posiciones = null; //posicion de salida

        public int MostrarRatio //de cara al usuario se muestra en disparos cada 10 segundos
        {
            get { return (int)Math.Round(ratio * 10); }
        }
    }


    //disparos de la nave
    class Disparos
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public List<Disparo> disparos;

        //imagenes del disparo, precargadas para reutilizarlas
        public static Sprite[] sprite = new Sprite[Enum.GetValues(typeof(eTipoDisparo)).Length]; 
        
        //contadores
        TimeSpan previoFireTime;
        private static int MAXDISPAROS = 100; //numero maximo de disparos en la lista (List Counter Max Disparo)
        

        //caracteristicas tipos de disparos
        private CaracteristicasTipoDisparo[] caracteristicas = new CaracteristicasTipoDisparo[Enum.GetValues(typeof(eTipoDisparo)).Length];
        private static int MAXNIVELDISPAROS = 5;


        //limites de la ventana
        private Viewport ventana;


        ////////////////// PUBLICA //////////////////
        public CaracteristicasTipoDisparo[] Caracteristicas
        {
            get { return caracteristicas; }
        }

        public static string DisparoToString(eTipoDisparo tipoDisparo)
        {
            string str = string.Empty;

            switch (tipoDisparo)
            {
                case eTipoDisparo.Simple:
                    str = Recursos.Simple;
                    break;
                case eTipoDisparo.Multiple:
                    str = Recursos.Multiple;
                    break;
                case eTipoDisparo.Laser:
                    str = Recursos.Laser;
                    break;
                case eTipoDisparo.Misiles:
                    str = Recursos.Misiles;
                    break;
                case eTipoDisparo.Rayo:
                    str = Recursos.Rayo;
                    break;
            }

            return str;
        }

        /*public int NivelArma(eTipoDisparo tipoDisparo)
        {
            if (!caracteristicas[(int)tipoDisparo].bActivo)
                return -1;
            else
                return caracteristicas[(int)tipoDisparo].nivel;
        }*/

        public string NivelArmaToString(eTipoDisparo tipoDisparo)
        {
            if (!caracteristicas[(int)tipoDisparo].bActivo)
                return string.Empty;
            else
                return caracteristicas[(int)tipoDisparo].nivel.ToString();
        }

        public string PrecioArmaToString(eTipoDisparo tipoDisparo)
        {
            if (!caracteristicas[(int)tipoDisparo].bActivo)
            {
                //comprar arma, precio nivel 0
                CaracteristicasTipoDisparo caract = GetCaracteristicas(caracteristicas, tipoDisparo, 0);
                return caract.precio.ToString();
            }
            else
            {
                //mejora de arma, precio nivel siguiente
                CaracteristicasTipoDisparo caract = GetCaracteristicas(caracteristicas, tipoDisparo, caracteristicas[(int)tipoDisparo].nivel + 1);
                return caract.precio.ToString();
            }
        }


        ////////////////// EVENTOS //////////////////
        public event EventHandler<EventArgsCambiarDisparosNave> CambiarDisparosNave;


        ////////////////// CONSTRUCTOR //////////////////
        public Disparos(Viewport viewport, Tooltip tooltip, Partida partida)
        {
            ventana = viewport;

            //inicializar lista de disparos
            disparos = new List<Disparo>();


            //inicializa caracteristicas de cada tipo de disparo
            /*SetCaracteristicas(eTipoDisparo.Multiple, -1, false); //arma no disponible
            SetCaracteristicas(eTipoDisparo.Laser, -1, false);
            SetCaracteristicas(eTipoDisparo.Misiles, -1, false);
            SetCaracteristicas(eTipoDisparo.Rayo, -1, false);*/

            //cargar disparos disponibles desde la partida
            partida.PartidaDatos.caracteristicas.CopyTo(caracteristicas, 0);


            //eventos
            this.CambiarDisparosNave += new EventHandler<EventArgsCambiarDisparosNave>(tooltip.CambiarDisparosNaveHandler);
            //inicializar tooltips
            CambiarDisparosNave(this, new EventArgsCambiarDisparosNave(this));
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public bool PosibleComprarArma(eTipoDisparo tipoDisparo)
        {
            if (!caracteristicas[(int)tipoDisparo].bActivo)
                return true;
            else if (caracteristicas[(int)tipoDisparo].nivel >= MAXNIVELDISPAROS)
                return false;
            else
                return true;
        }

        public bool ArmaActiva(eTipoDisparo tipoDisparo)
        {
            if (!caracteristicas[(int)tipoDisparo].bActivo)
                return false;
            else
                return true;
        }

        public void ArmaSiguiente(Nave nave, Opciones opciones)
        {
            int iArmaActual = (int)nave.TipoDisparo;
            int iArmaNew = -1;
            int iArma = iArmaActual;
            int i = 0; //limite de salida del bucle
            do  
            {
                i++;
                iArma++;
                if(iArma >= Enum.GetValues(typeof(eTipoDisparo)).Length)
                    iArma = 1; //simple no
                if((caracteristicas[iArma].bActivo) && (iArma != iArmaActual))
                    iArmaNew = iArma;
            } while ((iArmaNew == -1) && (i <= Enum.GetValues(typeof(eTipoDisparo)).Length * 2));

            if (iArmaNew != -1)
            {
                SeleccionarArma(nave, (eTipoDisparo)iArmaNew, opciones);
            }
        }

        public void ArmaAnterior(Nave nave, Opciones opciones)
        {
            int iArmaActual = (int)nave.TipoDisparo;
            int iArmaNew = -1;
            int iArma = iArmaActual;
            int i = 0; //limite de salida del bucle
            do
            {
                i++;
                iArma--;
                if (iArma <= 0) //simple no
                    iArma = Enum.GetValues(typeof(eTipoDisparo)).Length - 1;
                if ((caracteristicas[iArma].bActivo) && (iArma != iArmaActual))
                    iArmaNew = iArma;
            } while ((iArmaNew == -1) && (i <= Enum.GetValues(typeof(eTipoDisparo)).Length * 2));

            if (iArmaNew != -1)
            {
                SeleccionarArma(nave, (eTipoDisparo)iArmaNew, opciones);
            }
        }

        public bool SeleccionarArma(Nave nave, eTipoDisparo tipoDisparo, Opciones opciones)
        {
            if (caracteristicas[(int)tipoDisparo].bActivo)
            {
                nave.TipoDisparo = (eTipoDisparo)tipoDisparo;
                //SetCaracteristicas(nave.TipoDisparo);
                Sonidos.PlayFX(eSonido.CambiarArma, opciones);

                //evento tooltip
                EventArgsCambiarDisparosNave eventArgs = new EventArgsCambiarDisparosNave(this);
                EventHandler<EventArgsCambiarDisparosNave> temp = CambiarDisparosNave;
                if (temp != null)
                    temp(this, eventArgs);

                return true;
            }
            else
                return false;
        }

        //se mejora nivel del arma, hay comprobacion de nivel maximo, de si hay dinero suficiente. al mejorar se paga el precio
        public bool ComprarMejorarArma(Nave nave, eTipoDisparo tipoDisparo)
        {
            bool bComprarMejorar = false; //se pudo o no pudo hacer le upgrade

            if (PosibleComprarArma(tipoDisparo)) //futuro nivel debajo del maximo
            {
                CaracteristicasTipoDisparo caractActual = GetCaracteristicas(caracteristicas, tipoDisparo);
                int nivelUpgrade = caractActual.nivel + 1;
                CaracteristicasTipoDisparo caractUpgrade = GetCaracteristicas(caracteristicas, tipoDisparo, nivelUpgrade);
                int precioUpgrade = caractUpgrade.precio; // -caractActual.precio;

                if (nave.Dinero >= precioUpgrade) //hay dinero suficiente
                {
                    //hacer el upgrade
                    nave.PagarUpgradeTorre(precioUpgrade); //se paga el precio

                    nave.TipoDisparo = tipoDisparo; //se activa el arma
                    SetCaracteristicas(tipoDisparo, nivelUpgrade);
                    bComprarMejorar = true;

                    //evento tooltip
                    EventArgsCambiarDisparosNave eventArgs = new EventArgsCambiarDisparosNave(this);
                    EventHandler<EventArgsCambiarDisparosNave> temp = CambiarDisparosNave;
                    if (temp != null)
                        temp(this, eventArgs);

                }
            }

            return bComprarMejorar;
        }


        //si nivelNew = -1 es q no se modifica el nivel del disparo. bActivo indica si el arma la tenemos disponible
        public static CaracteristicasTipoDisparo GetCaracteristicas(CaracteristicasTipoDisparo[] caracteristicas, eTipoDisparo tipoDisparo, int nivelNew = -1, bool bActivo = true)
        {
            int nivelAct = -1;
            if(caracteristicas[(int)tipoDisparo] != null)
                nivelAct = caracteristicas[(int)tipoDisparo].nivel;


            CaracteristicasTipoDisparo caract = new CaracteristicasTipoDisparo();

            if (nivelNew != -1)
                caract.nivel = nivelNew;
            else
                caract.nivel = nivelAct;

            caract.bActivo = bActivo;


            //http://math2.org/math/trig/es-tables.htm
            //double angulo90 = Math.PI / 2;  //angulo 90º = PI / 2
            //double angulo75 = Math.PI * 15 / 36;  //angulo 75º = PI * 75 / 180 = PI * 15 / 36
            //double angulo60 = Math.PI / 3;  //angulo 60º = PI / 3
            double angulo45 = Math.PI / 4;  //angulo 45º = PI / 4
            double angulo30 = Math.PI / 6;  //angulo 30º = PI / 6
            double angulo15 = Math.PI / 12; //angulo 15º = PI / 12
            //double angulo = A * Math.PI / 180; //angulo A


            caract.dañoAire = -1;
            caract.radioMisiles = -1;
            

            ////////////// SIMPLE
            if (tipoDisparo == eTipoDisparo.Simple)
            {
                caract.precio = -1;
                caract.velocidad = 300f; //velocidad del disparo
                caract.daño = 3; //daño del disparo
                caract.ratio = 2; //disparos por segundo (uno cada 0.5 segs)

                caract.numDisparos = 1; //maximos disparos a la vez
                caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados
                caract.direcciones[0] = new Vector2(0, -1); //disparo arriba

                caracteristicas[(int)tipoDisparo] = caract;
            }

            ////////////// MULTIPLE
            else if (tipoDisparo == eTipoDisparo.Multiple)
            {
                caract.precio = 100 + 50 * caract.nivel;
                caract.velocidad = 250f; //velocidad del disparo
                caract.daño = 3; //daño del disparo
                caract.ratio = 1.43f; //cada 0.7 segs

                if (caract.nivel == 0)
                {
                    caract.numDisparos = 3; //maximos disparos a la vez, 3 arriba
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    //3 disparos, los 3 suben a la misma velocidad vertical
                    caract.direcciones[0] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f);  //disparo arriba izquierda (angulo 30)
                    caract.direcciones[1] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[2] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f); //disparo arriba derecha (angulo 30)

                    //3 disparos, los diagonales tienen velocidad (hor+vert) igual al disparo vertical (1)
                    /*                
                    caract.direcciones[0] = new Vector2((float)-Math.Sin(angulo), (float)-Math.Cos(angulo));    //disparo arriba izquierda (angulo 30)
                    caract.direcciones[1] = new Vector2(0, -1);                                                 //disparo arriba
                    caract.direcciones[2] = new Vector2((float)Math.Sin(angulo), (float)-Math.Cos(angulo));     //disparo arriba derecha (angulo 30)
                    */
                }
                else if (caract.nivel == 1)
                {
                    caract.numDisparos = 6; //maximos disparos a la vez, 5 arriba 1 abajo
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f);  //disparo arriba izquierda (angulo 30)
                    caract.direcciones[1] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f);  //disparo arriba izquierda (angulo 15)
                    caract.direcciones[2] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[3] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f); //disparo arriba derecha (angulo 15)
                    caract.direcciones[4] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f); //disparo arriba derecha (angulo 30)

                    caract.direcciones[5] = new Vector2(0f, 1f);  //disparo abajo
                }
                else if (caract.nivel == 2)
                {
                    caract.numDisparos = 10; //maximos disparos a la vez, 5 arriba 3 abajo, 2 laterales
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f);  //disparo arriba izquierda (angulo 30)
                    caract.direcciones[1] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f);  //disparo arriba izquierda (angulo 15)
                    caract.direcciones[2] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[3] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f); //disparo arriba derecha (angulo 15)
                    caract.direcciones[4] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f); //disparo arriba derecha (angulo 30)

                    caract.direcciones[5] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), 1f);  //disparo abajo izquierda (angulo 30)
                    caract.direcciones[6] = new Vector2(0, 1f);                                                 //disparo abajo
                    caract.direcciones[7] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), 1f); //disparo abajo derecha (angulo 30)

                    caract.direcciones[8] = new Vector2(-1f, 0f);                                                //disparo izquierda
                    caract.direcciones[9] = new Vector2(1f, 0f);                                                //disparo derecha

                }
                else if (caract.nivel == 3)
                {
                    caract.numDisparos = 14; //maximos disparos a la vez, 7 arriba 5 abajo 2 laterales
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2((float)-(Math.Sin(angulo45) / Math.Cos(angulo45)), -1f);  //disparo arriba izquierda (angulo 45)
                    caract.direcciones[1] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f);  //disparo arriba izquierda (angulo 30)
                    caract.direcciones[2] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f);  //disparo arriba izquierda (angulo 15)
                    caract.direcciones[3] = new Vector2(0, -1f);                                                  //disparo arriba
                    caract.direcciones[4] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f);   //disparo arriba derecha (angulo 15)
                    caract.direcciones[5] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f);   //disparo arriba derecha (angulo 30)
                    caract.direcciones[6] = new Vector2((float)(Math.Sin(angulo45) / Math.Cos(angulo45)), -1f);   //disparo arriba derecha (angulo 45)

                    caract.direcciones[7] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), 1f);  //disparo abajo izquierda (angulo 30)
                    caract.direcciones[8] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), 1f);  //disparo abajo izquierda (angulo 15)
                    caract.direcciones[9] = new Vector2(0, 1f);                                                  //disparo abajo
                    caract.direcciones[10] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), 1f);  //disparo abajo derecha (angulo 15)
                    caract.direcciones[11] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), 1f);  //disparo abajo derecha (angulo 30)

                    caract.direcciones[12] = new Vector2(-1f, 0f);                                                //disparo izquierda
                    caract.direcciones[13] = new Vector2(1f, 0f);                                                 //disparo derecha
                }
                else if (caract.nivel == 4)
                {
                    caract.numDisparos = 18; //maximos disparos a la vez, 9 arriba 7 abajo 2 laterales
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[1] = new Vector2((float)-(Math.Sin(angulo45) / Math.Cos(angulo45)), -1f);  //disparo arriba izquierda
                    caract.direcciones[2] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f);  //disparo arriba izquierda
                    caract.direcciones[3] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f);  //disparo arriba izquierda 
                    caract.direcciones[4] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[5] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f); //disparo arriba derecha 
                    caract.direcciones[6] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f); //disparo arriba derecha 
                    caract.direcciones[7] = new Vector2((float)(Math.Sin(angulo45) / Math.Cos(angulo45)), -1f); //disparo arriba derecha

                    caract.direcciones[9] = new Vector2((float)-(Math.Sin(angulo45) / Math.Cos(angulo45)), 1f);  //disparo abajo izquierda 
                    caract.direcciones[10] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), 1f);  //disparo abajo izquierda 
                    caract.direcciones[11] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), 1f);  //disparo abajo izquierda 
                    caract.direcciones[12] = new Vector2(0, 1f);                                                //disparo abajo
                    caract.direcciones[13] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), 1f); //disparo abajo derecha
                    caract.direcciones[14] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), 1f); //disparo abajo derecha 
                    caract.direcciones[15] = new Vector2((float)(Math.Sin(angulo45) / Math.Cos(angulo45)), 1f); //disparo abajo derecha 

                    caract.direcciones[16] = new Vector2(-1f, 0f);                                                //disparo izquierda
                    caract.direcciones[17] = new Vector2(1f, 0f);                                                //disparo derecha

                    caract.direcciones[0] = new Vector2(-1f, (float)-(Math.Sin(angulo30) / Math.Cos(angulo30)));  //disparo arriba izquierda 60
                    caract.direcciones[8] = new Vector2(1f, (float)-(Math.Sin(angulo30) / Math.Cos(angulo30)));  //disparo arriba derecha 60
                }
                else if (caract.nivel == 5)
                {
                    caract.numDisparos = 24; //maximos disparos a la vez, 11 arriba 11 abajo 2 laterales
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[2] = new Vector2((float)-(Math.Sin(angulo45) / Math.Cos(angulo45)), -1f);  //disparo arriba izquierda 
                    caract.direcciones[3] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f);  //disparo arriba izquierda
                    caract.direcciones[4] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f);  //disparo arriba izquierda 
                    caract.direcciones[5] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[6] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f); //disparo arriba derecha
                    caract.direcciones[7] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f); //disparo arriba derecha
                    caract.direcciones[8] = new Vector2((float)(Math.Sin(angulo45) / Math.Cos(angulo45)), -1f); //disparo arriba derecha 

                    caract.direcciones[13] = new Vector2((float)-(Math.Sin(angulo45) / Math.Cos(angulo45)), 1f);  //disparo abajo izquierda 
                    caract.direcciones[14] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), 1f);  //disparo abajo izquierda
                    caract.direcciones[15] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), 1f);  //disparo abajo izquierda 
                    caract.direcciones[16] = new Vector2(0, 1f);                                                //disparo abajo
                    caract.direcciones[17] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), 1f); //disparo abajo derecha
                    caract.direcciones[18] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), 1f); //disparo abajo derecha
                    caract.direcciones[19] = new Vector2((float)(Math.Sin(angulo45) / Math.Cos(angulo45)), 1f); //disparo abajo derecha 

                    caract.direcciones[22] = new Vector2(-1f, 0f);                                                //disparo izquierda
                    caract.direcciones[23] = new Vector2(1f, 0f);                                                //disparo derecha

                    caract.direcciones[0] = new Vector2(-1f, (float)-(Math.Sin(angulo15) / Math.Cos(angulo15)));  //disparo arriba izquierda 75
                    caract.direcciones[1] = new Vector2(-1f, (float)-(Math.Sin(angulo30) / Math.Cos(angulo30)));  //disparo arriba izquierda 60
                    caract.direcciones[9] = new Vector2(1f, (float)-(Math.Sin(angulo30) / Math.Cos(angulo30)));  //disparo arriba derecha 60
                    caract.direcciones[10] = new Vector2(1f, (float)-(Math.Sin(angulo15) / Math.Cos(angulo15)));  //disparo arriba derecha 75
                    caract.direcciones[11] = new Vector2(-1f, (float)(Math.Sin(angulo15) / Math.Cos(angulo15)));  //disparo abajo izquierda 75
                    caract.direcciones[12] = new Vector2(-1f, (float)(Math.Sin(angulo30) / Math.Cos(angulo30)));  //disparo abajo izquierda 60
                    caract.direcciones[20] = new Vector2(1f, (float)(Math.Sin(angulo30) / Math.Cos(angulo30)));  //disparo abajo derecha 60
                    caract.direcciones[21] = new Vector2(1f, (float)(Math.Sin(angulo15) / Math.Cos(angulo15)));  //disparo abajo derecha 75
                }
            }

            ////////////// LASER
            else if (tipoDisparo == eTipoDisparo.Laser)
            {
                caract.precio = 150 + 50 * caract.nivel;
                caract.velocidad = 350f; //velocidad del disparo
                caract.daño = 3; //daño del disparo
                caract.ratio = 1.66f; //cada 0.6 segs

                if (caract.nivel == 0)
                {
                    caract.numDisparos = 1; //maximos disparos a la vez, 1 arriba
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2(0, -1f);                                                //disparo arriba
                }
                else if (caract.nivel == 1)
                {
                    caract.numDisparos = 2; //maximos disparos a la vez, 2 arriba
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[1] = new Vector2(0, -1f);                                                //disparo arriba

                    caract.posiciones = new Vector2[caract.numDisparos]; //posiciones de salida
                    caract.posiciones[0] = new Vector2(-5f, 0f);                                                //disparo arriba
                    caract.posiciones[1] = new Vector2(5f, 0f);                                                //disparo arriba
                }
                else if (caract.nivel == 2)
                {
                    caract.numDisparos = 4; //maximos disparos a la vez, 3 arriba 1 abajo
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[1] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[2] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[3] = new Vector2(0, 1f);                                                 //disparo abajo

                    caract.posiciones = new Vector2[caract.numDisparos]; //posiciones de salida
                    caract.posiciones[0] = new Vector2(-10f, 0f);                                               //disparo arriba
                    caract.posiciones[1] = Vector2.Zero;                                                        //disparo arriba
                    caract.posiciones[2] = new Vector2(10f, 0f);                                                //disparo arriba
                    caract.posiciones[3] = Vector2.Zero;                                                        //disparo abajo
                }
                else if (caract.nivel == 3)
                {
                    caract.numDisparos = 6; //maximos disparos a la vez, 4 arriba 2 abajo
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[1] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[2] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[3] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[4] = new Vector2(0, 1f);                                                  //disparo abajo
                    caract.direcciones[5] = new Vector2(0, 1f);                                                  //disparo abajo

                    caract.posiciones = new Vector2[caract.numDisparos]; //posiciones de salida
                    caract.posiciones[0] = new Vector2(-12f, 0f);                                               //disparo arriba
                    caract.posiciones[1] = new Vector2(-4f, 0f);                                                //disparo arriba
                    caract.posiciones[2] = new Vector2(4f, 0f);                                                 //disparo arriba
                    caract.posiciones[3] = new Vector2(12f, 0f);                                                //disparo arriba
                    caract.posiciones[4] = new Vector2(-5f, 0f);                                                 //disparo abajo
                    caract.posiciones[5] = new Vector2(5f, 0f);                                                  //disparo abajo
                }
                else if (caract.nivel == 4)
                {
                    caract.numDisparos = 8; //maximos disparos a la vez, 5 arriba 3 abajo
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[1] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[2] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[3] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[4] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[5] = new Vector2(0, 1f);                                                  //disparo abajo
                    caract.direcciones[6] = new Vector2(0, 1f);                                                  //disparo abajo
                    caract.direcciones[7] = new Vector2(0, 1f);                                                  //disparo abajo

                    caract.posiciones = new Vector2[caract.numDisparos]; //posiciones de salida
                    caract.posiciones[0] = new Vector2(-14f, 0f);                                               //disparo arriba
                    caract.posiciones[1] = new Vector2(-7f, 0f);                                                //disparo arriba
                    caract.posiciones[2] = new Vector2(0f, 0f);                                                 //disparo arriba
                    caract.posiciones[3] = new Vector2(7f, 0f);                                                 //disparo arriba
                    caract.posiciones[4] = new Vector2(14f, 0f);                                                //disparo arriba
                    caract.posiciones[5] = new Vector2(-10f, 0f);                                                //disparo abajo
                    caract.posiciones[6] = new Vector2(0f, 0f);                                                  //disparo abajo
                    caract.posiciones[7] = new Vector2(10f, 0f);                                                 //disparo abajo
                }
                else if (caract.nivel == 5)
                {
                    caract.numDisparos = 13; //maximos disparos a la vez, 8 arriba 5 abajo
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[1] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[2] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[3] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[4] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[5] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[6] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[7] = new Vector2(0, -1f);                                                //disparo arriba
                    caract.direcciones[8] = new Vector2(0, 1f);                                                  //disparo abajo
                    caract.direcciones[9] = new Vector2(0, 1f);                                                  //disparo abajo
                    caract.direcciones[10] = new Vector2(0, 1f);                                                 //disparo abajo
                    caract.direcciones[11] = new Vector2(0, 1f);                                                 //disparo abajo
                    caract.direcciones[12] = new Vector2(0, 1f);                                                 //disparo abajo

                    caract.posiciones = new Vector2[caract.numDisparos]; //posiciones de salida
                    caract.posiciones[0] = new Vector2(-14f, -17f);                                             //disparo arriba
                    caract.posiciones[1] = new Vector2(-7f, -17f);                                              //disparo arriba
                    caract.posiciones[2] = new Vector2(0f, -17f);                                               //disparo arriba
                    caract.posiciones[3] = new Vector2(7f, -17f);                                               //disparo arriba
                    caract.posiciones[4] = new Vector2(14f, -17f);                                              //disparo arriba
                    caract.posiciones[5] = new Vector2(-10f, 0f);                                               //disparo arriba
                    caract.posiciones[6] = new Vector2(0f, 0f);                                                 //disparo arriba
                    caract.posiciones[7] = new Vector2(10f, 0f);                                                 //disparo arriba
                    caract.posiciones[8] = new Vector2(-14f, 0f);                                                //disparo abajo
                    caract.posiciones[9] = new Vector2(-7f, 0f);                                                 //disparo abajo
                    caract.posiciones[10] = new Vector2(0f, 0f);                                                 //disparo abajo
                    caract.posiciones[11] = new Vector2(7f, 0f);                                                 //disparo abajo
                    caract.posiciones[12] = new Vector2(14f, 0f);                                                //disparo abajo
                }
            }

            ////////////// MISILES
            else if (tipoDisparo == eTipoDisparo.Misiles)
            {
                caract.precio = 200 + 50 * caract.nivel;
                caract.velocidad = 200f; //velocidad del disparo
                caract.daño = 3; //daño del disparo
                caract.dañoAire = 6; //daño disparo contra enemigo aereo
                caract.ratio = 1f; //cada 1 segs

                caract.radioMisiles = 40 + caract.nivel * 10; //dentro del radio localiza enemigos y se dirije a por ellos

                if (caract.nivel == 0)
                {
                    caract.numDisparos = 1; //maximos disparos a la vez, 1 arriba
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2(0, -1f);                                                //disparo arriba
                }
                else if (caract.nivel == 1)
                {
                    caract.numDisparos = 2; //maximos disparos a la vez, 2 arriba
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    //caract.direcciones[0] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f); //disparo arriba izq
                    //caract.direcciones[1] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f); //disparo arriba der
                    caract.direcciones[0] = new Vector2(0, -1f); //disparo arriba izq
                    caract.direcciones[1] = new Vector2(0, -1f); //disparo arriba der

                    caract.posiciones = new Vector2[caract.numDisparos]; //posiciones de salida
                    caract.posiciones[0] = new Vector2(-10f, 0f);                                                //disparo arriba izq
                    caract.posiciones[1] = new Vector2(10f, 0f);                                                 //disparo arriba der
                }
                else if (caract.nivel == 2)
                {
                    caract.numDisparos = 4; //maximos disparos a la vez, 3 arriba 1 abajo
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f); //disparo arriba izq
                    caract.direcciones[1] = new Vector2(0, -1f);                                                 //disparo arriba
                    caract.direcciones[2] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f); //disparo arriba der
                    caract.direcciones[3] = new Vector2(0, 1f);                                                   //disparo abajo

                    caract.posiciones = new Vector2[caract.numDisparos]; //posiciones de salida
                    caract.posiciones[0] = new Vector2(-10f, 0f);                                                //disparo arriba izq
                    caract.posiciones[1] = new Vector2(0, 0f);                                                   //disparo arriba
                    caract.posiciones[2] = new Vector2(10f, 0f);                                                 //disparo arriba der
                    caract.posiciones[3] = new Vector2(0, 0f);                                                    //disparo abajo
                }
                else if (caract.nivel == 3)
                {
                    caract.numDisparos = 6; //maximos disparos a la vez, 4 arriba 2 abajo
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f); //disparo arriba izq
                    caract.direcciones[1] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f); //disparo arriba izq
                    caract.direcciones[2] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f);  //disparo arriba der
                    caract.direcciones[3] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f);  //disparo arriba der
                    caract.direcciones[4] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), 1f);   //disparo abajo izq
                    caract.direcciones[5] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), 1f);    //disparo abajo der

                    caract.posiciones = new Vector2[caract.numDisparos]; //posiciones de salida
                    caract.posiciones[0] = new Vector2(-12f, 0f);                                                //disparo arriba izq
                    caract.posiciones[1] = new Vector2(-4, 0f);                                                  //disparo arriba izq
                    caract.posiciones[2] = new Vector2(4f, 0f);                                                  //disparo arriba der
                    caract.posiciones[3] = new Vector2(12f, 0f);                                                 //disparo arriba der
                    caract.posiciones[4] = new Vector2(-5f, 0f);                                                  //disparo abajo izq
                    caract.posiciones[5] = new Vector2(5f, 0f);                                                   //disparo abajo der
                }
                else if (caract.nivel == 4)
                {
                    caract.numDisparos = 10; //maximos disparos a la vez, 5 arriba 3 abajo, 2 laterales
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f); //disparo arriba izq
                    caract.direcciones[1] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f); //disparo arriba izq
                    caract.direcciones[2] = new Vector2(0f, -1f);                                                //disparo arriba
                    caract.direcciones[3] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f);  //disparo arriba der
                    caract.direcciones[4] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f);  //disparo arriba der
                    caract.direcciones[5] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), 1f);   //disparo abajo izq
                    caract.direcciones[6] = new Vector2(0f, 1f);                                                  //disparo abajo
                    caract.direcciones[7] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), 1f);    //disparo abajo der
                    caract.direcciones[8] = new Vector2(-1f, 0f);                                                //disparo izquierda
                    caract.direcciones[9] = new Vector2(1f, 0f);                                                 //disparo derecha

                    caract.posiciones = new Vector2[caract.numDisparos]; //posiciones de salida
                    caract.posiciones[0] = new Vector2(-14f, 0f);                                                //disparo arriba izq
                    caract.posiciones[1] = new Vector2(-7, 0f);                                                  //disparo arriba izq
                    caract.posiciones[2] = new Vector2(0f, 0f);                                                  //disparo arriba
                    caract.posiciones[3] = new Vector2(7f, 0f);                                                  //disparo arriba der
                    caract.posiciones[4] = new Vector2(14f, 0f);                                                 //disparo arriba der
                    caract.posiciones[5] = new Vector2(-10f, 0f);                                                 //disparo abajo izq
                    caract.posiciones[6] = new Vector2(0f, 0f);                                                   //disparo abajo
                    caract.posiciones[7] = new Vector2(10f, 0f);                                                  //disparo abajo der
                    caract.posiciones[8] = new Vector2(0f, 0f);                                                  //disparo izquierda
                    caract.posiciones[9] = new Vector2(0f, 0f);                                                  //disparo derecha
                }
                else if (caract.nivel == 5)
                {
                    caract.numDisparos = 14; //maximos disparos a la vez, 5 arriba 5 abajo, 4 laterales
                    caract.direcciones = new Vector2[caract.numDisparos]; //vectores normalizados

                    caract.direcciones[0] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f);  //disparo arriba izq
                    caract.direcciones[1] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f);  //disparo arriba izq
                    caract.direcciones[2] = new Vector2(0f, -1f);                                                 //disparo arriba
                    caract.direcciones[3] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), -1f);   //disparo arriba der
                    caract.direcciones[4] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), -1f);   //disparo arriba der
                    caract.direcciones[5] = new Vector2((float)-(Math.Sin(angulo30) / Math.Cos(angulo30)), 1f);    //disparo abajo izq
                    caract.direcciones[6] = new Vector2((float)-(Math.Sin(angulo15) / Math.Cos(angulo15)), 1f);    //disparo abajo izq
                    caract.direcciones[7] = new Vector2(0f, 1f);                                                   //disparo abajo
                    caract.direcciones[8] = new Vector2((float)(Math.Sin(angulo15) / Math.Cos(angulo15)), 1f);     //disparo abajo izq
                    caract.direcciones[9] = new Vector2((float)(Math.Sin(angulo30) / Math.Cos(angulo30)), 1f);     //disparo abajo der
                    caract.direcciones[10] = new Vector2(-1f, (float)-(Math.Sin(angulo15) / Math.Cos(angulo15))); //disparo arriba izquierda 75
                    caract.direcciones[11] = new Vector2(-1f, (float)(Math.Sin(angulo15) / Math.Cos(angulo15)));  //disparo abajo izquierda 75
                    caract.direcciones[12] = new Vector2(1f, (float)-(Math.Sin(angulo15) / Math.Cos(angulo15)));  //disparo arriba derecha 75
                    caract.direcciones[13] = new Vector2(1f, (float)(Math.Sin(angulo15) / Math.Cos(angulo15)));   //disparo abajo derecha 75

                    caract.posiciones = new Vector2[caract.numDisparos]; //posiciones de salida
                    caract.posiciones[0] = new Vector2(-14f, 0f);                                                //disparo arriba izq
                    caract.posiciones[1] = new Vector2(-7, 0f);                                                  //disparo arriba izq
                    caract.posiciones[2] = new Vector2(0f, 0f);                                                  //disparo arriba
                    caract.posiciones[3] = new Vector2(7f, 0f);                                                  //disparo arriba der
                    caract.posiciones[4] = new Vector2(14f, 0f);                                                 //disparo arriba der
                    caract.posiciones[5] = new Vector2(-14f, 0f);                                                 //disparo abajo izq
                    caract.posiciones[6] = new Vector2(-7f, 0f);                                                  //disparo abajo izq
                    caract.posiciones[7] = new Vector2(0f, 0f);                                                   //disparo abajo
                    caract.posiciones[8] = new Vector2(7f, 0f);                                                   //disparo abajo der
                    caract.posiciones[9] = new Vector2(14f, 0f);                                                  //disparo abajo der
                    caract.posiciones[10] = new Vector2(0f, 0f);                                                 //disparo arriba izquierda 75
                    caract.posiciones[11] = new Vector2(0f, 0f);                                                 //disparo abajo izquierda 75
                    caract.posiciones[12] = new Vector2(0f, 0f);                                                 //disparo arriba derecha 75
                    caract.posiciones[13] = new Vector2(0f, 0f);                                                 //disparo abajo derecha 75
                }
            }


            if (caract.dañoAire == -1)
                caract.dañoAire = caract.daño;

            return caract;
        }

        private void SetCaracteristicas(eTipoDisparo tipoDisparo, int nivelNew, bool bActivo = true)
        {
            caracteristicas[(int)tipoDisparo] = GetCaracteristicas(caracteristicas, tipoDisparo, nivelNew, bActivo);
        }

        public static void SetCaracteristicas(CaracteristicasTipoDisparo[] caract, eTipoDisparo tipoDisparo, int nivelNew, bool bActivo = true)
        {
            caract[(int)tipoDisparo] = GetCaracteristicas(caract, tipoDisparo, nivelNew, bActivo);
        }


        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager Content)
        {
            //precarga el contenido de los disparos
            sprite[(int)eTipoDisparo.Simple] = new Sprite();
            sprite[(int)eTipoDisparo.Simple].imagen = Content.Load<Texture2D>("Sprites\\nave_disparos_01");
            sprite[(int)eTipoDisparo.Simple].dimImagen = new Point(9, 3);
            sprite[(int)eTipoDisparo.Simple].posImagen = new Point(0, 0);

            sprite[(int)eTipoDisparo.Multiple] = new Sprite();
            //sprite[(int)eTipoDisparo.Multiple].imagen = Content.Load<Texture2D>("Sprites\\nave_disparos_01");
            sprite[(int)eTipoDisparo.Multiple].imagen = sprite[(int)eTipoDisparo.Simple].imagen;
            sprite[(int)eTipoDisparo.Multiple].dimImagen = new Point(9, 3);
            sprite[(int)eTipoDisparo.Multiple].posImagen = new Point(0, 0);

            sprite[(int)eTipoDisparo.Laser] = new Sprite();
            //sprite[(int)eTipoDisparo.Laser].imagen = Content.Load<Texture2D>("Sprites\\nave_disparos_01");
            sprite[(int)eTipoDisparo.Laser].imagen = sprite[(int)eTipoDisparo.Simple].imagen;
            sprite[(int)eTipoDisparo.Laser].dimImagen = new Point(16, 3);
            sprite[(int)eTipoDisparo.Laser].posImagen = new Point(9, 0);

            sprite[(int)eTipoDisparo.Misiles] = new Sprite();
            //sprite[(int)eTipoDisparo.Misiles].imagen = Content.Load<Texture2D>("Sprites\\nave_disparos_missil_01");
            //sprite[(int)eTipoDisparo.Misiles].dimImagen = new Point(27, 9);
            sprite[(int)eTipoDisparo.Misiles].imagen = Content.Load<Texture2D>("Sprites\\nave_disparos_misil_01");
            sprite[(int)eTipoDisparo.Misiles].dimImagen = new Point(32, 7);
            sprite[(int)eTipoDisparo.Misiles].posImagen = new Point(0, 0);
        }


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, KeyboardState keyboardState, Nave nave, eTipoDisparo tipoDisparo, Enemigos enemigos, Opciones opciones)
        {
            CaracteristicasTipoDisparo caract = caracteristicas[(int)tipoDisparo];

            // Fire only every interval we set as the fireRate
            if (gameTime.TotalGameTime - previoFireTime > TimeSpan.FromSeconds(1 / caract.ratio))
            {
                if (keyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.Disparo)) || nave.autoDisparo)
                {
                    //si hay menos disparos q el limite
                    if (disparos.Count < MAXDISPAROS)
                    {
                        //lanza los disparos
                        for (int iDisparo = 0; iDisparo < caract.numDisparos; iDisparo++)
                        {
                            Vector2 posicionDisparo = Vector2.Zero;
                            if (caract.posiciones != null)
                            {
                                if (caract.posiciones[iDisparo] != null)
                                {
                                    posicionDisparo = caract.posiciones[iDisparo];
                                }
                            }

                            Disparo disp = new Disparo(gameTime, tipoDisparo, caract.direcciones[iDisparo], posicionDisparo,
                                                       nave.posicion, sprite[(int)tipoDisparo], caract);
                            disp.FueraDePantalla += new EventHandler(FueraDePantallaHandler);
                            disparos.Add(disp);
                        }

                        // Reset our current time
                        previoFireTime = gameTime.TotalGameTime;

                        //sonido disparo
                        if((tipoDisparo == eTipoDisparo.Simple) || (tipoDisparo == eTipoDisparo.Multiple))
                            Sonidos.PlayFX(eSonido.Laser1, opciones);
                        else if(tipoDisparo == eTipoDisparo.Laser)
                            Sonidos.PlayFX(eSonido.Laser2, opciones);
                        else if(tipoDisparo == eTipoDisparo.Misiles)
                            Sonidos.PlayFX(eSonido.Misil, opciones);

                    }
                }
            }

            //expresion lambda, no se usa foreach pq puede saltar el evento fuera pantalla y reducirse la lista
            //disparos.ForEach(d => d.Update(gameTime, sprite[(int)d.TipoDisparo], ventana));
            Disparo disparo;
            for (int iDisparo = disparos.Count - 1; iDisparo >= 0 ; iDisparo--)
            {
                disparo = disparos[iDisparo];

                if(disparo.bEliminar)
                    disparos.RemoveAt(iDisparo);
                else
                    disparo.Update(gameTime, sprite[(int)disparo.TipoDisparo], ventana, enemigos);
            }
        }


        ////////////////// EVENTOS //////////////////
        private void FueraDePantallaHandler(Object sender, EventArgs args)
        {
            //disparos.Remove((Disparo)sender);
            ((Disparo)sender).bEliminar = true;
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb)
        {
            foreach (Disparo disparo in disparos)
            {
                disparo.Draw(spb, sprite[(int)disparo.TipoDisparo]);
            }

        }

    }
}
