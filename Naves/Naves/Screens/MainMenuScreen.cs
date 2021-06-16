#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
#endregion

namespace Naves
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        private bool bCargarOpciones;
        private bool bCargarPartida;

        private Opciones opciones;
        private Partida partida;

        MenuEntry newGameGameMenuEntry;
        MenuEntry continueGameGameMenuEntry;
        MenuEntry optionsMenuEntry;
        MenuEntry exitMenuEntry;

        private bool bExisteContinuar = false;
        private int volverDeOpciones = 0;


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen(Opciones opciones, Partida partida)
            : base(String.Empty)
        {
            this.opciones = opciones;
            this.partida = partida;


            //carga de opciones----------
            //solo en la 1ª ejecucion del programa deberia cargarse de disco duro
            //esa carga podria ser en game() o en pantalla previa de presentacion juego (solo se veria la 1ª ejecucion)
            //por ahora cargo siempre de disco duro, pero si ya esta cargado no vuelve a cargarlo
            bool bCargandoOpciones = opciones.Load();


            if (bCargandoOpciones)
            {
                bCargarPartida = false;
                bCargarOpciones = true;
            }
            else
            {
                bool bCargandoPartida = partida.Load();

                //carga de partida---------
                if (bCargandoPartida)
                {
                    bCargarPartida = true;
                    bCargarOpciones = false;
                }
                else
                {
                    bCargarPartida = false;
                    bCargarOpciones = false;
                    bExisteContinuar = false;
                    SetMenuEntryText();
                }
            }

            // Create our menu entries.
            newGameGameMenuEntry = new MenuEntry(string.Empty);
            continueGameGameMenuEntry = new MenuEntry(string.Empty);
            optionsMenuEntry = new MenuEntry(string.Empty);
            exitMenuEntry = new MenuEntry(string.Empty);

            // Hook up menu event handlers.
            newGameGameMenuEntry.Selected += NewGameMenuEntrySelected;
            continueGameGameMenuEntry.Selected += ContinueGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(newGameGameMenuEntry);
            //MenuEntries.Add(continueGameGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);

        }

        #endregion


        #region Update

        void SetMenuEntryText()
        {
            MenuTitle = Recursos.Menu_Principal;
            newGameGameMenuEntry.Text = Recursos.Nueva_Partida;
            continueGameGameMenuEntry.Text = Recursos.Continuar_Partida;
            optionsMenuEntry.Text = Recursos.Opciones;
            exitMenuEntry.Text = Recursos.Salir;

            if(bExisteContinuar)
            {
                MenuEntries.Clear();
                MenuEntries.Add(newGameGameMenuEntry);
                MenuEntries.Add(continueGameGameMenuEntry);
                MenuEntries.Add(optionsMenuEntry);
                MenuEntries.Add(exitMenuEntry);
            }
            else
            {
                MenuEntries.Clear();
                MenuEntries.Add(newGameGameMenuEntry);
                MenuEntries.Add(optionsMenuEntry);
                MenuEntries.Add(exitMenuEntry);
            }

        }

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (volverDeOpciones == 1 && !otherScreenHasFocus)
            {
                volverDeOpciones = 0;
                SetMenuEntryText(); //posible cambio de idioma

                if (opciones.bSave)
                {
                    opciones.bSave = false;
                    bCargarOpciones = true;
                }
            }


            if(bCargarOpciones)
                opciones.Update(); //va haciendo la carga o el guardado

            if (bCargarPartida)
                partida.Update(); //va haciendo la carga o el guardado


            if (opciones.FinishLoad()) //carga opciones ha finalizado
            {
                //reproducir musica de menu (hay q esperar por las opciones)
                Sonidos.PlayMusic(eMusica.CancionTitulo, opciones);

                System.Console.WriteLine("carga de opciones finalizada");

                bool bCargandoPartida = partida.Load();

                //carga de partida---------
                if (bCargandoPartida)
                {
                    bCargarPartida = true;
                    bCargarOpciones = false;
                }
                else
                {
                    bCargarPartida = false;
                    bCargarOpciones = false;
                    bExisteContinuar = false;
                    SetMenuEntryText(); //posible cambio de idioma
                }

            }

            if (opciones.FinishSave()) //guardado opciones ha finalizado
            {
                System.Console.WriteLine("guardado de opciones finalizada");

                bCargarOpciones = false;
            }


            if (partida.FinishLoad()) //carga partida ha finalizado
            {
                bExisteContinuar = partida.ExisteLoad;

                System.Console.WriteLine("carga de partida finalizada");

                SetMenuEntryText(); //posible cambio de continuar partida

                bCargarPartida = false;
            }

            if (partida.FinishSave()) //guardado partida ha finalizado
            {
                System.Console.WriteLine("guardado de partida finalizada");

                bCargarPartida = false;
            }

        }

        #endregion


        #region Handle Input

        /// <summary>
        /// Event handler for when the New Game menu entry is selected.
        /// </summary>
        void NewGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
            //                   new GameOverScreen(opciones));
            //                   new GameTorresScreen(idioma, opciones));

            //test!!!!!!! completado el planeta mercurio
            //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
            //                   new SistemaSolarScreen(opciones, partida, 1));

            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new SistemaSolarScreen(opciones, partida));
        }


        /// <summary>
        /// Event handler for when the New Game menu entry is selected.
        /// </summary>
        void ContinueGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
            //                   new GameOverScreen(opciones));
            //                   new GameTorresScreen(idioma, opciones));

            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new SistemaSolarScreen(opciones, partida));

        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            volverDeOpciones = 1;
            ScreenManager.AddScreen(new OptionsMenuScreen(opciones), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            /*
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
            */

            ScreenManager.Game.Exit();
        }


        /*
        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
        */

        #endregion
    }
}
