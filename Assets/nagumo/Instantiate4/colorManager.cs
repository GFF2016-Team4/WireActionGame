using UnityEngine;
using System.Collections;

public class colorManager : MonoBehaviour {

	[Header("徐々にアルファ値を変更する時の速さ")]
	public float slowlyAlpha;

	Color m_Color;
	Color m_resetColor;

	void Start()
	{
		m_Color = new Color (0, 0, 0, slowlyAlpha);
		m_resetColor = new Color (0, 0, 0, 1);
	}

	//徐々に減らす
	public void minusAlpha(Renderer renderer)
	{
		renderer.material.color -= m_Color;
	}
	//徐々に増やす
	public void plusAlpha(Renderer renderer)
	{
		renderer.material.color += m_Color;
	}
    //アルファ値を0に
    public void minusResetAlpha(Renderer renderer)
	{
		renderer.material.color -= m_resetColor;
	}
	//アルファ値を1に
	public void plusResetAlpha(Renderer renderer)
	{
		renderer.material.color += m_resetColor;
	}
}
