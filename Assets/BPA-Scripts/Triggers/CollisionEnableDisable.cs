using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CollisionEnableDisable : MonoBehaviour {

    [Header("Test Against")]
    public List<GameObject> TriggerIfTouching= new List<GameObject>();
    public bool includeHero;
    public List<string> checkTags = new List<string>();
    int[] checkTagsHashed;
    public bool CheckMultitag;

    public bool doNotExitIfTouchingSameTag;
    public bool exitOnStart;
    [Header("On Enter")]
    public GameObject[] enableIfEnter;
    public GameObject[] disableIfEnter;

    [Header("On Exit")]
    public GameObject[] enableIfExit;
    public GameObject[] disableIfExit;

    [Header("SFX")]
    public AudioClip[] playSFXOnEnter;
    public AudioClip[] playSFXOnExit;
    public float volume = 1f;
    public bool playAtPosition;
    public bool ignoreAudio;

    [Header("Effects:")]
    public MeshFilter meshFx;
    public bool useMeshEffects;
   
    public bool markDynamic = true;
    public Vector3 scaleFactor= new Vector3(1f,1f,1f);
    public Vector3 offsetFactor= new Vector3(0f,0f,0f);
    Vector3[] verts;
    Vector2[] uvs;
    Vector3[] vertsScaled;
    int[] tris;

    [Header("Rumble on Collision")]
    public bool useRumble=false;
    public int useCurveID=1;
    public float rumbleDuration=0.2f;
    public float rumbleMultiplier=0.8f;
    [Header("Screenshake")]
    public bool useScreenShake;
    public float power = 0.1f;
    public float duration = 0.1f;
    [Header("other:")]
    public Transform myTransform;
    public bool LookAtTriggerOnCollision;
    public Transform lookAtObject;
    [Header("Special")]
    public bool disableAttackZone;
    //register events:
    List<System.Action> actionsOnCollision= new List<System.Action>();
    private void OnDestroy()
    {
        actionsOnCollision = null;
    }
    public void RegisterActionOnCollision(System.Action newAction)
    {
        actionsOnCollision.Add(newAction);
    }
  
    void Start()
    {
        if (GameManager.instance == null) return;
        checkTagsHashed= MultiTags.GetHashedTagsArray(checkTags.ToArray());
        if (includeHero)
        {
            TriggerIfTouching.Add(GameManager.instance.Player.gameObject);
        }
        
       
        if (gameObject.isStatic)
            return;
        if (useMeshEffects && meshFx == null)
            meshFx = this.GetComponent<MeshFilter>();
        
        if (meshFx != null)
        {
            tris = meshFx.mesh.triangles;
            verts =GameMath.CopyVertorArray(meshFx.mesh.vertices);
            uvs = meshFx.mesh.uv;
            if (useMeshEffects)
            {
                vertsScaled = GameMath.CopyVertorArray(meshFx.mesh.vertices);
                for (int i = 0; i < vertsScaled.Length; i++)
                {
                    
                    vertsScaled[i].x *= scaleFactor.x;
                    vertsScaled[i].y *= scaleFactor.y;
                    vertsScaled[i].z *= scaleFactor.z;
                    //offset:
                    vertsScaled[i].x += offsetFactor.x;
                    vertsScaled[i].y += offsetFactor.y;
                    vertsScaled[i].z += offsetFactor.z;

                }

            }
           
        }
        if (exitOnStart)
        {
            SuccessOnExit();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        OnTriggerEnterGO(col.gameObject);
    }
    bool CheckTag(GameObject col)
    {
        MultiTags mt = col.GetComponent<MultiTags>();
        if (mt != null)
        {
            /*
             * Simple tag checks can be done with strings, but it's faster to check ints, so I'll use the hashed list instead:
             * 
             * String check example:
            for (int i = 0; i < mt.Tags.Length; i++)
            {
                string s = mt.Tags[i];
                for (int y = 0; y < checkTags.Count; y++)
                {
                    string t = checkTags[y];
                    if (s == t)
                    {
                        return true;
                    }
                }
            }
             */
            for (int i = 0; i < mt.TagsHashed.Length; i++)
            {
                int t1 = mt.TagsHashed[i];
                for (int y = 0; y < checkTagsHashed.Length; y++)
                {
                    int t2 = checkTagsHashed[y];
                    if (t1 == t2)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    void OnTriggerEnterGO(GameObject col)
    {
       if(CheckMultitag)
        {
            if (CheckTag(col))
            { 
                SuccessOnEnter(col);
                return;
            }
        }
        foreach (GameObject g in TriggerIfTouching)
        {
            if (col.gameObject == g)
            {
                SuccessOnEnter(g);
                return;
            }
        }
       
    }
    public void CancelSuccess()
    {
        Ackk.GameEngine.Helpers.GameObjectOperations.EnableDisableGameObjects(enableIfEnter, false);
        Ackk.GameEngine.Helpers.GameObjectOperations.EnableDisableGameObjects(disableIfEnter, true);
    }
    void SuccessOnEnter(GameObject g)
    {
        Ackk.GameEngine.Helpers.GameObjectOperations.EnableDisableGameObjects(enableIfEnter, true);
        Ackk.GameEngine.Helpers.GameObjectOperations.EnableDisableGameObjects(disableIfEnter, false);


        if (playAtPosition)
        {
            AudioManager.Instance.PlaySfx(playSFXOnEnter,transform.position,volume);
        }
        else
        {
            AudioManager.Instance.PlaySfx(playSFXOnEnter,volume);
        }

        if (useMeshEffects)
        {
            meshFx.mesh.Clear();
            //  meshFx.mesh.vertices = vertsScaled;
            meshFx.mesh.SetVertices(GameMath.VectorArrayToList(vertsScaled));
            meshFx.mesh.triangles = tris;
            meshFx.mesh.uv = uvs;
            meshFx.mesh.RecalculateBounds();
            meshFx.mesh.RecalculateNormals();

        }
        if (LookAtTriggerOnCollision)
        {
            if (myTransform == null) myTransform = this.transform;
            if (lookAtObject == null) lookAtObject = g.transform;
            /*
            Vector3 targetPoint = new Vector3(g.transform.position.x, myTransform.position.y, g.transform.position.z) - myTransform.position;
            Quaternion targetRotation = Quaternion.LookRotation(-targetPoint, Vector3.up);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, 1);*/
            var targetPosition = lookAtObject.transform.position;
            targetPosition.y = myTransform.position.y;
            myTransform.LookAt(targetPosition);


        }
        foreach (System.Action a in actionsOnCollision)
        {
            if (a != null)
            {
                a();
            }

        }
    }
    void OnTriggerExit(Collider col)
    {
        OnTriggerExitGO(col.gameObject);
     
    }
    void OnTriggerExitGO(GameObject col)
    {
        if (doNotExitIfTouchingSameTag)
        {
           /* if (GameManagerChibi.instance.GetPlayerTouchList().IsTouchingObjectWithTag(this.tag,this.gameObject))
            {
                return;
            }*/
        }
       
        foreach (GameObject g in TriggerIfTouching)
        {
            if (col.gameObject == g)
            {
                SuccessOnExit();
                
                return;
            }
        }

    }
    void SuccessOnExit()
    {
        if (playAtPosition)
        {
            if (!ignoreAudio) AudioManager.Instance.PlaySfx(playSFXOnExit, transform.position, volume);
        }
        else
        {
            if(!ignoreAudio) AudioManager.Instance.PlaySfx(playSFXOnExit, volume);
        }

        Ackk.GameEngine.Helpers.GameObjectOperations.EnableDisableGameObjects(enableIfExit, true);
        Ackk.GameEngine.Helpers.GameObjectOperations.EnableDisableGameObjects(disableIfExit, false);
        // AudioManger.instance.PlaySFX(playSFXOnExit);
        if (useMeshEffects)
        {
            if (markDynamic) meshFx.mesh.MarkDynamic();
            meshFx.mesh.Clear();
            meshFx.mesh.SetVertices(GameMath.VectorArrayToList(verts));
            meshFx.mesh.triangles = tris;
            meshFx.mesh.uv = uvs;
            meshFx.mesh.RecalculateBounds();
            meshFx.mesh.RecalculateNormals();
        }
    }

}
