using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("BreakManager.cs#Start");
    }

    public void OnClickReturnButton()
    {
        Debug.Log("BreakManager.cs#OnClickReturnButton");
        SceneManager.LoadScene("Title");
    }
}
