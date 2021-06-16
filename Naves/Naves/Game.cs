using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameStateManagement;

//using System.IO;
//using Microsoft.Xna.Framework.Storage;
//using System.Runtime.InteropServices;

namespace Naves
{
    /// <summary>
    /// This is the main type for your game
    /// 
    /// http://codecrab.blogspot.com/2010/07/programando-videojuegos-tutorial-xna.html
    /// 
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        GraphicsDeviceManager graphics;
        //SpriteBatch spriteBatch;

        //GameStateManagement (Windows): manager de menus: pantallas, estados, inputs
        ScreenManager screenManager;
        ScreenFactory screenFactory;

        //Idioma idioma;
        Opciones opciones;
        Partida partida;

        //pulsacion de teclas
        Boolean bTeclaPantallaCompleta = true; //espera a q se suelte la tecla para pulsarla de nuevo
        

        ////////////////// CONSTRUCTOR //////////////////
        public Game()
        {
            /*string path = DatosFaseWave.GetApplicationPath() +"Fases.xml";
            //string ruta = DatosFaseWave.GetApplicationPath() + "Fases.xml";
            //MessageBox(new IntPtr(0), string.Format("ruta:{0}, path:{1} ", ruta, path), "MessageBox title", 0);

            string nombre = "Fases.xml";
            //string path2 = Path.Combine(StorageContainer.TitleLocation, nombre);
            Stream stream = TitleContainer.OpenStream(nombre);
            string path2 = stream.ToString();

            string ruta = "D:\\Naves\\path.txt";
            StreamWriter sw = new StreamWriter(ruta);
            sw.WriteLine("path = " + path);
            //sw.WriteLine("path2 = " + path2);
            sw.Close();*/



            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            //graphics.PreferredBackBufferWidth = 800;
            //graphics.PreferredBackBufferHeight = 600;
            //graphics.ApplyChanges(); //si cambiamos resolucion fuera del constructor
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;


            //IsFixedTimeStep = true; //por defecto true
            //TargetElapsedTime = TimeSpan.FromMilliseconds(10);


            //añade componente "fps" -> No funciona con el ScreenManager
            //Components.Add(new FrameRateCounter(this));


            //idioma = new Idioma();
            opciones = new Opciones();
            partida = new Partida();


            //necesito q los sonidos se carguen antes q el mainmenu
            Sonidos.LoadContent(Content);


            // Create the screen factory and add it to the Services
            screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager component.
            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            AddInitialScreens();

        }

        private void AddInitialScreens()
        {
            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);

            screenManager.AddScreen(new MainMenuScreen(opciones, partida), null);
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// INICIALIZAR //////////////////
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.Window.Title = "Juego Naves";


            //muestra la resolucion de pantalla
            //Console.WriteLine(graphics.PreferredBackBufferWidth + "*" + graphics.PreferredBackBufferHeight);

            IsMouseVisible = true;


            base.Initialize();
        }


        ////////////////// LOAD CONTENT //////////////////
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //solo inicializan una vez, lo pongo en el game load content

            //necesito q los sonidos se carguen antes q el mainmenu
            //Sonidos.LoadContent(Content);

            AnimacionesSprites.LoadContent(Content);

        }


        ////////////////// UPDATE ////////////////////////
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.X))
            {
                this.Exit();
            }

            //modo pantalla compelta
            if (Keyboard.GetState().IsKeyDown(Keys.LeftAlt) && Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (bTeclaPantallaCompleta)
                {
                    graphics.ToggleFullScreen();
                    //graphics.ApplyChanges();
                    bTeclaPantallaCompleta = false;
                }
            }
            else
            {
                //espera a q se suelten las teclas para poder pulsarlas de nuevo
                bTeclaPantallaCompleta = true;
            }


            base.Update(gameTime);
        }


        ////////////////// DRAW //////////////////
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }


    }
}
