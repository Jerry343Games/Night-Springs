using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIClueList : MonoBehaviour
{
    public GameObject clueItemPrefab; // 线索项的预制件
    public Transform gridParent;      // Grid布局的父物体（通常是一个含有 GridLayoutGroup 的 GameObject）
    public RectTransform targetPanel;
    
    private void Start()
    {
        // 初始化线索列表
        UpdateClueList();
    }

    // 更新线索列表显示
    public void UpdateClueList()
    {
        // 清除现有的子物体
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        // 获取当前持有的全部线索
        List<ClueData> clues = MyEventManager.Instance.allClues;

        // 为每个线索创建一个UI项
        foreach (ClueData clue in clues)
        {
            GameObject clueItem = Instantiate(clueItemPrefab, gridParent);
            UIClueItem uiClueItem = clueItem.GetComponent<UIClueItem>();
            clueItem.GetComponent<DraggableUI>().targetPanel = targetPanel;
            if (uiClueItem != null)
            {
                uiClueItem.SetClueData(clue);
            }
        }
    }
}
