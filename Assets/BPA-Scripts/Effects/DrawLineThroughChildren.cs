using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[ExecuteInEditMode]
public class DrawLineThroughChildren : MonoBehaviour
{
    public LineRenderer lineRenderer;

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
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        List<Transform> childrenList = new List<Transform>(children);
        childrenList.Remove(this.transform);
        lineRenderer.positionCount = childrenList.Count;
        for(int i=0; i < childrenList.Count; i++)
        {
            lineRenderer.SetPosition(i, childrenList[i].position);
        }
    }
}
