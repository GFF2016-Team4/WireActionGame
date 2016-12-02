using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class changeColor : MonoBehaviour
{

    public Color originColor;
    public Color color;

    float A = 1.0f;

    void Start()
    {
        originColor.a = A;
        color.a = A;
    }
    public void chgColor()
    {
        GetComponent<Image>().color = color;
    }
    public void orgColor()
    {
        GetComponent<Image>().color = originColor;
    }
}