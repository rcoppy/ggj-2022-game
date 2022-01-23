using UnityEngine;
using System.Collections.Generic;
using System;

namespace GGJ2022.Dialogue.Schema
{
    [Serializable]
    public class CharacterBank : ScriptableObject
    {
        List<Character> _characters;

        public void Initialize(List<Character> characters)
        {
            _characters = characters; 
        }
    }
}
    

