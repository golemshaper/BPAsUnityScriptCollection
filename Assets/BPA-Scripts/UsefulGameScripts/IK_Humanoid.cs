using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Humnanoid : MonoBehaviour
{
    public bool enable = true;
    public Animator animator;
    public Transform hips;
    [Header("End effectors")]
    public Transform FootL_IK;
    public Transform FootR_IK;
    public Vector3 bodyPos=new Vector3(0,-1f,0);

    [Header("DEBUG")]
    public Transform DEBUG_L;
    public Transform DEBUG_R;

    private void OnAnimatorIK()
    {
        if (!enable) return;
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);

        //LEFT
        Vector3 foot_L= FootL_IK.position;
        Vector3 bodyVec = hips.position;
        foot_L = CastToGround(bodyVec, foot_L);

        //RIGHT
        Vector3 foot_R = FootR_IK.position;
        foot_R = CastToGround(bodyVec, foot_R);

        //SET
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, foot_L + animator.GetIKPosition(AvatarIKGoal.LeftFoot));
        animator.SetIKPosition(AvatarIKGoal.RightFoot, foot_R + animator.GetIKPosition(AvatarIKGoal.RightFoot));
        animator.bodyPosition += bodyPos;

        //DEBUG
        if (DEBUG_L != null) DEBUG_L.position = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        if (DEBUG_R != null) DEBUG_R.position = animator.GetIKPosition(AvatarIKGoal.RightFoot);
    }
    Vector3 CastToGround(Vector3 from,Vector3 footLoc)
    {
        from.x = footLoc.x;
        from.z = footLoc.z;

        Vector3 returnValue = Vector3.zero;
        RaycastHit hit = new RaycastHit();
        float dist = 40f;

        var point = new Vector3(from.x, from.y, from.z);
      
        if (Physics.Raycast(point, Vector3.down, out hit, dist))
        {
            footLoc.y = hit.point.y;
        }
       
        //zero out so only Y is added to IK Pos
       // footLoc.x = 0;
       // footLoc.z =0;
        
        return footLoc;
    }
}
