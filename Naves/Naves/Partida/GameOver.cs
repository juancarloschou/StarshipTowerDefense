
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;

namespace Naves
{
    enum ePartidaEstado
    {
        Jugando,
        GameOver,
        Victoria
    }


    class GameOver
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        private ePartidaEstado estado;

        private int estadoGameOver; //paso del gameover:
        //0= esperar x segundos, 1= oscurecer pantalla juego, (2= cambiar a pantalla de gameover)
        private TimeSpan pausaGameOver; //tiempo dura pausa en paso 0
        private TimeSpan previoTimeGameOver; //tiempo en q empieza paso 0
        //comunes a gameover y victoria (oscurecer pantalla)
        private float oscurecer; //alpha para paso 1 (alpha 0= transparente, 1= opaco)
        private TimeSpan tiempofadeOff; //tiempo dura el oscurecimiento en paso 1
        private TimeSpan previoFadeOff; //tiempo en q empieza paso 1


        ////////////////// PUBLICAS //////////////////
        public ePartidaEstado Estado
        {
            get { return estado; }
        }


        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public GameOver()
        {
            estado = ePartidaEstado.Jugando;
            pausaGameOver = TimeSpan.FromSeconds(1.25f); //tiempo dura pausa en paso 0 en gameover
            tiempofadeOff = TimeSpan.FromSeconds(1.25f);  //tiempo dura oscurecimiento en paso 1 gameover y en victoria
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// LOAD CONTENT //////////////////


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, Nave nave, GameScreen screen, Fase fase, Enemigos enemigos, Opciones opciones, Partida partida)
        {
            if (estado == ePartidaEstado.Jugando)
            {
                //controlar fin de partida (victoria o gameover)

                if (nave.Vida <= 0)
                {
                    estado = ePartidaEstado.GameOver;
 
                    //inicializa gameover
                    estadoGameOver = 0; //estado inicial

                    previoTimeGameOver = gameTime.TotalGameTime; //tiempo comienzo de la pausa
                }
 

                //si es ultima wave, ha terminado de salir enemigos, y hay cero enemigos vivos
                if ((fase.ActualWave == fase.waves.Count - 1) && (fase.waves[fase.ActualWave].Activa == eWaveActiva.Terminada))
                {
                    bool bVivo = false;
                    EnemigoBase enemigo;
                    for (int i = 0; (i < enemigos.enemigos.Count) && (!bVivo); i++)
                    {
                        enemigo = enemigos.enemigos[i];
                        if (!enemigo.bEliminar)
                            bVivo = true;
                    }

                    if (!bVivo)
                    {
                        estado = ePartidaEstado.Victoria;

                        //inicializa victoria
                        previoFadeOff = gameTime.TotalGameTime;
                    }
                }
            }

            else if (estado == ePartidaEstado.GameOver)
            {
                if (estadoGameOver == 0)
                {
                    //pausar unos seg para terminar sonido explosion nave

                    if (gameTime.TotalGameTime - previoTimeGameOver > pausaGameOver)
                    {
                        oscurecer = 0; //apha de la siguiente fase
                        previoFadeOff = gameTime.TotalGameTime;

                        estadoGameOver = 1;
                    }
                }
                else if (estadoGameOver == 1)
                {
                    //oscurecer pantalla

                    TimeSpan tiempoOscureciendo = (gameTime.TotalGameTime - previoFadeOff);
                    oscurecer = (float)(tiempoOscureciendo.TotalSeconds / tiempofadeOff.TotalSeconds);

                    if (oscurecer >= 1)
                    {
                        //-------gameover--------
                        partida.GameOver(nave.Puntos);

                        screen.ExitScreen(); //cierra pantalla actual de juego

                        screen.ScreenManager.AddScreen(new GameOverScreen(opciones, partida), screen.ControllingPlayer);
                    }
                }
            }

            else if (estado == ePartidaEstado.Victoria)
            {
                //oscurecer pantalla

                TimeSpan tiempoOscureciendo = (gameTime.TotalGameTime - previoFadeOff);
                oscurecer = (float)(tiempoOscureciendo.TotalSeconds / tiempofadeOff.TotalSeconds);

                if (oscurecer >= 1)
                {
                    //-------victoria--------
                    partida.CompletarPlaneta(partida.Planeta, nave.Puntos, nave.Dinero);

                    screen.ExitScreen(); //cierra pantalla actual de juego

                    screen.ScreenManager.AddScreen(new VictoriaScreen(opciones, partida), screen.ControllingPlayer);
                }
            }
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb, ScreenManager screen)
        {
            
            if (estado == ePartidaEstado.GameOver)
            {
                if (estadoGameOver == 1)
                {
                    //oscurecer pantalla lentamente
                    screen.FadeBackBufferToBlack(oscurecer);
                }
            }

            if (estado == ePartidaEstado.Victoria)
            {
                //oscurecer pantalla lentamente
                screen.FadeBackBufferToBlack(oscurecer);
            }
        }

    }
}
