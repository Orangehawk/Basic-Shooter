using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPos : MonoBehaviour
{
    [SerializeField]
    Vector3 maxPos = new Vector3(10, 10, 10);
    [SerializeField]
    float interval = 5;
    [SerializeField]
    bool forceUpdate = false;

    float lastChange = 0;

    void MoveToNewPos()
	{
        bool successfullyMoved = false;

        while (!successfullyMoved)
        {
            Vector3 newPos = new Vector3(Random.Range(-maxPos.x, maxPos.x), Random.Range(-maxPos.y, maxPos.y), Random.Range(-maxPos.z, maxPos.z));
            if(!Physics.CheckBox(newPos, Vector3.one/2))
			{
                transform.position = newPos;
                successfullyMoved = true;
                lastChange = Time.time;
			}
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MoveToNewPos();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > lastChange + interval || forceUpdate)
		{
            MoveToNewPos();
            forceUpdate = false;
        }
    }
}
