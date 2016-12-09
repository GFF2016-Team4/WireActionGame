using UnityEngine;
using System.Collections;

public class alfaChange : MonoBehaviour
{

    Renderer m_renderer;
    public Color c;
    public float alfa;

    void Start()
    {
        m_renderer = GetComponent<Renderer>();
        c.a = alfa;
    }

    public void alfaminus(Renderer rend)
    {
        rend.material.color -= c;
    }

}
