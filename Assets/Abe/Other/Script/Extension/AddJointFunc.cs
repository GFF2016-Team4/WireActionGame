using UnityEngine;

//Jointの関数を増やす
public static class AddJointFunc
{
    public static bool IsRootJoint(this Joint joint)
    {
        return joint.connectedBody == null;
    }
}
