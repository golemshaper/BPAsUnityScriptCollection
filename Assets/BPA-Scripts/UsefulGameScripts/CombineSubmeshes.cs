using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CombineSubmeshes : MonoBehaviour
{
    public bool disableCombineMesh;
    void Awake()
    {
        if (disableCombineMesh) return;
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        Matrix4x4 myTransformMatrix = transform.worldToLocalMatrix;
        Material captureMaterial = null;
        if (meshFilters[0].GetComponent<MeshRenderer>().material == null)
        {
            captureMaterial = meshFilters[0].GetComponent<MeshRenderer>().material;
        }
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = myTransformMatrix*meshFilters[i].transform.localToWorldMatrix;
            if(meshFilters[i].gameObject!=this.gameObject)
            {
                meshFilters[i].GetComponent<Renderer>().enabled = false; //disable render to maintain colliders.
            }

            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
        if(captureMaterial!=null) this.GetComponent<MeshRenderer>().material = captureMaterial;
        transform.localPosition=meshFilters[0].transform.localPosition;
        
    }
}
