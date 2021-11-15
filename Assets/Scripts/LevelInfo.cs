using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level")]
public class LevelInfo : ScriptableObject
{
    public string itsName;
    public int rows;
    public int columns;

    public CellInfo[] cellVariations;
}
