using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftScorer : MonoBehaviour {
    public CarScore cScore;
    public CarController cc;
    bool drift = false;
    float it = 9999999f;
    float koga;
    int klk;
    int lastAddedScore = 0;
    bool hit = false;

    private void LateUpdate() {
        if (!drift) {
            if (cc.IsSkidding()) {
                drift = true;
                it = Time.time;
                hit = false;
                AudioManager.Instance.PlayLoopingSound("Drifting");
            }
        } else {
            if (!cc.IsSkidding()) {
                if (Time.time < it) return;
                drift = false;
                float tm = Time.time - it;
                lastAddedScore = (int)Mathf.Pow(tm * 100f, 1.25f);
                klk = lastAddedScore;
                koga = Time.time;
                cScore.IncrScore(lastAddedScore);
                AudioManager.Instance.StopLoopingSound("Drifting");
            }
        }
    }

    public void Krai() {
        if (drift) {
            float tm = Time.time - it;
            lastAddedScore = (int)Mathf.Pow(tm * 100f, 1.25f);
            cScore.IncrScore(lastAddedScore);
            AudioManager.Instance.StopLoopingSound("Drifting");
        }
    }

    public int GetCarScore() {
        float tm = Time.time - it;
        if (hit) {
            return 0;
        }
        if (cc.IsSkidding()) {
            return (int)Mathf.Pow(tm * 100f, 1.25f);
        }
        if (Time.time - koga < 1) {
            return lastAddedScore;
        }
        return 0;
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        AudioManager.Instance.PlaySound("Crash");
        //Debug.Log("tuk");
        drift = false;
        it = 999999999999f;
        hit = true;
        AudioManager.Instance.StopLoopingSound("Drifting");
    }
}