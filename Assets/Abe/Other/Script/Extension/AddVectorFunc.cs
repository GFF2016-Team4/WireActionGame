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

    /// <summary>
    /// 線分との距離
    /// </summary>
    /// <param name="self"></param>
    /// <param name="linePoint">線分の始点</param>
    /// <param name="lineDir">線分の向き</param>
    /// <returns>自身の座標から線分に降ろした垂線のベクトル</returns>
    public static Vector3 DistanceToLine(this Vector3 self, Ray ray)
    {
        ray.direction.Normalize();
        Vector3 v = self - ray.origin;
        float   d = Vector3.Dot(v, ray.direction);
        return  -v + ray.direction * d;
    }

    public static Vector3 DistanceToLine(this Vector3 self, Vector3 lineStart, Vector3 lineEnd)
    {
        Ray ray = new Ray();
        ray.origin    = lineStart;
        ray.direction = lineEnd - lineStart;
        return self.DistanceToLine(ray);
    }
}