using UnityEditor;
using UnityEngine;

public static class ClearPlayerPrefs
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    public static void Execute()
    {
        Debug.Log("Clear PlayerPrefs");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
