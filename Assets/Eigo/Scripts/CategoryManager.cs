using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CategoryManager : MonoBehaviour
{
    public static string SelectedCategoryName;
    // public GameObject CategoryPanel;
    // public GameObject CategoryButtonPrefab;

    void Start()
    {
        Debug.Log("CategoryManager.cs#Start");
        GameObject[] categoryButtons = GameObject.FindGameObjectsWithTag("CategoryButton");
        foreach (GameObject categoryButton in categoryButtons)
        {
            Debug.Log(categoryButton.name);
            categoryButton.GetComponent<Button>().onClick.AddListener(() => OnClickCategoryButton(categoryButton.name));
        }

        // [TODO] jsonからカテゴリ情報を取得して動的にカテゴリボタンを生成するようにする。
        // https://zutto-oreno-turn.github.io/cdn/eigo/question/category.json
        // GameObject categoryButton = Instantiate(CategoryButtonPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        // categoryButton.transform.SetParent(CategoryPanel.transform, false);

        // [TODO] jsonからカテゴリ情報を取得して件数をtextに設定する。
        // Text categoryButtonText = categoryButton.GetComponentInChildren<Text>();
        // Debug.Log(categoryButtonText.text);
    }

    void OnClickCategoryButton(string name)
    {
        Debug.Log("CategoryManager.cs#OnClickCategoryButton: " + name);
        SelectedCategoryName = name.Replace("CategoryButton_", "");
        SceneManager.LoadScene("Play");
    }
}
