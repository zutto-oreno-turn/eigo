using Eigo.Common;
using GoogleMobileAds.Api;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakManager : MonoBehaviour
{
    public GameObject MessageText;
    public GameObject NextButton;

    private BannerView bannerView;

    void Start()
    {
        MakeAdBanner();
        MakeMessageText();
        MakeNextButton();
    }

    void MakeAdBanner()
    {
        MobileAds.Initialize("ca-app-pub-3155583508878616~9975036833");
        string adUnitId = "ca-app-pub-3155583508878616/3502937602";
        bannerView = new BannerView(adUnitId, AdSize.MediumRectangle, AdPosition.Center);
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }

    void MakeMessageText() {
        TextMeshProUGUI messageTextMeshProUGUI = MessageText.GetComponentInChildren<TextMeshProUGUI>();
        if (SceneParameter.BreakReason == SceneParameter.Coffee) {
            messageTextMeshProUGUI.text = "Take a break by watching the advertisement (^.^)b";
        } else if (SceneParameter.BreakReason == SceneParameter.NoMore) {
            messageTextMeshProUGUI.text = "No more. Wait until it's tweeted >_<";
        }
    }

    void MakeNextButton() {
        if (SceneParameter.BreakReason == SceneParameter.NoMore) {
            NextButton.SetActive(false);
        }
    }

    public void OnClickNextButton()
    {
        SceneManager.LoadScene("Play");
        bannerView.Destroy();
    }
}
