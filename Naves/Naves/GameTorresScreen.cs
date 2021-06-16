#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameStateManagement;
#endregion

namespace Naves
{
    /// <summary>
    /// PANTALLA DE JUEGO PRINCIPAL
    /// </summary>
    class GameTorresScreen : GameScreen
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        ////////////////// screen propiedades (menus) //////////////////
        float pauseAlpha;
        InputAction pauseAction;
        ContentManager content;


        ////////////////// game propiedades //////////////////
        //objetos principales
        Opciones opciones;
        Partida partida;
        MenuTorres menuTorres;
        Fondo fondo;
        Tablero tablero;
        Fase fase;
        Waves waves;
        Enemigos enemigos;
        Torres torres;
        Nave nave;
        Animaciones animaciones;
        GameOver gameOver;
        Marcador marcador;
        Tooltip tooltip;
        Frame frame;


        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Mouse states used to determine mouse click / over
        MouseState currentMouseState;
        MouseState previousMouseState;

        //viewports (ventanas)
        Viewport ventana;       //ventana total, 1024*768
        Viewport ventanaGame;   //ventana juego, 800*600 o más


        ////////////////// CONSTRUCTOR //////////////////
        public GameTorresScreen(Opciones Opciones, Partida partida)
        {
            this.opciones = Opciones;
            this.partida = partida;


            //screen constructor
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5); //transicion off para la pausa (no hace caso, usa variable de Upload())

            //teclas q activarán la pause screen
            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// ACTIVATE //////////////////
        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");


                Initialize();

                LoadContent();


                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

        }


        ////////////////// INICIALIZAR //////////////////
        protected void Initialize()
        {
            //ventanas
            ventana = ScreenManager.GraphicsDevice.Viewport;

            //700*600 = 28*24 (*25)   (14*12)
            //750*650 = 30*26 (*25)   (14*12) (incluido muro)
            ventanaGame = new Viewport();
            int X = 750;
            int Y = 650;
            //ventanaGame.X = (ventana.Width - X) / 2;
            ventanaGame.X = 220;
            ventanaGame.Y = (ventana.Height - Y) / 2;
            ventanaGame.Width = X;
            ventanaGame.Height = Y;

            //incluir el borde del tablero, q los enemigos y la nave puedan pisarlo (los enemigos las puertas solamente)


            //reproducir musica de juego
            Sonidos.PlayMusic(eMusica.Cancion1, opciones);


            //Inicializa objetos

            tooltip = new Tooltip(); //debe ir primero pq tiene delegados de eventos de MenuTorres y Nave

            menuTorres = new MenuTorres(ventana, ventanaGame, tooltip);

            fondo = new Fondo(ventanaGame);

            tablero = new Tablero(new Point(30, 26));

            waves = new Waves();

            enemigos = new Enemigos();

            torres = new Torres();

            nave = new Nave(ventanaGame, tooltip, partida);

            fase = new Fase(tablero, nave, partida);

            animaciones = new Animaciones();

            gameOver = new GameOver();

            marcador = new Marcador();

            frame = new Frame();

        }


        ////////////////// LOAD CONTENT //////////////////
        protected void LoadContent()
        {

            // TODO: use this.Content to load your game content here

            menuTorres.LoadContent(content);

            fondo.LoadContent(content);

            tablero.LoadContent(content);

            waves.LoadContent(content);

            enemigos.LoadContent(content);

            torres.LoadContent(content);

            nave.LoadContent(content);

            marcador.LoadContent(content);

            tooltip.LoadContent(content);

            frame.LoadContent(content);

        }


        ////////////////// UNLOAD CONTENT //////////////////
        public override void Deactivate()
        {
            base.Deactivate();
        }

        /*protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        */


        ////////////////// UPDATE //////////////////
        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);


            if (IsActive)
            {

                if (gameOver.Estado != ePartidaEstado.Jugando) //gameover
                {
                    // no permitir acciones del usuario

                    gameOver.Update(gameTime, nave, this, fase, enemigos, opciones, partida); //actualizar el gameover/victoria

                    animaciones.Update(gameTime);

                    frame.Update(gameTime); //actualizar el FPS

                }
                else
                {

                    ////////////////// INPUT //////////////////

                    previousKeyboardState = currentKeyboardState;
                    currentKeyboardState = Keyboard.GetState();
                    previousMouseState = currentMouseState;
                    currentMouseState = Mouse.GetState();


                    ////////////////// UPDATE LOGIC //////////////////

                    menuTorres.Update(gameTime, currentKeyboardState, previousKeyboardState, currentMouseState, previousMouseState, tablero, torres, enemigos, nave, tooltip, opciones);

                    marcador.Update(menuTorres, fase, tooltip);

                    tooltip.Update(gameTime, currentMouseState, nave, torres, menuTorres.SelectedTorre); //despues de MenuTorres y Marcador

                    fondo.Update();

                    fase.Update(gameTime, tablero, enemigos);

                    waves.Update(gameTime, fase);

                    enemigos.Update(gameTime, tablero, nave, torres, animaciones, opciones);

                    torres.Update(gameTime, tablero, nave, enemigos, animaciones, opciones);

                    nave.Update(gameTime, currentKeyboardState, previousKeyboardState, enemigos, opciones);

                    Colisiones.Update(gameTime, enemigos, nave, opciones, animaciones);

                    animaciones.Update(gameTime);

                    gameOver.Update(gameTime, nave, this, fase, enemigos, opciones, partida);

                    frame.Update(gameTime);

                }
            }
        }


        ////////////////// INPUT //////////////////
        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");


            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            
            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];


            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player))
            {
                ScreenManager.AddScreen(new PauseMenuScreen(opciones, partida), ControllingPlayer);
            }
        }


        
        ////////////////// DRAW //////////////////
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spb = ScreenManager.SpriteBatch;

            //dibujar solo en la ventana de juego (750*650)
            ScreenManager.GraphicsDevice.Viewport = ventanaGame;

            spb.Begin();

            fondo.Draw(spb);
            torres.Draw(spb, tablero);
            tablero.Draw(spb); //dibuja los bordes del tablero
            enemigos.Draw(spb);
            torres.disparos.Draw(spb, torres, tablero);
            nave.Draw(spb, gameOver);
            animaciones.Draw(spb);
            menuTorres.DrawTorreSel(spb, tablero, torres); //el circulo de la torre selecionada, q no se salga de la ventan de juego

            spb.End();


            //dibujar en la ventana del marcador de waves
            ScreenManager.GraphicsDevice.Viewport = new Viewport(waves.Ventana());
            spb.Begin();
            waves.Draw(spb);
            spb.End();


            //dibujar en la ventana total
            ScreenManager.GraphicsDevice.Viewport = ventana;
            spb.Begin();

            menuTorres.Draw(spb, currentMouseState, tablero, nave, torres, enemigos, tooltip);
            marcador.Draw(spb, nave, enemigos, menuTorres, fase, tooltip);
            tooltip.Draw(spb);
            frame.Draw(spb, gameTime);
            
            spb.End();
            //las screens de menu tienen su propio spriteBatch begin/end


            //screen GameOver
            if (gameOver.Estado == ePartidaEstado.GameOver)
            {
                gameOver.Draw(spb, ScreenManager);
            }


            //screens de menu. si la pantalla esta en transicion, oscurecerla
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


    }
}
