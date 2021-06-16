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
    public enum eIdioma
    {
        Ingles,
        Español,
    }

    class Idioma
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        private eIdioma _idioma;


        ////////////////// PUBLICAS //////////////////
        public eIdioma idioma
        {
            get { return _idioma; }
            //set { idioma = value; }
        }
      

        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public Idioma()
        {
            //idioma = eIdioma.Nothing;
        }

        public Idioma(eIdioma idioma)
        {
            this._idioma = idioma;
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public string toString()
        {
            if (_idioma == eIdioma.Español)
                return Recursos.Español;
            else if (_idioma == eIdioma.Ingles)
                return Recursos.Ingles;
            else
                return null;
        }

        public string cultureInfo()
        {
            if (_idioma == eIdioma.Español)
                return "es";
            else if (_idioma == eIdioma.Ingles)
                return "en";
            else
                return null;
        }

        public static string cultureInfo(eIdioma idioma)
        {
            if (idioma == eIdioma.Español)
                return "es";
            else if (idioma == eIdioma.Ingles)
                return "en";
            else
                return null;
        }

        /// <summary>
        /// avanza al siguiente idioma, al llegar a max vuelve al principio
        /// </summary>
        public void SiguienteIdioma()
        {
            _idioma = (eIdioma)(((int)_idioma + 1) % (int)Enum.GetValues(typeof(eIdioma)).Length);
        }

        /// <summary>
        /// avanza al siguiente idioma, al llegar a max vuelve al principio
        /// </summary>
        public void AnteriorIdioma()
        {
            if (_idioma == 0)
                _idioma = (eIdioma)((int)Enum.GetValues(typeof(eIdioma)).Length - 1);
            else
                _idioma = (eIdioma)(((int)_idioma - 1) % (int)Enum.GetValues(typeof(eIdioma)).Length);
        }

        public static void SetIdioma(eIdioma idioma)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Idioma.cultureInfo(idioma));
        }
    }
}
