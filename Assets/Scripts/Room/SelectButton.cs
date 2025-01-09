using System.Collections;
using System.Collections.Generic;
using Shapes2D;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SelectButton : MonoBehaviour
{
    private List<GameObject> Buttons;
    private GameObject RoomMasterObj;

    // Start is called before the first frame update
    void Start()
    {
        RoomMasterObj = GameObject.FindGameObjectWithTag("RoomMain");
        //自分の子のButtonPanelの子のボタンを取得
        //自分の子のButtonPanelを取得
        List<GameObject> ButtonList = new List<GameObject>();
        foreach (Transform child in transform)
        {
            if(child.gameObject.name == "ButtonPanel")
            {
                foreach (Transform Button in child)
                {
                    if (Button.gameObject.GetComponent<Button>() != null)
                    {
                        GameObject ButtonObj = Button.gameObject;
                        ButtonObj.GetComponent<Button>().onClick.AddListener(() => OnClickButton(ButtonObj));
                        ButtonList.Add(ButtonObj);
                    }
                }
            }
        }
        Buttons = ButtonList;

    }

    private void OnClickButton(GameObject TargetButton)
    {
        //Imageの色を変える
        TargetButton.GetComponent<Image>().color = new Color(0.1921569f, 0.6588235f, 0.8941177f, 1.0f);
        //Textの色を変える
        TargetButton.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        //自分以外のボタンのImageの色を変える
        foreach (GameObject Button in Buttons)
        {
            if(Button != TargetButton)
            {
                Button.GetComponent<Image>().color = new Color(1, 1, 1, 1.0f);
                Button.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            }
        }

        RoomMasterObj.GetComponent<RoomMaster>().MoveNextPanel();
    }
}
