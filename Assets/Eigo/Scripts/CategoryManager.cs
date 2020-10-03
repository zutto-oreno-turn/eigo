using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CategoryManager : MonoBehaviour
{
    public GameObject CategoryPanel;
    public GameObject CategoryButtonPrefab;

    void Start() {
        Debug.Log("CategoryManager.cs#Start");
        GameObject categoryButton = Instantiate(CategoryButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        categoryButton.transform.SetParent(CategoryPanel.transform, false);
    }
}
