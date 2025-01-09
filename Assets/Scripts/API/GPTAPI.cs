using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Text.RegularExpressions;

public class GPTAPI
{
    /// <summary>
    /// StartCoroutine(GPTTalk(Question,
    ///     (choice) =>
    ///     {
    ///         if (choice != null)
    ///         {
    ///             Debug.Log(choice.message.content);
    ///         }
    ///     }
    /// ));
    /// </summary>
    /// <param name="Question">
    /// 質問内容
    /// </param>
    /// <param name="callback">
    /// GPTChoiceを返す
    /// </param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="Question"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator GPTTalk_old(string Question,Action<GPTChoice> callback)
    {
        string openaiApiKey = "APIKey";
        string endpoint = "https://api.openai.com/v1/chat/completions";

        var payload = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = "あなたは先生です。わかりやすく、正確に質問に答えてください。" },
                new { role = "user", content = Question }
            }
        };
        // JSON形式に変換
        var jsonPayload = JsonConvert.SerializeObject(payload);

        using (UnityWebRequest request = UnityWebRequest.Post(endpoint, jsonPayload, "POST"))
        {
            request.SetRequestHeader("Authorization", $"Bearer {openaiApiKey}");
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                GPTRoot response = JsonConvert.DeserializeObject<GPTRoot>(request.downloadHandler.text);
                callback?.Invoke(response.choices[0]);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                callback?.Invoke(null);
            }
        }
    }

    public IEnumerator GPTTalk(string Question, Action<GPTChoice> callback)
    {
        string openaiApiKey = "APIKey";
        string endpoint = "https://api.openai.com/v1/chat/completions";

        var payload = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "system", content = "あなたは先生です。わかりやすく、正確に質問に答えてください。" },
                new { role = "user", content = Question }
            }
        };
        // JSON形式に変換
        var jsonPayload = JsonConvert.SerializeObject(payload);

        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {openaiApiKey}");
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                GPTRoot response = JsonConvert.DeserializeObject<GPTRoot>(request.downloadHandler.text);

                string content = response.choices[0].message.content;
                string processedText = ConvertMarkdownHeaders(content);
                // '**テキスト**'を'<b>テキスト</b>'に変換
                processedText = Regex.Replace(processedText, @"\*\*(.*?)\*\*", "<b>$1</b>");
                response.choices[0].message.content = processedText + "\n\n";

                callback?.Invoke(response.choices[0]);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
                callback?.Invoke(null);
            }
        }
    }

    string ConvertMarkdownHeaders(string text)
    {
        // 見出しレベル1から6までを処理
        for (int i = 6; i >= 1; i--)
        {
            string markdownHeader = new string('#', i);
            string pattern = @"^" + Regex.Escape(markdownHeader) + @"\s*(.*)$";
            string replacement = "";

            // レベルに応じてサイズを設定
            int sizeIncrement = (7 - i) * 3; // レベル1が最大サイズ

            replacement = $"<size=+{sizeIncrement}><b>$1</b></size>";

            text = Regex.Replace(text, pattern, replacement, RegexOptions.Multiline);
        }

        return text;
    }
}

public class GPTChoice
{
    public int index { get; set; }
    public GPTMessage message { get; set; }
    public object logprobs { get; set; }
    public string finish_reason { get; set; }
}

public class GPTMessage
{
    public string role { get; set; }
    public string content { get; set; }
}

public class GPTRoot
{
    public string id { get; set; }
    public string @object { get; set; }
    public int created { get; set; }
    public string model { get; set; }
    public List<GPTChoice> choices { get; set; }
    public GPTUsage usage { get; set; }
    public string system_fingerprint { get; set; }
}

public class GPTUsage
{
    public int prompt_tokens { get; set; }
    public int completion_tokens { get; set; }
    public int total_tokens { get; set; }
}

