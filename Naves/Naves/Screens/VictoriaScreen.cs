#region File Description
//-----------------------------------------------------------------------------
// VictoriaScreen.cs
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
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class VictoriaScreen : GameScreen
    {
        #region Fields

        Opciones opciones;
        Partida partida;

        ContentManager content;
        Texture2D backgroundTexture;
        Texture2D capaTexture;

        //esperar x seg para poder pulsar tecla y salir
        //TimeSpan pausaGameOver;
        //TimeSpan previoTimeGameOver;
        bool bNoPulsado;
        bool bMusicaVictoria = false;

        //mover la capa game over con el texto
        TimeSpan pausaCapa;
        TimeSpan previoTimeCapa;
        float velocidadCapa = 180f;
        int hastaY = 265;
        int capaX;
        int capaY;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public VictoriaScreen(Opciones Opciones, Partida partida)
        {
            this.opciones = Opciones;
            this.partida = partida;


            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(1.5);

            pausaCapa = TimeSpan.FromSeconds(0); //no incluye los 2 seg del transition on
            bNoPulsado = true;
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

                backgroundTexture = content.Load<Texture2D>("Screens\\Victoria");
                capaTexture = content.Load<Texture2D>("Screens\\CapaVictoria");

                capaY = -capaTexture.Height;
                capaX = ScreenManager.GraphicsDevice.Viewport.Width / 2 - capaTexture.Width / 2;
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
                previoTimeCapa = gameTime.TotalGameTime; //tiempo comienzo de la pausa
            }

            if (!bMusicaVictoria)
            {
                //poner musica de gameover
                Sonidos.PlayMusic(eMusica.Victoria, false, opciones);

                bMusicaVictoria = true;
            }

            if (gameTime.TotalGameTime - previoTimeCapa > pausaCapa)
            {
                capaY += (int)(velocidadCapa * gameTime.ElapsedGameTime.TotalSeconds);

                if (capaY > hastaY)
                {
                    capaY = hastaY;
                }
            }

            if (hastaY == capaY)
                if (bNoPulsado)
                {
                    //pulsar cualquier tecla o pulsar el raton
                    bool bTeclaRaton = false;
                    foreach (Keys key in Enum.GetValues(typeof(Keys)))
                    {
                        if (Keyboard.GetState().IsKeyDown(key))
                            bTeclaRaton = true;
                    }
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        bTeclaRaton = true;

                    if (bTeclaRaton)
                    {
                        bNoPulsado = false;

                        //volvemos a la pantalla del sistema solar
                        //LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new SistemaSolarScreen(opciones, partida));

                        //volvemos a la pantalla del sistema solar
                        LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new SistemaSolarScreen(opciones, partida, (int)partida.Planeta));

                        Sonidos.PlayMusic(eMusica.CancionTitulo, opciones);
                    }
                }

        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.Draw(capaTexture, new Rectangle(capaX, capaY, capaTexture.Width, capaTexture.Height),
                             new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));

            spriteBatch.End();
        }


        #endregion
    }
}
