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
    class Nave
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public Sprite sprite = new Sprite(); //imagen con todos los sprites

        public Vector2 posicion; //posicion en la pantalla (ventanagame)
        public Vector2 movimiento;
        
        //limites de la ventana
        private Viewport ventana;

        //caracteristicas
        private float velocidad; //pixeles por segundo
        private int vidaMax;
        private int vida;
        private int puntos;
        private int dinero;

        private float ventaTorre; //porcentaje de su valor q recibe al vender una torre


        //lista de disparos
        public Disparos disparos;


        private eTipoDisparo tipoDisparo; //tipo de disparos actual de la nave

        public bool autoDisparo; //autofire


        ////////////////// PUBLICAS //////////////////
        public eTipoDisparo TipoDisparo
        {
            get { return tipoDisparo; }
            set
            {
                //caracteristicas disparo, cambia imagenes nave
                tipoDisparo = value;
            }
        }

        public int Vida
        {
            get { return vida; }
        }

        public int Puntos
        {
            get { return puntos; }
            set { puntos = value; }
        }

        public int Dinero
        {
            get { return dinero; }
            set { dinero = value; }
        }


        ////////////////// CONSTRUCTOR //////////////////
        public Nave(Viewport viewport, Tooltip tooltip, Partida partida)
        {
            ventana = viewport;

            sprite.dimImagen = new Point(42, 44);
            posicion = new Vector2(ventana.Width / 2, ventana.Height - sprite.dimImagen.Y * 2);
            movimiento = new Vector2(0, 0);

            puntos = 0;
            dinero = 0; //se carga en fase
            velocidad = 100f;
            vidaMax = 100;
            vida = vidaMax;

            ventaTorre = 0.5f;

            //inicializar lista de disparos
            disparos = new Disparos(viewport, tooltip, partida);

            TipoDisparo = eTipoDisparo.Simple; //tipo de disparo inicial = simple

            autoDisparo = true;
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public int DineroVentaTorre(int precioTorre)
        {
            return (int)(ventaTorre * precioTorre);
        }

        public void SetDineroInicial(int dinero)
        {
            this.dinero = dinero;
        }

        public void PagarUpgradeTorre(int precio)
        {
            this.dinero -= precio; //se paga el precio
        }

        public void PagarCrearTorre(int precio)
        {
            this.dinero -= precio; //se paga el precio
        }

        private void ExplosionNave(GameTime gameTime, Animaciones animaciones, Opciones opciones)
        {
            //explosion nuestra nave (vamos a morir...)
            Sonidos.PlayFX(eSonido.ExplosionNave, opciones);

            //explosion de nave
            animaciones.AddAnimacion(gameTime, eAnimacionSprite.ExplosionNave, posicion);
        }

        public bool RecibeAtaque(int daño, GameTime gameTime, Animaciones animaciones, Opciones opciones)
        {
            bool bExplosionNave = false;
            
            vida -= daño;

            if (vida <= 0)
            {
                vida = 0;

                ExplosionNave(gameTime, animaciones, opciones);

                bExplosionNave = true; //no poner otros sonidos
            }
            else
            {
                //hit a nuestra nave
                Sonidos.PlayFX(eSonido.HitNave, opciones);
            }

            return bExplosionNave;
        }


        public bool EnemigoFinalCamino(int daño, GameTime gameTime, Animaciones animaciones, Opciones opciones)
        {
            bool bExplosionNave = false;

            vida -= daño;

            if (vida <= 0)
            {
                vida = 0;

                ExplosionNave(gameTime, animaciones, opciones);

                bExplosionNave = true; //no poner otros sonidos
            }
            else
            {
                //hit a nuestra nave
                Sonidos.PlayFX(eSonido.HitNave, opciones);
            }

            return bExplosionNave;
        }


        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager Content)
        {
            sprite.imagen = Content.Load<Texture2D>("Sprites\\battleship_01");
            //sprite.dimImagen = new Point(42, 44);
            //sprite.posImagen = new Point(0, 0);

            UpdateSprite(); //establece posImagen en base al movimiento


            //imagen del disparo, precargada para reutilizarla
            disparos.LoadContent(Content);

        }


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState previousKeyboardState, Enemigos enemigos, Opciones opciones)
        {
            UpdateArma(gameTime, keyboardState, previousKeyboardState, opciones);

            UpdatePosition(gameTime, keyboardState, opciones);
            UpdateSprite(); //elegir el sprite en base al movimiento

            disparos.Update(gameTime, keyboardState, this, tipoDisparo, enemigos, opciones);
        }


        private void UpdateArma(GameTime gameTime, KeyboardState keyboardState, KeyboardState previousKeyboardState, Opciones opciones)
        {
            if (keyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.ArmaAnterior)) && !previousKeyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.ArmaAnterior)))
            {
                disparos.ArmaAnterior(this, opciones);
            }
            if (keyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.ArmaSiguiente)) && !previousKeyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.ArmaSiguiente)))
            {
                disparos.ArmaSiguiente(this, opciones);
            }

            if (keyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.AutoDisparo)) && !previousKeyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.AutoDisparo)))
            {
                autoDisparo = !autoDisparo;
            }
        }


        private void UpdatePosition(GameTime gameTime, KeyboardState keyboardState, Opciones opciones)
        {
            movimiento = new Vector2(0, 0);

            if (keyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.Izquierda)))
            {
                movimiento.X -= 1f;
            }
            if (keyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.Derecha)))
            {
                movimiento.X += 1f;
            }
            if (keyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.Arriba)))
            {
                movimiento.Y -= 1f;
            }
            if (keyboardState.IsKeyDown(opciones.GetTeclas(eTeclas.Abajo)))
            {
                movimiento.Y += 1f;
            }

            //movimiento.Normalize();

            //we'll use move Speed and elapsed Time to find the how far the tank moves
            posicion += movimiento * velocidad * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Make sure that the player does not go out of bounds
            posicion.X = MathHelper.Clamp(posicion.X, sprite.dimImagen.X / 2, ventana.Width - sprite.dimImagen.X / 2);
            posicion.Y = MathHelper.Clamp(posicion.Y, sprite.dimImagen.Y / 2, ventana.Height - sprite.dimImagen.Y / 2);

        }

        private void UpdateSprite()
        {
            //a partir de aquí escojemos la parte de imagen que queremos dibujar y la almacenamos en rectangle (sprite)
            //en funcion de la combinacion de botones que se esten pulsando.
            
            // 1 | 2 | 3   bajar
            // ---------
            // 4 | 5 | 6   subir
            // ---------
            // 7 | 8 | 9   normal

            if (movimiento.X < 0 && movimiento.Y < 0)
            {
                CrearSprite(0, sprite.dimImagen.Y); //4
            }
            else if (movimiento.X > 0 && movimiento.Y < 0)
            {
                CrearSprite(sprite.dimImagen.X * 2, sprite.dimImagen.Y); //6
            }
            else if (movimiento.X == 0 && movimiento.Y < 0)
            {
                CrearSprite(sprite.dimImagen.X, sprite.dimImagen.Y); //5
            }
            else if (movimiento.X < 0 && movimiento.Y > 0)
            {
                CrearSprite(0, 0); //1
            }
            else if (movimiento.X > 0 && movimiento.Y > 0)
            {
                CrearSprite(sprite.dimImagen.X * 2, 0); //3
            }
            else if (movimiento.X == 0 && movimiento.Y > 0)
            {
                CrearSprite(sprite.dimImagen.X, 0); //2
            }
            else if (movimiento.X < 0 && movimiento.Y == 0)
            {
                CrearSprite(0, sprite.dimImagen.Y * 2); //7
            }
            else if (movimiento.X > 0 && movimiento.Y == 0)
            {
                CrearSprite(sprite.dimImagen.X * 2, sprite.dimImagen.Y * 2); //9
            }
            else //sprite estandar (sin movimiento)
            {
                CrearSprite(sprite.dimImagen.X, sprite.dimImagen.Y * 2); //8
            }
        }

        private void CrearSprite(int x, int y)
        {
            sprite.posImagen = new Point(x, y);
        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb, GameOver partida)
        {
            if (partida.Estado != ePartidaEstado.GameOver) //si muero desaparece la nave y explota
            {
                Rectangle rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, sprite.dimImagen.X, sprite.dimImagen.Y);

                spb.Draw(sprite.imagen, posicion, rectangulo, Color.White, 0f, sprite.center, 1f, SpriteEffects.None, 0f);
            }

            DrawDisparos(spb);
        }

        private void DrawDisparos(SpriteBatch spb)
        {
            disparos.Draw(spb);
        }
    }
}
