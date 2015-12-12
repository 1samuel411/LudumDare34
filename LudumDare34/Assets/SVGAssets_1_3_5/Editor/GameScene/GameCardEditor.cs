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

[ CustomEditor(typeof(GameCardBehaviour)) ]
public class GameCardEditor : Editor
{
	private void DrawInspector(GameCardBehaviour card)
	{
		bool needSpriteUpdate = false;
		bool active = EditorGUILayout.Toggle(new GUIContent("Active", ""), card.Active);
		bool backSide = EditorGUILayout.Toggle(new GUIContent("Back side", ""), card.BackSide);
		CardType animalType = (CardType)EditorGUILayout.EnumPopup("Animal type", card.AnimalType);
		int index = EditorGUILayout.IntField("Index in deck", card.Index);
		GameBehaviour game = EditorGUILayout.ObjectField("Game manager", card.Game, typeof(GameBehaviour), true) as GameBehaviour;

		// update active flag, if needed
		if (active != card.Active)
		{
			card.Active = active;
			EditorUtility.SetDirty(card);
			//needUpdate = true;
			if (card.Game != null)
			{
				if (card.Active)
					card.Game.ShowCard(card);
				else
					card.Game.HideCard(card);
			}
		}
		// update back side flag, if needed
		if (backSide != card.BackSide)
		{
			card.BackSide = backSide;
			EditorUtility.SetDirty(card);
			needSpriteUpdate = true;
		}
		// update animal/card type, if needed
		if (animalType != card.AnimalType)
		{
			card.AnimalType = animalType;
			EditorUtility.SetDirty(card);
			needSpriteUpdate = true;
		}
		// update index in deck, if needed
		if (index != card.Index)
		{
			card.Index = index;
			EditorUtility.SetDirty(card);
		}
		// update game manager, if needed
		if (game != card.Game)
		{
			card.Game = game;
			EditorUtility.SetDirty(card);
		}

		if (needSpriteUpdate && card.Game != null)
			card.Game.UpdateCardSprite(card);
	}

	public override void OnInspectorGUI()
	{
		// get the target object
		GameCardBehaviour card = target as GameCardBehaviour;
		
		if (card != null)
			this.DrawInspector(card);
	}
}
