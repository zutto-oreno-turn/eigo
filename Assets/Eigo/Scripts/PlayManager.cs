using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class Questions
{
    public Question[] questions;
}

[Serializable]
public class Question
{
    public string id;
    public Profile profile;
    public string sentence;
}

[Serializable]
public class Profile
{
    public string name;
    public string image;
}

public class PlayManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("PlayManager.cs#Start");
        StartCoroutine(APIExample());
    }

    IEnumerator APIExample()
    {
        string url = "https://zutto-oreno-turn.github.io/cdn/eigo/question/category/custom/v1.json";
        // string url = "https://zutto-oreno-turn.github.io/cdn/eigo/question/category/news/v1.json";
        // string url = "https://zutto-oreno-turn.github.io/cdn/eigo/question/category/quote/v1.json";
        // string url = "https://zutto-oreno-turn.github.io/cdn/eigo/question/category/tweet/v1.json";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        Questions data = JsonUtility.FromJson<Questions>(request.downloadHandler.text);
        Debug.Log(data.questions[0].id);
        Debug.Log(data.questions[0].profile.name);
        Debug.Log(data.questions[0].profile.image);
        Debug.Log(data.questions[0].sentence);
    }
}
