using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarScore : MonoBehaviour
{
    public TextMeshProUGUI txt;
    int score=0;
    float it = -10f;
    public float howm = 0.5f;
    int kkk;
    public int level = 1;

    private void FixedUpdate() {
        if (Time.time - it > howm) txt.text = score.ToString();
        else txt.text = "+"+kkk.ToString();
    }

    public void Krai() {
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetInt("score", score);
    }

    public void IncrScore(int klk) {
        score += klk;
        it = Time.time;
        kkk = klk;
    }
}
