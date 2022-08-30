using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMarkerManager : MonoBehaviour
{
    [SerializeField]
    GameObject enemyMarker;
    
    RectTransform map;
    Dictionary<Transform, RectTransform> minimapMarkers; //<Enemy, EnemyMarker>
    float xScale, yScale;

    // Start is called before the first frame update
    void Start()
    {
        map = Minimap.instance.GetMap();
        minimapMarkers = new Dictionary<Transform, RectTransform>();

        //xScale = minimap.sizeDelta.x / 100f;
        //yScale = minimap.sizeDelta.y / 100f;
        xScale = map.rect.width / 100f;
        yScale = map.rect.height / 100f;
    }

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Enemy"))
		{
            AddMarker(other.transform);
		}
        else
		{
            Debug.Log($"Hit but not adding {other.name} to minimap | {other.GetType()}");
        }
    }

	void OnTriggerExit(Collider other)
	{
        if (other.CompareTag("Enemy"))
        {
            RemoveMarker(other.transform);
        }
        else
        {
            Debug.Log($"Lost but not removing {other.name} from minimap | {other.GetType()}");
        }
    }

	public void AddMarker(Transform enemy)
	{
        if(!minimapMarkers.ContainsKey(enemy))
		{
            Debug.Log($"Adding {enemy.name} to minimap");
            minimapMarkers.Add(enemy, Instantiate(enemyMarker, new Vector3(-enemy.position.x, -enemy.position.z, 0), Quaternion.identity, map).GetComponent<RectTransform>());
		}
	}

    public void RemoveMarker(Transform enemy)
	{
        if (minimapMarkers.ContainsKey(enemy))
        {
            Debug.Log($"Removing {enemy.name} from minimap");
            Destroy(minimapMarkers[enemy].gameObject);
            minimapMarkers.Remove(enemy);
        }
        else
		{
            Debug.Log($"Failed to remove {enemy.name} from minimap");
		}
    }

    public void UpdateMarkers()
	{
        foreach(Transform enemy in minimapMarkers.Keys)
		{
            Vector2 markerPos = minimapMarkers[enemy].anchoredPosition;
            markerPos.x = -enemy.localPosition.x * xScale;
            markerPos.y = -enemy.localPosition.z * yScale;
            minimapMarkers[enemy].anchoredPosition = markerPos;
		}
	}

    // Update is called once per frame
    void Update()
    {
        UpdateMarkers();
    }
}
