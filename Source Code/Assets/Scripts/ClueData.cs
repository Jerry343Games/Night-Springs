using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

[CreateAssetMenu(fileName = "NewClueData", menuName = "ScriptableObjects/ClueData")]
public class ClueData : ScriptableObject
{
    public ClueName clueName;    // 线索名称（唯一枚举）
    public ClueType clueType;    // 线索类型（发现或地点）

    public string displayText;
    [TextArea]
    public string illustration;
}
