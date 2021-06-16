﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Naves
{
    class EnemigoRobot : EnemigoTerrestre
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        public static Sprite sprite = new Sprite();
        public Animacion animacion;


        ////////////////// PUBLICAS //////////////////


        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public EnemigoRobot(Vector2 posicion, Tablero tablero, int nivel = 0)
            : base(posicion, tablero, eTipoEnemigo.Robot, nivel)
        {
            movimiento = new Vector2(0, 1); //siempre empieza mirando abajo
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public override void SetCaracteristicasIniciales()
        {
            //velocidad del enemigo (pixeles por segundo)
            Velocidad = 25 + nivel * 8;
            //vida del enemigo
            maxVida = 10 * nivel + 15;
            //puntos por matarlo
            puntos = 1 * (nivel + 1);
            //dinero por matarlo
            dinero = 2 * (nivel + 1);
            //daño al tocarte
            daño = 12 + nivel * 4;
            //daño al llegar a la salida
            dañoFinal = 6 + (nivel * 3);

            //no dispara
            bDisparo = false;
        }
        

        ////////////////// LOAD CONTENT //////////////////
        public static void LoadContent(ContentManager Content)
        {
            sprite.imagen = Content.Load<Texture2D>("Sprites\\robot_01");
            sprite.dimImagen = new Point(25, 25);
            sprite.posImagen = new Point(0, 0);
        }


        ////////////////// UPDATE ////////////////////////
        public override void UpdateStart(GameTime gameTime)
        {
            animacion = new Animacion(gameTime, 4, TimeSpan.FromSeconds(0.35f), true);
        }

        public override void UpdateAnimation(GameTime gameTime)
        {
            animacion.Update(gameTime, sprite);
        }

        public override void UpdateDisparos(GameTime gameTime)
        {
        }


        ////////////////// DRAW //////////////////
        /*public override void Draw(SpriteBatch spb, Sprite _sprite)
        {
            if (!bEliminar)
            {
                animacion.Draw(spb, sprite, posicion, RotateMovimiento);
            }
        }*/
    }
}
