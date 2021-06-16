using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Naves
{
    struct Sonido
    {
        //efecto FX
        public SoundEffect snd;
        public float volume;
    }

    struct Musica
    {
        //musica
        public Song snd;
        public float volume;
    }

    enum eSonido
    {
        Pausa,
        Laser1,
        Laser2,
        Misil,
        HitEnemigo,
        HitNave,
        ExplosionPequeña,
        ExplosionMediana,
        //ExplosionGrande,
        ExplosionNave,
        BeepError,
        TankFire,
        ExplosionCanon,
        CambiarArma,
        Vender
    }

    enum eMusica
    {
        CancionTitulo,
        Cancion1,
        Cancion2,
        Cancion3,
        GameOver,
        Victoria
    }

    static class Sonidos
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        private static Dictionary<eSonido, Sonido> FX = new Dictionary<eSonido, Sonido>();
        private static Dictionary<eMusica, Musica> Music = new Dictionary<eMusica, Musica>();

        private static int MaxVolumenOpciones = 100;

        private static float volumenMinimo = 0.0f;
        private static float volumenBajo = 0.25f;
        private static float volumenNormal = 0.5f;
        private static float volumenAlto = 0.75f;
        private static float volumenMaximo = 1.0f;

        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// LOAD CONTENT //////////////////
        public static void LoadContent(ContentManager Content)
        {

            //sonidos FX
            Sonido sonido = new Sonido();

            sonido.snd = Content.Load<SoundEffect>("Audio\\pause");
            sonido.volume = volumenNormal;
            FX.Add(eSonido.Pausa, sonido);

            sonido.snd = Content.Load<SoundEffect>("Audio\\laser_shot_01");
            sonido.volume = volumenNormal;
            FX.Add(eSonido.Laser1, sonido);

            sonido.snd = Content.Load<SoundEffect>("Audio\\laser_shot_02");
            sonido.volume = volumenNormal;
            FX.Add(eSonido.Laser2, sonido);

            sonido.snd = Content.Load<SoundEffect>("Audio\\misil_launch_01");
            sonido.volume = volumenNormal;
            FX.Add(eSonido.Misil, sonido);

            sonido.snd =  Content.Load<SoundEffect>("Audio\\metal_hit_01");
            sonido.volume = volumenMaximo;
            FX.Add(eSonido.HitEnemigo, sonido);

            sonido.snd = Content.Load<SoundEffect>("Audio\\hit_nave_01");
            sonido.volume = volumenNormal;
            FX.Add(eSonido.HitNave, sonido);

            sonido.snd = Content.Load<SoundEffect>("Audio\\explosion_pequena_01");
            sonido.volume = volumenBajo;
            FX.Add(eSonido.ExplosionPequeña, sonido);

            sonido.snd = Content.Load<SoundEffect>("Audio\\explosion_mediana_01");
            sonido.volume = volumenNormal;
            FX.Add(eSonido.ExplosionMediana, sonido);

            sonido.snd = Content.Load<SoundEffect>("Audio\\explosion_nave_01");
            sonido.volume = volumenNormal;
            FX.Add(eSonido.ExplosionNave, sonido);

            sonido.snd = Content.Load<SoundEffect>("Audio\\beep_01");
            sonido.volume = volumenBajo;
            FX.Add(eSonido.BeepError, sonido);

            sonido.snd = Content.Load<SoundEffect>("Audio\\tank_firing_01");
            sonido.volume = volumenNormal;
            FX.Add(eSonido.TankFire, sonido);

            sonido.snd = Content.Load<SoundEffect>("Audio\\explosion_canon_01");
            sonido.volume = volumenBajo;
            FX.Add(eSonido.ExplosionCanon, sonido);

            sonido.snd = Content.Load<SoundEffect>("Audio\\reload_pistol_01");
            sonido.volume = volumenMaximo;
            FX.Add(eSonido.CambiarArma, sonido);

            sonido.snd = Content.Load<SoundEffect>("Audio\\vender_01");
            sonido.volume = volumenAlto;
            FX.Add(eSonido.Vender, sonido);



            //musica
            Musica musica = new Musica();

            musica.snd = Content.Load<Song>("Audio\\Title_Theme");
            musica.volume = volumenNormal; //esta cancion debe estar a volumen normal (se usa en el SetVolumen del menu opciones)
            Music.Add(eMusica.CancionTitulo, musica);

             musica.snd = Content.Load<Song>("Audio\\Staff_Roll");
            musica.volume = volumenNormal;
            Music.Add(eMusica.Cancion1, musica);

            musica.snd = Content.Load<Song>("Audio\\Galaxy_Force");
            musica.volume = volumenNormal;
            Music.Add(eMusica.Cancion2, musica);

            musica.snd = Content.Load<Song>("Audio\\Cyber_Star");
            musica.volume = volumenNormal;
            Music.Add(eMusica.Cancion3, musica);

            musica.snd = Content.Load<Song>("Audio\\Game_Over");
            musica.volume = volumenNormal;
            Music.Add(eMusica.GameOver, musica);

            musica.snd = Content.Load<Song>("Audio\\Stage_Clear");
            musica.volume = volumenNormal;
            Music.Add(eMusica.Victoria, musica);
        }


        ////////////////// DRAW //////////////////

        public static void PlayFX(eSonido nombre, Opciones opciones)
        {
            if (FX.ContainsKey(nombre))
            {
                if ((opciones.OpcionesDatos != null) && (opciones.OpcionesDatos.sonido))
                {
                    //reproduce el conido con el volumen configurado
                    float volumen = FX[nombre].volume;
                    volumen = volumen * opciones.OpcionesDatos.volumenFx / MaxVolumenOpciones;
                    if (volumen > 1f)
                        volumen = 1;

                    FX[nombre].snd.Play(volumen, 0, 0);
                }
            }
        }


        public static void PlayMusic(eMusica nombre, Opciones opciones)
        {
            PlayMusic(nombre, true, opciones);
        }

        public static void PlayMusic(eMusica nombre, bool bRepetirMusica, Opciones opciones)
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            if (Music.ContainsKey(nombre))
                if ((opciones.OpcionesDatos != null) && (opciones.OpcionesDatos.sonido))
                {
                    try
                    {
                        //parar la cancion anterior
                        if(MediaPlayer.State == MediaState.Playing)
                            MediaPlayer.Stop();

                        float volumen = Music[nombre].volume;
                        volumen = volumen * opciones.OpcionesDatos.volumenMusica / MaxVolumenOpciones;
                        if (volumen > 1f)
                            volumen = 1;

                        MediaPlayer.Volume = volumen;

                        // Play the music
                        MediaPlayer.Play(Music[nombre].snd);
                    
                        // Loop the currently playing song
                        MediaPlayer.IsRepeating = bRepetirMusica;

                    }
                    catch 
                    { 
                    }
                }
        }

        public static void SetVolumen(Opciones opciones)
        {
            if ((opciones.OpcionesDatos != null)) // && (opciones.OpcionesDatos.Sonido))
            {
                float volumen = volumenNormal * opciones.OpcionesDatos.volumenMusica / MaxVolumenOpciones;
                if (volumen > 1f)
                    volumen = 1;

                MediaPlayer.Volume = volumen;
            }
        }

        public static void PauseMusic()
        {
            try
            {
                //pausa la cancion anterior
                if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Pause();
            }
            catch
            {
            }
        }

        public static void ResumeMusic()
        {
            try
            {
                //reanuda la cancion anterior
                if (MediaPlayer.State == MediaState.Paused)
                    MediaPlayer.Resume();
            }
            catch
            {
            }
        }

        public static void PlayOrResumeMusic(eMusica nombre, Opciones opciones)
        {
            try
            {
                //reanuda la cancion anterior
                if (MediaPlayer.State == MediaState.Paused)
                {
                    MediaPlayer.Resume();
                }
                else
                {
                    PlayMusic(nombre, opciones);
                }
            }
            catch
            {
            }

        }
    }
}
