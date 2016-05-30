using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using System.Collections;
using Amazon.CognitoSync.SyncManager;
using UnityEngine.SceneManagement;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using Amazon;

public class CameraManager : MonoBehaviour
{

    public Image fadeImg;
    public float fadeSpeed;
    public bool fadeOut;
    public Color fadeImgColor;

    public static Camera ourCam;
    public static CameraManager instance;

    public float maxLeft;
    public float maxRight;

    private float screenshakeRegSpeed = 2;
    private float screenshakeAmount = 0;
    private float screenshakeReduceAmount = 0;
    
    private float _zoomSpeed = 1;
    private float _regZoom = 5;
    private float _currentZoom = 5;
    private float _targetZoom = 5;

    private float _timeScale = 1;
    private float _curTimeScale = 1;
    private float _targetTimeScale = 1;
    private float _timeScaleSpeed = 1;

    private float _timeToWaitZoom = 0;
    private float _curTimeToWaitZoom = 0;
    private float _targetTimeToWaitZoom = 0;

    private float _moveSpeed;
    private Vector3 _targetPosition;

    private bool _zoomingIn;
    private bool _zoomingOut;
    private bool _died;

    private Vector3 _position;

    public bool loading;
    public float timeToWait;
    public string levelToLoad;

    private new Transform transform;
    private VignetteAndChromaticAberration vignette;

	private GameManager _gameManager;
	public GameManager gameManager {
		get {
			if(_gameManager == null) {
				_gameManager = this.gameObject.GetComponent<GameManager>();
			}
			return _gameManager;
		}
	}

    void Awake() {
        transform = GetComponent<Transform>();
        instance = this;
        ourCam = this.GetComponent<Camera>();
        _position = transform.position;
        _regZoom = ourCam.orthographicSize;
        vignette = GetComponent<VignetteAndChromaticAberration>();
        fadeImgColor = Color.black;
        fadeImgColor.a = 1;

        if (loading) {
#if UNITY_ANDROID || UNITY_IOS
            gameManager.AuthenticateAndLoad(LoadLevel());
#else 
            StartCoroutine(LoadLevel());
#endif
        }
    }

    IEnumerator LoadLevel() {
        Debug.Log("Triggered LoadLevel");
        FadeOut();
        yield return new WaitForSeconds(0.6f);
        SceneManager.LoadScene(levelToLoad);
    }

    void Update()
    {
        if(fadeOut)
        {
            fadeImgColor.a += fadeSpeed * Time.deltaTime;
        }
        else
        {
            // Fade in at start
            fadeImgColor.a -= fadeSpeed * Time.deltaTime;
        }

		if(fadeImg != null)
			fadeImg.color = fadeImgColor;

        if (!PlayerController.instance)
            return;

        if (PlayerController.instance.baseHealth._died)
        {
            _curTimeScale = 0.9f;
            _targetTimeToWaitZoom = Time.time + 9999;
            vignette.intensity += 4 * Time.deltaTime;
        }
        _position.x = PlayerController.instance.transform.position.x;
        _position.x = Mathf.Clamp(_position.x, maxLeft, maxRight);
        if(!_zoomingIn && !_zoomingOut)
            transform.position = Vector3.Lerp(transform.position, new Vector3(_position.x + Random.Range(-screenshakeAmount, screenshakeAmount), _position.y + Random.Range(-screenshakeAmount, screenshakeAmount), -10), screenshakeRegSpeed * Time.deltaTime);

        // Handle screenshake
        screenshakeAmount -= screenshakeReduceAmount * Time.deltaTime;
        screenshakeAmount = Mathf.Clamp(screenshakeAmount, 0, screenshakeAmount);

        if(_zoomingIn)
        {
            // Handle zooming
            _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, _zoomSpeed * Time.deltaTime);
            _curTimeScale = Mathf.Lerp(_curTimeScale, _targetTimeScale, _timeScaleSpeed * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, new Vector3(_targetPosition.x + Random.Range(-screenshakeAmount, screenshakeAmount), _targetPosition.y + Random.Range(-screenshakeAmount, screenshakeAmount), -10), _moveSpeed * Time.deltaTime);
            ourCam.orthographicSize = _currentZoom;
            Time.timeScale = _curTimeScale;
            _curTimeToWaitZoom += Time.deltaTime/Time.timeScale;

            if(_curTimeToWaitZoom > _targetTimeToWaitZoom)
            {
                // Zoom out
                _targetZoom = _regZoom;
                _targetTimeScale = 1;
                _targetPosition = _position;
                _zoomingIn = false;
                _zoomingOut = true;
            }
        }
        else
        {
            // Handle zooming
            _currentZoom = Mathf.Lerp(_currentZoom, _targetZoom, _zoomSpeed * Time.deltaTime);
            _curTimeScale = Mathf.Lerp(_curTimeScale, _targetTimeScale, _timeScaleSpeed * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, new Vector3(_position.x + Random.Range(-screenshakeAmount, screenshakeAmount), _position.y + Random.Range(-screenshakeAmount, screenshakeAmount), -10), _moveSpeed * Time.deltaTime);
            ourCam.orthographicSize = _currentZoom;
            Time.timeScale = _curTimeScale;

            _curTimeToWaitZoom += Time.deltaTime;

            if (_curTimeToWaitZoom > _targetTimeToWaitZoom)
            {
                // Done zooming out
                _zoomingIn = false;
                _zoomingOut = false;
            }
        }
    }

    public static void ZoomIn(float zoomSpeed, float zoomTarget, float timeScaleSpeed, float timeScale, Vector3 targetPos, float moveSpeed, float timeTaken)
    {
        if (!CameraManager.instance || LevelManager.instance.player.baseHealth._died)
            return;

        CameraManager.instance._zoomingIn = true;
        CameraManager.instance._targetTimeToWaitZoom = Time.time + timeTaken;
        CameraManager.instance._curTimeToWaitZoom = Time.time;
        CameraManager.instance._targetZoom = zoomTarget;
        CameraManager.instance._zoomSpeed = zoomSpeed;
        CameraManager.instance._timeScaleSpeed = timeScaleSpeed;
        CameraManager.instance._targetTimeScale = timeScale;
        CameraManager.instance._targetPosition = targetPos;
        CameraManager.instance._moveSpeed = moveSpeed;

        CameraManager.instance._currentZoom = CameraManager.ourCam.orthographicSize;
        CameraManager.instance._curTimeScale = Time.timeScale;
    }

    public static void ShakeScreen(float amount, float reduceAmount)
    {
        if (!CameraManager.instance || LevelManager.instance.player.baseHealth._died)
            return;
        CameraManager.instance.screenshakeAmount = amount;
        CameraManager.instance.screenshakeReduceAmount = reduceAmount;
    }

    public void FadeOut()
    {
        fadeImgColor.a = 0;
        fadeOut = true;
    }
}
