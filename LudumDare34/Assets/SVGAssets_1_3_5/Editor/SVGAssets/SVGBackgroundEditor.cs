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
using UnityEngine;
using UnityEditor;

[ CustomEditor(typeof(SVGBackgroundBehaviour)) ]
public class SVGBackgroundEditor : Editor
{
	private void DrawInspector(SVGBackgroundBehaviour svgBackground)
	{
		SVGBackgroundScaleType scaleAdaption = SVGBackgroundScaleType.Vertical;
		int size = 256;
		int slicedWidth = 256;
		int slicedHeight = 256;
		bool needUpdate = false;
		bool fullUpdate = false;
		TextAsset svgFile = EditorGUILayout.ObjectField("SVG file", svgBackground.SVGFile, typeof(TextAsset), true) as TextAsset;
		bool sliced = EditorGUILayout.Toggle(new GUIContent("Sliced", "Check if you want to slice the background in order to fit specified width/height"), svgBackground.Sliced);
		if (sliced)
		{
			slicedWidth = EditorGUILayout.IntField(new GUIContent("Width", "Sliced width, in pixels"), svgBackground.SlicedWidth);
			slicedHeight = EditorGUILayout.IntField(new GUIContent("Height", "Sliced height, in pixels"), svgBackground.SlicedHeight);
		}
		else
		{
			scaleAdaption = (SVGBackgroundScaleType)EditorGUILayout.EnumPopup("Scale adaption", svgBackground.ScaleAdaption);
			size = EditorGUILayout.IntField(new GUIContent("Size", "Size in pixels"), svgBackground.Size);
		}
		Color clearColor = EditorGUILayout.ColorField("Clear color", svgBackground.ClearColor);
		bool generateOnStart = EditorGUILayout.Toggle(new GUIContent("Generate on Start()", "Generate the background texture/sprite on Start()"), svgBackground.GenerateOnStart);
		// show dimensions in pixel and world units
		EditorGUILayout.LabelField("Width (pixel unit)", svgBackground.PixelWidth.ToString());
		EditorGUILayout.LabelField("Height (pixel units)", svgBackground.PixelHeight.ToString());
		EditorGUILayout.LabelField("Width (world units)", svgBackground.WorldWidth.ToString());
		EditorGUILayout.LabelField("Height (world units)", svgBackground.WorldHeight.ToString());

		// update svg file, if needed
		if (svgFile != svgBackground.SVGFile)
		{
			svgBackground.SVGFile = svgFile;
			EditorUtility.SetDirty(svgBackground);
			needUpdate = true;
			// in this case we must destroy the current document and load the new one
			fullUpdate = true;
		}

		if (sliced != svgBackground.Sliced)
		{
			svgBackground.Sliced = sliced;
			EditorUtility.SetDirty(svgBackground);
			needUpdate = true;
		}

		if (sliced)
		{
			// update sliced width (in pixels), if needed
			if (slicedWidth != svgBackground.SlicedWidth)
			{
				svgBackground.SlicedWidth = slicedWidth;
				EditorUtility.SetDirty(svgBackground);
				needUpdate = true;
			}
			// update sliced height (in pixels), if needed
			if (slicedHeight != svgBackground.SlicedHeight)
			{
				svgBackground.SlicedHeight = slicedHeight;
				EditorUtility.SetDirty(svgBackground);
				needUpdate = true;
			}
		}
		else
		{
			// update scale adaption, if needed
			if (scaleAdaption != svgBackground.ScaleAdaption)
			{
				svgBackground.ScaleAdaption = scaleAdaption;
				EditorUtility.SetDirty(svgBackground);
				needUpdate = true;
			}
			// update size (in pixels), if needed
			if (size != svgBackground.Size)
			{
				svgBackground.Size = size;
				EditorUtility.SetDirty(svgBackground);
				needUpdate = true;
			}
		}
		// update clear color, if needed
		if (clearColor != svgBackground.ClearColor)
		{
			svgBackground.ClearColor = clearColor;
			EditorUtility.SetDirty(svgBackground);
			needUpdate = true;
		}
		// update "update on start" flag, if needed
		if (generateOnStart != svgBackground.GenerateOnStart)
		{
			svgBackground.GenerateOnStart = generateOnStart;
			EditorUtility.SetDirty(svgBackground);
		}
		// update the background, if needed
		if (needUpdate)
			svgBackground.UpdateBackground(fullUpdate);
	}

	public override void OnInspectorGUI()
	{
		// get the target object
		SVGBackgroundBehaviour svgBackground = target as SVGBackgroundBehaviour;
		
		if (svgBackground != null)
			this.DrawInspector(svgBackground);
	}
}
