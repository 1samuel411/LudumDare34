using UnityEngine;
using System.Collections;

public class ScoreDisplayScroll : MonoBehaviour
{

    private float size;
    public float elementSize;
    public int initialSizeMax;
    public float initialSizeSubtract;

    private new RectTransform transform;

    public enum Type { vertical, horizontal };
    public Type type;

    void Start()
    {
        transform = GetComponent<RectTransform>();
    }

    void Update()
    {
        int children = transform.childCount;
        size = children * elementSize;
        if (children <= initialSizeMax)
            size = 100;
        else
            size -= ((initialSizeMax - initialSizeSubtract) * elementSize);
        transform.sizeDelta = new Vector2((type == Type.horizontal) ? size : transform.sizeDelta.x, (type == Type.vertical) ? size : transform.sizeDelta.y);
    }
}
