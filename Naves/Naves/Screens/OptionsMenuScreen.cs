#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
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
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry idiomaMenuEntry;
        MenuEntry sonidoMenuEntry;
        MenuEntry volumenFXMenuEntry;
        MenuEntry volumenMusicaMenuEntry;
        MenuEntry backMenuEntry;


        //private Idioma idioma;
        private Opciones opciones;


        private Idioma _idioma;
        private bool _sonido = true;
        private byte _volumenFX = 50; //1-100
        private byte _volumenMusica = 50; //1-100


        private byte avanceVolumen = 5;

        #endregion


        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen(Opciones Opciones)
            : base(string.Empty)
        {
            //this.idioma = Idioma;
            this.opciones = Opciones;


            //cargo desde la clase opciones
            _idioma = new Idioma(opciones.OpcionesDatos.idioma);
            _sonido = opciones.OpcionesDatos.sonido;
            _volumenFX = opciones.OpcionesDatos.volumenFx;
            _volumenMusica = opciones.OpcionesDatos.volumenMusica;


            // Create our menu entries.
            idiomaMenuEntry = new MenuEntry(string.Empty);
            sonidoMenuEntry = new MenuEntry(string.Empty);
            volumenFXMenuEntry = new MenuEntry(string.Empty);
            volumenMusicaMenuEntry = new MenuEntry(string.Empty);
            backMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();


            // Hook up menu event handlers.
            idiomaMenuEntry.Selected += IdiomaMenuEntrySelected;
            idiomaMenuEntry.Left += IdiomaMenuEntryLeft;
            idiomaMenuEntry.Right += IdiomaMenuEntrySelected;

            sonidoMenuEntry.Selected += SonidoMenuEntrySelected;
            sonidoMenuEntry.Left += SonidoMenuEntrySelected;
            sonidoMenuEntry.Right += SonidoMenuEntrySelected;

            volumenFXMenuEntry.Selected += VolumenFXMenuEntrySelected;
            volumenFXMenuEntry.Left += VolumenFXMenuEntryLeft;
            volumenFXMenuEntry.Right += VolumenFXMenuEntrySelected;

            volumenMusicaMenuEntry.Selected += VolumenMusicaMenuEntrySelected;
            volumenMusicaMenuEntry.Left += VolumenMusicaMenuEntryLeft;
            volumenMusicaMenuEntry.Right += VolumenMusicaMenuEntrySelected;

            backMenuEntry.Selected += OnCancel;
            backMenuEntry.Selected += BackEntrySelected;

            
            // Add entries to the menu.
            MenuEntries.Add(idiomaMenuEntry);
            MenuEntries.Add(sonidoMenuEntry);
            MenuEntries.Add(volumenFXMenuEntry);
            MenuEntries.Add(volumenMusicaMenuEntry);
            MenuEntries.Add(backMenuEntry);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            MenuTitle = Recursos.Opciones;
            idiomaMenuEntry.Text = Recursos.Idioma + ": " + _idioma.toString().Replace('ñ', 'n');
            sonidoMenuEntry.Text = Recursos.Sonido + ": " + (_sonido ? Recursos.On : Recursos.Off);
            volumenFXMenuEntry.Text = Recursos.Volumen_FX + ": " + _volumenFX;
            volumenMusicaMenuEntry.Text = Recursos.Volumen_Musica + ": " + _volumenMusica;
            backMenuEntry.Text = Recursos.Atras;
        }

        #endregion


        #region Handle Input (Events)

        /// <summary>
        /// guardar las opciones
        /// </summary>
        void BackEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            opciones.OpcionesDatos.idioma = _idioma.idioma;
            opciones.OpcionesDatos.sonido = _sonido;
            opciones.OpcionesDatos.volumenFx = _volumenFX;
            opciones.OpcionesDatos.volumenMusica = _volumenMusica;

            //guardar en disco duro
            opciones.Save();
        }


        /// <summary>
        /// idioma
        /// </summary>
        void IdiomaMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _idioma.SiguienteIdioma();

            opciones.OpcionesDatos.idioma = _idioma.idioma;
            Idioma.SetIdioma(_idioma.idioma);
            SetMenuEntryText();
        }

        /// <summary>
        /// idioma
        /// </summary>
        void IdiomaMenuEntryLeft(object sender, PlayerIndexEventArgs e)
        {
            _idioma.AnteriorIdioma();

            opciones.OpcionesDatos.idioma = _idioma.idioma;
            Idioma.SetIdioma(_idioma.idioma);
            SetMenuEntryText();
        }


        /// <summary>
        /// Sonido
        /// </summary>
        void SonidoMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _sonido = !_sonido;

            SetMenuEntryText();

            opciones.OpcionesDatos.sonido = _sonido;
            if (_sonido)
            {
                Sonidos.PlayOrResumeMusic(eMusica.CancionTitulo, opciones);
            }
            else
            {
                Sonidos.PauseMusic();
            }
        }


        /// <summary>
        /// Volumen FX, de 0 a 100, aumenta en avanceVolumen
        /// </summary>
        void VolumenFXMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _volumenFX += avanceVolumen;

            if (_volumenFX > 100)
                _volumenFX = 100;

            SetMenuEntryText();

            opciones.OpcionesDatos.volumenFx = _volumenFX;
            Sonidos.PlayFX(eSonido.Laser1, opciones);
        }

        /// <summary>
        /// Volumen FX, de 0 a 100, disminuye en avanceVolumen
        /// </summary>
        void VolumenFXMenuEntryLeft(object sender, PlayerIndexEventArgs e)
        {
            if (_volumenFX - avanceVolumen < 0)
                _volumenFX = 0;
            else
                _volumenFX -= avanceVolumen;

            SetMenuEntryText();

            opciones.OpcionesDatos.volumenFx = _volumenFX;
            Sonidos.PlayFX(eSonido.Laser1, opciones);
        }


        /// <summary>
        /// Volumen Musica, de 0 a 100, aumenta en avanceVolumen
        /// </summary>
        void VolumenMusicaMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _volumenMusica += avanceVolumen;

            if (_volumenMusica > 100)
                _volumenMusica = 100;

            SetMenuEntryText();

            opciones.OpcionesDatos.volumenMusica = _volumenMusica;
            Sonidos.SetVolumen(opciones);
        }

        /// <summary>
        /// Volumen Musica, de 0 a 100, disminuye en avanceVolumen
        /// </summary>
        void VolumenMusicaMenuEntryLeft(object sender, PlayerIndexEventArgs e)
        {
            if (_volumenMusica - avanceVolumen < 0)
                _volumenMusica = 0;
            else
                _volumenMusica -= avanceVolumen;

            SetMenuEntryText();

            opciones.OpcionesDatos.volumenMusica = _volumenMusica;
            Sonidos.SetVolumen(opciones);
        }

        #endregion
    }
}
