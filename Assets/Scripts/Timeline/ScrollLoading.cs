using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollLoading : MonoBehaviour
{
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private GameObject loadIcon;
    [SerializeField]
    private TimelineMaster timelineMaster;

    private GameObject LogObject;

    private bool listReloadFlag = true; //リロード可能フラグ
    private int scrollViewReloadHeight = -100; //ScrollViewがこのポジション以下まで下がったら更新開始 ReloadPanelの高さに準ずる

    private ScrollRect scrollViewScrollRect;
    private RectTransform scrollViewRectTransform;
    private RectTransform contentRectTransform;

    void Start () {
        scrollViewScrollRect = GetComponent<ScrollRect> ();
        scrollViewRectTransform = GetComponent<RectTransform> ();
        contentRectTransform = content.GetComponent<RectTransform> ();
        listReloadFlag = true;
        scrollViewReloadHeight = -100; 
        LogObject = GameObject.FindGameObjectWithTag("LogGameObject");
    }

    void Update () {
        if (contentRectTransform.anchoredPosition.y < 0.0f && scrollViewRectTransform.anchoredPosition.y > scrollViewReloadHeight) {
            //Debug.Log("Normal");
        } else if(contentRectTransform.anchoredPosition.y <= scrollViewReloadHeight) {
            //reloadしてもいい状態か確認
            if (listReloadFlag) {
                listReloadFlag = false;
                //リスト取得通信(デモなので実際の通信はしないよ)
                StartCoroutine (PullBackScrollView ());
            }
        }
    }

    IEnumerator PullBackScrollView(){
        //とりあえず２秒クルクルさせる
        LogObject.GetComponent<LogObject>().AddLogData("ReloadTimeline", "");
        loadIcon.SetActive(true);
        loadIcon.GetComponent<RectTransform>().DORotate(new Vector3(0,0,-720), 10f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        yield return StartCoroutine(timelineMaster.GetTimelineData());
        loadIcon.SetActive(false);
        //ローディングから矢印に切り替え
        yield return new WaitForSeconds(5f);
        loadIcon.SetActive(true);
        listReloadFlag = true;

        yield break;
    }
}
