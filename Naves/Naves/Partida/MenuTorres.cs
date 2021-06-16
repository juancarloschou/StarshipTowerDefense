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
    enum eItems
    {
        Dinero, Vida, Tiempo, Ataque, Alcance, Velocidad
    }

    enum eItemArma
    {
        Multiple, Laser, Misiles, Rayo
    }


    /// <summary>
    /// http://www.kongregate.com/games/CasualCollective/desktop-td-pro
    /// construir torres, crearlas, destruirlas y mejorarlas, incluye el menu de construccion
    /// </summary>
    class MenuTorres
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        //private const int MAXOPCIONESMENU = 5; //num de tipos de torres

        ////////////////imagen de la opcion de menu normal
        private Texture2D imagen; //incluye marcar opcion seleccionada y todas las opciones de menu
        private Point[] dimImagen; //ancho y alto de la imagen
        private Point[] posImagen;  //posicion inicial dentro del sprite
        private Vector2[] posicion; //posicion de la opcion de menu en la pantalla
        private eTipoTorre[] tipoTorre; //tipo de torre de cada opcion de menu

        ////////////////imagen opcion de menu seleccionada, rodeada de amarillo (es el primer sprite de "imagen")
        public Point dimImagenSel; //ancho y alto de la imagen
        private Point posImagenSel; //posicion inicial dentro del sprite

        ////////////////imagen torre seleccionada
        private Texture2D imagenTorreSel; //circulo sombreado de tamaño segun alcance torre
        //ancho y alto de la imagen
        public Point dimImagenTorreSel;
        //posicion inicial dentro del sprite
        private Point posImagenTorreSel;

        ////////////////imagen items (dinero, corazon, reloj, espada, radar, velocimetro)
        public Sprite spriteItems = new Sprite();
        //posicion inicial dentro del sprite
        private Point[] posImagenItems; 
        //color del texto del item
        private Color[] colorTextItems;

        ////////////////imagen nave tipo disparo, eItemArma (multiple, laser, misiles, rayo) y tambien en B/N  y seleccion arma
        public Sprite spriteItemsArma = new Sprite();
        //posicion inicial dentro del sprite
        private Point[] posImagenItemsArma;
        //posicion inicial dentro del sprite Blanco/Negro (armas no disponibles)
        private Point[] posImagenItemsArmaBN;
        //color del texto del item
        private Color[] colorTextItemsArma;
        //comprar arma o mejora de arma (icono dinero)
        private Rectangle[] botonComprarArma; //rectangulo con boton comprar o mejorar arma
        private bool[] bVisibleComprarArma; //boton comprar visible
        private bool[] bActivoArma; //comprar arma activo
        private bool[] pulsadaComprarArma; //almacena si pulsamos o no boton para ver si al soltar seguimos ahi
        //seleccionar arma (icono del arma)
        private Rectangle[] botonSelArma; //rectangulo con boton del arma
        //private bool[] bActivoSelArma; //boton activo
        private bool[] pulsadaSelArma; //almacena si pulsamos o no boton para ver si al soltar seguimos ahi

        ////////////////imagen arma seleccionada, rodeada de amarillo (es el 9º sprite de "spriteItemsArma")
        private Point dimImagenArmaSel; //ancho y alto de la imagen
        private Point posImagenArmaSel; //posicion inicial dentro del sprite
        private Vector2 restaPosArmaSel; //restar posicion para poner arma seleccioada


        //boton upgrade (mejorar)
        bool bBotonUpgrade; //boton visible
        private Sprite spriteUpgrade = new Sprite();
        private bool pulsadaMouseUpgrade = false; //almacena si pulsamos o no boton para ver si al soltar seguimos ahi
        //boton sell (vender)
        bool bBotonSell; //boton visible
        private Sprite spriteSell = new Sprite();
        private bool pulsadaMouseSell = false; //almacena si pulsamos o no boton para ver si al soltar seguimos ahi


        //limites de la ventana (usa la ventana total)
        private Viewport ventana;
        private Viewport ventanaGame; //para saber donde colocar las torres


        private int selectedOpcion = -1; //opcion de menu seleccionada (tipo de torre)
        private int selectedTorre = -1; //torre seleccionada para mejorarla o destruirla
        private int selectedEnemigo = -1; //enemigo seleccionado para ver su descripcion
        private int selectedNave = -1; //nave seleccionada para ver descripcion y mejorarla

        private int mouseOverOpcion = -1; //pasar el raton por encima de opcion de menu (se muestran detalles si no hay opcion seleccionada)

        private Point pulsadaMouseConstruir = NOPOINT; //almacena coord casillas a la q pulsamos para ver si al soltar seguimos ahi

        private static Point NOPOINT = new Point(-1, -1); //constante


        //posicion del apartado de descripcion de cada opcion de menu (comun a todos)
        private Vector2 posicionDescripcion = new Vector2(4, 300);
        //tamaño de los separadores en panel de descripcion
        private Vector2 sepDescVerTitulo = new Vector2(0, 40); //separacion vertical titulo
        private Vector2 sepDescVer = new Vector2(0, 60); //separacion vertical items
        private Vector2 sepDescHor = new Vector2(70, 10); //separacion horizontal texto
        private Vector2 sepDescHorUpgrade = new Vector2(140, 10); //separacion horizontal upgrade (absoluta)
        private Vector2 sepDescHorArma = new Vector2(60, 15); //separacion horizontal texto armas
        private Vector2 sepDescHorComprar = new Vector2(120, 0); //separacion horizontal comprar arma (absoluta)
        private Color colorDescTitulo = Color.Orange; //color del titulo de la descripcion
        //fuente para las descripciones
        private SpriteFont font;
        private SpriteFont fontArma;



        ////////////////// PUBLICAS //////////////////
        public int SelectedOpcion
        {
            get { return selectedOpcion; }
        }

        public int SelectedTorre
        {
            get { return selectedTorre; }
        }

        public int SelectedEnemigo
        {
            get { return selectedEnemigo; }
        }

        public int SelectedNave
        {
            get { return selectedNave; }
        }

        public int MaxOpcionesMenu
        {
            get { return Enum.GetValues(typeof(eTipoTorre)).Length; }
        }

        public eTipoDisparo TipoDisparoItemArma(eItemArma arma)
        {
            return (eTipoDisparo)(((int)arma) + 1); //me salto el disparo simple
        }


        ////////////////// EVENTOS //////////////////
        public event EventHandler<EventArgsCambiarTorreSelected> CambiarTorreSelected;
 

        ////////////////// CONSTRUCTOR //////////////////
        public MenuTorres(Viewport Ventana, Viewport VentanaGame, Tooltip tooltip)
        {
            this.ventana = Ventana;
            this.ventanaGame = VentanaGame;


            posicion = new Vector2[MaxOpcionesMenu];
            posImagen = new Point[MaxOpcionesMenu];
            dimImagen = new Point[MaxOpcionesMenu];
            tipoTorre = new eTipoTorre[MaxOpcionesMenu];

            //###TORRE###
            //posicion iconos de construir torres
            posicion[0] = new Vector2(20, 100); //20 + (50 + 15) = +65
            posicion[1] = new Vector2(85, 100);
            posicion[2] = new Vector2(150, 100);
            posicion[3] = new Vector2(20, 165);
            posicion[4] = new Vector2(85, 165);
            posicion[5] = new Vector2(150, 165);
            //...

            tipoTorre[0] = eTipoTorre.Bloque;
            tipoTorre[1] = eTipoTorre.Tanque;
            tipoTorre[2] = eTipoTorre.Triple;
            tipoTorre[3] = eTipoTorre.Canon;
            tipoTorre[4] = eTipoTorre.Hielo;
            tipoTorre[5] = eTipoTorre.AntiAire;
            //...


            //eventos
            this.CambiarTorreSelected += new EventHandler<EventArgsCambiarTorreSelected>(tooltip.CambiarTorreSelectedHandler);
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public Point PosImagenItems(eItems item)
        {
            return posImagenItems[(int)item];
        }

        public Point PosImagenItemsArma(eItemArma tipoArma)
        {
            return posImagenItemsArma[(int)tipoArma];
        }

        public Point PosImagenItemsArmaBN(eItemArma tipoArma)
        {
            return posImagenItemsArmaBN[(int)tipoArma];
        }

        public Color ColorTextItems(eItems item)
        {
            return colorTextItems[(int)item];
        }

        public Color ColorTextItemsArma(eItemArma tipoArma)
        {
            return colorTextItemsArma[(int)tipoArma];
        }


        //posicion de la opcion de menu seleccionada
        private Vector2 PosicionSel(int indice)
        {
            return new Vector2(posicion[indice].X - 10, posicion[indice].Y - 10);
        }


        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager content)
        {
            //fuente de la descripcion
            font = content.Load<SpriteFont>("Fuentes\\MenuTorres");

            //fuente de la descripcion de armas (mas pequeña)
            fontArma = content.Load<SpriteFont>("Fuentes\\MenuTorresArma");


            //sprite opciones de menu
            imagen = content.Load<Texture2D>("Menu\\menu_torre_01");
            //primer sprite es opcion seleccionada, rodeado de amarillo
            dimImagenSel = new Point(70, 70);
            posImagenSel = new Point(0, 0);
            //luego las opciones de menu
            for (int i = 0; i < MaxOpcionesMenu; i++)
            {
                dimImagen[i] = new Point(50, 50); //por ahora todas de tamaño fijo
                posImagen[i] = new Point(70 + i * 50, 0);
            }


            //imagen torre seleccionada (circulo gris de tamaño alcance torre)
            imagenTorreSel = content.Load<Texture2D>("Menu\\torre_selected_01");
            dimImagenTorreSel = new Point(200, 200);
            posImagenTorreSel = new Point(0, 0);


            //items (dinero, corazon, reloj, espada, radar, velocimetro)
            spriteItems.imagen = content.Load<Texture2D>("Menu\\items_01");
            spriteItems.dimImagen = new Point(50, 50);
            //posImagen de los items
            posImagenItems = new Point[Enum.GetValues(typeof(eItems)).Length];
            for (int i = 0; i < Enum.GetValues(typeof(eItems)).Length; i++)
            {
                posImagenItems[i] = new Point(i * 50, 0);
            }
            //color del texto de los items (dinero, vida, tiempo, ataque, alcance, velocidad)
            colorTextItems = new Color[Enum.GetValues(typeof(eItems)).Length];
            colorTextItems[0] = Color.Yellow;
            colorTextItems[1] = Color.Red;
            colorTextItems[2] = Color.LightGray;
            colorTextItems[3] = Color.SkyBlue;
            colorTextItems[4] = Color.Green;
            colorTextItems[5] = Color.Gray;


            //eItemArma (multiple, laser, misiles, rayo)
            spriteItemsArma.imagen = content.Load<Texture2D>("Menu\\icono_nave_disparos_01");
            spriteItemsArma.dimImagen = new Point(50, 50);
            //posImagen de los items
            posImagenItemsArma = new Point[Enum.GetValues(typeof(eItemArma)).Length];
            for (int i = 0; i < Enum.GetValues(typeof(eItemArma)).Length; i++)
            {
                posImagenItemsArma[i] = new Point(i * 50, 0);
            }
            //posImagen de los items en blanco y negro
            posImagenItemsArmaBN = new Point[Enum.GetValues(typeof(eItemArma)).Length];
            for (int i = 0; i < Enum.GetValues(typeof(eItemArma)).Length; i++)
            {
                posImagenItemsArmaBN[i] = new Point(Enum.GetValues(typeof(eItemArma)).Length * 50 + i * 50, 0);
            }
            //arma seleccionada
            dimImagenArmaSel = new Point(58, 58);
            posImagenArmaSel = new Point(50 * Enum.GetValues(typeof(eItemArma)).Length * 2, 0);
            restaPosArmaSel = new Vector2(4, 4);
            //color del texto de los items (multiple, laser, misiles, rayo)
            colorTextItemsArma = new Color[Enum.GetValues(typeof(eItemArma)).Length];
            colorTextItemsArma[0] = Color.Green;
            colorTextItemsArma[1] = Color.Orange;
            colorTextItemsArma[2] = Color.Violet;
            colorTextItemsArma[3] = Color.SkyBlue;

            //botones de comprar o mejorar arma
            botonComprarArma = new Rectangle[Enum.GetValues(typeof(eItemArma)).Length];
            bVisibleComprarArma = new bool[Enum.GetValues(typeof(eItemArma)).Length];
            bActivoArma = new bool[Enum.GetValues(typeof(eItemArma)).Length];
            pulsadaComprarArma = new bool[Enum.GetValues(typeof(eItemArma)).Length];

            //botones de seleccionar arma
            botonSelArma = new Rectangle[Enum.GetValues(typeof(eItemArma)).Length];
            //bActivoSelArma = new bool[Enum.GetValues(typeof(eItemArma)).Length];
            pulsadaSelArma = new bool[Enum.GetValues(typeof(eItemArma)).Length];


            //boton upgrade
            spriteUpgrade.imagen = content.Load<Texture2D>(Recursos.Boton_Upgrade);
            spriteUpgrade.dimImagen = new Point(100, 40);
            spriteUpgrade.posImagen = new Point(0, 0);
            //boton sell
            spriteSell.imagen = content.Load<Texture2D>(Recursos.Boton_Sell);
            spriteSell.dimImagen = new Point(100, 40);
            spriteSell.posImagen = new Point(0, 0);
        }





        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState previousKeyboardState, MouseState mouseState, MouseState previousMouseState, Tablero tablero, Torres torres, Enemigos enemigos, Nave nave, Tooltip tooltip, Opciones opciones)
        {
            Point posRaton = new Point(mouseState.X, mouseState.Y);
            Rectangle rectVentanaGame = new Rectangle((int)ventanaGame.X, (int)ventanaGame.Y, (int)ventanaGame.Width, (int)ventanaGame.Height);
            //pasar a posicion relativa (ventana game)
            Point posRatonRelativa = new Point(posRaton.X - ventanaGame.X, posRaton.Y - ventanaGame.Y);
            Rectangle rectangulo;


            //controlar si se pulsa en las opciones de menu
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                 ///////////// opciones de menu

                bool bPulsarOpcion = false;
                for (int i = 0; (i < MaxOpcionesMenu) && !bPulsarOpcion; i++)
                {
                    rectangulo = new Rectangle((int)posicion[i].X, (int)posicion[i].Y, dimImagen[i].X, dimImagen[i].Y);
                    if (rectangulo.Contains(posRaton))
                    {
                        selectedOpcion = i;
                        bPulsarOpcion = true;
                        pulsadaMouseConstruir = NOPOINT;
                        pulsadaMouseUpgrade = false;
                        pulsadaMouseSell = false;

                        selectedTorre = -1; //deseleccionar torre
                        selectedEnemigo = -1; //deseleccionar enemigo
                        selectedNave = -1; //deseccionar nave
                    }
                }


                //hay opcion seleccionada, pulsamos sobre tablero, no pulsamos sobre opcion, y es la primera pulsacion sin soltar 
                if (!bPulsarOpcion && (selectedOpcion != -1) && (pulsadaMouseConstruir == NOPOINT))
                {
                    if (rectVentanaGame.Contains(posRaton)) //pulsamos sobre tablero
                    {
                        ///////////// marcar raton para q al soltar se construya una torre

                        //convertir la posRaton en coordenadas del tablero
                        Point coord = tablero.PosicionToCoord(posRatonRelativa);

                        pulsadaMouseConstruir = coord; //se ha pulsado raton y hay q esperar q se suelte
                    }
                    else
                    {
                        //deseleccionar opcion
                        selectedOpcion = -1;
                        pulsadaMouseConstruir = NOPOINT;
                    }
                }


                //no pulsamos sobre opcion, hay torre seleccionada, hay boton, y es la primera pulsacion sin soltar 
                if (!bPulsarOpcion && (selectedTorre != -1) && bBotonUpgrade && !pulsadaMouseUpgrade)
                {
                    ///////////// marcar raton para q al soltar se mejore una torre (upgrade)

                    rectangulo = new Rectangle((int)tooltip.datos[(int)eTooltip.Mejorar].posicion.X, (int)tooltip.datos[(int)eTooltip.Mejorar].posicion.Y,
                                               spriteUpgrade.dimImagen.X, spriteUpgrade.dimImagen.Y);
                    if (rectangulo.Contains(posRaton)) //pulsamos sobre boton upgrade
                    {
                        pulsadaMouseUpgrade = true; //se ha pulsado raton y hay q esperar q se suelte
                    }
                }

                //no pulsamos sobre opcion, hay torre seleccionada, hay boton, y es la primera pulsacion sin soltar 
                if (!bPulsarOpcion && (selectedTorre != -1) && bBotonSell && !pulsadaMouseSell)
                {
                    ///////////// marcar raton para q al soltar se venda una torre (sell)

                    rectangulo = new Rectangle((int)tooltip.datos[(int)eTooltip.Vender].posicion.X, (int)tooltip.datos[(int)eTooltip.Vender].posicion.Y,
                                               spriteSell.dimImagen.X, spriteSell.dimImagen.Y);
                    if (rectangulo.Contains(posRaton)) //pulsamos sobre boton upgrade
                    {
                        pulsadaMouseSell = true; //se ha pulsado raton y hay q esperar q se suelte
                    }
                }


                //no pulsamos sobre opcion, hay nave seleccionada, hay boton, y es la primera pulsacion sin soltar 
                if (!bPulsarOpcion && (selectedNave != -1))
                {
                    for (int iArma = 0; iArma < Enum.GetValues(typeof(eItemArma)).Length; iArma++)
                    {
                        if (bVisibleComprarArma[(int)iArma] && !pulsadaComprarArma[(int)iArma])
                        {
                            ///////////// marcar raton para q al soltar se compre o mejore un arma

                            rectangulo = botonComprarArma[iArma];
                            if (rectangulo.Contains(posRaton)) //pulsamos sobre boton comprar arma
                            {
                                pulsadaComprarArma[(int)iArma] = true; //se ha pulsado raton y hay q esperar q se suelte
                            }
                        }
                    }
                }

                //no pulsamos sobre opcion, hay nave seleccionada, y es la primera pulsacion sin soltar 
                if (!bPulsarOpcion && (selectedNave != -1))
                {
                    for (int iArma = 0; iArma < Enum.GetValues(typeof(eItemArma)).Length; iArma++)
                    {
                        if (!pulsadaSelArma[(int)iArma])
                        {
                            ///////////// marcar raton para q al soltar se seleccione un arma

                            rectangulo = botonSelArma[iArma];
                            if (rectangulo.Contains(posRaton)) //pulsamos sobre boton seleccionar arma
                            {
                                pulsadaSelArma[(int)iArma] = true; //se ha pulsado raton y hay q esperar q se suelte
                            }
                        }
                    }
                }


                //pulsamos sobre el tablero y no estamos construyendo
                if (selectedOpcion == -1)
                {
                    if (rectVentanaGame.Contains(posRaton))
                    {
                        //dentro de ventanaGame, y no estamos construyendo

                        ///////////// seleccionar un enemigo

                        bool bSelEnemigo = false;

                        Rectangle rectEnemigo;
                        EnemigoBase enemigo;
                        for (int iEnemigo = 0; iEnemigo < enemigos.enemigos.Count; iEnemigo++)
                        {
                            enemigo = enemigos.enemigos[iEnemigo];

                            if (!enemigo.bEliminar)
                            {
                                Sprite sprite = EnemigoBase.GetSprite(enemigo.TipoEnemigo);
                                rectEnemigo = new Rectangle((int)(enemigo.posicion.X - sprite.center.X), (int)(enemigo.posicion.Y - sprite.center.Y),
                                                            sprite.dimImagen.X, sprite.dimImagen.Y);
                                if (rectEnemigo.Contains(posRatonRelativa))
                                {
                                    selectedEnemigo = iEnemigo;
                                    bSelEnemigo = true;
                                }
                            }
                        }

                        if (bSelEnemigo)
                        {
                            selectedTorre = -1; //deselecciona torre
                            selectedNave = -1; 
                            pulsadaMouseUpgrade = false;
                            pulsadaMouseSell = false;
                        }


                        ///////////// seleccionar una torre
                        bool bSelTorre = false;

                        if (!bSelEnemigo)
                        {
                            //no pulsamos sobre enemigo, dentro de ventanaGame, y no estamos construyendo

                            //convertir la posRaton en coordenadas del tablero
                            Point coord = tablero.PosicionToCoord(posRatonRelativa);

                            // pulso en coord x,y
                            // ver si bTablero vacio -> no seguir
                            // ver si tablero(x,y) es un torre -> seleccionar y no seguir
                            // ver si tablero (x-1, y) (x, y-1) (x-1, y-1) es una torre -> seleccionar y no seguir
                            // idem con tamaños mayores de torre 3 y luego 4, comprobando el tamaño de la torre encontrada

                            int x = coord.X;
                            int y = coord.Y;
                            if (tablero.bTablero[x, y] == Tablero.BTABLERO_TORRE)
                            {
                                if (tablero.tablero[x, y] != -1)
                                {
                                    selectedTorre = tablero.tablero[x, y];
                                    bSelTorre = true;
                                }
                                else
                                {
                                    for (int i = 1; (i < TorreBase.TAMAÑOMAXIMOTORRES.X) && (!bSelTorre); i++)
                                    {
                                        if (tablero.tablero[x - i, y] != -1)
                                        {
                                            selectedTorre = tablero.tablero[x - i, y];
                                            bSelTorre = true;
                                        }
                                        else if (tablero.tablero[x, y - i] != -1)
                                        {
                                            selectedTorre = tablero.tablero[x, y - i];
                                            bSelTorre = true;
                                        }
                                        else if (tablero.tablero[x - i, y - i] != -1)
                                        {
                                            selectedTorre = tablero.tablero[x - i, y - i];
                                            bSelTorre = true;
                                        }
                                    }
                                }
                            }

                            if (bSelTorre)
                            {
                                //evento tooltip
                                EventArgsCambiarTorreSelected eventArgs = new EventArgsCambiarTorreSelected(nave, torres, selectedTorre);
                                EventHandler<EventArgsCambiarTorreSelected> temp = CambiarTorreSelected;
                                if (temp != null)
                                    temp(this, eventArgs);

                                //CambiarTorreSelected(this, eventArgs); 

                                selectedEnemigo = -1; //deselecciona enemigo
                                selectedNave = -1;
                            }
                        }


                        ///////////// seleccionar la nave
                        bool bSelNave = false;

                        if (!bSelEnemigo && !bSelTorre)
                        {
                            Rectangle rectNave = new Rectangle((int)(nave.posicion.X - nave.sprite.center.X), (int)(nave.posicion.Y - nave.sprite.center.Y),
                                                               nave.sprite.dimImagen.X, nave.sprite.dimImagen.Y);
                            if (rectNave.Contains(posRatonRelativa))
                            {
                                bSelNave = true;
                                selectedNave = 1;
                                selectedEnemigo = -1;
                                selectedTorre = -1;
                            }
                        }


                        //si pulsas en tablero y no hay enemigo ni torre ni nave
                        if (!bSelEnemigo && !bSelTorre && !bSelNave)
                        {
                            //desseleccionar todo menos opcion menu

                            selectedEnemigo = -1;
                            selectedTorre = -1;
                            //selectedNave = -1; //quiero q se mantenga la nave seleccionada
                        }

                    }

                    else ///////////// pulsas fuera de tablero
                    {
                        selectedEnemigo = -1; //deselecciona enemigo

                        if ((!pulsadaMouseUpgrade) && (!pulsadaMouseSell))
                        {
                            selectedTorre = -1; //deselecciona torre
                            pulsadaMouseUpgrade = false;
                            pulsadaMouseSell = false;
                        }

                        bool bPulsadaArma = false; //se pulsa en algun boton
                        for (int iArma = 0; iArma < Enum.GetValues(typeof(eItemArma)).Length; iArma++)
                        {
                            if (pulsadaComprarArma[iArma])
                                bPulsadaArma = true;
                            if (pulsadaSelArma[iArma])
                                bPulsadaArma = true;
                        }
                        if (!bPulsadaArma)
                        {
                            selectedNave = -1; //deselecciona nave
                            for (int iArma = 0; iArma < Enum.GetValues(typeof(eItemArma)).Length; iArma++)
                            {
                                pulsadaComprarArma[iArma] = false;
                                pulsadaSelArma[iArma] = false;
                            }
                        }
                    }
                }
            }


            else //si se suelta el boton del raton
            {
                //se pulso anteriormente en construir torre y hay opcion seleccionada
                if ((selectedOpcion != -1) && (!pulsadaMouseConstruir.Equals(NOPOINT)))
                {
                    ///////////// construir torre

                    //convertir la posRaton en coordenadas del tablero
                    Point coord = tablero.PosicionToCoord(posRatonRelativa);

                    //!!!!!!!!!! quitar esta comprobacion y ver si cuando hay lag las torres se construyen siempre sin fallo
                    if (pulsadaMouseConstruir.Equals(coord)) //estamos en esas coordenadas
                    {
                        int iOK = tablero.AddTorre(gameTime, coord, tipoTorre[selectedOpcion], torres, enemigos, nave);

                        if (iOK != 0)
                        {
                            Sonidos.PlayFX(eSonido.BeepError, opciones);
                        }
                    }
                    pulsadaMouseConstruir = NOPOINT;
                }


                //se pulso anteriormente en mejorar torre y hay torre seleccionada
                if ((selectedTorre != -1) && (pulsadaMouseUpgrade))
                {
                    ///////////// mejorar torre (upgrade)

                    rectangulo = new Rectangle((int)tooltip.datos[(int)eTooltip.Mejorar].posicion.X, (int)tooltip.datos[(int)eTooltip.Mejorar].posicion.Y,
                                               spriteUpgrade.dimImagen.X, spriteUpgrade.dimImagen.Y);
                    if (rectangulo.Contains(posRaton)) //soltamos sobre boton upgrade
                    {
                        if (torres.torres[selectedTorre].Upgrade(nave))
                        {
                            //evento tooltip
                            EventArgsCambiarTorreSelected eventArgs = new EventArgsCambiarTorreSelected(nave, torres, selectedTorre);
                            EventHandler<EventArgsCambiarTorreSelected> temp = CambiarTorreSelected;
                            if (temp != null)
                                temp(this, eventArgs);
                        }
                        else
                        {
                            Sonidos.PlayFX(eSonido.BeepError, opciones); //no se ha podido hacer upgrade
                        }
                    }
                    pulsadaMouseUpgrade = false;
                }

                //se pulso anteriormente en vender torre y hay torre seleccionada
                if ((selectedTorre != -1) && (pulsadaMouseSell))
                {
                    ///////////// vender torre (sell)

                    rectangulo = new Rectangle((int)tooltip.datos[(int)eTooltip.Vender].posicion.X, (int)tooltip.datos[(int)eTooltip.Vender].posicion.Y,
                                               spriteSell.dimImagen.X, spriteSell.dimImagen.Y);
                    if (rectangulo.Contains(posRaton)) //soltamos sobre boton sell
                    {
                        //vender torre
                        if(torres.VenderTorre(gameTime, selectedTorre))
                            selectedTorre = -1;
                        else
                            Sonidos.PlayFX(eSonido.BeepError, opciones); //no se ha podido hacer venta

                    }
                    pulsadaMouseSell = false;
                }


                //se pulso anteriormente en comprar o mejorar arma y hay nave seleccionada
                if ((selectedNave != -1) )
                {
                    for (int iArma = 0; iArma < Enum.GetValues(typeof(eItemArma)).Length; iArma++)
                    {
                        if (pulsadaComprarArma[iArma])
                        {
                            ///////////// comprar o mejorar arma

                            rectangulo = botonComprarArma[iArma];
                            if (rectangulo.Contains(posRaton)) //soltamos sobre boton comprar arma
                            {
                                if (!nave.disparos.ComprarMejorarArma(nave, TipoDisparoItemArma((eItemArma)iArma)))
                                {
                                    Sonidos.PlayFX(eSonido.BeepError, opciones); //no se ha podido hacer compra arma
                                }
                            }
                            pulsadaComprarArma[iArma] = false;
                        }
                    }
                }

                //se pulso anteriormente en seleccionar arma y hay nave seleccionada
                if ((selectedNave != -1))
                {
                    for (int iArma = 0; iArma < Enum.GetValues(typeof(eItemArma)).Length; iArma++)
                    {
                        if (pulsadaSelArma[iArma])
                        {
                            ///////////// seleccionar arma

                            rectangulo = botonSelArma[iArma];
                            if (rectangulo.Contains(posRaton)) //soltamos sobre boton seleccionar arma
                            {
                                if (!nave.disparos.SeleccionarArma(nave, TipoDisparoItemArma((eItemArma)iArma), opciones))
                                {
                                    Sonidos.PlayFX(eSonido.BeepError, opciones); //no se ha podido hacer seleccionar arma
                                }
                            }
                            pulsadaSelArma[iArma] = false;
                        }
                    }
                }


                //no hay opcion de menu, ni torre ni enemigo seleccionado y se pasa el raton por encima de opcion de menu
                if ((selectedOpcion == -1) && (selectedTorre == -1) && (selectedEnemigo == -1) && (selectedNave == -1))
                {
                    bool bOver = false;
                    for (int i = 0; (i < MaxOpcionesMenu) && !bOver; i++)
                    {
                        rectangulo = new Rectangle((int)posicion[i].X, (int)posicion[i].Y, dimImagen[i].X, dimImagen[i].Y);
                        if (rectangulo.Contains(posRaton))
                        {
                            bOver = true;
                            mouseOverOpcion = i;
                        }
                    }
                    if(!bOver)
                        mouseOverOpcion = -1;
                }
                else
                {
                    mouseOverOpcion = -1;
                }

            }


            /*if (selectedTorre != -1) //hay torre seleccionada 
            {
                ///////////// destruir torre seleccionada (deberia venderse solamente)

                if (keyboardState.IsKeyDown(Keys.Delete) || keyboardState.IsKeyDown(Keys.Back)) //tecla delete o suprimir
                {
                    tablero.DeleteTorre(gameTime, selectedTorre, torres, enemigos);
                    selectedTorre = -1;
                }
            }*/

            if(selectedTorre != -1) //hay torre seleccionada 
            {
                if (keyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.MejorarTorre)) && previousKeyboardState.IsKeyUp(opciones.GetTeclas(eTeclas.MejorarTorre)))
                {
                    ///////////// mejorar torre
                    if (torres.torres[selectedTorre].Upgrade(nave))
                    {
                        //evento tooltip
                        EventArgsCambiarTorreSelected eventArgs = new EventArgsCambiarTorreSelected(nave, torres, selectedTorre);
                        EventHandler<EventArgsCambiarTorreSelected> temp = CambiarTorreSelected;
                        if (temp != null)
                            temp(this, eventArgs);
                    }
                    else
                    {
                        Sonidos.PlayFX(eSonido.BeepError, opciones); //no se ha podido hacer upgrade
                    }
                }

                if (keyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.VenderTorre)) && previousKeyboardState.IsKeyUp(opciones.GetTeclas(eTeclas.VenderTorre)))
                {
                    ///////////// vender torre
                    if(torres.VenderTorre(gameTime, selectedTorre))
                        selectedTorre = -1;
                    else
                        Sonidos.PlayFX(eSonido.BeepError, opciones); //no se ha podido hacer venta
                }
            }


            if (selectedTorre != -1) //hay torre seleccionada 
            {
                ///////////// botones upgrade y sell
                bBotonSell = true;

                CaracteristicasTorre caractUpgrade = torres.torres[selectedTorre].GetCaracteristicas(torres.torres[selectedTorre].Nivel + 1);
                if (caractUpgrade.precio != -1)
                    bBotonUpgrade = true;
                else
                    bBotonUpgrade = false;
            }
            else
            {
                bBotonSell = false;
                bBotonUpgrade = false;
            }


            if (selectedEnemigo != -1) //hay enemigo selecionado
            {
                if (enemigos.enemigos[selectedEnemigo].bEliminar) //si se muere deselecionarlo
                    selectedEnemigo = -1;
            }


            if (selectedNave != -1) //hay nave seleccionada
            {
                ///////////// botones comprar o mejorar arma
                
                //lo tengo en tooltip, un poco mas abajo

            }
            else
            {
                for (int iArma = 0; iArma < Enum.GetValues(typeof(eItemArma)).Length; iArma++)
                {
                    bVisibleComprarArma[iArma] = false;
                }
            }


            ///////////// tooltip
            tooltip.BorrarPosicion();

            //posicion de iconos y botones para tooltip
            Vector2 posDesc = posicionDescripcion;

            if ((selectedOpcion != -1) || (mouseOverOpcion != -1))
            {
                //dinero (precio)
                posDesc += sepDescVerTitulo;
                tooltip.datos[(int)eTooltip.Precio].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                       spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //ataque (daño)
                posDesc += sepDescVer;
                tooltip.datos[(int)eTooltip.Ataque_Torre].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                             spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //alcance
                posDesc += sepDescVer;
                tooltip.datos[(int)eTooltip.Alcance].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                        spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //velocidad (ratio)
                posDesc += sepDescVer;
                tooltip.datos[(int)eTooltip.Ratio].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                      spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
            }

            if (selectedTorre != -1)
            {
                //dinero (precio)
                posDesc += sepDescVerTitulo;
                tooltip.datos[(int)eTooltip.Precio].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                       spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //ataque (daño)
                posDesc += sepDescVer;
                tooltip.datos[(int)eTooltip.Ataque_Torre].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                             spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //alcance
                posDesc += sepDescVer;
                tooltip.datos[(int)eTooltip.Alcance].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                        spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //velocidad (ratio)
                posDesc += sepDescVer;
                tooltip.datos[(int)eTooltip.Ratio].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                      spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
            }

            if (bBotonUpgrade)
            {
                tooltip.datos[(int)eTooltip.Mejorar].posicion = new Rectangle(3, 600, spriteUpgrade.dimImagen.X, spriteUpgrade.dimImagen.Y);
                //posicionBotonUpgrade = new Vector2(3, 600);
            }

            if (bBotonSell)
            {
                tooltip.datos[(int)eTooltip.Vender].posicion = new Rectangle(110, 600, spriteSell.dimImagen.X, spriteSell.dimImagen.Y);
                //posicionBotonSell = new Vector2(110, 600);
            }

            if (selectedEnemigo != -1)
            {
                //dinero (precio)
                posDesc += sepDescVerTitulo;
                tooltip.datos[(int)eTooltip.Precio_Enemigo].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                               spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //ataque (dañoFinal y daño)
                posDesc += sepDescVer;
                tooltip.datos[(int)eTooltip.Ataque_Enemigo].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                               spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //vida y maxVida
                posDesc += sepDescVer;
                tooltip.datos[(int)eTooltip.Vida].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                     spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //velocidad
                posDesc += sepDescVer;
                tooltip.datos[(int)eTooltip.Velocidad].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                          spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
            }

            if (selectedNave != -1)
            {
                //multiple
                posDesc += sepDescVerTitulo;
                eTipoDisparo tipoDisparo = eTipoDisparo.Multiple;
                eItemArma arma = eItemArma.Multiple;
                bActivoArma[(int)arma] = nave.disparos.ArmaActiva(tipoDisparo);
                if (bActivoArma[(int)arma])
                    tooltip.datos[(int)eTooltip.Multiple].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                               spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                else
                    tooltip.datos[(int)eTooltip.Inactivo_Multiple].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                               spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                botonSelArma[(int)arma] = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                        spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //comprar
                bool bComprar = nave.disparos.PosibleComprarArma(tipoDisparo);
                bVisibleComprarArma[(int)arma] = bComprar;
                if (bComprar)
                {
                    if (bActivoArma[(int)arma])
                        tooltip.datos[(int)eTooltip.Mejorar_Multiple].posicion = new Rectangle((int)(posDesc + sepDescHorComprar).X, (int)(posDesc + sepDescHorComprar).Y,
                                                                                 spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                    else
                        tooltip.datos[(int)eTooltip.Comprar_Multiple].posicion = new Rectangle((int)(posDesc + sepDescHorComprar).X, (int)(posDesc + sepDescHorComprar).Y,
                                                                             spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                    botonComprarArma[(int)arma] = new Rectangle((int)(posDesc + sepDescHorComprar).X, (int)(posDesc + sepDescHorComprar).Y,
                                                                             spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                }

                //laser
                posDesc += sepDescVer;
                tipoDisparo = eTipoDisparo.Laser;
                arma = eItemArma.Laser;
                bActivoArma[(int)arma] = nave.disparos.ArmaActiva(tipoDisparo);
                if (bActivoArma[(int)arma])
                    tooltip.datos[(int)eTooltip.Laser].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                               spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                else
                    tooltip.datos[(int)eTooltip.Inactivo_Laser].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                                   spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                botonSelArma[(int)arma] = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                        spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //comprar
                bComprar = nave.disparos.PosibleComprarArma(tipoDisparo);
                bVisibleComprarArma[(int)arma] = bComprar;
                if (bComprar)
                {
                    if (bActivoArma[(int)arma])
                        tooltip.datos[(int)eTooltip.Mejorar_Laser].posicion = new Rectangle((int)(posDesc + sepDescHorComprar).X, (int)(posDesc + sepDescHorComprar).Y,
                                                                             spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                    else
                        tooltip.datos[(int)eTooltip.Comprar_Laser].posicion = new Rectangle((int)(posDesc + sepDescHorComprar).X, (int)(posDesc + sepDescHorComprar).Y,
                                                                             spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                    botonComprarArma[(int)arma] = new Rectangle((int)(posDesc + sepDescHorComprar).X, (int)(posDesc + sepDescHorComprar).Y,
                                                                             spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                }

                //misiles
                posDesc += sepDescVer;
                tipoDisparo = eTipoDisparo.Misiles;
                arma = eItemArma.Misiles;
                bActivoArma[(int)arma] = nave.disparos.ArmaActiva(tipoDisparo);
                if (bActivoArma[(int)arma])
                    tooltip.datos[(int)eTooltip.Misiles].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                     spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                else
                    tooltip.datos[(int)eTooltip.Inactivo_Misiles].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                     spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                botonSelArma[(int)arma] = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                        spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //comprar
                bComprar = nave.disparos.PosibleComprarArma(tipoDisparo);
                bVisibleComprarArma[(int)arma] = bComprar;
                if (bComprar)
                {
                    if (bActivoArma[(int)arma])
                        tooltip.datos[(int)eTooltip.Mejorar_Misiles].posicion = new Rectangle((int)(posDesc + sepDescHorComprar).X, (int)(posDesc + sepDescHorComprar).Y,
                                                                                 spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                    else
                        tooltip.datos[(int)eTooltip.Comprar_Misiles].posicion = new Rectangle((int)(posDesc + sepDescHorComprar).X, (int)(posDesc + sepDescHorComprar).Y,
                                                                             spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                    botonComprarArma[(int)arma] = new Rectangle((int)(posDesc + sepDescHorComprar).X, (int)(posDesc + sepDescHorComprar).Y,
                                                                             spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                }

                //rayo
                posDesc += sepDescVer;
                tipoDisparo = eTipoDisparo.Rayo;
                arma = eItemArma.Rayo;
                bActivoArma[(int)arma] = nave.disparos.ArmaActiva(tipoDisparo);
                if (bActivoArma[(int)arma])
                    tooltip.datos[(int)eTooltip.Rayo].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                          spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                else
                    tooltip.datos[(int)eTooltip.Inactivo_Rayo].posicion = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                                          spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                botonSelArma[(int)arma] = new Rectangle((int)posDesc.X, (int)posDesc.Y,
                                                        spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                //comprar
                bComprar = nave.disparos.PosibleComprarArma(tipoDisparo);
                bVisibleComprarArma[(int)arma] = bComprar;
                if (bComprar)
                {
                    if (bActivoArma[(int)arma])
                        tooltip.datos[(int)eTooltip.Mejorar_Rayo].posicion = new Rectangle((int)(posDesc + sepDescHorComprar).X, (int)(posDesc + sepDescHorComprar).Y,
                                                                             spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                    else
                        tooltip.datos[(int)eTooltip.Comprar_Rayo].posicion = new Rectangle((int)(posDesc + sepDescHorComprar).X, (int)(posDesc + sepDescHorComprar).Y,
                                                                             spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                    botonComprarArma[(int)arma] = new Rectangle((int)(posDesc + sepDescHorComprar).X, (int)(posDesc + sepDescHorComprar).Y,
                                                                             spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                }
            }

        }





        ////////////////// DRAW //////////////////

        public void DrawTorreSel(SpriteBatch spb, Tablero tablero, Torres torres)
        {
            if (selectedTorre != -1)
            {
                TorreBase torre = torres.torres[selectedTorre];

                //el dibujo de la torre sombreada con su alcance lo hago aqui para q respete ventanaGame

                //posic relativa de la torre en ventana game
                Vector2 posTorreRel = tablero.CoordToPosicion(torre.coordenadas);
                //posicion relativa centro torre
                Vector2 posCentroTorre = posTorreRel + TorreBase.CentroTorre(torre.TipoTorre, tablero);
                //poner posicion absoluta de ventana total
                //Vector2 posTorre = new Vector2(posCentroTorre.X + ventanaGame.X, posCentroTorre.Y + ventanaGame.Y);

                float alpha = 0.5f;
                float radio = torre.Alcance;
                if (radio == 0) //torre sin alcance, no ataca
                {
                    radio = (float)Math.Sqrt(Math.Pow(TorreBase.GetSprite(torre.TipoTorre).dimImagen.X / 2, 2) * 2); //hipo2 = lado2 + lado2
                    //alpha = 0.25f;
                }
                float scale = (radio * 2) / dimImagenTorreSel.X; //diametro 160 / 200 = 0.8f

                Vector2 posicion = new Vector2(posCentroTorre.X - radio, posCentroTorre.Y - radio); //centro torre relativo
                Rectangle rectangulo = new Rectangle(posImagenTorreSel.X, posImagenTorreSel.Y,
                                           dimImagenTorreSel.X, dimImagenTorreSel.Y);
                Vector2 posImagen = new Vector2(posImagenTorreSel.X, posImagenTorreSel.Y);

                spb.Draw(imagenTorreSel, posicion, rectangulo, Color.Yellow * alpha, 0f, posImagen, scale, SpriteEffects.None, 0);
            }
        }


        private void DibujaDescOpcionMenu(SpriteBatch spb, int selected, Color colorDescTitulo, Vector2 sepDescVerTitulo,
                                          Vector2 sepDescVer, Vector2 sepDescHor, Tooltip tooltip)
        {
            //descripcion - detalles opcion de menu
            Vector2 posDesc = posicionDescripcion;
            string sDesc = Recursos.Torre + " " + TorreBase.TipoTorreToString(tipoTorre[selected]);
            spb.DrawString(font, sDesc, posDesc, colorDescTitulo);
            spb.DrawString(font, sDesc, posDesc + new Vector2(1, 1), Color.White);

            CaracteristicasTorre caract = TorreBase.GetCaracteristicasOpcionTorre(tipoTorre[selected]);

            //dinero (precio)
            posDesc = tooltip.VectorPosicion(eTooltip.Precio);
            Rectangle rectangulo = new Rectangle(PosImagenItems(eItems.Dinero).X, PosImagenItems(eItems.Dinero).Y,
                                       spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
            spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);
            spb.DrawString(font, caract.precio.ToString(), posDesc + sepDescHor, ColorTextItems(eItems.Dinero));
            spb.DrawString(font, caract.precio.ToString(), posDesc + sepDescHor + new Vector2(1, 1), Color.White);

            //ataque (daño)
            posDesc = tooltip.VectorPosicion(eTooltip.Ataque_Torre);
            rectangulo = new Rectangle(PosImagenItems(eItems.Ataque).X, PosImagenItems(eItems.Ataque).Y,
                                       spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
            spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);
            spb.DrawString(font, caract.daño.ToString(), posDesc + sepDescHor, ColorTextItems(eItems.Ataque));
            spb.DrawString(font, caract.daño.ToString(), posDesc + sepDescHor + new Vector2(1, 1), Color.White);

            //alcance
            posDesc = tooltip.VectorPosicion(eTooltip.Alcance);
            rectangulo = new Rectangle(PosImagenItems(eItems.Alcance).X, PosImagenItems(eItems.Alcance).Y,
                                       spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
            spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);
            spb.DrawString(font, caract.MostrarAlcance.ToString(), posDesc + sepDescHor, ColorTextItems(eItems.Alcance));
            spb.DrawString(font, caract.MostrarAlcance.ToString(), posDesc + sepDescHor + new Vector2(1, 1), Color.White);

            //velocidad (ratio)
            posDesc = tooltip.VectorPosicion(eTooltip.Ratio);
            rectangulo = new Rectangle(PosImagenItems(eItems.Velocidad).X, PosImagenItems(eItems.Velocidad).Y,
                                       spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
            spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);
            spb.DrawString(font, caract.MostrarRatio.ToString(), posDesc + sepDescHor, ColorTextItems(eItems.Velocidad));
            spb.DrawString(font, caract.MostrarRatio.ToString(), posDesc + sepDescHor + new Vector2(1, 1), Color.White);
        }


        public void Draw(SpriteBatch spb, MouseState mouseState, Tablero tablero, Nave nave, Torres torres, Enemigos enemigos, Tooltip tooltip)
        {
            Rectangle rectangulo;
            Vector2 posDesc; //posicion descripcion


            //poner opciones de menu
            for (int i = 0; i < MaxOpcionesMenu; i++)
            {
                rectangulo = new Rectangle(posImagen[i].X, posImagen[i].Y,
                                           dimImagen[i].X, dimImagen[i].Y);
                spb.Draw(imagen, posicion[i], rectangulo, Color.White);

                if (selectedOpcion == i)
                {
                    //resaltamos opcion seleccionada
                    rectangulo = new Rectangle(posImagenSel.X, posImagenSel.Y,
                                               dimImagenSel.X, dimImagenSel.Y);
                    spb.Draw(imagen, PosicionSel(i), rectangulo, Color.White);
                }
            }


            //si hay opcion seleccionada hay q poner la torre en el tablero sobre el cursor del raton (sombreada)
            if (selectedOpcion != -1)
            {
                //dibuja torre sombreada donde se pueda construir
                Point posRaton = new Point(mouseState.X, mouseState.Y);

                if (ventanaGame.Bounds.Contains(posRaton)) //si estamos dentro del tablero
                {
                    //pasar a posicion relativa (ventana game)
                    Point posRatonRelativa = new Point(posRaton.X - ventanaGame.X, posRaton.Y - ventanaGame.Y);

                    //queremos q se dibuje donde se pondra realmente la torre teniendo en cuenta la cuadricula
                    Point coord = tablero.PosicionToCoord(posRatonRelativa);
                    Vector2 posRelativa = tablero.CoordToPosicion(coord);

                    //pasar a posicion absoluta (ventana total)
                    Vector2 posTorre = new Vector2(posRelativa.X + ventanaGame.X, posRelativa.Y + ventanaGame.Y);

                    rectangulo = new Rectangle(posImagen[selectedOpcion].X, posImagen[selectedOpcion].Y,
                                               dimImagen[selectedOpcion].X, dimImagen[selectedOpcion].Y);

                    float alpha = 0.5f;
                    spb.Draw(imagen, posTorre, rectangulo, Color.White * alpha);
                }

                //descripcion - detalles opcion de menu
                DibujaDescOpcionMenu(spb, selectedOpcion, colorDescTitulo, sepDescVerTitulo, sepDescVer, sepDescHor, tooltip);
            }


            //si se pasa el raton por encima de opcion de menu
            if (mouseOverOpcion != -1)
            {
                //descripcion - detalles opcion de menu
                DibujaDescOpcionMenu(spb, mouseOverOpcion, colorDescTitulo, sepDescVerTitulo, sepDescVer, sepDescHor, tooltip);
            }


            //si hay torre seleccionada resaltarla, poner circulo con su alcance
            if (selectedTorre != -1)
            {
                TorreBase torre = torres.torres[selectedTorre];


                //el dibujo de la torre sombreada con su alcance lo hago fuera de aqui para q respete ventanaGame (no se salga del borde)
                //DrawTorreSel


                //descripcion - detalles torre
                posDesc = posicionDescripcion;
                string sDesc = Recursos.Torre + " " + TorreBase.TipoTorreToString(torre.TipoTorre) +
                               " " + Recursos.nivel + " " + torre.Nivel.ToString();
                spb.DrawString(font, sDesc, posDesc, colorDescTitulo);
                spb.DrawString(font, sDesc, posDesc + new Vector2(1, 1), Color.White);

                //caracteristicas si se mejora la torre (+X)
                bool bUpgrade = false;
                CaracteristicasTorre caractActual = torre.GetCaracteristicas(torre.Nivel);
                CaracteristicasTorre caractUpgrade = torre.GetCaracteristicas(torre.Nivel + 1);
                CaracteristicasTorre caract = new CaracteristicasTorre();
                if (caractUpgrade.precio != -1)
                {
                    bUpgrade = true;
                    caract.alcance = caractUpgrade.alcance - caractActual.alcance;
                    caract.daño = caractUpgrade.daño - caractActual.daño;
                    caract.ratio = caractUpgrade.ratio - caractActual.ratio;
                    caract.precio = caractUpgrade.precio - caractActual.precio;
                }

                //dinero (precio)
                posDesc = tooltip.VectorPosicion(eTooltip.Precio);
                rectangulo = new Rectangle(PosImagenItems(eItems.Dinero).X, PosImagenItems(eItems.Dinero).Y,
                                           spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);
                spb.DrawString(font, torre.Precio.ToString(), posDesc + sepDescHor, ColorTextItems(eItems.Dinero));
                spb.DrawString(font, torre.Precio.ToString(), posDesc + sepDescHor + new Vector2(1, 1), Color.White);

                if (bUpgrade)
                {
                    spb.DrawString(font, "+" + caract.precio.ToString(), posDesc + sepDescHorUpgrade, ColorTextItems(eItems.Dinero));
                    spb.DrawString(font, "+" + caract.precio.ToString(), posDesc + sepDescHorUpgrade + new Vector2(1, 1), Color.White);
                }

                //ataque (daño)
                posDesc = tooltip.VectorPosicion(eTooltip.Ataque_Torre);
                rectangulo = new Rectangle(PosImagenItems(eItems.Ataque).X, PosImagenItems(eItems.Ataque).Y,
                           spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);
                spb.DrawString(font, torre.Daño.ToString(), posDesc + sepDescHor, ColorTextItems(eItems.Ataque));
                spb.DrawString(font, torre.Daño.ToString(), posDesc + sepDescHor + new Vector2(1, 1), Color.White);

                if (bUpgrade)
                {
                    spb.DrawString(font, "+" + caract.daño.ToString(), posDesc + sepDescHorUpgrade, ColorTextItems(eItems.Ataque));
                    spb.DrawString(font, "+" + caract.daño.ToString(), posDesc + sepDescHorUpgrade + new Vector2(1, 1), Color.White);
                }

                //alcance
                posDesc = tooltip.VectorPosicion(eTooltip.Alcance);
                rectangulo = new Rectangle(PosImagenItems(eItems.Alcance).X, PosImagenItems(eItems.Alcance).Y,
                                           spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);
                spb.DrawString(font, torre.MostrarAlcance.ToString(), posDesc + sepDescHor, ColorTextItems(eItems.Alcance));
                spb.DrawString(font, torre.MostrarAlcance.ToString(), posDesc + sepDescHor + new Vector2(1, 1), Color.White);

                if (bUpgrade)
                {
                    spb.DrawString(font, "+" + caract.MostrarAlcance.ToString(), posDesc + sepDescHorUpgrade, ColorTextItems(eItems.Alcance));
                    spb.DrawString(font, "+" + caract.MostrarAlcance.ToString(), posDesc + sepDescHorUpgrade + new Vector2(1, 1), Color.White);
                }

                //velocidad (ratio)
                posDesc = tooltip.VectorPosicion(eTooltip.Ratio);
                rectangulo = new Rectangle(PosImagenItems(eItems.Velocidad).X, PosImagenItems(eItems.Velocidad).Y,
                           spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);
                spb.DrawString(font, torre.MostrarRatio.ToString(), posDesc + sepDescHor, ColorTextItems(eItems.Velocidad));
                spb.DrawString(font, torre.MostrarRatio.ToString(), posDesc + sepDescHor + new Vector2(1, 1), Color.White);

                if (bUpgrade)
                {
                    spb.DrawString(font, "+" + caract.MostrarRatio.ToString(), posDesc + sepDescHorUpgrade, ColorTextItems(eItems.Velocidad));
                    spb.DrawString(font, "+" + caract.MostrarRatio.ToString(), posDesc + sepDescHorUpgrade + new Vector2(1, 1), Color.White);
                }
            }


            //boton upgrade y sell
            if (bBotonUpgrade)
            {
                rectangulo = new Rectangle(spriteUpgrade.posImagen.X, spriteUpgrade.posImagen.Y,
                                           spriteUpgrade.dimImagen.X, spriteUpgrade.dimImagen.Y);
                spb.Draw(spriteUpgrade.imagen, tooltip.VectorPosicion(eTooltip.Mejorar), rectangulo, Color.White);
            }

            if (bBotonSell)
            {
                rectangulo = new Rectangle(spriteSell.posImagen.X, spriteSell.posImagen.Y,
                                           spriteSell.dimImagen.X, spriteUpgrade.dimImagen.Y);
                spb.Draw(spriteSell.imagen, tooltip.VectorPosicion(eTooltip.Vender), rectangulo, Color.White);
            }


            //si hay enemigo seleccionado poner descripcion
            if (selectedEnemigo != -1)
            {
                EnemigoBase enemigo = enemigos.enemigos[selectedEnemigo];

                //descripcion - detalles enemigo
                posDesc = posicionDescripcion;
                string sNombre = EnemigoBase.TipoEnemigoToString(enemigo.TipoEnemigo);
                if (sNombre.Length > 12)
                    sNombre = sNombre.Substring(0, 12);
                string sDesc = sNombre + " " + Recursos.nivel + " " + enemigo.Nivel.ToString();
                spb.DrawString(font, sDesc, posDesc, colorDescTitulo);
                spb.DrawString(font, sDesc, posDesc + new Vector2(1, 1), Color.White);
                
                //dinero (precio)
                posDesc = tooltip.VectorPosicion(eTooltip.Precio_Enemigo);
                rectangulo = new Rectangle(PosImagenItems(eItems.Dinero).X, PosImagenItems(eItems.Dinero).Y,
                                           spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);
                spb.DrawString(font, enemigo.Dinero.ToString(), posDesc + sepDescHor, ColorTextItems(eItems.Dinero));
                spb.DrawString(font, enemigo.Dinero.ToString(), posDesc + sepDescHor + new Vector2(1, 1), Color.White);

                //ataque (dañoFinal y daño)
                posDesc = tooltip.VectorPosicion(eTooltip.Ataque_Enemigo);
                rectangulo = new Rectangle(PosImagenItems(eItems.Ataque).X, PosImagenItems(eItems.Ataque).Y,
                           spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);
                spb.DrawString(font, enemigo.DañoFinal.ToString(), posDesc + sepDescHor, ColorTextItems(eItems.Ataque));
                spb.DrawString(font, enemigo.DañoFinal.ToString(), posDesc + sepDescHor + new Vector2(1, 1), Color.White);

                spb.DrawString(font, "(" + enemigo.Daño.ToString() + ")", posDesc + sepDescHorUpgrade, ColorTextItems(eItems.Ataque));
                spb.DrawString(font, "(" + enemigo.Daño.ToString() + ")", posDesc + sepDescHorUpgrade + new Vector2(1, 1), Color.White);

                //vida y maxVida
                posDesc = tooltip.VectorPosicion(eTooltip.Vida);
                rectangulo = new Rectangle(PosImagenItems(eItems.Vida).X, PosImagenItems(eItems.Vida).Y,
                                           spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);
                spb.DrawString(font, enemigo.Vida.ToString(), posDesc + sepDescHor, ColorTextItems(eItems.Vida));
                spb.DrawString(font, enemigo.Vida.ToString(), posDesc + sepDescHor + new Vector2(1, 1), Color.White);

                spb.DrawString(font, "(" + enemigo.MaxVida.ToString() + ")", posDesc + sepDescHorUpgrade, ColorTextItems(eItems.Vida));
                spb.DrawString(font, "(" + enemigo.MaxVida.ToString() + ")", posDesc + sepDescHorUpgrade + new Vector2(1, 1), Color.White);

                //velocidad
                posDesc = tooltip.VectorPosicion(eTooltip.Velocidad);
                rectangulo = new Rectangle(PosImagenItems(eItems.Velocidad).X, PosImagenItems(eItems.Velocidad).Y,
                           spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);
                spb.DrawString(font, enemigo.MostrarVelocidad.ToString(), posDesc + sepDescHor, ColorTextItems(eItems.Velocidad));
                spb.DrawString(font, enemigo.MostrarVelocidad.ToString(), posDesc + sepDescHor + new Vector2(1, 1), Color.White);
            }


            //si hay nave seleccionada resaltarla, poner descripcion y botones
            if (selectedNave != -1)
            {
                //descripcion - detalles nave
                posDesc = posicionDescripcion;
                string sDesc = Recursos.Nave + " " + Recursos.disparo + " " + Disparos.DisparoToString(nave.TipoDisparo);
                sDesc += " " + nave.disparos.Caracteristicas[(int)nave.TipoDisparo].nivel.ToString();
                spb.DrawString(font, sDesc, posDesc, colorDescTitulo);
                spb.DrawString(font, sDesc, posDesc + new Vector2(1, 1), Color.White);

                //multiple
                eItemArma arma = eItemArma.Multiple;
                eTipoDisparo tipoDisparo = eTipoDisparo.Multiple;
                if (bActivoArma[(int)arma])
                {
                    posDesc = tooltip.VectorPosicion(eTooltip.Multiple);
                    rectangulo = new Rectangle(PosImagenItemsArma(arma).X, PosImagenItemsArma(arma).Y,
                                               spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                }
                else
                {
                    posDesc = tooltip.VectorPosicion(eTooltip.Inactivo_Multiple);
                    rectangulo = new Rectangle(PosImagenItemsArmaBN(arma).X, PosImagenItemsArmaBN(arma).Y,
                                               spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                }
                spb.Draw(spriteItemsArma.imagen, posDesc, rectangulo, Color.White);

                if (nave.TipoDisparo == tipoDisparo)
                {
                    rectangulo = new Rectangle(posImagenArmaSel.X, posImagenArmaSel.Y,
                                               dimImagenArmaSel.X, dimImagenArmaSel.Y);
                    spb.Draw(spriteItemsArma.imagen, posDesc - restaPosArmaSel, rectangulo, Color.White);
                }

                string sTexto = Disparos.DisparoToString(tipoDisparo) + " ";
                sTexto += nave.disparos.NivelArmaToString(tipoDisparo);
                spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma, ColorTextItemsArma(arma));
                spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma + new Vector2(1, 1), Color.White);

                if (bVisibleComprarArma[(int)arma])
                {
                    if (bActivoArma[(int)arma])
                        posDesc = tooltip.VectorPosicion(eTooltip.Mejorar_Multiple);
                    else
                        posDesc = tooltip.VectorPosicion(eTooltip.Comprar_Multiple);
                    rectangulo = new Rectangle(PosImagenItems(eItems.Dinero).X, PosImagenItems(eItems.Dinero).Y,
                                               spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                    spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);

                    sTexto = nave.disparos.PrecioArmaToString(tipoDisparo);
                    spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma, ColorTextItemsArma(arma));
                    spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma + new Vector2(1, 1), Color.White);
                }

                //laser
                arma = eItemArma.Laser;
                tipoDisparo = eTipoDisparo.Laser;
                if (bActivoArma[(int)arma])
                {
                    posDesc = tooltip.VectorPosicion(eTooltip.Laser);
                    rectangulo = new Rectangle(PosImagenItemsArma(arma).X, PosImagenItemsArma(arma).Y,
                                               spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                }
                else
                {
                    posDesc = tooltip.VectorPosicion(eTooltip.Inactivo_Laser);
                    rectangulo = new Rectangle(PosImagenItemsArmaBN(arma).X, PosImagenItemsArmaBN(arma).Y,
                                               spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                }
                spb.Draw(spriteItemsArma.imagen, posDesc, rectangulo, Color.White);

                if (nave.TipoDisparo == tipoDisparo)
                {
                    rectangulo = new Rectangle(posImagenArmaSel.X, posImagenArmaSel.Y,
                                               dimImagenArmaSel.X, dimImagenArmaSel.Y);
                    spb.Draw(spriteItemsArma.imagen, posDesc - restaPosArmaSel, rectangulo, Color.White);
                }

                sTexto = Disparos.DisparoToString(tipoDisparo) + " ";
                sTexto += nave.disparos.NivelArmaToString(tipoDisparo);
                spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma, ColorTextItemsArma(arma));
                spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma + new Vector2(1, 1), Color.White);

                if (bVisibleComprarArma[(int)arma])
                {
                    if (bActivoArma[(int)arma])
                        posDesc = tooltip.VectorPosicion(eTooltip.Mejorar_Laser);
                    else
                        posDesc = tooltip.VectorPosicion(eTooltip.Comprar_Laser);
                    rectangulo = new Rectangle(PosImagenItems(eItems.Dinero).X, PosImagenItems(eItems.Dinero).Y,
                                               spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                    spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);

                    sTexto = nave.disparos.PrecioArmaToString(tipoDisparo);
                    spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma, ColorTextItemsArma(arma));
                    spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma + new Vector2(1, 1), Color.White);
                }

                //misiles
                arma = eItemArma.Misiles;
                tipoDisparo = eTipoDisparo.Misiles;
                if (bActivoArma[(int)arma])
                {
                    posDesc = tooltip.VectorPosicion(eTooltip.Misiles);
                    rectangulo = new Rectangle(PosImagenItemsArma(arma).X, PosImagenItemsArma(arma).Y,
                                               spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                }
                else
                {
                    posDesc = tooltip.VectorPosicion(eTooltip.Inactivo_Misiles);
                    rectangulo = new Rectangle(PosImagenItemsArmaBN(arma).X, PosImagenItemsArmaBN(arma).Y,
                                               spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                }
                spb.Draw(spriteItemsArma.imagen, posDesc, rectangulo, Color.White);

                if (nave.TipoDisparo == tipoDisparo)
                {
                    rectangulo = new Rectangle(posImagenArmaSel.X, posImagenArmaSel.Y,
                                               dimImagenArmaSel.X, dimImagenArmaSel.Y);
                    spb.Draw(spriteItemsArma.imagen, posDesc - restaPosArmaSel, rectangulo, Color.White);
                }

                sTexto = Disparos.DisparoToString(tipoDisparo) + " ";
                sTexto += nave.disparos.NivelArmaToString(tipoDisparo);
                spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma, ColorTextItemsArma(arma));
                spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma + new Vector2(1, 1), Color.White);

                if (bVisibleComprarArma[(int)arma])
                {
                    if (bActivoArma[(int)arma])
                        posDesc = tooltip.VectorPosicion(eTooltip.Mejorar_Misiles);
                    else
                        posDesc = tooltip.VectorPosicion(eTooltip.Comprar_Misiles);
                    rectangulo = new Rectangle(PosImagenItems(eItems.Dinero).X, PosImagenItems(eItems.Dinero).Y,
                                               spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                    spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);

                    sTexto = nave.disparos.PrecioArmaToString(tipoDisparo);
                    spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma, ColorTextItemsArma(arma));
                    spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma + new Vector2(1, 1), Color.White);
                }

                //rayo
                arma = eItemArma.Rayo;
                tipoDisparo = eTipoDisparo.Rayo;
                if (bActivoArma[(int)arma])
                {
                    posDesc = tooltip.VectorPosicion(eTooltip.Rayo);
                    rectangulo = new Rectangle(PosImagenItemsArma(arma).X, PosImagenItemsArma(arma).Y,
                                               spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                }
                else
                {
                    posDesc = tooltip.VectorPosicion(eTooltip.Inactivo_Rayo);
                    rectangulo = new Rectangle(PosImagenItemsArmaBN(arma).X, PosImagenItemsArmaBN(arma).Y,
                                               spriteItemsArma.dimImagen.X, spriteItemsArma.dimImagen.Y);
                }
                spb.Draw(spriteItemsArma.imagen, posDesc, rectangulo, Color.White);

                if (nave.TipoDisparo == tipoDisparo)
                {
                    rectangulo = new Rectangle(posImagenArmaSel.X, posImagenArmaSel.Y,
                                               dimImagenArmaSel.X, dimImagenArmaSel.Y);
                    spb.Draw(spriteItemsArma.imagen, posDesc - restaPosArmaSel, rectangulo, Color.White);
                }

                sTexto = Disparos.DisparoToString(tipoDisparo) + " ";
                sTexto += nave.disparos.NivelArmaToString(tipoDisparo);
                spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma, ColorTextItemsArma(arma));
                spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma + new Vector2(1, 1), Color.White);

                if (bVisibleComprarArma[(int)arma])
                {
                    if (bActivoArma[(int)arma])
                        posDesc = tooltip.VectorPosicion(eTooltip.Mejorar_Rayo);
                    else
                        posDesc = tooltip.VectorPosicion(eTooltip.Comprar_Rayo);
                    rectangulo = new Rectangle(PosImagenItems(eItems.Dinero).X, PosImagenItems(eItems.Dinero).Y,
                                               spriteItems.dimImagen.X, spriteItems.dimImagen.Y);
                    spb.Draw(spriteItems.imagen, posDesc, rectangulo, Color.White);

                    sTexto = nave.disparos.PrecioArmaToString(tipoDisparo);
                    spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma, ColorTextItemsArma(arma));
                    spb.DrawString(fontArma, sTexto, posDesc + sepDescHorArma + new Vector2(1, 1), Color.White);
                }

            }

        }

    }
}
