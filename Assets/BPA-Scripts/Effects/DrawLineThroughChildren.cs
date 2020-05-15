using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[ExecuteInEditMode]
public class DrawLineThroughChildren : MonoBehaviour
{
    public LineRenderer lineRenderer;
    Transform[] children;
    // Start is called before the first frame update
   /* void OnEnable()
    {
        //consider this:
        EditorApplication.update += DrawLines;
    }
    void OnDisable()
    {
        //consider this:
        EditorApplication.update += DrawLines;
    }
    private void OnDestroy()
    {
        EditorApplication.update -= DrawLines;
    }*/
    // Update is called once per frame
    void Update()
    {
        DrawLines();
    }
    void DrawLines()
    {
        children = gameObject.GetComponentsInChildren<Transform>();

        lineRenderer.positionCount = children.Length;
        for(int i=0; i < children.Length; i++)
        {
            lineRenderer.SetPosition(i, children[i].position);
        }
    }
}
