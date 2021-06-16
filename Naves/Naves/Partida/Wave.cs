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
    enum eWaveActiva
    {
        SinEmpezar,
        EnMarcha, //saliendo enemigos de la wave
        Terminada //ya se han creado todos los enemigos de la wave
    }

    class Wave
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public int entrada; //id de la puerta
        public int salida;

        public int numEnemigos; //numero de enemigos de la wave
        public int countEnemigos; //cuenta los q van saliendo hasta llegar a numEnemigos
        public int nivel; //nivel de enemigos
        public eTipoEnemigo tipoEnemigo; //tipo de enemigos

        public TimeSpan enemigoRate; //frecuencia salen los enemigos por la puerta
        public TimeSpan previoEnemigoTime; //para calcular cuando debe salir un nuevo enemigo

        public TimeSpan waveStart; //tiempo en q empezará la wave (a partir de tiempoInicial)
        //public TimeSpan waveTime; //tiempo dura la wave (suele ser menor q el tiempo necesario q los enemigos lleguen al final)

        public eWaveActiva Activa; //si la wave está activa, si estan saliendo los enemigos
        //sin empezar, terminada, en marcha


        ////////////////// PUBLICAS //////////////////
        
        //duracion en segundos de los enemigos de la wave (durante cuanto tiempo salen enemigos)
        public float DuracionWave()
        {
            return numEnemigos * (float)enemigoRate.TotalSeconds;
        }

        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public Wave()
        {
            countEnemigos = 0;

            Activa = eWaveActiva.SinEmpezar;

            /*
            //inicializa la wave
            numEnemigos = 10;
            countEnemigos = 0;
            nivel = 0;

            enemigoRate = TimeSpan.FromSeconds(1.0f);
            waveStart = TimeSpan.FromSeconds(5f);

            tipoEnemigo = typeof(EnemigoTanque);

            this.entrada = Entrada;
            this.salida = Salida;
            */
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// LOAD CONTENT //////////////////


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, TimeSpan tiempoInicial, Fase fase, Enemigos enemigos, Tablero tablero)
        {
            //bool bEmpezo = false;

            //nuevos enemigos de la wave
            if (gameTime.TotalGameTime - tiempoInicial > waveStart) //empezó la wave
            {
                if (Activa == eWaveActiva.SinEmpezar)
                    Activa = eWaveActiva.EnMarcha;

                //bEmpezo = true;
                if (gameTime.TotalGameTime - previoEnemigoTime > enemigoRate) //tiempo de nuevo enemigo
                {
                    //si hay menos enemigos q el limite maximo
                    if (enemigos.enemigos.Count < enemigos.MaxEnemigos)
                    {
                        if (countEnemigos < numEnemigos)
                        {
                            //crea enemigo del tipo y nivel de la wave
                            enemigos.AddEnemigo(tipoEnemigo, tablero.puertas[entrada], tablero, nivel);
                            countEnemigos++;

                            // Reset our current time
                            previoEnemigoTime = gameTime.TotalGameTime;
                        }
                    }
                }

                if (Activa == eWaveActiva.EnMarcha)
                    if (countEnemigos == numEnemigos)
                        Activa = eWaveActiva.Terminada;
            }

            //return bEmpezo;
        }


        ////////////////// DRAW //////////////////

    }
}
