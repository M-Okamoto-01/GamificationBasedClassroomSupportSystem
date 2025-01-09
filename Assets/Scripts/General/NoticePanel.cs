using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Unity.Loading;

public enum NoticeType
{
    OK,
    NO,
    Loding
}

public class NoticePanel : MonoBehaviour
{
    private GameObject DTPanel;
    private GameObject FlagEvent;
    private GameObject NoticeImageObj;
    private GameObject NoticeTextObj;
    private GameObject LodingImageObj;
    public Sprite OKSprite;
    public Sprite NOSprite;
    public Sprite NoneSprite;

    void Start()
    {
        //自分の子供のオブジェクトをDTPanelに格納
        DTPanel = transform.GetChild(0).gameObject;
        //DTPanelの子供のオブジェクトをFlagEventに格納
        FlagEvent = DTPanel.transform.GetChild(0).gameObject;
        //FlagEventの子供のオブジェクトでImageを持つものをNoticeImageObjに格納
        //FlagEventの子供のオブジェクトでTextを持つものをNoticeTextObjに格納
        foreach (Transform child in FlagEvent.transform)
        {
            if (child.gameObject.GetComponent<Image>() != null)
            {
                //LoadingImageは別で格納
                if (child.gameObject.name == "LodingImage")
                {
                    LodingImageObj = child.gameObject;
                }
                else
                {
                    NoticeImageObj = child.gameObject;
                }
            }
            if (child.gameObject.GetComponent<TMP_Text>() != null)
            {
                NoticeTextObj = child.gameObject;
            }
        }

        DTPanel.SetActive(false);
        FlagEvent.SetActive(false);
        NoticeImageObj.SetActive(false);
        NoticeTextObj.SetActive(false);
        LodingImageObj.SetActive(false);
        
    }

    //フラグイベントを表示する
    public void ShowFlag(NoticeType type,string InfoText,bool AutoEnd)
    {
        //使うものを表示する
        DTPanel.SetActive(true);
        FlagEvent.SetActive(true);
        NoticeTextObj.SetActive(true);
        //DTPanel以下をFadeInする
        DTPanel.GetComponent<CanvasGroup>().alpha = 0.0f;
        DTPanel.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f);
        switch (type)
        {
            case NoticeType.OK:
                LodingImageObj.SetActive(false);
                NoticeImageObj.SetActive(true);
                NoticeImageObj.GetComponent<Image>().sprite = OKSprite;
                break;
            case NoticeType.NO:
                LodingImageObj.SetActive(false);
                NoticeImageObj.SetActive(true);
                NoticeImageObj.GetComponent<Image>().sprite = NOSprite;
                break;
            case NoticeType.Loding:
                LodingImageObj.SetActive(true);
                NoticeImageObj.SetActive(false);
                LodingImageObj.GetComponent<Animation>().wrapMode = WrapMode.Loop;
                //Animationをはじめから再生する
                LodingImageObj.GetComponent<Animation>().Stop();
                LodingImageObj.GetComponent<Animation>().Play();
                break;
        }
        NoticeTextObj.GetComponent<TMP_Text>().text = InfoText;

        if (AutoEnd)
        {
            StartCoroutine(AutoEndCoroutine());
        }
    }

    IEnumerator AutoEndCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        HideFlag();
    }

    //フラグイベントを非表示にする
    public void HideFlag()
    {
        //DTPanel以下をFadeOutする
        DTPanel.GetComponent<CanvasGroup>().DOFade(0.0f, 0.5f);
        //DTPanel以下を非表示にする
        StartCoroutine(HideFlagCoroutine());
    }

    IEnumerator HideFlagCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        DTPanel.SetActive(false);
        FlagEvent.SetActive(false);
        NoticeImageObj.SetActive(false);
        NoticeTextObj.SetActive(false);
        LodingImageObj.SetActive(false);
    }




}
