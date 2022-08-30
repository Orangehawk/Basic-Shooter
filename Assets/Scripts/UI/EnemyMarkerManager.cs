using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMarkerManager : MonoBehaviour
{
    [SerializeField]
    GameObject enemyMarker;

    RectTransform map;
    Dictionary<Transform, RectTransform> minimapMarkers; //<Enemy, EnemyMarker>
    Vector2 mapScale;

    // Start is called before the first frame update
    void Start()
    {
        minimapMarkers = new Dictionary<Transform, RectTransform>();

        map = Minimap.instance.GetMap();
        mapScale = Minimap.instance.GetMapScale();

        Minimap.instance.onZoom += UpdateScale;
    }

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Enemy"))
		{
            AddMarker(other.transform);
		}
    }

	void OnTriggerExit(Collider other)
	{
        if (other.CompareTag("Enemy"))
        {
            RemoveMarker(other.transform);
        }
    }

	public void AddMarker(Transform enemy)
	{
        if(!minimapMarkers.ContainsKey(enemy))
		{
            minimapMarkers.Add(enemy, Instantiate(enemyMarker, new Vector3(-enemy.position.x, -enemy.position.z, 0), Quaternion.identity, map).GetComponent<RectTransform>());
            if(enemy.TryGetComponent(out HealthComponent hc))
			{
                hc.onDead += delegate { RemoveMarker(hc.transform); };
			}
		}
	}

    public void RemoveMarker(Transform enemy)
	{
        if (minimapMarkers.ContainsKey(enemy))
        {
            Destroy(minimapMarkers[enemy].gameObject);
            minimapMarkers.Remove(enemy);
        }
    }

    void UpdateScale()
	{
        mapScale = Minimap.instance.GetMapScale();
        UpdateMarkers();
    }

    public void UpdateMarkers()
	{
        for(int i = 0; i < minimapMarkers.Count; i++)
		{
            var item = minimapMarkers.ElementAt(i);

            Transform enemy = item.Key;
            RectTransform marker = item.Value;

			if (item.Key != null)
			{
				Vector2 markerPos = marker.anchoredPosition;
                markerPos.x = -enemy.localPosition.x * mapScale.x;
				markerPos.y = -enemy.localPosition.z * mapScale.y;
                marker.anchoredPosition = markerPos;
			}
			else
			{
				Debug.LogWarning($"Invalid minimap transform, deleting marker");
				Destroy(marker.gameObject);
				minimapMarkers.Remove(enemy);
				i--;
			}
		}
	}

    // Update is called once per frame
    void Update()
    {
        UpdateMarkers();
    }
}
