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

[ CustomEditor(typeof(SVGCameraBehaviour)) ]
public class SVGCameraEditor : Editor
{
	private void DrawInspector(SVGCameraBehaviour svgCamera)
	{
		// show dimensions in pixel and world units
		EditorGUILayout.LabelField("Width (pixel units)", svgCamera.PixelWidth.ToString());
		EditorGUILayout.LabelField("Height (pixel units)", svgCamera.PixelHeight.ToString());
		EditorGUILayout.LabelField("Width (world units)", svgCamera.WorldWidth.ToString());
		EditorGUILayout.LabelField("Height (world units)", svgCamera.WorldHeight.ToString());
	}

	public override void OnInspectorGUI()
	{
		// get the target object
		SVGCameraBehaviour camera = target as SVGCameraBehaviour;
		
		if (camera != null)
			this.DrawInspector(camera);
	}
}
