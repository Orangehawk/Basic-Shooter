using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
	public static Minimap instance;

	public event System.Action onZoom = delegate { };

	[SerializeField]
	Transform player;
	[SerializeField]
	RectTransform map;
	[SerializeField]
	RectTransform playerMarker;
	[SerializeField]
	Vector2 defaultZoom = new Vector2(300, 300);
	[SerializeField]
	float zoomStep = 20;

	Vector2 currentZoom;
	Vector2 mapScale;

	void Awake()
	{
		if (instance != null && instance != this)
		{
			Debug.LogWarning("Duplicate Minimap!");
			Destroy(gameObject);
		}
		else
		{
			instance = this;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		currentZoom = new Vector2(map.rect.width, map.rect.height);
		mapScale = GetMapScale();
		UpdateMap();
	}

	public Vector2 GetMapScale()
	{
		return new Vector2(map.rect.width / 100f, map.rect.height / 100f);
	}

	public void ResetZoom()
	{
		currentZoom = defaultZoom;
		map.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentZoom.x);
		map.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentZoom.y);
		mapScale = GetMapScale();
		onZoom();
	}

	public void Zoom(bool zoomIn)
	{
		if (zoomIn)
		{
			currentZoom.x += zoomStep;
			currentZoom.y += zoomStep;
		}
		else
		{
			currentZoom.x -= zoomStep;
			currentZoom.y -= zoomStep;
		}

		map.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentZoom.x);
		map.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentZoom.y);

		mapScale = GetMapScale();

		onZoom();
	}

	public RectTransform GetMap()
	{
		return map;
	}

	void UpdateMap()
	{
		Vector2 mapPos = map.anchoredPosition;
		mapPos.x = player.localPosition.x * mapScale.x;
		mapPos.y = player.localPosition.z * mapScale.y;
		map.anchoredPosition = mapPos;

		playerMarker.localRotation = Quaternion.Euler(0, 0, player.rotation.eulerAngles.y * -1 + 180);
	}

	// Update is called once per frame
	void Update()
	{
		UpdateMap();
	}
}
