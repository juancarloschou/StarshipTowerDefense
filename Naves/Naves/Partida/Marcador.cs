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
    class Marcador
    {
        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        private Vector2 posicion = new Vector2(250, 0);

        //fuentes
        private SpriteFont font;


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        ////////////////// LOAD CONTENT //////////////////
        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("Fuentes\\Marcador");
        }


        ////////////////// UPDATE ////////////////////////

        public void Update(MenuTorres menuTorres, Fase fase, Tooltip tooltip)
        {
            Vector2 posicionMarc = new Vector2(posicion.X, posicion.Y);

            //corazon - vida de la nave
            posicionMarc = new Vector2(posicion.X + 150, posicion.Y);
            tooltip.datos[(int)eTooltip.Vida_Nave].posicion = new Rectangle((int)posicionMarc.X, (int)posicionMarc.Y,
                                                                      menuTorres.spriteItems.dimImagen.X, menuTorres.spriteItems.dimImagen.Y);
            //money - dinero de la nave
            posicionMarc = new Vector2(posicion.X + 300, posicion.Y);
            tooltip.datos[(int)eTooltip.Dinero].posicion = new Rectangle((int)posicionMarc.X, (int)posicionMarc.Y,
                                                                   menuTorres.spriteItems.dimImagen.X, menuTorres.spriteItems.dimImagen.Y);
            //reloj - tiempo siguiente wave
            posicionMarc = new Vector2(posicion.X + 450, posicion.Y);
            tooltip.datos[(int)eTooltip.Tiempo].posicion = new Rectangle((int)posicionMarc.X, (int)posicionMarc.Y,
                                                                   menuTorres.spriteItems.dimImagen.X, menuTorres.spriteItems.dimImagen.Y);
            //wave
            posicionMarc = new Vector2(posicion.X + 600, posicion.Y);
            string sTexto = Recursos.Wave + " " + fase.ActualWave.ToString();
            Vector2 tamTexto = font.MeasureString(sTexto);
            int ancho = (int)tamTexto.X;
            int alto = (int)tamTexto.Y;
            tooltip.datos[(int)eTooltip.Wave_Tooltip].posicion = new Rectangle((int)posicionMarc.X, (int)posicionMarc.Y, ancho, alto);

        }


        ////////////////// DRAW //////////////////
        public void Draw(SpriteBatch spb, Nave nave, Enemigos enemigos, MenuTorres menuTorres, Fase fase, Tooltip tooltip)
        {
            Vector2 posicionMarc = new Vector2(posicion.X, posicion.Y);
            spb.DrawString(font, "Enem " + enemigos.enemigos.Count.ToString(), posicionMarc, Color.Yellow);
            spb.DrawString(font, "Enem " + enemigos.enemigos.Count.ToString(), posicionMarc + new Vector2(1, 1), Color.White);

            
            //corazon - vida de la nave
            posicionMarc = tooltip.VectorPosicion(eTooltip.Vida_Nave);
            Rectangle rect = new Rectangle(menuTorres.PosImagenItems(eItems.Vida).X, menuTorres.PosImagenItems(eItems.Vida).Y,
                                           menuTorres.spriteItems.dimImagen.X, menuTorres.spriteItems.dimImagen.Y);
            spb.Draw(menuTorres.spriteItems.imagen, posicionMarc, rect, Color.White);

            posicionMarc = new Vector2(posicion.X + 220, posicion.Y + 10);
            spb.DrawString(font, nave.Vida.ToString(), posicionMarc, menuTorres.ColorTextItems(eItems.Vida));
            spb.DrawString(font, nave.Vida.ToString(), posicionMarc + new Vector2(1, 1), Color.White);


            //money - dinero de la nave
            posicionMarc = tooltip.VectorPosicion(eTooltip.Dinero);
            rect = new Rectangle(menuTorres.PosImagenItems(eItems.Dinero).X, menuTorres.PosImagenItems(eItems.Dinero).Y,
                                 menuTorres.spriteItems.dimImagen.X, menuTorres.spriteItems.dimImagen.Y);
            spb.Draw(menuTorres.spriteItems.imagen, posicionMarc, rect, Color.White);

            posicionMarc = new Vector2(posicion.X + 370, posicion.Y + 10);
            spb.DrawString(font, nave.Dinero.ToString(), posicionMarc, menuTorres.ColorTextItems(eItems.Dinero));
            spb.DrawString(font, nave.Dinero.ToString(), posicionMarc + new Vector2(1, 1), Color.White);


            //reloj - tiempo siguiente wave
            posicionMarc = tooltip.VectorPosicion(eTooltip.Tiempo);
            rect = new Rectangle(menuTorres.PosImagenItems(eItems.Tiempo).X, menuTorres.PosImagenItems(eItems.Tiempo).Y,
                                 menuTorres.spriteItems.dimImagen.X, menuTorres.spriteItems.dimImagen.Y);
            spb.Draw(menuTorres.spriteItems.imagen, posicionMarc, rect, Color.White);

            posicionMarc = new Vector2(posicion.X + 520, posicion.Y + 10);
            spb.DrawString(font, ((int)fase.TiempoSiguienteWave.TotalSeconds).ToString(), posicionMarc, menuTorres.ColorTextItems(eItems.Tiempo));
            spb.DrawString(font, ((int)fase.TiempoSiguienteWave.TotalSeconds).ToString(), posicionMarc + new Vector2(1, 1), Color.White);


            /*posicionMarc = new Vector2(posicion.X + 200, posicion.Y);
            spb.DrawString(font, "Vida " + nave.Vida.ToString(), posicionMarc, Color.Yellow);
            spb.DrawString(font, "Vida " + nave.Vida.ToString(), posicionMarc + new Vector2(1, 1), Color.White);*/
            
            /*posicionMarc = new Vector2(posicion.X + 300, posicion.Y);
            spb.DrawString(font, "Punt " + nave.Puntos.ToString(), posicionMarc, Color.Yellow);
            spb.DrawString(font, "Punt " + nave.Puntos.ToString(), posicionMarc + new Vector2(1, 1), Color.White);*/

            /*posicionMarc = new Vector2(posicion.X + 400, posicion.Y);
            spb.DrawString(font, "Torre " + menuTorres.SelectedTorre.ToString(), posicionMarc, Color.Yellow);
            spb.DrawString(font, "Torre " + menuTorres.SelectedTorre.ToString(), posicionMarc + new Vector2(1, 1), Color.White);*/


            //Wave
            posicionMarc = tooltip.VectorPosicion(eTooltip.Wave_Tooltip);
            string sTexto = Recursos.Wave + " " + (fase.ActualWave + 1).ToString();
            spb.DrawString(font, sTexto, posicionMarc, Color.Yellow);
            spb.DrawString(font, sTexto, posicionMarc + new Vector2(1, 1), Color.White);


            //selectedOpcion, selectedTorre, selectedEnemigo
            int i;
            posicionMarc = new Vector2(10, posicion.Y + 25);
            if (menuTorres.SelectedOpcion == -1)
                i = 0;
            else
                i = 1;
            spb.DrawString(font, i.ToString(), posicionMarc, Color.Yellow);

            posicionMarc += new Vector2(30, 0);
            if (menuTorres.SelectedTorre == -1)
                i = 0;
            else
                i = 1;
            spb.DrawString(font, i.ToString(), posicionMarc, Color.Yellow);

            posicionMarc += new Vector2(30, 0);
            if (menuTorres.SelectedEnemigo == -1)
                i = 0;
            else
                i = 1;
            spb.DrawString(font, i.ToString(), posicionMarc, Color.Yellow);

            posicionMarc += new Vector2(30, 0);
            if (menuTorres.SelectedNave == -1)
                i = 0;
            else
                i = 1;
            spb.DrawString(font, i.ToString(), posicionMarc, Color.Yellow);
        }

    }
}
