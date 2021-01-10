using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("BreakManager.cs#Start");
    }

    public void OnClickNextButton()
    {
        Debug.Log("BreakManager.cs#OnClickNextButton");
        SceneManager.LoadScene("Play");
    }
}
