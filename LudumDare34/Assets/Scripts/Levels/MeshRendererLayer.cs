using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif

public class MeshRendererLayer : MonoBehaviour
{

    public string layerNames = "Player";
    public int layerOrder;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.GetComponent<Renderer>().sortingOrder = layerOrder;
        meshRenderer.GetComponent<Renderer>().sortingLayerName = layerNames;
    }

#if UNITY_EDITOR
    void Update()
    {
        if (!meshRenderer)
            meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.GetComponent<Renderer>().sortingOrder = layerOrder;
        meshRenderer.GetComponent<Renderer>().sortingLayerName = layerNames;
    }
#endif
}