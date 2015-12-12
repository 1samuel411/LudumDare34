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
using UnityEditor;
using System;
using System.Collections;

public enum PivotEditingResult
{
	Ok,
	Cancel
}

public class SVGPivotEditor : EditorWindow
{
	static private void CreatePivotTexture()
	{
		Color32[] pixels = new Color32[SVGPivotEditor.PIVOT_CURSOR_DIMENSION * SVGPivotEditor.PIVOT_CURSOR_DIMENSION] {
			new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 67, 194, 42), new Color32(0, 67, 193, 111), new Color32(0, 65, 193, 172), new Color32(0, 66, 192, 166), 
			new Color32(0, 66, 194, 96), new Color32(0, 66, 189, 35), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), 
			new Color32(0, 78, 196, 39), new Color32(0, 74, 197, 144), new Color32(0, 73, 195, 196), new Color32(0, 73, 196, 204), new Color32(0, 74, 195, 201), new Color32(0, 74, 197, 201), new Color32(0, 73, 196, 204), new Color32(0, 73, 197, 192), new Color32(0, 75, 196, 129), new Color32(0, 78, 196, 26), 
			new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 86, 199, 59), new Color32(0, 83, 200, 182), new Color32(0, 82, 201, 198), new Color32(0, 83, 201, 197), new Color32(0, 83, 200, 190), 
			new Color32(0, 83, 201, 176), new Color32(0, 82, 201, 178), new Color32(0, 82, 201, 192), new Color32(0, 83, 201, 197), new Color32(0, 82, 200, 199), new Color32(0, 83, 201, 170), new Color32(0, 83, 204, 40), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), 
			new Color32(0, 92, 205, 36), new Color32(0, 92, 205, 175), new Color32(0, 92, 204, 189), new Color32(0, 93, 205, 184), new Color32(0, 92, 205, 117), new Color32(0, 87, 203, 44), new Color32(0, 96, 207, 16), new Color32(0, 94, 201, 19), new Color32(0, 87, 202, 53), new Color32(0, 91, 205, 132), 
			new Color32(0, 92, 203, 188), new Color32(0, 91, 204, 190), new Color32(0, 92, 204, 160), new Color32(0, 94, 201, 19), new Color32(0, 0, 0, 0), new Color32(0, 128, 255, 2), new Color32(0, 102, 211, 128), new Color32(0, 101, 208, 184), new Color32(0, 100, 208, 178), new Color32(0, 101, 208, 76), 
			new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 128, 255, 2), new Color32(0, 99, 207, 100), new Color32(0, 101, 209, 181), new Color32(0, 101, 208, 185), new Color32(0, 101, 208, 104), 
			new Color32(0, 0, 0, 0), new Color32(0, 116, 216, 33), new Color32(0, 110, 213, 169), new Color32(0, 110, 213, 176), new Color32(0, 109, 213, 108), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), 
			new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 170, 170, 3), new Color32(0, 109, 212, 131), new Color32(0, 110, 213, 176), new Color32(0, 110, 213, 157), new Color32(0, 112, 223, 16), new Color32(0, 120, 218, 89), new Color32(0, 118, 219, 168), new Color32(0, 119, 217, 163), 
			new Color32(0, 117, 214, 37), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 119, 217, 60), 
			new Color32(0, 118, 217, 168), new Color32(0, 119, 217, 167), new Color32(0, 119, 217, 47), new Color32(0, 128, 220, 132), new Color32(0, 128, 221, 159), new Color32(0, 129, 223, 144), new Color32(0, 118, 216, 13), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), 
			new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 128, 221, 30), new Color32(0, 129, 222, 154), new Color32(0, 128, 222, 162), new Color32(0, 128, 222, 70), new Color32(0, 138, 226, 122), 
			new Color32(0, 138, 226, 152), new Color32(0, 138, 227, 139), new Color32(0, 146, 219, 14), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), 
			new Color32(0, 0, 0, 0), new Color32(0, 132, 222, 31), new Color32(0, 138, 226, 148), new Color32(0, 138, 227, 155), new Color32(0, 139, 223, 64), new Color32(0, 146, 229, 68), new Color32(0, 146, 229, 148), new Color32(0, 147, 230, 144), new Color32(0, 150, 229, 39), new Color32(0, 0, 0, 0), 
			new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 147, 233, 59), new Color32(0, 146, 229, 148), new Color32(0, 146, 230, 145), 
			new Color32(0, 148, 228, 38), new Color32(0, 151, 232, 22), new Color32(0, 155, 234, 132), new Color32(0, 156, 233, 141), new Color32(0, 155, 237, 97), new Color32(0, 255, 255, 1), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), 
			new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 159, 223, 8), new Color32(0, 156, 235, 113), new Color32(0, 156, 233, 141), new Color32(0, 155, 234, 122), new Color32(0, 153, 230, 10), new Color32(0, 0, 0, 0), new Color32(0, 164, 237, 84), new Color32(0, 166, 238, 134), 
			new Color32(0, 164, 238, 132), new Color32(0, 166, 237, 72), new Color32(0, 170, 255, 3), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 170, 227, 9), new Color32(0, 168, 240, 85), new Color32(0, 165, 238, 133), 
			new Color32(0, 166, 240, 134), new Color32(0, 162, 236, 66), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 175, 239, 16), new Color32(0, 173, 243, 109), new Color32(0, 174, 243, 126), new Color32(0, 173, 243, 125), new Color32(0, 175, 244, 93), new Color32(0, 177, 244, 46), 
			new Color32(0, 181, 244, 24), new Color32(0, 173, 245, 25), new Color32(0, 180, 245, 51), new Color32(0, 173, 243, 102), new Color32(0, 174, 243, 126), new Color32(0, 175, 243, 127), new Color32(0, 175, 244, 96), new Color32(0, 182, 255, 7), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), 
			new Color32(0, 0, 0, 0), new Color32(0, 184, 245, 25), new Color32(0, 182, 245, 101), new Color32(0, 183, 247, 120), new Color32(0, 181, 244, 120), new Color32(0, 182, 246, 119), new Color32(0, 183, 246, 114), new Color32(0, 182, 246, 115), new Color32(0, 181, 244, 120), new Color32(0, 182, 246, 119), 
			new Color32(0, 183, 247, 120), new Color32(0, 182, 247, 91), new Color32(0, 182, 255, 14), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 191, 255, 12), new Color32(0, 188, 251, 65), 
			new Color32(0, 191, 252, 100), new Color32(0, 191, 250, 111), new Color32(0, 194, 253, 112), new Color32(0, 194, 253, 112), new Color32(0, 192, 250, 110), new Color32(0, 192, 252, 97), new Color32(0, 188, 246, 57), new Color32(0, 170, 255, 6), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), 
			new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 204, 255, 10), new Color32(0, 197, 255, 31), new Color32(0, 200, 255, 46), new Color32(0, 197, 255, 44), 
			new Color32(0, 200, 255, 28), new Color32(0, 191, 255, 8), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0), new Color32(0, 0, 0, 0)
		};

		m_PivotTexture = new Texture2D(SVGPivotEditor.PIVOT_CURSOR_DIMENSION, SVGPivotEditor.PIVOT_CURSOR_DIMENSION, TextureFormat.ARGB32, false);
		m_PivotTexture.name = "SVGPivotEditor.m_PivotTexture";
		m_PivotTexture.hideFlags = HideFlags.DontSave;
		m_PivotTexture.filterMode = FilterMode.Point;
		m_PivotTexture.SetPixels32(pixels);
		m_PivotTexture.Apply(false, true);
	}

	static private Texture2D PivotTexture
	{
		get
		{
			if (m_PivotTexture == null)
				SVGPivotEditor.CreatePivotTexture();
			return m_PivotTexture;
		}
	}

	void OnEnable()
	{
		m_Instance = this;
	}
	
	void OnDisable()
	{
		m_Instance = null;
	}

	private void Cancel()
	{
		if (this.m_Callback != null)
			this.m_Callback (PivotEditingResult.Cancel, this.m_SpriteAsset, new Vector2(this.m_Pivot.x, 1 - this.m_Pivot.y));
		Close();
	}
	
	private void Ok()
	{
		if (this.m_Callback != null)
			this.m_Callback (PivotEditingResult.Ok, this.m_SpriteAsset, new Vector2(this.m_Pivot.x, 1 - this.m_Pivot.y));
		Close();
	}

	private void Draw(Rect region)
	{
		float mouseX, mouseY, px, py;
		Rect spritePreviewRect = new Rect();
		GUILayoutOption[] okButtonOptions = new GUILayoutOption[2] { GUILayout.Width(this.m_WindowWidth / 2 - 8), GUILayout.Height(SVGPivotEditor.BUTTONS_HEIGHT - 3) };
		GUILayoutOption[] cancelButtonOptions = new GUILayoutOption[2] { GUILayout.Width(this.m_WindowWidth / 2 - 3), GUILayout.Height(SVGPivotEditor.BUTTONS_HEIGHT - 3) };
		
		GUILayout.BeginArea(region);
		// ----------------
		// custom code here
		// ----------------
		if (this.m_SpriteAsset != null)
		{
			Sprite sprite = this.m_SpriteAsset.SpriteData.Sprite;
			Texture2D texture = sprite.texture;
			Rect spriteRect = sprite.textureRect;
			Rect uvRect = new Rect(spriteRect.x / texture.width, spriteRect.y / texture.height, spriteRect.width / texture.width, spriteRect.height / texture.height);
			float spritePreviewWidth = this.m_SpritePreviewWidth;
			float spritePreviewHeight = this.m_SpritePreviewHeight;
			float spritePreviewX = 0;
			float spritePreviewY = 0;
			
			// draw the sprite preview
			spritePreviewRect = new Rect(spritePreviewX, spritePreviewY, spritePreviewWidth, spritePreviewHeight);
			GUI.DrawTextureWithTexCoords(spritePreviewRect, texture, uvRect, true);
			// draw pivot
			px = spritePreviewX + this.m_Pivot.x * spritePreviewWidth;
			py = spritePreviewY + this.m_Pivot.y * spritePreviewHeight;
			GUI.DrawTexture(new Rect(px - SVGPivotEditor.PIVOT_CURSOR_DIMENSION * 0.5f, py - SVGPivotEditor.PIVOT_CURSOR_DIMENSION * 0.5f, SVGPivotEditor.PIVOT_CURSOR_DIMENSION, SVGPivotEditor.PIVOT_CURSOR_DIMENSION), SVGPivotEditor.PivotTexture);
		}
		// button bar
		GUILayout.BeginArea(new Rect(3, region.height - SVGPivotEditor.BUTTONS_HEIGHT, region.width, SVGPivotEditor.BUTTONS_HEIGHT));
		GUILayout.BeginHorizontal(GUILayout.Height(SVGPivotEditor.BUTTONS_HEIGHT));
		if (GUILayout.Button("Cancel", cancelButtonOptions))
			this.Cancel();
		if (GUILayout.Button("Ok", okButtonOptions))
			this.Ok();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		GUILayout.EndArea();
		
		// events handler
		switch (Event.current.type)
		{
			// keyboard press
			case EventType.KeyDown:
				if (Event.current.keyCode == KeyCode.Return)
					this.Ok();
				if (Event.current.keyCode == KeyCode.Escape)
					this.Cancel();
				break;
			// mouse left button
			case EventType.MouseDown:
				mouseX = Event.current.mousePosition.x;
				mouseY = Event.current.mousePosition.y;
				// clamp x coordinate
				if (mouseX < spritePreviewRect.xMin + SVGPivotEditor.SNAP_CORNER_THRESHOLD)
					mouseX = spritePreviewRect.xMin;
				if (mouseX + SVGPivotEditor.SNAP_CORNER_THRESHOLD > spritePreviewRect.xMax)
					mouseX = spritePreviewRect.xMax;
					// clamp y coordinate
				if (mouseY < spritePreviewRect.yMin + SVGPivotEditor.SNAP_CORNER_THRESHOLD)
					mouseY = spritePreviewRect.yMin;
				if (mouseY + SVGPivotEditor.SNAP_CORNER_THRESHOLD > spritePreviewRect.yMax)
					mouseY = spritePreviewRect.yMax;
				// assign the new pivot value
				px = (mouseX - spritePreviewRect.xMin) / spritePreviewRect.width;
				py = (mouseY - spritePreviewRect.yMin) / spritePreviewRect.height;
				this.m_Pivot.Set(Mathf.Clamp(px, 0, 1), Mathf.Clamp(py, 0, 1));
				// force a repaint
				this.Repaint();
				break;
		}
	}

	void OnGUI()
	{
		Rect content = new Rect(0, 0, position.width, position.height);
		this.Draw(content);
	}

	private static void Init(SVGPivotEditor editor, SVGSpriteAssetFile spriteAsset, Vector2 pivot, OnPivotEditedCallback callback)
	{
		float v;
		Rect spriteRect = spriteAsset.SpriteData.Sprite.rect;
		float minDim = Math.Min(spriteRect.width, spriteRect.height);
		
		// keep track of the sprite and the input/output pivot
		editor.m_SpriteAsset = spriteAsset;
		editor.m_Pivot.Set(pivot.x, 1 - pivot.y);
		editor.m_SpritePreviewWidth = spriteRect.width;
		editor.m_SpritePreviewHeight = spriteRect.height;
		// adapt window dimension
		if (minDim < SVGPivotEditor.SPRITE_PREVIEW_MIN_DIMENSION)
		{
			float scl = SVGPivotEditor.SPRITE_PREVIEW_MIN_DIMENSION / minDim;
			editor.m_SpritePreviewWidth *= scl;
			editor.m_SpritePreviewHeight *= scl;
		}
		// we must not exceed screen resolution (width)
		v = Screen.currentResolution.width * 0.9f;
		if (editor.m_SpritePreviewWidth > v)
		{
			float scl = v / editor.m_SpritePreviewWidth;
			editor.m_SpritePreviewWidth *= scl;
			editor.m_SpritePreviewHeight *= scl;
		}
		v = Screen.currentResolution.height * 0.9f;
		if (editor.m_SpritePreviewHeight > v)
		{
			float scl = v / editor.m_SpritePreviewHeight;
			editor.m_SpritePreviewWidth *= scl;
			editor.m_SpritePreviewHeight *= scl;
		}
		
		editor.m_SpritePreviewWidth = Mathf.Round(editor.m_SpritePreviewWidth);
		editor.m_SpritePreviewHeight = Mathf.Round(editor.m_SpritePreviewHeight);
		editor.m_WindowWidth = editor.m_SpritePreviewWidth;
		editor.m_WindowHeight = editor.m_SpritePreviewHeight + SVGPivotEditor.BUTTONS_HEIGHT;
		// set title
		editor.title = "Pivot editor";
		// set callback
		editor.m_Callback = callback;
	}

	private void ShowEditor()
	{
		// position and size the window
		this.position = new Rect(Screen.currentResolution.width - (this.m_WindowWidth / 2), Screen.currentResolution.height - (this.m_WindowHeight / 2), this.m_WindowWidth, this.m_WindowHeight);
		this.minSize = new Vector2(this.m_WindowWidth, this.m_WindowHeight);
		this.maxSize = new Vector2(this.m_WindowWidth, this.m_WindowHeight);
		// show window
		this.ShowUtility();
	}

	// show the sprite selector
	static public void Show(SVGSpriteAssetFile spriteAsset, OnPivotEditedCallback callback)
	{
		// close the current selector instance, if any
		SVGPivotEditor.CloseAll();
		
		if (spriteAsset != null)
		{
			SVGPivotEditor pivotEditor = SVGPivotEditor.CreateInstance<SVGPivotEditor>();
			SVGPivotEditor.Init(pivotEditor, spriteAsset, spriteAsset.SpriteData.Pivot, callback);
			pivotEditor.ShowEditor();
		}
	}
	
	static public void CloseAll()
	{
		// close the current selector instance, if any
		if (m_Instance != null)
		{
			m_Instance.Close();
			m_Instance = null;
		}
	}

	// Selection callback
	public delegate void OnPivotEditedCallback(PivotEditingResult result, SVGSpriteAssetFile spriteAsset, Vector2 editedPivot);
	// The current selector instance
	[NonSerialized]
	static private SVGPivotEditor m_Instance;
	// The callback to be invoked editing is finished (i.e. Cancel or Ok button click)
	[NonSerialized]
	private OnPivotEditedCallback m_Callback;
	// the currently edited pivot value
	[NonSerialized]
	private Vector2 m_Pivot;
	[NonSerialized]
	static private Texture2D m_PivotTexture;
	// edited sprite asset
	[NonSerialized]
	private SVGSpriteAssetFile m_SpriteAsset;
	// dimensions of the sprite preview area
	[NonSerialized]
	private float m_SpritePreviewWidth;
	[NonSerialized]
	private float m_SpritePreviewHeight;
	// window (canvas) dimensions
	[NonSerialized]
	private float m_WindowWidth;
	[NonSerialized]
	private float m_WindowHeight;
	
	public const float SNAP_CORNER_THRESHOLD = 5;
	// pivot cursor
	public const int PIVOT_CURSOR_DIMENSION = 16;
	// height of the buttons bar
	public const int BUTTONS_HEIGHT = 25;
	// minimum dimension of the sprite preview area
	public const float SPRITE_PREVIEW_MIN_DIMENSION = 128;
}
