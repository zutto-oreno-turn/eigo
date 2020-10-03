using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CategoryManager : MonoBehaviour
{
    public GameObject CategoryPanel;
    public GameObject CategoryButtonPrefab;

    void Start()
    {
        Debug.Log("CategoryManager.cs#Start");

        // https://zutto-oreno-turn.github.io/cdn/eigo/question/category.json
        
        // GameObject categoryButton = Instantiate(CategoryButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        // categoryButton.transform.SetParent(CategoryPanel.transform, false);
        // categoryButton.GetComponent<Button>().onClick.AddListener(CallOnClick);
    }

    // void CallOnClick()
    // {
    //     Debug.Log("CategoryManager.cs#CallOnClick");
    //     MyOnClick();
    // }

    // void MyOnClick()
    // {
    //     Debug.Log("CategoryManager.cs#MyOnClick");
    // }
}
