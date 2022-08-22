using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailMaker : MonoBehaviour
{
    [SerializeField]
    TrailRenderer trailRenderer;

    // Start is called before the first frame update
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.autodestruct = true;
        trailRenderer.startColor = Color.green;
        trailRenderer.endColor = Color.red;
        trailRenderer.startWidth = 0.01f;
        trailRenderer.endWidth = 0.01f;
    }
}
