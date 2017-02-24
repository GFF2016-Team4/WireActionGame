using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class changeColor : MonoBehaviour
{

    private Color originColor = new Color(1, 1, 1);
    private Color color = new Color(1, 1, 1);

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