using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollPublishButton : MonoBehaviour
{
    public GameObject publishButton;
    private Vector2 PreVector2;

    // Start is called before the first frame update
    void Start()
    {
        //onValueChange()によってpublishButtonの表示・非表示を切り替える
        GetComponent<ScrollRect>().onValueChanged.AddListener(onValueChanged);
    }

    private void onValueChanged(Vector2 value)
    {
        //下向きにスクロールしていたらpublishButtonを表示
        if(value.y > 1)
        {
            publishButton.GetComponent<Image>().DOFade(1.0f, 0.3f);
        }
        else if (PreVector2.y < value.y)
        {
            publishButton.GetComponent<Image>().DOFade(1.0f, 0.3f);
        }
        else
        {
            publishButton.GetComponent<Image>().DOFade(0.5f, 0.3f);
        }
        PreVector2 = value;
    }
}
