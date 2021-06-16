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
    class DibujaWave
    {
        public Vector2 wave; //figura q representa el periodo de actividad de la wave (X es posicion superior izq, Y es posicion superior dcha)
        public bool bInicio; //indica si es la wave inicial, antes de empezar la primera wave
        public bool waveVisible;
        public Vector2 waveTexto; //posicion para calcular el centro del texto (permite x negativa)

        public Vector2 calma; //figura q representa el periodo de calma entre la wave y la siguiente
        public bool bCalma; //indica si hay periodo de calma (ultima wave)
        public bool calmaVisible;

        public int num; //numero de wave
    }


    class Waves
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public Vector2 posicionVentana = new Vector2(220, 720); //posicion de la ventana
        public const int ANCHO = 750;
        public const int ALTURA = 25;
        private Vector2 posicion = new Vector2(0, 0); //posicion inicial dentro de la ventana

        private TimeSpan tiempoPeriodo = TimeSpan.FromSeconds(100); //tiempo q se muestra en waves (si hay 10 waves, y 6 waves son en el periodo, se muestran las 6)
        private float pixelSegundo; //lo q ocupa 1 segundo de duracion de wave en pixeles

        private List<DibujaWave> dibujaWaves = new List<DibujaWave>();

        private static Sprite sprite = new Sprite();
        private SpriteFont font;


        ////////////////// CONSTRUCTOR //////////////////
        public Waves()
        {
            pixelSegundo = ANCHO / (float)tiempoPeriodo.TotalSeconds;
        }

        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public Rectangle Ventana()
        {
            return new Rectangle((int)posicionVentana.X, (int)posicionVentana.Y, ANCHO, ALTURA);
        }

        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("Fuentes\\Waves");

            sprite.imagen = content.Load<Texture2D>("Sprites\\blank");
            sprite.dimImagen = new Point(4, 4);
            sprite.posImagen = new Point(0, 0);
        }


        ////////////////// UPDATE ////////////////////////

        private void ComprobarWave(ref Vector2 wave, ref bool bSalir)
        {
            if (wave.X + wave.Y >= posicion.X + ANCHO)
            {
                wave.Y = posicion.X + ANCHO - wave.X;
                bSalir = true;
            }
        }

        public void Update(GameTime gametime, Fase fase)
        {
            Vector2 pos = new Vector2(posicion.X, posicion.Y);
            dibujaWaves = new List<DibujaWave>();
            DibujaWave dibujaWave;
            float ancho, duracion, anchoTotal;
            bool bSalir = false;
            for(int iWave = fase.ActualWave; (iWave < fase.waves.Count) && !bSalir; iWave++)
            {
                if (iWave == -1) //antes de empezar primera wave
                {
                    dibujaWave = new DibujaWave();
                    dibujaWave.bInicio = true;
                    dibujaWave.bCalma = false;
                    duracion = (float)fase.waves[iWave + 1].waveStart.TotalSeconds;
                    duracion -= (float)(gametime.TotalGameTime - fase.TiempoInicial).TotalSeconds;
                    ancho = (float)Math.Round(pixelSegundo * duracion);
                    if (ancho < 0)
                        ancho = 0;
                    dibujaWave.wave = new Vector2(pos.X, ancho);
                    pos += new Vector2(ancho, 0);
                    ComprobarWave(ref dibujaWave.wave, ref bSalir);
                }
                else
                {

                    Wave wave = fase.waves[iWave];

                    dibujaWave = new DibujaWave();
                    dibujaWave.bInicio = false;
                    dibujaWave.num = iWave + 1; //se muestra la wave + 1
                    duracion = wave.DuracionWave();
                    if(iWave == fase.ActualWave)
                        duracion -= (float)(gametime.TotalGameTime - fase.TiempoInicial - wave.waveStart).TotalSeconds;
                    ancho = (float)Math.Round(pixelSegundo * duracion);
                    if (ancho <= 0)
                    {
                        ancho = 0;
                        dibujaWave.waveVisible = false;
                    }
                    else
                    {
                        anchoTotal = pixelSegundo * wave.DuracionWave();
                        dibujaWave.waveTexto.X = pos.X + (ancho - anchoTotal);
                        dibujaWave.waveTexto.Y = anchoTotal;
                        dibujaWave.waveVisible = true;
                    }
                    dibujaWave.wave = new Vector2(pos.X, ancho);
                    pos += new Vector2(ancho, 0);
                    ComprobarWave(ref dibujaWave.wave, ref bSalir);


                    if ((iWave < fase.waves.Count - 1) && !bSalir) //si no es la ultima wave hay periodo calma
                    {
                        dibujaWave.bCalma = true;
                        duracion = (float)(fase.waves[iWave + 1].waveStart - fase.waves[iWave].waveStart).TotalSeconds - wave.DuracionWave();
                        if ((iWave == fase.ActualWave) && !dibujaWave.waveVisible)
                            duracion -= (float)(gametime.TotalGameTime - fase.TiempoInicial - wave.waveStart).TotalSeconds - wave.DuracionWave();
                        ancho = (float)Math.Round(pixelSegundo * duracion);
                        if (ancho <= 0)
                        {
                            ancho = 0;
                            dibujaWave.calmaVisible = false;
                        }
                        else
                            dibujaWave.calmaVisible = true;
                        dibujaWave.calma = new Vector2(pos.X, ancho);
                        pos += new Vector2(ancho, 0);
                        ComprobarWave(ref dibujaWave.calma, ref bSalir);
                    }
                    else
                    {
                        dibujaWave.bCalma = false;
                    }
                }

                dibujaWaves.Add(dibujaWave);
            }
            
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb)
        {
            string texto;
            Vector2 posTexto;
            Rectangle rectangulo;
            foreach(DibujaWave dibujaWave in dibujaWaves)
            {
                if (dibujaWave.bInicio)
                {
                    //recuadro antes la primera wave
                    Color colorWave = Color.DarkGray;
                    rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, (int)dibujaWave.wave.Y, (int)ALTURA);
                    spb.Draw(sprite.imagen, new Vector2(dibujaWave.wave.X, posicion.Y), rectangulo, colorWave);
                }
                else
                {
                    if (dibujaWave.waveVisible)
                    {
                        //recuadro de la wave
                        Color colorWave = Color.Orange;
                        rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, (int)dibujaWave.wave.Y, (int)ALTURA);
                        spb.Draw(sprite.imagen, new Vector2(dibujaWave.wave.X, posicion.Y), rectangulo, colorWave);

                        //centrar texto en recuadro
                        texto = dibujaWave.num.ToString();
                        posTexto = new Vector2(dibujaWave.waveTexto.X + dibujaWave.waveTexto.Y / 2 - font.MeasureString(texto).X / 2,
                                               posicion.Y + ALTURA / 2 - font.MeasureString(texto).Y / 2);
                        spb.DrawString(font, texto, posTexto, Color.Blue);
                    }

                    //recuadro de la calma
                    if (dibujaWave.bCalma && dibujaWave.calmaVisible)
                    {
                        Color colorCalma = Color.Olive;
                        rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, (int)dibujaWave.calma.Y, (int)ALTURA);
                        spb.Draw(sprite.imagen, new Vector2(dibujaWave.calma.X, posicion.Y), rectangulo, colorCalma);
                    }
                }
            }

        }

    }
}
