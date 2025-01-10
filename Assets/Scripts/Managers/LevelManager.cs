using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void LoadScene(string scene)
    {
        GameManager.instance.LoadState(scene);
        SceneManager.LoadScene(scene);
    }
}
