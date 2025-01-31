using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIAutoMask : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().color=Color.black;
        Invoke("FadeOutMask",0.5f);
    }

    private void FadeOutMask()
    {
        GetComponent<Image>().DOFade(0, 0.5f);
    }
}
