#region File Description
//-----------------------------------------------------------------------------
// PathfinderPointList.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
#endregion

namespace Naves
{
    /// <summary>
    /// PathfinderPointList is a drawable List of screen locations
    /// </summary>
    class PathfinderPointList : Queue<Vector2>
    {
        #region Constants
        /// <summary>
        /// Scales the draw size of the search nodes
        /// </summary>
        //const float waypointNodeDrawScale = 1f;
        #endregion

        #region Fields

        /*private float scale = 1f;
        public float Scale
        {
            get { return scale; }
            set { scale = value * waypointNodeDrawScale; }
        }*/

        public int Num = 0; //numero de nodos en la pila

        // Draw data
        //private Texture2D waypointTexture;
        //private Vector2 waypointCenter;
        #endregion

        #region Initialization

        /// <summary>
        /// Load the PathfinderPointList' texture resources
        /// </summary>
        /// <param name="content"></param>
        /*
        public void LoadContent(ContentManager content)
        {
            waypointTexture = content.Load<Texture2D>("Pathfinder\\dot");
            waypointCenter = new Vector2(waypointTexture.Width / 2, waypointTexture.Height / 2);
        }
        */
        #endregion

        #region Draw
        /// <summary>
        /// Draw the waypoint list, fading from red for the first to 
        /// blue for the last
        /// </summary>
        /// <param name="spb"></param>
        /*
        public void Draw(SpriteBatch spb, Viewport ventanaGame, Tablero tablero)
        {
            int numberPoints = this.Count - 1;
            // This catches a special case where we have only one waypoint in the
            // list, in this case the waypoint won’t draw correctly because we divide
            // 0 by 0 later on and get NaN for our result, fortunately for us this 
            // doesn’t cause the code to crash, but we end up getting a bad color 
            // later on, so we catch this special case early and fix it
            if (numberPoints == 0)
            {
                numberPoints = 1;
            }

            float lerpAmt;
            float i = 0f;
            Color drawColor;

            //spb.Begin();
            foreach (Vector2 location in this)
            {
                // This creates a gradient between 0 for the first waypoint on the 
                // list and 1 for the last, 0 creates a color that's completely red 
                // and 1 creates a color that's completely blue
                lerpAmt = i / numberPoints;
                drawColor = new Color(Vector4.Lerp(
                    Color.Red.ToVector4(), Color.Blue.ToVector4(), lerpAmt));

                //pasar a posicion absoluta (ventana total)
                Vector2 position = new Vector2(location.X + ventanaGame.X, location.Y + ventanaGame.Y);
                //colocar en el centro casillas
                position = new Vector2(position.X + tablero.tamCasillas.X / 2,
                                       position.Y + tablero.tamCasillas.Y / 2);

                spb.Draw(waypointTexture, position, null, drawColor, 0f,
                    waypointCenter, scale, SpriteEffects.None, 0f);

                i += 1f;
            }
            //spb.End();
        }
        */
        #endregion
    }
}