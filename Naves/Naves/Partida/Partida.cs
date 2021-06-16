using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Naves
{
    public enum eEstadoPlaneta
    {
        Completado,
        SinCompletar,
        Bloqueado
    }

    [Serializable]
    public class DatosPartida
    {
        public eEstadoPlaneta[] estado; //estado de cada planeta
        public int[] puntos; //record de puntos en cada planeta
        public int[] dinero; //record de dinero sobrante en cada planeta

        //public int dinero; //dinero de la nave
        public bool autoDisparo; //autofire
        public eTipoDisparo tipoDisparo; //tipo de disparos actual de la nave
        //nivel de los disparos de la nave
        public CaracteristicasTipoDisparo[] caracteristicas = new CaracteristicasTipoDisparo[Enum.GetValues(typeof(eTipoDisparo)).Length];
    }


    /// <summary>
    /// http://stackoverflow.com/questions/3723287/good-example-of-xna-4-0-to-save-game-data
    /// llevar el guardador/cargador de datos a clase base para q hereden las dos q hay !!!!!!!!!!!!!!!
    /// </summary>
    class Partida
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        private DatosPartida partidaDatos = null; //partida a cargar/guardar

        private ePlaneta planeta; //planeta actual q estamos jugando

        private bool existeLoad = false;
        private StorageDevice storageDevice;
        private eSavingState savingState = eSavingState.Nothing;
        private eSaveLoad saveLoad = eSaveLoad.Nothing;
        private IAsyncResult asyncResult;
        private PlayerIndex playerIndex;
        private StorageContainer storageContainer;
        private static string dirContainer = "Naves_Save";
        private static string filename = "Naves_Save.sav";


        ////////////////// PUBLICAS //////////////////
        public DatosPartida PartidaDatos
        {
            get { return partidaDatos; }
            set { partidaDatos = value; }
        }

        public bool ExisteLoad
        {
            get { return existeLoad; }
        }

        public ePlaneta Planeta
        {
            get { return planeta; }
            set { planeta = value; }
        }

        public int PlanetaSiguiente
        {
            get
            {
                int planetaSiguiente = 0; // (int)ePlaneta.Mercurio;
                foreach (ePlaneta planeta in Enum.GetValues(typeof(ePlaneta)))
                {
                    if (PartidaDatos.estado[(int)planeta] == eEstadoPlaneta.Completado)
                    {
                        //el ultimo planeta completado
                        if ((int)planeta + 1 < Enum.GetValues(typeof(ePlaneta)).Length) // probarlo !!!!!
                            planetaSiguiente = (int)planeta + 1;
                        else
                            planetaSiguiente = -1;
                    }
                }
                return planetaSiguiente;
            }
        }


        ////////////////// METODOS //////////////////
        public void GameOver(int puntos)
        {
            if (puntos > partidaDatos.puntos[(int)planeta])
                partidaDatos.puntos[(int)planeta] = puntos;
        }

        //pasar de fase
        public void CompletarPlaneta(ePlaneta planeta, int puntos, int dinero)
        {
            //completa el planeta y abre el siguiente
            PartidaDatos.estado[(int)planeta] = eEstadoPlaneta.Completado;
            if((int)planeta + 1 < Enum.GetValues(typeof(ePlaneta)).Length) //probarlo
            {
                PartidaDatos.estado[(int)planeta + 1] = eEstadoPlaneta.SinCompletar;
            }

            //records
            if (puntos > partidaDatos.puntos[(int)planeta])
                partidaDatos.puntos[(int)planeta] = puntos;
            if (dinero > partidaDatos.dinero[(int)planeta])
                partidaDatos.dinero[(int)planeta] = dinero;
        }

        public void CompletarPlaneta(ePlaneta planeta, Nave nave)
        {
            CompletarPlaneta(planeta, nave.Puntos, nave.Dinero);
        }


        ////////////////// CONSTRUCTOR //////////////////
        public Partida()
        {
            this.playerIndex = PlayerIndex.One;
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        private void LoadPartidaDefecto()
        {
            PartidaDatos = new DatosPartida();
            PartidaDatos.estado = new eEstadoPlaneta[Enum.GetValues(typeof(ePlaneta)).Length];
            PartidaDatos.puntos = new int[Enum.GetValues(typeof(ePlaneta)).Length];
            PartidaDatos.dinero = new int[Enum.GetValues(typeof(ePlaneta)).Length];
            foreach (ePlaneta planeta in Enum.GetValues(typeof(ePlaneta)))
            {
                PartidaDatos.estado[(int)planeta] = eEstadoPlaneta.Bloqueado;
                PartidaDatos.puntos[(int)planeta] = 0;
                PartidaDatos.dinero[(int)planeta] = 0;
            }
            PartidaDatos.estado[0] = eEstadoPlaneta.SinCompletar; //abrir el primer planeta

            //test!!!!!! completar el primer planeta y abrir el segundo
            //CompletarPlaneta(ePlaneta.Mercurio, 1000, 100);
            //CompletarPlaneta(ePlaneta.Venus, 400, 80);


            PartidaDatos.autoDisparo = true;
            PartidaDatos.tipoDisparo = eTipoDisparo.Simple;

            Disparos.SetCaracteristicas(PartidaDatos.caracteristicas, eTipoDisparo.Simple, 0, true); //unica disponible inicialmente
            Disparos.SetCaracteristicas(PartidaDatos.caracteristicas, eTipoDisparo.Multiple, -1, false); //arma no disponible
            Disparos.SetCaracteristicas(PartidaDatos.caracteristicas, eTipoDisparo.Laser, -1, false);
            Disparos.SetCaracteristicas(PartidaDatos.caracteristicas, eTipoDisparo.Misiles, -1, false);
            Disparos.SetCaracteristicas(PartidaDatos.caracteristicas, eTipoDisparo.Rayo, -1, false);
        }


        /*private static bool OpcionesIguales(DatosOpciones o1, DatosOpciones o2)
        {
            if ((o1 != null) && (o2 != null) &&
                (o1.idioma == o2.idioma) && (o1.sonido == o2.sonido) &&
                (o1.volumenFx == o2.volumenFx) && (o1.volumenMusica == o2.volumenMusica) &&
                (o2.teclas != null))
            {
                bool bTeclas = true;
                for (int iTeclas = 0; iTeclas < Enum.GetValues(typeof(eTeclas)).Length; iTeclas++)
                {
                    if (o1.teclas[iTeclas] != o2.teclas[iTeclas])
                        bTeclas = false;
                }
                if(bTeclas)
                    return true;
            }
            return false;
        }*/

        /*private static void CopiarOpciones(DatosOpciones desde, ref DatosOpciones hasta)
        {
            if (hasta == null)
                hasta = new DatosOpciones();
            hasta.idioma = desde.idioma;
            hasta.sonido = desde.sonido;
            hasta.volumenFx = desde.volumenFx;
            hasta.volumenMusica = desde.volumenMusica;
            hasta.teclas = desde.teclas;
        }*/


        public bool Save() //OpcionesDatos OpcionesDatos, PlayerIndex PlayerIndex)
        {
            //this.playerIndex = PlayerIndex;
            //this.opcionesDatos = OpcionesDatos;

            if (savingState == eSavingState.Nothing)
                //if (!OpcionesIguales(opcionesDatos, opcionesGuardadas))
                {
                    saveLoad = eSaveLoad.Save;
                    savingState = eSavingState.ReadyToOpenStorageContainer;
                    return true;
                }
            return false;
        }

        public bool Load() //PlayerIndex PlayerIndex)
        {
            //this.playerIndex = PlayerIndex;

            if (savingState == eSavingState.Nothing)
                //if (!OpcionesIguales(opcionesDatos, opcionesGuardadas))
                {
                    existeLoad = false;
                    saveLoad = eSaveLoad.Load;
                    savingState = eSavingState.ReadyToOpenStorageContainer;
                    return true;
                }
            return false;
        }


        //cuando termine de guardar
        public bool FinishSave()
        {
            if ((saveLoad == eSaveLoad.Save) && (savingState == eSavingState.Finish))
            {
                //CopiarOpciones(opcionesDatos, ref opcionesGuardadas);
                savingState = eSavingState.Nothing;
                saveLoad = eSaveLoad.Nothing;
                return true;
            }
            return false;
        }

        //cuando termine de cargar
        public bool FinishLoad()
        {
            if ((saveLoad == eSaveLoad.Load) && (savingState == eSavingState.Finish))
            {
                //CopiarOpciones(opcionesDatos, ref opcionesGuardadas);
                savingState = eSavingState.Nothing;
                saveLoad = eSaveLoad.Nothing;

                //Idioma.SetIdioma(opcionesDatos.idioma);

                return true;
            }
            return false;
        }


        /// <summary>
        /// Allows the screen to run logic, pensado para load/save asincrono
        /// </summary>
        public void Update()
        {
            if (saveLoad != eSaveLoad.Nothing)
            {
                switch (savingState)
                {
                    case eSavingState.ReadyToSelectStorageDevice:
#if XBOX
                    if (!Guide.IsVisible)
#endif
                        {
                            asyncResult = StorageDevice.BeginShowSelector(playerIndex, null, null);
                            savingState = eSavingState.SelectingStorageDevice;
                        }
                        break;

                    case eSavingState.SelectingStorageDevice:
                        if (asyncResult.IsCompleted)
                        {
                            storageDevice = StorageDevice.EndShowSelector(asyncResult);
                            savingState = eSavingState.ReadyToOpenStorageContainer;
                        }
                        break;

                    case eSavingState.ReadyToOpenStorageContainer:
                        if (storageDevice == null || !storageDevice.IsConnected)
                        {
                            savingState = eSavingState.ReadyToSelectStorageDevice;
                        }
                        else
                        {
                            asyncResult = storageDevice.BeginOpenContainer(dirContainer, null, null);
                            savingState = eSavingState.OpeningStorageContainer;
                        }
                        break;

                    case eSavingState.OpeningStorageContainer:
                        if (asyncResult.IsCompleted)
                        {
                            storageContainer = storageDevice.EndOpenContainer(asyncResult);
                            savingState = eSavingState.ReadyToSaveLoad;
                        }
                        break;

                    case eSavingState.ReadyToSaveLoad:
                        if (storageContainer == null)
                        {
                            savingState = eSavingState.ReadyToOpenStorageContainer;
                        }
                        else
                        {
                            try
                            {
                                if (saveLoad == eSaveLoad.Load) 
                                {
                                    //load-------------
                                    existeLoad = LoadData();
                                }
                                else if (saveLoad == eSaveLoad.Save) 
                                {
                                    //save-------------
                                    DeleteExisting();
                                    SaveData();
                                }
                            }
                            catch (Exception e)
                            {
                                // Replace with in game dialog notifying user of error
                                Console.WriteLine(e.Message);

                                savingState = eSavingState.Nothing;
                                saveLoad = eSaveLoad.Nothing;
                                existeLoad = false;

                                // si el archivo esta corrupto lo borramos y si es load cargamos generico
                                DeleteExisting();
                                if (saveLoad == eSaveLoad.Load)
                                    LoadPartidaDefecto();
                            }
                            finally
                            {
                                storageContainer.Dispose();
                                storageContainer = null;
                            }
                            savingState = eSavingState.Finish;
                            
                        }
                        break;
                }
            }
        }


        private void DeleteExisting()
        {
            try
            {
                if (storageContainer.FileExists(filename))
                {
                    storageContainer.DeleteFile(filename);
                }
            }
            catch
            {
            }
        }

        private void SaveData()
        {
            Stream stream = null;
            try
            {
                using (stream = storageContainer.CreateFile(filename))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(DatosPartida));
                    serializer.Serialize(stream, partidaDatos);
                }
            }
            catch
            {
                if(stream != null)
                    stream.Close();
                throw new Exception();
            }

        }

        private bool LoadData()
        {
            bool bLoad = false;
            Stream stream = null;
            try
            {
                if (storageContainer.FileExists(filename))
                {
                    using (stream = storageContainer.OpenFile(filename, FileMode.Open, FileAccess.Read))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(DatosPartida));
                        partidaDatos = (DatosPartida)serializer.Deserialize(stream);
                    }
                    bLoad = true;
                }
                else
                {
                    //es la primera vez q se juega, no hay opciones guardadas
                    LoadPartidaDefecto();
                    bLoad = false;
                }
            }
            catch
            {
                if (stream != null)
                    stream.Close();
                throw new Exception();
            }
            return bLoad;
        }

    }
}
