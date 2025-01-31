using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class DialogueDisplay : MonoBehaviour
{
    [HideInInspector]
    public TextData textData;

    public GameObject uiTextBox;
    public TMP_Text uiText;
    public GameObject uiHeadImg;

    private string[] _sentences;
    private int _currentSentenceIndex = 0;
    private bool _isSkipping = false;
    private bool _isDisplaying = false;
    private RectTransform _uiTextBoxRectTransform;
    private RectTransform _uiHeadImgRectTransform;

    private AudioSource _audioSource;
    
    private Action onComplete;
    
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _uiTextBoxRectTransform = uiTextBox.GetComponent<RectTransform>();
        _uiHeadImgRectTransform = uiHeadImg.GetComponent<RectTransform>();
        _uiTextBoxRectTransform.localScale = Vector3.zero;
        _uiHeadImgRectTransform.GetComponent<RectTransform>().localScale = Vector3.zero;
    }
    

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isSkipping = true;
        }
    }

    // 公共方法，用于开始显示对话
    public void StartDialogue(TextData newTextData = null)
    {
        if (_isDisplaying)
            return;

        if (newTextData != null)
            textData = newTextData;

        _sentences = textData.fullText.Split(textData.delimiter);
        _currentSentenceIndex = 0;
        _isDisplaying = true;
        _uiHeadImgRectTransform.DOScale(1, 0.3f).OnComplete(() =>
        {
            StartCoroutine(DisplaySentences());
        });
    }
    
    public DialogueDisplay OnComplete(Action callback)
    {
        onComplete = callback;
        return this;
    }

    /// <summary>
    /// 文本框相关动画
    /// </summary>
    /// <returns></returns>
    IEnumerator DisplaySentences()
    {
        while (_currentSentenceIndex < _sentences.Length-1)
        {
            _audioSource.PlayOneShot(Resources.Load<AudioClip>("Audio/DialogueSwitch"));
            MyEventManager.Instance.isDisplayDialogue = true;
            uiText.text = "";
            _uiTextBoxRectTransform.DOScale(1, 0.25f);
            uiText.DOText(_sentences[_currentSentenceIndex],0.5f,true,ScrambleMode.None);
            float timer = 0f;

            while (timer < textData.displayDuration)
            {
                if (_isSkipping)
                {
                    _isSkipping = false;
                    uiText.DOKill();
                    _uiTextBoxRectTransform.DOKill();
                    break;
                }
                timer += Time.deltaTime;
                yield return null;
            }
            _currentSentenceIndex++;
            _uiTextBoxRectTransform.DOScale(0, 0.1f);
            yield return new WaitForSeconds(0.1f);
        }
        
        uiText.text = ""; // 清空文本
        _isDisplaying = false;
        uiText.DOKill();
        _uiTextBoxRectTransform.DOKill();
        
        
        ExitDialogue();
    }

    /// <summary>
    /// 退出文本程序
    /// </summary>
    private void ExitDialogue()
    {
        _uiHeadImgRectTransform.DOScale(0, 0.2f).OnComplete(() =>
        {
            MyEventManager.Instance.isDisplayDialogue = false;
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }
}
