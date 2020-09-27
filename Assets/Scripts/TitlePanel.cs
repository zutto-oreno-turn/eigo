using UnityEngine;
using UnityEngine.SceneManagement;

public class TitlePanel : MonoBehaviour
{
    void Start() {
        Debug.Log("TitlePanel#Start");
    }

    public void OnClickPlayButton() {
        Debug.Log("OnClickPlayButton");
        SceneManager.LoadScene("Play");
    }

    public void OnClickSettingButton() {
        Debug.Log("OnClickSettingButton");
        SceneManager.LoadScene("Setting");
    }
}
