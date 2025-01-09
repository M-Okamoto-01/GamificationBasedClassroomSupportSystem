using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NumberInput : MonoBehaviour
{
    public GameObject[] InputList;

    // Start is called before the first frame update
    void Start()
    {
        //自分の値が変更されたら、次のInputFieldにフォーカスを移す
        for (int i = 0; i < InputList.Length - 1; i++)
        {
            TMP_InputField inputfield = InputList[i].GetComponent<TMP_InputField>();
            TMP_InputField NextInputField = InputList[i + 1].GetComponent<TMP_InputField>();
            //inputfield.onValueChanged.AddListener((value) => ChangeNext(inputfield,NextInputField));
        }
        //最後のInputFieldの値が変更されたら、フォーカスを外す
        TMP_InputField LastInputField = InputList[InputList.Length - 1].GetComponent<TMP_InputField>();
        LastInputField.onValueChanged.AddListener((value) => LastInputField.DeactivateInputField());
    }

    private void ChangeNext(TMP_InputField inputfield,TMP_InputField NextInputField)
    {
        //次のInputFieldにフォーカスを移す
        inputfield.DeactivateInputField();
        NextInputField.ActivateInputField();
    }

    public string GetValue()
    {
        string Value = "";
        foreach (GameObject Input in InputList)
        {
            Value += Input.GetComponent<TMP_InputField>().text;
        }
        return Value;
    }

    public void ResetValue()
    {
        foreach (GameObject Input in InputList)
        {
            Input.GetComponent<TMP_InputField>().text = "";
        }
    }
}
