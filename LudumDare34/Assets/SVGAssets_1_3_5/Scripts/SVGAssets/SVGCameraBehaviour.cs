/****************************************************************************
** Copyright (C) 2013-2015 Mazatech S.r.l. All rights reserved.
**
** This file is part of SVGAssets software, an SVG rendering engine.
**
** W3C (World Wide Web Consortium) and SVG are trademarks of the W3C.
** OpenGL is a registered trademark and OpenGL ES is a trademark of
** Silicon Graphics, Inc.
**
** This file is provided AS IS with NO WARRANTY OF ANY KIND, INCLUDING THE
** WARRANTY OF DESIGN, MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
**
** For any information, please contact info@mazatech.com
**
****************************************************************************/
using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif

[ExecuteInEditMode]
public class SVGCameraBehaviour : MonoBehaviour
{
	private void Resize(int screenWidth, int screenHeight, bool shotEvent)
    {
		// map the camera rectangle to the whole screen; NB: we handle orthographic cameras only
		this.GetComponent<Camera>().aspect = (float)screenWidth / (float)screenHeight;
		this.GetComponent<Camera>().orthographicSize = (screenHeight * this.GetComponent<Camera>().rect.height) / (SVGAtlas.SPRITE_PIXELS_PER_UNIT * 2);
		// keep track of current device dimensions
		this.m_LastScreenWidth = screenWidth;
		this.m_LastScreenHeight = screenHeight;
       	// call OnResize handlers
		if (this.OnResize != null && shotEvent)
			this.OnResize(screenWidth, screenHeight);
    }

	public void Resize(bool shotEvent)
	{
		// update camera aspect ratio and orthographic size
		this.Resize((int)SVGAssets.ScreenResolutionWidth, (int)SVGAssets.ScreenResolutionHeight, shotEvent);
	}

	public float PixelWidth
	{
		// get the camera viewport width, in pixels
		get
		{
			return this.m_LastScreenWidth;
		}
	}
	
	public float PixelHeight
	{
		// get the camera viewport height, in pixels
		get
		{
			return this.m_LastScreenHeight;
		}
	}

	public float WorldWidth
	{
		// get the camera viewport width, in world coordinates
        get
        {
			return this.PixelWidth / SVGAtlas.SPRITE_PIXELS_PER_UNIT;
        }
    }
    
    public float WorldHeight
    {
        // get the camera viewport height, in world coordinates
        get
        {
			return this.PixelHeight / SVGAtlas.SPRITE_PIXELS_PER_UNIT;
        }
    }

    void Start()
    {
		// set the camera so that its viewing volume coincides with the whole device screen (or with the GameView, if inside Editor)
		this.Resize(false);
    }
    
    void Update()
    {
        // get the current screen size
		int curScreenWidth = (int)SVGAssets.ScreenResolutionWidth;
		int curScreenHeight = (int)SVGAssets.ScreenResolutionHeight;

        // if screen size has changed (e.g. device orientation changed), fire the event
		if (curScreenWidth != this.m_LastScreenWidth || curScreenHeight != this.m_LastScreenHeight)
			// update camera aspect ratio and orthographic size
			this.Resize(curScreenWidth, curScreenHeight, Application.isPlaying);
    }

#if UNITY_EDITOR
	// this script works with orthographic cameras only
	private bool RequirementsCheck()
	{
		if (this.GetComponent<Camera>() == null || (!this.GetComponent<Camera>().orthographic))
		{
			EditorUtility.DisplayDialog("Incompatible game object",
			                            string.Format("In order to work properly, the component {0} must be attached to an orthographic camera", this.GetType()),
			                            "Ok");
			DestroyImmediate(this);
			return false;
		}
		return true;
	}

    // Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time.
    // This function is only called in editor mode. Reset is most commonly used to give good default values in the inspector.
    void Reset()
    {
		this.RequirementsCheck();
    }
#endif

    public delegate void OnResizeEvent(int newScreenWidth, int newScreenHeight);
    public event OnResizeEvent OnResize;
    // device screen dimensions
	private int m_LastScreenWidth;
    private int m_LastScreenHeight;
}
