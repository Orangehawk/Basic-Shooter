using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        if(!cam)
		{
            Debug.LogError($"Camera not assigned for FaceCamera in {name}");
		}
    }

    // Update is called once per frame
    void Update()
    {
        if (cam)
        {
            transform.LookAt(cam.transform, Vector3.up);
        }
    }
}
