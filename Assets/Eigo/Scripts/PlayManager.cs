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
    int Level = 1;

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
        string[] shuffledSentences = MakeShuffleSentences(sentences);
        string[] maskedSentences = MakeMaskedSentences(sentences, shuffledSentences);
        AnswerText.GetComponent<Text>().text = string.Join(" ", maskedSentences);

        int x = -350, y = 135;
        for (int i = 0; i < Level + 2; i++)
        {
            GameObject wordButton = Instantiate(WordButtonPrefab, new Vector3(x, y, 0), Quaternion.identity);
            wordButton.transform.SetParent(WordContent.transform, false);

            Text wordButtonText = wordButton.GetComponentInChildren<Text>();
            wordButtonText.text = shuffledSentences[i];

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
        string[] shuffledSentences = new string[sentences.Length];
        Array.Copy(sentences, shuffledSentences, sentences.Length);
        for (int i = 0; i < shuffledSentences.Length; i++)
        {
            string tmp = shuffledSentences[i];
            int randomIndex = UnityEngine.Random.Range(i, shuffledSentences.Length);
            shuffledSentences[i] = shuffledSentences[randomIndex];
            shuffledSentences[randomIndex] = tmp;
        }
        return shuffledSentences;
    }

    string[] MakeMaskedSentences(string[] sentences, string[] shuffled)
    {
        Debug.Log("PlayManager.cs#MakeMaskedSentences");
        string[] maskedSentences = new string[sentences.Length];
        Array.Copy(sentences, maskedSentences, sentences.Length);
        for (int i = 0; i < Level + 2; i++)
        {
            for (int j = 0; j < maskedSentences.Length; j++)
            {
                if (maskedSentences[j] == shuffled[i])
                {
                    maskedSentences[j] = "*****";
                    break;
                }
            }
        }
        int count = 1;
        for (int i = 0; i < maskedSentences.Length; i++)
        {
            if (maskedSentences[i] == "*****")
            {
                maskedSentences[i] = $"***({count})***";
                count++;
            }
        }
        return maskedSentences;
    }

    void OnClickWordButton(string word)
    {
        Debug.Log("PlayManager.cs#OnClickWordButton: " + word);
        string test = AnswerText.GetComponent<Text>().text + word;
        string correct = Questions[CurrentQuestionNumber].sentence.Substring(0, test.Length);

        if (test != correct)
        {
            Debug.Log("PlayManager.cs#OnClickWordButton Wrong 1: " + test);
            Debug.Log("PlayManager.cs#OnClickWordButton Wrong 2: " + correct);
            return;
        }

        Debug.Log("PlayManager.cs#OnClickWordButton Correct !");
        AnswerText.GetComponent<Text>().text = test + " ";

        if (test.Length == Questions[CurrentQuestionNumber].sentence.Length)
        {
            Debug.Log("PlayManager.cs#OnClickWordButton Clear ! 1 : " + CurrentQuestionNumber);
            Debug.Log("PlayManager.cs#OnClickWordButton Clear ! 2 : " + Questions.Length);
            if (CurrentQuestionNumber == Questions.Length - 1)
            {
                Debug.Log("PlayManager.cs#OnClickWordButton All Clear !");
                SceneManager.LoadScene("Break");
                return;
            }
            AnswerText.GetComponent<Text>().text = "";
            CurrentQuestionNumber++;
            DestroyWordButton();
            MakePlayScreen();
        }
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
