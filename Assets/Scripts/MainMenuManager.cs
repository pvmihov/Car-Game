using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string[] names;
    public TextMeshProUGUI skinche;

    private void Start() {
        AudioManager.Instance.StopLoopingSound("Drifting");
    }

    public void Map1() {
        AudioManager.Instance.PlaySound("Button");
        SceneManager.LoadScene(1);
    }

    public void Map2() {
        AudioManager.Instance.PlaySound("Button");
        SceneManager.LoadScene(2);
    }

    public void SkinNum(int num) {
        AudioManager.Instance.PlaySound("Button");
        if (num < 10) {
            skinche.text = "Current skin: " + names[num];
            PlayerPrefs.SetInt("skin", num);
        }
        else if (num==10) {
            if (PlayerPrefs.GetInt("10") == 1) {
                skinche.text = "Current skin: " + names[num];
                PlayerPrefs.SetInt("skin", num);
            } else {
                skinche.text = "Receive a score above 10000 on Map 1 to receive " + names[num];
            }
        }
        else {
            if (PlayerPrefs.GetInt("11") == 1) {
                skinche.text = "Current skin: " + names[num];
                PlayerPrefs.SetInt("skin", num);
            } else {
                skinche.text = "Receive a score above 10000 on Map 2 to receive " + names[num];
            }
        }
    }
}
