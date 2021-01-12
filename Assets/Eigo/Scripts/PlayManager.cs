using DG.Tweening;
using Eigo.Models;
using GoogleMobileAds.Api;
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
    public GameObject NextPanel;

    const int SpacePx = 5;
    const string MaskString = "*******";

    BannerView bannerView;

    Question[] Questions;

    bool IsCorrect = true;
    int CurrentQuestionNumber = 0;
    int TotalCorrectQuestionNumber = 0;
    int TotalaAlreadyQuestionNumber = 0;
    int ChoiceNumber = 3;
    int AnswerNumber = 1;

    string[] Sentences;
    string[] Shuffles;
    string[] Masks;
    string[] Correct;

    void Start()
    {
        // MobileAds.Initialize("ca-app-pub-3940256099942544~3347511713"); // test
        MobileAds.Initialize("ca-app-pub-3155583508878616~9975036833"); // production

        LoadData();
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

    void LoadData()
    {
        // PlayerPrefs.DeleteAll(); // [memo] Debug Code
        CurrentQuestionNumber = PlayerPrefs.GetInt("CurrentQuestionNumber", 0);
        TotalCorrectQuestionNumber = PlayerPrefs.GetInt("TotalCorrectQuestionNumber", 0);
        TotalaAlreadyQuestionNumber = PlayerPrefs.GetInt("TotalaAlreadyQuestionNumber", 0);
    }

    void SaveData()
    {
        PlayerPrefs.SetInt("CurrentQuestionNumber", CurrentQuestionNumber);
        PlayerPrefs.SetInt("TotalCorrectQuestionNumber", TotalCorrectQuestionNumber);
        PlayerPrefs.SetInt("TotalaAlreadyQuestionNumber", TotalaAlreadyQuestionNumber);
    }

    void MakePlayPanel()
    {
        // Question Initialize
        IsCorrect = true;
        AnswerNumber = 1;
        Array.Resize(ref Correct, ChoiceNumber);

        // Make Data
        Sentences = Questions[CurrentQuestionNumber].sentence.Split(' ');
        MakeShuffleSentencesArray();
        MakeMaskedSentencesArray();
        MakeCorrectArray();

        // Make View
        MakeHeader();
        MakeSentencePanel();
        MakeRate();
        MakeWordContent();
        MakeNextPanel();
    }

    void MakeShuffleSentencesArray()
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

    void MakeMaskedSentencesArray()
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
                Correct[maskNumber] = Sentences[i];
                maskNumber++;
            }
        }
    }

    void MakeHeader()
    {
        TextMeshProUGUI questionNumberTextMeshProUGUI = QuestionNumberText.GetComponentInChildren<TextMeshProUGUI>();
        questionNumberTextMeshProUGUI.text = $"Question {String.Format("{0:#,0}", CurrentQuestionNumber + 1)}";

        TextMeshProUGUI dateTextMeshProUGUI = DateText.GetComponentInChildren<TextMeshProUGUI>();
        dateTextMeshProUGUI.text = Questions[CurrentQuestionNumber].date;
    }

    void MakeSentencePanel()
    {
        foreach (Transform children in SentencePanel.transform)
        {
            GameObject.Destroy(children.gameObject);
        }

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
            if (sx > SentencePanel.GetComponent<RectTransform>().rect.width)
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

    void MakeRate()
    {
        if (TotalaAlreadyQuestionNumber == 0)
        {
            return;
        }
        string rate = ((decimal)TotalCorrectQuestionNumber / TotalaAlreadyQuestionNumber).ToString("P2");
        string correct = String.Format("{0:#,0}", TotalCorrectQuestionNumber);
        string total = String.Format("{0:#,0}", TotalaAlreadyQuestionNumber);

        TextMeshProUGUI rateTextMeshProUGUI = RateText.GetComponentInChildren<TextMeshProUGUI>();
        rateTextMeshProUGUI.text = $"Rate: {rate} ({correct}/{total})";
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

            wordButton.name = $"WordButton{i}";

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
            wordButton.GetComponent<Button>().onClick.AddListener(() => OnClickWordButton(wordButton.name, wordButtonTextMeshProUGUI.text));
        }
    }

    void MakeNextPanel() {
        NextPanel.SetActive(false);
        GameObject nextButton = NextPanel.transform.Find("NextButton").gameObject;
        nextButton.GetComponent<Button>().onClick.AddListener(() => OnClickNextButton());
    }

    void OnClickWordButton(string name, string word)
    {
        GameObject wordButton = WordContent.transform.Find(name).gameObject;
        if (word != Correct[AnswerNumber - 1])
        {
            IsCorrect = false;

            Image image = wordButton.GetComponent<Image>();
            wordButton.GetComponent<Image>().color = new Color(1.0f, 0.4575472f, 0.4575472f);
            DOTween.To(
                () => image.color,
                color => image.color = color,
                new Color(1.0f, 1.0f, 1.0f),
                1.0f
            );
            return;
        }
        else
        {
            wordButton.GetComponent<Image>().color = new Color(0.6214992f, 0.9716981f, 0.5821022f);
        }

        AnswerNumber++;
        MakeSentencePanel();

        if (AnswerNumber <= ChoiceNumber)
        {
            return;
        }

        if (IsCorrect)
        {
            TotalCorrectQuestionNumber++;
        }

        TotalaAlreadyQuestionNumber++;
        CurrentQuestionNumber++;
        SaveData();
        MakeRate();

        NextPanel.SetActive(true);
        RequestBanner();
    }

    public void OnClickNextButton()
    {
        if (CurrentQuestionNumber < Questions.Length)
        {
            MakePlayPanel();
            bannerView.Destroy();
            return;
        } else {
            SceneManager.LoadScene("Break");
        }
    }

    void RequestBanner()
    {
        // string adUnitId = "ca-app-pub-3940256099942544/6300978111"; // test
        string adUnitId = "ca-app-pub-3155583508878616/3502937602"; // production
        bannerView = new BannerView(adUnitId, AdSize.IABBanner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }
}
