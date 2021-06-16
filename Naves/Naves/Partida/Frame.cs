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
    class Frame
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        private Vector2 posicion = new Vector2(0, 0);

        //Update Por Segundo
        public float UPS; 
        private float elapseTimeUPS = 0;
        private int frameCounterUPS = 0;
        //private Vector2 posicionUPS = new Vector2(100, 0);

        //Draw Por Segundo
        public float DPS; 
        private float elapseTimeDPS = 0;
        private int frameCounterDPS = 0;
        //private Vector2 posicionDPS = new Vector2(100, 0);

        //tiempo de juego
        //private Vector2 posicionTime = new Vector2(200, 0);

        //fuentes
        private SpriteFont font; //solo con caracteres para poner FPS
        private SpriteFont fontTime;

            
        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager Content)
        {
            font = Content.Load<SpriteFont>("Fuentes\\FrameRateCounter");
            fontTime = Content.Load<SpriteFont>("Fuentes\\Marcador");
        }


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime)
        {
            elapseTimeUPS += (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounterUPS++;

            if (elapseTimeUPS >= 1)
            {
                UPS = frameCounterUPS / elapseTimeUPS;
                frameCounterUPS = 0;
                elapseTimeUPS = 0;
            }
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb, GameTime gameTime)
        {

            elapseTimeDPS += (float)gameTime.ElapsedGameTime.TotalSeconds;
            frameCounterDPS++;

            if (elapseTimeDPS >= 1)
            {
                DPS = frameCounterDPS / elapseTimeDPS;
                frameCounterDPS = 0;
                elapseTimeDPS = 0;
            }

        
            /*Vector2 posicionUPS = new Vector2(posicion.X, posicion.Y);
            spb.DrawString(font, "UPS " + ((int)UPS).ToString(), posicionUPS, Color.Black);
            spb.DrawString(font, "UPS " + ((int)UPS).ToString(), new Vector2(posicionUPS.X + 1, posicionUPS.Y + 1), Color.White);*/

            Vector2 posicionDPS = new Vector2(posicion.X, posicion.Y);
            spb.DrawString(font, "DPS " + ((int)DPS).ToString(), posicionDPS, Color.Black);
            spb.DrawString(font, "DPS " + ((int)DPS).ToString(), new Vector2(posicionDPS.X + 1, posicionDPS.Y + 1), Color.White);

            Vector2 posicionTime = new Vector2(posicion.X + 90, posicion.Y);
            spb.DrawString(fontTime, gameTime.TotalGameTime.ToString(@"hh\:mm\:ss\.FFF"), posicionTime, Color.Black);
            spb.DrawString(fontTime, gameTime.TotalGameTime.ToString(@"hh\:mm\:ss\.FFF"), new Vector2(posicionTime.X + 1, posicionTime.Y + 1), Color.White);
            

            //Console.WriteLine(gameTime.TotalGameTime.ToString(@"hh\:mm\:ss\.FFF"));
        }

    }
}
