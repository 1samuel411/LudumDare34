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
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif

public enum CardType {
	Undefined  =  -2,
	BackSide   =  -1,
	Panda      =   0,
	Monkey     =   1,
	Orangutan  =   2,
	Panther    =   3,
	Puma       =   4,
	Leopard    =   5,
	Lion       =   6,
	Cougar     =   7,
	Tiger      =   8,
	Elephant   =   9,
	Penguin    =  10,
	Zebra      =  11,
	Hen        =  12,
	Rooster    =  13,
	Pig        =  14,
	Dog        =  15,
	Rabbit     =  16,
	Owl        =  17,
	Sheep      =  18,
	Cat        =  19,
	Deer       =  20,
	Donkey     =  21,
	Cow        =  22,
	Fox        =  23
};

[ExecuteInEditMode]
public class GameCardBehaviour : MonoBehaviour {

	public static string AnimalSpriteName(CardType animalType)
	{
		switch (animalType)
		{
			case CardType.BackSide:
				return("animals_back");
			case CardType.Panda:
				return("animals_Panda");
			case CardType.Monkey:
				return("animals_Monkey");
			case CardType.Orangutan:
				return("animals_Orangutan");
			case CardType.Panther:
				return("animals_Panther");
			case CardType.Puma:
				return("animals_Puma");
			case CardType.Leopard:
				return("animals_Leopard");
			case CardType.Lion:
				return("animals_Lion");
			case CardType.Cougar:
				return("animals_Cougar");
			case CardType.Tiger:
				return("animals_Tiger");
			case CardType.Elephant:
				return("animals_Elephant");
			case CardType.Penguin:
				return("animals_Penguin");
			case CardType.Zebra:
				return("animals_Zebra");
			case CardType.Hen:
				return("animals_Hen");
			case CardType.Rooster:
				return("animals_Rooster");
			case CardType.Pig:
				return("animals_Pig");
			case CardType.Dog:
				return("animals_Dog");
			case CardType.Rabbit:
				return("animals_Rabbit");
			case CardType.Owl:
				return("animals_Owl");
			case CardType.Sheep:
				return("animals_Sheep");
			case CardType.Cat:
				return("animals_Cat");
			case CardType.Deer:
				return("animals_Deer");
			case CardType.Donkey:
				return("animals_Donkey");
			case CardType.Cow:
				return("animals_Cow");
			case CardType.Fox:
				return("animals_Fox");
			default:
				return("");
		}
	}

	void OnMouseDown()
	{
		if (this.Game != null)
			this.Game.SelectCard(this);
	}

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}

#if UNITY_EDITOR
	private bool RequirementsCheck()
	{
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
			this.Active = true;
			this.BackSide = true;
			this.AnimalType = CardType.Undefined;
			this.Index = 0;
		}
	}
#endif

	public bool Active;
	public bool BackSide;
	public CardType AnimalType;
	public int Index;
	public GameBehaviour Game;
}
