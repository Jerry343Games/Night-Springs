using System.Collections;
using System.Collections.Generic;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIClueItem : MonoBehaviour
{
    public ClueData clueData;     // 分配的线索数据
    public TMP_Text clueText;         // 用于显示线索文本的UI组件
    public Image clueIcon;        // 如果有图标，显示线索图标的UI组件
    public TMP_Text illustrationText;
    public Color locationColor;
    public void SetClueData(ClueData data)
    {
        clueData = data;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (clueData != null)
        {
            // 更新显示文本
            if (clueText != null)
            {
                clueText.text = clueData.displayText;
                illustrationText.text = clueData.illustration;
            }

            // 更新图标
            if (clueData.clueType==ClueType.Location)
            {
                clueIcon.sprite = Resources.Load<Sprite>("Sprites/Shop_tab_picked");
                clueText.rectTransform.anchoredPosition=Vector2.zero;
                clueText.color = locationColor;
            }
            else
            {
                clueIcon.sprite = Resources.Load<Sprite>("Sprites/Skill_paper2");
            }
            
        }
    }
    
}
