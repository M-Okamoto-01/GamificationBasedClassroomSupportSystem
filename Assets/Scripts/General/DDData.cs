using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// シーンをまたいでもデータを保持する
/// Roomシーン→Homeシーンに移動しても値を保持する
/// </summary>
public class DDData : MonoBehaviour
{
    public RoomData[] RoomData;
    public string UserName;
    public string UserUUID;
    public bool OwnerFlag = false;

    void Start()
    {
        //自分自身が破棄されないようにする
        DontDestroyOnLoad(this.gameObject);   
    }

    //強制リロードが入っても簡単に戻れるようにする
    public void SavePlayerPrefs()
    {
        Debug.Log("SavePlayerPrefs");
        PlayerPrefs.SetString("UserName", UserName);
        PlayerPrefs.SetString("UserUUID", UserUUID);
        PlayerPrefs.SetString("RoomID", RoomData[0].RoomID);
        PlayerPrefs.SetInt("OwnerFlag", OwnerFlag ? 1 : 0);
        PlayerPrefs.Save();
    }

    //簡単に戻るためにPlayerPrefsをロードする
    public void LoadPlayerPrefs()
    {
        UserName = PlayerPrefs.GetString("UserName");
        Debug.Log("UserName:" + UserName);
        UserUUID = PlayerPrefs.GetString("UserUUID");
        RoomData = new RoomData[1];
        RoomData[0] = new RoomData();
        RoomData[0].RoomID = PlayerPrefs.GetString("RoomID");
        OwnerFlag = PlayerPrefs.GetInt("OwnerFlag") == 1;
    }

    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); 
    }

    public bool ExitPlayerPrefs()
    {
        if(UserName == null || UserName == "")
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
