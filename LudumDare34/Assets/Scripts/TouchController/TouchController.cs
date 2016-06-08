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
    }

    void InitializeConfirm()
    {
        _isInitialized = true;
    }

    // Object
    private Vector2 _beginTouchPosition;
    private Vector2 _endedTouchPosition;
    private Vector2 _difference;
    private bool _began, _ended, _tapInitialized;
    private float _swipeDifference = 15.0f;
    private float _swipeTime = 0.3f;
    private float _swipeTimer;
    private float _screenWidthHalf, _screenHeightHalf;

    public float screenWidthHalf { get { return _screenWidthHalf; } }
    public float screenHeightHalf { get { return _screenHeightHalf; } }

    void Awake() {
        InitializeConfirm();
        _screenWidthHalf = Screen.width / 2;
        _screenHeightHalf = Screen.height / 2;
    }

    void Update()
    {
        if (Input.touchCount <= 0)
            return;

        int touchId = (Input.touchCount == 1) ? 0 : 1;
        if (Input.GetTouch(touchId).phase == TouchPhase.Began)
        {
            _began = true;
            _ended = false;
            _swipeTimer = Time.time + _swipeTime;
            _beginTouchPosition = Input.GetTouch(0).position;
        }
        if (Input.GetTouch(touchId).phase == TouchPhase.Ended || Time.time > _swipeTimer)
        {
            if (!_ended)
            {
                _tapInitialized = true;
                _began = false;
                _ended = true;
                _endedTouchPosition = Input.GetTouch(touchId).position;
                _difference = _endedTouchPosition - _beginTouchPosition;
            }
        }
    }

    // Swiping
    public bool GetSwipe(SwipeLocations location, bool clear = false)
    {
        bool swiped = false;
        if (!_tapInitialized || Time.time > _swipeTimer || !_ended)
            return false;

        if (location == SwipeLocations.Down && _difference.y <= -_swipeDifference)
            swiped = true;
        else if (location == SwipeLocations.Up && _difference.y >= _swipeDifference)
            swiped = true;
        else if(location == SwipeLocations.Left && _difference.x <= -_swipeDifference)
            swiped = true;
        else if(location == SwipeLocations.Right && _difference.x >= _swipeDifference)
            swiped = true;

        if (clear)
            _ended = false;

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
                touching = (position.x <= (_screenWidthHalf - xIgnoreDist)) ? 
                    (location == TouchLocations.Left) : 
                    (position.x >= (_screenWidthHalf + xIgnoreDist)) && (location == TouchLocations.Right);

            // Up and Down
            if (location == TouchLocations.Up || location == TouchLocations.Down)
                if (position.x <= (Screen.width - xIgnoreDist) && position.x >= xIgnoreDist)
                    touching = (position.y <= (_screenHeightHalf - yIgnoreDist)) ? 
                        (location == TouchLocations.Down) : 
                        (position.y >= (_screenHeightHalf + yIgnoreDist)) && ((location == TouchLocations.Up));

            // Left Quads
            if (location == TouchLocations.LowerLeft || location == TouchLocations.UpperLeft)
                if (position.x <= (_screenWidthHalf - xIgnoreDist))
                    touching = (position.y <= (_screenHeightHalf - yIgnoreDist)) ? 
                        (location == TouchLocations.LowerLeft) : 
                        (location == TouchLocations.UpperLeft) && (position.y >= (_screenHeightHalf + yIgnoreDist));

            // Right Quads
            if (location == TouchLocations.LowerRight || location == TouchLocations.UpperRight)
                if (position.x >= (_screenWidthHalf + xIgnoreDist))
                    touching = (position.y <= (_screenHeightHalf - yIgnoreDist)) ? 
                        (location == TouchLocations.LowerRight) : 
                        (location == TouchLocations.UpperRight) && (position.y >= (_screenHeightHalf + yIgnoreDist));
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
                    touching = (position.x <= (_screenWidthHalf - xIgnoreDist)) ?
                        (location == TouchLocations.Left) : 
                        (position.x >= (_screenWidthHalf + xIgnoreDist)) && (location == TouchLocations.Right);

                // Up and Down
                if (location == TouchLocations.Up || location == TouchLocations.Down)
                    if (position.x <= (Screen.width - xIgnoreDist) && position.x >= xIgnoreDist)
                        touching = (position.y <= (_screenHeightHalf - yIgnoreDist)) ?
                            (location == TouchLocations.Down) : 
                            (position.y >= (_screenHeightHalf + yIgnoreDist)) && (location == TouchLocations.Up);

                // Left Quads
                if (location == TouchLocations.LowerLeft || location == TouchLocations.UpperLeft)
                    if (position.x <= (_screenWidthHalf - xIgnoreDist))
                        touching = (position.y <= (_screenHeightHalf - yIgnoreDist)) ?
                            (location == TouchLocations.LowerLeft) :
                            (location == TouchLocations.UpperLeft) && (position.y >= (_screenHeightHalf + yIgnoreDist));

                // Right Quads
                if (location == TouchLocations.LowerRight || location == TouchLocations.UpperRight)
                    if (position.x >= (_screenWidthHalf + xIgnoreDist))
                        touching = (position.y <= (_screenHeightHalf - yIgnoreDist)) ?
                            (location == TouchLocations.LowerRight) :
                            (location == TouchLocations.UpperRight) && (position.y >= (_screenHeightHalf + yIgnoreDist));
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

        Debug.Log(xIgnoreDist);

        bool touching = false;
        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (Input.touches[i].phase == TouchPhase.Ended)
            {
                Vector2 position = Input.GetTouch(i).position;
                // Left and Right
                if (location == TouchLocations.Left || location == TouchLocations.Right)
                    touching = (position.x <= (_screenWidthHalf - xIgnoreDist)) ?
                        (location == TouchLocations.Left) : 
                        (position.x >= (_screenWidthHalf + xIgnoreDist)) && (location == TouchLocations.Right);

                // Up and Down
                if (location == TouchLocations.Up || location == TouchLocations.Down)
                    if(position.x <= (Screen.width - xIgnoreDist) && position.x >= xIgnoreDist)
                        touching = (position.y <= (_screenHeightHalf - yIgnoreDist)) ?
                            (location == TouchLocations.Down) : 
                            (position.y >= (_screenHeightHalf + yIgnoreDist)) && (location == TouchLocations.Up);

                // Left Quads
                if (location == TouchLocations.LowerLeft || location == TouchLocations.UpperLeft)
                    if (position.x <= (_screenWidthHalf - xIgnoreDist))
                        touching = (position.y <= (_screenHeightHalf - yIgnoreDist)) ?
                            (location == TouchLocations.LowerLeft) : 
                            (location == TouchLocations.UpperLeft) && (position.y >= (_screenHeightHalf + yIgnoreDist));

                // Right Quads
                if (location == TouchLocations.LowerRight || location == TouchLocations.UpperRight)
                    if (position.x >= (_screenWidthHalf + xIgnoreDist))
                        touching = (position.y <= (_screenHeightHalf - yIgnoreDist)) ? 
                            (location == TouchLocations.LowerRight) : 
                            (location == TouchLocations.UpperRight) && (position.y >= (_screenHeightHalf + yIgnoreDist));
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