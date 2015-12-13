using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif

public class Layers : MonoBehaviour
{

    [System.Serializable]
    public enum Type { none, trail, particle };
    public Type type;
    public string layerNames = "Player";
    public int layerOrder;
    private ParticleSystem particleRenderer;
    private TrailRenderer trailRenderer;

    void Awake()
    {
        if (type == Type.particle)
        {
            particleRenderer = GetComponent<ParticleSystem>();
            particleRenderer.GetComponent<Renderer>().sortingOrder = layerOrder;
            particleRenderer.GetComponent<Renderer>().sortingLayerName = layerNames;
        }
        if (type == Type.trail)
        {
            trailRenderer = GetComponent<TrailRenderer>();
            trailRenderer.GetComponent<TrailRenderer>().sortingOrder = layerOrder;
            trailRenderer.GetComponent<TrailRenderer>().sortingLayerName = layerNames;
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (type == Type.particle)
        {
            if (!particleRenderer)
                particleRenderer = GetComponent<ParticleSystem>();
            particleRenderer.GetComponent<Renderer>().sortingOrder = layerOrder;
            particleRenderer.GetComponent<Renderer>().sortingLayerName = layerNames;
        }
        if (type == Type.trail)
        {
            if (!trailRenderer)
                trailRenderer = GetComponent<TrailRenderer>();
            trailRenderer.GetComponent<TrailRenderer>().sortingOrder = layerOrder;
            trailRenderer.GetComponent<TrailRenderer>().sortingLayerName = layerNames;
        }
    }
#endif
}
