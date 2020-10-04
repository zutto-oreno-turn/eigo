using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("TitleManager.cs#Start");
    }

    public void OnClickPlayButton()
    {
        Debug.Log("TitleManager.cs#OnClickPlayButton");
        // SceneManager.LoadScene("Category");
        SceneManager.LoadScene("Play");
    }

    public void OnClickSettingButton()
    {
        Debug.Log("TitleManager.cs#OnClickSettingButton");
        SceneManager.LoadScene("Setting");
    }
}
