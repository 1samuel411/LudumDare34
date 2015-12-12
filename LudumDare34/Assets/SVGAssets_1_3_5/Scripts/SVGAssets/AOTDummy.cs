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

#if UNITY_IPHONE
// avoid "ExecutionEngineException: Attempting to JIT compile method '...' while running with --aot-only
public static class AOTDummy
{
    public static void Dummy()
    {
        System.Collections.Generic.Dictionary<SVGSpriteRef, int> dummy0;
    }
}
#endif
