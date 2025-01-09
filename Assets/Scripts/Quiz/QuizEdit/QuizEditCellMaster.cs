using System.Collections;
using System.Collections.Generic;
using Shapes2D;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizEditCellMaster : MonoBehaviour
{
    public GameObject QuizEditCellPrefab;
    public GameObject QuizEditCellContents;
    public GameObject TitleFieldObject;

    public QuizTable quizTable;
    public GameObject AddCellObj;

    public QuizGraphQL quizGraphQL;
    public QuizEditMaster quizEditMaster;

    public GameObject ScrollVireObj;

    public void ImportData(QuizTable quizTable)
    {
        //AddCellObjを持っていないクラスを削除
        foreach (Transform n in QuizEditCellContents.transform)
        {
            if (n.GetComponent<QuizEditCellObj>() == null) continue;
            Destroy(n.gameObject);
        }

        this.quizTable = quizTable;

        foreach (QuizData quizData in quizTable.QuizMainData.QuizDatas)
        {
            GameObject quizEditCellObj = Instantiate(QuizEditCellPrefab);
            quizEditCellObj.transform.SetParent(QuizEditCellContents.transform, false);
            quizEditCellObj.GetComponent<QuizEditCellObj>().ImportData(quizData);
        }

        AddCellObj.transform.SetAsLastSibling();

        //一番上までスクロール
        ScrollVireObj.GetComponent<ScrollRect>().verticalNormalizedPosition = 1.0f;

        TitleFieldObject.GetComponent<TMP_InputField>().text = quizTable.QuizMainData.title;
    }

    public void OnClick_submit()
    {
        StartCoroutine(Submit_QuizTable());
    }

    public IEnumerator Submit_QuizTable()
    {
        quizTable.QuizMainData.title = TitleFieldObject.GetComponent<TMP_InputField>().text;

        // 一時的なリストを使用してデータを収集する
        List<QuizData> quizDataList = new List<QuizData>();

        for (int i = 0; i < QuizEditCellContents.transform.childCount; i++)
        {
            QuizEditCellObj quizEditCellObj = QuizEditCellContents.transform.GetChild(i).GetComponent<QuizEditCellObj>();
            if (quizEditCellObj != null)
            {
                quizDataList.Add(quizEditCellObj.ExportData());
            }
        }

        // リストを配列に変換してQuizDatasに設定する
        quizTable.QuizMainData.QuizDatas = quizDataList.ToArray();

        //quizTableの必須項目を登録
        if (quizTable.RoomID == null || quizTable.RoomID == "")
        {
            quizTable.RoomID = quizEditMaster.RoomID;
        }
        if(quizTable.QuizID == 0)
        {
            yield return StartCoroutine(quizEditMaster.GetMaxQuizIndex((output) => quizTable.QuizID = output));
        }
        


        //これをサーバーに送信する
        bool result = false;
        yield return StartCoroutine(quizGraphQL.MutationQuizUpsert(quizTable, (output) => result = output));
        if (result)
        {
            Debug.Log("Success");
        }
        else
        {
            Debug.Log("Failed");
        }

        yield return StartCoroutine(quizEditMaster.LoadQuizData(quizTable.RoomID));

        //自分を閉じる
        gameObject.SetActive(false);

    }
    
}
