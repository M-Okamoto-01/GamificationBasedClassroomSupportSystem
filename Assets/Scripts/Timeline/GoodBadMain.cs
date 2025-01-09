using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using AOT;
using System;
using Gilzoide.LottiePlayer;   

public class GoodBadMain : MonoBehaviour
{
    //GameObject
    public static GameObject GoodBadArea;
    public GameObject sadImage;
    public GameObject smileImage;
    public GameObject EBar;
    public static GameObject sadBar;
    public static GameObject smileBar;
    public GameObject TimelineMaster;
    public GameObject RealTimeEndPoint;
    //Sprite
    public Sprite sadSprite;
    public Sprite smileSprite;
    public Sprite sadSprite_color;
    public Sprite smileSprite_color;
    public GameObject SmileAnimation;
    public GameObject SadAnimation;

    private static GoodBadData[] goodBadDatas;

    void Start()
    {
        //初期化
        GoodBadArea = GameObject.FindGameObjectWithTag("GoodBadArea");
        sadBar = GameObject.FindGameObjectWithTag("sadBar");
        smileBar = GameObject.FindGameObjectWithTag("smileBar");
    }
    

    public void WebGL_Load(string test)
    {
        Debug.Log("WebGL_Load");
        //サイズを調節する
        UpdateGoodBadData(new GoodBadData("00000",test,1));
    }
    
    public void OnStartImage()
    {
        goodBadDatas = new GoodBadData[0];
        //サイズを調節する
        Image image = GoodBadArea.GetComponent<Image>();
        Canvas.ForceUpdateCanvases();
        float MainHeight = image.rectTransform.rect.height;
        float MainWidth = image.rectTransform.rect.width;
        //アイコンサイズの設定
        float IconSize = MainHeight * 0.7f;
        sadImage.GetComponent<RectTransform>().sizeDelta = new Vector2(IconSize,IconSize);
        smileImage.GetComponent<RectTransform>().sizeDelta = new Vector2(IconSize,IconSize);
        //アイコンの位置の設定
        float IconPosition = IconSize * 0.6f;
        sadImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(IconPosition,0,0);
        smileImage.GetComponent<RectTransform>().anchoredPosition = new Vector3(-IconPosition,0,0);
        //バーの位置の設定
        float BarHeight = MainHeight * 0.8f;
        float BarWidth = MainWidth * 0.7f;
        EBar.GetComponent<RectTransform>().sizeDelta = new Vector2(BarWidth,BarHeight);
        EBar.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
        sadBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0,BarHeight);
        sadBar.GetComponent<RectTransform>().localPosition = new Vector3(-BarWidth,0,0);
        smileBar.GetComponent<RectTransform>().sizeDelta = new Vector2(0,BarHeight);
        smileBar.GetComponent<RectTransform>().localPosition = new Vector3(BarWidth,0,0);
        StartCoroutine(GetGoodBadData());
    }

    private void SetSubscribe()
    {
        //初期化
        ASEndpoint aSEndpoint = AppSyncURL.GetASGoodBad(TimelineMaster.GetComponent<TimelineMaster>().RoomID);
        RealTimeEndPoint.GetComponent<AppSyncWebSocketManager>().WebGL_WebSocketSubscribe(aSEndpoint,TimelineMaster.GetComponent<TimelineMaster>().MeUUID,"GoodBadData",UpdateGoodBadData_WebSocket);
    }

    //理解していることを示す
    public void OnClick_Smile()
    {
        if(smileImage.GetComponent<Image>().sprite == smileSprite_color){
            return;
        }
        //理解している状態に変更
        smileImage.GetComponent<Image>().sprite = smileSprite_color;
        sadImage.GetComponent<Image>().sprite = sadSprite;
        StartCoroutine( MoveKaomoji(smileImage.GetComponent<RectTransform>().anchoredPosition.x,smileImage.GetComponent<RectTransform>().anchoredPosition.y,SmileAnimation));
        SendGoodBadData(1);
    }

    //顔のアニメーションを再生する
    private IEnumerator MoveKaomoji(float StartX,float StartY,GameObject Kaomoji)
    {
        //初期値の設定
        Kaomoji.GetComponent<RectTransform>().anchoredPosition = new Vector2(StartX,StartY);
        Kaomoji.GetComponent<CanvasGroup>().alpha = 1;
        Kaomoji.GetComponent<RectTransform>().DOKill();
        Kaomoji.GetComponent<CanvasGroup>().DOKill();
        Kaomoji.GetComponent<RectTransform>().DOAnchorPos(new Vector2(StartX,StartY + 200),2);
        // すでにアニメーションが再生中かチェック
        ImageLottiePlayer lottiePlayer = Kaomoji.GetComponent<ImageLottiePlayer>();
        if (lottiePlayer != null && !lottiePlayer.IsPlaying) 
        {
            lottiePlayer.StopAllCoroutines();
            lottiePlayer.Play(); // アニメーション再生
        }
        yield return new WaitForSeconds(1f);
        Kaomoji.GetComponent<CanvasGroup>().DOFade(0,1);
    }

    //理解していないことを示す
    public void OnClick_Sad()
    {
        if(sadImage.GetComponent<Image>().sprite == sadSprite_color){
            return;
        }
        //理解していない状態に変更
        sadImage.GetComponent<Image>().sprite = sadSprite_color;
        smileImage.GetComponent<Image>().sprite = smileSprite;
        StartCoroutine(MoveKaomoji(sadImage.GetComponent<RectTransform>().anchoredPosition.x,sadImage.GetComponent<RectTransform>().anchoredPosition.y,SadAnimation));
        SendGoodBadData(-1);
    }

    //データを送信する
    private void SendGoodBadData(int Evaluation)
    {
        //データを送信する
        GoodBadData goodBadData = new GoodBadData(TimelineMaster.GetComponent<TimelineMaster>().RoomID,
                                                  TimelineMaster.GetComponent<TimelineMaster>().MeUUID,
                                                  Evaluation);
        //AppSyncで送信
        StartCoroutine(gameObject.GetComponent<GoodBadGraphQL>().MutationUpdate(goodBadData,(result) => {
            if(result){
                //Debug.Log("送信成功");
                UpdateGoodBadData(goodBadData);
            }else{
                StartCoroutine(gameObject.GetComponent<GoodBadGraphQL>().MutationInsert(goodBadData,(result)=>{
                    if(result){
                        StartCoroutine(gameObject.GetComponent<GoodBadGraphQL>().MutationUpdate(goodBadData,(result) => {
                            if(result){
                                //Debug.Log("送信成功");
                                UpdateGoodBadData(goodBadData);
                            }else{
                                Debug.Log("送信失敗");
                            }
                        }));
                    }else{
                        Debug.Log("送信失敗");
                    }
                }));
            }
        }));
    }

    //データを受信する
    public IEnumerator GetGoodBadData()
    {
        //データを受信する
        yield return StartCoroutine(gameObject.GetComponent<GoodBadGraphQL>().GetAllData(TimelineMaster.GetComponent<TimelineMaster>().RoomID,(output) => goodBadDatas = output));
        SetBarSize();
        SetSubscribe();
    }

    //データを受信する
    public IEnumerator GetGoodBadData_Reload()
    {
        //データを受信する
        yield return StartCoroutine(gameObject.GetComponent<GoodBadGraphQL>().GetAllData(TimelineMaster.GetComponent<TimelineMaster>().RoomID,(output) => goodBadDatas = output));
        SetBarSize();
    }

    private static void SetBarSize()
    {
        //データを表示する
        Debug.Log("Move");
        if(goodBadDatas == null){
            return;
        }
        int AllCount = goodBadDatas.Length;
        int GoodCount = 0;
        int SadCount = 0;
        for(int i = 0;i < AllCount;i++){
            if(goodBadDatas[i].Evaluation == 1){
                GoodCount++;
            }else if(goodBadDatas[i].Evaluation == -1){
                SadCount++;
            }
        }
        AllCount = GoodCount + SadCount;
        if(AllCount != 0){
            //割合
            float GoodRate = (float)GoodCount / (float)AllCount;
            float SadRate = (float)SadCount / (float)AllCount;
            //バーのサイズを調整する
            Image image = GoodBadArea.GetComponent<Image>();
            Canvas.ForceUpdateCanvases();
            float MainHeight = image.rectTransform.rect.height;
            float MainWidth = image.rectTransform.rect.width;
            //バーの位置の設定
            float BarHeight = MainHeight * 0.8f;
            float BarWidth = MainWidth * 0.7f;
            sadBar.GetComponent<RectTransform>().DOAnchorPosX((BarWidth * SadRate) / 2,1);
            smileBar.GetComponent<RectTransform>().DOAnchorPosX((-BarWidth * GoodRate) / 2,1);
            sadBar.GetComponent<RectTransform>().DOSizeDelta(new Vector2(BarWidth * SadRate,BarHeight),1);
            smileBar.GetComponent<RectTransform>().DOSizeDelta(new Vector2(BarWidth * GoodRate,BarHeight),1);
            
        }
    }

    //データを更新する
    public static void UpdateGoodBadData(GoodBadData goodBadData)
    {
        //データを更新する
        bool isUpdate = false;
        List<GoodBadData> goodBadDataList = goodBadDatas.ToList();
        for(int i = 0;i < goodBadDataList.Count;i++){
            if(goodBadDataList[i].UUID == goodBadData.UUID){
                goodBadDataList[i] = goodBadData;
                isUpdate = true;
                break;
            }
        }

        if(!isUpdate){
            goodBadDataList.Add(goodBadData);
        }

        goodBadDatas = goodBadDataList.ToArray();

        SetBarSize();
    }

    //サブスク用の処理
    [MonoPInvokeCallback(typeof(Action<string,string>))]
    public static void UpdateGoodBadData_WebSocket(string type,string result)
    {
        //データを更新する
        //resultをGetSub_GoodBadDataに変換する
        GetSub_GoodBadData goodBadData = JsonConvert.DeserializeObject<GetSub_GoodBadData>(result);
        if(goodBadData.onUpdateGoodBadData == null){
            return;
        }
        else {
            UpdateGoodBadData(goodBadData.onUpdateGoodBadData);
        }
    }

    [System.Serializable]
    public class GetSub_GoodBadData{
        [SerializeField] public GoodBadData onUpdateGoodBadData;
    }

}

