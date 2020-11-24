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
    public GameObject MaskedImagePrefab;
    public GameObject RateText;
    public GameObject WordContent;
    public GameObject WordButtonPrefab;

    public Material MaskMaterial;

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

            if (masks[i].IndexOf($"***") > -1)
            {
                var t = sentenceText.GetComponent<TextMeshProUGUI>();
                Debug.Log("Mask0: " + t.rectTransform.rect);
                // float height = t.sizeDelta.y;
                // Debug.Log($"width: {width}, height: {height}");


                // TextMeshPro sentenceTextMeshPro = sentenceText.GetComponentInChildren<TextMeshPro>();

                // Debug.Log("Mask1a: " + $"{sentenceTextMeshPro}"); // Mask1a: Center: (0.0, 0.0, 0.0), Extents: (0.0, 0.0, 0.0)
                // Debug.Log("Mask1b: " + $"{sentenceText.transform.localPosition}"); // Mask1b: (171.5, -80.0, 0.0)
                // Debug.Log("Mask1c: " + $"{sentenceText.transform.localScale}"); // Mask1c: (1.0, 1.0, 1.0)

                // GameObject maskedImage = Instantiate(MaskedImagePrefab, new Vector3(sx, sy, 0), Quaternion.identity);
                // maskedImage.transform.SetParent(MaskPanel.transform, false);
                // var a = maskedImage.GetComponent<Image>();

                // TextMeshProUGUI maskedImageTextMeshProUGUI = maskedImage.GetComponentInChildren<TextMeshProUGUI>();
                // maskedImageTextMeshProUGUI.preferredWidth = sentenceTextMeshProUGUI.preferredWidth;

                // TextMeshPro textMeshPro = sentenceText.GetComponent<TextMeshPro>();
                // GameObject background = GameObject.CreatePrimitive(PrimitiveType.Plane);
                // background.name = "background";
                // background.transform.Rotate(-90, 0, 0);
                // background.transform.SetParent(sentenceText.transform);
                // background.GetComponent<MeshRenderer>().material = MaskMaterial;

                // var bounds = sentenceTextMeshProUGUI.bounds;
                // var pos = bounds.center;
                // var hoseiZ = 0.01f;
                // background.transform.localPosition = new Vector3(pos.x, pos.y, pos.z + hoseiZ);

                // var scale = bounds.extents;
                // background.transform.localScale = new Vector3((scale.x / 10 * 2), 1, (scale.y / 10 * 2));
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
        int maskLocation = AnswerText.IndexOf($"***({AnswerNumber})***");
        int maskLength = maskLocation + word.Length + 1;
        int correctLength = Questions[CurrentQuestionNumber].sentence.Length;
        if (maskLength > correctLength)
        {
            maskLength = correctLength;
        }

        string answer = AnswerText.Replace($"***({AnswerNumber})***", word);
        string anserPart = answer.Substring(0, maskLength);
        string correctPart = Questions[CurrentQuestionNumber].sentence.Substring(0, maskLength);

        Debug.Log("Part1: " + anserPart);
        Debug.Log("Part2: " + correctPart);

        if (anserPart != correctPart)
        {
            Debug.Log("PlayManager.cs#OnClickWordButton Wrong !");
            IsWrong = true;
            return;
        }
        Debug.Log("PlayManager.cs#OnClickWordButton Correct !");
        AnswerText = answer;

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
