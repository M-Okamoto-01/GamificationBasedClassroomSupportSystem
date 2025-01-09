using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogButton : MonoBehaviour
{
    private GameObject LogObject;
    public string LogType;
    public string Optional;

    // Start is called before the first frame update
    void Awake()
    {
        LogObject = GameObject.FindGameObjectWithTag("LogGameObject");
        //Buttonがあれば追加する
        if (GetComponent<UnityEngine.UI.Button>() != null)
        {
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnClick);
        }
        else 
        {
            Debug.Log("NoButton" + LogType + " " + Optional);
        }
    }

    private void OnClick()
    {
        LogObject.GetComponent<LogObject>().AddLogData(LogType, Optional);
    }
}
