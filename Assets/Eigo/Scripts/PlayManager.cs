using Eigo.Models;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayManager : MonoBehaviour
{
    public GameObject QuestionNumberText;
    public GameObject DateText;
    public GameObject MaskPanel;
    public GameObject SentenceTextPrefab;
    public GameObject RateText;
    public GameObject WordContent;
    public GameObject WordButtonPrefab;

    const int SpacePx = 5;

    Question[] Questions;

    bool IsWrong;
    int CurrentQuestionNumber = 0;
    int ChoiceNumber = 2;
    int AnswerNumber;
    int TotalQuestionNumber = 0;
    int TotalCorrectQuestionNumber = 0;
    string AnswerText;

    void Start()
    {
        Debug.Log("PlayManager.cs#Start");
        StartCoroutine(GetQuestion());
    }

    IEnumerator GetQuestion()
    {
        Debug.Log("PlayManager.cs#GetQuestion");
        string url = $"https://www.zutto-oreno-turn.com/cdn/eigo/question/category/tweet/CNN.json";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        QuestionParser data = JsonUtility.FromJson<QuestionParser>(request.downloadHandler.text);
        Questions = data.questions;

        MakePlayPanel();
    }

    void ClearPlayPanel()
    {
        Debug.Log("PlayManager.cs#ClearPlayPanel");
        TotalQuestionNumber++;
        IsWrong = false;
        AnswerText = "";
        AnswerNumber = 1;
        foreach (Transform children in MaskPanel.transform)
        {
            GameObject.Destroy(children.gameObject);
        }
        foreach (Transform children in WordContent.transform)
        {
            GameObject.Destroy(children.gameObject);
        }
    }

    void MakePlayPanel()
    {
        Debug.Log("PlayManager.cs#MakePlayPanel");
        ClearPlayPanel();

        string[] sentences = Questions[CurrentQuestionNumber].sentence.Split(' ');
        string[] shuffles = MakeShuffleSentences(sentences);
        string[] masks = MakeMaskedSentences(sentences, shuffles);
        AnswerText = string.Join(" ", masks);

        TextMeshProUGUI questionNumberTextMeshProUGUI = QuestionNumberText.GetComponentInChildren<TextMeshProUGUI>();
        questionNumberTextMeshProUGUI.text = $"Question {CurrentQuestionNumber + 1}";

        TextMeshProUGUI dateTextMeshProUGUI = DateText.GetComponentInChildren<TextMeshProUGUI>();
        dateTextMeshProUGUI.text = Questions[CurrentQuestionNumber].date;

        float maskPanelWidth = MaskPanel.GetComponent<RectTransform>().rect.width;
        float sx = 0, sy = 0;
        for (int i = 0; i < sentences.Length; i++)
        {
            GameObject sentenceText = Instantiate(SentenceTextPrefab, new Vector3(sx, sy, 0), Quaternion.identity);
            sentenceText.transform.SetParent(MaskPanel.transform, false);

            TextMeshProUGUI sentenceTextMeshProUGUI = sentenceText.GetComponentInChildren<TextMeshProUGUI>();
            sentenceTextMeshProUGUI.text = masks[i];

            sx += sentenceTextMeshProUGUI.preferredWidth;
            if (sx > maskPanelWidth) {
                sx = sentenceTextMeshProUGUI.preferredWidth + SpacePx;
                sy -= 20;
                sentenceText.transform.localPosition = new Vector3(0, sy, 0);
            } else {
                sx += SpacePx;
            }
        }

        float ax = -350, ay = 60;
        for (int i = 0; i < ChoiceNumber; i++)
        {
            GameObject wordButton = Instantiate(WordButtonPrefab, new Vector3(ax, ay, 0), Quaternion.identity);
            wordButton.transform.SetParent(WordContent.transform, false);

            TextMeshProUGUI wordButtonTextMeshProUGUI = wordButton.GetComponentInChildren<TextMeshProUGUI>();
            wordButtonTextMeshProUGUI.text = shuffles[i];

            if (ax < -200)
            {
                ax += 130;
            }
            else
            {
                ax = -350;
                ay -= 70;
            }
            wordButton.GetComponent<Button>().onClick.AddListener(() => OnClickWordButton(wordButtonTextMeshProUGUI.text));
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
        Debug.Log("PlayManager.cs#MakeMaskedSentences");
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
        int maskLocation = AnswerText.IndexOf($"***({AnswerNumber})***");
        int maskLength = maskLocation + word.Length + 1;
        int correctLength = Questions[CurrentQuestionNumber].sentence.Length;
        if (maskLength > correctLength) {
            maskLength = correctLength;    
        }

        AnswerText = AnswerText.Replace($"***({AnswerNumber})***", word);
        string anserPart = AnswerText.Substring(0, maskLength);
        string correctPart = Questions[CurrentQuestionNumber].sentence.Substring(0, maskLength);

        Debug.Log("anserPart: " + anserPart);
        Debug.Log("correctPart: " + correctPart);

        if (anserPart != correctPart)
        {
            Debug.Log("PlayManager.cs#OnClickWordButton Wrong !");
            IsWrong = true;
            return;
        }

        Debug.Log("PlayManager.cs#OnClickWordButton Correct !");

        if (AnswerNumber < ChoiceNumber) {
            AnswerNumber++;
            return;
        }

        Debug.Log("PlayManager.cs#OnClickWordButton Clear !");

        // [todo] 正解したら正解文をみたいので、次の問題へボタンがほしい

        if (IsWrong == false) {
            TotalCorrectQuestionNumber++;
        }

        // [todo] 割合計算がうまくいかん
        // decimal rate = ((decimal) TotalCorrectQuestionNumber / TotalQuestionNumber) * 100;
        // TextMeshProUGUI rateTextMeshProUGUI = RateText.GetComponentInChildren<TextMeshProUGUI>();
        // rateTextMeshProUGUI.text = $"Rate: {rate}% ({TotalQuestionNumber}/{TotalCorrectQuestionNumber})";

        if (CurrentQuestionNumber < Questions.Length - 1)
        {
            CurrentQuestionNumber++;
            MakePlayPanel();
            return;
        }

        Debug.Log("PlayManager.cs#OnClickWordButton All Clear !");
        SceneManager.LoadScene("Break");
    }
}
