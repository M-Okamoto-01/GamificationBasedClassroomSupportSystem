using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class QuizEditMaster : MonoBehaviour
{
    //GameObject
    public GameObject QuizEditContents;
    public GameObject QuizEditPrefab;
    public QuizGraphQL quizGraphQL;

    public GameObject QuizEditView;
    public GameObject QuizScoreView;

    public QuizTable[] quizTables;

    public QuizMaster quizMaster;

    public string RoomID;
    

    // Start is called before the first frame update
    void Start()
    {
        RoomID = quizMaster.RoomID;
        StartCoroutine(LoadQuizData(quizMaster.RoomID));
    }

    public void LoadQuizData()
    {
        StartCoroutine(LoadQuizData(RoomID));
    }

    public IEnumerator GetMaxQuizIndex(Action<int> callback)
    {
        int maxID = 0;

        yield return StartCoroutine(LoadQuizData(RoomID));
        
        foreach (QuizTable quizDataItem in quizTables)
        {
            if (quizDataItem.QuizID > maxID)
            {
                maxID = quizDataItem.QuizID;
            }
        }

        callback(maxID + 1);
    }

    public IEnumerator LoadQuizData(string RoomID)
    {
        //追加前にコンテンツを空にする
        foreach (Transform n in QuizEditContents.transform)
        {
            Destroy(n.gameObject);
        }

        QuizTable[] quizDataMaster = null;
        yield return StartCoroutine(quizGraphQL.GetQuizAll(RoomID,(output) => quizDataMaster = output));

        quizTables = quizDataMaster;

        // デバッグログ
        if (quizDataMaster == null)
        {
            Debug.LogError("quizDataMaster is null");
            yield break;
        }

        foreach (QuizTable quizDataItem in quizDataMaster)
        {
            //オブジェクトを作成して、データを入れる
            GameObject quizEditObj = Instantiate(QuizEditPrefab);
            quizEditObj.transform.SetParent(QuizEditContents.transform, false);
            quizEditObj.GetComponent<QuizEditObj>().ImportData(quizDataItem);
            quizEditObj.GetComponent<QuizEditObj>().QuizEditView = QuizEditView;
            quizEditObj.GetComponent<QuizEditObj>().QuizScoreView = QuizScoreView;
            quizEditObj.GetComponent<QuizEditObj>().quizMaster = quizMaster;
        }
    }

    public void OnClick_New()
    {
        QuizEditView.SetActive(true);
        QuizTable quizTable = new QuizTable();
        quizTable.QuizMainData = new QuizDataMaster();
        quizTable.QuizMainData.QuizDatas = new QuizData[0];
        QuizEditView.GetComponent<QuizEditCellMaster>().ImportData(quizTable);
    }
}
