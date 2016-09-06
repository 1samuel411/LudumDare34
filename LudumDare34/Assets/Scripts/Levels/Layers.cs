using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif

public class Layers : MonoBehaviour
{

    [System.Serializable]
    public enum Type { none, trail, particle, mesh, skinnedMesh };
    public Type type;
    public string layerNames = "Player";
    public int layerOrder;
    private ParticleSystem particleRenderer;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;
    private SkinnedMeshRenderer skinnedMeshRenderer;

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
        if (type == Type.mesh)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.GetComponent<MeshRenderer>().sortingOrder = layerOrder;
            meshRenderer.GetComponent<MeshRenderer>().sortingLayerName = layerNames;
        }
        if (type == Type.skinnedMesh)
        {
            skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.GetComponent<SkinnedMeshRenderer>().sortingOrder = layerOrder;
            skinnedMeshRenderer.GetComponent<SkinnedMeshRenderer>().sortingLayerName = layerNames;
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
        if (type == Type.mesh)
        {
            if (!meshRenderer)
                meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.GetComponent<MeshRenderer>().sortingOrder = layerOrder;
            meshRenderer.GetComponent<MeshRenderer>().sortingLayerName = layerNames;
        }
        if (type == Type.skinnedMesh)
        {
            if (!skinnedMeshRenderer)
                skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
            skinnedMeshRenderer.GetComponent<SkinnedMeshRenderer>().sortingOrder = layerOrder;
            skinnedMeshRenderer.GetComponent<SkinnedMeshRenderer>().sortingLayerName = layerNames;
        }
    }
#endif
}
