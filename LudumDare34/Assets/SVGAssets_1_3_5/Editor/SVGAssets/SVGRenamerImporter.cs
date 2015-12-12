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
using System.IO;
using UnityEditor;

public class SVGRenamerImporter : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets,
                                       string[] deletedAssets,
                                       string[] movedAssets,
                                       string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            if (Path.GetExtension(assetPath) == ".svg")
            {
                // try to change ".svg" file extension with ".svg.txt", so Unity can recognize those files as text assets
                string newAssetPath = Path.ChangeExtension(assetPath, ".svg.txt");
                if (File.Exists(newAssetPath))
                {
                    bool ok = EditorUtility.DisplayDialog("Asset already exists!",
                                                          string.Format("{0} asset file already exists, would you like to import it anyway and overwrite the previous one?", newAssetPath),
                                                          "Import and overwrite", "Do NOT import");

                    if (!ok)
                    {
                        AssetDatabase.DeleteAsset(assetPath);
                        AssetDatabase.Refresh();
                        return;
                    }
                    // remove previous asset
                    AssetDatabase.DeleteAsset(newAssetPath);
                }
                FileUtil.MoveFileOrDirectory(assetPath, newAssetPath);
                AssetDatabase.Refresh();
            }
        }
    }
}
