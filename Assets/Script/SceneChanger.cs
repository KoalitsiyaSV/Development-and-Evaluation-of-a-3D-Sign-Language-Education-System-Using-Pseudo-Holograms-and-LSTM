using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public int nextSceneIndex;

    public void SceneChangeByIndex()
    {
        SceneManager.LoadScene(nextSceneIndex);

    }
}
