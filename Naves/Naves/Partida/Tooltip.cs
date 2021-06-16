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
    enum eTooltip
    {
        Precio, Precio_Enemigo, Vida, Ataque_Torre, Ataque_Nave, Ataque_Enemigo, Alcance, Velocidad, Ratio, //menuTorres
        Multiple, Laser, Misiles, Rayo, Comprar_Multiple, Comprar_Laser, Comprar_Misiles, Comprar_Rayo,     //menuTorres nave
        Mejorar_Multiple, Mejorar_Laser, Mejorar_Misiles, Mejorar_Rayo,                                     //menuTorres nave
        Inactivo_Multiple, Inactivo_Laser, Inactivo_Misiles, Inactivo_Rayo,                                 //menuTorres nave
        Mejorar, Vender,                                                                                    //menuTorres btn torre
        Dinero, Tiempo, Vida_Nave, Wave_Tooltip                                                             //marcador
    }


    class EventArgsCambiarTorreSelected : EventArgs
    {
        public Nave nave;
        public Torres torres;
        public int selectedTorre;

        public EventArgsCambiarTorreSelected(Nave nave, Torres torres, int selectedTorre)
        {
            this.nave = nave;
            this.torres = torres;
            this.selectedTorre = selectedTorre;
        }
    }

    class EventArgsCambiarDisparosNave : EventArgs
    {
        public Disparos disparos;

        public EventArgsCambiarDisparosNave(Disparos disparos)
        {
            this.disparos = disparos;
        }
    }


    struct DatosTooltip
    {
        public Rectangle posicion; //posicion y tamaño
        public TimeSpan tiempo; //tiempo lleva el raton encima
        //public string sPre; //pre mensaje
        public string sPost; //post mensaje
    }

    class Tooltip
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        //posicion de iconos y botones para poner el tooltip
        public DatosTooltip[] datos;

        private Sprite sprite = new Sprite();
        private SpriteFont font;

        private static TimeSpan tiempoTooltip = TimeSpan.FromSeconds(0.5); //cuanto tiempo es necesario para sacar el tooltip
        private static float ALTURA = 5; //distancia entre icono y panel tooltip
        private static Rectangle NOPOSICION = new Rectangle(-999, -999, -999, -999);
        private static int margenX = 6;
        private static int margenY = 0;


        ////////////////// PUBLICAS //////////////////

        public Vector2 VectorPosicion(eTooltip tooltip)
        {
            return new Vector2(datos[(int)tooltip].posicion.X, datos[(int)tooltip].posicion.Y);
        }

        public string Texto(eTooltip tooltip)
        {
            string str = string.Empty;

            switch(tooltip)
            {
                case eTooltip.Alcance:
                    str = Recursos.Alcance;
                    break;
                case eTooltip.Ataque_Enemigo:
                    str = Recursos.Ataque_Enemigo;
                    break;
                case eTooltip.Ataque_Nave:
                    str = Recursos.Ataque_Nave;
                    break;
                case eTooltip.Ataque_Torre:
                    str = Recursos.Ataque_Torre;
                    break;
                case eTooltip.Comprar_Laser:
                    str = Recursos.Comprar + " " + Recursos.Laser; //añade las estadisticas (daño y numero de disparos) del nivel 0
                    break;
                case eTooltip.Comprar_Multiple:
                    str = Recursos.Comprar + " " + Recursos.Multiple; //añade las estadisticas (daño y numero de disparos) del nivel 0
                    break;
                case eTooltip.Comprar_Misiles:
                    str = Recursos.Comprar + " " + Recursos.Misiles; //añade las estadisticas (daño y numero de disparos) del nivel 0
                    break;
                case eTooltip.Comprar_Rayo:
                    str = Recursos.Comprar + " " + Recursos.Rayo; //añade las estadisticas (daño y numero de disparos) del nivel 0
                    break;
                case eTooltip.Inactivo_Laser:
                    str = Recursos.Laser + " " + Recursos.inactivo; //indica q no lo has comprado
                    break;
                case eTooltip.Inactivo_Multiple:
                    str = Recursos.Multiple + " " + Recursos.inactivo; //indica q no lo has comprado
                    break;
                case eTooltip.Inactivo_Misiles:
                    str = Recursos.Misiles + " " + Recursos.inactivo; //indica q no lo has comprado
                    break;
                case eTooltip.Inactivo_Rayo:
                    str = Recursos.Rayo + " " + Recursos.inactivo; //indica q no lo has comprado
                    break;
                case eTooltip.Dinero:
                    str = Recursos.Dinero;
                    break;
                case eTooltip.Laser:
                    str = Recursos.Laser; //añade las estadisticas (daño y numero de disparos) del nivel actual
                    break;
                case eTooltip.Mejorar:
                    str = Recursos.Mejorar;
                    break;
                case eTooltip.Mejorar_Laser:
                    str = Recursos.Mejorar + " " + Recursos.Laser; //añade las estadisticas (daño y numero de disparos) del nivel siguiente
                    break;
                case eTooltip.Mejorar_Multiple:
                    str = Recursos.Mejorar + " " + Recursos.Multiple; //añade las estadisticas (daño y numero de disparos) del nivel siguiente
                    break;
                case eTooltip.Mejorar_Misiles:
                    str = Recursos.Mejorar + " " + Recursos.Misiles; //añade las estadisticas (daño y numero de disparos) del nivel siguiente
                    break;
                case eTooltip.Mejorar_Rayo:
                    str = Recursos.Mejorar + " " + Recursos.Rayo; //añade las estadisticas (daño y numero de disparos) del nivel siguiente
                    break;
                case eTooltip.Multiple:
                    str = Recursos.Multiple; //añade las estadisticas (daño y numero de disparos) del nivel actual
                    break;
                case eTooltip.Misiles:
                    str = Recursos.Misiles; //añade las estadisticas (daño y numero de disparos) del nivel actual
                    break;
                case eTooltip.Precio:
                    str = Recursos.Precio;
                    break;
                case eTooltip.Precio_Enemigo:
                    str = Recursos.Precio_Enemigo;
                    break;
                case eTooltip.Ratio:
                    str = Recursos.Ratio;
                    break;
                case eTooltip.Rayo: //añade las estadisticas (daño y numero de disparos) del nivel actual
                    str = Recursos.Rayo;
                    break;
                case eTooltip.Vender:
                    str = Recursos.Sell; //añade el dinero q se recibe con la venta
                    break;
                case eTooltip.Tiempo:
                    str = Recursos.Tiempo;
                    break;
                case eTooltip.Velocidad:
                    str = Recursos.Velocidad;
                    break;
                case eTooltip.Vida:
                    str = Recursos.Vida;
                    break;
                case eTooltip.Vida_Nave:
                    str = Recursos.Vida_Nave;
                    break;
                case eTooltip.Wave_Tooltip:
                    str = Recursos.Wave_Tooltip;
                    break;
            }

            return str;
        }


        ////////////////// EVENTOS //////////////////
        public void CambiarTorreSelectedHandler(Object sender, EventArgsCambiarTorreSelected args)
        {
            if (args.selectedTorre != -1)
                datos[(int)eTooltip.Vender].sPost = " (" + Recursos.Ganas + " " + args.nave.DineroVentaTorre(args.torres.torres[args.selectedTorre].Precio).ToString() + " $)";
        }

        private string PostDisparos(CaracteristicasTipoDisparo caract)
        {
            return " (" + Recursos.Daño + " " + caract.daño.ToString() + ", " +
                   Recursos.Daño_antiaereo + " " + caract.dañoAire.ToString() + ", " +
                   Recursos.Numero_disparos + " " + caract.numDisparos.ToString() + ", " +
                   Recursos.Ratio + " " + caract.MostrarRatio.ToString() + ")";
        }

        public void CambiarDisparosNaveHandler(Object sender, EventArgsCambiarDisparosNave args)
        {
            CaracteristicasTipoDisparo[] caracteristicas = args.disparos.Caracteristicas;
            CaracteristicasTipoDisparo caract;
            eTipoDisparo tipoDisparo;

            tipoDisparo = eTipoDisparo.Multiple;
            caract = args.disparos.Caracteristicas[(int)tipoDisparo];
            datos[(int)eTooltip.Multiple].sPost = PostDisparos(caract);
            caract = Disparos.GetCaracteristicas(caracteristicas, tipoDisparo, caract.nivel + 1);
            datos[(int)eTooltip.Mejorar_Multiple].sPost = PostDisparos(caract);
            caract = Disparos.GetCaracteristicas(caracteristicas, tipoDisparo, 0);
            datos[(int)eTooltip.Comprar_Multiple].sPost = PostDisparos(caract);

            tipoDisparo = eTipoDisparo.Laser;
            caract = args.disparos.Caracteristicas[(int)tipoDisparo];
            datos[(int)eTooltip.Laser].sPost = PostDisparos(caract);
            caract = Disparos.GetCaracteristicas(caracteristicas, tipoDisparo, caract.nivel + 1);
            datos[(int)eTooltip.Mejorar_Laser].sPost = PostDisparos(caract);
            caract = Disparos.GetCaracteristicas(caracteristicas, tipoDisparo, 0);
            datos[(int)eTooltip.Comprar_Laser].sPost = PostDisparos(caract);

            tipoDisparo = eTipoDisparo.Misiles;
            caract = args.disparos.Caracteristicas[(int)tipoDisparo];
            datos[(int)eTooltip.Misiles].sPost = PostDisparos(caract);
            caract = Disparos.GetCaracteristicas(caracteristicas, tipoDisparo, caract.nivel + 1);
            datos[(int)eTooltip.Mejorar_Misiles].sPost = PostDisparos(caract);
            caract = Disparos.GetCaracteristicas(caracteristicas, tipoDisparo, 0);
            datos[(int)eTooltip.Comprar_Misiles].sPost = PostDisparos(caract);

            tipoDisparo = eTipoDisparo.Rayo;
            caract = args.disparos.Caracteristicas[(int)tipoDisparo];
            datos[(int)eTooltip.Rayo].sPost = PostDisparos(caract);
            caract = Disparos.GetCaracteristicas(caracteristicas, tipoDisparo, caract.nivel + 1);
            datos[(int)eTooltip.Mejorar_Rayo].sPost = PostDisparos(caract);
            caract = Disparos.GetCaracteristicas(caracteristicas, tipoDisparo, 0);
            datos[(int)eTooltip.Comprar_Rayo].sPost = PostDisparos(caract);
        }


        ////////////////// CONSTRUCTOR //////////////////
        public Tooltip()
        {
            datos = new DatosTooltip[Enum.GetValues(typeof(eTooltip)).Length];
            BorrarPosicion();

            for (int iTooltip = 0; iTooltip < Enum.GetValues(typeof(eTooltip)).Length; iTooltip++)
            {
                datos[iTooltip].tiempo = TimeSpan.Zero;
            }

        }

        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public void BorrarPosicion()
        {
            //en cada update (de menuTorres) se borra y luego se rellenan las posiciones (en menuTorres y marcador)
            for (int iTooltip = 0; iTooltip < Enum.GetValues(typeof(eTooltip)).Length; iTooltip++)
            {
                datos[iTooltip].posicion = NOPOSICION;
            }
        }


        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager content)
        {
            //recuadro del tooltip
            sprite.imagen = content.Load<Texture2D>("Sprites\\blank");
            sprite.dimImagen = new Point(4, 4);
            sprite.posImagen = new Point(0, 0);

            //fuente
            font = content.Load<SpriteFont>("Fuentes\\Tooltip");
        }


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, MouseState currentMouseState, Nave nave, Torres torres, int selectedTorre)
        {
            Point posRaton = new Point(currentMouseState.X, currentMouseState.Y);

            for (int iTooltip = 0; iTooltip < Enum.GetValues(typeof(eTooltip)).Length; iTooltip++)
            {
                if (datos[iTooltip].posicion != NOPOSICION) //si existe el icono/boton
                {
                    if (datos[iTooltip].posicion.Contains(posRaton))
                    {
                        datos[iTooltip].tiempo += gameTime.ElapsedGameTime;
                    }
                    else
                    {
                        datos[iTooltip].tiempo = TimeSpan.Zero;
                    }
                }
            }
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb)
        {
            for (int iTooltip = 0; iTooltip < Enum.GetValues(typeof(eTooltip)).Length; iTooltip++)
            {
                if (datos[iTooltip].posicion != NOPOSICION)
                {
                    if (datos[iTooltip].tiempo > tiempoTooltip)
                    {
                        //calcular posicion del tooltip, debajo del icono boton (a una distancia de ALTURA)
                        Vector2 pos = new Vector2(datos[iTooltip].posicion.X, datos[iTooltip].posicion.Y);
                        pos += new Vector2(0, datos[iTooltip].posicion.Height + ALTURA);

                        //poner tooltip
                        string sTexto = Texto((eTooltip)iTooltip);

                        //if (datos[iTooltip].sPre != string.Empty)
                        //    sTexto = datos[iTooltip].sPre + sTexto;
                        if (datos[iTooltip].sPost != string.Empty)
                            sTexto = sTexto + datos[iTooltip].sPost;

                        Vector2 tamTexto = font.MeasureString(sTexto);
                        int ancho = (int)tamTexto.X;
                        int alto = (int)tamTexto.Y;

                        Rectangle rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, ancho + margenX * 2, alto + margenY * 2);
                        spb.Draw(sprite.imagen, pos, rectangulo, Color.LightGray);

                        spb.DrawString(font, sTexto, pos + new Vector2(margenX, margenY), Color.Black);

                    }
                }
            }
        }
    }
}
