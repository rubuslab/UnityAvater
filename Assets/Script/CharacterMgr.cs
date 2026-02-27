using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterMgr  {

    private int characterIndex = 0;
    private Dictionary<int, Character> characters = new Dictionary<int, Character>();

	public CharacterMgr() { }

	public Character Generatecharacter (string skeleton, string weapon, string head, string chest, string hand, string feet, bool combine = false)
	{
		Character instance = new Character (characterIndex,skeleton,weapon,head,chest,hand,feet,combine);
		characters.Add(characterIndex, instance);
		characterIndex ++;

		return instance;
	}

	public void Removecharacter (CharacterController character)
	{
	}

	public void Update () {

		foreach(Character character in characters.Values)
		{
			character.Update();
		}
	}
}
