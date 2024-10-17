using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSceneOnEnable : MonoBehaviour
{
    public string scene = null;

    public void OnEnable()
    {
        if (string.IsNullOrEmpty(scene))
        {
            scene = SceneManager.GetActiveScene().name;
        }

        SceneManager.LoadScene(scene);
    }
}
