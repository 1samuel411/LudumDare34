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
using System.Collections;
using UnityEngine;

public class SVGTextureAtlasBehaviour : MonoBehaviour
{

    private Texture2D DrawAtlas(string svgXml1, string svgXml2, string svgXml3, string svgXml4,
                                uint texWidth, uint texHeight,
                                Color clearColor,
                                bool makeNoLongerReadable)
    {

        SVGDocument document1;
        SVGDocument document2;
        SVGDocument document3;
        SVGDocument document4;
        SVGSurface surface;
        
        // create a 2D texture (no mipmaps)
        Texture2D texture = new Texture2D((int)texWidth, (int)texHeight, TextureFormat.ARGB32, false);
        texture.hideFlags = HideFlags.HideAndDontSave;
        // create a drawing surface, with the same texture dimensions
        surface = SVGAssets.CreateSurface(texWidth, texHeight);
        if (surface == null)
            return null;
        // create the SVG document
        document1 = SVGAssets.CreateDocument(svgXml1);
        if (document1 == null)
        {
            surface.Dispose();
            return null;
        }
        document2 = SVGAssets.CreateDocument(svgXml2);
        if (document2 == null)
        {
            surface.Dispose();
            document1.Dispose();
            return null;
        }
        document3 = SVGAssets.CreateDocument(svgXml3);
        if (document3 == null)
        {
            surface.Dispose();
            document1.Dispose();
            document2.Dispose();
            return null;
        }
        document4 = SVGAssets.CreateDocument(svgXml4);
        if (document4 == null)
        {
            surface.Dispose();
            document1.Dispose();
            document2.Dispose();
            document3.Dispose();
            return null;
        }

        // draw the SVG document1 onto the surface
        surface.Viewport = new SVGViewport(0.0f, 0.0f, texWidth / 2.0f, texHeight / 2.0f);
        surface.Draw(document1, new SVGColor(clearColor.r, clearColor.g, clearColor.b, clearColor.a), SVGRenderingQuality.Better);
        // draw the SVG document2 onto the surface
        surface.Viewport = new SVGViewport(texWidth / 2.0f, 0.0f, texWidth / 2.0f, texHeight / 2.0f);
        surface.Draw(document2, null, SVGRenderingQuality.Better);
        // draw the SVG document3 onto the surface
        surface.Viewport = new SVGViewport(0.0f, texHeight / 2.0f, texWidth / 2.0f, texHeight / 2.0f);
        surface.Draw(document3, null, SVGRenderingQuality.Better);
        // draw the SVG document4 onto the surface
        surface.Viewport = new SVGViewport(texWidth / 2.0f, texHeight / 2.0f, texWidth / 2.0f, texHeight / 2.0f);
        surface.Draw(document4, null, SVGRenderingQuality.Better);

        // copy the surface content inside the texture
        surface.Copy(texture, true);
        // call Apply() so it's actually uploaded to the GPU
        texture.Apply(true, makeNoLongerReadable);
        // destroy SVG surface and document
        surface.Dispose();
        document1.Dispose();
        document2.Dispose();
        document3.Dispose();
        document4.Dispose();
        // return the created texture
        return texture;
    }

    // Use this for initialization
    void Start()
    {

        if (SVGFile1 != null && SVGFile2 != null && SVGFile3 != null && SVGFile4 != null)
        {
            GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
            GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            // set texture onto our material
            GetComponent<Renderer>().material.mainTexture = DrawAtlas(SVGFile1.text, SVGFile2.text, SVGFile3.text, SVGFile4.text,
                                                      (uint)Math.Max(2, TextureWidth) , (uint)Math.Max(2, TextureHeight),
                                                      ClearColor,
                                                      true);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public TextAsset SVGFile1 = null;
    public TextAsset SVGFile2 = null;
    public TextAsset SVGFile3 = null;
    public TextAsset SVGFile4 = null;
    public int TextureWidth = 512;
    public int TextureHeight = 512;
    public Color ClearColor = new Color(1, 1, 1, 1);
}
