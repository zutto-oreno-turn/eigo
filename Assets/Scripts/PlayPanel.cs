using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayPanel : MonoBehaviour
{
    public void OnClickReturnButton() {
        Debug.Log("OnClickReturnButton from Play");
        SceneManager.LoadScene("Title");
    }
}
