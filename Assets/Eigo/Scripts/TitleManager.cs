using Eigo.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    void Start() {
        PlayData.LoadData();
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
