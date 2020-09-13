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

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        //DrawRadialLinesExample();
        TringleStripWave_BPA();
        QuadPixels();
    }
    void QuadPixels()
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

                float a = i / (float)128;
                // Vertex colors change from red to green
                GL.Color(new Color(a, 1 - a, 0, 0.8F));


                float X = i* quadSpacing;
                float Y = j* quadSpacing;
                //bottom left
                //GL.Color(Color.red);
                GL.Vertex3(X, Y, 0);
                //bottom right
                //GL.Color(Color.blue);
                GL.Vertex3(quadUnitSize + X, Y, 0);
                //upper right
                //GL.Color(Color.cyan);
                GL.Vertex3(quadUnitSize + X, quadUnitSize + Y, 0);
                //upper left
                //GL.Color(Color.green);
                GL.Vertex3(0+X, quadUnitSize + Y, 0);

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

        for (int i = 0; i < lineCount; ++i)
        {
            float speed = 5f;
            float a = i / (float)lineCount;
            // Vertex colors change from red to green
            GL.Color(new Color(a, 1 - a, 0, 0.8F));
            float Y = Mathf.Sin(i + speed * Time.time) * radius;
            GL.Vertex3(i * radius,Y, 0f);

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
