using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif

public class ParticleLayers : MonoBehaviour
{

    public string layerNames = "Player";
    public int layerOrder;
    private ParticleSystem particleRenderer;

    void Awake()
    {
        particleRenderer = GetComponent<ParticleSystem>();
        particleRenderer.GetComponent<Renderer>().sortingOrder = layerOrder;
        particleRenderer.GetComponent<Renderer>().sortingLayerName = layerNames;
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!particleRenderer)
            particleRenderer = GetComponent<ParticleSystem>();
        particleRenderer.GetComponent<Renderer>().sortingOrder = layerOrder;
        particleRenderer.GetComponent<Renderer>().sortingLayerName = layerNames;
    }
#endif
}