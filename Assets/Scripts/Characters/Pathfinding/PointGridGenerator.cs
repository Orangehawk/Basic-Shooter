using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGridGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject node;
    [SerializeField]
    GameObject nodeParent;

    [SerializeField]
    Vector3 maxPos = new Vector3(50, 1, 50);
    [SerializeField]
    Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField]
    float increments = 1;
    [SerializeField]
    bool regenerate = false;

    void GenerateGrid()
    {
        for(float i = -maxPos.x; i < maxPos.x; i += increments)
		{
            for (float j = -maxPos.z; j < maxPos.z; j += increments)
            {
                Vector3 pos = new Vector3(i, maxPos.y, j);

                int layerMask = LayerMask.GetMask("Obstacles");

                if (!Physics.CheckBox(pos, halfExtents, Quaternion.identity, layerMask))
                {
                    Instantiate(node, pos, Quaternion.identity, nodeParent.transform);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(!nodeParent)
            nodeParent = new GameObject("Nodes");
        GenerateGrid();
        bool regenerate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(regenerate)
		{
            foreach(Transform child in nodeParent.transform)
			{
                Destroy(child.gameObject);
			}

            GenerateGrid();
            regenerate = false;
		}
    }
}
