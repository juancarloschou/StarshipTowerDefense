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

    class Fase
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public ePlaneta planeta;
        public int dineroInicial; //dinero con el que enmpieza la nave

        public List<Wave> waves = new List<Wave>();
        private int actualWave = 0;

        public List<Puerta> puertas = new List<Puerta>();

        private TimeSpan tiempoSigWave; //tiempo q falta para la siguiente wave (reloj marcador)
       
        private TimeSpan tiempoInicial; //tiempo en q empieza el juego (termina menu y loading), para calcular waveStart
        private static TimeSpan TIEMPOMAX = TimeSpan.MaxValue; //constante para empezar la cuenta de waveStart


        ////////////////// PUBLICAS //////////////////
        public TimeSpan TiempoSiguienteWave
        {
            get { return tiempoSigWave; }
        }

        public TimeSpan TiempoInicial
        {
            get { return tiempoInicial; }
        }

        public int ActualWave
        {
            get { return actualWave; }
        }


        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public Fase(Tablero tablero, Nave nave, Partida partida)
        {
            //tiempos para cambio de waves
            tiempoInicial = TIEMPOMAX;




/*
            //**** crear waves ****

            DatosFases dfs = new DatosFases();


            ////////////////// MERCURIO /////////////////
            this.planeta = ePlaneta.Mercurio;

            //las puertas del tablero se deciden en la fase
            puertas = new List<Puerta>();

            Puerta puertaE, puertaS;
            int tamPuerta = 6; //tamaño de las puertas
            //puerta entrada arriba izquierda
            puertaE = new Puerta();
            puertaE.bEntrada = true;
            puertaE.coordTopLeft = new Point(1, 0);
            puertaE.coordBottomRight = new Point(tamPuerta, 0);
            puertaE.idPuerta = 0;
            puertas.Add(puertaE);

            //puerta salida abajo centro
            puertaS = new Puerta();
            puertaS.bEntrada = false;
            puertaS.coordTopLeft = new Point((int)(tablero.dimTablero.X / 2 - (int)(tamPuerta / 2)), tablero.dimTablero.Y - 1);
            puertaS.coordBottomRight = new Point((int)(tablero.dimTablero.X / 2 - (int)(tamPuerta / 2) + (tamPuerta - 1)), tablero.dimTablero.Y - 1);
            puertaS.idPuerta = 1;
            puertas.Add(puertaS);

            
            //pasa las puertas de la fase al tablero
            tablero.puertas = puertas;
            tablero.TableroBool(null);


            //carga waves
            this.waves.Clear();
            LoadWaves(puertaE.idPuerta, puertaS.idPuerta, 5);
            actualWave = -1;


            //dinero inicial nave
            dineroInicial = 200;
            nave.SetDineroInicial(dineroInicial);


            // guardar en DatosFaseWave
            DatosFaseWave.SaveFase(this, dfs);



            /////////////////// VENUS /////////////////

            this.planeta = ePlaneta.Venus;

            //las puertas del tablero se deciden en la fase
            puertas = new List<Puerta>();

            //puerta entrada arriba derecha
            puertaE = new Puerta();
            puertaE.bEntrada = true;
            puertaE.coordTopLeft = new Point(tablero.dimTablero.X - (tamPuerta), 0);
            puertaE.coordBottomRight = new Point(tablero.dimTablero.X - 1, 0);
            puertaE.idPuerta = 0;
            puertas.Add(puertaE);

            //puerta salida abajo centro
            puertaS = new Puerta();
            puertaS.bEntrada = false;
            puertaS.coordTopLeft = new Point((int)(tablero.dimTablero.X / 2 - (int)(tamPuerta / 2)), tablero.dimTablero.Y - 1);
            puertaS.coordBottomRight = new Point((int)(tablero.dimTablero.X / 2 - (int)(tamPuerta / 2) + (tamPuerta - 1)), tablero.dimTablero.Y - 1);
            puertaS.idPuerta = 1;
            puertas.Add(puertaS);


            //pasa las puertas de la fase al tablero
            tablero.puertas = puertas;
            tablero.TableroBool(null);


            //carga waves
            this.waves.Clear();
            LoadWaves(puertaE.idPuerta, puertaS.idPuerta, 10);
            actualWave = -1;


            //dinero inicial nave
            dineroInicial = 400;
            nave.SetDineroInicial(dineroInicial);


            // guardar en DatosFaseWave
            DatosFaseWave.SaveFase(this, dfs);



            /////////////////// TIERRA /////////////////

            this.planeta = ePlaneta.Tierra;

            //las puertas del tablero se deciden en la fase
            puertas = new List<Puerta>();

            //puerta entrada arriba derecha
            puertaE = new Puerta();
            puertaE.bEntrada = true;
            puertaE.coordTopLeft = new Point(tablero.dimTablero.X - (tamPuerta), -1);
            puertaE.coordBottomRight = new Point(tablero.dimTablero.X - 1, -1);
            puertaE.idPuerta = 0;
            puertas.Add(puertaE);

            //puerta salida abajo izquierda
            puertaS = new Puerta();
            puertaS.bEntrada = false;
            puertaS.coordTopLeft = new Point(1, tablero.dimTablero.Y - 1);
            puertaS.coordBottomRight = new Point(tamPuerta, tablero.dimTablero.Y - 1);
            puertaS.idPuerta = 1;
            puertas.Add(puertaS);


            //pasa las puertas de la fase al tablero
            tablero.puertas = puertas;
            tablero.TableroBool(null);


            //carga waves
            this.waves.Clear();
            LoadWaves(puertaE.idPuerta, puertaS.idPuerta, 15);
            actualWave = -1;


            //dinero inicial nave
            dineroInicial = 600;
            nave.SetDineroInicial(dineroInicial);


            // guardar en DatosFaseWave
            DatosFaseWave.SaveFase(this, dfs);



            
            // guardar en xml ---------------
            DatosFaseWave.SaveFasesToXML(dfs);

            //**** crear waves ****
 */




            DatosFaseWave.LoadFases(this, partida.Planeta);

            nave.SetDineroInicial(dineroInicial);
            tablero.puertas = puertas;
            tablero.TableroBool(null);

        }


        private void LoadWaves(int Entrada, int Salida, int num)
        {

            TimeSpan startFase = TimeSpan.FromSeconds(15f); // tiempo para construir
            //TimeSpan startFase = TimeSpan.FromSeconds(5f); // tiempo para construir
            TimeSpan tiempo = startFase; //contador
            Wave antesWave;


            //inicializa la wave 0
            Wave wave = new Wave();

            wave.numEnemigos = 10;
            wave.nivel = 0;

            wave.enemigoRate = TimeSpan.FromSeconds(1.5f);
            tiempo += TimeSpan.FromSeconds(0f);
            wave.waveStart = tiempo;
            //dura 10 segs

            wave.tipoEnemigo = eTipoEnemigo.Garras; //eTipoEnemigo.CazaMercurio;

            wave.entrada = Entrada;
            wave.salida = Salida;

            antesWave = wave;
            this.waves.Add(wave);


            //inicializa la wave 1
            wave = new Wave();

            wave.numEnemigos = 10;
            wave.nivel = 0;

            wave.enemigoRate = TimeSpan.FromSeconds(1.5f);
            tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 10);
            wave.waveStart = tiempo;
            //dura 15 segs

            wave.tipoEnemigo = eTipoEnemigo.Robot;

            wave.entrada = Entrada;
            wave.salida = Salida;

            antesWave = wave;
            this.waves.Add(wave);


            //inicializa la wave 2
            wave = new Wave();

            wave.numEnemigos = 7;
            wave.nivel = 0;

            wave.enemigoRate = TimeSpan.FromSeconds(2.5f);
            tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 10);
            wave.waveStart = tiempo;
            //dura 12.5 segs

            wave.tipoEnemigo = eTipoEnemigo.Helicoptero;

            wave.entrada = Entrada;
            wave.salida = Salida;

            antesWave = wave;
            this.waves.Add(wave);


            //inicializa la wave 3
            wave = new Wave();

            wave.numEnemigos = 20;
            wave.nivel = 0;

            wave.enemigoRate = TimeSpan.FromSeconds(1.5f);
            tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 10);
            wave.waveStart = tiempo;
            //dura 30 segs

            wave.tipoEnemigo = eTipoEnemigo.Tanque;

            wave.entrada = Entrada;
            wave.salida = Salida;

            antesWave = wave;
            this.waves.Add(wave);


            //inicializa la wave 4
            wave = new Wave();

            wave.numEnemigos = 15;
            wave.nivel = 1;

            wave.enemigoRate = TimeSpan.FromSeconds(2.5f);
            tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 15);
            wave.waveStart = tiempo;
            //dura 25 segs

            wave.tipoEnemigo = eTipoEnemigo.Helicoptero;

            wave.entrada = Entrada;
            wave.salida = Salida;

            antesWave = wave;
            this.waves.Add(wave);



            if (num > 5)
            {
                //inicializa la wave 5
                wave = new Wave();

                wave.numEnemigos = 30;
                wave.nivel = 2;

                wave.enemigoRate = TimeSpan.FromSeconds(1.5f);
                tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 10);
                wave.waveStart = tiempo;
                //dura 30 segs

                wave.tipoEnemigo = eTipoEnemigo.Tanque;

                wave.entrada = Entrada;
                wave.salida = Salida;

                antesWave = wave;
                this.waves.Add(wave);


                //inicializa la wave 6
                wave = new Wave();

                wave.numEnemigos = 40;
                wave.nivel = 3;

                wave.enemigoRate = TimeSpan.FromSeconds(1.5f);
                tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 15);
                wave.waveStart = tiempo;
                //dura 60 segs

                wave.tipoEnemigo = eTipoEnemigo.Robot;

                wave.entrada = Entrada;
                wave.salida = Salida;

                antesWave = wave;
                this.waves.Add(wave);


                //inicializa la wave 7
                wave = new Wave();

                wave.numEnemigos = 15;
                wave.nivel = 2;

                wave.enemigoRate = TimeSpan.FromSeconds(3.0f);
                tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 15);
                wave.waveStart = tiempo;
                //dura 30 segs

                wave.tipoEnemigo = eTipoEnemigo.CazaMercurio;

                wave.entrada = Entrada;
                wave.salida = Salida;

                antesWave = wave;
                this.waves.Add(wave);


                //inicializa la wave 8
                wave = new Wave();

                wave.numEnemigos = 50;
                wave.nivel = 5;

                wave.enemigoRate = TimeSpan.FromSeconds(1.5f);
                tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 10);
                wave.waveStart = tiempo;
                //dura 75 segs

                wave.tipoEnemigo = eTipoEnemigo.Garras;

                wave.entrada = Entrada;
                wave.salida = Salida;

                antesWave = wave;
                this.waves.Add(wave);


                //inicializa la wave 9
                wave = new Wave();

                wave.numEnemigos = 50;
                wave.nivel = 5;

                wave.enemigoRate = TimeSpan.FromSeconds(1f);
                //tiempo += TimeSpan.FromSeconds(45f);
                tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 20);
                wave.waveStart = tiempo;
                //dura 50 segs

                wave.tipoEnemigo = eTipoEnemigo.Tanque;

                wave.entrada = Entrada;
                wave.salida = Salida;

                antesWave = wave;
                this.waves.Add(wave);



                if (num > 10)
                {
                    //inicializa la wave 10

                    wave = new Wave();

                    wave.numEnemigos = 30;
                    wave.nivel = 6;

                    wave.enemigoRate = TimeSpan.FromSeconds(1.3f);
                    tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 20);
                    wave.waveStart = tiempo;
                    //dura 39 segs

                    wave.tipoEnemigo = eTipoEnemigo.Helicoptero;

                    wave.entrada = Entrada;
                    wave.salida = Salida;

                    antesWave = wave;
                    this.waves.Add(wave);


                    //inicializa la wave 11
                    wave = new Wave();

                    wave.numEnemigos = 30;
                    wave.nivel = 6;

                    wave.enemigoRate = TimeSpan.FromSeconds(1.5f);
                    tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 10);
                    wave.waveStart = tiempo;
                    //dura 30 segs

                    wave.tipoEnemigo = eTipoEnemigo.Tanque;

                    wave.entrada = Entrada;
                    wave.salida = Salida;

                    antesWave = wave;
                    this.waves.Add(wave);


                    //inicializa la wave 12
                    wave = new Wave();

                    wave.numEnemigos = 40;
                    wave.nivel = 7;

                    wave.enemigoRate = TimeSpan.FromSeconds(1.5f);
                    tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 15);
                    wave.waveStart = tiempo;
                    //dura 60 segs

                    wave.tipoEnemigo = eTipoEnemigo.Robot;

                    wave.entrada = Entrada;
                    wave.salida = Salida;

                    antesWave = wave;
                    this.waves.Add(wave);


                    //inicializa la wave 13
                    wave = new Wave();

                    wave.numEnemigos = 20;
                    wave.nivel = 5;

                    wave.enemigoRate = TimeSpan.FromSeconds(2.5f);
                    tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 15);
                    wave.waveStart = tiempo;
                    //dura 50 segs

                    wave.tipoEnemigo = eTipoEnemigo.CazaMercurio;

                    wave.entrada = Entrada;
                    wave.salida = Salida;

                    antesWave = wave;
                    this.waves.Add(wave);


                    //inicializa la wave 14
                    wave = new Wave();

                    wave.numEnemigos = 50;
                    wave.nivel = 8;

                    wave.enemigoRate = TimeSpan.FromSeconds(1.25f);
                    tiempo += TimeSpan.FromSeconds(antesWave.numEnemigos * antesWave.enemigoRate.TotalSeconds + 10);
                    wave.waveStart = tiempo;
                    //dura 62.5 segs

                    wave.tipoEnemigo = eTipoEnemigo.Garras;

                    wave.entrada = Entrada;
                    wave.salida = Salida;

                    antesWave = wave;
                    this.waves.Add(wave);

                }
            }

        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// LOAD CONTENT //////////////////
        /*public static void LoadContent(ContentManager content, Fase fase, Tablero tablero, Nave nave)
        {
            //Fase fase = new Fase(tablero, nave);
            DatosFaseWave.LoadFases(fase);


            nave.SetDineroInicial(fase.dineroInicial);
            tablero.puertas = fase.puertas;
            tablero.TableroBool(null);
        }*/


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, Tablero tablero, Enemigos enemigos)
        {
            if (tiempoInicial == TIEMPOMAX) //es la primera vez
            {
                //cuento el tiempo desde q llegamos por primera vez (pasado menu y loading)
                tiempoInicial = gameTime.TotalGameTime; //para q el wave start cuente desde ahora mismo

                actualWave = -1;
            }

            int previoWave = actualWave;
            int maxWave = -1;
            for (int iWave = 0; (iWave <= actualWave + 1) && (iWave < waves.Count); iWave++)
            {
                if (gameTime.TotalGameTime - tiempoInicial > waves[iWave].waveStart) //empezo la wave
                {
                    if (iWave > maxWave)
                        maxWave = iWave;
                }
            }
            actualWave = maxWave;

            if (previoWave != actualWave)
            {
                //esto se hace en cada cambio de wave
                waves[actualWave].previoEnemigoTime = -waves[actualWave].enemigoRate; //para q el primer enemigo salgan cuando empiece el wave start
            }

            //tiempo falta para siguiente wave
            if (actualWave + 1 < waves.Count)
                tiempoSigWave = waves[actualWave + 1].waveStart - (gameTime.TotalGameTime - tiempoInicial);
            else
                tiempoSigWave = TimeSpan.Zero;


            for (int iWave = 0; iWave <= actualWave; iWave++)
            {
                //aqui tengo toda la logica de sacar enemigos en las waves
                waves[iWave].Update(gameTime, tiempoInicial, this, enemigos, tablero);
            }

        }


        ////////////////// DRAW //////////////////

    }
}
