using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] 
    public TextData textData;

    public ClueData newClue;

    public ClueData newClue2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MyEventManager.Instance.CreatDialogue(textData);
            if (newClue!=null)
            {
                MyEventManager.Instance.AddClue(newClue);
            }
            if (newClue2!=null)
            {
                MyEventManager.Instance.AddClue(newClue2);
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
