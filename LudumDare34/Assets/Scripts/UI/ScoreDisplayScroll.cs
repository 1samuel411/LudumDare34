using UnityEngine;
using System.Collections;

public class ScoreDisplayScroll : MonoBehaviour
{

    private float size;
    public float elementSize;
    public float subtractTotal;
    public int initialSizeMax;
    public float initialSizeSubtract;
    public bool childrenAccountant = true;
    private new RectTransform transform;

    public enum Type { vertical, horizontal };
    public Type type;

    void Start()
    {
        transform = GetComponent<RectTransform>();
        if (type == Type.horizontal)
            transform.position = new Vector2(10000, transform.position.y);
    }

    private int lastChildCount;
    void Update()
    {
        int children = transform.childCount;

        size = children * elementSize;
        size -= subtractTotal;
        if (childrenAccountant)
        {
            if (children <= initialSizeMax)
                size = 100;
            else
                size -= ((initialSizeMax - initialSizeSubtract) * elementSize);
        }
        
        transform.sizeDelta = new Vector2((type == Type.horizontal) ? size : transform.sizeDelta.x, (type == Type.vertical) ? size : transform.sizeDelta.y);
         
        if (transform.childCount != lastChildCount)
        {
            lastChildCount = transform.childCount;
            if (type == Type.horizontal)
                transform.position = new Vector2(1000, transform.position.y);
        }
    }
}
