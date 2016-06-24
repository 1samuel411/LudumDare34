using UnityEngine;
using System.Collections;

public class TouchController : MonoBehaviour
{

    // Static
    private static bool _isInitialized;
    public static bool initialized
    {
        get { return _isInitialized; }
    }
    public static TouchController controller;

    public static void Initialize()
    {
        GameObject touchControllerObj = new GameObject();
        touchControllerObj.name = "TouchController";
        touchControllerObj.AddComponent<TouchController>();
        controller = touchControllerObj.GetComponent<TouchController>();
        DontDestroyOnLoad(touchControllerObj);
    }

    void InitializeConfirm()
    {
        _isInitialized = true;
    }

    // Object
    private Vector2 beginTouchPosition;
    private Vector2 endedTouchPosition;
    private Vector2 difference;
    private bool began, ended, tapInitialized;
    private float swipeDifference = 15.0f;
    private float swipeTime = 0.3f;
    private float swipeTimer;
    private float screenWidthHalf, screenHeightHalf;

    void Awake()
    {
        InitializeConfirm();
        screenWidthHalf = Screen.width / 2;
        screenHeightHalf = Screen.height / 2;
    }

    void Update()
    {
        if (Input.touchCount <= 0)
            return;

        int touchId = (Input.touchCount == 1) ? 0 : 1;
        if (Input.GetTouch(touchId).phase == TouchPhase.Began)
        {
            began = true;
            ended = false;
            swipeTimer = Time.time + swipeTime;
            beginTouchPosition = Input.GetTouch(0).position;
        }
        if (Input.GetTouch(touchId).phase == TouchPhase.Ended || Time.time > swipeTimer)
        {
            if (!ended)
            {
                tapInitialized = true;
                began = false;
                ended = true;
                endedTouchPosition = Input.GetTouch(touchId).position;
                difference = endedTouchPosition - beginTouchPosition;
            }
        }
    }

    // Swiping
    public bool GetSwipe(SwipeLocations location, bool clear = false)
    {
        bool swiped = false;
        if (!tapInitialized || Time.time > swipeTimer || !ended)
            return false;
        if(location == SwipeLocations.Up)
        {
            Debug.Log(difference.y);
            Debug.Log(swipeDifference);
        }
        if (location == SwipeLocations.Down && difference.y <= -swipeDifference)
            swiped = true;
        else if (location == SwipeLocations.Up && difference.y >= swipeDifference)
            swiped = true;
        else if(location == SwipeLocations.Left && difference.x <= -swipeDifference)
            swiped = true;
        else if(location == SwipeLocations.Right && difference.x >= swipeDifference)
            swiped = true;

        if (clear)
            ended = false;

        return swiped;
    }

    // Touching
    public bool GetTouch(TouchLocations location, int xIgnoreDist = 0, int yIgnoreDist = 0)
    {
        if (Input.touchCount <= 0)
            return false;

        bool touching = false;
        for (int i = 0; i < Input.touches.Length; i++)
        {
            Vector2 position = Input.GetTouch(i).position;
            // Left and Right
            if (location == TouchLocations.Left || location == TouchLocations.Right)
                touching = (position.x <= (screenWidthHalf - xIgnoreDist)) ? 
                    (location == TouchLocations.Left) : 
                    (position.x >= (screenWidthHalf + xIgnoreDist)) && (location == TouchLocations.Right);

            // Up and Down
            if (location == TouchLocations.Up || location == TouchLocations.Down)
                if (position.x <= (Screen.width - xIgnoreDist) && position.x >= xIgnoreDist)
                    touching = (position.y <= (screenHeightHalf - yIgnoreDist)) ? 
                        (location == TouchLocations.Down) : 
                        (position.y >= (screenHeightHalf + yIgnoreDist)) && ((location == TouchLocations.Up));

            // Left Quads
            if (location == TouchLocations.LowerLeft || location == TouchLocations.UpperLeft)
                if (position.x <= (screenWidthHalf - xIgnoreDist))
                    touching = (position.y <= (screenHeightHalf - yIgnoreDist)) ? (location == TouchLocations.LowerLeft) ? true : false : (location == TouchLocations.UpperLeft) ? (position.y >= (screenHeightHalf + yIgnoreDist)) ? true : false : false;

            // Right Quads
            if (location == TouchLocations.LowerRight || location == TouchLocations.UpperRight)
                if (position.x >= (screenWidthHalf + xIgnoreDist))
                    touching = (position.y <= (screenHeightHalf - yIgnoreDist)) ? (location == TouchLocations.LowerRight) ? true : false : (location == TouchLocations.UpperRight) ? (position.y >= (screenHeightHalf + yIgnoreDist)) ? true : false : false;
            if (touching)
                return touching;
        }
        return touching;
    }

    public bool GetTouchDown(TouchLocations location, int xIgnoreDist = 0, int yIgnoreDist = 0)
    {
        if (Input.touchCount <= 0)
            return false;

        bool touching = false;
        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (Input.touches[i].phase == TouchPhase.Began)
            {
                Vector2 position = Input.GetTouch(i).position;
                // Left and Right
                if (location == TouchLocations.Left || location == TouchLocations.Right)
                    touching = (position.x <= (screenWidthHalf - xIgnoreDist)) ? (location == TouchLocations.Left) ? true : false : (position.x >= (screenWidthHalf + xIgnoreDist)) ? (location == TouchLocations.Right) ? true : false : false;

                // Up and Down
                if (location == TouchLocations.Up || location == TouchLocations.Down)
                    if (position.x <= (Screen.width - xIgnoreDist) && position.x >= xIgnoreDist)
                        touching = (position.y <= (screenHeightHalf - yIgnoreDist)) ? (location == TouchLocations.Down) ? true : false : (position.y >= (screenHeightHalf + yIgnoreDist)) ? (location == TouchLocations.Up) ? true : false : false;

                // Left Quads
                if (location == TouchLocations.LowerLeft || location == TouchLocations.UpperLeft)
                    if (position.x <= (screenWidthHalf - xIgnoreDist))
                        touching = (position.y <= (screenHeightHalf - yIgnoreDist)) ? (location == TouchLocations.LowerLeft) ? true : false : (location == TouchLocations.UpperLeft) ? (position.y >= (screenHeightHalf + yIgnoreDist)) ? true : false : false;

                // Right Quads
                if (location == TouchLocations.LowerRight || location == TouchLocations.UpperRight)
                    if (position.x >= (screenWidthHalf + xIgnoreDist))
                        touching = (position.y <= (screenHeightHalf - yIgnoreDist)) ? (location == TouchLocations.LowerRight) ? true : false : (location == TouchLocations.UpperRight) ? (position.y >= (screenHeightHalf + yIgnoreDist)) ? true : false : false;
                if (touching)
                    return touching;
            }
        }
        return touching;
    }

    public bool GetTouchUp(TouchLocations location, int xIgnoreDist = 0, int yIgnoreDist = 0)
    {
        if (Input.touchCount <= 0)
            return false;

        bool touching = false;
        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (Input.touches[i].phase == TouchPhase.Ended)
            {
                Vector2 position = Input.GetTouch(i).position;
                // Left and Right
                if (location == TouchLocations.Left || location == TouchLocations.Right)
                    touching = (position.x <= (screenWidthHalf - xIgnoreDist)) ? (location == TouchLocations.Left) ? true : false : (position.x >= (screenWidthHalf + xIgnoreDist)) ? (location == TouchLocations.Right) ? true : false : false;

                // Up and Down
                if (location == TouchLocations.Up || location == TouchLocations.Down)
                    if(position.x <= (Screen.width - xIgnoreDist) && position.x >= xIgnoreDist)
                        touching = (position.y <= (screenHeightHalf - yIgnoreDist)) ? (location == TouchLocations.Down) ? true : false : (position.y >= (screenHeightHalf + yIgnoreDist)) ? (location == TouchLocations.Up) ? true : false : false;

                // Left Quads
                if (location == TouchLocations.LowerLeft || location == TouchLocations.UpperLeft)
                    if (position.x <= (screenWidthHalf - xIgnoreDist))
                        touching = (position.y <= (screenHeightHalf - yIgnoreDist)) ? (location == TouchLocations.LowerLeft) ? true : false : (location == TouchLocations.UpperLeft) ? (position.y >= (screenHeightHalf + yIgnoreDist)) ? true : false : false;

                // Right Quads
                if (location == TouchLocations.LowerRight || location == TouchLocations.UpperRight)
                    if (position.x >= (screenWidthHalf + xIgnoreDist))
                        touching = (position.y <= (screenHeightHalf - yIgnoreDist)) ? (location == TouchLocations.LowerRight) ? true : false : (location == TouchLocations.UpperRight) ? (position.y >= (screenHeightHalf + yIgnoreDist)) ? true : false : false;
                if (touching)
                    return touching;
            }
        }
        return touching;
    }
}

public enum TouchLocations
{
    UpperLeft,
    UpperRight,
    LowerLeft,
    LowerRight,
    Left,
    Right,
    Up,
    Down
}

public enum SwipeLocations
{
    Up,
    Down,
    Right,
    Left
}