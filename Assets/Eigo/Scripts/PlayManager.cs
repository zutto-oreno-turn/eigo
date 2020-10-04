using Eigo.Models;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayManager : MonoBehaviour
{
    public GameObject WordContent;
    public GameObject WordButtonPrefab;

    Question[] Questions;
    int CurrentQuestionNumber = 0;

    void Start()
    {
        Debug.Log("PlayManager.cs#Start: " + CategoryManager.SelectedCategoryName);
        StartCoroutine(GetQuestion());
    }

    IEnumerator GetQuestion()
    {
        Debug.Log("PlayManager.cs#GetQuestion");
        // string url = $"https://zutto-oreno-turn.github.io/cdn/eigo/question/category/{CategoryManager.SelectedCategoryName}/v1.json";
        string url = $"https://zutto-oreno-turn.github.io/cdn/eigo/question/category/tweet/v1.json";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        QuestionParser data = JsonUtility.FromJson<QuestionParser>(request.downloadHandler.text);
        Questions = data.questions;

        // foreach (Question Question in Questions)
        // {
        //     Debug.Log(Question.id);
        //     Debug.Log(Question.profile.name);
        //     Debug.Log(Question.profile.image);
        //     Debug.Log(Question.sentence);
        // }
        MakePlayScreen();
    }

    void MakePlayScreen()
    {
        Debug.Log("PlayManager.cs#MakePlayScreen");
        string[] sentences = Questions[CurrentQuestionNumber].sentence.Split(' ');

        int x = -350, y = 135;
        foreach (string sentence in sentences)
        {
            Debug.Log($"{sentence}, x:{x}, y:{y}");

            GameObject wordButton = Instantiate(WordButtonPrefab, new Vector3(x, y, 0), Quaternion.identity);
            wordButton.transform.SetParent(WordContent.transform, false);

            Text wordButtonText = wordButton.GetComponentInChildren<Text>();
            wordButtonText.text = sentence;

            if (x < -200) {
                x += 130;
            } else {
                x = -350;
                y -= 70;
            }

            wordButton.GetComponent<Button>().onClick.AddListener(() => OnClickWordButton(wordButtonText.text));
        }
    }

    void OnClickWordButton(string word) {
        Debug.Log("PlayManager.cs#OnClickWordButton: " + word);
    }
}
