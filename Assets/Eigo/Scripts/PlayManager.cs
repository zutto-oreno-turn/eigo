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
    const string MaskString = "*******";

    Question[] Questions;

    bool IsCorrect;
    int CurrentQuestionNumber = 0;
    int ChoiceNumber = 3;
    int AnswerNumber;
    int TotalQuestionNumber = 0;
    int TotalCorrectQuestionNumber = 0;

    string[] Sentences;
    string[] Shuffles;
    string[] Masks;
    string[] CorrectArray;

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

    void LoadData() {

    }

    void MakePlayPanel()
    {
        IsCorrect = true;
        AnswerNumber = 1;
        Array.Resize(ref CorrectArray, ChoiceNumber);

        Sentences = Questions[CurrentQuestionNumber].sentence.Split(' ');
        MakeShuffleSentences();
        MakeMaskedSentences();
        MakeCorrectArray();

        TextMeshProUGUI questionNumberTextMeshProUGUI = QuestionNumberText.GetComponentInChildren<TextMeshProUGUI>();
        questionNumberTextMeshProUGUI.text = $"Question {String.Format("{0:#,0}", CurrentQuestionNumber + 1)}";

        TextMeshProUGUI dateTextMeshProUGUI = DateText.GetComponentInChildren<TextMeshProUGUI>();
        dateTextMeshProUGUI.text = Questions[CurrentQuestionNumber].date;

        MakeSentencePanel();
        MakeWordContent();
    }

    void MakeShuffleSentences()
    {
        Shuffles = new string[Sentences.Length];
        Array.Copy(Sentences, Shuffles, Sentences.Length);
        for (int i = 0; i < Shuffles.Length; i++)
        {
            string tmp = Shuffles[i];
            int randomIndex = UnityEngine.Random.Range(i, Shuffles.Length);
            Shuffles[i] = Shuffles[randomIndex];
            Shuffles[randomIndex] = tmp;
        }
    }

    void MakeMaskedSentences()
    {
        Masks = new string[Sentences.Length];
        Array.Copy(Sentences, Masks, Sentences.Length);
        for (int i = 0; i < ChoiceNumber; i++)
        {
            for (int j = 0; j < Masks.Length; j++)
            {
                if (Masks[j] == Shuffles[i])
                {
                    Masks[j] = MaskString;
                    break;
                }
            }
        }
    }

    void MakeCorrectArray()
    {
        int maskNumber = 0;
        for (int i = 0; i < Masks.Length; i++)
        {
            if (Masks[i] == MaskString)
            {
                CorrectArray[maskNumber] = Sentences[i];
                maskNumber++;
            }
        }
    }

    void MakeSentencePanel()
    {
        foreach (Transform children in SentencePanel.transform)
        {
            GameObject.Destroy(children.gameObject);
        }

        float maskPanelWidth = SentencePanel.GetComponent<RectTransform>().rect.width;
        float sx = 0, sy = 0;
        int maskNumber = 1;
        for (int i = 0; i < Sentences.Length; i++)
        {
            bool isMask = Masks[i] == MaskString;
            bool isFixed = isMask && maskNumber >= AnswerNumber;

            GameObject sentenceText = Instantiate(SentenceTextPrefab, new Vector3(sx, sy, 0), Quaternion.identity);
            sentenceText.transform.SetParent(SentencePanel.transform, false);

            TextMeshProUGUI sentenceTextMeshProUGUI = sentenceText.GetComponentInChildren<TextMeshProUGUI>();

            if (isFixed)
            {
                sentenceTextMeshProUGUI.text = MaskString;
            }
            else
            {
                sentenceTextMeshProUGUI.text = Sentences[i];
            }

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

            if (isFixed)
            {
                RectTransform sentenceTextRectTransform = sentenceText.GetComponent<RectTransform>();
                Vector3 sentenceTextPosition = new Vector3(sentenceTextRectTransform.localPosition.x, sentenceTextRectTransform.localPosition.y, 0);

                GameObject maskPanel = Instantiate(MaskPanelPrefab, sentenceTextPosition, Quaternion.identity);
                maskPanel.transform.SetParent(SentencePanel.transform, false);

                RectTransform maskPanelRectTransform = maskPanel.GetComponent<RectTransform>();
                maskPanelRectTransform.sizeDelta = new Vector2(
                    sentenceTextMeshProUGUI.preferredWidth,
                    sentenceTextMeshProUGUI.preferredHeight
                );

                GameObject maskText = maskPanel.transform.Find("MaskText").gameObject;
                TextMeshProUGUI maskTextMeshProUGUI = maskText.GetComponentInChildren<TextMeshProUGUI>();
                maskTextMeshProUGUI.text = $"{maskNumber}"; ;
            }

            if (isMask)
            {
                maskNumber++;
            }
        }
    }

    void MakeWordContent()
    {
        foreach (Transform children in WordContent.transform)
        {
            GameObject.Destroy(children.gameObject);
        }

        float ax = -350, ay = 60;
        for (int i = 0; i < ChoiceNumber; i++)
        {
            GameObject wordButton = Instantiate(WordButtonPrefab, new Vector3(ax, ay, 0), Quaternion.identity);
            wordButton.transform.SetParent(WordContent.transform, false);

            TextMeshProUGUI wordButtonTextMeshProUGUI = wordButton.GetComponentInChildren<TextMeshProUGUI>();
            wordButtonTextMeshProUGUI.text = Shuffles[i];

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

    void OnClickWordButton(string word)
    {
        if (word != CorrectArray[AnswerNumber - 1])
        {
            Debug.Log("PlayManager.cs#OnClickWordButton Wrong !");
            IsCorrect = false;
            return;
        }
        else
        {
            Debug.Log("PlayManager.cs#OnClickWordButton Correct !");
        }

        if (AnswerNumber < ChoiceNumber)
        {
            AnswerNumber++;
            MakeSentencePanel();
            return;
        }

        // [todo] 正解文を表示してから次の問題に行く

        if (IsCorrect)
        {
            TotalCorrectQuestionNumber++;
        }
        TotalQuestionNumber++;

        string rate = ((decimal)TotalCorrectQuestionNumber / TotalQuestionNumber).ToString("P2");
        string correct = String.Format("{0:#,0}", TotalCorrectQuestionNumber);
        string total = String.Format("{0:#,0}", TotalQuestionNumber);

        TextMeshProUGUI rateTextMeshProUGUI = RateText.GetComponentInChildren<TextMeshProUGUI>();
        rateTextMeshProUGUI.text = $"Rate: {rate} ({correct}/{total})";

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
