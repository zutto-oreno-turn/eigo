using Eigo.Models;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayManager : MonoBehaviour
{

    public GameObject AnswerText;
    public GameObject WordContent;
    public GameObject WordButtonPrefab;

    Question[] Questions;

    int CurrentQuestionNumber = 0;
    int ChoiceNumber = 2;
    int AnserNumber = 1;

    void Start()
    {
        Debug.Log("PlayManager.cs#Start: " + CategoryManager.SelectedCategoryName);
        StartCoroutine(GetQuestion());
    }

    IEnumerator GetQuestion()
    {
        Debug.Log("PlayManager.cs#GetQuestion");
        string url = $"https://www.zutto-oreno-turn.com/cdn/eigo/question/category/tweet/realDonaldTrump.json";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        QuestionParser data = JsonUtility.FromJson<QuestionParser>(request.downloadHandler.text);
        Questions = data.questions;
        MakePlayScreen();
    }

    void MakePlayScreen()
    {
        Debug.Log("PlayManager.cs#MakePlayScreen 1: " + Questions[CurrentQuestionNumber].sentence);
        string[] sentences = Questions[CurrentQuestionNumber].sentence.Split(' ');
        string[] shuffles = MakeShuffleSentences(sentences);
        string[] masks = MakeMaskedSentences(sentences, shuffles);
        AnswerText.GetComponent<Text>().text = string.Join(" ", masks);

        int x = -350, y = 135;
        for (int i = 0; i < ChoiceNumber; i++)
        {
            GameObject wordButton = Instantiate(WordButtonPrefab, new Vector3(x, y, 0), Quaternion.identity);
            wordButton.transform.SetParent(WordContent.transform, false);

            Text wordButtonText = wordButton.GetComponentInChildren<Text>();
            wordButtonText.text = shuffles[i];

            if (x < -200)
            {
                x += 130;
            }
            else
            {
                x = -350;
                y -= 70;
            }
            wordButton.GetComponent<Button>().onClick.AddListener(() => OnClickWordButton(wordButtonText.text));
        }
    }

    string[] MakeShuffleSentences(string[] sentences)
    {
        Debug.Log("PlayManager.cs#MakeShuffleSentences");
        string[] shuffles = new string[sentences.Length];
        Array.Copy(sentences, shuffles, sentences.Length);
        for (int i = 0; i < shuffles.Length; i++)
        {
            string tmp = shuffles[i];
            int randomIndex = UnityEngine.Random.Range(i, shuffles.Length);
            shuffles[i] = shuffles[randomIndex];
            shuffles[randomIndex] = tmp;
        }
        return shuffles;
    }

    string[] MakeMaskedSentences(string[] sentences, string[] shuffles)
    {
        Debug.Log("PlayManager.cs#MakeMaskedSentences 1");
        string[] masks = new string[sentences.Length];
        Array.Copy(sentences, masks, sentences.Length);
        for (int i = 0; i < ChoiceNumber; i++)
        {
            for (int j = 0; j < masks.Length; j++)
            {
                if (masks[j] == shuffles[i])
                {
                    masks[j] = "*****";
                    break;
                }
            }
        }
        int count = 1;
        for (int i = 0; i < masks.Length; i++)
        {
            if (masks[i] == "*****")
            {
                masks[i] = $"***({count})***";
                count++;
            }
        }
        return masks;
    }

    void OnClickWordButton(string word)
    {
        Debug.Log("PlayManager.cs#OnClickWordButton: " + word);

        string text = AnswerText.GetComponent<Text>().text;

        int maskLocation = text.IndexOf($"***({AnserNumber})***");
        int maskLength = maskLocation + word.Length + 1;
        int correctLength = Questions[CurrentQuestionNumber].sentence.Length;
        if (maskLength > correctLength) {
            maskLength = correctLength;    
        }

        string anser = text.Replace($"***({AnserNumber})***", word);
        string anserPart = anser.Substring(0, maskLength);
        string correctPart = Questions[CurrentQuestionNumber].sentence.Substring(0, maskLength);

        Debug.Log("anserPart: " + anserPart);
        Debug.Log("correctPart: " + correctPart);

        if (anserPart != correctPart)
        {
            Debug.Log("PlayManager.cs#OnClickWordButton Wrong !");
            return;
        }

        Debug.Log("PlayManager.cs#OnClickWordButton Correct !");
        AnswerText.GetComponent<Text>().text = anser;

        if (AnserNumber < ChoiceNumber) {
            AnserNumber++;
            return;
        }

        Debug.Log("PlayManager.cs#OnClickWordButton Clear ! 1 : " + CurrentQuestionNumber);
        Debug.Log("PlayManager.cs#OnClickWordButton Clear ! 2 : " + Questions.Length);

        // [todo] 正解したら正解文をみたいので、次の問題へボタンがほしい

        if (CurrentQuestionNumber < Questions.Length - 1)
        {
            CurrentQuestionNumber++;
            AnserNumber = 1;
            AnswerText.GetComponent<Text>().text = "";
            DestroyWordButton();
            MakePlayScreen();
            return;
        }

        Debug.Log("PlayManager.cs#OnClickWordButton All Clear !");
        SceneManager.LoadScene("Break");
    }

    bool IsCorrect(string word) {
        return false;
    }

    void DestroyWordButton()
    {
        Debug.Log("PlayManager.cs#DestroyWordButton 1");
        foreach (Transform children in WordContent.transform)
        {
            Debug.Log("PlayManager.cs#DestroyWordButton 2: " + children.gameObject.name);
            GameObject.Destroy(children.gameObject);
        }
    }
}
