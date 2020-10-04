using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Eigo.Models;

public class PlayManager : MonoBehaviour
{
    Question[] Questions;

    void Start()
    {
        Debug.Log("PlayManager.cs#Start: " + CategoryManager.SelectedCategoryName);
        StartCoroutine(APIExample());
    }

    IEnumerator APIExample()
    {
        string url = $"https://zutto-oreno-turn.github.io/cdn/eigo/question/category/news/v1.json";
        // string url = $"https://zutto-oreno-turn.github.io/cdn/eigo/question/category/{CategoryManager.SelectedCategoryName}/v1.json";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        QuestionParser data = JsonUtility.FromJson<QuestionParser>(request.downloadHandler.text);
        Questions = data.questions;

        foreach (Question Question in Questions)
        {
            Debug.Log(Question.id);
            Debug.Log(Question.profile.name);
            Debug.Log(Question.profile.image);
            Debug.Log(Question.sentence);
        }
    }
}
