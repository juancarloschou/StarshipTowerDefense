#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Naves
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        #region Initialization


        private Opciones opciones;
        private Partida partida;


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen(Opciones opciones, Partida partida)
            : base("Paused")
        {
            this.opciones = opciones;
            this.partida = partida;


            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game");
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game");
            
            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            resumeGameMenuEntry.Selected += ResumeGameMenuEntrySelected; //aparte del onCancel generico necesito reanudar musica
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;



            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);


            Sonidos.PauseMusic();
            Sonidos.PlayFX(eSonido.Pausa, opciones);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Resume Game menu entry is selected.
        /// </summary>
        void ResumeGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            Sonidos.ResumeMusic();
        }


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen(),
            //                                                        new MainMenuScreen(opciones, partida));

            //Sonidos.PlayMusic(eMusica.CancionTitulo, opciones);


            LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new SistemaSolarScreen(opciones, partida));

            
            /*
            const string message = "Are you sure you want to quit this game?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
            */
        }


        /*
        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, e.PlayerIndex, new BackgroundScreen(),
                                                                    new MainMenuScreen());

            Sonidos.PlayMusic(eMusica.CancionTitulo);
        }
        */

        #endregion
    }
}
