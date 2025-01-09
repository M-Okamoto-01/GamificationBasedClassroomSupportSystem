using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using Shapes2D;
using System.Linq;
using AOT;
using System;


public class QuizResultMaster : MonoBehaviour
{
    //オブジェクトの紐付け
    public GameObject QuizTitleObj;
    public GameObject QuizGraphQL;
    public QuizMaster quizMaster;

    public GameObject QuizResultCellPrefab;


    //取得するオブジェクト
    private static GameObject ScrollViewContent;

    //この問題のデータ
    private static QuizTable MeQT;
    private static List<GameObject> QuizSelections;

    void Awake()
    {
        ScrollViewContent = GameObject.FindWithTag("QuizResultScrollViewContent");
    }

    //クイズの回答の内容を表示する
    public void GetQuizData(QuizTable quizTable)
    {
        MeQT = quizTable;
        Debug.Log(MeQT.QuizMainData.title);
        QuizTitleObj.GetComponent<TMP_Text>().text = MeQT.QuizMainData.title;
        SetQuizSelections();
    }

    //表示の初期化
    private void SetQuizSelections()
    {
        //初期化
        if(QuizSelections != null)
        {
            foreach (GameObject QuizSelection in QuizSelections)
            {
                Destroy(QuizSelection);
            }
        }

        QuizSelections = new List<GameObject>();

        int indexCount = 0;
        
        foreach(QuizData QD in MeQT.QuizMainData.QuizDatas)
        {
            ScrollViewContent = GameObject.FindWithTag("QuizResultScrollViewContent");
            GameObject QuizResultCell = Instantiate(QuizResultCellPrefab, ScrollViewContent.transform);
            QuizResultCell.GetComponent<QuizResultCell>().SetQuizData(QD, MeQT.RoomID, MeQT.QuizID, indexCount);
            QuizSelections.Add(QuizResultCell);
            indexCount++;
        }

        GetStudentAnswerData();
    }

    //学生の回答を全て取得する
    public void GetStudentAnswerData()
    {
        StartCoroutine(QuizGraphQL.GetComponent<QuizGraphQL>().GetQuizScoreAll(quizMaster.RoomID,(output) =>
        {
            if (output != null)
            {
                SetScoreData(output.ToList());
            }
            //サブスクの開始
            SubscribeScoreData();
        }));
    }

    //回答情報を全てSelectionに渡す
    private void SetScoreData(List<QuizScoreTable> QSTL)
    {
        foreach(QuizScoreTable QST in QSTL)
        {
            //QuizIDの一致
            if(QST.QuizID != MeQT.QuizID)
            {
                continue;
            }

            if(QST.QuizIndex >= QuizSelections.Count)
            {
                continue;
            }

            Debug.Log(QST.QuizIndex);

            QuizSelections[QST.QuizIndex].GetComponent<QuizResultCell>().SetQuizScoreDataUseStatic(QST);
        }
    }

    //回答情報の更新
    private static void UpdateScoreData(QuizScoreTable QST)
    {
        //QuizIDの一致
        if(QST.QuizID == MeQT.QuizID)
        {
            if(QST.QuizIndex >= QuizSelections.Count)
            {
                return;
            }
            QuizSelections[QST.QuizIndex].GetComponent<QuizResultCell>().SetQuizScoreDataUseStatic(QST);
        }
    }

    //Subscriptionsでデータが更新された時に呼び出される
    //生徒の数のサブスクを開始
    public void SubscribeScoreData()
    {
        ASEndpoint ASE = AppSyncURL.GetASQuizScore(quizMaster.RoomID);
        QuizGraphQL.GetComponent<AppSyncWebSocketManager>().WebGL_WebSocketSubscribe(ASE,quizMaster.MeUUID,"QuizScoreTable",UpdateScoreData_WebSocket);
    }

    //サブスク用の処理
    [MonoPInvokeCallback(typeof(Action<string,string>))]
    public static void UpdateScoreData_WebSocket(string type,string result)
    {
        //データを更新する
        //resultをGetSub_GoodBadDataに変換する
        GetSub_QuizScoreTable quizwating = JsonConvert.DeserializeObject<GetSub_QuizScoreTable>(result);
        if(quizwating.onCreateQuizScoreTable == null)
        {
            return;
        }
        else 
        {
                UpdateScoreData(quizwating.onCreateQuizScoreTable);
        }
    }

    [System.Serializable]
    public class GetSub_QuizScoreTable{
        [SerializeField] public QuizScoreTable onCreateQuizScoreTable;
    }


    

    
}
