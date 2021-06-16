using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Naves
{
    [Serializable]
    public class DatosFases
    {
        public List<DatosFase> fases = new List<DatosFase>();
    }


    [Serializable]
    public class DatosFase
    {
        public ePlaneta planeta;
        public int dineroInicial; //dinero con el que enmpieza la nave

        public List<DatosWave> waves = new List<DatosWave>();

        public List<DatosPuerta> puertas = new List<DatosPuerta>();
    }


    [Serializable]
    public class DatosWave
    {
        public int entrada; //id de la puerta
        public int salida;

        public int numEnemigos; //numero de enemigos de la wave
        public int nivel; //nivel de enemigos
        public eTipoEnemigo tipoEnemigo; //tipo de enemigos

        [XmlIgnore]
        public TimeSpan enemigoRate; //frecuencia salen los enemigos por la puerta
        [XmlElement("enemigoRate")]
        public double XmlEnemigoRate 
        {
            get { return enemigoRate.TotalSeconds; }
            set { enemigoRate = TimeSpan.FromSeconds(value); } 
        }

        [XmlIgnore]
        public TimeSpan waveStart; //tiempo en q empezará la wave (a partir de tiempoInicial)
        [XmlElement("waveStart")]
        public double XmlWaveStart
        {
            get { return waveStart.TotalSeconds; }
            set { waveStart = TimeSpan.FromSeconds(value); }
        }
    }


    [Serializable]
    public class DatosPuerta
    {
        public int idPuerta; //posicion en la lista de puertas
        public bool bEntrada; //true entrada, false salida
        public Point coordTopLeft; //posicion arriba izquierda puerta (en casillas)
        public Point coordBottomRight; //posicion abajo derecha puerta (en casillas)
    }


    class DatosFaseWave
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        //public DatosFases datos;

  
        ////////////////// PUBLICAS //////////////////


        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public DatosFaseWave()
        {
            //datos = new DatosFases();
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        public static void SaveFase(Fase fase, DatosFases dfs)
        {
            //cargar Fase en DatosFases de mercurio

            DatosFase df = new DatosFase();
            df.planeta = fase.planeta; //ePlaneta.Mercurio
            df.dineroInicial = fase.dineroInicial;

            for (int iPuerta = 0; iPuerta < fase.puertas.Count; iPuerta++)
            {
                DatosPuerta dp = new DatosPuerta();
                Puerta p = fase.puertas[iPuerta];
                dp.idPuerta = iPuerta;
                dp.bEntrada = p.bEntrada;
                dp.coordTopLeft = p.coordTopLeft;
                dp.coordBottomRight = p.coordBottomRight;
                df.puertas.Add(dp);
            }

            for (int iWave = 0; iWave < fase.waves.Count; iWave++)
            {
                DatosWave dw = new DatosWave();
                Wave w = fase.waves[iWave];
                dw.entrada = w.entrada;
                dw.salida = w.salida;
                dw.enemigoRate = w.enemigoRate;
                dw.nivel = w.nivel;
                dw.numEnemigos = w.numEnemigos;
                dw.tipoEnemigo = w.tipoEnemigo;
                dw.waveStart = w.waveStart;
                df.waves.Add(dw);
            }

            //DatosFases dfs = new DatosFases();
            dfs.fases.Add(df);
        }

        public static void SaveFasesToXML(DatosFases dfs)
        {
            //cargar Fase en DatosFases de mercurio
            /*
            DatosFase df = new DatosFase();
            df.planeta = fase.planeta; //ePlaneta.Mercurio
            df.dineroInicial = fase.dineroInicial;
            
            for(int iPuerta = 0; iPuerta < fase.puertas.Count; iPuerta++)
            {
                DatosPuerta dp = new DatosPuerta();
                Puerta p = fase.puertas[iPuerta];
                dp.idPuerta = iPuerta;
                dp.bEntrada = p.bEntrada;
                dp.coordTopLeft = p.coordTopLeft;
                dp.coordBottomRight = p.coordBottomRight;
                df.puertas.Add(dp);
            }

            for (int iWave = 0; iWave < fase.waves.Count; iWave++)
            {
                DatosWave dw = new DatosWave();
                Wave w = fase.waves[iWave];
                dw.entrada = w.entrada;
                dw.salida = w.salida;
                dw.enemigoRate = w.enemigoRate;
                dw.nivel = w.nivel;
                dw.numEnemigos = w.numEnemigos;
                dw.tipoEnemigo = w.tipoEnemigo;
                dw.waveStart = w.waveStart;
                df.waves.Add(dw);
            }

            DatosFases dfs = new DatosFases();
            dfs.fases.Add(df);

            //this.datos = dfs;
            */


            //guardar DatosFases en content/Fases/fases.xml

            //string ruta = "D:\\XNA\\Naves\\Naves 40\\Naves\\Naves\\Fases.xml";
            //string ruta = GetApplicationPath() + "Fases.xml";

            Stream streamFases = TitleContainer.OpenStream("Fases.xml");

            XmlSerializer serializer = new XmlSerializer(typeof(DatosFases));
            //XmlTextWriter stream = new XmlTextWriter(ruta, Encoding.UTF8);
            XmlTextWriter stream = new XmlTextWriter(streamFases, Encoding.UTF8);

            //----------------------------------------------------------------------
            // A fin de permitir una mejor claridad si lo abrimos desde el bloc de
            // notas, le indicaremos al stream, que emplee un estilo de formato
            // identado con tabulaciones.
            //----------------------------------------------------------------------
            stream.Formatting = Formatting.Indented;
            stream.Indentation = 1;
            //stream.IndentChar = 't';

            //----------------------------------------------------------------------
            // Finalmente mandamos que cree el XML y se cierre el stream.
            //----------------------------------------------------------------------
            serializer.Serialize(stream, dfs);
            stream.Close();

        }


        /*public static string GetApplicationPath()
        {
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //"D:\\XNA\\Naves\\Naves 41\\Naves\\Naves\\bin\\x86\\Debug\\Naves.exe"
            int iPos = exePath.IndexOf("bin\\");
            string appPath = exePath.Substring(0, iPos);
            return appPath;
        }*/

        public static void LoadFases(Fase fase, ePlaneta planeta)
        {
            //guardar DatosFases en content/Fases/fases.xml

            //string ruta = "D:\\XNA\\Naves\\Naves 40\\Naves\\Naves\\Fases.xml";
            //string ruta = GetApplicationPath() + "Fases.xml";

            Stream streamFases = TitleContainer.OpenStream("Fases.xml");

            //FileStream ReadFileStream = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.Read);
            XmlSerializer SerializerObj = new XmlSerializer(typeof(DatosFases));
            DatosFases dfs = (DatosFases)SerializerObj.Deserialize(streamFases);
            streamFases.Close();

            /*
            XmlDocument xmlDoc = content.Load<XmlDocument>("Fases\\Fases");
            MemoryStream xmlStream = new MemoryStream();
            xmlDoc.Save(xmlStream);
            xmlStream.Flush();
            xmlStream.Position=0;
            xmlDoc.Load(xmlStream);
            XmlSerializer SerializerObj = new XmlSerializer(typeof(DatosFases));
            DatosFases dfs = (DatosFases)SerializerObj.Deserialize(xmlStream);
            xmlStream.Close();
            */


            //cargar DatosFases de mercurio en fase

            DatosFase df = new DatosFase();
            df = dfs.fases[(int)planeta];

            fase.planeta = df.planeta;
            fase.dineroInicial = df.dineroInicial;

            fase.puertas.Clear();
            for (int iPuerta = 0; iPuerta < df.puertas.Count; iPuerta++)
            {
                Puerta p = new Puerta();
                DatosPuerta dp = df.puertas[iPuerta];
                p.idPuerta = iPuerta;
                p.bEntrada = dp.bEntrada;
                p.coordTopLeft = dp.coordTopLeft;
                p.coordBottomRight = dp.coordBottomRight;
                fase.puertas.Add(p);
            }

            fase.waves.Clear();
            for (int iWave = 0; iWave < df.waves.Count; iWave++)
            {
                Wave w = new Wave();
                DatosWave dw = df.waves[iWave];
                w.entrada = dw.entrada;
                w.salida = dw.salida;
                w.enemigoRate = dw.enemigoRate;
                w.nivel = dw.nivel;
                w.numEnemigos = dw.numEnemigos;
                w.tipoEnemigo = dw.tipoEnemigo;
                w.waveStart = dw.waveStart;
                fase.waves.Add(w);
            }

            //Fases fases = new Fases();
            //dfs.fases.Add(df);

            //this.datos = dfs;

        }


        ////////////////// LOAD CONTENT //////////////////
        /*public void LoadContent(ContentManager Content)
        {
            //imagen = Content.Load<Texture2D>("Fondo\\space_background_01");
        }*/


        ////////////////// UPDATE ////////////////////////
        /*public void Update()
        {

        }*/


        ////////////////// DRAW //////////////////
        /*public void Draw(SpriteBatch spb)
        {

        }*/

    }
}
