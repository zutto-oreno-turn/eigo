using DG.Tweening;
using Eigo.Common;
using Eigo.Models;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public GameObject AdPanel;
    public GameObject MessageText;

    const int SpacePx = 5;
    const string MaskString = "*******";

    BannerView BannerView;

    Question[] Questions;

    bool IsCorrect = true;
    int CurrentQuestionNumberAfterStart = 0;
    int ChoiceNumber = 3;
    int AnswerNumber = 1;

    string[] Sentences;
    string[] Shuffles;
    string[] Masks;
    string[] Correct;

    void Start()
    {
        NextPanel.SetActive(false);
        AdPanel.SetActive(false);

        GameObject nextButtonNextPanel = NextPanel.transform.Find("NextButtonNextPanel").gameObject;
        nextButtonNextPanel.GetComponent<Button>().onClick.AddListener(() => OnClickNextButtonNextPanel());

        GameObject nextButtonAdPanel = AdPanel.transform.Find("NextButtonAdPanel").gameObject;
        nextButtonAdPanel.GetComponent<Button>().onClick.AddListener(() => OnClickNextButtonAdPanel());

        StartCoroutine(GetQuestion());

        BannerView = new BannerView("ca-app-pub-3155583508878616/3502937602", AdSize.MediumRectangle, AdPosition.Center);
        BannerView.Hide();

        AdRequest request = new AdRequest.Builder().Build();
        BannerView.LoadAd(request);
    }

    IEnumerator GetQuestion()
    {
        string url = $"https://www.zutto-oreno-turn.com/cdn/eigo/question/category/tweet/CNN.json";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        QuestionParser data = JsonUtility.FromJson<QuestionParser>(request.downloadHandler.text);
        Questions = data.questions;

        if (PlayData.CurrentQuestionNumber >= Questions.Length)
        {
            MakeWaitScreen();
        }
        else
        {
            MakePlayPanel();
        }
    }

    void MakePlayPanel()
    {
        // Question Initialize
        IsCorrect = true;
        AnswerNumber = 1;
        Array.Resize(ref Correct, ChoiceNumber);

        // Make Data
        Sentences = Questions[PlayData.CurrentQuestionNumber].sentence.Split(' ');
        MakeShuffleSentencesArray();
        MakeMaskedSentencesArray();
        MakeCorrectArray();

        // Make View
        MakeHeader();
        MakeSentencePanel();
        MakeRate();
        MakeWordContent();
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
        questionNumberTextMeshProUGUI.text = $"Question {String.Format("{0:#,0}", PlayData.CurrentQuestionNumber + 1)}";

        TextMeshProUGUI dateTextMeshProUGUI = DateText.GetComponentInChildren<TextMeshProUGUI>();
        dateTextMeshProUGUI.text = Questions[PlayData.CurrentQuestionNumber].date;
    }

    void MakeSentencePanel()
    {
        RemoveSentencePanel();

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
        TextMeshProUGUI rateTextMeshProUGUI = RateText.GetComponentInChildren<TextMeshProUGUI>();
        rateTextMeshProUGUI.text = PlayData.GetRate();
    }

    void MakeWordContent()
    {
        RemoveWordContent();

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

    void RemoveSentencePanel()
    {
        foreach (Transform children in SentencePanel.transform)
        {
            GameObject.Destroy(children.gameObject);
        }
    }

    void RemoveWordContent()
    {
        foreach (Transform children in WordContent.transform)
        {
            GameObject.Destroy(children.gameObject);
        }
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
            PlayData.TotalCorrectQuestionNumber++;
        }

        PlayData.TotalaAlreadyQuestionNumber++;
        PlayData.CurrentQuestionNumber++;
        CurrentQuestionNumberAfterStart++;
        SaveData();
        MakeRate();

        NextPanel.SetActive(true);
    }

    void SaveData()
    {
        PlayerPrefs.SetInt("CurrentQuestionNumber", PlayData.CurrentQuestionNumber);
        PlayerPrefs.SetInt("TotalCorrectQuestionNumber", PlayData.TotalCorrectQuestionNumber);
        PlayerPrefs.SetInt("TotalaAlreadyQuestionNumber", PlayData.TotalaAlreadyQuestionNumber);
    }

    public void OnClickNextButtonNextPanel()
    {
        NextPanel.SetActive(false);

        if (PlayData.CurrentQuestionNumber >= Questions.Length)
        {
            MakeWaitScreen();
            return;
        }

        if (CurrentQuestionNumberAfterStart > 4)
        {
            MakeAdScreen();
            return;
        }

        MakePlayPanel();
    }

    void MakeWaitScreen()
    {
        MakeMessageText("Please wait until there is a tweet.");
        MakeAdPanel();
    }

    void MakeAdScreen()
    {
        MakeMessageText("Press next again.");
        MakeAdPanel();
    }

    void MakeAdPanel()
    {
        CurrentQuestionNumberAfterStart = 0;
        RemoveSentencePanel();
        RemoveWordContent();
        AdPanel.SetActive(true);
        BannerView.Show();
    }

    void MakeMessageText(string message)
    {
        TextMeshProUGUI textmeshpro = MessageText.GetComponentInChildren<TextMeshProUGUI>();
        textmeshpro.text = message;
    }

    public void OnClickNextButtonAdPanel()
    {
        BannerView.Hide();

        if (PlayData.CurrentQuestionNumber >= Questions.Length)
        {
            SceneManager.LoadScene("Title");
            return;
        }

        AdPanel.SetActive(false);
        MakePlayPanel();
    }
}
