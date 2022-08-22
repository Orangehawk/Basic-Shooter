using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFlash : MonoBehaviour
{
	[SerializeField]
	Image image;
	[SerializeField]
	float increaseAmount = 0.05f;
	[SerializeField]
	float fadeSpeed = 0.05f;
	[SerializeField]
	float maxFlash = 0.3f;

	// Start is called before the first frame update
	void Start()
	{

	}

	void AddImageAlpha(Image image, float amount, float max = 1, float min = 0)
	{
		Color c = image.color;

		c.a += amount;

		c.a = Mathf.Clamp(c.a, min, max);
		image.color = c;
	}

	public void Flash()
	{
		AddImageAlpha(image, increaseAmount, maxFlash);
	}

	// Update is called once per frame
	void Update()
	{
		AddImageAlpha(image, -fadeSpeed * Time.deltaTime);
	}
}
