using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLev : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyUp("p")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyUp("o")) {
            SceneManager.LoadScene(0);
        }
    }
}
