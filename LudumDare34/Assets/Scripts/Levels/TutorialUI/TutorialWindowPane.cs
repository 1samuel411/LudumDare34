using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TutorialWindowPane : MonoBehaviour {

    public float fadeTime = 1.0f;
    public bool IsLeftSide;
    private Image _image;
    private float _startingAlpha = 0.0f;

    public void Awake() {
        _image = GetComponent<Image>();
        _startingAlpha = _image.color.a;
        _image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
            TouchController.controller.screenWidthHalf);
        StartCoroutine(FadeInAndOut());
    }

    public void Start() {
        float horizontalDistance = (IsLeftSide) ? 0 : TouchController.controller.screenWidthHalf;
        Debug.Log("Hor Distance: " + horizontalDistance);
        _image.rectTransform.position = new Vector3(horizontalDistance, _image.transform.position.y);
    }

    public IEnumerator FadeInAndOut() {
        while (true) {
            _image.CrossFadeAlpha(0.1f, fadeTime, false);
            yield return new WaitForSeconds(fadeTime + 0.1f);
            _image.CrossFadeAlpha(_startingAlpha, fadeTime, false);
            yield return new WaitForSeconds(fadeTime + 0.1f);
        }
    }
}
