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
    public GameObject SentencePanel;
    public GameObject SentenceTextPrefab;
    public GameObject MaskPanelPrefab;
    public GameObject RateText;
    public GameObject WordContent;
    public GameObject WordButtonPrefab;

    const int SpacePx = 5;

    Question[] Questions;

    bool IsWrong;
    int CurrentQuestionNumber = 0;
    int ChoiceNumber = 3;
    int AnswerNumber;
    int TotalQuestionNumber = 0;
    int TotalCorrectQuestionNumber = 0;

    void Start()
    {
        StartCoroutine(GetQuestion());
    }

    IEnumerator GetQuestion()
    {
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
        IsWrong = false;
        AnswerNumber = 1;
        foreach (Transform children in SentencePanel.transform)
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
        ClearPlayPanel();

        string[] sentences = Questions[CurrentQuestionNumber].sentence.Split(' ');
        string[] shuffles = MakeShuffleSentences(sentences);
        string[] masks = MakeMaskedSentences(sentences, shuffles);

        TextMeshProUGUI questionNumberTextMeshProUGUI = QuestionNumberText.GetComponentInChildren<TextMeshProUGUI>();
        questionNumberTextMeshProUGUI.text = $"Question {CurrentQuestionNumber + 1}";

        TextMeshProUGUI dateTextMeshProUGUI = DateText.GetComponentInChildren<TextMeshProUGUI>();
        dateTextMeshProUGUI.text = Questions[CurrentQuestionNumber].date;

        float maskPanelWidth = SentencePanel.GetComponent<RectTransform>().rect.width;
        float sx = 0, sy = 0;
        int count = 1;
        for (int i = 0; i < sentences.Length; i++)
        {
            GameObject sentenceText = Instantiate(SentenceTextPrefab, new Vector3(sx, sy, 0), Quaternion.identity);
            sentenceText.transform.SetParent(SentencePanel.transform, false);

            TextMeshProUGUI sentenceTextMeshProUGUI = sentenceText.GetComponentInChildren<TextMeshProUGUI>();
            sentenceTextMeshProUGUI.text = sentences[i];

            sx += sentenceTextMeshProUGUI.preferredWidth;
            if (sx > maskPanelWidth)
            {
                sx = sentenceTextMeshProUGUI.preferredWidth + SpacePx;
                sy -= 20;
                sentenceText.transform.localPosition = new Vector3(0, sy, 0);
            }
            else
            {
                sx += SpacePx;
            }

            if (masks[i] == "*****")
            {
                sentenceText.name = $"SentenceText_{count}";

                RectTransform sentenceTextRectTransform = sentenceText.GetComponent<RectTransform>();
                Vector3 sentenceTextPosition = new Vector3(sentenceTextRectTransform.localPosition.x, sentenceTextRectTransform.localPosition.y, 0);

                GameObject maskPanel = Instantiate(MaskPanelPrefab, sentenceTextPosition, Quaternion.identity);
                maskPanel.transform.SetParent(SentencePanel.transform, false);
                maskPanel.name = $"MaskPanel_{count}";

                RectTransform maskPanelRectTransform = maskPanel.GetComponent<RectTransform>();
                maskPanelRectTransform.sizeDelta = new Vector2(
                    sentenceTextMeshProUGUI.preferredWidth,
                    sentenceTextMeshProUGUI.preferredHeight
                );

                GameObject maskText = maskPanel.transform.Find("MaskText").gameObject;
                TextMeshProUGUI maskTextMeshProUGUI = maskText.GetComponentInChildren<TextMeshProUGUI>();
                maskTextMeshProUGUI.text = $"{count}";;
                count++;
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
        return masks;
    }

    void OnClickWordButton(string word)
    {
        GameObject sentenceText = SentencePanel.transform.Find($"SentenceText_{AnswerNumber}").gameObject;
        TextMeshProUGUI sentenceTextMeshProUGUI = sentenceText.GetComponentInChildren<TextMeshProUGUI>();

        if (word != sentenceTextMeshProUGUI.text)
        {
            Debug.Log("PlayManager.cs#OnClickWordButton Wrong !");
            IsWrong = true;
            return;
        }
        Debug.Log("PlayManager.cs#OnClickWordButton Correct !");

        if (AnswerNumber < ChoiceNumber)
        {
            AnswerNumber++;
            return;
        }

        // [todo] 正解文を表示してから次の問題に行く

        TotalQuestionNumber++;
        if (IsWrong == false)
        {
            TotalCorrectQuestionNumber++;
        }
        string rate = ((decimal)TotalCorrectQuestionNumber / TotalQuestionNumber).ToString("P2");
        TextMeshProUGUI rateTextMeshProUGUI = RateText.GetComponentInChildren<TextMeshProUGUI>();
        rateTextMeshProUGUI.text = $"Rate: {rate} ({TotalQuestionNumber}/{TotalCorrectQuestionNumber})";

        if (CurrentQuestionNumber < Questions.Length - 1)
        {
            CurrentQuestionNumber++;
            MakePlayPanel();
            return;
        }

        // [todo] ある程度クリアしたら休憩のための広告表示ページを表示する
        SceneManager.LoadScene("Break");
    }
}
