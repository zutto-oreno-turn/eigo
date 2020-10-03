using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingManager : MonoBehaviour
{
    void Start() {
        Debug.Log("SettingManager.cs#Start");
    }

    public void OnClickReturnButton() {
        Debug.Log("SettingManager.cs#OnClickReturnButton");
        SceneManager.LoadScene("Title");
    }
}
