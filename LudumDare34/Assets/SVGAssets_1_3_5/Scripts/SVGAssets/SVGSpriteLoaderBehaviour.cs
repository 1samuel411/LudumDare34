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
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class SVGSpriteLoaderBehaviour : MonoBehaviour
{
    private void FixLocalPosition(float deltaScale)
    {
        if (deltaScale > 0)
        {
            Vector3 newPos = this.transform.localPosition;
            newPos.x *= deltaScale;
            newPos.y *= deltaScale;
            this.transform.localPosition = newPos;
            this.m_OldPos = newPos;
            this.transform.hasChanged = false;
        }
    }

	private void UpdateSprite(int currentScreenWidth, int currentScreenHeight)
    {
        if (this.Atlas != null && this.SpriteReference != null)
        {
            SpriteRenderer renderer = this.gameObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
				float scale;
				if (this.Atlas.UpdateRuntimeSprites(currentScreenWidth, currentScreenHeight, out scale))
				{
					SVGRuntimeSprite newSprite = this.Atlas.GetRuntimeSprite(this.SpriteReference);
					if (newSprite != null)
					{
						renderer.sprite = newSprite.Sprite;
						// fix for the first time the sprite is regenerated at runtime
						if (this.m_RuntimeScale == 0)
							this.m_RuntimeScale = this.Atlas.EditorGenerationScale;
						// update local position
						this.FixLocalPosition(newSprite.GenerationScale / this.m_RuntimeScale);
						this.m_RuntimeScale = newSprite.GenerationScale;
					}
				}
            }
        }
    }

    public void UpdateSprite(bool updateChildren, int currentScreenWidth, int currentScreenHeight)
    {
        // regenerate (if needed) the sprite
        this.UpdateSprite(currentScreenWidth, currentScreenHeight);
        
        if (updateChildren)
        {
            int childCount = this.transform.childCount;
            // update children
            for (int i = 0; i < childCount; ++i)
            {
                GameObject gameObj = this.transform.GetChild(i).gameObject;
                SVGSpriteLoaderBehaviour loader = gameObj.GetComponent<SVGSpriteLoaderBehaviour>();
                if (loader != null)
                    loader.UpdateSprite(updateChildren, currentScreenWidth, currentScreenHeight);
            }
        }
    }

    void Start()
    {
        // keep track of the current position
        this.m_OldPos = this.transform.localPosition;
        // update/regenerate sprite, if requested
        if (this.ResizeOnStart)
			this.UpdateSprite((int)SVGAssets.ScreenResolutionWidth, (int)SVGAssets.ScreenResolutionHeight);
    }
    
    void LateUpdate()
    {
        // when position has changed (due to an animation keyframe) we must rescale respect to the original factor, not the last used one
		if (this.UpdateTransform && this.transform.hasChanged)
        {
            Vector3 newPos = this.transform.localPosition;
            if (newPos != this.m_OldPos)
                // fix the local position according to the original scale (i.e. the scale used to generate sprites from within the Unity editor)
                this.FixLocalPosition(this.m_RuntimeScale / this.Atlas.EditorGenerationScale);
        }
    }

#if UNITY_EDITOR
    private bool RequirementsCheck()
    {
        // get list of attached components
        Component[] components = gameObject.GetComponents(this.GetType());
        // check for duplicate components
        foreach (Component component in components)
        {
            if (component == this)
                continue;
            // show warning
            EditorUtility.DisplayDialog("Can't add the same component multiple times!",
                                        string.Format("The component {0} can't be added because {1} already contains the same component.", this.GetType(), gameObject.name),
                                        "Ok");
            // destroy the duplicate component
            DestroyImmediate(this);
        }

		SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
		if (renderer == null)
		{
			EditorUtility.DisplayDialog("Incompatible game object",
			                            string.Format("In order to work properly, the component {0} requires the presence of a SpriteRenderer component", this.GetType()),
			                            "Ok");
			return false;
		}
		return true;
    }

    // Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time.
    // This function is only called in editor mode. Reset is most commonly used to give good default values in the inspector.
    void Reset()
    {
		if (this.RequirementsCheck())
		{
        	this.Atlas = null;
        	this.SpriteReference = null;
        	this.ResizeOnStart = true;
			//this.FixTransformOnStart = true;
			this.UpdateTransform = true;
			SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
			renderer.sprite = null;
		}
    }
#endif

    // Atlas generator the sprite reference to
	public SVGAtlas Atlas;
    // Sprite reference
	public SVGSpriteRef SpriteReference;
    // True if sprite must be regenerated at Start, else false
    public bool ResizeOnStart;
    // True if we have to fix (local) position according to the (updated) sprite dimensions, else false
	public bool UpdateTransform;
    // Keep track of last (local) position
	[NonSerialized]
    private Vector3 m_OldPos;
    // Keep track of the scale used by last runtime generation
	[NonSerialized]
    private float m_RuntimeScale = 0;
}
