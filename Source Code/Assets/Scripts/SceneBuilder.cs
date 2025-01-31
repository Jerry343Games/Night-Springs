using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Enums;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneBuilder : MonoBehaviour
{
    public StoryType storyType;

    public SpriteRenderer[] spriteRenderers;

    public GameObject afterAti;

    private Image mask;

    private void Awake()
    {
        mask = GameObject.FindWithTag("AutoMask").GetComponentInChildren<Image>();
    }

    private void OnDisable()
    {
        mask.DOKill();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject dia=null;
        if (other.CompareTag("Player"))
        {
            switch (storyType)
            {
                case StoryType.RecreateTheEvent:
                    foreach (var vSpriteRenderer in spriteRenderers)
                    {
                        vSpriteRenderer.DOFade(0, 1.5f);
                    }
                    break;
                case StoryType.FirstArriveBlock:
                    DialogueDisplay dialogueDisplay= Instantiate(Resources.Load<GameObject>("Dialogue_Achi")).GetComponent<DialogueDisplay>();
                    dialogueDisplay.textData = Resources.Load<TextData>("DialogueSO/Level/AtiSays");
                    dialogueDisplay.StartDialogue();
                    dialogueDisplay.OnComplete(() =>
                    {
                        MyEventManager.Instance.CreatDialogue(
                            Resources.Load<TextData>("DialogueSO/Level/ThinkAtiWords"));
                        MyEventManager.Instance.AddClue(Resources.Load<ClueData>("ClueSO/Subway"));
                        SceneManager.LoadScene("StoryWriting");
                    });
                    break;
                case StoryType.GoCheckHouseNumber:
                    mask = GameObject.FindWithTag("AutoMask").GetComponentInChildren<Image>();
                    mask.DOFade(1, 1f).OnComplete(()=>
                    {
                        dia = MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/Level/SearchTheHouse"));
                        dia.GetComponent<DialogueDisplay>().OnComplete(() =>
                        {
                            MyEventManager.Instance.AddClue(Resources.Load<ClueData>("ClueSO/Manuscript"));
                            SceneManager.LoadScene("StoryWriting");
                        });
                    });
                    break;
                case StoryType.CrowdsShowNewPlan:
                    dia = MyEventManager.Instance.CreatDialogue(
                        Resources.Load<TextData>("DialogueSO/Level/FindDarkness"));
                    dia.GetComponent<DialogueDisplay>().OnComplete(() =>
                    {
                        MyEventManager.Instance.AddClue(Resources.Load<ClueData>("ClueSO/Darkness"));
                        SceneManager.LoadScene("StoryWriting");
                    });
                    break;
                case StoryType.TheLastBattle:
                    mask = GameObject.FindWithTag("AutoMask").GetComponentInChildren<Image>();
                    mask.DOFade(1, 1f).OnComplete(()=>
                    {
                        dia = MyEventManager.Instance.CreatDialogue(Resources.Load<TextData>("DialogueSO/Level/LastBattle"));
                        dia.GetComponent<DialogueDisplay>().OnComplete(() =>
                        {
                            SceneManager.LoadScene("EnterPage");
                        });
                    });
                    break;
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
