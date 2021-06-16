using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Naves
{
    /// <summary>
    /// http://forums.create.msdn.com/forums/t/7414.aspx
    /// </summary>
    class DibujarLineas
    {

        Texture2D pixel; 
        List<Vector2> vectors; 
       
        /// <summary>        
        /// Gets/sets the colour of the primitive line object.        
        /// </summary>        
        public Color Colour;
        
        /// <summary>        
        /// Gets/sets the position of the primitive line object.        
        /// </summary>        
        public Vector2 Position;
        
        /// <summary>        
        /// Gets/sets the render depth of the primitive line object (0 = front, 1 = back)        
        /// </summary>      
        public float Depth;

        /// <summary>        
        /// Gets/sets the scale        
        /// </summary>      
        public float Scale;

        /// <summary>        
        /// Gets the number of vectors which make up the primtive line object.        
        /// </summary>        
        public int CountVectors        
        {            
            get            
            {                
                return vectors.Count;            
            }        
        }        

        /// <summary>        
        /// Creates a new primitive line object.        
        /// </summary>        
        /// <param name="graphicsDevice">The Graphics Device object to use.</param>        
        public DibujarLineas(GraphicsDevice graphicsDevice)        
        {            
            // create pixels            
            //pixel = new Texture2D(graphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            pixel = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color); 

            Color[] pixels = new Color[1];            
            pixels[0] = Color.White;            
            pixel.SetData<Color>(pixels);            
            Colour = Color.White;            
            Position = new Vector2(0, 0);            
            Depth = 0;
            Scale = 1f;

            vectors = new List<Vector2>();        
        }
 
        /// <summary>       
        /// /// Called when the primive line object is destroyed.        
        /// </summary>        
        ~DibujarLineas()        
        {        
        }      
        
        /// <summary>        
        /// Adds a vector to the primive live object.        
        /// </summary>        
        /// <param name="vector">The vector to add.</param>        
        public void AddVector(Vector2 vector)        
        {            
            vectors.Add(vector);        
        }        
        
        /// <summary>        
        /// Insers a vector into the primitive line object.        
        /// </summary>        
        /// <param name="index">The index to insert it at.</param>        
        /// <param name="vector">The vector to insert.</param>        
        public void InsertVector(int index, Vector2 vector)        
        {            
            //vectors.Insert(index, vectors);  
            this.vectors.Insert(index, vector); 
        }        
        
        /// <summary>        
        /// Removes a vector from the primitive line object.        
        /// </summary>        
        /// <param name="vector">The vector to remove.</param>        
        public void RemoveVector(Vector2 vector)        
        {            
            vectors.Remove(vector);        
        }        
        
        /// <summary>        
        /// Removes a vector from the primitive line object.        
        /// </summary>        
        /// <param name="index">The index of the vector to remove.</param>        
        public void RemoveVector(int index)        
        {            
            vectors.RemoveAt(index);        
        }        
        
        /// <summary>        
        /// Clears all vectors from the primitive line object.        
        /// </summary>        
        public void ClearVectors()        
        {            
            vectors.Clear();        
        }        
        

        /// <summary>        
        /// Creates a circle starting from 0, 0.        
        /// </summary>        
        /// <param name="radius">The radius (half the width) of the circle.</param>        
        /// <param name="sides">The number of sides on the circle (the more the detailed).</param>        
        public void CreateCircle(float x, float y, float radius, int sides)        
        {            
            vectors.Clear();            
            float max = 2 * (float)Math.PI;            
            float step = max / (float)sides;            
            for (float theta = 0; theta < max; theta += step)            
            {                
                vectors.Add(new Vector2(radius * (float)Math.Cos((double)theta) + x,                    
                    radius * (float)Math.Sin((double)theta) + y));            
            }            
            // then add the first vector again so it's a complete loop            
            vectors.Add(new Vector2(radius * (float)Math.Cos(0) + x,                    
                radius * (float)Math.Sin(0) + y));        
        }

        /// <summary>
        /// Creates an ellipse starting from 0, 0 with the given width and height.
        /// Vectors are generated using the parametric equation of an ellipse.
        /// </summary>
        /// <param name="semimajor_axis">The width of the ellipse at its center.</param>
        /// <param name="semiminor_axis">The height of the ellipse at its center.</param>
        /// <param name="angle_offset">The counterlockwise rotation in radians.</param>
        /// <param name="sides">The number of sides on the ellipse (a higher value yields more resolution).</param>
        public void CreateEllipse(float x, float y, float semimajor_axis, float semiminor_axis, float angle_offset, int sides)
        {    vectors.Clear();    
            float max = 2.0f * (float)Math.PI;    
            float step = max / (float)sides;    
            float h = x;    
            float k = y;    
            
            for (float t = 0.0f; t < max; t += step)    
            {        
                // center point: (h,k); add as argument if you want (to circumvent modifying this.Position)        
                // x = h + a*cos(t)  -- a is semimajor axis, b is semiminor axis        
                // y = k + b*sin(t)        
                vectors.Add(new Vector2((float)(h + semimajor_axis * Math.Cos(t)),                                
                    (float)(k + semiminor_axis * Math.Sin(t))));    
            }    
            // then add the first vector again so it's a complete loop    
            vectors.Add(new Vector2((float)(h + semimajor_axis * Math.Cos(step)),                            
                (float)(k + semiminor_axis * Math.Sin(step))));    
            // now rotate it as necessary    
            Matrix m = Matrix.CreateRotationZ(angle_offset);    
            for (int i = 0; i < vectors.Count; i++ )    
            {        
                vectors[ i ] = Vector2.Transform((Vector2)vectors[ i ],m);    
            }
        }

        /// <summary>   
        /// Create a line box    
        /// </summary>   
        /// <param name="topLeft">Top Left hand corner of the box</param>   
        /// <param name="botRight">Bottom Right hand coner of the box</param>   
        public void CreateBox(Vector2 topLeft, Vector2 botRight)  
        {  
            vectors.Clear();  
  
            vectors.Add(topLeft);  
            vectors.Add(new Vector2(topLeft.X, botRight.Y));  
  
            vectors.Add(botRight);  
            vectors.Add(new Vector2(botRight.X, topLeft.Y));  
  
            vectors.Add(topLeft);  
        }

        public void CreateBox(Rectangle rect)
        {
            vectors.Clear();

            vectors.Add(new Vector2(rect.X, rect.Y));
            vectors.Add(new Vector2(rect.X, rect.Y + rect.Height));

            vectors.Add(new Vector2(rect.X + rect.Width, rect.Y + rect.Height));
            vectors.Add(new Vector2(rect.X + rect.Width, rect.Y));

            vectors.Add(new Vector2(rect.X, rect.Y));
        }   



        /// <summary>        
        /// Renders the primtive line object.        
        /// </summary>        
        /// <param name="spriteBatch">The sprite batch to use to render the primitive line object.</param>    
        /*
        public void Render(SpriteBatch spriteBatch)        
        {            
            if (vectors.Count < 2)                
                return;            
            for (int i = 1; i < vectors.Count; i++)            
            {                
                Vector2 vector1 = (Vector2)vectors[i-1];                
                Vector2 vector2 = (Vector2)vectors[i];                
                // calculate the distance between the two vectors                
                float distance = Vector2.Distance(vector1, vector2);                
                // calculate the angle between the two vectors                
                float angle = (float)Math.Atan2((double)(vector2.Y - vector1.Y),                    
                    (double)(vector2.X - vector1.X));                
                // stretch the pixel between the two vectors                
                spriteBatch.Draw(pixel,                    
                    Position + vector1,                    
                    null,                    
                    Colour,                    
                    angle,                    
                    Vector2.Zero,                    
                    new Vector2(distance, 1),                    
                    SpriteEffects.None,                    
                    Depth);            
            }        
        }        
        */
        public void Render(SpriteBatch spriteBatch)  
        {  
             if (vectors.Count < 2)  
                  return;  
  
             for (int i = 1; i < vectors.Count; i++)  
             {  
                  Vector2 vector1 = vectors[i - 1];  
                  Vector2 vector2 = vectors[i];  
  
                  // calculate the distance between the two vectors  
                  float distance = Vector2.Distance(vector1, vector2);  
                  Vector2 length = vector2 - vector1;  
                  length.Normalize();  
  
                  // calculate the angle between the two vectors  
                  //float angle = (float)Math.Atan2(vector2.Y - vector1.Y, vector2.X - vector1.X);  
  
                  int count = (int)Math.Round(distance);  
  
                  for (int x = 0; x < count; ++x)  
                  {  
                       vector1 += length;  
                       // stretch the pixel between the two vectors  
                       spriteBatch.Draw(pixel, Position + vector1, null, Colour, 0, Vector2.Zero, Scale, SpriteEffects.None, Depth);  
                  }  
             }  
        }  

        
    }
}
