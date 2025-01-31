using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterLevelManager : MonoBehaviour
{
    private bool _isStart;

    public Image mask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)&&!_isStart)
        {
            _isStart = true;
            StartGame();
        }
    }

    void StartGame()
    {
        mask.DOFade(1, 1f).OnComplete(()=>SceneManager.LoadScene("Valley_EnterGame"));
            
        Destroy(gameObject);
    }
}
