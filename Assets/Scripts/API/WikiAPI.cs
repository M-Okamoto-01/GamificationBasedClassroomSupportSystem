using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text.RegularExpressions;

public class WikiAPI 
{
    //WikipediaのAPI

    // void Start()
    // {
    //     StartCoroutine(GetWikiData_free("AWS", (data) =>
    //     {
    //         foreach (var d in data)
    //         {
    //             Debug.Log(d.Split("!_!")[0]);
    //             Debug.Log(d.Split("!_!")[1]);
    //         }
    //     }));
    // }

    // /// <summary>
    // /// GetWikiData
    // /// WikipediaのAPIを使用して、データを取得する
    // /// List<string>のデータを取得する
    // /// 中身：タイトル!_!概要
    // /// d.Split("!_!")[0]して使う</summary>
    // /// <param name="title">検索タイトル</param>
    // /// <param name="callback">data</param>
    // /// <returns></returns> 
    // public IEnumerator GetWikiData(string title,Action<List<string>> callback)
    // {
    //     //Titleの空白を消す
    //     title = title.Replace(" ", "");
        
    //     var param = new Dictionary<string, string> ();
        
    //     param ["action"] = "query"; // アクション
    //     param ["titles"] = WWW.EscapeURL(title); // 検索キーワード
    //     param ["prop"] = "extracts"; // 概要を取得
    //     param ["format"] = "json"; // JSON形式で取得
    //     param ["utf8"] = "1"; // UTF-8で取得

    //     string BaseURL = "https://ja.wikipedia.org/w/api.php?";

    //     //URLを作成
    //     string URL = BaseURL;
    //     foreach (var p in param)
    //     {
    //         URL += string.Format ("{0}={1}&", p.Key, p.Value);
    //     }
    //     //これを追加すると、概要を取得することができる。
    //     URL = URL + "exintro&explaintext&redirects=1";

    //     //上記をUnityWebRequestに変更
    //     var handle = UnityWebRequest.Get(URL);
    //     yield return handle.SendWebRequest();
    //     if (handle.result == UnityWebRequest.Result.ConnectionError)
    //     {
    //         //Error Handle
    //         Debug.LogWarning($"{handle.error}");
    //         handle.Dispose();
    //         callback?.Invoke(null);
    //     }
    //     else
    //     {
    //         //Success
    //         var jsonText = handle.downloadHandler.text;
    //         QueryResponse response = JsonConvert.DeserializeObject<QueryResponse>(jsonText);
    //         callback?.Invoke(GetkWikiStrData(response));
    //         handle.Dispose();
    //     }
        
    // }

    // private List<string> GetkWikiStrData(QueryResponse WikiData)
    // {

    //     //正規表現で取得
    //     List<string> result = new List<string>();
    //     if (WikiData != null)
    //     {
    //         if(WikiData.query != null)
    //         {
    //             foreach (var page in WikiData.query.pages)
    //             {
    //                 result.Add(page.Value.title + "!_!" + page.Value.extract);
    //             }
    //         }
    //     }

    //     return result;
    // }

    /// <summary>
    /// GetWikiData_free
    /// WikipediaのAPIを使用して、データを取得する
    /// List<string>のデータを取得する
    /// 中身：タイトル!_!概要
    /// d.Split("!_!")[0]して使う</summary>
    /// <param name="title">検索タイトル</param>
    /// <param name="callback">data</param>
    /// <returns></returns> 
    public IEnumerator GetWikiData_free(string title,Action<List<string>> callback)
    {
        //Titleの空白を消す
        title = title.Replace(" ", "");
        
        var param = new Dictionary<string, string> ();
        
        param ["action"] = "query"; // アクション
        param ["format"] = "json"; // JSON形式で取得
        param ["utf8"] = "1"; // UTF-8で取得
        param ["list"] = "search"; // 検索結果を取得
        param ["srsearch"] = WWW.EscapeURL(title); // 検索キーワード
        param ["srprop"] = "snippet"; // スニペットを取得
        param ["srlimit"] = "5"; // 取得数
        param ["origin"] = "*"; // CORS対策

        string BaseURL = "https://ja.wikipedia.org/w/api.php?";

        //URLを作成
        string URL = BaseURL;
        foreach (var p in param)
        {
            URL += string.Format ("{0}={1}&", p.Key, p.Value);
        }
        //これを追加すると、概要を取得することができる。
        URL = URL + "exintro&explaintext&redirects=1";

        Debug.Log(URL);

        //上記をUnityWebRequestに変更
        var handle = UnityWebRequest.Get(URL);
        //handle.SetRequestHeader("Access-Control-Allow-Origin", "*");
        yield return handle.SendWebRequest();
        if (handle.result == UnityWebRequest.Result.ConnectionError)
        {
            //Error Handle
            Debug.LogWarning($"{handle.error}");
            handle.Dispose();
            callback?.Invoke(null);
        }
        else
        {
            //Success
            var jsonText = handle.downloadHandler.text;
            WikiFreeData response = JsonConvert.DeserializeObject<WikiFreeData>(jsonText);
            callback?.Invoke(GetkWikiStrData(response));
            handle.Dispose();
        }
        
    } 

    private List<string> GetkWikiStrData(WikiFreeData WikiData)
    {

        //正規表現で取得
        List<string> result = new List<string>();
        if (WikiData != null)
        {
            if(WikiData.query != null)
            {
                foreach (var page in WikiData.query.search)
                {
                    result.Add(page.title + "!_!" + RemoveHtmlTags(page.snippet));
                }
            }
        }

        return result;
    }

    // スニペットからHTMLタグを削除する関数
    private string RemoveHtmlTags(string input)
    {
        return Regex.Replace(input, "<.*?>", string.Empty);
    }
    
}

[System.Serializable]
public class QueryResponse
{
    public Query query;
}

[System.Serializable]
public class Query
{
    public Dictionary<string, PageInfo> pages;
}

[System.Serializable]
public class PageInfo
{
    public int pageid;
    public int ns;
    public string title;
    public string extract;
}

[System.Serializable]
public class WikiSearch {
    public int ns { get; set; }
    public string title { get; set; }
    public int pageid { get; set; }
    public int size { get; set; }
    public int wordcount { get; set; }
    public string snippet { get; set; }
    public DateTime timestamp { get; set; }

}
[System.Serializable]
public class WikiQuery {
    public List<WikiSearch> search { get; set; }

}
[System.Serializable]
public class WikiFreeData {
    public string batchcomplete { get; set; }
    public WikiQuery query { get; set; } 

}
