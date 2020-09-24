using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Polygon2DGE
{
    public class Polygon2DRenderer : MonoBehaviour
    {
        /*
         * Not done yet.
         * Idea: This will be a renderer for a 2D canvas. Logic for game should be in another file and interface with this.
         * Maybe make an interface for this so you can replace the unity part of this in the future and make it a C# only game Lib.
         * 
         * Give it all of the Pico-8 functions.
         * 
         * 
         * 
         * 
         * 
         */
        public float quadSpacing = 3.0f;
        public float quadUnitSize = 0.5f;
        public float canvasScale = 1f;
        public const int canvasSizeX = 128;
        public const int canvasSizeY = 128;
        Color[,] canvas = new Color[canvasSizeX, canvasSizeY];
        /// <summary>
        /// Auto-called by unity...
        /// </summary>
        public void OnRenderObject()
        {
            DrawQuadPixelCanvas();
        }
        static Material lineMaterial;
        static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }
        void DrawQuadPixelCanvas()
        {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);
            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            GL.MultMatrix(transform.localToWorldMatrix);
            GL.Begin(GL.QUADS);
            
            for (int i = 0; i < 128; ++i)
            {
                for (int j = 0; j < 128; ++j)
                {
                    Color setColor = canvas[i, j];
                    //  setColor.a = 1;
                    GL.Color(setColor);

                    float X = i * quadSpacing * canvasScale;
                    float Y = j * quadSpacing * canvasScale;
                    float Z = Mathf.Sin(j); //Wavy for fun!

                    //bottom left
                    //GL.Color(Color.red * setColor);
                    GL.Vertex3(X * canvasScale, Y * canvasScale, Z);
                    //bottom right
                    GL.Color(Color.blue * setColor); //<-----COLOR FOR FUN!!
                    GL.Vertex3(quadUnitSize + X * canvasScale, Y * canvasScale, Z);
                    //upper right
                    //GL.Color(Color.cyan * setColor);
                    GL.Color(setColor);
                    GL.Vertex3(quadUnitSize + X * canvasScale, quadUnitSize + Y * canvasScale, Z);

                    //upper left
                    //GL.Color(Color.green * setColor);
                    GL.Vertex3(0 + X * canvasScale, quadUnitSize + Y * canvasScale, Z);

                }
            }
            GL.End();
            GL.PopMatrix();
        }
        public void ClearCanvas(Color canvasColor)
        {
            for (int i = 0; i < canvasSizeX; i++)
            {
                for (int j = 0; j < canvasSizeY; j++)
                {
                    canvas[i, j] = canvasColor;
                }
            }
        }
        public void SpriteBlit(Texture2D spriteSheet, int tileSize, int tileX, int tileY, int x, int y, bool flipX, bool flipY)
        {

            //X LOOP:::::::::::::::
            int xStartPos = tileX;
            int xEndPos = tileSize + tileX;
            int xIncrement = 1;
            if (flipX)
            {
                xStartPos = tileSize + tileX;
                xEndPos = tileX;
                xIncrement = -1;
            }
            //X LOOP:::::::::::::::
            //::
            //Y LOOP:::::::::::::::
            int yStartPos = tileY;
            int yEndPos = tileSize + tileY;
            int yIncrement = 1;
            if (flipX)
            {
                yStartPos = tileSize + tileY;
                yEndPos = tileY;
                yIncrement = -1;
            }
            //Y LOOP:::::::::::::::

            //.........................................................
            //Loop
            int localX = 0;
            for (int i = xStartPos; i != xEndPos; i += xIncrement)
            {
                int localY = 0;
                localX++;
                for (int j = yStartPos; j != yEndPos; j += yIncrement)
                {
                    localY++;
                    int finalX = localX + x;
                    int finalY = localY + y;
                    finalX = finalX % canvasSizeX;
                    finalY = finalY % canvasSizeY;
                    if (finalY < 0)
                    {
                        finalY = canvasSizeY + finalY;
                    }
                    if (finalX < 0)
                    {
                        finalX = canvasSizeX + finalX;
                    }

                    canvas[finalX, finalY] = spriteSheet.GetPixel(i, j);
                }
            }
        }
    }
}