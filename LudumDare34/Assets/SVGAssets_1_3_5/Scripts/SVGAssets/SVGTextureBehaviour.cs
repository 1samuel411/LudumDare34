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

public class SVGTextureBehaviour : MonoBehaviour
{
    private Texture2D DrawSVG(string svgXml,
                              uint texWidth, uint texHeight,
                              Color clearColor,
                              bool makeNoLongerReadable)
    {
        SVGDocument document;
        SVGSurface surface;
        
        // create a 2D texture (no mipmaps)
        Texture2D texture = new Texture2D((int)texWidth, (int)texHeight, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Bilinear;
        // create a drawing surface, with the same texture dimensions
        surface = SVGAssets.CreateSurface(texWidth, texHeight);
        if (surface == null)
            return null;
        // create the SVG document
        document = SVGAssets.CreateDocument(svgXml);
        if (document == null)
        {
            surface.Dispose();
            return null;
        }
        // draw the SVG document onto the surface
        surface.Draw(document, new SVGColor(clearColor.r, clearColor.g, clearColor.b, clearColor.a), SVGRenderingQuality.Better);
        // copy the surface content inside the texture; NB: we set the delateEdgesFix flag because we are using bilinear texture filtering
        surface.Copy(texture, true);
        // call Apply() so it's actually uploaded to the GPU
        texture.Apply(true, makeNoLongerReadable);
        // destroy SVG surface and document
        surface.Dispose();
        document.Dispose();
        // return the created texture
        return texture;
    }

    // Use this for initialization
    void Start()
    {
        if (SVGFile != null)
        {
            GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
            GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1);
            // set texture onto our material
            GetComponent<Renderer>().material.mainTexture = DrawSVG(SVGFile.text,
                                                    // we want at least a 1x1 texture
                                                    (uint)Math.Max(1, TextureWidth) , (uint)Math.Max(1, TextureHeight),
                                                    this.ClearColor,
                                                    true);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public TextAsset SVGFile = null;
    public int TextureWidth = 512;
    public int TextureHeight = 512;
    public Color ClearColor = new Color(1, 1, 1, 1);
}
