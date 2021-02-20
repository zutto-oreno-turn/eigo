using Eigo.Common;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    public GameObject RateText;

    void Start()
    {
        MakeRate();
    }

    void MakeRate()
    {
        TextMeshProUGUI rateTextMeshProUGUI = RateText.GetComponentInChildren<TextMeshProUGUI>();
        rateTextMeshProUGUI.text = PlayData.GetRate();
    }

    public void OnClickClearButton()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        PlayData.LoadData();
        MakeRate();
    }

    public void OnClickReturnButton()
    {
        SceneManager.LoadScene("Title");
    }
}
