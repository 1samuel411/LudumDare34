using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour {
    
    //public GameObject

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

    private delegate void TutorialHandler();
    private TutorialHandler TutorialEvent;

    void Awake() {
        TutorialEvent = LeftEvent;
    }

    void Update() {
        if(TutorialEvent != null)
            TutorialEvent();
    }

    private float _timeHeld;
    private float _timeHoldLimit = 2.0f;
    public void LeftEvent() {
        if(TouchController.controller.GetTouch(TouchLocations.Left, 250)) {
            _timeHeld += Time.deltaTime;
            if (_timeHoldLimit > _timeHeld) {
                TutorialEvent = RightEvent;
            }
        } else {
            _timeHeld = 0.0f;
        }
    }

    private string displayMessage;
    public void RightEvent() {
        displayMessage = "Nice Job!";

    }

    public IEnumerable MovementLeft() {
        yield return new WaitUntil(
            () => TouchController.controller.GetTouch(TouchLocations.Left, 250)
        );
    }

    /*
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
    */
}
