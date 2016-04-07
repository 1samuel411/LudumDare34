using UnityEngine;
using System.Collections;
using SVGImporter;
using UnityEngine.UI;

public class WeaponElement : MonoBehaviour
{

    public SVGImage background;
    public SVGImage foreground;
    public SVGAsset targetImage;
    public GameObject artParent;
    public Image foregroundImageMask;
    public float scale;
    public float rotation;
    public BaseWeapon reference;

    void Start()
    {
        background.rectTransform.sizeDelta = new Vector2(scale, background.rectTransform.sizeDelta.y);
        foreground.rectTransform.sizeDelta = new Vector2(scale, foreground.rectTransform.sizeDelta.y);
        transform.localScale = new Vector2(1, 1);
        foreground.vectorGraphics = targetImage;
        background.vectorGraphics = targetImage;
        foregroundImageMask.rectTransform.sizeDelta = new Vector2(scale, foregroundImageMask.rectTransform.sizeDelta.y);
    }

    void Update()
    {
        if(reference.weaponAttribute.usingAmmo)
        {
            if (reference.weaponAttribute.currentAmmo <= 0)
            {
                Destroy(gameObject);
            }
            foregroundImageMask.fillAmount = (float)reference.weaponAttribute.currentAmmo / (float)reference.weaponAttribute.currentMaxAmmo;
        }

        if(reference.weaponAttribute.usingTimer)
        {
            if(reference.weaponAttribute.CurAlottedTime <= 0)
            {
                Destroy(gameObject);
            }
            foregroundImageMask.fillAmount = reference.weaponAttribute.CurAlottedTime / reference.weaponAttribute.CurMaxAlottedTime;
        }
    }
}
