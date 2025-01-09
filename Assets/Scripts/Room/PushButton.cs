using System.Collections;
using System.Collections.Generic;
using Shapes2D;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PushButton : MonoBehaviour
{
    //Buttons
    [SerializeField] private string NumberPanelName = "NumberPanel";
    [SerializeField] private GameObject[] NPushButtons;
    private GameObject NumberPanelParent;
    private List<GameObject> NumberPanels;


    // Start is called before the first frame update
    void Awake()
    {
        NumberPanels = new List<GameObject>();
        //自分の親を取得
        Transform parent = transform.parent;
        //親の子のNumberPanelNameを持つ子を取得
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name == NumberPanelName)
            {
                NumberPanelParent = parent.GetChild(i).gameObject;
            }
        }
        //NumberPanelParentの子を取得
        for (int i = 0; i < NumberPanelParent.transform.childCount; i++)
        {
            NumberPanels.Add(NumberPanelParent.transform.GetChild(i).gameObject);
        }
        //名前でソート
        NumberPanels.Sort((a, b) => string.Compare(a.name, b.name));
        //ボタンに関数を追加
        AddClickFunction();
        
    }

    private void AddClickFunction()
    {
        for (int i = 0; i < NPushButtons.Length; i++)
        {
            //ボタンに関数を追加
            //名前に依存する
            switch(NPushButtons[i].name)
            {
                case "NPushButton0":
                    NPushButtons[i].GetComponent<Button>().onClick.AddListener(() => ClickPushButton(0));
                    break;
                case "NPushButton1":
                    NPushButtons[i].GetComponent<Button>().onClick.AddListener(() => ClickPushButton(1));
                    break;
                case "NPushButton2":
                    NPushButtons[i].GetComponent<Button>().onClick.AddListener(() => ClickPushButton(2));
                    break;
                case "NPushButton3":
                    NPushButtons[i].GetComponent<Button>().onClick.AddListener(() => ClickPushButton(3));
                    break;
                case "NPushButton4":
                    NPushButtons[i].GetComponent<Button>().onClick.AddListener(() => ClickPushButton(4));
                    break;
                case "NPushButton5":
                    NPushButtons[i].GetComponent<Button>().onClick.AddListener(() => ClickPushButton(5));
                    break;
                case "NPushButton6":
                    NPushButtons[i].GetComponent<Button>().onClick.AddListener(() => ClickPushButton(6));
                    break;
                case "NPushButton7":
                    NPushButtons[i].GetComponent<Button>().onClick.AddListener(() => ClickPushButton(7));
                    break;
                case "NPushButton8":
                    NPushButtons[i].GetComponent<Button>().onClick.AddListener(() => ClickPushButton(8));
                    break;
                case "NPushButton9":
                    NPushButtons[i].GetComponent<Button>().onClick.AddListener(() => ClickPushButton(9));
                    break;
                case "NPushButtonB":
                    NPushButtons[i].GetComponent<Button>().onClick.AddListener(() => ClickPushButton(-1));
                    break;
            }
        }
    }

    private void ClickPushButton(int num)
    {
        //現在入力されている場所を取得
        int nowindex = 0;
        foreach(GameObject solo in NumberPanels)
        {
            if (solo.GetComponent<TMP_InputField>().text != "")
            {
                nowindex++;
            }
        }
        //index最大値ならreturn
        if(num == -1 && nowindex != 0)
        {
            NumberPanels[nowindex - 1].GetComponent<TMP_InputField>().text = "";
        }
        if (nowindex >= NumberPanels.Count)
        {
            return;
        }
        else if(num != -1)
        {
            NumberPanels[nowindex].GetComponent<TMP_InputField>().text = num.ToString();
        }
    }

}
