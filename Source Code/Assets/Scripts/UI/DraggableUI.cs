using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 initialPosition;
    private Transform initialParent;
    private int initialSiblingIndex;
    public RectTransform targetPanel; // 目标Panel
    private Canvas canvas;
    private bool isInStoryPanel;
    
    private AudioSource _audioSource;
    
    void Start()
    {
        initialPosition = transform.position;
        initialParent = transform.parent;
        initialSiblingIndex = transform.GetSiblingIndex();
        canvas = GetComponentInParent<Canvas>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!MyEventManager.Instance.isDisplayDialogue)
        {
            // 保存初始位置、父对象和层级索引
            //initialPosition = transform.position;
            //initialParent = transform.parent;
            initialSiblingIndex = transform.GetSiblingIndex();
            PlayPickAudio();
            // 将UI元素的父对象临时设置为Canvas，以避免被GridLayoutGroup控制
            transform.SetParent(canvas.transform, true); // true表示保持世界坐标不变
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!MyEventManager.Instance.isDisplayDialogue)
        {
            // 更新UI元素的位置，使其跟随鼠标
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out localPoint);
            transform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!MyEventManager.Instance.isDisplayDialogue)
        {
            PlayDropAudio();
            // 检查线索是否被放入 StoryPanel
            if (RectTransformUtility.RectangleContainsScreenPoint(
                    targetPanel,
                    eventData.position,
                    canvas.worldCamera))
            {
                // 将父对象设置为 StoryPanel
                transform.SetParent(targetPanel, true);
                // 如果之前不在 StoryPanel 中，则添加到 selectedClues
                if (!isInStoryPanel)
                {
                    UIClueItem clueItem = GetComponent<UIClueItem>();
                    if (clueItem != null && clueItem.clueData != null)
                    {
                        MyEventManager.Instance.SelectClue(clueItem.clueData);
                    }

                    isInStoryPanel = true;
                }
            }
            else
            {
                // 返回初始父对象（线索列表）
                transform.SetParent(initialParent, false);
                transform.SetSiblingIndex(initialParent.childCount - 1);

                // 如果之前在 StoryPanel 中，则从 selectedClues 中移除
                if (isInStoryPanel)
                {
                    UIClueItem clueItem = GetComponent<UIClueItem>();
                    if (clueItem != null && clueItem.clueData != null)
                    {
                        MyEventManager.Instance.DeselectClue(clueItem.clueData);
                    }

                    isInStoryPanel = false;
                }

                // 如有需要，强制刷新布局
                LayoutRebuilder.ForceRebuildLayoutImmediate(initialParent as RectTransform);
            }
        }
    }

    private void PlayPickAudio()
    {
        _audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/PickNote"));
    }
    
    private void PlayDropAudio()
    {
        _audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/DropNote"));
    }
}
