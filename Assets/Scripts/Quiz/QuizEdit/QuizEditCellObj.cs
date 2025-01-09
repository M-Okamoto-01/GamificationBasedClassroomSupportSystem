using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Newtonsoft.Json;

public class QuizEditCellObj : MonoBehaviour
{
    public GameObject QuizTileField;
    public GameObject[] QuizSelectFields_CorrenctButton;
    public GameObject[] QuizSelectFields_CorrenctLabel;
    public GameObject[] QuizSelectFields_Texts;

    private QuizData quizData;

    public void Awake()
    {
        quizData = new QuizData();
        quizData.QuizTitle = "";
        quizData.ImageURL = "";
        quizData.QuestionType = "";
        quizData.Slections = new QuizSlection[4];
        for (int i = 0; i < 4; i++)
        {
            quizData.Slections[i] = new  QuizSlection();
            quizData.Slections[i].Label = "";
            quizData.Slections[i].CorrectFlag = false;
        }
    }

    public void OnClick_Delete()
    {
        Destroy(gameObject);
    }

    //クイズデータのインポート
    public void ImportData(QuizData quizData)
    {
        this.quizData = quizData;
        
        //quizDataのSlentionsの数が4未満の場合は追加する
        if (quizData.Slections.Length < 4)
        {
            QuizSlection[] newSlections = new QuizSlection[4];
            for (int i = 0; i < 4; i++)
            {
                if (i < quizData.Slections.Length)
                {
                    newSlections[i] = quizData.Slections[i];
                }
                else
                {
                    newSlections[i] = new QuizSlection();
                    newSlections[i].Label = "";
                    newSlections[i].CorrectFlag = false;
                }
            }
            quizData.Slections = newSlections;
        }

        this.quizData = quizData;

        QuizTileField.GetComponent<TMP_InputField>().text = quizData.QuizTitle;
        
        for (int i = 0; i <4; i++)
        {
            QuizSelectFields_Texts[i].GetComponent<TMP_InputField>().text = quizData.Slections[i].Label;
            if(quizData.Slections[i].CorrectFlag)
            {
                QuizSelectFields_CorrenctLabel[i].GetComponent<TMP_Text>().text = "正解";
                // ColorBlockを取得
                ColorBlock colorBlock = QuizSelectFields_CorrenctButton[i].GetComponent<Button>().colors;

                // Normal Colorを設定
                colorBlock.normalColor = new Color(1f, 0.4117647f, 0.5555856f, 1.0f);

                // ColorBlockを再設定
                QuizSelectFields_CorrenctButton[i].GetComponent<Button>().colors = colorBlock;
            }    
            else
            {
                QuizSelectFields_CorrenctLabel[i].GetComponent<TMP_Text>().text = "不正解";
                // ColorBlockを取得
                ColorBlock colorBlock = QuizSelectFields_CorrenctButton[i].GetComponent<Button>().colors;

                // Normal Colorを設定
                colorBlock.normalColor = new Color(0.4117647f, 0.8200425f, 1f, 1.0f);

                // ColorBlockを再設定
                QuizSelectFields_CorrenctButton[i].GetComponent<Button>().colors = colorBlock;
            }
        }
    }

    //正解の選択肢の変更
    public void OnClick_Change1()
    {
        ChangeCorrectFlag(0);
    }
    public void OnClick_Change2()
    {
        ChangeCorrectFlag(1);
    }
    public void OnClick_Change3()
    {
        ChangeCorrectFlag(2);
    }
    public void OnClick_Change4()
    {
        ChangeCorrectFlag(3);
    }

    private void ChangeCorrectFlag(int index)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == index)
            {
                QuizSelectFields_CorrenctLabel[i].GetComponent<TMP_Text>().text = "正解";
                // ColorBlockを取得
                ColorBlock colorBlock = QuizSelectFields_CorrenctButton[i].GetComponent<Button>().colors;

                // Normal Colorを設定
                colorBlock.normalColor = new Color(1f, 0.4117647f, 0.5555856f, 1.0f);

                // ColorBlockを再設定
                QuizSelectFields_CorrenctButton[i].GetComponent<Button>().colors = colorBlock;
                quizData.Slections[i].CorrectFlag = true;
            }
            else
            {
                QuizSelectFields_CorrenctLabel[i].GetComponent<TMP_Text>().text = "不正解";
                // ColorBlockを取得
                ColorBlock colorBlock = QuizSelectFields_CorrenctButton[i].GetComponent<Button>().colors;

                // Normal Colorを設定
                colorBlock.normalColor = new Color(0.4117647f, 0.8200425f, 1f, 1.0f);

                // ColorBlockを再設定
                QuizSelectFields_CorrenctButton[i].GetComponent<Button>().colors = colorBlock;
                quizData.Slections[i].CorrectFlag = false;
            }
        }
    }

    //クイズデータのエクスポート
    public QuizData ExportData()
    {
        quizData.QuizTitle = QuizTileField.GetComponent<TMP_InputField>().text;
        for (int i = 0; i < 4; i++)
        {
            quizData.Slections[i].Label = QuizSelectFields_Texts[i].GetComponent<TMP_InputField>().text;
            quizData.Slections[i].CorrectFlag = QuizSelectFields_CorrenctLabel[i].GetComponent<TMP_Text>().text == "正解";
        }

        return quizData;
    }

}
