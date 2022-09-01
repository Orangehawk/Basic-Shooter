using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitManager : MonoBehaviour
{
    public static BulletHitManager instance;

    [SerializeField]
    BulletHit defaultBulletHit;
    [SerializeField]
    List<BulletHit> bulletHits = new List<BulletHit>();
    [SerializeField]
    Dictionary<PhysicMaterial, BulletHit> bullethitDictionary = new Dictionary<PhysicMaterial, BulletHit>();

	void Awake()
	{
		if(instance != null && instance != this)
		{
            Debug.LogWarning($"Duplicate BulletHitManager! Destroying {name}");
            Destroy(gameObject);
		}
        else
		{
            instance = this;
		}

        foreach(BulletHit hit in bulletHits)
		{
            bullethitDictionary.Add(hit.material, hit);
		}
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    public BulletHit GetBulletHit(PhysicMaterial material)
	{
        if (material != null && bullethitDictionary.TryGetValue(material, out BulletHit hit))
		{
            return hit;
        }

        return defaultBulletHit ?? null;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Serializable]
    public class BulletHit
	{
        public PhysicMaterial material;
        public GameObject particle;
        public AudioClip sound;
	}

}
