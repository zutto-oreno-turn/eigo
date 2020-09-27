using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingPanel : MonoBehaviour
{
    public void OnClickReturnButton() {
        Debug.Log("OnClickReturnButton from Setting");
        SceneManager.LoadScene("Title");
    }
}
