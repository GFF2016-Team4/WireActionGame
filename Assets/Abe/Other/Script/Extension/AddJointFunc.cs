using UnityEngine;

//Jointの関数を増やす
public static class AddJointFunc
{
    public static bool IsRootJoint(this Joint joint)
    {
        return joint.connectedBody == null;
    }

    public static Joint GetParentJoint(this Joint joint)
    {
        return joint.connectedBody.GetComponent<Joint>();
    }
}
