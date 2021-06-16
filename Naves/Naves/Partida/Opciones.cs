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
    enum eTeclas
    {
        Arriba,
        Abajo,
        Izquierda,
        Derecha,
        Disparo,
        AutoDisparo,
        ArmaSiguiente,
        ArmaAnterior,
        MejorarTorre,
        VenderTorre
    }

    [Serializable]
    public class DatosOpciones
    {
        public eIdioma idioma;
        public bool sonido;
        public byte volumenFx; //0-100
        public byte volumenMusica; //0-100
        public Keys[] teclas;
    }


    enum eSavingState
    {
        Nothing,
        ReadyToSelectStorageDevice,
        SelectingStorageDevice,
        ReadyToOpenStorageContainer, // once we have a storage device start here
        OpeningStorageContainer,
        ReadyToSaveLoad,
        Finish //save al terminar va a nothing, load se queda en este estado hasta llamada a GetLoad
    }

    enum eSaveLoad
    {
        Nothing,
        Save,
        Load
    }
    

    /// <summary>
    /// http://stackoverflow.com/questions/3723287/good-example-of-xna-4-0-to-save-game-data
    /// </summary>
    class Opciones
    {

        ///////////////////////////////////////
        // PROPIEDADES
        ///////////////////////////////////////

        private DatosOpciones opcionesDatos = null; //opciones a cargar/guardar

        private DatosOpciones opcionesGuardadas = null; //para comparar y ver si han cambiado

        private StorageDevice storageDevice;
        private eSavingState savingState = eSavingState.Nothing;
        private eSaveLoad saveLoad = eSaveLoad.Nothing;
        private IAsyncResult asyncResult;
        private PlayerIndex playerIndex;
        private StorageContainer storageContainer;
        private static string dirContainer = "Naves_Save";
        private static string filename = "Naves_Options.sav";

        public bool bSave = false;


        ////////////////// PUBLICAS //////////////////
        public DatosOpciones OpcionesDatos
        {
            get { return opcionesDatos; }
            set { opcionesDatos = value; }
        }

        public Keys GetTeclas(eTeclas tecla)
        {
            return OpcionesDatos.teclas[(int)tecla];
        }

        ////////////////// EVENTOS //////////////////


        ////////////////// CONSTRUCTOR //////////////////
        public Opciones()
        {
            this.playerIndex = PlayerIndex.One;
        }


        ///////////////////////////////////////
        // METODOS
        ///////////////////////////////////////

        private void LoadOpcionesDefecto()
        {
            opcionesDatos = new DatosOpciones();
            opcionesDatos.idioma = eIdioma.Español;
            opcionesDatos.sonido = true;
            opcionesDatos.volumenFx = 50;
            opcionesDatos.volumenMusica = 50;

            opcionesDatos.teclas = new Keys[(int)Enum.GetValues(typeof(eTeclas)).Length];
            opcionesDatos.teclas[(int)eTeclas.Arriba] = Keys.Up;
            opcionesDatos.teclas[(int)eTeclas.Abajo] = Keys.Down;
            opcionesDatos.teclas[(int)eTeclas.Izquierda] = Keys.Left;
            opcionesDatos.teclas[(int)eTeclas.Derecha] = Keys.Right;
            opcionesDatos.teclas[(int)eTeclas.Disparo] = Keys.A;
            opcionesDatos.teclas[(int)eTeclas.AutoDisparo] = Keys.CapsLock;
            opcionesDatos.teclas[(int)eTeclas.ArmaAnterior] = Keys.Q;
            opcionesDatos.teclas[(int)eTeclas.ArmaSiguiente] = Keys.E;
            opcionesDatos.teclas[(int)eTeclas.MejorarTorre] = Keys.M;
            opcionesDatos.teclas[(int)eTeclas.VenderTorre] = Keys.V;
        }


        private static bool OpcionesIguales(DatosOpciones o1, DatosOpciones o2)
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
        }

        private static void CopiarOpciones(DatosOpciones desde, ref DatosOpciones hasta)
        {
            if (hasta == null)
                hasta = new DatosOpciones();
            hasta.idioma = desde.idioma;
            hasta.sonido = desde.sonido;
            hasta.volumenFx = desde.volumenFx;
            hasta.volumenMusica = desde.volumenMusica;
            hasta.teclas = desde.teclas;
        }


        public bool Save() //OpcionesDatos OpcionesDatos, PlayerIndex PlayerIndex)
        {
            //this.playerIndex = PlayerIndex;
            //this.opcionesDatos = OpcionesDatos;

            if (savingState == eSavingState.Nothing)
                if (!OpcionesIguales(opcionesDatos, opcionesGuardadas))
                {
                    saveLoad = eSaveLoad.Save;
                    savingState = eSavingState.ReadyToOpenStorageContainer;
                    bSave = true;
                    return true;
                }
            return false;
        }

        public bool Load() //PlayerIndex PlayerIndex)
        {
            //this.playerIndex = PlayerIndex;

            if (savingState == eSavingState.Nothing)
                if (!OpcionesIguales(opcionesDatos, opcionesGuardadas))
                {
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
                CopiarOpciones(opcionesDatos, ref opcionesGuardadas);
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
                CopiarOpciones(opcionesDatos, ref opcionesGuardadas);
                savingState = eSavingState.Nothing;
                saveLoad = eSaveLoad.Nothing;

                Idioma.SetIdioma(opcionesDatos.idioma);

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
                                    LoadData();
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

                                // si el archivo esta corrupto lo borramos y si es load cargamos generico
                                DeleteExisting();
                                if (saveLoad == eSaveLoad.Load)
                                    LoadOpcionesDefecto();
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
                    XmlSerializer serializer = new XmlSerializer(typeof(DatosOpciones));
                    serializer.Serialize(stream, opcionesDatos);
                }
            }
            catch
            {
                if(stream != null)
                    stream.Close();
                throw new Exception();
            }

        }

        private void LoadData()
        {
            Stream stream = null;
            try
            {
                if (storageContainer.FileExists(filename))
                {
                    using (stream = storageContainer.OpenFile(filename, FileMode.Open, FileAccess.Read))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(DatosOpciones));
                        opcionesDatos = (DatosOpciones)serializer.Deserialize(stream);
                    }
                }
                else
                {
                    //es la primera vez q se juega, no hay opciones guardadas

                    LoadOpcionesDefecto();
                }
            }
            catch
            {
                if (stream != null)
                    stream.Close();
                throw new Exception();
            }
        }

    }
}
