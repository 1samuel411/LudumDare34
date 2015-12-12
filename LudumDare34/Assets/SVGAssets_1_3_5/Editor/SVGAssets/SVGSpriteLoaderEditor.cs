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
using System;
using System.Collections.Generic;
using UnityEngine; 
using UnityEditor;

[ CustomEditor(typeof(SVGSpriteLoaderBehaviour)) ]
public class SVGSpriteLoaderEditor : Editor {

	private void OnSpriteSelect(SVGSpriteAssetFile spriteAsset)
	{
		//if (this.m_EditedLoader.SpriteReference != spriteAsset.SpriteRef)
		if (this.m_EditedLoader.SpriteReference.TxtAsset != spriteAsset.SpriteRef.TxtAsset ||
		    this.m_EditedLoader.SpriteReference.ElemIdx != spriteAsset.SpriteRef.ElemIdx)
		{
			// set the selected sprite (reference)
			this.m_EditedLoader.SpriteReference.TxtAsset = spriteAsset.SpriteRef.TxtAsset;
			this.m_EditedLoader.SpriteReference.ElemIdx = spriteAsset.SpriteRef.ElemIdx;
			// set the selected sprite into the renderer
			SpriteRenderer renderer = this.m_EditedLoader.GetComponent<SpriteRenderer>();
			if (renderer != null)
				renderer.sprite = spriteAsset.SpriteData.Sprite;
		}
	}

	private void OnAtlasSelect(SVGAtlas atlas)
	{
		if (this.m_EditedLoader.Atlas != atlas)
		{
			this.m_EditedLoader.Atlas = atlas;
			this.m_EditedLoader.SpriteReference = null;

			SpriteRenderer renderer = this.m_EditedLoader.GetComponent<SpriteRenderer>();
			if (renderer != null)
				renderer.sprite = null;
		}
	}

	private void DrawInspector()
	{
		bool resizeOnStart = EditorGUILayout.Toggle("Resize on Start", this.m_EditedLoader.ResizeOnStart);
		bool updateTransform = EditorGUILayout.Toggle("Update transform", this.m_EditedLoader.UpdateTransform);

		string atlasName = (this.m_EditedLoader.Atlas != null) ? this.m_EditedLoader.Atlas.name : "<select>";
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.PrefixLabel("Atlas");
			if (GUILayout.Button(atlasName, "DropDown"))
				SVGAtlasSelector.Show("", this.OnAtlasSelect);
		}
		EditorGUILayout.EndHorizontal();


		if (this.m_EditedLoader.Atlas != null && this.m_EditedLoader.SpriteReference != null)
		{
			SVGSpriteAssetFile spriteAsset = this.m_EditedLoader.Atlas.GetGeneratedSprite(this.m_EditedLoader.SpriteReference);
			string buttonText = (spriteAsset != null) ? spriteAsset.SpriteData.Sprite.name : "<select>";

			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel("Sprite");
				if (GUILayout.Button(buttonText, "DropDown"))
					SVGSpriteSelector.Show(this.m_EditedLoader.Atlas, "", this.OnSpriteSelect);
			}
			EditorGUILayout.EndHorizontal();
		}

		if (this.m_EditedLoader.ResizeOnStart != resizeOnStart)
		{
			this.m_EditedLoader.ResizeOnStart = resizeOnStart;
			EditorUtility.SetDirty(this.m_EditedLoader);
		}

		if (this.m_EditedLoader.UpdateTransform != updateTransform)
		{
			this.m_EditedLoader.UpdateTransform = updateTransform;
			EditorUtility.SetDirty(this.m_EditedLoader);
		}
	}

    public override void OnInspectorGUI()
    {
        // get the target object
		this.m_EditedLoader = target as SVGSpriteLoaderBehaviour;

		if (this.m_EditedLoader != null)
		{
        	GUI.enabled = (Application.isPlaying) ? false : true;
			this.DrawInspector();
		}
    }

    void OnDestroy()
    {
		/*
        // avoid to leak textures
        this.DestroyCustomStyles();
        this.m_CustomStylesGenerated = false;
        */

		//this.m_EditedLoader = null;
    }

	[NonSerialized]
	private SVGSpriteLoaderBehaviour m_EditedLoader = null;
}
