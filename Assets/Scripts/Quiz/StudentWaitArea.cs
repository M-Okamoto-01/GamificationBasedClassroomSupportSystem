using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AOT;
using System;
using Newtonsoft.Json;
using System.Linq;

public class StudentWaitArea : MonoBehaviour
{
    public QuizMaster quizMaster;
    public QuizTable NowQuizTable;

    public GameObject QuizTitle;
    public static GameObject StudentCount;
    public static GameObject quizGraphql;
    public GameObject quizWebSocket_2;
    public GameObject JoinButton;
    public GameObject JoinLabel;
    public GameObject InfoLabel;

    private int NowUserCount = 0;
    private static List<QuizWaitingTable> WatingUserList;
    private static QuizGameTable QuizGameTable;
    private QuizWaitingTable quizWaitingTable;

    public void Start()
    {
        StudentCount = GameObject.FindGameObjectWithTag("StudentCount_S");
        quizGraphql = GameObject.FindGameObjectWithTag("QuizArea");
        quizWaitingTable = new QuizWaitingTable();
        WatingUserList = new List<QuizWaitingTable>();
    }

    public IEnumerator ImportQuizData(QuizGameTable quizGameTable)
    {
        quizGraphql = GameObject.FindGameObjectWithTag("QuizArea");
        //二重読み込みの防止
        if(QuizGameTable != null)
        {
            if(QuizGameTable.QuizID == quizGameTable.QuizID && QuizGameTable.TargetQuizIndex == quizGameTable.TargetQuizIndex)
            {
                Debug.Log("二重に読み込まれました");
                yield break;
            }
        }

        WatingUserList = new List<QuizWaitingTable>();
        QuizGameTable = quizGameTable;
        //クイズデータの取得
        yield return StartCoroutine(quizGraphql.GetComponent<QuizGraphQL>().GetQuiz(quizGameTable.RoomID,quizGameTable.QuizID, (result) =>
        {
            NowQuizTable = result;
            Debug.Log(NowQuizTable.QuizMainData.title);
            QuizTitle.GetComponent<TMP_Text>().text = NowQuizTable.QuizMainData.title;
        }));
        //自分がクイズに参加しているか確認
        yield return StartCoroutine(quizGraphql.GetComponent<QuizGraphQL>().GetQuizWatingSolo(quizGameTable.RoomID, quizMaster.MeUUID, (result) =>
        {
            if (result != null)
            {
                if(result.QuizID == quizGameTable.QuizID && result.TargetQuizIndex == quizGameTable.TargetQuizIndex)
                {
                    JoinButton.SetActive(false);
                    JoinLabel.SetActive(true);
                    InfoLabel.GetComponent<TMP_Text>().text = "教員が開始するのを\n待っています";
                    SubscribeQuizTable();
                }
                else
                {
                    JoinButton.SetActive(true);
                    JoinLabel.SetActive(false);
                    InfoLabel.GetComponent<TMP_Text>().text = "あなたが参加するのを\nお待ちしています";
                }
            }
        }));

        GetWatingStudentList();
        //サブスクの開始
        SubscribeUserCount();

    }
    

    public void OnClick_StudentJoin()
    {
        QuizWaitingTable quizWaitingTable = new QuizWaitingTable();
        quizWaitingTable.RoomID = QuizGameTable.RoomID;
        quizWaitingTable.QuizID = QuizGameTable.QuizID;
        quizWaitingTable.TargetQuizIndex = QuizGameTable.TargetQuizIndex;
        quizWaitingTable.UserUUID = quizMaster.MeUUID;
        quizWaitingTable.Point = this.quizWaitingTable.Point;

        //自分が参加することを登録
        StartCoroutine(quizGraphql.GetComponent<QuizGraphQL>().MutationQuizWaitingUpsert(quizWaitingTable, (result) => { }));
        
        SubscribeQuizTable();
        JoinButton.SetActive(false);
        JoinLabel.SetActive(true);
    }

    public void SetWatingUser(QuizWaitingTable[] quizWaitingTable)
    {
        foreach(QuizWaitingTable quizWaiting in quizWaitingTable)
        {
            if(quizWaiting.RoomID == QuizGameTable.RoomID && quizWaiting.QuizID == QuizGameTable.QuizID && quizWaiting.TargetQuizIndex == QuizGameTable.TargetQuizIndex)
            {
                WatingUserList.Add(quizWaiting);
            }
        }

        NowUserCount = WatingUserList.Count;
        SetStudentCount();
    }

    public static void SetWatingUser(QuizWaitingTable quizWaiting)
    {
        
        if(quizWaiting.RoomID == QuizGameTable.RoomID && quizWaiting.QuizID == QuizGameTable.QuizID && quizWaiting.TargetQuizIndex == QuizGameTable.TargetQuizIndex)
        {
            WatingUserList.Add(quizWaiting);
        }

        SetStudentCount();
    }


    public static void SetStudentCount()
    {
        //生徒の数を表示する

        //UserUUIDが重複している場合は削除する
        WatingUserList = WatingUserList.GroupBy(x => x.UserUUID).Select(x => x.First()).ToList();

        StudentCount = GameObject.FindGameObjectWithTag("StudentCount_S");

        StudentCount.GetComponent<TMP_Text>().text = WatingUserList.Count.ToString();
    }

    //生徒の数のサブスクを開始
    public void SubscribeUserCount()
    {
        ASEndpoint ASE = AppSyncURL.GetASQuiz(quizMaster.RoomID);
        quizGraphql.GetComponent<AppSyncWebSocketManager>().WebGL_WebSocketSubscribe(ASE,quizMaster.MeUUID,"QuizWatingTable",UpdateUserCount_WebSocket);
    }

    //クイズゲームの開始のサブスクを開始
    public void SubscribeQuizTable()
    {
        ASEndpoint ASE = AppSyncURL.GetASQuizGame(quizMaster.RoomID);
        quizWebSocket_2.GetComponent<AppSyncWebSocketManager>().WebGL_WebSocketSubscribe(ASE,quizMaster.MeUUID,"QuizGameTable",UpdateStartQuizGame_WebSocket);
    }

    public void GetWatingStudentList()
    {
        //生徒の数を取得する
        StartCoroutine(quizGraphql.GetComponent<QuizGraphQL>().GetQuizWatingAll(quizMaster.RoomID, (result) => {
            if(result != null){
                SetWatingUser(result);
            }
        }));
    }

    private static void StartQuiz_init(QuizGameTable qgt)
    {
        //クイズの開始
        QuizMaster quizMaster = quizGraphql.GetComponent<QuizMaster>();
        quizMaster.LoadQuizData(qgt);

    }

    //サブスク用の処理
    [MonoPInvokeCallback(typeof(Action<string,string>))]
    public static void UpdateUserCount_WebSocket(string type,string result)
    {
        //データを更新する
        //resultをGetSub_GoodBadDataに変換する
        GetSub_QuizWatingTable quizwating = JsonConvert.DeserializeObject<GetSub_QuizWatingTable>(result);
        if(quizwating.onCreateQuizWating == null){
            return;
        }
        else {
            SetWatingUser(quizwating.onCreateQuizWating);
            SetStudentCount();
            }
        }

    [System.Serializable]
    public class GetSub_QuizWatingTable{
        [SerializeField] public QuizWaitingTable onCreateQuizWating;
    }

    //サブスク用の処理
    [MonoPInvokeCallback(typeof(Action<string,string>))]
    public static void UpdateStartQuizGame_WebSocket(string type,string result)
    {
        //データを更新する
        //resultをGetSub_GoodBadDataに変換する
        
        GetSub_QuizGameTable quizwating = JsonConvert.DeserializeObject<GetSub_QuizGameTable>(result);
        if(quizwating.onCreateQuizGameModel == null){
            return;
        }
        else {
                if(quizwating.onCreateQuizGameModel.StartFlag == 1)
                {
                    //クイズデータの取得
                    StartQuiz_init(quizwating.onCreateQuizGameModel);
                }
                
            }
        }

    [System.Serializable]
    public class GetSub_QuizGameTable{
        [SerializeField] public QuizGameTable onCreateQuizGameModel;
    }


    
}
