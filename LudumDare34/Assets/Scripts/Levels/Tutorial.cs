using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour
{

    public static Tutorial instance;
    public bool jumped, 
                    boosted,
                    movedLeft,
                    movedRight, 
                    strafedLeft, 
                    strafedRight, 
                    toggled, 
                    movingLeft, 
                    movingRight;
    public bool finishedTutorial;

    public GameObject mainUI;

    public GameObject jumpedObj, 
                        boostedObj, 
                        moveLeftObj,
                        movedRightObj, 
                        strafedLeftObj, 
                        strafedRightObj, 
                        toggledObj;
    public Sprite completedIcon;

    void Awake()
    {
        instance = this;
        Debug.Log(InfoManager.NewPlayer());

        finishedTutorial = (InfoManager.GetInfo("finishedTutorial") == "1");
    }

    void Update()
    {
        if (finishedTutorial)
        {
            mainUI.SetActive(true);
            jumpedObj.SetActive(false);
            boostedObj.SetActive(false);
            moveLeftObj.SetActive(false);
            movedRightObj.SetActive(false);
            strafedLeftObj.SetActive(false);
            strafedRightObj.SetActive(false);
            toggledObj.SetActive(false);
        }
        else
        {
            mainUI.SetActive(false);
            if (!boosted)
                jumpedObj.SetActive(!jumped);
            else
                jumpedObj.SetActive(false);
            if (jumped)
                boostedObj.SetActive(!boosted);
            else if(!boosted)
                boostedObj.SetActive(false);

            if (!strafedLeft)
            {
                if (movingLeft)
                    moveLeftObj.SetActive(false);
                else
                    moveLeftObj.SetActive(!movingRight);
            }
            else
                moveLeftObj.SetActive(!movingLeft);

            if (!strafedRight)
            {
                if (movingRight)
                    movedRightObj.SetActive(false);
                else
                    movedRightObj.SetActive(!movingLeft);
            }
            else
                movedRightObj.SetActive(!movingRight);

            toggledObj.SetActive(!toggled);

            strafedRightObj.SetActive(movingLeft);
            strafedLeftObj.SetActive(movingRight);

            if (strafedRight)
            {
                strafedLeftObj.SetActive(false);
                movedRightObj.SetActive(false);
            }
            if (strafedLeft)
            {
                strafedRightObj.SetActive(false);
                moveLeftObj.SetActive(false);
            }

            if (boosted && strafedLeft && strafedRight && toggled)
            {
                finishedTutorial = true;
                InfoManager.SetInfo("finishedTutorial", "1");
                Notification.Notify("Controller Master!", "Completed the tutorial!", completedIcon);
            }
        }
    }

}
