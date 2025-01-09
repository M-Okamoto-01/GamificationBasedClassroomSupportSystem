using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonSolo : MonoBehaviour
{
    public GameObject NextPanel;
    private GameObject SVContentObj;

    //自分の子を探し続けて、Buttonを取得し、MoveNextPanelを呼び出す
    void Start()
    {
        SVContentObj = GameObject.FindGameObjectWithTag("RoomSVContent");
        foreach (Transform child in transform)
        {
            CheckButton(child);
        }
    }

    private void CheckButton(Transform child)
    {
        if(child.gameObject.GetComponent<Button>() != null)
        {
            GameObject ButtonObj = child.gameObject;
            ButtonObj.GetComponent<Button>().onClick.AddListener(() => MoveNextPanel());
        }
        else
        {
            foreach (Transform grandchild in child)
            {
                CheckButton(grandchild);
            }
        }
    }

    private void MoveNextPanel()
    {
        if(NextPanel == null)
        {
            return;
        }
        NextPanel.SetActive(true);
        Vector2 PreVec = SVContentObj.GetComponent<RectTransform>().anchoredPosition;
        SVContentObj.GetComponent<RectTransform>().DOAnchorPos(PreVec + new Vector2(-800,0), 0.5f);
    }
}
