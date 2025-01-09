using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// StructList
///
/// 構造体が全て並ぶ
/// 
/// </summary>

[SerializeField]
// memo
// UUIDで自分がいいねを押したか管理すればいいんでね？
public class TimelineData
{
    public string RoomID { get; set; } = "";
    public string RegisterDateUUID { get; set; } = "";
    public System.DateTime SendDateTime { get; set; } = System.DateTime.Now;
    public string SenderUUID { get; set; } = "";
    public string SenderName { get; set; } = "";
    public string content { get; set; } = "";
    public string[] HeartSenderUUIDList { get; set; } = new string[0];
    public string[] ReplyRegisterDateUUIDList { get; set; } = new string[0];

    public TimelineData(string roomID, string registerDateUUID, System.DateTime sendDateTime, string senderUUID, string senderName, string Content, string[] replyRegisterDateUUIDList = null, string[] heartSenderUUIDList = null)
    {
        RoomID = roomID;
        RegisterDateUUID = registerDateUUID;
        SendDateTime = sendDateTime;
        SenderUUID = senderUUID;
        SenderName = senderName;
        content = Content;
        HeartSenderUUIDList = heartSenderUUIDList ?? new string[0];
        ReplyRegisterDateUUIDList = replyRegisterDateUUIDList ?? new string[0];
    }

    public TimelineData()
    {
        RoomID = "";
        RegisterDateUUID = "";
        SendDateTime = System.DateTime.Now;
        SenderUUID = "";
        SenderName = "";
        content = "";
        HeartSenderUUIDList = new string[0];
        ReplyRegisterDateUUIDList = new string[0];
    }
}

[SerializeField]
public class AppendReplyRegisterDateUUIDList
{
    public string RegisterDateUUID { get; set; } = "";
    public string AddUUID { get; set; } = "";
    public string RoomID { get; set; } = "";

    public AppendReplyRegisterDateUUIDList(string registerDateUUID, string addUUID,string roomID)
    {
        RegisterDateUUID = registerDateUUID;
        AddUUID = addUUID;
        RoomID = roomID;
    }

    public AppendReplyRegisterDateUUIDList()
    {
        RegisterDateUUID = "";
        AddUUID = "";
        RoomID = "";
    }
}

[SerializeField]
public class DeleteHeartSenderUUIDList
{
    public string RegisterDateUUID { get; set; } = "";
    public int DeleteUUIDIndex { get; set; } = 0;
    public string RoomID { get; set; } = "";

    public DeleteHeartSenderUUIDList(string registerDateUUID, int deleteUUIDIndex, string roomID)
    {
        RegisterDateUUID = registerDateUUID;
        DeleteUUIDIndex = deleteUUIDIndex;
        RoomID = roomID;
    }

    public DeleteHeartSenderUUIDList()
    {
        RegisterDateUUID = "";
        DeleteUUIDIndex = 0;
        RoomID = "";
    }
}

[SerializeField]
public class RoomData
{
    public string RoomID { get; set; } = "";
    public string RoomName { get; set; } = "";
    public string OwnerID { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public string[] Keyword { get; set; } = new string[0];

    public RoomData(string roomID, string roomName, string ownerID, string ownerName, string[] keyword)
    {
        RoomID = roomID;
        RoomName = roomName;
        OwnerID = ownerID;
        OwnerName = ownerName;
        Keyword = keyword;
    }

    public RoomData()
    {
        RoomID = "";
        RoomName = "";
        OwnerID = "";
        OwnerName = "";
        Keyword = new string[0];
    }
}

[System.Serializable]
public class GoodBadData
{
    public string RoomID { get; set; } = "";
    public string UUID { get; set; } = "";
    public int Evaluation { get; set; } = 0;

    public GoodBadData(string roomID, string uuid, int evaluation)
    {
        RoomID = roomID;
        UUID = uuid;
        Evaluation = evaluation;
    }

    public GoodBadData()
    {
        RoomID = "";
        UUID = "";
        Evaluation = 0;
    }
}

[System.Serializable]
public class KeywordData
{
    public string RoomID { get; set; } = "";
    public string UUID { get; set; } = "";
    public string Data { get; set; } = "";
    public bool VisibleFlag { get; set; } = true;

    public KeywordData(string roomID, string uuid, string data, bool visibleFlag)
    {
        RoomID = roomID;
        UUID = uuid;
        Data = data;
        VisibleFlag = visibleFlag;
    }

    public KeywordData()
    {
        RoomID = "";
        UUID = "";
        Data = "";
        VisibleFlag = true;
    }
}