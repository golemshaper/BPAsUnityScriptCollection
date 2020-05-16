using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToRotationOnEnable : MonoBehaviour
{
    public Transform modifyTransform;
    public Vector3 addToAnngle;
    public bool autoDisable;
    private void OnEnable()
    {
        modifyTransform.localEulerAngles += addToAnngle;
        if (autoDisable) this.gameObject.SetActive(false);
    }
}
