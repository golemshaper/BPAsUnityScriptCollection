using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLDrawTest : MonoBehaviour
{
    // When added to an object, draws colored rays from the
    // transform position.
    //https://docs.unity3d.com/ScriptReference/GL.html
    /*
     * IT MIGHT BE BETTER TO USE:
     https://docs.unity3d.com/ScriptReference/Graphics.DrawMesh.html
     instead!
     Still, this is just to try using GL
     * 
     */
    public int lineCount = 100;
    public float radius = 3.0f;
    public float quadSpacing = 3.0f;
    public float quadUnitSize = 0.5f;

    static Material lineMaterial;
    [Header("2D Draw Vars")]
    public Texture2D readMe;
    public int tileSize = 8;
    public int tilePosX = 0;
    public int tilePosY = 0;
    const int canvasSizeX = 128;
    const int canvasSizeY = 128;
    public float canvasScale = 1f;
    Color[,] canvas= new Color[canvasSizeX, canvasSizeY];



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

    float xPosSprite = 0;
    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        //DrawRadialLinesExample();
        TringleStripWave_BPA();
        int sinY = Mathf.RoundToInt((Mathf.Sin(Time.time)) * 32);
        xPosSprite+=32f*Time.deltaTime;
        ClearCanvas(Color.black);
        DrawSpriteTileToCanvas(readMe,8,0,64,Mathf.RoundToInt (xPosSprite), sinY);
        DrawQuadPixelCanvas();
    }
    void ClearCanvas(Color canvasColor)
    {
        for (int i = 0; i < canvasSizeX; i++)
        {
            for (int j = 0; j < canvasSizeY; j++)
            {
                canvas[i, j] = canvasColor;
            }
        }
    }
    void DrawSpriteTileToCanvas(Texture2D spriteSheet, int tileSize,int tileX,int tileY ,int x,int y)
    {
        //Loop


        int localX = 0;
        for(int i= tileX; i<tileSize+ tileX; i++)
        {
            int localY = 0;
            localX++;
            for (int j = tileY; j < tileSize + tileY; j++)
            {
                localY++;
                int finalX = localX + x;
                int finalY = localY + y;
                finalX = finalX % canvasSizeX;
                if (finalY < 0)
                {
                    finalY = canvasSizeY+finalY;
                }
                if (finalX < 0)
                {
                    finalX = canvasSizeX + finalX;
                }

                canvas[finalX, finalY] = spriteSheet.GetPixel(i,j);
            }
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
        
        /* 
          //---
          //bottom left
          GL.Color(Color.red);
          GL.Vertex3(0, 0, 0);
          //bottom right
          GL.Color(Color.blue);
          GL.Vertex3(1, 0, 0);
          //upper right
          GL.Color(Color.cyan);
          GL.Vertex3(1, 1, 0);
          //upper left
          GL.Color(Color.green);
          GL.Vertex3(0, 1, 0);
          //---
        */
        /*
         * TODO: Load an image and assign the color values to each quad to create a sort of canvas that can be drawn on.
         * This would be really cool for old school style game programming. It would also fix the issue with unity 2D looking bad for pixel art...
         * 
         */
        for (int i = 0; i < 128; ++i)
        {
            for (int j = 0; j < 128; ++j)
            {
                Color setColor = canvas[i, j];
              //  setColor.a = 1;
                GL.Color(setColor);

                float X = i* quadSpacing* canvasScale;
                float Y = j* quadSpacing* canvasScale;
                //bottom left
                //GL.Color(Color.red);
                GL.Vertex3(X* canvasScale, Y* canvasScale, 0);
                //bottom right
                //GL.Color(Color.blue);
                GL.Vertex3(quadUnitSize + X* canvasScale, Y* canvasScale, 0);
                //upper right
                //GL.Color(Color.cyan);
                GL.Vertex3(quadUnitSize + X* canvasScale, quadUnitSize + Y* canvasScale, 0);
               
                //upper left
                //GL.Color(Color.green);
                GL.Vertex3(0+X* canvasScale, quadUnitSize + Y* canvasScale, 0);

            }
        }
        GL.End();
        GL.PopMatrix();
    }
    void TringleStripWave_BPA()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        // GL.Begin(GL.QUADS);
        GL.Begin(GL.TRIANGLE_STRIP);
       
        GL.Color(Color.red);
        float scale = 10;
        for (int i = 0; i < lineCount; ++i)
        {
            float speed = 5f;
            float a = i / (float)lineCount;
            // Vertex colors change from red to green
            GL.Color(new Color(a, 1 - a, 0, 0.8F));
            float Y = Mathf.Sin(i + speed * Time.time) * radius;
            GL.Vertex3((i * radius) * scale, (Y)* scale, 0f);

        }

        GL.End();
        GL.PopMatrix();
    }
    void DrawRadialLinesExample()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        
        for (int i = 0; i < lineCount; ++i)
        {
            float a = i / (float)lineCount;
            float angle = a * Mathf.PI * 2;
            // Vertex colors change from red to green
            GL.Color(new Color(a, 1 - a, 0, 0.8F));
            // One vertex at transform position
            GL.Vertex3(0, 0, 0);
            // Another vertex at edge of circle
            GL.Vertex3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        }
        GL.End();
        GL.PopMatrix();
    }
}
