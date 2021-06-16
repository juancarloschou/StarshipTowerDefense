using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Naves
{
    public enum eTipoEnemigo
    {
        //###ENEMIGO###
        Tanque,
        Garras,
        Robot,
        Helicoptero,
        CazaMercurio
    }

    public enum eTipoMovimiento
    {
        Terrestre,
        Aereo
    }


    abstract class EnemigoBase
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public Vector2 posicion;
        //public Point coordenadas;
        public Vector2 movimiento; //vector de longitud 1, representa la direccion

        public Vector2 previaPosicion; //para colisiones paso a paso necesito posicion previa

        //limites de la ventana
        //protected Viewport ventanaGame;

        //caracteristicas
        protected int nivel;          //multiplicador de potencia del enemigo
        private float velocidad;      //velocidad en pixeles por segundo
        protected int vida;           //vida actual
        protected int maxVida;        //vida total
        protected int puntos;         //puntos por matarlo (deberia depender de la cantidad de camino restante, cuanto antes lo mates mas puntos)
        protected int dinero;         //dinero por matarlo
        protected int daño;           //daño q realiza por contacto
        protected int dañoFinal;      //daño q realiza al llegar al final

        //si el enemigo está helado, ralentizado por torreHielo
        static Sprite spriteHielo = new Sprite();
        private bool bHelado;
        private float reduceVelocidad;
        private TimeSpan tiempoInicioHelado;
        private TimeSpan tiempoTotalHelado;

        private bool bStart;          //inicializa animacion, solo es true la primera vez pasa por update
        protected bool bDisparo;      //si el enemigo dispara
        protected bool bAnimacion;    //si hay animacion
        public bool bEliminar;        //eliminar enemigo

        protected eTipoMovimiento tipoMovimiento; //terrestre o aereo
        protected eTipoEnemigo tipoEnemigo; //todos los tipos de enemigos

        public EnemigoVida enemigoVida; //guarda detalles sobre la vida del enemigo para mostrar la barra de salud
        

        ////////////////// PUBLICAS //////////////////
        public eTipoMovimiento TipoMovimiento
        {
            get { return tipoMovimiento; }
        }

        public eTipoEnemigo TipoEnemigo
        {
            get { return tipoEnemigo; }
        }

        public float RotateMovimiento
        {
            get { return (float)Math.Atan2(movimiento.Y, movimiento.X); }
        }

        public int Nivel
        {
            get { return nivel; }
        }

        public int Daño
        {
            get { return daño; }
        }

        public int DañoFinal
        {
            get { return dañoFinal; }
        }

        public int Vida
        {
            get { return vida; }
        }

        public int MaxVida
        {
            get { return maxVida; }
        }

        public int Dinero
        {
            get { return dinero; }
        }

        protected float Velocidad //tiene en cuenta la reduccion por helado
        {
            get 
            {
                if (!bHelado)
                    return velocidad;
                else
                    return velocidad * reduceVelocidad;
            }
            set { velocidad = value; }
        }

        public int MostrarVelocidad //de cara al usuario se muestra en pixeles / 10 por segundo
        {
            get { return (int)(Math.Round(velocidad) / 10); }
        }


        public static Type GetTipo(eTipoEnemigo tipoEnemigo)
        {
            //###ENEMIGO###
            Type type = null;
            if (tipoEnemigo == eTipoEnemigo.Tanque)
                type = typeof(EnemigoTanque);
            else if (tipoEnemigo == eTipoEnemigo.Garras)
                type = typeof(EnemigoGarras);
            else if (tipoEnemigo == eTipoEnemigo.Robot)
                type = typeof(EnemigoRobot);
            else if (tipoEnemigo == eTipoEnemigo.Helicoptero)
                type = typeof(EnemigoHelicoptero);
            else if (tipoEnemigo == eTipoEnemigo.CazaMercurio)
                type = typeof(EnemigoCazaMercurio);
            
            return type;
        }

        public Animacion GetAnimacion(eTipoEnemigo tipoEnemigo)
        {
            //return (Animacion)(EnemigoBase.GetTipo(tipoEnemigo)).GetField("animacion").GetValue(Activator.CreateInstance(EnemigoBase.GetTipo(tipoEnemigo)));
            return (Animacion)(EnemigoBase.GetTipo(tipoEnemigo)).GetField("animacion").GetValue(this);

            /*
            if (tipoEnemigo == eTipoEnemigo.Tanque)
                animacion = ((EnemigoTanque)this).animacion;
            else if (tipoEnemigo == eTipoEnemigo.Garras)
                animacion = ((EnemigoGarras)this).animacion;
            else if (tipoEnemigo == eTipoEnemigo.Robot)
                animacion = ((EnemigoRobot)this).animacion;
            else if (tipoEnemigo == eTipoEnemigo.Helicoptero)
                animacion = ((EnemigoHelicoptero)this).animacion;
            else if (tipoEnemigo == eTipoEnemigo.CazaMercurio)
                animacion = ((EnemigoCazaMercurio)this).animacion;
            return animacion;
            */
        }

        public Animacion GetAnimacion()
        {
            return GetAnimacion(tipoEnemigo);
        }

        public static Sprite GetSprite(eTipoEnemigo tipoEnemigo)
        {
            //return ((EnemigoBase)(EnemigoBase.GetTipo(tipoEnemigo))).sprite;
            return (Sprite)(EnemigoBase.GetTipo(tipoEnemigo)).GetField("sprite").GetValue(0);

            /*
            Sprite sprite = null;
            if (tipoEnemigo == eTipoEnemigo.Tanque)
                sprite = EnemigoTanque.sprite;
            else if (tipoEnemigo == eTipoEnemigo.Garras)
                sprite = EnemigoGarras.sprite;
            else if (tipoEnemigo == eTipoEnemigo.Robot)
                sprite = EnemigoRobot.sprite;
            else if (tipoEnemigo == eTipoEnemigo.Helicoptero)
                sprite = EnemigoHelicoptero.sprite;
            else if (tipoEnemigo == eTipoEnemigo.CazaMercurio)
                sprite = EnemigoCazaMercurio.sprite;
            return sprite;
            */
        }

        public Sprite GetSprite()
        {
            return GetSprite(tipoEnemigo);
        }

        /*public static Sprite GetSprite(EnemigoBase enemigo)
        {
            return GetSprite(enemigo.tipoEnemigo);
        }*/

        public static string TipoEnemigoToString(eTipoEnemigo tipoEnemigo)
        {
            string str = string.Empty;

            //###ENEMIGO###
            if (tipoEnemigo == eTipoEnemigo.Tanque)
                str = Recursos.Enemigo_Tanque;
            else if (tipoEnemigo == eTipoEnemigo.Garras)
                str = Recursos.Enemigo_Garras;
            else if (tipoEnemigo == eTipoEnemigo.Robot)
                str = Recursos.Enemigo_Robot;
            else if (tipoEnemigo == eTipoEnemigo.Helicoptero)
                str = Recursos.Enemigo_Helicoptero;
            else if (tipoEnemigo == eTipoEnemigo.CazaMercurio)
                str = Recursos.Enemigo_CazaMercurio;

            return str;
        }


        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public EnemigoBase(Vector2 posicion, eTipoMovimiento TipoMovimiento, eTipoEnemigo tipoEnemigo, int nivel = 0, bool bAnimacion = true)
        {
            //this.ventanaGame = VentanaGame;
            this.nivel = nivel;
            this.posicion = posicion;
            this.previaPosicion = this.posicion;

            this.tipoMovimiento = TipoMovimiento;
            this.tipoEnemigo = tipoEnemigo;
            this.bEliminar = false;

            SetCaracteristicasIniciales();
            this.vida = this.maxVida;

            this.bStart = true;
            this.bHelado = false;
            this.reduceVelocidad = 1f;
            this.tiempoInicioHelado = TimeSpan.Zero;
            this.tiempoTotalHelado = TimeSpan.Zero;

            this.bAnimacion = bAnimacion;

            this.enemigoVida = new EnemigoVida(this);
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public abstract void SetCaracteristicasIniciales();


        private void EliminarEnemigo(Nave nave, Opciones opciones)
        {
            //destruccion enemigo por disparo
            bEliminar = true;
            nave.Dinero += dinero;
            Sonidos.PlayFX(eSonido.ExplosionPequeña, opciones);

            //depende de la cantidad de camino restante, cuanto antes lo mates, mas puntos
            int nodosRestan;
            if (tipoMovimiento == eTipoMovimiento.Aereo)
                nodosRestan = ((EnemigoAereo)this).TamañoCamino;
            else
                nodosRestan = ((EnemigoTerrestre)this).TamañoCamino;

            if (nodosRestan != int.MaxValue)
            {
                nave.Puntos += puntos * (100 - nodosRestan);
            }
        }

        //recibe ataque por disparo nave
        public void RecibeAtaque(int daño, int dañoAire, Nave nave, Opciones opciones)
        {
            if (tipoMovimiento == eTipoMovimiento.Terrestre)
                vida -= daño;
            else if (tipoMovimiento == eTipoMovimiento.Aereo)
                vida -= dañoAire;

            if (vida <= 0)
            {
                vida = 0;
                //destruccion enemigo por disparo
                EliminarEnemigo(nave, opciones);
            }
            else
            {
                //hit
                Sonidos.PlayFX(eSonido.HitEnemigo, opciones);
            }
        }

        public void RecibeAtaqueTorre(int daño, Nave nave, Opciones opciones)
        {
            vida -= daño;

            if (vida <= 0)
            {
                vida = 0;
                //destruccion enemigo por disparo torre
                EliminarEnemigo(nave, opciones);
            }
            else
            {
                //hit
                //Sonidos.PlayFX(eSonido.HitEnemigo, opciones);
            }
        }

        public void RecibeAtaqueTorreHelado(GameTime gameTime, int daño, float reduceVelocidad, TimeSpan tiempoReduceVelocidad, Nave nave, Opciones opciones)
        {
            vida -= daño;

            if (vida <= 0)
            {
                vida = 0;
                //destruccion enemigo por disparo torre
                EliminarEnemigo(nave, opciones);
            }
            else
            {
                //helar enemigo
                if (bHelado)
                {
                    if (reduceVelocidad < this.reduceVelocidad) //si esta helada es mas fuerte q la ya existente
                        this.reduceVelocidad = reduceVelocidad;
                }
                else
                    this.reduceVelocidad = reduceVelocidad;
                bHelado = true;
                tiempoInicioHelado = gameTime.TotalGameTime;
                tiempoTotalHelado = tiempoReduceVelocidad;
            }
        }

        public void FinalCamino(GameTime gameTime, Nave nave, Animaciones animaciones, Opciones opciones)
        {
            nave.EnemigoFinalCamino(dañoFinal, gameTime, animaciones, opciones);
        }


        public void Explosion(GameTime gameTime, Opciones opciones, Animaciones animaciones, bool bSonido)
        {
            //destruccion enemigo al chocar
            bEliminar = true;
            if (bSonido) //si explota la nave, no poner sonido de otras explosiones
            {
                Sonidos.PlayFX(eSonido.ExplosionMediana, opciones);
            }

            //explosion de enemigo
            animaciones.AddAnimacion(gameTime, eAnimacionSprite.ExplosionMediana, posicion);
        }


        ////////////////// LOAD CONTENT //////////////////
        public static void LoadContent(ContentManager Content)
        {
            spriteHielo.imagen = Content.Load<Texture2D>("Sprites\\enemigo_disparo_hielo_01");
            spriteHielo.dimImagen = new Point(132, 132);
            spriteHielo.posImagen = new Point(0, 0);
        }


        ////////////////// UPDATE ////////////////////////
        public void Update(GameTime gameTime, Tablero tablero, Nave nave, Animaciones animaciones, Opciones opciones)
        {
            if (bStart)
            {
                //inicialiaza animation
                bStart = false;
                UpdateStart(gameTime);
            }

            if (!bEliminar)
            {
                UpdateHelado(gameTime);

                UpdateMovimiento(gameTime, tablero, nave, animaciones, opciones);

                UpdateAnimation(gameTime);

                if (bDisparo)
                {
                    UpdateDisparos(gameTime);
                }

                //rotateMovimiento = (float)Math.Atan2(movimiento.Y, movimiento.X);
            }
        }

        public virtual void UpdateStart(GameTime gameTime)
        {
        }

        public virtual void UpdateHelado(GameTime gameTime)
        {
            if (bHelado)
            {
                if (gameTime.TotalGameTime - tiempoInicioHelado > tiempoTotalHelado)
                {
                    bHelado = false;
                    reduceVelocidad = 1f;
                    tiempoInicioHelado = TimeSpan.Zero;
                    tiempoTotalHelado = TimeSpan.Zero;
                }
            }
        }

        public abstract void UpdateMovimiento(GameTime gameTime, Tablero tablero, Nave nave, Animaciones animaciones, Opciones opciones);

        public virtual void UpdateAnimation(GameTime gameTime)
        {
        }

        public abstract void UpdateDisparos(GameTime gameTime);


        ////////////////// DRAW //////////////////
        protected void DrawHelado(SpriteBatch spb, Sprite sprite)
        {
            if (bHelado)
            {
                float alpha = 0.35f;
                Vector2 scale = new Vector2((float)sprite.dimImagen.X / spriteHielo.dimImagen.X, 
                                            (float)sprite.dimImagen.Y / spriteHielo.dimImagen.Y);
                Rectangle rectangulo = new Rectangle(spriteHielo.posImagen.X, spriteHielo.posImagen.Y, spriteHielo.dimImagen.X, spriteHielo.dimImagen.Y);
                spb.Draw(spriteHielo.imagen, posicion, rectangulo, Color.White * alpha, 1f, spriteHielo.center, scale * 1.25f, SpriteEffects.None, 0f);
            }
        }

        public virtual void Draw(SpriteBatch spb, Sprite sprite, Animacion animacion)
        {
            //dibuja al enemigo con sprite no animado. si hay animación se sobreescribe este metodo
            if (!bEliminar)
            {
                if (bAnimacion)
                {
                    animacion.Draw(spb, sprite, posicion, RotateMovimiento);
                }
                else
                {
                    Rectangle rectangulo = new Rectangle(sprite.posImagen.X, sprite.posImagen.Y, sprite.dimImagen.X, sprite.dimImagen.Y);
                    spb.Draw(sprite.imagen, posicion, rectangulo, Color.White, RotateMovimiento, sprite.center, 1f, SpriteEffects.None, 0f);
                }

                DrawHelado(spb, sprite);
            }
        }

        //public abstract void Draw(SpriteBatch spb);

    }
}
