using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingPanel : MonoBehaviour
{
    void Start() {
        Debug.Log("SettingPanel#Start");
    }

    public void OnClickReturnButton() {
        Debug.Log("OnClickReturnButton from Setting");
        SceneManager.LoadScene("Title");
    }
}
