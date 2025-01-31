using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextData", menuName = "ScriptableObjects/TextData", order = 1)]
public class TextData : ScriptableObject
{
    [Header("Full Text")]
    [TextArea(5, 10)]
    public string fullText;

    [Header("Split Mark")]
    public char delimiter = '|';

    [Header("Duration")]
    public float displayDuration = 2f;
}
