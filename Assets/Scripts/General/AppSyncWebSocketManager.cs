using System;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Threading;

/// <summary>
/// AppSyncWebSocketManager
/// 
/// MonoBehaviourを継承しているため
/// 必ずGameObjectにアタッチすること
///</summary> 

public class AppSyncWebSocketManager : MonoBehaviour
{
    public static GameObject TMPtextObj;
    private SynchronizationContext context;
    private WebSocketConnection webSocketConnection;

    [DllImport("__Internal")]
    private static extern void CreateWebSocketConnection(string host,string endpoint, string apiKey,string UserUUID, string subscriptionQuery,string returnType,Action<string, string> returnFunc);
    [DllImport("__Internal")]
    private static extern void CloseWebSocketConnection();

    //WebSocketからのメッセージを受け取る
    [MonoPInvokeCallback(typeof(Action<string,string>))]
    static void WebSocketCallBack(string type,string result)
    {
        //Typeによって処理を分岐
        TMPtextObj = GameObject.FindGameObjectsWithTag("Test")[0];
        TMPtextObj.GetComponent<TMP_Text>().text = result;
    }

    void Start()
    {
        //Main threadを取得
        #if UNITY_EDITOR
            // WebGLの場合はWebSocketの接続を行う
            context = SynchronizationContext.Current;
            webSocketConnection = new WebSocketConnection();
        #else
            
        #endif
        
        // AppSyncのエンドポイントとAPIキーを設定
        //string UserUUID = "00000";
        //string returnType = "Test";
        //ASEndpoint aSEndpoint = AppSyncURL.GetASTimeline("00000");
        //WebGL_WebSocketSubscribe(aSEndpoint,UserUUID,returnType,WebSocketCallBack);
    }

    void OnDestroy()
    {
        // WebGLの場合は接続を切断
        #if UNITY_WEBGL && !UNITY_EDITOR
            CloseWebSocketConnection();
        #else
            webSocketConnection.CloseWebSocketConnection();
        #endif
    }

    public void Disconnected()
    {
        // WebGLの場合は接続を切断
        #if UNITY_WEBGL && !UNITY_EDITOR
            CloseWebSocketConnection();
        #else
            webSocketConnection.CloseWebSocketConnection();
        #endif
    }

    /// <summary>
    /// WebGL_WebSocketSubscribe
    /// 
    /// WebGL環境でWebSocketの通信を行う 
    /// </summary>
    public void WebGL_WebSocketSubscribe(ASEndpoint ASE,string UserUUID,string returnType,Action<string, string> returnFunc)
    {
        //WebGLの場合はAppSyncのエンドポイントとAPIキーを設定して接続
        #if UNITY_WEBGL && !UNITY_EDITOR
            // AppSyncのエンドポイントとAPIキーを設定
            CreateWebSocketConnection(ASE.Host, ASE.RealTimeEndpoint, ASE.ApiKey,UserUUID, ASE.SubscriptionQuery,returnType,returnFunc);
        #else
            // WebGL以外の場合はUnityWebRequestを使用して接続
            NativeWebSocket(ASE.Host, ASE.RealTimeEndpoint, ASE.ApiKey,UserUUID, ASE.SubscriptionQuery,returnType,ASE.GraphQLEndPoint,returnFunc);
        #endif
    }

    private async void NativeWebSocket(string host,string endpoint, string apiKey,string UserUUID, string subscriptionQuery,string returnType,string GraphQLEndPoint,Action<string, string> returnFunc)
    {
        await webSocketConnection.AddSubscription(host, endpoint, apiKey,UserUUID, subscriptionQuery,GraphQLEndPoint,returnType,returnFunc,context);
    }
}
