using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DisplayText : MonoBehaviour
{
    private TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        text= GetComponent<TMP_Text>();
        text.DOText("THE MANUSCRIPT HAS BEEN ALTERED",0.5f);
    }
}
