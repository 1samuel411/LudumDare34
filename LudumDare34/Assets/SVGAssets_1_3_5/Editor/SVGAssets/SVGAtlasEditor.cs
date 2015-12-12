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
using UnityEditor.Callbacks;
using System;
using System.Collections.Generic;
using System.IO;

public static class ScriptableObjectUtility
{
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (path == "") 
            path = "Assets";
        else
        if (Path.GetExtension(path) != "") 
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}

// Menu item used to create a new atlas generator
public class SVGAtlasAsset
{
    [MenuItem("Assets/SVGAssets/Create SVG atlas", false, 0)]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<SVGAtlas>();
    }
}

// Used to keep track of dragged object inside the list of input SVG rows
public class DragInfo
{
    public DragInfo()
    {
        this.m_Dragging = false;
        this.m_Object = null;
        this.m_InsertIdx = -1;
        this.m_InsertBefore = false;
    }

    // Start a drag operation.
    public void StartDrag(System.Object obj)
    {
        this.m_Dragging = true;
        this.m_Object = obj;
        this.m_InsertIdx = -1;
        this.m_InsertBefore = false;
    }

    // Stop a drag operation.
    public void StopDrag()
    {
        this.m_Dragging = false;
        this.m_Object = null;
        this.m_InsertIdx = -1;
        this.m_InsertBefore = false;
    }

    public bool Dragging
    {
        get
        {
            return this.m_Dragging;
        }
    }

    public int InsertIdx
    {
        get
        {
            return this.m_InsertIdx;
        }

        set
        {
            this.m_InsertIdx = value;
        }
    }

    public bool InsertBefore
    {
        get
        {
            return this.m_InsertBefore;
        }

        set
        {
            this.m_InsertBefore = value;
        }
    }

    public System.Object DraggedObject
    {
        get
        {
            return this.m_Object;
        }

        set
        {
            this.m_Object = value;
        }
    }

    // True if the user is dragging a text asset or an already present SVG row, else false.
    private bool m_Dragging;
    // The dragged object.
    private System.Object m_Object;
    // Target insertion position.
    private int m_InsertIdx;
    // True/False if the dragged object must be inserted before/after the selected position.
    private bool m_InsertBefore;
}

public static class SVGBuildProcessor
{
    private static void ProcessAtlas(SVGAtlas atlas)
    {
        Texture2D tmpTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);

        foreach (KeyValuePair<SVGSpriteRef, SVGSpriteAssetFile> file in atlas.GeneratedSpritesFiles)
        {
            SVGSpriteAssetFile spriteAsset = file.Value;
            SVGSpriteData spriteData = spriteAsset.SpriteData;
            Sprite original = spriteData.Sprite;

            // we must reference the original texture, because we want to keep the file reference (rd->texture.IsValid())
            Sprite tmpSprite = Sprite.Create(original.texture, new Rect(0, 0, 1, 1), new Vector2(0, 0), SVGAtlas.SPRITE_PIXELS_PER_UNIT);
            // now we change the (sprite) asset content: actually we have just reduced its rectangle to a 1x1 pixel
            EditorUtility.CopySerialized(tmpSprite, original);
        }
        
        for (int i = 0; i < atlas.GeneratedTexturesFiles.Count; i++)
        {
            AssetFile file = atlas.GeneratedTexturesFiles[i];
            Texture2D original = file.Object as Texture2D;

            // copy the 1x1 texture inside the original texture
            EditorUtility.CopySerialized(tmpTexture, original);
        }
    }

    private static void ProcessScene()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        List<SVGAtlas> unexportedAtlases = new List<SVGAtlas>();

        // scan all game objects in the current scene, and keep track of used atlas generators
        foreach (GameObject gameObj in allObjects)
        {
            if (gameObj.activeInHierarchy)
            {
                SVGSpriteLoaderBehaviour loader = gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
                if (loader != null)
                {
                    // if this atlas has not been already flagged, lets keep track of it
                    if (!loader.Atlas.Exporting)
                    {
                        unexportedAtlases.Add(loader.Atlas);
                        loader.Atlas.Exporting = true;
                    }
                }
            }
        }

        foreach (SVGAtlas atlas in unexportedAtlases)
        {
            SVGBuildProcessor.ProcessAtlas(atlas);
            // keep track of this atlas in the global list
            SVGBuildProcessor.m_Atlases.Add(atlas);
        }
    }

    [PostProcessScene]
    public static void OnPostprocessScene()
    {
        if (!Application.isPlaying)
            SVGBuildProcessor.ProcessScene();
    }

    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (!Application.isPlaying)
        {
            // unflag processed atlases
            foreach (SVGAtlas atlas in SVGBuildProcessor.m_Atlases)
            {
                // update sprites using the last used scale factor
                atlas.UpdateSprites(false);
                atlas.Exporting = false;
            }
            // clear the list
            SVGBuildProcessor.m_Atlases.Clear();
        }
    }

    [NonSerialized]
    private static List<SVGAtlas> m_Atlases = new List<SVGAtlas>();
}

[ CustomEditor(typeof(SVGAtlas)) ]
public class SVGAtlasEditor : Editor
{
	// pivot editing callback
	private void OnPivotEdited(PivotEditingResult result, SVGSpriteAssetFile spriteAsset, Vector2 editedPivot)
	{
		SVGAtlas atlas = target as SVGAtlas;
		if (atlas != null && result == PivotEditingResult.Ok)
		{
			// assign the new pivot
			atlas.UpdatePivot(spriteAsset, editedPivot);
			EditorUtility.SetDirty(atlas);
		}
	}

    // generate a 1x1 texture used as background for custom styles
    private Texture2D BackgroundTextureGen(Color32 color)
    {
        Color32[] pixels = new Color32[1] { color };
        Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        // we take care to destroy the texture, when it will be the moment
        texture.hideFlags = HideFlags.DontSave;
        texture.SetPixels32(pixels);
        texture.Apply(false, true);
        return texture;
    }

    // generate custom tyles
    private void GenerateCustomStyles()
    {
        // blue line separator
        this.m_BlueLine = new GUIStyle();
        this.m_BlueLineTexture = BackgroundTextureGen(new Color32(51, 81, 226, 255));
        this.m_BlueLine.normal.background = m_BlueLineTexture;
        // grey line separator
        this.m_GreyLine = new GUIStyle();
        this.m_GreyLineTexture = BackgroundTextureGen(new Color32(128, 128, 128, 255));
        this.m_GreyLine.normal.background = m_GreyLineTexture;
        this.m_GreyLine.padding.bottom = m_GreyLine.padding.top = 0;
        this.m_GreyLine.border.top = m_GreyLine.border.bottom = 0;
        // blue highlighted background
        this.m_HighlightRow = new GUIStyle();
        this.m_HighlightRowTexture = BackgroundTextureGen(new Color32(65, 92, 150, 255));
        this.m_HighlightRow.normal.background = m_HighlightRowTexture;
        this.m_HighlightRow.normal.textColor = Color.white;
    }

    // destroy custom tyles
    private void DestroyCustomStyles()
    {
        if (this.m_BlueLineTexture != null)
        {
            Texture2D.DestroyImmediate(this.m_BlueLineTexture);
            this.m_BlueLineTexture = null;
        }
        if (this.m_GreyLineTexture != null)
        {
            Texture2D.DestroyImmediate(this.m_GreyLineTexture);
            this.m_GreyLineTexture = null;
        }
        if (this.m_HighlightRowTexture != null)
        {
            Texture2D.DestroyImmediate(this.m_HighlightRowTexture);
            this.m_HighlightRowTexture = null;
        }
    }

    private void SpritePreview(SVGAtlas atlas, SVGSpriteAssetFile spriteAsset)
    {
        Sprite sprite = spriteAsset.SpriteData.Sprite;
        Texture2D texture = sprite.texture;
        Rect spriteRect = sprite.textureRect;
        Rect uv = new Rect(spriteRect.x / texture.width, spriteRect.y / texture.height, spriteRect.width / texture.width, spriteRect.height / texture.height);
        GUILayoutOption[] spriteTextureOptions = new GUILayoutOption[2] { GUILayout.Width(SVGAtlasEditor.SPRITE_PREVIEW_DIMENSION), GUILayout.Height(SVGAtlasEditor.SPRITE_PREVIEW_DIMENSION) };
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(sprite.name, GUILayout.MinWidth(10));
        // reserve space for drawing sprite
        EditorGUILayout.LabelField("", spriteTextureOptions);
        Rect guiRect = GUILayoutUtility.GetLastRect();
        float maxSpriteDim = Math.Max(spriteRect.width, spriteRect.height);
        float previewWidth = (spriteRect.width / maxSpriteDim) * SVGAtlasEditor.SPRITE_PREVIEW_DIMENSION;
        float previewHeight = (spriteRect.height / maxSpriteDim) * SVGAtlasEditor.SPRITE_PREVIEW_DIMENSION;
        float previewX = (SVGAtlasEditor.SPRITE_PREVIEW_DIMENSION - previewWidth) / 2;
        //float previewY = (SVGAtlasEditor.SPRITE_PREVIEW_DIMENSION - previewHeight) / 2;
        //float previewY = (previewWidth > previewHeight) ? 0 : ((SVGAtlasEditor.SPRITE_PREVIEW_DIMENSION - previewHeight) / 2);
        float previewY = 0;
        Rect previewRect = new Rect(guiRect.xMin + previewX, guiRect.yMin + previewY, previewWidth, previewHeight);
        GUI.DrawTextureWithTexCoords(previewRect, texture, uv, true);
        EditorGUILayout.Space();
        // current pivot
        EditorGUILayout.LabelField("[" + string.Format("{0:0.00}", spriteAsset.SpriteData.Pivot.x) + " , " + string.Format("{0:0.00}", spriteAsset.SpriteData.Pivot.y) + "]", GUILayout.Width(76));
        // edit pivot
        if (GUILayout.Button("Edit pivot", GUILayout.Width(70)))
			// show pivot editor
			SVGPivotEditor.Show(spriteAsset, this.OnPivotEdited);
        // instantiate
        if (GUILayout.Button("Instantiate", GUILayout.Width(80)))
        {
            GameObject gameObj = atlas.InstantiateSprite(spriteAsset.SpriteRef);
            // set the created instance as selected
            if (gameObj != null)
                Selection.objects = new UnityEngine.Object[1] { gameObj as UnityEngine.Object };
        }
        EditorGUILayout.EndHorizontal();
    }

    private Rect DrawInputAsset(SVGAtlas atlas, int index)
    {
        Rect rowRect;
        SVGAssetInput svgAsset = atlas.SvgList[index];
        bool highlight = (this.m_DragInfo.Dragging && this.m_DragInfo.DraggedObject == svgAsset) ? true : false;

        if (this.m_DragInfo.InsertIdx == index && this.m_DragInfo.InsertBefore)
            // draw a separator before the row
            GUILayout.Box(GUIContent.none, this.m_BlueLine, GUILayout.ExpandWidth(true), GUILayout.Height(2));

        // if the SVG row is the dragged one, change colors
        if (highlight)
        {
            EditorGUILayout.BeginHorizontal(this.m_HighlightRow);
            // a row: asset name, separate groups checkbox, remove button, instantiate button
            EditorGUILayout.LabelField(svgAsset.TxtAsset.name, this.m_HighlightRow, GUILayout.MinWidth(10));
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            // a row: asset name, separate groups checkbox, remove button, instantiate button
            EditorGUILayout.LabelField(svgAsset.TxtAsset.name, GUILayout.MinWidth(10));
        }

        // 'explode groups' flag
        bool separateGroups = EditorGUILayout.Toggle("", svgAsset.SeparateGroups, GUILayout.Width(14));
        EditorGUILayout.LabelField("Separate groups", GUILayout.Width(105));
        // if group explosion flag has been changed, update it
        if (separateGroups != svgAsset.SeparateGroups) {
            atlas.InputAssetSeparateGroupsSet(svgAsset, separateGroups);
            EditorUtility.SetDirty(atlas);
        }
        // if 'Remove' button is clicked, remove the SVG entry
        if (GUILayout.Button("Remove", GUILayout.Width(70))) {
            //svgList.Remove(packedSvg);
            atlas.InputAssetRemove(index);
            EditorUtility.SetDirty(atlas);
        }
        // instantiate all groups
        GUI.enabled = ((svgAsset.Instantiable && (!Application.isPlaying)) ? true : false);
        if (GUILayout.Button("Instantiate", GUILayout.Width(80)))
        {
            GameObject[] gameObjs = atlas.InstantiateGroups(svgAsset);
            // set the created instances as selected
            if (gameObjs != null)
                Selection.objects = gameObjs;
        }
        GUI.enabled = !Application.isPlaying;
        EditorGUILayout.EndHorizontal();
        rowRect = GUILayoutUtility.GetLastRect();

        if (this.m_DragInfo.InsertIdx == index && (!this.m_DragInfo.InsertBefore))
            // draw a separator after the row
            GUILayout.Box(GUIContent.none, this.m_BlueLine, GUILayout.ExpandWidth(true), GUILayout.Height(2));

        return rowRect;
    }

    private void DrawInspector(SVGAtlas atlas)
    {
        Rect scollRect;
        int i;
        Vector2 scrollPos;
        // get current event
        Event currentEvent = Event.current;
        // show current options
        int refWidth = EditorGUILayout.IntField("Reference width", atlas.ReferenceWidth);
        int refHeight = EditorGUILayout.IntField("Reference height", atlas.ReferenceHeight);
        int deviceTestWidth = EditorGUILayout.IntField("Device test width", atlas.DeviceTestWidth);
        int deviceTestHeight = EditorGUILayout.IntField("Device test height", atlas.DeviceTestHeight);
        SVGScaleType scaleType = (SVGScaleType)EditorGUILayout.EnumPopup("Scale adaption", atlas.ScaleType);
        float offsetScale = EditorGUILayout.FloatField("Offset scale", atlas.OffsetScale);
        bool pow2Textures = EditorGUILayout.Toggle("Force pow2 textures", atlas.Pow2Textures);
        int maxTexturesDimension = EditorGUILayout.IntField("Max textures dimension", atlas.MaxTexturesDimension);
        int border = EditorGUILayout.IntField("Sprites border", atlas.SpritesBorder);
        Color clearColor = EditorGUILayout.ColorField("Clear color", atlas.ClearColor);
		// output folder
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel("Output folder");
		string outputFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(atlas));
		EditorGUILayout.LabelField(outputFolder + "/Atlases");
		EditorGUILayout.EndHorizontal();

        // current list of SVG assets
        List<SVGAssetInput> svgList = atlas.SvgList;
        
        GUILayout.Space(18);
        EditorGUILayout.LabelField("Drag & drop SVG assets here");

        // keep track of drawn rows
        if (currentEvent.type != EventType.Layout)
            this.m_InputAssetsRects = new List<Rect>();

        //scrollPos = EditorGUILayout.BeginScrollView(this.m_SvgListScrollPos, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(102), GUILayout.MaxHeight(102));
        scrollPos = EditorGUILayout.BeginScrollView(this.m_SvgListScrollPos, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(102), GUILayout.Height(102));
        // perform backward loop because we could even remove entries without issues
        for (i = 0; i < svgList.Count; ++i)
        {
            Rect rowRect = this.DrawInputAsset(atlas, i);
            // keep track of row rectangle
            if (currentEvent.type != EventType.Layout)
                this.m_InputAssetsRects.Add(rowRect);
        }
        EditorGUILayout.EndScrollView();
        // keep track of the scrollview area
        scollRect = GUILayoutUtility.GetLastRect();

        // update button
        string updateStr = (atlas.NeedsUpdate()) ? "Update *" : "Update";
        if (GUILayout.Button(updateStr))
        {
			// close all modal popup editors
			SVGPivotEditor.CloseAll();
			SVGSpriteSelector.CloseAll();
			SVGAtlasSelector.CloseAll();
			// regenerate/update sprites
			atlas.UpdateSprites(true);
            EditorUtility.SetDirty(atlas);
        }
        GUILayout.Space(10);

        // list of sprites, grouped by SVG document
        Vector2 spritesScrollPos = EditorGUILayout.BeginScrollView(this.m_SvgSpritesScrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        bool separatorNeeded = false;
        for (i = 0; i < svgList.Count; ++i)
        {
            SVGAssetInput svgAsset = svgList[i];
            List<SVGSpriteAssetFile> spritesAssets = atlas.GetGeneratedSpritesByDocument(svgAsset.TxtAsset);
            if (spritesAssets != null && spritesAssets.Count > 0)
            {
                // line separator
                if (separatorNeeded)
                {
                    EditorGUILayout.Separator();
                    GUILayout.Box(GUIContent.none, this.m_GreyLine, GUILayout.ExpandWidth(true), GUILayout.Height(1));
                    EditorGUILayout.Separator();
                }
                // display sprites list
                foreach (SVGSpriteAssetFile spriteAsset in spritesAssets)
                    this.SpritePreview(atlas, spriteAsset);
                // we have displayed some sprites, next time a line separator is needed
                separatorNeeded = true;
            }
        }
        EditorGUILayout.EndScrollView();
        
        // events handler
        if (currentEvent.type != EventType.Layout)
        {
            bool needRepaint = false;
            // get mouse position relative to scollRect
            Vector2 mousePos = currentEvent.mousePosition - new Vector2(scollRect.xMin, scollRect.yMin);

            if (scollRect.Contains(currentEvent.mousePosition))
            {
                bool separatorInserted = false;

                for (i = 0; i < svgList.Count; ++i)
                {
                    // get the row rectangle relative to atlas.SvgList[i]
                    Rect rowRect = this.m_InputAssetsRects[i];
                    // expand the rectangle height
                    rowRect.yMin -= 3;
                    rowRect.yMax += 3;

                    if (rowRect.Contains(mousePos))
                    {
                        // a mousedown on a row, will stop an already started drag operation
                        if (currentEvent.type == EventType.MouseDown)
                            this.m_DragInfo.StopDrag();

                        // check if we are already dragging an object
                        if (this.m_DragInfo.Dragging)
                        {
                            if (!separatorInserted)
                            {
                                bool ok = true;
                                bool dragBefore = (mousePos.y <= rowRect.yMin + rowRect.height / 2) ? true : false;
                                // if we are dragging a text (asset) file, all positions are ok
                                // if we are dragging an already present SVG row, we must perform additional checks
                                if (!(this.m_DragInfo.DraggedObject is TextAsset))
                                {
                                    if (this.m_DragInfo.DraggedObject == atlas.SvgList[i])
                                        ok = false;
                                    else
                                    {
                                        if (dragBefore)
                                        {
                                            if (i > 0 && this.m_DragInfo.DraggedObject == atlas.SvgList[i - 1])
                                                ok = false;
                                        }
                                        else
                                        {
                                            if (i < (svgList.Count - 1) && this.m_DragInfo.DraggedObject == atlas.SvgList[i + 1])
                                                ok = false;
                                        }
                                    }
                                }

                                if (ok)
                                {
                                    if (dragBefore)
                                    {
                                        this.m_DragInfo.InsertIdx = i;
                                        this.m_DragInfo.InsertBefore = true;
                                        separatorInserted = true;
                                    }
                                    else
                                    {
                                        this.m_DragInfo.InsertIdx = i;
                                        this.m_DragInfo.InsertBefore = false;
                                        separatorInserted = true;
                                    }
                                    needRepaint = true;
                                }
                            }
                        }
                        else
                        {
                            // initialize the drag of an already present SVG document
                            if (currentEvent.type == EventType.MouseDrag)
                            {
                                DragAndDrop.PrepareStartDrag();
                                DragAndDrop.StartDrag("Start drag");
                                this.m_DragInfo.StartDrag(atlas.SvgList[i]);
                                needRepaint = true;
                            }
                        }
                    }
                }

                // mouse is dragging inside the drop box, but not under an already present row; insertion point is inside the last element
                if (this.m_DragInfo.Dragging && !separatorInserted && svgList.Count > 0 && mousePos.y > this.m_InputAssetsRects[svgList.Count - 1].yMax)
                {
                    bool ok = true;

                    if (!(this.m_DragInfo.DraggedObject is TextAsset))
                    {
                        if (this.m_DragInfo.DraggedObject == atlas.SvgList[svgList.Count - 1])
                            ok = false;
                    }

                    if (ok)
                    {
                        this.m_DragInfo.InsertIdx = svgList.Count - 1;
                        this.m_DragInfo.InsertBefore = false;
                        needRepaint = true;
                    }
                }
            }
            else
                this.m_DragInfo.InsertIdx = -1;

            if (needRepaint)
                Repaint();
        }

        //if (currentEvent.type == EventType.MouseUp || currentEvent.rawType == EventType.MouseUp || currentEvent.type == EventType.DragExited)
        if (currentEvent.type == EventType.DragExited)
        {
            this.m_DragInfo.StopDrag();
            DragAndDrop.objectReferences = new UnityEngine.Object[0];
        }
        else
        {
            switch (currentEvent.type) {

                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (this.m_DragInfo.Dragging)
                    {
                        bool dragValid = true;

                        if (scollRect.Contains(currentEvent.mousePosition) && dragValid)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                            if (currentEvent.type == EventType.DragPerform)
                            {
                                int index;

                                // accept drag&drop operation
                                DragAndDrop.AcceptDrag();
                                // check if we are dropping a text asset
                                if (this.m_DragInfo.DraggedObject is TextAsset)
                                {
                                    // if a valid inter-position has not been selected, append the new asset at the end of list
                                    if (this.m_DragInfo.InsertIdx < 0)
                                        index = atlas.SvgList.Count;
                                    else
                                        index = (this.m_DragInfo.InsertBefore) ? this.m_DragInfo.InsertIdx : (this.m_DragInfo.InsertIdx + 1);
                                    // add the text asset to the SVG list
                                    if (atlas.InputAssetAdd(this.m_DragInfo.DraggedObject as TextAsset, index))
                                        EditorUtility.SetDirty(atlas);
                                }
                                else
                                {
                                    // we are dropping an already present SVG row
                                    index = (this.m_DragInfo.InsertBefore) ? this.m_DragInfo.InsertIdx : (this.m_DragInfo.InsertIdx + 1);
                                    if (atlas.InputAssetMove(this.m_DragInfo.DraggedObject as SVGAssetInput, index))
                                        EditorUtility.SetDirty(atlas);
                                }
                                // now we can close the drag operation
                                this.m_DragInfo.StopDrag();
                            }
                        }
                        else
                        {
                            // if we are dragging outside of the allowed drop region, simply reject the drag&drop
                            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        }
                    }
                    else
                    {
                        if (scollRect.Contains(currentEvent.mousePosition))
                        {
                            if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
                            {
                                UnityEngine.Object draggedObject = DragAndDrop.objectReferences[0];
                                // check object type, only TextAssets are allowed
                                if (draggedObject is TextAsset)
                                {
                                    this.m_DragInfo.StartDrag(DragAndDrop.objectReferences[0]);
                                    Repaint();
                                }
                                else
                                    // acceptance is not confirmed (e.g. we are dragging a binary file)
                                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        // negative values are not allowed for reference resolution
        if (refWidth <= 0)
            refWidth = Screen.currentResolution.width;
        if (refHeight <= 0)
            refHeight = Screen.currentResolution.height;
        if (deviceTestWidth <= 0)
            deviceTestWidth = refWidth;
        if (deviceTestHeight <= 0)
            deviceTestHeight = refHeight;
        // a negative value is not allowed for texture max dimension
        if (maxTexturesDimension < 0)
            maxTexturesDimension = 1024;
        // a negative value is not allowed for border
        if (border < 0)
            border = 0;
        
        // if reference resolution has been changed, update it
        if (atlas.ReferenceWidth != refWidth)
        {
            atlas.ReferenceWidth = refWidth;
            EditorUtility.SetDirty(atlas);
        }
        if (atlas.ReferenceHeight != refHeight)
        {
            atlas.ReferenceHeight = refHeight;
            EditorUtility.SetDirty(atlas);
        }
        // if device (test) resolution has been changed, update it
        if (atlas.DeviceTestWidth != deviceTestWidth)
        {
            atlas.DeviceTestWidth = deviceTestWidth;
            EditorUtility.SetDirty(atlas);
        }
        if (atlas.DeviceTestHeight != deviceTestHeight)
        {
            atlas.DeviceTestHeight = deviceTestHeight;
            EditorUtility.SetDirty(atlas);
        }
        // if scale adaption method has been changed, update it
        if (atlas.ScaleType != scaleType)
        {
            atlas.ScaleType = scaleType;
            EditorUtility.SetDirty(atlas);
        }
        // if offset additional scale has been changed, update it
        if (atlas.OffsetScale != offsetScale)
        {
            atlas.OffsetScale = Math.Abs(offsetScale);
            EditorUtility.SetDirty(atlas);
        }
        // if power-of-two forcing flag has been changed, update it
        if (atlas.Pow2Textures != pow2Textures)
        {
            atlas.Pow2Textures = pow2Textures;
            EditorUtility.SetDirty(atlas);
        }
        // if desired maximum texture dimension has been changed, update it
        if (atlas.MaxTexturesDimension != maxTexturesDimension)
        {
            atlas.MaxTexturesDimension = maxTexturesDimension;
            EditorUtility.SetDirty(atlas);
        }
        // if border between each packed SVG has been changed, update it
        if (atlas.SpritesBorder != border)
        {
            atlas.SpritesBorder = border;
            EditorUtility.SetDirty(atlas);
        }
        // if surface clear color has been changed, update it
        if (atlas.ClearColor != clearColor)
        {
            atlas.ClearColor = clearColor;
            EditorUtility.SetDirty(atlas);
        }
		// if output folder (generated sprite and textures) has been changed, update it
		if (atlas.OutputFolder != outputFolder)
		{
			atlas.OutputFolder = outputFolder;
			EditorUtility.SetDirty(atlas);
		}
        
        if (this.m_SvgListScrollPos != scrollPos)
            this.m_SvgListScrollPos = scrollPos;
        
        if (this.m_SvgSpritesScrollPos != spritesScrollPos)
            this.m_SvgSpritesScrollPos = spritesScrollPos;
    }

    public override void OnInspectorGUI()
    {
        // get the target object
		SVGAtlas atlas = target as SVGAtlas;

		if (atlas != null)
		{
	        if (!this.m_CustomStylesGenerated)
	        {
	            this.GenerateCustomStyles();
	            this.m_CustomStylesGenerated = true;
	        }

	        GUI.enabled = (Application.isPlaying) ? false : true;
	        this.DrawInspector(atlas);
		}
    }

    void OnDestroy()
    {
        // avoid to leak textures
        this.DestroyCustomStyles();
        this.m_CustomStylesGenerated = false;
    }

    // Custom styles
    [NonSerialized]
    private bool m_CustomStylesGenerated = false;
    [NonSerialized]
    private Texture2D m_BlueLineTexture = null;
    [NonSerialized]
    private GUIStyle m_BlueLine = null;
    [NonSerialized]
    private Texture2D m_GreyLineTexture = null;
    [NonSerialized]
    private GUIStyle m_GreyLine = null;
    [NonSerialized]
    private Texture2D m_HighlightRowTexture = null;
    [NonSerialized]
    private GUIStyle m_HighlightRow = null;

    private List<Rect> m_InputAssetsRects;
    private DragInfo m_DragInfo = new DragInfo();

    // Current scroll position inside the list of input SVG
    private Vector2 m_SvgListScrollPos = new Vector2(0, 0);
    // Current scroll position inside the list of generated sprites
    private Vector2 m_SvgSpritesScrollPos = new Vector2(0, 0);
    // Dimensions of the sprite preview image (list of generated sprites)
    private const float SPRITE_PREVIEW_DIMENSION = 32;
}
