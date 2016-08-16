using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Pulse : MonoBehaviour
{

    public enum Type { Image, Text};
    public Type type;

    public Vector2 alphaSpread;
    public float speed;

    private Color currentAlpha;
    private Image image;
    private Text text;
    public bool increasing;

    void Start()
    {
        image = GetComponent<Image>();
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (!image && type == Type.Image)
            return;

        if (!text && type == Type.Text)
            return;


        if (type == Type.Image)
            currentAlpha = image.color;
        else if (type == Type.Text)
            currentAlpha = text.color;
        float newAlpha = currentAlpha.a * 255;

        if (newAlpha < alphaSpread.x && !increasing)
            increasing = !increasing;
        if(newAlpha > alphaSpread.y && increasing)
            increasing = !increasing;

        newAlpha += ((increasing) ? speed : -speed) * Time.deltaTime;

        currentAlpha.a = newAlpha / 255;

        if (type == Type.Image)
            image.color = currentAlpha;
        else if (type == Type.Text)
            text.color = currentAlpha;
    }
}
