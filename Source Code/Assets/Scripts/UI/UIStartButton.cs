using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIStartButton : MonoBehaviour
{
    private Button startButton;
    private AudioSource _audioSource;
    public Image frontMask;
    public CanvasGroup uiWritingPanel;
    public CanvasGroup uiInteractivePanel;
    private string _loadSceneName;
    void Start()
    {
        startButton = GetComponent<Button>();
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        _audioSource = GetComponent<AudioSource>();
    }

    void OnDestroy()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
        }
    }

    // 点击事件处理方法
    public void OnStartButtonClicked()
    {
        if (!MyEventManager.Instance.isDisplayDialogue)
        {
            _audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/ClickButton"));
            CheckSelectedCluesValidity();
        }
    }
    

    // 合法性检查方法
    private void CheckSelectedCluesValidity()
    {
        // 获取放入 StoryPanel 的线索列表
        List<ClueData> selectedClues = MyEventManager.Instance.selectedClues;
        bool hasValley = MyEventManager.Instance.HasClue(ClueName.Valley);
        bool hasBlock = MyEventManager.Instance.HasClue(ClueName.Block);
        bool hasSewers = MyEventManager.Instance.HasClue(ClueName.Sewers);
        bool hasSubway = MyEventManager.Instance.HasClue(ClueName.Subway);
        bool hasName = MyEventManager.Instance.HasClue(ClueName.GeorgeTyndale);
        bool hasMark = MyEventManager.Instance.HasClue(ClueName.ReligiousMark);
        bool hasDarkness = MyEventManager.Instance.HasClue(ClueName.Darkness);
        bool hasGathering = MyEventManager.Instance.HasClue(ClueName.Gathering);
        bool hasManuscript = MyEventManager.Instance.HasClue(ClueName.VoynichManuscript);
        bool hasNewPath=MyEventManager.Instance.HasClue(ClueName.NewPath);
        bool hasHouseNumber=MyEventManager.Instance.HasClue(ClueName.HouseNumber);
        bool hasLocation = MyEventManager.Instance.HasClueType(ClueType.Location);
        
        
        if (selectedClues.Count==0)//如果没有选择
        {
            MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryEmpty"));
        }
        else if (hasLocation)//如果选中了场景
        {
            //只单选场景的四种情况，为每个场景的开场介绍对话
            if (selectedClues.Count==1 && hasValley)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryValleyStart"));
                _loadSceneName = "Valley";
                MyEventManager.Instance.myStoryType = StoryType.GoFindFirstMark;
                StartWriting();
            }
            else if (selectedClues.Count==1 && hasBlock)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryBlockStart"));
            }
            else if (selectedClues.Count==1 && hasSubway)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StorySubwayStart"));
                MyEventManager.Instance.myStoryType = StoryType.FirstArriveSubway;
                StartWriting();
            }
            else if (selectedClues.Count==1 && hasSewers)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StorySewerStart"));
                MyEventManager.Instance.myStoryType = StoryType.FirstArriveSewer;
                StartWriting();
            }
            
            //选中两个，为每个场景的二阶故事
            //发现图案
            else if (selectedClues.Count==2 && hasValley && hasMark)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryFindMark"));
                MyEventManager.Instance.myStoryType = StoryType.RecreateTheEvent;
                StartWriting();
            }
            
            //只选择人名和地点
            else if (selectedClues.Count==2 && hasName && hasValley)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryOnlyHaveName"));
            }
            //只选幽谷和手稿，回去确认仪式完成情况
            else if (selectedClues.Count==2 && hasManuscript && hasValley)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryManuscriptBackToValley"));
                MyEventManager.Instance.myStoryType = StoryType.ManuscriptLeadToNewMark;
                StartWriting();
            }
            
            //只选市区和记号，显示不好找，需要缩小范围
            else if(selectedClues.Count==2 && hasBlock && hasMark)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryToLargeToFindMark"));
            }
            
            //有了名字 市区，可以定位场景
            else if(selectedClues.Count==2 && hasBlock && hasName)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryFindBolck"));
                MyEventManager.Instance.myStoryType = StoryType.FirstArriveBlock;
                _loadSceneName = "Block";
                StartWriting();
            }
            
            //找到门牌号
            else if(selectedClues.Count==2 && hasBlock && hasHouseNumber)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryFindHouseInBlock"));
                MyEventManager.Instance.myStoryType = StoryType.GoCheckHouseNumber;
                StartWriting();
            }
            
            else if(selectedClues.Count==2 && hasSubway && hasGathering)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryCrowdPeople"));
                MyEventManager.Instance.myStoryType = StoryType.CrowdsShowNewPlan;
                StartWriting();
            }
            //错误的标记
            else if ( hasSewers && hasMark)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryNoOldMark"));
            }
            //该去哪找标记
            else if (selectedClues.Count==2 && hasSewers && hasNewPath)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryNoDarkness"));
            }
            //会迷路 需要标记
            else if (selectedClues.Count==2 && hasSewers && hasDarkness)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryNoNewPath"));
            }

            //有了名字 市区 印记，可以定位下水道
            else if(selectedClues.Count==3 && hasBlock && hasName && hasMark)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryFindSewerInBlock"));
                MyEventManager.Instance.myStoryType = StoryType.MarkLeadToSewer;
                StartWriting();
            }
            //同时选择 幽谷 教团标记 人名，揭示廷代尔失踪期间
            else if (selectedClues.Count==3 && hasName && hasValley && hasMark)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryVallyCamp"));
                MyEventManager.Instance.myStoryType = StoryType.NameLeadToCamp;
                StartWriting();
            }
            //最终
            else if (selectedClues.Count==3 && hasNewPath && hasSewers && hasDarkness)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryTheFinalSewer"));
                MyEventManager.Instance.myStoryType = StoryType.TheLastBattle;
                StartWriting();
            }
            //过多
            else if(selectedClues.Count==4)
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryTooMuchClues"));
            }
            //其他
            else
            {
                MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/DefaultError"));
            }
        }
        else if(!hasLocation)//如果没选中场景
        {
            MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/StoryNeedLocation"));
        }
        else
        {
            MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/DefaultError"));
        }
    }

    private bool _isStartWriting=false;
    /// <summary>
    /// 条件符合 开始写作
    /// </summary>
    private void StartWriting()
    {
        Sequence q = DOTween.Sequence();
        MyEventManager.Instance.selectedClues.Clear();
        q.AppendInterval(1f).OnComplete(() => _isStartWriting = true);
    }

    private void Update()
    {
        if (!MyEventManager.Instance.isDisplayDialogue&&_isStartWriting)
        {
            _audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/Writing"));
            _isStartWriting = false;
            Sequence q = DOTween.Sequence();
            q.Append(frontMask.DOFade(1, 0.5f).OnComplete(() =>
            {
                uiInteractivePanel.alpha = 0;
            }));
            q.AppendInterval(2.5f).OnComplete(() =>
            {
                MyEventManager.Instance.LoadSceneByStory(MyEventManager.Instance.myStoryType);
            });
            
        }
    }
    
}
