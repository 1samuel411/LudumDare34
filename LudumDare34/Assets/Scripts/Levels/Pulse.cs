using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Pulse : MonoBehaviour
{

    public enum Type { Image, Text};
    public Type type;

    public Vector2 alphaSpread;
    public Vector2 scaleSpreadX;
    public Vector2 scaleSpreadY;
    public float speedScale;
    public float speed;

    private Color currentAlpha;
    private Image image;
    private Text text;
    public bool increasing;
    public bool increasingX;
    public bool increasingY;

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

        if (scaleSpreadX != Vector2.zero)
        {
            Vector2 newScale = transform.localScale;
            if (newScale.x < scaleSpreadX.x && !increasingX)
                increasingX = !increasingX;
            if (newScale.x > scaleSpreadX.y && increasingX)
                increasingX = !increasingX;
            newScale.x += (((increasingX) ? speedScale : -speedScale) * Time.deltaTime);

            transform.localScale = newScale;
        }

        if (scaleSpreadY != Vector2.zero)
        {
            Vector2 newScale = transform.localScale;
            if (newScale.y < scaleSpreadY.x && !increasingY)
                increasingY = !increasingY;
            if (newScale.y > scaleSpreadY.y && increasingY)
                increasingY = !increasingY;
            newScale.y += (((increasingY) ? speedScale : -speedScale) * Time.deltaTime);

            transform.localScale = newScale;
        }
    }
}
