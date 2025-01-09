using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

public class SelectionsMaster : MonoBehaviour
{
    //自分の情報を格納
    private QuizSlection select;
    private int OriginalIndex;

    //QuizMasterを取得
    private QuizMaster quizMaster;
    
    //値
    const float NormalHeight = 60.0f;
    const float BigHeight = 90.0f;
    private Color NormalColor = new Color(0.9607843f,0.9607843f,0.9607843f,1);
    private Color CorrectColor = new Color(0.5411765f, 1, 0.5019608f, 1);
    private Color WrongColor = new Color(0.9921569f, 0.4431373f, 0.4431373f, 1);

    // Start is called before the first frame update
    void Awake()
    {
        this.GetComponent<Button>().onClick.AddListener(() => {
            //クイズの選択肢を選択した時の処理
            OnClick();
        });
    }

    //回答を設定する
    public void SetQuizData(QuizSlection select,string ImageURL,QuizMaster quizMaster,int OriginalIndex)
    {
        this.GetComponent<Outline>().enabled = false;
        //クイズの情報をセットする　
        this.select = select;
        this.quizMaster = quizMaster;
        this.OriginalIndex = OriginalIndex;
        this.GetComponentInChildren<TMP_Text>().text = select.Label;
        if(ImageURL != "" || ImageURL != null)
        {
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(400,BigHeight);
        }
        else
        {
            this.GetComponent<RectTransform>().sizeDelta = new Vector2(400,NormalHeight);
        }
        this.GetComponent<Image>().color = NormalColor;
    }

    //自分がクリックされたとき
    private void OnClick()
    {
        //クイズの選択肢を選択した時の処理
        if(quizMaster != null)
        {
            if(quizMaster.WaitFlag == false)
            {
                return;
            }

            if(select.CorrectFlag)
            {
                this.GetComponent<Image>().color = CorrectColor;
            }
            else
            {
                this.GetComponent<Image>().color = WrongColor;
            }

            this.GetComponent<Outline>().enabled = true;
            quizMaster.OnClickSelection(select,OriginalIndex);
        }
    }

    //自分が正解かどうかを表示
    public void ShowCorrect()
    {
        if(select.CorrectFlag)
        {
            this.GetComponent<Image>().color = CorrectColor;
        }
        else
        {
            this.GetComponent<Image>().color = WrongColor;
        }
    }
}
