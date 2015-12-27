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
#if UNITY_2_6
    #define UNITY_2_X
    #define UNITY_2_PLUS
#elif UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
    #define UNITY_3_X
    #define UNITY_2_PLUS
    #define UNITY_3_PLUS
#elif UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
    #define UNITY_4_X
    #define UNITY_2_PLUS
    #define UNITY_3_PLUS
    #define UNITY_4_PLUS
#elif UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9
    #define UNITY_5_X
    #define UNITY_2_PLUS
    #define UNITY_3_PLUS
    #define UNITY_4_PLUS
    #define UNITY_5_PLUS
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;
    using UnityEditor;
    using UnityEditorInternal;
    using System.Reflection;
#endif

public enum SVGScaleType {
    // Do not scale packed SVG.
    None                = 0,
    // Scale each packed SVG according to width.
    Horizontal          = 1,
    // Scale each packed SVG according to height.
    Vertical            = 2,
    // Scale each packed SVG according to the minimum dimension between width and height.
    MinDimension        = 3,
    // Scale each packed SVG according to the maximum dimension between width and height.
    MaxDimension        = 4
};

[System.Serializable]
public class SVGAssetInput
{
    // Constructor.
    public SVGAssetInput(TextAsset txtAsset, bool explodeGroups)
    {
        this.TxtAsset = txtAsset;
        this.SeparateGroups = explodeGroups;
        this.Instantiable = false;
        this.InstanceBaseIdx = 0;
    }

#if UNITY_EDITOR
    public string Hash()
    {
        string assetpath = AssetDatabase.GetAssetPath(this.TxtAsset);
        FileInfo fileInfo = new FileInfo(assetpath);
        return null;
#if !UNITY_WEBPLAYER
        return AssetDatabase.GetAssetPath(this.TxtAsset) + "_" + fileInfo.Length.ToString() + "_" + this.SeparateGroups.ToString();
#endif
    }
#endif

        // Text asset containing the svg xml.
    public TextAsset TxtAsset;
    // If true, it tells the packer to not pack the whole SVG document, but instead to pack each first-level element separately.
    public bool SeparateGroups;
    // True if the document can be instantiated (i.e. groups of the whole rootmost <svg> element)
    public bool Instantiable;
    // Base index for instances to be created.
    public int InstanceBaseIdx;
}

// a link between a packed SVG and the relative SVG document
public class PackedSvgAssetDocLink
{
    // Constructor.
    public PackedSvgAssetDocLink(SVGAssetInput svgAsset, SVGDocument document)
    {
        this.m_Asset = svgAsset;
        this.m_Document = document;
    }
    
    public SVGAssetInput Asset
    {
        get
        {
            return this.m_Asset;
        }
    }
    
    public SVGDocument Document
    {
        get
        {
            return this.m_Document;
        }
    }
    
    private SVGAssetInput m_Asset;
    private SVGDocument m_Document;
}
    
// Reference counting on a single SVG document
public class PackedSvgDocRef
{
    // Constructor
    public PackedSvgDocRef(SVGDocument svgDoc, TextAsset txtAsset)
    {
        this.m_SvgDoc = svgDoc;
        this.m_TxtAsset = txtAsset;
        // SVGRenameImporter substitutes the ".svg" file extension with ".svg.txt" one (e.g. orc.svg --> orc.svg.txt)
        // Unity assets explorer does not show the last ".txt" postfix (e.g. in the editor we see orc.svg without the last .txt trait)
        // txtAsset.name does not contain the last ".txt" trait, but it still contains the ".svg"; at this level, we want to remove even that
        this.m_Name = txtAsset.name.Replace(".svg", "");
        this.m_RefCount = 0;
    }
    
    // Add references
    public uint Inc(uint increment)
    {
        this.m_RefCount += increment;
        return this.m_RefCount;
    }
    
    // Remove references
    public uint Dec(uint decrement)
    {
        if (this.m_RefCount >= decrement)
            this.m_RefCount -= decrement;
        else
            this.m_RefCount = 0;
        return this.m_RefCount;
    }
    
    // Get referenced SVG document
    public SVGDocument Document
    {
        get
        {
            return this.m_SvgDoc;
        }
    }

    public TextAsset TxtAsset
    {
        get
        {
            return this.m_TxtAsset;
        }
    }

    // Get current reference count
    public uint RefCount
    {
        get
        {
            return this.m_RefCount;
        }
    }

    public string Name
    {
        get
        {
            return this.m_Name;
        }
    }
    
    // The referenced SVG document
    private SVGDocument m_SvgDoc;
    // Current reference count
    private uint m_RefCount;
    // Text asset
    private TextAsset m_TxtAsset;
    // Document (short) name
    private string m_Name;
}

[System.Serializable]
public class SVGSpriteData
{
    // Constructor.
    public SVGSpriteData(Sprite sprite, Vector2 pivot, int zOrder, int originalX, int originalY, bool inCurrentInstancesGroup)
    {
        this.Sprite = sprite;
        this.Pivot = pivot;
        this.ZOrder = zOrder;
        this.OriginalX = originalX;
        this.OriginalY = originalY;
        this.InCurrentInstancesGroup = inCurrentInstancesGroup;
    }

    public Sprite Sprite;
    public Vector2 Pivot;
    public int ZOrder;
    public int OriginalX;
    public int OriginalY;
    public bool InCurrentInstancesGroup;
}

[System.Serializable]
public class SVGSpriteRef
{
    // Constructor.
    public SVGSpriteRef(TextAsset txtAsset, int elemIdx)
    {
        this.TxtAsset = txtAsset;
        this.ElemIdx = elemIdx;
    }

    public bool Equals(SVGSpriteRef other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        // we are referencing the same sprite if the xml text asset is the same and element id matches
        return (this.TxtAsset == other.TxtAsset && this.ElemIdx == other.ElemIdx) ? true : false;
    }
    
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != typeof (SVGSpriteRef))
            return false;
        return Equals((SVGSpriteRef)obj);
    }
    
    public override int GetHashCode()
    {
        unchecked
        {
			if (this.TxtAsset != null)
			{
            	int res = ((this.TxtAsset.GetHashCode() * 397) ^ this.ElemIdx);
            	return res;
			}
			return 0;
        }
    }

    public TextAsset TxtAsset;
    public int ElemIdx; 
}

[System.Serializable]
public class AssetFile
{
    // Constructor.
    public AssetFile(string path, UnityEngine.Object obj)
    {
        this.Path = path;
        this.Object = obj;
    }

    public string Path;
    public UnityEngine.Object Object;
}

[System.Serializable]
public class SVGSpriteAssetFile
{
    // Constructor.
    public SVGSpriteAssetFile(string path, SVGSpriteRef spriteRef, SVGSpriteData spriteData)
    {
        this.Path = path;
        this.SpriteRef = spriteRef;
        this.SpriteData = spriteData;
    }
    
    public string Path;
    public SVGSpriteRef SpriteRef;
    public SVGSpriteData SpriteData;
}

// A list of sprites relative to an SVG document
[System.Serializable]
public class SVGSpritesList
{
    // Constructor
    public SVGSpritesList()
    {
        this.Sprites = new List<SVGSpriteRef>();
    }

    [SerializeField]
    public List<SVGSpriteRef> Sprites;
}

[System.Serializable]
public class SVGSpritesDictionary : SerializableDictionary<SVGSpriteRef, SVGSpriteAssetFile>
{
}

// Given a text asset (i.e. the SVG file) instance id, get the list of sprites and their original location
[System.Serializable]
public class SVGSpritesListDictionary : SerializableDictionary<int, SVGSpritesList>
{
}

public class SVGRuntimeSprite
{
    public SVGRuntimeSprite(Sprite sprite, float generationScale, SVGSpriteRef spriteReference)
    {
        this.m_Sprite = sprite;
        this.m_GenerationScale = generationScale;
		this.m_SpriteReference = spriteReference;
    }

    public Sprite Sprite
    {
        get
        {
            return this.m_Sprite;
        }
    }

    public float GenerationScale
    {
        get
        {
            return this.m_GenerationScale;
        }
    }

	public SVGSpriteRef SpriteReference
	{
		get
		{
			return this.m_SpriteReference;
		}
	}

    private Sprite m_Sprite;
    private float m_GenerationScale;
	private SVGSpriteRef m_SpriteReference;
}

public class SVGRuntimeGenerator
{
    // Constructor.
    SVGRuntimeGenerator()
    {
    }

	public static float ScaleFactorCalc(float referenceScreenWidth, float referenceScreenHeight,
                                        float currentWidth, float currentHeight,
                                        SVGScaleType scaleType, float offsetScale)
    {
        float scale;
		bool referenceLandscape, currentLandscape;

        switch (scaleType)
        {
            case SVGScaleType.Horizontal:
				scale = currentWidth / referenceScreenWidth;
                break;
            case SVGScaleType.Vertical:
				scale = currentHeight / referenceScreenHeight;
                break;
            case SVGScaleType.MinDimension:
				referenceLandscape = (referenceScreenWidth > referenceScreenHeight) ? true : false;
				currentLandscape = (currentWidth > currentHeight) ? true : false;

				if (referenceLandscape != currentLandscape)
					scale = (currentWidth <= currentHeight) ? (currentWidth / referenceScreenHeight) : (currentHeight / referenceScreenWidth);
				else
					scale = (currentWidth <= currentHeight) ? (currentWidth / referenceScreenWidth) : (currentHeight / referenceScreenHeight);
                break;
            case SVGScaleType.MaxDimension:
				referenceLandscape = (referenceScreenWidth > referenceScreenHeight) ? true : false;
				currentLandscape = (currentWidth > currentHeight) ? true : false;
				if (referenceLandscape != currentLandscape)
					scale = (currentWidth >= currentHeight) ? (currentWidth / referenceScreenHeight) : (currentHeight / referenceScreenWidth);
				else
					scale = (currentWidth >= currentHeight) ? (currentWidth / referenceScreenWidth) : (currentHeight / referenceScreenHeight);
                break;
            default:
                scale = 1;
                break;
        }
        
        // DEBUG STUFF
        //Debug.Log ("ScaleFactorCalc, current screen: " + currentWidth + " x " + currentHeight + "  scale factor: " + scale * offsetScale);

        return (scale * offsetScale);
    }

    private static SVGPackedBin[] GenerateBins(// input
                                               List<SVGAssetInput> svgList,
                                               int maxTexturesDimension,
                                               int border,
                                               bool pow2Textures,
                                               float scale,
                                               // output
                                               Dictionary<int, PackedSvgAssetDocLink> processedAssets, Dictionary<uint, PackedSvgDocRef> loadedDocuments)
    {
        SVGPacker packer = SVGAssets.CreatePacker(scale, (uint)maxTexturesDimension, (uint)border, pow2Textures);
        
        // start the packing process
        if (packer.Begin())
        {
            foreach (SVGAssetInput svgAsset in svgList)
            {
                int assetKey = svgAsset.TxtAsset.GetInstanceID();
                // if the text asset has not been already processed, lets create an SVG document out of it
                if (!processedAssets.ContainsKey(assetKey))
                {
                    // create the SVG document
                    SVGDocument svgDoc = SVGAssets.CreateDocument(svgAsset.TxtAsset.text);
                    if (svgDoc != null)
                    {
                        PackedSvgDocRef svgDocRef = new PackedSvgDocRef(svgDoc, svgAsset.TxtAsset);
                        // add the document to the packer, and get back the actual number of packed bounding boxes
                        uint[] info = packer.Add(svgDoc, svgAsset.SeparateGroups);
                        // info[0] = number of collected bounding boxes
                        // info[1] = the actual number of packed bounding boxes
                        if (info != null && info[1] == info[0])
                        {
                            // increment references
                            svgDocRef.Inc(info[1]);
                            // keep track of the processed asset / created document
                            processedAssets.Add(assetKey, new PackedSvgAssetDocLink(svgAsset, svgDoc));
                            loadedDocuments.Add(svgDoc.Handle, svgDocRef);
                        }
                        else
                        {
                        #if UNITY_EDITOR
                            if (info[1] < info[0])
                                EditorUtility.DisplayDialog("Some SVG elements cannot be packed!",
                                                            "Specified maximum texture dimensions do not allow to pack all SVG elements, please increase the value",
                                                            "Ok");
                        #else
                            UnityEngine.Debug.Log("Some SVG elements cannot be packed! Specified maximum texture dimensions do not allow to pack all SVG elements, please increase the value");
                        #endif
                            // close the packing process without doing anything
                            packer.End(false);
                            // free memory allocated by all loaded SVG documents
                            foreach (PackedSvgAssetDocLink docLink in processedAssets.Values)
                                docLink.Document.Dispose();
                            // return the error
                            return null;
                        }
                    }
                }
                else
                {
                    PackedSvgAssetDocLink existingAsset;
                    // get the (already processed svg) asset    
                    if (processedAssets.TryGetValue(assetKey, out existingAsset))
                    {
                        PackedSvgDocRef svgDocRef;
                        // get the (already created) svg document
                        if (loadedDocuments.TryGetValue(existingAsset.Document.Handle, out svgDocRef))
                        {
                            // add the document to the packer, and get back the actual number of packed bounding boxes
                            uint[] info = packer.Add(svgDocRef.Document, svgAsset.SeparateGroups);
                            // info[0] = number of collected bounding boxes
                            // info[1] = the actual number of packed bounding boxes
                            if (info != null && info[1] == info[0])
                                // increment references
                                svgDocRef.Inc(info[1]);
                            else
                            {
                            #if UNITY_EDITOR
                                if (info[1] < info[0])
                                    EditorUtility.DisplayDialog("Some SVG elements cannot be packed!",
                                                                "Specified maximum texture dimensions do not allow to pack all SVG elements, please increase the value",
                                                                "Ok");
                            #else
                                UnityEngine.Debug.Log("Some SVG elements cannot be packed! Specified maximum texture dimensions do not allow to pack all SVG elements, please increase the value");
                            #endif
                                // close the packing process without doing anything
                                packer.End(false);
                                // free memory allocated by all loaded SVG documents
                                foreach (PackedSvgAssetDocLink docLink in processedAssets.Values)
                                    docLink.Document.Dispose();
                                // return the error
                                return null;
                            }
                        }
                    }
                }
            }
            // return generated bins
            return packer.End(true);
        }
        else
            return null;
    }

    private static bool GenerateSpritesFromBins(// input
                                                SVGPackedBin[] bins, Dictionary<uint, PackedSvgDocRef> loadedDocuments,
                                                Color clearColor,
                                                //SpritesCustomDataDictionary srcSpritesData,
                                                SVGSpritesDictionary previousSprites,
                                                // output
                                                List<Texture2D> textures, List<KeyValuePair<SVGSpriteRef, SVGSpriteData>> sprites,
                                                SVGSpritesListDictionary spritesListDict)
    {
        if (bins == null || loadedDocuments == null || textures == null || sprites == null)
            return false;
        
        // sprite reference/key used to get pivot
        SVGSpriteRef tmpRef = new SVGSpriteRef(null, 0);
        
        for (int i = 0; i < bins.Length; ++i)
        {
            // extract the bin
            SVGPackedBin bin = bins[i];
            // create drawing surface
            SVGSurface surface = SVGAssets.CreateSurface(bin.Width, bin.Height);
            
            if (surface != null)
            {
                // draw packed rectangles of the current bin
                if (surface.Draw(bin, new SVGColor(clearColor.r, clearColor.g, clearColor.b, clearColor.a), SVGRenderingQuality.Better))
                {
                    // bin rectangles
                    SVGPackedRectangle[] rectangles = bin.Rectangles;
                    
                    // create the texture
                    Texture2D texture = new Texture2D((int)surface.Width, (int)surface.Height, TextureFormat.ARGB32, false);
                    texture.filterMode = FilterMode.Bilinear;
                    texture.wrapMode = TextureWrapMode.Clamp;
                    texture.anisoLevel = 1;
                #if UNITY_EDITOR
                    texture.alphaIsTransparency = true;
                #endif
                    // copy the surface content inside the texture; NB: we set the delateEdgesFix flag because we are using bilinear texture filtering
                    //surface.Copy(texture, true);        
                    // push the created texture
                    textures.Add(texture);
                    
                    for (int j = 0; j < rectangles.Length; ++j)
                    {
                        PackedSvgDocRef svgDocRef;
                        SVGPackedRectangle rect = rectangles[j];
                        // get access to the referenced SVG document
                        if (loadedDocuments.TryGetValue(rect.DocHandle, out svgDocRef))
                        {
                            SVGSpriteAssetFile spriteAsset;
                            Vector2 pivot;
                            bool inCurrentInstancesGroup;
                            // try to see if this sprite was previously generated, and if so get its pivot
                            tmpRef.TxtAsset = svgDocRef.TxtAsset;
                            tmpRef.ElemIdx = (int)rect.ElemIdx;
                            // get the previous pivot if present, else start with a default centered pivot
                            if (previousSprites.TryGetValue(tmpRef, out spriteAsset))
                            {
                                pivot = spriteAsset.SpriteData.Pivot;
                                inCurrentInstancesGroup = spriteAsset.SpriteData.InCurrentInstancesGroup;
                            }
                            else
                            {
                                pivot = new Vector2(0.5f, 0.5f);
                                inCurrentInstancesGroup = false;
                            }
                            // create a new sprite
                            Sprite sprite = Sprite.Create(texture, new Rect((float)rect.X, (float)((int)bin.Height - rect.Y - rect.Height), (float)rect.Width, (float)rect.Height), pivot, SVGAtlas.SPRITE_PIXELS_PER_UNIT);
                            sprite.name = svgDocRef.Name + "_" + rect.Name;
                            // push the sprite reference
                            SVGSpriteRef key = new SVGSpriteRef(svgDocRef.TxtAsset, (int)rect.ElemIdx);
                            SVGSpriteData value = new SVGSpriteData(sprite, pivot, rect.ZOrder, /*rect.Scale,*/ rect.OriginalX, rect.OriginalY, inCurrentInstancesGroup);
                            sprites.Add(new KeyValuePair<SVGSpriteRef, SVGSpriteData>(key, value));
                            // check if we are interested in getting, for each SVG document, the list of its generated sprites
                            if (spritesListDict != null)
                            {
                                SVGSpritesList spritesList;
                                if (!spritesListDict.TryGetValue(svgDocRef.TxtAsset.GetInstanceID(), out spritesList))
                                {
                                    // create the list of sprites location relative to the SVG text asset
                                    spritesList = new SVGSpritesList();
                                    spritesListDict.Add(svgDocRef.TxtAsset.GetInstanceID(), spritesList);
                                }
                                // add the new sprite the its list
                                spritesList.Sprites.Add(key);
                            }

                            // decrement document references
                            if (svgDocRef.Dec(1) == 0)
                                // we can free AmanithSVG native SVG document
                                svgDocRef.Document.Dispose();
                        }
                    }
                    // copy the surface content inside the texture; NB: we set the delateEdgesFix flag because we are using bilinear texture filtering
                    surface.Copy(texture, true);
                #if UNITY_EDITOR
                    texture.Apply(false, false);
                #else
                    // make the texture no longer readable
                    texture.Apply(false, true);
                #endif
                }
                // destroy the AmanithSVG rendering surface
                surface.Dispose();
            }
        }
        return true;
    }

    public static bool GenerateSprites(// input
                                       List<SVGAssetInput> svgList,
                                       int maxTexturesDimension,
                                       int border,
                                       bool pow2Textures,
                                       float scale,
                                       Color clearColor,
                                       SVGSpritesDictionary previousSprites,
                                       // output
                                       List<Texture2D> textures,
                                       List<KeyValuePair<SVGSpriteRef, SVGSpriteData>> sprites,
                                       SVGSpritesListDictionary spritesList)
    {
        // create dictionaries
        Dictionary<int, PackedSvgAssetDocLink> processedAssets = new Dictionary<int, PackedSvgAssetDocLink>();
        Dictionary<uint, PackedSvgDocRef> loadedDocuments = new Dictionary<uint, PackedSvgDocRef>();
        // generate bins
        SVGPackedBin[] bins = SVGRuntimeGenerator.GenerateBins(svgList, maxTexturesDimension, border, pow2Textures, scale, processedAssets, loadedDocuments);
        if (bins != null)
        {
            // generate textures and sprites
            if (SVGRuntimeGenerator.GenerateSpritesFromBins(bins, loadedDocuments, clearColor, previousSprites, textures, sprites, spritesList))
                return true;
        }
        return false;
    }
}

public class SVGAtlas: ScriptableObject
{
#if UNITY_EDITOR
    private void CheckAndCreateFolder(string parentFolder, string newFolderName)
    {
        string path = Application.dataPath + parentFolder.TrimStart("Assets".ToCharArray()) + "/" + newFolderName;

        if (!System.IO.Directory.Exists(path))
            AssetDatabase.CreateFolder(parentFolder, newFolderName);
    }

    private void CheckAndDeleteFolder(string path)
    {
#if !UNITY_WEBPLAYER
        if (System.IO.Directory.Exists(path))
            System.IO.Directory.Delete(path, true);
#endif
    }

    private string CreateFolders()
    {
		this.CheckAndCreateFolder(this.m_OutputFolder, "Atlases");
		this.CheckAndCreateFolder(this.m_OutputFolder + "/Atlases", this.name);

		string atlasesPath = this.m_OutputFolder + "/Atlases/" + this.name;
		this.CheckAndCreateFolder(atlasesPath, "Textures");
		this.CheckAndCreateFolder(atlasesPath, "Sprites");
        
        return atlasesPath;
    }

    private void FixAnimationClip(AnimationClip clip, float deltaScaleX, float deltaScaleY)
    {
        AnimationClipCurveData[] curves = AnimationUtility.GetAllCurves(clip, true);
        foreach (AnimationClipCurveData curveData in curves)
        {
            // check for keyframe animation of localPosition
            bool localPosX = (curveData.propertyName.IndexOf("LocalPosition.x", StringComparison.OrdinalIgnoreCase) >= 0) ? true : false;
            bool localPosY = (curveData.propertyName.IndexOf("LocalPosition.y", StringComparison.OrdinalIgnoreCase) >= 0) ? true : false;

            if (localPosX || localPosY)
            {
                AnimationCurve curve = curveData.curve;
                // get the scale factor
                float deltaScale = localPosX ? deltaScaleX : deltaScaleY;

                // "Note that the keys array is by value, i.e. getting keys returns a copy of all keys and setting keys copies them into the curve"
                Keyframe[] keys = curve.keys;
                for (int i = 0; i < keys.Length; ++i) {
                    keys[i].value *= deltaScale;
                    keys[i].inTangent *= deltaScale;
                    keys[i].outTangent *= deltaScale;
                }
                curve.keys = keys;
                // set the new keys
                clip.SetCurve(curveData.path, curveData.type, curveData.propertyName, curve);
            }
        }
    }

    private void FixPositions(GameObject gameObj, float deltaScaleX, float deltaScaleY)
    {
        Vector3 newPos = gameObj.transform.localPosition;
        newPos.x *= deltaScaleX;
        newPos.y *= deltaScaleY;
        gameObj.transform.localPosition = newPos;

        // fix Animation components
        Animation[] animations = gameObj.GetComponents<Animation>();
        foreach (Animation animation in animations)
        {
            foreach (AnimationState animState in animation)
            {
                if (animState.clip != null)
                    this.FixAnimationClip(animState.clip, deltaScaleX, deltaScaleY);
            }
        }

        // fix Animator components
        Animator[] animators = gameObj.GetComponents<Animator>();
        foreach (Animator animator in animators)
        {
            UnityEditor.Animations.AnimatorController animController = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
#if UNITY_5_PLUS
			for (int i = 0; i < animController.layers.Length; i++)
#else
            for (int i = 0; i < animator.layerCount; i++)
#endif
            {
#if UNITY_5_PLUS
				UnityEditor.Animations.AnimatorStateMachine stateMachine = animController.layers[i].stateMachine;
				for (int j = 0; j < stateMachine.states.Length; j++)
#else
                UnityEditor.Animations.AnimatorStateMachine stateMachine = animController.GetLayer(i).stateMachine;
				for (int j = 0; j < stateMachine.stateCount; j++)
#endif
                {
#if UNITY_5_PLUS
					UnityEditor.Animations.ChildAnimatorState state = stateMachine.states[j];
					Motion mtn = state.state.motion;
#else
                    UnityEditor.Animations.AnimatorState state = stateMachine.GetState(j);
					Motion mtn = state.GetMotion();
#endif

                    if (mtn != null)
                    {
                        AnimationClip clip = mtn as AnimationClip;
                        this.FixAnimationClip(clip, deltaScaleX, deltaScaleY);
                    }
                }
            }
        }
    }

    private void GetSpritesInstances(List<GameObject> spritesInstances)
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        
        foreach (GameObject gameObj in allObjects)
        {
            // check if the game object is an "SVG sprite" instance of this atlas generator
            if (gameObj.activeInHierarchy)
            {
                SVGSpriteLoaderBehaviour loader = gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
                // we must be sure that the loader component must refer to this atlas
                if (loader != null && loader.Atlas == this)
                    // add this instance to the output lists
                    spritesInstances.Add(gameObj);
            }
        }
    }

    private void DeleteTextures()
    {
        foreach (AssetFile file in this.GeneratedTexturesFiles)
            AssetDatabase.DeleteAsset(file.Path);
        
        this.GeneratedTexturesFiles.Clear();
    }
    
    private void DeleteSprites()
    {
        foreach (KeyValuePair<SVGSpriteRef, SVGSpriteAssetFile> file in this.GeneratedSpritesFiles)
        {
            SVGSpriteAssetFile spriteAsset = file.Value;
            AssetDatabase.DeleteAsset(spriteAsset.Path);
        }

        this.GeneratedSpritesFiles.Clear();
    }

    private void DeleteGameObjects(List<GameObject> objects)
    {
        if (objects != null && objects.Count > 0)
        {
            foreach (GameObject gameObj in objects)
                GameObject.DestroyImmediate(gameObj);
        }
    }

    private void UpdateSprites(float newScale)
    {
        // get the list of instantiated SVG sprites
        List<GameObject> spritesInstances = new List<GameObject>();
        this.GetSpritesInstances(spritesInstances);
        // regenerate the list of sprite locations
        this.GeneratedSpritesLists = new SVGSpritesListDictionary();

        if (this.SvgList.Count <= 0)
        {
            AssetDatabase.StartAssetEditing();
            // delete previously generated textures (i.e. get all this.GeneratedTextures entries and delete the relative files)
            this.DeleteTextures();
            // delete previously generated sprites (i.e. get all this.GeneratedSprites entries and delete the relative files)
            this.DeleteSprites();

            if (spritesInstances.Count > 0)
            {
                bool remove = EditorUtility.DisplayDialog("Missing sprite!",
                                                          string.Format("{0} gameobjects reference sprites that do not exist anymore. Would you like to remove them from the scene?", spritesInstances.Count),
                                                          "Remove", "Keep");
                if (remove)
                    this.DeleteGameObjects(spritesInstances);
            }
            AssetDatabase.StopAssetEditing();
            // input SVG list is empty, simply reset both hash
            this.m_SvgListHashOld = this.m_SvgListHashCurrent = "";
            return;
        }

        // generate textures and sprites
        List<Texture2D> textures = new List<Texture2D>();
        List<KeyValuePair<SVGSpriteRef, SVGSpriteData>> sprites = new List<KeyValuePair<SVGSpriteRef, SVGSpriteData>>();

        if (SVGRuntimeGenerator.GenerateSprites(// input
                                                this.SvgList, this.m_MaxTexturesDimension, this.m_SpritesBorder, this.m_Pow2Textures, newScale, this.m_ClearColor, this.GeneratedSpritesFiles,
                                                // output
                                                textures, sprites, this.GeneratedSpritesLists))
        {
            int i, j;

            if (this.EditorGenerationScale > 0 && newScale != this.EditorGenerationScale)
            {
                // calculate how much we have to scale (relative) positions
                float deltaScale = newScale / this.EditorGenerationScale;
                // fix objects positions and animations
                foreach (GameObject gameObj in spritesInstances)
                    this.FixPositions(gameObj, deltaScale, deltaScale);
            }
            // keep track of the new generation scale
            this.EditorGenerationScale = newScale;

            AssetDatabase.StartAssetEditing();
            // delete previously generated textures (i.e. get all this.GeneratedTextures entries and delete the relative files)
            this.DeleteTextures();
            // delete previously generated sprites (i.e. get all this.GeneratedSprites entries and delete the relative files)
            this.DeleteSprites();
            // ensure the presence of needed subdirectories
            string atlasesPath = this.CreateFolders();
            string texturesDir = atlasesPath + "/Textures/";
            string spritesDir = atlasesPath + "/Sprites/";
            // save new texture assets
            i = 0;
            foreach (Texture2D texture in textures)
            {
                string textureFileName = texturesDir + "texture" + i + ".asset";
                // save texture
                AssetDatabase.CreateAsset(texture, textureFileName);

                // DEBUG STUFF
                //byte[] pngData = texture.EncodeToPNG();
                //if (pngData != null)
                //  System.IO.File.WriteAllBytes(texturesDir + "texture" + i + ".png", pngData);

                // keep track of the saved texture
                this.GeneratedTexturesFiles.Add(new AssetFile(textureFileName, texture));
                i++;
            }
            // save sprite assets
            j = sprites.Count;
            for (i = 0; i < j; ++i)
            {
                // get sprite reference and its pivot
                SVGSpriteRef spriteRef = sprites[i].Key;
                SVGSpriteData spriteData = sprites[i].Value;

                // build sprite file name
                string spriteFileName = spritesDir + spriteData.Sprite.name + ".asset";
                // save sprite asset
                AssetDatabase.CreateAsset(spriteData.Sprite, spriteFileName);
                // keep track of the saved sprite and its pivot
                this.GeneratedSpritesFiles.Add(spriteRef, new SVGSpriteAssetFile(spriteFileName, spriteRef, spriteData));
            }
            AssetDatabase.StopAssetEditing();

            // for already instantiated (SVG) game object, set the new sprites
            // in the same loop we keep track of those game objects that reference missing sprites (i.e. sprites that do not exist anymore)
            List<GameObject> missingSpriteObjs = new List<GameObject>();
            foreach (GameObject gameObj in spritesInstances)
            {
                SVGSpriteAssetFile spriteAsset;
                SVGSpriteLoaderBehaviour spriteLoader = (SVGSpriteLoaderBehaviour)gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
                
				if (spriteLoader.SpriteReference.TxtAsset != null)
				{
	                if (this.GeneratedSpritesFiles.TryGetValue(spriteLoader.SpriteReference, out spriteAsset))
	                {
	                    // link the new sprite to the renderer
	                    SpriteRenderer renderer = (SpriteRenderer)gameObj.GetComponent<SpriteRenderer>();
	                    if (renderer != null)
	                    {
	                        SVGSpriteData spriteData = spriteAsset.SpriteData;
	                        // assign the new sprite
	                        renderer.sprite = spriteData.Sprite;
	                        // NB: existing instances do not change sorting order!
	                    }
	                }
	                else
	                    missingSpriteObjs.Add(gameObj);
				}
            }

            if (missingSpriteObjs.Count > 0)
            {
                bool remove = EditorUtility.DisplayDialog("Missing sprite!",
                                                          string.Format("{0} gameobjects reference sprites that do not exist anymore. Would you like to remove them from the scene?", missingSpriteObjs.Count),
                                                          "Remove", "Keep");
                if (remove)
                    this.DeleteGameObjects(missingSpriteObjs);
            }

            // now SVG documents are instantiable
            foreach (SVGAssetInput svgAsset in this.SvgList)
                svgAsset.Instantiable = true;
            // keep track of the new hash
            this.m_SvgListHashOld = this.m_SvgListHashCurrent;
        }
    }

    public void UpdateSprites(bool recalcScale)
    {
        float newScale;

        if (recalcScale)
        {
			Vector2 gameViewRes = SVGUtils.GetGameView();

			float currentWidth = (this.m_DeviceTestWidth <= 0) ? gameViewRes.x : (float)this.m_DeviceTestWidth;
			float currentHeight = (this.m_DeviceTestHeight <= 0) ? gameViewRes.y : (float)this.m_DeviceTestHeight;
            newScale = SVGRuntimeGenerator.ScaleFactorCalc((float)this.m_ReferenceWidth, (float)this.m_ReferenceHeight, currentWidth, currentHeight, this.m_ScaleType, this.m_OffsetScale);
        }
        else
            newScale = this.EditorGenerationScale;

        this.UpdateSprites(newScale);
    }

    private void UpdatePivotHierarchy(GameObject gameObj, Vector2 delta, uint depthLevel)
    {
        SVGSpriteLoaderBehaviour loader = gameObj.GetComponent<SVGSpriteLoaderBehaviour>();

        if (loader != null)
        {
            Vector2 realDelta = (depthLevel > 0) ? (new Vector2(-delta.x, -delta.y)) : (new Vector2(delta.x * gameObj.transform.localScale.x, delta.y * gameObj.transform.localScale.y));
            Vector2 newPos = new Vector2(gameObj.transform.localPosition.x + realDelta.x, gameObj.transform.localPosition.y + realDelta.y);
            // modify the current node
            gameObj.transform.localPosition = newPos;
        }

        // traverse children
        int j = gameObj.transform.childCount;
        for (int i = 0; i < j; ++i)
        {
            GameObject child = gameObj.transform.GetChild(i).gameObject;
            this.UpdatePivotHierarchy(child, delta, depthLevel + 1);
        }
    }

    public void UpdatePivot(SVGSpriteAssetFile spriteAsset, Vector2 newPivot)
    {
        SVGSpriteRef spriteRef = spriteAsset.SpriteRef;
        SVGSpriteData spriteData = spriteAsset.SpriteData;
        Sprite oldSprite = spriteData.Sprite;
        // keep track of pivot movement
        Vector2 deltaPivot = newPivot - spriteData.Pivot;
        Vector2 deltaMovement = (new Vector2(deltaPivot.x * oldSprite.rect.width, deltaPivot.y * oldSprite.rect.height)) / SVGAtlas.SPRITE_PIXELS_PER_UNIT;
        // create a new sprite (same texture, same rectangle, different pivot)
        Sprite newSprite = Sprite.Create(oldSprite.texture, oldSprite.rect, newPivot, SVGAtlas.SPRITE_PIXELS_PER_UNIT);
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject gameObj in allObjects)
        {
            if (gameObj.activeInHierarchy)
            {
                SVGSpriteLoaderBehaviour loader = gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
                // we must be sure that the loader component must refer to this atlas
                if (loader != null && loader.Atlas == this)
                {
                    // check if the instance uses the specified sprite
                    if (loader.SpriteReference.TxtAsset == spriteRef.TxtAsset && loader.SpriteReference.ElemIdx == spriteRef.ElemIdx)
                        this.UpdatePivotHierarchy(gameObj, deltaMovement, 0);
                }
            }
        }

        spriteData.Pivot = newPivot;
        newSprite.name = oldSprite.name;
        EditorUtility.CopySerialized(newSprite, oldSprite);
		// destroy the temporary sprite
		GameObject.DestroyImmediate (newSprite);
    }

    private int SvgIndexGet(TextAsset txtAsset)
    {
        if (txtAsset != null)
        {
            // find the SVG index inside the SvgList
            int j = this.SvgList.Count;
            for (int i = 0; i < j; ++i)
            {
                SVGAssetInput svgAsset = this.SvgList[i];
                if (svgAsset.TxtAsset == txtAsset)
                    return i;
            }
        }
        // if we have not found the svg index, return -1 as error
        return -1;
    }

    private void SortingOrdersCompact(SVGAssetInput svgAsset)
    {
        List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
        // get the list of instantiated sprites relative to this atlas generator
        List<GameObject> spritesInstances = new List<GameObject>();
        this.GetSpritesInstances(spritesInstances);
        
        foreach (GameObject gameObj in spritesInstances)
        {
            SVGSpriteLoaderBehaviour spriteLoader = (SVGSpriteLoaderBehaviour)gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
            SVGSpriteRef spriteRef = spriteLoader.SpriteReference;
            // if the sprite belongs to the specified SVG asset input, keep track of it
            if (spriteRef.TxtAsset == svgAsset.TxtAsset)
            {
                SpriteRenderer renderer = (SpriteRenderer)gameObj.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    spriteRenderers.Add(renderer);
            }
        }

        if (spriteRenderers.Count > 0)
        {
            // order the list by current sorting order
            spriteRenderers.Sort(delegate(SpriteRenderer renderer1, SpriteRenderer renderer2) {
                if (renderer1.sortingOrder < renderer2.sortingOrder)
                    return -1;
                if (renderer1.sortingOrder > renderer2.sortingOrder)
                    return 1;
                return 0;
            });

            int j = spriteRenderers.Count;
            for (int i = 0; i < j; ++i)
            {
                SpriteRenderer renderer = spriteRenderers[i];
                int currentOrder = renderer.sortingOrder;
                // isolate high part
                int svgIndex = currentOrder & SPRITES_SORTING_DOCUMENTS_MASK;
                // assign the new order
                renderer.sortingOrder = SVGAtlas.SortingOrderCalc(svgIndex, i);
            }
            svgAsset.InstanceBaseIdx = j;
        }
        else
            // there are no sprite instances relative to the specified SVG, so we can start from 0
            svgAsset.InstanceBaseIdx = 0;
    }

    private void ResetGroupFlags(SVGSpritesList spritesList)
    {
        // now we can unflag sprites
        foreach (SVGSpriteRef spriteRef in spritesList.Sprites)
        {
            // get sprite and its data
            SVGSpriteAssetFile spriteAsset;
            if (this.GeneratedSpritesFiles.TryGetValue(spriteRef, out spriteAsset))
            {
                SVGSpriteData spriteData = spriteAsset.SpriteData;
                spriteData.InCurrentInstancesGroup = false;
            }
        }
    }

    private bool InstancesGroupWrap(SVGAssetInput svgAsset, int spritesCount)
    {
        int rangeLo = svgAsset.InstanceBaseIdx;
        int rangeHi = rangeLo + spritesCount;
        return (rangeHi >= SPRITES_SORTING_MAX_INSTANCES) ? true : false;
    }

    private void NextInstancesGroup(SVGAssetInput svgAsset, SVGSpritesList spritesList, int instantiationCount)
    {
        int spritesCount = spritesList.Sprites.Count;

        svgAsset.InstanceBaseIdx += spritesCount;
        if (this.InstancesGroupWrap(svgAsset, spritesCount))
        {
            // try to compact used sorting orders (looping game objects that reference this svg)
            this.SortingOrdersCompact(svgAsset);

            // after compaction, if the instantiation of one or all sprites belonging to the new instances group will wrap
            // we have two options:
            //
            // 1. to instantiate sprites in the normal consecutive way, wrapping aroung SPRITES_SORTING_MAX_INSTANCES: in this case a part of sprites will
            // result (sortingOrder) consistent, but the whole sprites group won't
            //
            // 2. to reset the base index to 0 and generate the sprites according to their natural z-order: in this case the whole sprites group will
            // be (sortingOrder) consistent, but it is not granted to be totally (z)separated from other sprites/instances
            //

            if (this.InstancesGroupWrap(svgAsset, spritesCount))
            {
                svgAsset.InstanceBaseIdx = 0;
                /*
                // option 2
                if (instantiationCount > 1)
                    packedSvg.InstanceBaseIdx = 0;
                // for single sprite instantiation we implicitly use option 1
                */
            }
        }

        // now we can unflag sprites
        this.ResetGroupFlags(spritesList);
    }

    private static int SortingOrderCalc(int svgIndex, int instance)
    {
        svgIndex = svgIndex % SPRITES_SORTING_MAX_DOCUMENTS;
        instance = instance % SPRITES_SORTING_MAX_INSTANCES;

        return ((svgIndex << SPRITES_SORTING_INSTANCES_BITS) + instance);
    }

    private static int SortingOrderCalc(int svgIndex, int instanceBaseIdx, int zOrder)
    {
        return SVGAtlas.SortingOrderCalc(svgIndex, instanceBaseIdx + zOrder);
    }

    private int SortingOrderGenerate(SVGSpriteAssetFile spriteAsset)
    {
        if (spriteAsset != null)
        {
            SVGSpriteRef spriteRef = spriteAsset.SpriteRef;
            SVGSpriteData spriteData = spriteAsset.SpriteData;

            int svgIndex = this.SvgIndexGet(spriteRef.TxtAsset);
            if (svgIndex >= 0)
            {
                SVGSpritesList spritesList;
                SVGAssetInput svgAsset = this.SvgList[svgIndex];

                // if needed, advance in the instances group
                if (spriteData.InCurrentInstancesGroup)
                {
                    // get the list of sprites (references) relative to the SVG input asset
                    if (this.GeneratedSpritesLists.TryGetValue(svgAsset.TxtAsset.GetInstanceID(), out spritesList))
                        // advance instances group, telling that we are going to instantiate one sprite only
                        this.NextInstancesGroup(svgAsset, spritesList, 1);
                }
                return SVGAtlas.SortingOrderCalc(svgIndex, svgAsset.InstanceBaseIdx, spriteData.ZOrder);
            }
        }
        return -1;
    }

    // recalculate sorting orders of instantiated sprites: changing is due only to SVG index, so the lower part (group + zNatural) is left unchanged
    private void SortingOrdersUpdateSvgIndex()
    {
        // get the list of instantiated sprites relative to this atlas generator
        List<GameObject> spritesInstances = new List<GameObject>();
        this.GetSpritesInstances(spritesInstances);
        
        foreach (GameObject gameObj in spritesInstances)
        {
            SVGSpriteLoaderBehaviour spriteLoader = (SVGSpriteLoaderBehaviour)gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
            SpriteRenderer renderer = (SpriteRenderer)gameObj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                SVGSpriteRef spriteRef = spriteLoader.SpriteReference;
                int svgIndex = this.SvgIndexGet(spriteRef.TxtAsset);
                if (svgIndex >= 0)
                {
                    int instance = renderer.sortingOrder & SPRITES_SORTING_INSTANCES_MASK;
                    renderer.sortingOrder = SVGAtlas.SortingOrderCalc(svgIndex, instance);
                }
            }
        }
    }

    private GameObject Instantiate(SVGSpriteAssetFile spriteAsset, int sortingOrder)
    {
        SVGSpriteRef spriteRef = spriteAsset.SpriteRef;
        SVGSpriteData spriteData = spriteAsset.SpriteData;
        GameObject gameObj = new GameObject();
        SpriteRenderer renderer = (SpriteRenderer)gameObj.AddComponent<SpriteRenderer>();
        SVGSpriteLoaderBehaviour spriteLoader = (SVGSpriteLoaderBehaviour)gameObj.AddComponent<SVGSpriteLoaderBehaviour>();
		renderer.sprite = spriteData.Sprite;
		renderer.sortingOrder = sortingOrder;
        spriteLoader.Atlas = this;
        spriteLoader.SpriteReference = spriteRef;
        spriteLoader.ResizeOnStart = true;
        gameObj.name = spriteData.Sprite.name;
        spriteData.InCurrentInstancesGroup = true;
        return gameObj;
    }

    private GameObject InstantiateSprite(SVGSpriteAssetFile spriteAsset, Vector3 worldPos, int sortingOrder)
    {
        GameObject gameObj = this.Instantiate(spriteAsset, sortingOrder);
        // assign world position
        gameObj.transform.position = worldPos;
        return gameObj;
    }

    public GameObject InstantiateSprite(SVGSpriteRef spriteRef, Vector3 worldPos)
    {
        SVGSpriteAssetFile spriteAsset;

        if (this.GeneratedSpritesFiles.TryGetValue(spriteRef, out spriteAsset))
        {
            int sortingOrder = this.SortingOrderGenerate(spriteAsset);
            GameObject gameObj = this.Instantiate(spriteAsset, sortingOrder);
            // assign world position
            gameObj.transform.position = worldPos;
            return gameObj;
        }
        return null;
    }

    public GameObject InstantiateSprite(SVGSpriteRef spriteRef)
    {
        SVGSpriteAssetFile spriteAsset;

        if (this.GeneratedSpritesFiles.TryGetValue(spriteRef, out spriteAsset))
        {
            int sortingOrder = this.SortingOrderGenerate(spriteAsset);
            return this.Instantiate(spriteAsset, sortingOrder);
        }
        return null;
    }

    public GameObject[] InstantiateGroups(SVGAssetInput svgAsset)
    {
        SVGSpritesList spritesList;

        if (svgAsset != null && this.GeneratedSpritesLists.TryGetValue(svgAsset.TxtAsset.GetInstanceID(), out spritesList))
        {
            int spritesCount = spritesList.Sprites.Count;
            int svgIndex = this.SvgIndexGet(svgAsset.TxtAsset);
            if (svgIndex >= 0 && spritesCount > 0)
            {
                // list of sprite assets (file) relative to the specified SVG; in this case we can set the right list capacity
                List<SVGSpriteAssetFile> spriteAssets = new List<SVGSpriteAssetFile>(spritesCount);

                bool advanceInstancesGroup = false;
                // now we are sure that at least one valid sprite box exists
                float xMin = float.MaxValue;
                float yMin = float.MaxValue;
                float xMax = float.MinValue;
                float yMax = float.MinValue;

                foreach (SVGSpriteRef spriteRef in spritesList.Sprites)
                {
                    SVGSpriteAssetFile spriteAsset;
                    if (this.GeneratedSpritesFiles.TryGetValue(spriteRef, out spriteAsset))
                    {
                        SVGSpriteData spriteData = spriteAsset.SpriteData;
                        Sprite sprite = spriteData.Sprite;
                        //float scl = 1 / spriteData.Scale;
                        float scl = 1;
                        float ox = (float)spriteData.OriginalX;
                        float oy = (float)spriteData.OriginalY;
                        float spriteMinX = ox * scl;
                        float spriteMinY = oy * scl;
                        float spriteMaxX = (ox + sprite.rect.width) * scl;
                        float spriteMaxY = (oy + sprite.rect.height) * scl;

                        // update min corner
                        if (spriteMinX < xMin)
                            xMin = spriteMinX;
                        if (spriteMinY < yMin)
                            yMin = spriteMinY;
                        // update max corner
                        if (spriteMaxX > xMax)
                            xMax = spriteMaxX;
                        if (spriteMaxY > yMax)
                            yMax = spriteMaxY;
                        // if there is a single sprite already instantiated in the current group, we have to advance in the next instances group
                        if (spriteData.InCurrentInstancesGroup)
                            advanceInstancesGroup = true;
                        // keep track of this sprite asset
                        spriteAssets.Add(spriteAsset);
                    }
                }

                if (spriteAssets.Count > 0)
                {
                    // because at least one valid sprite box exists, now we are sure that a valid "global" box has been calculated
                    float centerX = (xMin + xMax) / 2;
                    float centerY = (yMin + yMax) / 2;
                    float boxHeight = yMax - yMin;
                    List<GameObject> instances = new List<GameObject>();

                    if (advanceInstancesGroup)
                        // advance in the instances group, telling that we are going to instantiate N sprites
                        this.NextInstancesGroup(svgAsset, spritesList, spriteAssets.Count);

                    foreach (SVGSpriteAssetFile spriteAsset in spriteAssets)
                    {
                        SVGSpriteData spriteData = spriteAsset.SpriteData;
                        Sprite sprite = spriteData.Sprite;
                        Vector2 pivot = spriteData.Pivot;
                        //float scl = 1 / spriteData.Scale;
                        float scl = 1;
                        float px = (sprite.rect.width * pivot.x + (float)spriteData.OriginalX) * scl - centerX;
                        float py = boxHeight - (sprite.rect.height * (1 - pivot.y) + (float)spriteData.OriginalY) * scl - centerY;
                        Vector2 worldPos = new Vector2(px / SVGAtlas.SPRITE_PIXELS_PER_UNIT, py / SVGAtlas.SPRITE_PIXELS_PER_UNIT);
                        // instantiate the object
                        int sortingOrder = SVGAtlas.SortingOrderCalc(svgIndex, svgAsset.InstanceBaseIdx, spriteData.ZOrder);
                        //instances.Add(this.InstantiateSprite(spriteAsset, worldPos, sortingOrder));
                        GameObject newObj = this.InstantiateSprite(spriteAsset, worldPos, sortingOrder);
                        newObj.transform.localScale = new Vector3(scl, scl, 1);
                        spriteData.InCurrentInstancesGroup = true;
                    }
                    // return the created instances
                    return instances.ToArray();
                }
            }
        }
        return null;
    }

    // Given a text asset representing an SVG file, return the list of generated sprites relative to that document
    public List<SVGSpriteAssetFile> GetGeneratedSpritesByDocument(TextAsset txtAsset)
    {
        if (txtAsset == null)
            return null;

        // create the output list
        List<SVGSpriteAssetFile> result = new List<SVGSpriteAssetFile>();

        foreach (KeyValuePair<SVGSpriteRef, SVGSpriteAssetFile> file in this.GeneratedSpritesFiles)
        {
            SVGSpriteAssetFile spriteAsset = file.Value;
            if (spriteAsset.SpriteRef.TxtAsset == txtAsset)
                result.Add(spriteAsset);
        }

        return result;
    }

	public SVGSpriteAssetFile GetGeneratedSprite(SVGSpriteRef spriteRef)
	{
		SVGSpriteAssetFile spriteAsset;

		if (this.GeneratedSpritesFiles.TryGetValue(spriteRef, out spriteAsset))
		    return spriteAsset;
		return null;
	}

    public bool InputAssetAdd(TextAsset newSvg, int index)
    {
        bool alreadyExist = false;

        if (index < 0)
            index = 0;
        if (index > this.SvgList.Count)
            index = this.SvgList.Count;

        foreach (SVGAssetInput svgAsset in this.SvgList)
        {
            if (svgAsset.TxtAsset == newSvg)
            {
                alreadyExist = true;
                break;
            }
        }
        if (alreadyExist)
        {
            // show warning
            EditorUtility.DisplayDialog("Can't add the same SVG file multiple times!",
                                        string.Format("The list of SVG assets already contains the {0} file.", newSvg.name),
                                        "Ok");
            return false;
        }
        else
        {
            if (this.SvgList.Count < SPRITES_SORTING_MAX_DOCUMENTS)
            {
                this.SvgList.Insert(index, new SVGAssetInput(newSvg, false));
                // recalculate atlas hash
                this.UpdateAtlasHash();
                return true;
            }
            else
            {
                // show warning
                EditorUtility.DisplayDialog("Can't add the SVG file, slots full!",
                                            string.Format("SVG list cannot exceed its maximum capacity of {0} entries. Try to merge some SVG files.", SPRITES_SORTING_MAX_DOCUMENTS),
                                            "Ok");

                return false;
            }
        }
    }

    public bool InputAssetAdd(TextAsset newSvg)
    {
        return this.InputAssetAdd(newSvg, this.SvgList.Count);
    }

    public bool InputAssetRemove(int index)
    {
        if (index >= 0 && index < this.SvgList.Count)
        {
            this.SvgList.RemoveAt(index);
            // recalculate atlas hash
            this.UpdateAtlasHash();
            return true;
        }
        return false;
    }

    public bool InputAssetMove(SVGAssetInput svgAsset, int toIndex)
    {
        int fromIndex = this.SvgIndexGet(svgAsset.TxtAsset);

        if (fromIndex >= 0)
        {
            // clamp the destination index
            if (toIndex < 0)
                toIndex = 0;
            if (toIndex > this.SvgList.Count)
                toIndex = this.SvgList.Count;
            // check if movement has sense
            if (fromIndex != toIndex)
            {
                // perform the real movement
                this.SvgList.Insert(toIndex, this.SvgList[fromIndex]);
                if (toIndex <= fromIndex)
                    ++fromIndex;
                this.SvgList.RemoveAt(fromIndex);
                // recalculate sorting orders of instantiated sprites
                this.SortingOrdersUpdateSvgIndex();
                return true;
            }
        }
        return false;
    }

    public void InputAssetSeparateGroupsSet(SVGAssetInput svgAsset, bool separateGroups)
    {
        if (svgAsset != null && svgAsset.SeparateGroups != separateGroups)
        {
            svgAsset.SeparateGroups = separateGroups;
            // recalculate atlas hash
            this.UpdateAtlasHash();
        }
    }

    private static string MD5Calc(string input)
    {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hash = SVGAtlas.m_Md5.ComputeHash(inputBytes);
        
        // step 2, convert byte array to hex string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
            sb.Append(hash[i].ToString("X2"));
        return sb.ToString();
    }

    // return true if the atlas needs an update (i.e. a call to UpdateSprites), else false
    private string CalcAtlasHash()
    {
        int count = this.SvgList.Count;

        if (count > 0)
        {
            // we want the parameters string to come alwas in front (when sorted)
            string paramsStr = "#*";
            string[] hashList = new string[count + 1];
            // parameters string
            paramsStr += this.m_ReferenceWidth + "-";
            paramsStr += this.m_ReferenceHeight + "-";
            paramsStr += this.m_DeviceTestWidth + "-";
            paramsStr += this.m_DeviceTestHeight + "-";
            paramsStr += this.m_OffsetScale + "-";
            paramsStr += this.m_Pow2Textures + "-";
            paramsStr += this.m_MaxTexturesDimension + "-";
            paramsStr += this.m_SpritesBorder + "-";
            paramsStr += this.m_ClearColor.ToString();
			paramsStr += this.m_OutputFolder;
            hashList[0] = paramsStr;
            // for each input SVG row we define an "id string"
            for (int i = 0; i < count; ++i)
                hashList[i + 1] = this.SvgList[i].Hash();
            // sort strings, so we can be SVG rows order independent
            Array.Sort(hashList);
            // return MD5 hash
            return SVGAtlas.MD5Calc(String.Join("-", hashList));
        }
        return "";
    }

    // recalculate atlas hash
    private void UpdateAtlasHash()
    {
        this.m_SvgListHashCurrent = this.CalcAtlasHash();
    }

    public bool NeedsUpdate()
    {
        // if current hash is different than the last valid one, atlas generator needs to be updated
        return (this.m_SvgListHashCurrent != this.m_SvgListHashOld) ? true : false;
    }

#endif

        // Calculate the scale factor that would be used to generate sprites if the screen would have the specified dimensions
    public float ScaleFactorCalc(int currentScreenWidth, int currentScreenHeight)
	{
		return SVGRuntimeGenerator.ScaleFactorCalc((float)this.m_ReferenceWidth, (float)this.m_ReferenceHeight, currentScreenWidth, currentScreenHeight, this.m_ScaleType, this.m_OffsetScale);
	}

	// Generate a sprite set, according to specified screen dimensions
	public bool GenerateSprites(int currentScreenWidth, int currentScreenHeight, List<Texture2D> textures, List<KeyValuePair<SVGSpriteRef, SVGSpriteData>> sprites)
	{
		float scale = this.ScaleFactorCalc(currentScreenWidth, currentScreenHeight);
		if (textures != null && sprites != null && scale > 0)
			return (SVGRuntimeGenerator.GenerateSprites(// input
			                                            this.SvgList, this.m_MaxTexturesDimension, this.m_SpritesBorder, this.m_Pow2Textures, scale, this.m_ClearColor, this.GeneratedSpritesFiles,
			                                            // output)
			                                            textures, sprites,
			                                            // here we are not interested in original locations
			                                            null));
		return false;
	}

	// return true if case of success, else false
	private bool UpdateRuntimeSprites(float newScale)
	{
		if (this.m_RuntimeTextures == null)
			this.m_RuntimeTextures = new List<Texture2D>();
		else
			this.m_RuntimeTextures.Clear();
		
		if (this.m_RuntimeSprites == null)
			this.m_RuntimeSprites = new SVGSpritesDictionary();
		else
			this.m_RuntimeSprites.Clear();
		
		List<KeyValuePair<SVGSpriteRef, SVGSpriteData>> sprites = new List<KeyValuePair<SVGSpriteRef, SVGSpriteData>>();
		
		if (SVGRuntimeGenerator.GenerateSprites(// input
		                                        this.SvgList, this.m_MaxTexturesDimension, this.m_SpritesBorder, this.m_Pow2Textures, newScale, this.m_ClearColor, this.GeneratedSpritesFiles,
		                                        // output)
		                                        this.m_RuntimeTextures, sprites,
		                                        // here we are not interested in original locations
		                                        null))
		{
			// create the runtime sprites dictionary
			foreach (KeyValuePair<SVGSpriteRef, SVGSpriteData> data in sprites)
				this.m_RuntimeSprites.Add(data.Key, new SVGSpriteAssetFile("", data.Key, data.Value));
			this.m_RuntimeGenerationScale = newScale;
			return true;
		}
		else
			return false;
	}

	// return true if case of success, else false
	public bool UpdateRuntimeSprites(int currentScreenWidth, int currentScreenHeight, out float scale)
	{
		float newScale = SVGRuntimeGenerator.ScaleFactorCalc((float)this.m_ReferenceWidth, (float)this.m_ReferenceHeight, currentScreenWidth, currentScreenHeight, this.m_ScaleType, this.m_OffsetScale);
		
		if (Math.Abs(this.m_RuntimeGenerationScale - newScale) > Single.Epsilon)
		{
			scale = newScale;
			return this.UpdateRuntimeSprites(newScale);
		}
		else
			scale = this.m_RuntimeGenerationScale;

		return true;
	}

	public SVGRuntimeSprite GetRuntimeSprite(SVGSpriteRef spriteRef)
	{
		if (this.m_RuntimeGenerationScale > 0)
		{
			SVGSpriteAssetFile spriteAsset;
			// get the requested sprite
			if (this.m_RuntimeSprites.TryGetValue(spriteRef, out spriteAsset))
				return new SVGRuntimeSprite(spriteAsset.SpriteData.Sprite, this.m_RuntimeGenerationScale, spriteRef);
		}
		return null;
	}

	public SVGRuntimeSprite GetSpriteByName(string spriteName)
	{
		SVGSpritesDictionary spritesSet = Application.isPlaying ? this.m_RuntimeSprites : this.GeneratedSpritesFiles;

		if (spritesSet != null)
		{
			// slow linear search
			foreach (SVGSpriteAssetFile spriteAsset in spritesSet.Values())
			{
				Sprite sprite = spriteAsset.SpriteData.Sprite;
				if (sprite.name == spriteName)
					return new SVGRuntimeSprite(sprite, Application.isPlaying ? this.m_RuntimeGenerationScale : this.EditorGenerationScale, spriteAsset.SpriteRef);
			}
		}
		return null;
	}

    void OnEnable()
    {
        if (this.SvgList == null)
            this.SvgList = new List<SVGAssetInput>();

		if (this.m_ReferenceWidth == 0)
			this.m_ReferenceWidth = (int)SVGAssets.ScreenResolutionWidth;
		
		if (this.m_ReferenceHeight == 0)
			this.m_ReferenceHeight = (int)SVGAssets.ScreenResolutionHeight;

    #if UNITY_EDITOR
        if (this.m_DeviceTestWidth == 0)
			this.m_DeviceTestWidth = this.m_ReferenceWidth;
        
        if (this.m_DeviceTestHeight == 0)
			this.m_DeviceTestHeight = this.m_ReferenceHeight;
    #endif

        // create the list of generated textures
        if (this.GeneratedTexturesFiles == null)
            this.GeneratedTexturesFiles = new List<AssetFile>();

        // create the list of generated sprites
        if (this.GeneratedSpritesFiles == null)
            this.GeneratedSpritesFiles = new SVGSpritesDictionary();

        // for each SVG text asset, a list of sprites (references) relative to the document
        if (this.GeneratedSpritesLists == null)
            this.GeneratedSpritesLists = new SVGSpritesListDictionary();

        // prepare structures for runtime generation
        this.m_RuntimeTextures = new List<Texture2D>();
        this.m_RuntimeSprites = new SVGSpritesDictionary();
        this.m_RuntimeGenerationScale = 0;
    }

    public int ReferenceWidth
    {
        get
        {
            return this.m_ReferenceWidth;
        }
        set
        {
            this.m_ReferenceWidth = value;
        #if UNITY_EDITOR
            this.UpdateAtlasHash();
        #endif
        }
    }

    public int ReferenceHeight
    {
        get
        {
            return this.m_ReferenceHeight;
        }
        set
        {
            this.m_ReferenceHeight = value;
        #if UNITY_EDITOR
            this.UpdateAtlasHash();
        #endif
        }
    }

#if UNITY_EDITOR
    public int DeviceTestWidth
    {
        get
        {
            return this.m_DeviceTestWidth;
        }
        set
        {
            this.m_DeviceTestWidth = value;
            this.UpdateAtlasHash();
        }
    }
    
    public int DeviceTestHeight
    {
        get
        {
            return this.m_DeviceTestHeight;
        }
        set
        {
            this.m_DeviceTestHeight = value;
            this.UpdateAtlasHash();
        }
    }
#endif

    public SVGScaleType ScaleType
    {
        get
        {
            return this.m_ScaleType;
        }
        set
        {
            this.m_ScaleType = value;
        #if UNITY_EDITOR
            this.UpdateAtlasHash();
        #endif
        }
    }

    public float OffsetScale
    {
        get
        {
            return this.m_OffsetScale;
        }
        set
        {
            this.m_OffsetScale = value;
        #if UNITY_EDITOR
            this.UpdateAtlasHash();
        #endif
        }
    }

    public bool Pow2Textures
    {
        get
        {
            return this.m_Pow2Textures;
        }
        set
        {
            this.m_Pow2Textures = value;
        #if UNITY_EDITOR
            this.UpdateAtlasHash();
        #endif
        }
    }

    public int MaxTexturesDimension
    {
        get
        {
            return this.m_MaxTexturesDimension;
        }
        set
        {
            this.m_MaxTexturesDimension = value;
        #if UNITY_EDITOR
            this.UpdateAtlasHash();
        #endif
        }
    }

    public int SpritesBorder
    {
        get
        {
            return this.m_SpritesBorder;
        }
        set
        {
            this.m_SpritesBorder = value;
        #if UNITY_EDITOR
            this.UpdateAtlasHash();
        #endif
        }
    }

    public Color ClearColor
    {
        get
        {
            return this.m_ClearColor;
        }
        set
        {
            this.m_ClearColor = value;
        #if UNITY_EDITOR
            this.UpdateAtlasHash();
        #endif
        }
    }

	public string OutputFolder
	{
		get
		{
			return this.m_OutputFolder;
		}
		set
		{
			this.m_OutputFolder = value;
		#if UNITY_EDITOR
			this.UpdateAtlasHash();
		#endif
		}
	}

    // Scale adaption
    [SerializeField]
    private int m_ReferenceWidth = 0;
    [SerializeField]
    private int m_ReferenceHeight = 0;
#if UNITY_EDITOR
    [SerializeField]
    private int m_DeviceTestWidth = 0;
    [SerializeField]
    private int m_DeviceTestHeight = 0;
#endif
    [SerializeField]
    private SVGScaleType m_ScaleType = SVGScaleType.MinDimension;
    [SerializeField]
    private float m_OffsetScale = 1;
    [SerializeField]
    private bool m_Pow2Textures = false;
    [SerializeField]
    // Maximum texture dimension
    private int m_MaxTexturesDimension = 2048;
    [SerializeField]
    // Border between each generated sprite and its neighbors, in pixels
    private int m_SpritesBorder = 1;
	// Clear color
    [SerializeField]
    private Color m_ClearColor = new Color(1, 1, 1, 0);
	// Output folder
	[SerializeField]
	private string m_OutputFolder = "";
    // List of SVG that we want to pack
    public List<SVGAssetInput> SvgList = null;
    // List of generated texture assets (files)
    public List<AssetFile> GeneratedTexturesFiles;
    // List of generated sprite assets (files)
    public SVGSpritesDictionary GeneratedSpritesFiles;
    // For each SVG text asset, a list of sprites (references) relative to the document
    public SVGSpritesListDictionary GeneratedSpritesLists;
    // The scale factor used from the last generation inside the editor
    public float EditorGenerationScale = 0;

    // Texture atlases generated at runtime
    [NonSerialized]
    private List<Texture2D> m_RuntimeTextures = null;
    // Sprites generated at runtime
    [NonSerialized]
    private SVGSpritesDictionary m_RuntimeSprites = null;
    // Scale factor relative to the last runtime sprite/texture generation
    [NonSerialized]
    private float m_RuntimeGenerationScale = 0;

#if UNITY_EDITOR
    // Hash relative to the last valid (i.e. updated) set of generated sprites
    [SerializeField]
    private string m_SvgListHashOld = "";
    // The hash relative to the current list of input SVG
    [SerializeField]
    private string m_SvgListHashCurrent = "";
    // The MD5 hash generator
    private static MD5 m_Md5 = System.Security.Cryptography.MD5.Create();
    // Used to flag atlas generators when building the scenes.
    [NonSerialized]
    public bool Exporting = false;
#endif

    // Scaling to map pixels in the image to world space units, used by all generated sprtes.
    public const float SPRITE_PIXELS_PER_UNIT = 100;
    // number of bits (sortingOrder) dedicated to SVG index
    public const int SPRITES_SORTING_DOCUMENTS_BITS = 4;
    // number of bits (sortingOrder) dedicated to instance index + zOrder
    public const int SPRITES_SORTING_INSTANCES_BITS = (15 - SPRITES_SORTING_DOCUMENTS_BITS);
    // Isolate the sortingOrder high part
    public const int SPRITES_SORTING_DOCUMENTS_MASK = (((1 << SPRITES_SORTING_DOCUMENTS_BITS) - 1) << SPRITES_SORTING_INSTANCES_BITS);
    // Isolate the sortingOrder low part
    public const int SPRITES_SORTING_INSTANCES_MASK = ((1 << SPRITES_SORTING_INSTANCES_BITS) - 1);
    // Max number of SVG inputs/row
    public const int SPRITES_SORTING_MAX_DOCUMENTS = (1 << SPRITES_SORTING_DOCUMENTS_BITS);
    // Max number of sprites instances for each SVG document
    public const int SPRITES_SORTING_MAX_INSTANCES = (1 << SPRITES_SORTING_INSTANCES_BITS);
}
