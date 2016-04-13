using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Notification : MonoBehaviour
{

    public Image iconImage;
    public Text titleText;
    public Text descText;
    [HideInInspector]
    public string title, desc;
    [HideInInspector]
    public Sprite icon;

    private float disposeTimer;
    private bool disposing;
    private Animation animation;

	void Start ()
    {
        animation = GetComponentInChildren<Animation>();
        animation.Play("notification_start");
        disposeTimer = Time.time + 1;
	}
	
	void Update ()
    {
        if (icon)
            iconImage.sprite = icon;
        else
            iconImage.gameObject.SetActive(false);

        titleText.text = title;
        descText.text = desc;

	    if(Time.time >= disposeTimer)
        {
            if (!disposing)
                Dispose();
            else
                Destroy(gameObject);
        }   
	}

    void Dispose()
    {
        disposing = true;
        disposeTimer = Time.time + 1.5f;
        animation.Play("notification_stop");
    }

    public static void Notify(string title = "", string desc= "", Sprite icon = null)
    {
        GameObject notificationObj = Instantiate(Resources.Load("Notification")) as GameObject;
        Notification notificationComponent = notificationObj.GetComponent<Notification>();
        notificationComponent.title = title;
        notificationComponent.desc = desc;
        notificationComponent.icon = icon;
    }
}
