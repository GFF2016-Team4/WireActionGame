using UnityEngine;

public static class AddVectorFunc
{
    /// <summary>
    /// 今の座標からの距離
    /// </summary>
    /// <param name="self"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static float Distance(this Vector3 self, Vector3 point)
    {
        return (self-point).magnitude;
    }
}