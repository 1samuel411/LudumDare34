using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour
{

    public enum Axis { X, Y, Z };
    public Axis axis;

    public float speed;

    private new Transform transform;

    void Start()
    {
        transform = GetComponent<Transform>();
    }

    private Vector3 newRotation;
    void Update()
    {
        newRotation = transform.localEulerAngles;
        if(axis == Axis.X)
        {
            newRotation.x += speed * Time.deltaTime;
        }
        else if(axis == Axis.Y)
        {
            newRotation.y += speed * Time.deltaTime;
        }
        else if(axis == Axis.Z)
        {
            newRotation.z += speed * Time.deltaTime;
        }
        transform.localEulerAngles = newRotation;
    }
}
