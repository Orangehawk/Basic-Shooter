using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public static Minimap instance;

    [SerializeField]
    Transform player;
    [SerializeField]
    RectTransform map;
    [SerializeField]
    RectTransform playerMarker;

    float xScale, yScale;
    float lastRotation;

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
        xScale = map.rect.width / 100f;
        yScale = map.rect.height / 100f;
        lastRotation = player.rotation.eulerAngles.y;
        playerMarker.localRotation = Quaternion.Euler(0, 0, lastRotation + 180);
    }

    public RectTransform GetMap()
	{
        return map;
	}

    // Update is called once per frame
    void Update()
    {
        Vector2 mapPos = map.anchoredPosition;
        mapPos.x = player.localPosition.x * xScale;
        mapPos.y = player.localPosition.z * yScale;
        map.anchoredPosition = mapPos;

        playerMarker.Rotate(0, 0, lastRotation - player.rotation.eulerAngles.y);
        lastRotation = player.rotation.eulerAngles.y;
    }
}
