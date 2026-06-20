using System;
using System.Collections.Generic;
using PerfectRandom.Sulfur.Core.Items;

[Serializable]
public class EnhancementDTO
{
    public string name;
    public List<ModifierDTO> modifiers;
}

[Serializable]
public class ModifierDTO
{
    public string modifierName;
    public string statModType;
    public float value;
}