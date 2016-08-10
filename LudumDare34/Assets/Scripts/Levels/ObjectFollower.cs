using UnityEngine;
using System.Collections;

public class ObjectFollower : MonoBehaviour
{

    public Transform original;

    private Transform follower;

    public int scaleModifier = 1;
    private int _scaleModifier = 1;

    void Start()
    {
        follower = this.transform;
    }

    void Update()
    {
        Transform parent = original;
        while(parent.localScale.x > 0)
        {
            if (parent.parent)
                parent = parent.parent;
            else
                break;
        }
        scaleModifier = (parent.localScale.x < 0) ? -1 : 1;
        if (_scaleModifier != scaleModifier)
        {
            follower.localScale = new Vector2(-follower.localScale.x, follower.localScale.y);
            _scaleModifier = scaleModifier;
        }
        follower.rotation = (scaleModifier == -1) ? Quaternion.Inverse(original.rotation) : original.rotation;
        follower.position = original.position;
    }
}
