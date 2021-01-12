// using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakManager : MonoBehaviour
{
    // private BannerView bannerView;
    void Start()
    {
        Debug.Log("BreakManager.cs#Start Android");
        // string appId = "ca-app-pub-3940256099942544~3347511713";
        // MobileAds.Initialize(appId);
        // this.RequestBanner();
    }

    // void RequestBanner()
    // {
    //     string adUnitId = "ca-app-pub-3940256099942544/6300978111";
    //     this.bannerView = new BannerView(adUnitId, AdSize.IABBanner, AdPosition.Bottom);
    //     AdRequest request = new AdRequest.Builder().Build();
    //     bannerView.LoadAd(request);
    // }

    public void OnClickNextButton()
    {
        Debug.Log("BreakManager.cs#OnClickNextButton");
        // bannerView.Destroy();
        SceneManager.LoadScene("Play");
    }
}
