#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
#endregion

namespace Naves
{
    public enum ePlaneta
    {
        Mercurio,
        Venus,
        Tierra,
        Marte,
        Jupiter,
        Saturno,
        Urano,
        Neptuno
    }

    class Planeta
    {
        public Sprite sprite;
        public Vector2 posicion;
        public Vector2 tamano;
        public float zoomIn;
        public float resaltarHasta;
        public float jugarZoom;

        public Planeta()
        {
            sprite = new Sprite();
        }
    }


    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class SistemaSolarScreen : GameScreen
    {
        #region Fields

        private Opciones opciones;
        private Partida partida;

        private ContentManager content;
        private Sprite background = new Sprite();
        private Sprite spriteMarco = new Sprite();
        private SpriteFont font;
        private Sprite spriteNave = new Sprite();
        private Animacion animacionNave;
        private bool bAnimacionNaveStart;
        private Sprite spriteCompletado = new Sprite();
        private Animacion animacionCompletado;
        private bool bAnimacionCompletado;

        private bool bNave; //dibujar nave (si ha terminado la pausa inicial)
        private TimeSpan pausaInicio;
        private TimeSpan previoTimeInicio;

        private bool bMoverNave; //mover nave de un planeta a otro
        private Vector2 movimiento; //vector direccion
        private Vector2 moverNavePosicion; //posicion nave
        private Vector2 moverDestino; //posicion destino
        private ePlaneta moverPlaneta; //mover a cual planeta
        private float moverVelocidad;
        private const float DISTANCIAALCANZARDESTINO = 10;

        private bool bZoomIn; //haciendo zoom a planeta
        private bool bZoomOut; //saliendo de zoom a planeta
        private TimeSpan zoomInDuracion; //tiempo total
        private TimeSpan zoomOutDuracion; //tiempo total
        private TimeSpan zoomTimeInicio; //tiempo inicial
        private TimeSpan zoomTiempo; //tiempo transcurrido
        //private float zoomIn; //scala con zoom
        private float zoomOut; //scala sin zoom

        private float zoomActual; //zoom actual (al terminar animacion zoom in/out)

        private bool bEspera; //si acabamos de seleccionar planeta y no aun hay q hacer zoom in (permitir otro movimiento)
        private TimeSpan esperaDuracion;
        private TimeSpan esperaTimeInicio;

        //resalta planeta seleccionado
        private bool bResaltarCrece; //crece o decrece
        //private float resaltarHasta; //crece hasta esta escala
        private TimeSpan resaltarDuracion; //tiempo tarda en crecer/decrecer
        private TimeSpan resaltarTimeInicio; //tiempo inicial
        private TimeSpan resaltarTiempo; //tiempo transcurrido
        private float resaltarScalaActual; //escala de planeta actual, para usarlo en bJugar

        //animacion completa un planeta y avanza al siguiente
        private bool bCompletado; //si es cierto pone la animacion y mueve la nave
        private bool bCompletadoStart; //inicializa el update una unica vez
        //private ePlaneta completadoPlaneta; //planeta q se ha completado
        private TimeSpan completadoDuracion; //tiempo tarda en avanzar
        //private TimeSpan completadoTimeInicio; //tiempo inicial
        //private TimeSpan completadoTiempo; //tiempo transcurrido

        private bool bJugar; //empezar partida, aterrizaje nave en el planeta
        private TimeSpan jugarDuracion; //tiempo tarda en crecer/decrecer
        private TimeSpan jugarTimeInicio; //tiempo inicial
        private TimeSpan jugarTiempo; //tiempo transcurrido
        private float jugarResaltarPlaneta; //escala del planeta q va aumentando
        //private float jugarZoomIn; //zoom in hasta esta escala
        private bool bSalir; //no dibujar nada

        private int panelPlaneta; //si se pasa el raton encima de un planeta, el panel nos muestra su info

        private ePlaneta selPlaneta; //planeta selecionado
        private Planeta[] planetas = new Planeta[Enum.GetValues(typeof(ePlaneta)).Length]; //clase con detalles de cada planeta
        private ePlaneta planetaInicial; //planeta cargado de la partida

        #endregion


        #region Initialization

        ////////////////// PUBLICA //////////////////
        public float RotateMovimiento
        {
            get { return (float)Math.Atan2(movimiento.Y, movimiento.X) + (float)Math.PI / 2; }
        }

        public static string PlanetaToString(ePlaneta planeta)
        {
            string str = "";

            switch (planeta)
            {
                case ePlaneta.Mercurio:
                    str = Recursos.Mercurio;
                    break;
                case ePlaneta.Venus:
                    str = Recursos.Venus;
                    break;
                case ePlaneta.Tierra:
                    str = Recursos.Tierra;
                    break;
                case ePlaneta.Marte:
                    str = Recursos.Marte;
                    break;
                case ePlaneta.Jupiter:
                    str = Recursos.Jupiter;
                    break;
                case ePlaneta.Saturno:
                    str = Recursos.Saturno;
                    break;
                case ePlaneta.Urano:
                    str = Recursos.Urano;
                    break;
                case ePlaneta.Neptuno:
                    str = Recursos.Neptuno;
                    break;
            }

            return str;
        }


        ////////////////// CONSTRUCTOR //////////////////
        /// <summary>
        /// constructor de SistemaSolarScreen
        /// </summary>
        /// <param name="opciones"></param>
        /// <param name="partida"></param>
        /// <param name="planetaCompletado">el numero de ePlaneta del planeta q se acaba de completar (animacion avanza de planeta)</param>
        public SistemaSolarScreen(Opciones opciones, Partida partida, int planetaCompletado = -1)
        {
            this.opciones = opciones;
            this.partida = partida;


            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.5);

            bNave = false;
            pausaInicio = TimeSpan.FromSeconds(1);
            
            bZoomIn = false;
            bZoomOut = false;
            //zoomIn = 1f; //depende de cada planeta
            //zoomOut = (float)viewport.Width / background.dimImagen.X; //en activate, despues de establecer dimImagen
            //zoomActual = zoomOut;                                     //en activate, despues de establecer zoomOut
            zoomInDuracion = TimeSpan.FromSeconds(2f);
            zoomOutDuracion = TimeSpan.FromSeconds(1.2f);

            bMoverNave = false;
            //moverVelocidad = 250f;

            //resaltarHasta = 1.6f; //depende de cada planeta
            resaltarDuracion = TimeSpan.FromSeconds(1.0f);
            resaltarScalaActual = 1f;

            bEspera = false;
            esperaDuracion = TimeSpan.FromSeconds(1f);

            bJugar = false;
            jugarDuracion = TimeSpan.FromSeconds(3.0);
            jugarResaltarPlaneta = 3f;
            //jugarZoomIn = 4f; //depende de cada planeta
            bSalir = false;

            panelPlaneta = -1;

            bCompletado = false;
            completadoDuracion = TimeSpan.FromSeconds(3.5f);
            if (planetaCompletado != -1)
            {
                bCompletadoStart = true;
                planetaInicial = (ePlaneta)planetaCompletado;
            }
            else
            {
                bCompletadoStart = false;

                if (partida.PlanetaSiguiente != -1)
                    planetaInicial = (ePlaneta)partida.PlanetaSiguiente;
                else
                    planetaInicial = ePlaneta.Neptuno; //todos los planetas completados
            }
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                background.imagen = content.Load<Texture2D>("Screens\\Sistema_Solar");
                background.dimImagen = new Point(3300, 2475);
                background.posImagen = new Point(0, 0);


                zoomOut = (float)ScreenManager.GraphicsDevice.Viewport.Width / background.dimImagen.X;
                zoomActual = zoomOut;


                font = content.Load<SpriteFont>("Fuentes\\SistemaSolarMed");

                
                spriteMarco.imagen = content.Load<Texture2D>("Screens\\frame_01");
                spriteMarco.dimImagen = new Point(400, 250);
                spriteMarco.posImagen = new Point(0, 0);


                spriteNave.imagen = content.Load<Texture2D>("Screens\\nave_01");
                spriteNave.dimImagen = new Point(39, 43);
                spriteNave.posImagen = new Point(0, 0);
                bAnimacionNaveStart = true;


                spriteCompletado.imagen = content.Load<Texture2D>("Screens\\planeta_completado_01");
                spriteCompletado.dimImagen = new Point(256, 256);
                spriteCompletado.posImagen = new Point(0, 0);
                bAnimacionCompletado = false;


                float zoomPeq = 1f;
                float zoomGran = 0.65f;
                float resHasPeq = 1.80f;
                float resHasGran = 1.50f;
                float jugarPeq = 4.5f;
                float jugarGran = 2.4f;

                planetas[(int)ePlaneta.Mercurio] = new Planeta();
                planetas[(int)ePlaneta.Mercurio].sprite.imagen = content.Load<Texture2D>("Screens\\mercurio_01");
                planetas[(int)ePlaneta.Mercurio].sprite.dimImagen = new Point(350, 350);
                planetas[(int)ePlaneta.Mercurio].sprite.posImagen = new Point(0, 0);
                planetas[(int)ePlaneta.Mercurio].posicion = new Vector2(620, 1121);
                planetas[(int)ePlaneta.Mercurio].tamano = new Vector2(78, 78); //75*75 lo agrando un poco
                planetas[(int)ePlaneta.Mercurio].zoomIn = zoomPeq;
                planetas[(int)ePlaneta.Mercurio].resaltarHasta = resHasPeq;
                planetas[(int)ePlaneta.Mercurio].jugarZoom = jugarPeq;

                planetas[(int)ePlaneta.Venus] = new Planeta();
                planetas[(int)ePlaneta.Venus].sprite.imagen = content.Load<Texture2D>("Screens\\venus_01");
                planetas[(int)ePlaneta.Venus].sprite.dimImagen = new Point(500, 500);
                planetas[(int)ePlaneta.Venus].sprite.posImagen = new Point(0, 0);
                planetas[(int)ePlaneta.Venus].posicion = new Vector2(854, 1022);
                planetas[(int)ePlaneta.Venus].tamano = new Vector2(123, 123);
                planetas[(int)ePlaneta.Venus].zoomIn = zoomPeq;
                planetas[(int)ePlaneta.Venus].resaltarHasta = resHasPeq;
                planetas[(int)ePlaneta.Venus].jugarZoom = jugarPeq;

                planetas[(int)ePlaneta.Tierra] = new Planeta();
                planetas[(int)ePlaneta.Tierra].sprite.imagen = content.Load<Texture2D>("Screens\\tierra_01");
                planetas[(int)ePlaneta.Tierra].sprite.dimImagen = new Point(500, 500);
                planetas[(int)ePlaneta.Tierra].sprite.posImagen = new Point(0, 0);
                planetas[(int)ePlaneta.Tierra].posicion = new Vector2(1091, 890);
                planetas[(int)ePlaneta.Tierra].tamano = new Vector2(126, 126); //123*123 lo agrando un poco
                planetas[(int)ePlaneta.Tierra].zoomIn = zoomPeq;
                planetas[(int)ePlaneta.Tierra].resaltarHasta = resHasPeq;
                planetas[(int)ePlaneta.Tierra].jugarZoom = jugarPeq;

                planetas[(int)ePlaneta.Marte] = new Planeta();
                planetas[(int)ePlaneta.Marte].sprite.imagen = content.Load<Texture2D>("Screens\\marte_01");
                planetas[(int)ePlaneta.Marte].sprite.dimImagen = new Point(500, 500);
                planetas[(int)ePlaneta.Marte].sprite.posImagen = new Point(0, 0);
                planetas[(int)ePlaneta.Marte].posicion = new Vector2(1256, 736);
                planetas[(int)ePlaneta.Marte].tamano = new Vector2(100, 100); //99*99
                planetas[(int)ePlaneta.Marte].zoomIn = zoomPeq;
                planetas[(int)ePlaneta.Marte].resaltarHasta = resHasPeq;
                planetas[(int)ePlaneta.Marte].jugarZoom = jugarPeq;


                planetas[(int)ePlaneta.Jupiter] = new Planeta();
                planetas[(int)ePlaneta.Jupiter].sprite.imagen = content.Load<Texture2D>("Screens\\jupiter_01");
                planetas[(int)ePlaneta.Jupiter].sprite.dimImagen = new Point(700, 700);
                planetas[(int)ePlaneta.Jupiter].sprite.posImagen = new Point(0, 0);
                planetas[(int)ePlaneta.Jupiter].posicion = new Vector2(1674, 794);
                planetas[(int)ePlaneta.Jupiter].tamano = new Vector2(318, 318);
                planetas[(int)ePlaneta.Jupiter].zoomIn = zoomGran;
                planetas[(int)ePlaneta.Jupiter].resaltarHasta = resHasGran;
                planetas[(int)ePlaneta.Jupiter].jugarZoom = jugarGran;

                planetas[(int)ePlaneta.Saturno] = new Planeta();
                planetas[(int)ePlaneta.Saturno].sprite.imagen = content.Load<Texture2D>("Screens\\saturno_01");
                planetas[(int)ePlaneta.Saturno].sprite.dimImagen = new Point(969, 422);
                planetas[(int)ePlaneta.Saturno].sprite.posImagen = new Point(0, 0);
                planetas[(int)ePlaneta.Saturno].posicion = new Vector2(2087, 600);
                planetas[(int)ePlaneta.Saturno].tamano = new Vector2(567, 247);
                planetas[(int)ePlaneta.Saturno].zoomIn = zoomGran;
                planetas[(int)ePlaneta.Saturno].resaltarHasta = resHasGran;
                planetas[(int)ePlaneta.Saturno].jugarZoom = jugarGran;

                planetas[(int)ePlaneta.Urano] = new Planeta();
                planetas[(int)ePlaneta.Urano].sprite.imagen = content.Load<Texture2D>("Screens\\urano_01");
                planetas[(int)ePlaneta.Urano].sprite.dimImagen = new Point(367, 730);
                planetas[(int)ePlaneta.Urano].sprite.posImagen = new Point(0, 0);
                planetas[(int)ePlaneta.Urano].posicion = new Vector2(2411, 424);
                //planetas[(int)ePlaneta.Urano].tamano = new Vector2(175, 301); 
                planetas[(int)ePlaneta.Urano].tamano = new Vector2(175, 348); //ajuste para mantener proporciones
                planetas[(int)ePlaneta.Urano].zoomIn = zoomGran;
                planetas[(int)ePlaneta.Urano].resaltarHasta = resHasGran;
                planetas[(int)ePlaneta.Urano].jugarZoom = jugarGran;

                planetas[(int)ePlaneta.Neptuno] = new Planeta();
                planetas[(int)ePlaneta.Neptuno].sprite.imagen = content.Load<Texture2D>("Screens\\neptuno_01");
                planetas[(int)ePlaneta.Neptuno].sprite.dimImagen = new Point(500, 500);
                planetas[(int)ePlaneta.Neptuno].sprite.posImagen = new Point(0, 0);
                planetas[(int)ePlaneta.Neptuno].posicion = new Vector2(2680, 277);
                planetas[(int)ePlaneta.Neptuno].tamano = new Vector2(173, 173);
                planetas[(int)ePlaneta.Neptuno].zoomIn = zoomGran;
                planetas[(int)ePlaneta.Neptuno].resaltarHasta = resHasGran;
                planetas[(int)ePlaneta.Neptuno].jugarZoom = jugarGran;

            }
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void Unload()
        {
            content.Unload();
        }

        #endregion


        #region Update and Draw

        private void MoverNavePlaneta(GameTime gameTime)
        {
            if (moverPlaneta != selPlaneta)
            {
                if (bEspera)
                {
                    //no necesario zoom out
                    bEspera = false;
                    bMoverNave = true;
                }
                else if (bCompletado)
                {
                    //no necesario zoom out
                    bMoverNave = true;
                }
                else
                {
                    bZoomOut = true;
                    zoomTimeInicio = gameTime.TotalGameTime;
                }

                Vector2 moverOrigen = planetas[(int)selPlaneta].posicion;
                moverNavePosicion = moverOrigen;
                moverDestino = planetas[(int)moverPlaneta].posicion;
                movimiento = Matematicas.Vector(moverOrigen, moverDestino);
                movimiento.Normalize();

                float distancia = Matematicas.Distancia(moverOrigen, moverDestino);
                if (bCompletado)
                    moverVelocidad = distancia / (float)(completadoDuracion.TotalMilliseconds / 1000);
                else
                {
                    if (distancia > 500)
                        moverVelocidad = distancia / 2.5f;
                    else
                        moverVelocidad = distancia / 1.5f;
                }
            }
        }

        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false); //false

            if (this.ScreenState == GameStateManagement.ScreenState.TransitionOn)
            {
                previoTimeInicio = gameTime.TotalGameTime; //tiempo comienzo de la pausa
            }


            if (bAnimacionNaveStart)
            {
                animacionNave = new Animacion(gameTime, 4, TimeSpan.FromSeconds(0.15f), true);
                bAnimacionNaveStart = false;
            }
            else
            {
                animacionNave.Update(gameTime, spriteNave);
            }

            if (bAnimacionCompletado)
            {
                animacionCompletado.Update(gameTime, spriteCompletado);
            }

            if (gameTime.TotalGameTime - previoTimeInicio > pausaInicio)
            {
                if (!bNave) //pausa incial, aun no se ve la nave
                {
                    bNave = true;
                    selPlaneta = planetaInicial;

                    bZoomIn = true;
                    zoomTimeInicio = gameTime.TotalGameTime;
                }
            }

            if (bNave) //dibujar la nave
            {
                //captura raton
                if (!bZoomIn && !bZoomOut && !bJugar) 
                {
                    Point posRaton = new Point(Mouse.GetState().X, Mouse.GetState().Y);

                    bool bPanelPlaneta = false;

                    float zoom = zoomActual;
                    Vector2 posPlaneta;
                    if(bMoverNave)
                        posPlaneta = moverNavePosicion;
                    else
                        posPlaneta = planetas[(int)selPlaneta].posicion;
                    Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                    Rectangle fullscreen = new Rectangle(0, 0, (int)Math.Round(viewport.Width / zoom), (int)Math.Round(viewport.Height / zoom));
                    Vector2 posIni = new Vector2(posPlaneta.X - fullscreen.Width / 2, posPlaneta.Y - fullscreen.Height / 2);

                    if (posIni.X < 0)
                        posIni.X = 0;
                    if (posIni.Y < 0)
                        posIni.Y = 0;

                    if (posIni.X + fullscreen.Width > background.dimImagen.X)
                        posIni.X = background.dimImagen.X - fullscreen.Width;
                    if (posIni.Y + fullscreen.Height > background.dimImagen.Y)
                        posIni.Y = background.dimImagen.Y - fullscreen.Height;

                    Rectangle rectPlaneta;
                    Vector2 posicionPlaneta;
                    Vector2 scale;
                    for (int iPlaneta = 0; iPlaneta < Enum.GetValues(typeof(ePlaneta)).Length; iPlaneta++)
                    {
                        posicionPlaneta = (planetas[iPlaneta].posicion - posIni) * zoom;

                        if (iPlaneta != (int)selPlaneta) //planeta no seleccionado
                        {
                            scale = zoom * planetas[iPlaneta].tamano / new Vector2(planetas[iPlaneta].sprite.dimImagen.X, planetas[iPlaneta].sprite.dimImagen.Y);
                        }
                        else //planeta ya seleccionado
                        {
                            float resaltarScala;
                            float progreso = (float)resaltarTiempo.TotalMilliseconds / (float)resaltarDuracion.TotalMilliseconds;
                            if (bResaltarCrece)
                                resaltarScala = MathHelper.Lerp(1f, planetas[(int)selPlaneta].resaltarHasta, progreso);
                            else
                                resaltarScala = MathHelper.Lerp(planetas[(int)selPlaneta].resaltarHasta, 1f, progreso);

                            scale = resaltarScala * zoom * planetas[iPlaneta].tamano / new Vector2(planetas[iPlaneta].sprite.dimImagen.X, planetas[iPlaneta].sprite.dimImagen.Y);
                        }

                        rectPlaneta = new Rectangle(
                            (int)(posicionPlaneta.X - scale.X * planetas[iPlaneta].sprite.center.X),
                            (int)(posicionPlaneta.Y - scale.Y * planetas[iPlaneta].sprite.center.Y),
                            (int)(scale.X * planetas[iPlaneta].sprite.dimImagen.X),
                            (int)(scale.Y * planetas[iPlaneta].sprite.dimImagen.Y));

                        if (rectPlaneta.Contains(posRaton))
                        {
                            if (Mouse.GetState().LeftButton == ButtonState.Pressed) //pulsar boton
                            {
                                if (!bMoverNave) //no pulsar mientras te mueves
                                {
                                    if (iPlaneta == (int)selPlaneta)
                                    {
                                        if (partida.PartidaDatos.estado[(int)selPlaneta] != eEstadoPlaneta.Bloqueado)
                                        {
                                            //aterrizar en planeta
                                            bJugar = true;
                                            jugarTimeInicio = gameTime.TotalGameTime;
                                        }
                                    }
                                    else
                                    {
                                        //mover a otro planeta
                                        moverPlaneta = (ePlaneta)iPlaneta;
                                        MoverNavePlaneta(gameTime);
                                    }
                                }
                            }
                            else //pasar raton por encima
                            {
                                if (bMoverNave || (iPlaneta != (int)selPlaneta))
                                {
                                    panelPlaneta = iPlaneta;
                                    bPanelPlaneta = true;
                                }

                            }
                        }
                    }

                    if (!bPanelPlaneta)
                    {
                        panelPlaneta = -1;
                    }
                }
                

                //captura teclado
                if (!bMoverNave && !bZoomIn && !bZoomOut && !bCompletado)
                {
                    if (Keyboard.GetState().IsKeyDown(opciones.GetTeclas(eTeclas.Izquierda)))
                    {
                        int iPlaneta = (int)selPlaneta - 1;
                        if (iPlaneta < 0)
                            iPlaneta = 0;
                        moverPlaneta = (ePlaneta)iPlaneta;

                        MoverNavePlaneta(gameTime);
                    }

                    if (Keyboard.GetState().IsKeyDown(opciones.GetTeclas(eTeclas.Derecha)))
                    {
                        int iPlaneta = (int)selPlaneta + 1;
                        if (iPlaneta >= Enum.GetValues(typeof(ePlaneta)).Length - 1) //controlar planeta maximo de los q estan abiertos
                            iPlaneta = Enum.GetValues(typeof(ePlaneta)).Length - 1;
                        moverPlaneta = (ePlaneta)iPlaneta;

                        MoverNavePlaneta(gameTime);
                    }

                    resaltarTiempo = gameTime.TotalGameTime - resaltarTimeInicio;
                    if (resaltarTiempo > resaltarDuracion)
                    {
                        //invierte crecer/decrecer
                        bResaltarCrece = !bResaltarCrece;
                        resaltarTimeInicio = gameTime.TotalGameTime;
                        resaltarTiempo = TimeSpan.Zero;
                    }
                }

                if (!bMoverNave && !bZoomIn && !bZoomOut && !bEspera && !bJugar && !bCompletado)
                {
                    if (zoomActual != zoomOut)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        {
                            if (partida.PartidaDatos.estado[(int)selPlaneta] != eEstadoPlaneta.Bloqueado)
                            {
                                bJugar = true;
                                jugarTimeInicio = gameTime.TotalGameTime;
                            }
                        }
                    }
                }

                //empezar partida
                if (bJugar)
                {
                    jugarTiempo = gameTime.TotalGameTime - jugarTimeInicio;
                    if (jugarTiempo >= jugarDuracion)
                    {
                        //cambiar a GameTorresScreen, empezar partida
                        bJugar = false;
                        bSalir = true;
                        partida.Planeta = selPlaneta;
                        ScreenManager.AddScreen(new GameTorresScreen(opciones, partida), ControllingPlayer);
                        this.ExitScreen();
                    }
                }

                //mover la nave
                if (bMoverNave)
                {
                    moverNavePosicion += movimiento * moverVelocidad * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (Matematicas.Distancia(moverNavePosicion, moverDestino) < DISTANCIAALCANZARDESTINO)
                    {
                        bMoverNave = false;
                        selPlaneta = moverPlaneta;

                        bEspera = true;
                        esperaTimeInicio = gameTime.TotalGameTime;

                        if (bCompletado)
                        {
                            bCompletado = false; //termina de avanzar al siguiente planeta
                            bAnimacionCompletado = false;
                        }
                    }
                }

                //despues de moverse, espera antes del zoom in
                if (bEspera)
                {
                    if (gameTime.TotalGameTime - esperaTimeInicio > esperaDuracion)
                    {
                        bEspera = false;

                        if (zoomActual == zoomOut)
                        {
                            bZoomIn = true;
                            zoomTimeInicio = gameTime.TotalGameTime;
                        }
                    }
                }

                //zoom in / out
                if (bZoomIn || bZoomOut)
                {
                    zoomTiempo = gameTime.TotalGameTime - zoomTimeInicio;
                    if (bZoomIn)
                    {
                        if (zoomTiempo >= zoomInDuracion)
                        {
                            bZoomIn = false;
                            zoomActual = planetas[(int)selPlaneta].zoomIn;
 
                            bResaltarCrece = true;
                            resaltarTimeInicio = gameTime.TotalGameTime;
                            resaltarTiempo = TimeSpan.Zero;
                        }
                    }
                    else
                    {
                        if (zoomTiempo >= zoomOutDuracion)
                        {
                            bZoomOut = false;
                            zoomActual = zoomOut;

                            if (moverPlaneta != selPlaneta)
                            {
                                bMoverNave = true;
                            }
                        }
                    }
                }

                //planeta completado
                if (bCompletadoStart && bNave && !bZoomIn)
                {
                    //animacion planeta completado
                    animacionCompletado = new Animacion(gameTime, 16, TimeSpan.FromSeconds(0.14f), false);
                    bAnimacionCompletado = true;
 
                    //avanzar al planeta siguiente
                    bCompletado = true;
                    bCompletadoStart = false;
                    //completadoTimeInicio = gameTime.TotalGameTime;
                    //mover a planeta siguiente, si no lo hay -> mover a neptuno
                    moverPlaneta = partida.PlanetaSiguiente != -1 ? (ePlaneta)partida.PlanetaSiguiente : ePlaneta.Neptuno;
                    MoverNavePlaneta(gameTime);
                }

            }

        }



        private void DibujaInfoPlaneta(SpriteBatch spb, Viewport viewport)
        {
            //marco estrellas con informacion planeta y waves

            ePlaneta infoPlaneta;
            if (panelPlaneta != -1) //si está el raton sobre un planeta (panelPlaneta), sino ponemos el planeta seleccionado
                infoPlaneta = (ePlaneta)panelPlaneta;
            else
                infoPlaneta = selPlaneta;

            Vector2 posMarco = new Vector2(viewport.Width / 2 - spriteMarco.dimImagen.X / 2,
                                        viewport.Height - spriteMarco.dimImagen.Y - 25);
            Rectangle rectangulo = new Rectangle(spriteMarco.posImagen.X, spriteMarco.posImagen.Y,
                                                    spriteMarco.dimImagen.X, spriteMarco.dimImagen.Y);
            spb.Draw(spriteMarco.imagen, posMarco, rectangulo, Color.LightGray);

            //nombre planeta
            string sTexto = Recursos.Planeta + " " + PlanetaToString(infoPlaneta);
            Vector2 tamTexto = font.MeasureString(sTexto);
            int ancho = (int)tamTexto.X;
            int alto = (int)tamTexto.Y;
            Vector2 pos = posMarco + new Vector2(spriteMarco.dimImagen.X / 2 - ancho / 2, 50);
            spb.DrawString(font, sTexto, pos, Color.WhiteSmoke);

            //completado / no completado
            Color col = Color.Blue;
            switch (partida.PartidaDatos.estado[(int)infoPlaneta])
            {
                case eEstadoPlaneta.Bloqueado:
                    sTexto = Recursos.Bloqueado;
                    col = Color.Red;
                    break;
                case eEstadoPlaneta.Completado:
                    sTexto = Recursos.Completado;
                    col = Color.Green;
                    break;
                case eEstadoPlaneta.SinCompletar:
                    sTexto = Recursos.Sin_Completar;
                    col = Color.FromNonPremultiplied(60, 60, 255, 255); //azulado
                    break;
            }
            tamTexto = font.MeasureString(sTexto);
            ancho = (int)tamTexto.X;
            alto = (int)tamTexto.Y;
            pos = posMarco + new Vector2(spriteMarco.dimImagen.X / 2 - ancho / 2, 90);
            spb.DrawString(font, sTexto, pos, col);

            if (partida.PartidaDatos.estado[(int)infoPlaneta] != eEstadoPlaneta.Bloqueado)
            {
                //record dinero sobrante
                sTexto = Recursos.Record + " " + Recursos.Dinero + " " + partida.PartidaDatos.dinero[(int)infoPlaneta];
                tamTexto = font.MeasureString(sTexto);
                ancho = (int)tamTexto.X;
                alto = (int)tamTexto.Y;
                pos = posMarco + new Vector2(spriteMarco.dimImagen.X / 2 - ancho / 2, 130);
                spb.DrawString(font, sTexto, pos, Color.Gold);

                //record puntos
                sTexto = Recursos.Record + " " + Recursos.Puntos + " " + partida.PartidaDatos.puntos[(int)infoPlaneta];
                tamTexto = font.MeasureString(sTexto);
                ancho = (int)tamTexto.X;
                alto = (int)tamTexto.Y;
                pos = posMarco + new Vector2(spriteMarco.dimImagen.X / 2 - ancho / 2, 170);
                spb.DrawString(font, sTexto, pos, Color.White);
            }

        }

        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spb = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle rectangle;

            spb.Begin();

            if (!bSalir)
            {
                if (bNave) //termina pausa inicial, dibuja la nave
                {
                    Vector2 posPlaneta = Vector2.Zero;
                    Vector2 posNave = Vector2.Zero;
                    float zoom = 1f;
                    float rotacionNave = 0f;
                    float resaltarScala = 1f; //resaltar planeta (escala de crecimiento)


                    if (!bMoverNave && !bZoomIn && !bZoomOut && !bEspera && !bJugar)
                    {
                        float progreso = (float)resaltarTiempo.TotalMilliseconds / (float)resaltarDuracion.TotalMilliseconds;
                        if (bResaltarCrece)
                            resaltarScala = MathHelper.Lerp(1f, planetas[(int)selPlaneta].resaltarHasta, progreso);
                        else
                            resaltarScala = MathHelper.Lerp(planetas[(int)selPlaneta].resaltarHasta, 1f, progreso);

                        resaltarScalaActual = resaltarScala;

                        posPlaneta = planetas[(int)selPlaneta].posicion;
                        posNave = posPlaneta;
                        zoom = zoomActual;
                        rotacionNave = 0f;
                    }

                    if (bMoverNave)
                    {
                        posPlaneta = moverNavePosicion;
                        posNave = moverNavePosicion;
                        zoom = zoomActual;
                        rotacionNave = RotateMovimiento;
                        resaltarScala = 1f;
                    }

                    if (bEspera)
                    {
                        posPlaneta = planetas[(int)selPlaneta].posicion;
                        posNave = posPlaneta;
                        zoom = zoomActual;
                        rotacionNave = 0f;
                        resaltarScala = 1f;
                    }

                    if (bJugar)
                    {
                        float progreso = (float)jugarTiempo.TotalMilliseconds / (float)jugarDuracion.TotalMilliseconds;
                        zoom = MathHelper.Lerp(planetas[(int)selPlaneta].zoomIn, planetas[(int)selPlaneta].jugarZoom, progreso);

                        resaltarScala = MathHelper.Lerp(resaltarScalaActual, jugarResaltarPlaneta, progreso);

                        posPlaneta = planetas[(int)selPlaneta].posicion;
                        posNave = posPlaneta;
                        rotacionNave = 0f;
                    }

                    if (bZoomIn || bZoomOut)
                    {

                        if (bZoomIn)
                        {
                            float progreso = (float)zoomTiempo.TotalMilliseconds / (float)zoomInDuracion.TotalMilliseconds;
                            zoom = MathHelper.Lerp(zoomOut, planetas[(int)selPlaneta].zoomIn, progreso);
                        }
                        else
                        {
                            float progreso = (float)zoomTiempo.TotalMilliseconds / (float)zoomOutDuracion.TotalMilliseconds;
                            zoom = MathHelper.Lerp(planetas[(int)selPlaneta].zoomIn, zoomOut, progreso);
                        }

                        posPlaneta = planetas[(int)selPlaneta].posicion;
                        posNave = posPlaneta;
                        rotacionNave = 0f;
                        resaltarScala = 1f;
                    }


                    //dibuja sistema solar
                    Rectangle fullscreen = new Rectangle(0, 0, (int)Math.Round(viewport.Width / zoom), (int)Math.Round(viewport.Height / zoom));
                    Vector2 posIni = new Vector2(posPlaneta.X - fullscreen.Width / 2, posPlaneta.Y - fullscreen.Height / 2);

                    if (posIni.X < 0)
                        posIni.X = 0;
                    if (posIni.Y < 0)
                        posIni.Y = 0;

                    if (posIni.X + fullscreen.Width > background.dimImagen.X)
                        posIni.X = background.dimImagen.X - fullscreen.Width;
                    if (posIni.Y + fullscreen.Height > background.dimImagen.Y)
                        posIni.Y = background.dimImagen.Y - fullscreen.Height;

                    rectangle = new Rectangle((int)Math.Round(posIni.X), (int)Math.Round(posIni.Y),
                                              fullscreen.Width, fullscreen.Height);

                    spb.Draw(background.imagen, Vector2.Zero, rectangle, Color.White, 0f, Vector2.Zero, zoom, SpriteEffects.None, 0f);

                    //dibuja planetas
                    Vector2 posicionPlaneta;
                    Vector2 scale;
                    for (int i = 0; i < Enum.GetValues(typeof(ePlaneta)).Length; i++)
                    {
                        if (i != (int)selPlaneta) //el resaltado dibujarlo despues
                        {
                            posicionPlaneta = (planetas[i].posicion - posIni) * zoom;
                            rectangle = new Rectangle(planetas[i].sprite.posImagen.X, planetas[i].sprite.posImagen.Y,
                                                      planetas[i].sprite.dimImagen.X, planetas[i].sprite.dimImagen.Y);
                            scale = zoom * planetas[i].tamano / new Vector2(planetas[i].sprite.dimImagen.X, planetas[i].sprite.dimImagen.Y);

                            spb.Draw(planetas[i].sprite.imagen, posicionPlaneta, rectangle, Color.White, 0f, planetas[i].sprite.center, scale, SpriteEffects.None, 0f);
                        }
                    }

                    //dibuja planeta resaltado
                    posicionPlaneta = (planetas[(int)selPlaneta].posicion - posIni) * zoom;
                    rectangle = new Rectangle(planetas[(int)selPlaneta].sprite.posImagen.X, planetas[(int)selPlaneta].sprite.posImagen.Y,
                                              planetas[(int)selPlaneta].sprite.dimImagen.X, planetas[(int)selPlaneta].sprite.dimImagen.Y);
                    scale = resaltarScala * zoom * planetas[(int)selPlaneta].tamano / new Vector2(planetas[(int)selPlaneta].sprite.dimImagen.X, planetas[(int)selPlaneta].sprite.dimImagen.Y);

                    spb.Draw(planetas[(int)selPlaneta].sprite.imagen, posicionPlaneta, rectangle, Color.White, 0f, planetas[(int)selPlaneta].sprite.center, scale, SpriteEffects.None, 0f);


                    //dibuja animacion planeta completado
                    if (bAnimacionCompletado)
                    {
                        posicionPlaneta = (planetas[(int)selPlaneta].posicion - posIni) * zoom;
                        float tamPlaneta = Math.Min(planetas[(int)selPlaneta].tamano.X, planetas[(int)selPlaneta].tamano.Y);
                        float escala = 1.25f * zoom * tamPlaneta / spriteCompletado.dimImagen.X;

                        animacionCompletado.Draw(spb, spriteCompletado, posicionPlaneta, 0f, escala);
                    }


                    //dibuja nave
                    Vector2 posicionNave = (posNave - posIni) * zoom;
                    animacionNave.Draw(spb, spriteNave, posicionNave, rotacionNave);


                    if (!bZoomOut && !bJugar)
                    {
                        if (!bMoverNave || panelPlaneta != -1) //con nave quieta, o moviendose y pasamos raton por encima de un planeta
                        {
                            //marco estrellas con informacion planeta y waves
                            DibujaInfoPlaneta(spb, viewport);
                        }
                    }
                }


                else //pausa inicial, sin zoom ni nave
                {
                    //dibuja sistema solar
                    rectangle = new Rectangle(background.posImagen.X, background.posImagen.Y,
                                              background.dimImagen.X, background.dimImagen.Y);

                    spb.Draw(background.imagen, Vector2.Zero, rectangle, new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha),
                             0f, Vector2.Zero, zoomActual, SpriteEffects.None, 0f);

                    //dibuja planetas
                    for (int i = 0; i < Enum.GetValues(typeof(ePlaneta)).Length; i++)
                    {
                        Vector2 posicionPlaneta = planetas[i].posicion * zoomActual;
                        rectangle = new Rectangle(planetas[i].sprite.posImagen.X, planetas[i].sprite.posImagen.Y,
                                                  planetas[i].sprite.dimImagen.X, planetas[i].sprite.dimImagen.Y);
                        Vector2 scale = zoomActual * planetas[i].tamano / new Vector2(planetas[i].sprite.dimImagen.X, planetas[i].sprite.dimImagen.Y);

                        spb.Draw(planetas[i].sprite.imagen, posicionPlaneta, rectangle, Color.White, 0f, planetas[i].sprite.center, scale, SpriteEffects.None, 0f);
                    }
                }
            }

            spb.End();
        }
        
        #endregion
    }
}
