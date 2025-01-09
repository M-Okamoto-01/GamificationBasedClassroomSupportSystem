using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// <summary>
// https://qiita.com/00riono/items/ef20c344dac937bd0248
// </summary>

public class InputFieldManager : MonoBehaviour
{
    private TMP_InputField inputField;
    public string resultText;   // 入力されたテキストを格納

    private RectTransform parentRect;
    private Vector3 defaultParentPos;  // 初期位置
    private bool isOnceInput = true; // 入力時のfooter・bodyの位置移動フラグ

    private bool isCancel = false;  // cancelボタンが押されたか

    private string inputText;

    //高さを変える時に使用
    public GameObject ParentInputFieldObjct;


    void Start()
    {
        inputField = this.gameObject.GetComponent<TMP_InputField>();
        parentRect = this.transform.parent.GetComponent<RectTransform>();
        defaultParentPos = parentRect.localPosition;
        InitInputField();
    }

    void Update()
    {
        //StartInputText();
    }

    // 入力開始時
    private void StartInputText()
    {
        if (inputField.isFocused && isOnceInput)
        {
            isOnceInput = false;
            // y軸をいい感じの値にする
            parentRect.localPosition += new Vector3(0, 940f, 0);
        }
    }

    // キーボードによって上にずれたUIの位置を戻す
    public void ResetKeybord()
    {
        isOnceInput = true;
        parentRect.localPosition = defaultParentPos;
        isCancel = false;
    }

    // フィールドの初期化
    private void InitInputField()
    {
        inputField.text = "";
        inputText = "";
        ResetKeybord();
    }

    // OnValueCangeで呼び出す関数
    //Dynamic stringが必須
    public void ChangeText(string textvalue)
    {
        //if (inputField.touchScreenKeyboard.status == TouchScreenKeyboard.Status.Canceled)
        if (inputField.wasCanceled)
        {
            // Cancleを押した時
            isCancel = true;
        }
        else if (inputField.isFocused && !isCancel)
        {
            // 他のところをタップした時
            inputText = textvalue;
            inputField.textComponent.text = textvalue;
            int LineCount = GetLine(textvalue);
            //Debug.Log(inputField.textComponent.textInfo.lineCount);
            Debug.Log(LineCount);
            if (LineCount > 2)
            {
                ParentInputFieldObjct.GetComponent<LayoutElement>().minHeight = 70 + 70;
            }
            else
            {

                ParentInputFieldObjct.GetComponent<LayoutElement>().minHeight = 70 * LineCount;
                
            }
        }
    }

    private int GetLine(string str)
    {
        float UpValue = 40;
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UpValue = 40;
        }
        else
        {
            UpValue = 40;
        }
        //Tmp_Textの幅を取得し、それに対して文字数取得
        float TmpWidth = this.gameObject.GetComponent<RectTransform>().GetWidth();
        int addline = 0;
        var lineCount = str.Split('\n');
        foreach(string solo in lineCount)
        {
            if(solo.Length * UpValue > TmpWidth)
            {
                addline = addline + (int)(Math.Ceiling(solo.Length * UpValue / TmpWidth) - 1);
            }
        }
        return lineCount.Length + addline;
        //return inputField.textComponent.textInfo.lineCount;
    }

    public void ResetHeight()
    {
        ParentInputFieldObjct.GetComponent<LayoutElement>().minHeight = 70;
    }

}
