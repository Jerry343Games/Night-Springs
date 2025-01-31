using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyEventManager : MonoBehaviour
{
    // 静态实例，用于全局访问
    public static MyEventManager Instance { get; private set; }
    
    // 当前持有的全部线索
    public List<ClueData> allClues = new List<ClueData>();

    // 本次选择的线索
    public  List<ClueData> selectedClues = new List<ClueData>();

    public bool isDisplayDialogue;

    public StoryType myStoryType=StoryType.FirstEnterGame;
    

    // 确保在 Awake 中正确设置实例
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Initialize()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            SceneManager.LoadScene("StoryWriting");
        }
    }

    /// <summary>
    /// 创建一段对话
    /// </summary>
    /// <param name="textData">对话数据</param>
    public GameObject CreatDialogue(TextData textData)
    {
        GameObject dialogue = Instantiate(Resources.Load<GameObject>("Dialogue"));
        DialogueDisplay dialogueDisplay = dialogue.GetComponent<DialogueDisplay>();
        dialogueDisplay.textData = textData;
        dialogueDisplay.StartDialogue();
        return dialogue;
    }
    
    // 添加线索到持有列表
    public void AddClue(ClueData clue)
    {
        if (!allClues.Contains(clue))
        {
            allClues.Add(clue);
            // 这里可以添加逻辑，例如通知UI更新
            // 如果需要自动刷新UI，可以触发事件或调用方法
        }
    }

    // 从持有列表中移除线索（如果需要）
    public void RemoveClue(ClueData clue)
    {
        if (allClues.Contains(clue))
        {
            allClues.Remove(clue);
            // 更新UI等
        }
    }

    // 添加线索到 selectedClues
    public void SelectClue(ClueData clue)
    {
        if (!selectedClues.Contains(clue))
        {
            selectedClues.Add(clue);
        }
    }

    // 从 selectedClues 中移除线索
    public void DeselectClue(ClueData clue)
    {
        if (selectedClues.Contains(clue))
        {
            selectedClues.Remove(clue);
        }
    }
    
    public bool HasClue(ClueName name)
    {
        // 使用 foreach 循环遍历 selectedClues 列表
        foreach (ClueData clue in selectedClues)
        {
            if (clue.clueName == name)
            {
                return true; // 找到匹配的线索，返回 true
            }
        }
        return false; // 未找到匹配的线索，返回 false
    }
    
    
    public bool HasClueType(ClueType type)
    {
        foreach (ClueData clue in selectedClues)
        {
            if (clue.clueType == type)
            {
                return true; // 找到匹配的线索类型，返回 true
            }
        }
        return false; // 未找到匹配的线索类型，返回 false
    }
    
    public void LoadSceneByStory(StoryType storyType)
    {
        switch (storyType)
        {
            case StoryType.FirstArriveSubway:
                SceneManager.LoadScene("Subway_FirstArrive");
                break;
            case StoryType.FirstArriveSewer:
                SceneManager.LoadScene("Sewer_FirstArrive");
                break;
            case StoryType.FirstArriveBlock:
                SceneManager.LoadScene("Block_FirstArrive");
                break;
            case StoryType.GoFindFirstMark:
                SceneManager.LoadScene("Valley_FirstArrive");
                break;
            case StoryType.FirstEnterGame:
                SceneManager.LoadScene("Valley_EnterGame");
                break;
            case StoryType.RecreateTheEvent:
                SceneManager.LoadScene("Valley_DuringEvent");
                break;
            case StoryType.NameLeadToCamp:
                SceneManager.LoadScene("Vally_Camp");
                break;
            case StoryType.ManuscriptLeadToNewMark:
                SceneManager.LoadScene("Valley_Manuscript");
                break;
            case StoryType.MarkLeadToSewer:
                SceneManager.LoadScene("Block_FindSewer");
                break;
            case StoryType.GoCheckHouseNumber:
                SceneManager.LoadScene("Block_HouseNumber");
                break;
            case StoryType.CrowdsShowNewPlan:
                SceneManager.LoadScene("Subway_Darkness");
                break;
            case StoryType.TheLastBattle:
                SceneManager.LoadScene("Sewer_LastBattle");
                break;
        }
    }
}
