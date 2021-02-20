using Eigo.Common;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    void Start()
    {
        Input.backButtonLeavesApp = true;
        PlayData.LoadData();
        MobileAds.Initialize("ca-app-pub-3155583508878616~9975036833");
    }

    public void OnClickPlayButton()
    {
        SceneManager.LoadScene("Play");
    }

    public void OnClickSettingButton()
    {
        SceneManager.LoadScene("Setting");
    }
}
