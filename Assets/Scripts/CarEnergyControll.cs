using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarEnergyControll : MonoBehaviour
{
    public float energyMax = 10f;
    public float speed = 1f;
    public float recharge = 0.2f;
    float energy;
    public Transform energyBar;
    public SpriteRenderer lightRenderer;
    float it = 99999999999f;
    public float spFct = 3;
    public float accFct = 2;
    public float spdFct = 10;
    public float lngSup = 10f;
    bool sup = false;
    public CarController cc;
    public CarScore cscr;
    public DriftScorer drft;
    public GameObject superCan;

    private void Start() {
        energy = energyMax;
    }

    private void FixedUpdate() {
        if (sup && Time.time - it > lngSup) {
            sup = false;
            speed *= spFct;
            cc.accelerationFactor *= accFct;
            cc.maxSpeed *= spdFct;
            superCan.SetActive(false);
        AudioManager.Instance.MusicSpeed(1f);
        }
        energy -= speed * Time.fixedDeltaTime;
        if (energy <= 0) {
            drft.Krai();
            cscr.Krai();
            SceneManager.LoadScene(3);
        }
        energyBar.localScale = new Vector3 (energy/energyMax, energyBar.localScale.y, energyBar.localScale.z);
        lightRenderer.color = new Color(energy / energyMax, energy / energyMax, energy / energyMax, 1);
    }

    private void ActivateSuper() {
        if (sup) return;
        AudioManager.Instance.MusicSpeed(0.5f);
        superCan.SetActive(true);
        sup = true;
        it = Time.time;
        speed /= spFct;
        cc.accelerationFactor /= accFct;
        cc.maxSpeed /= spdFct;

    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.tag != "Battery") return;
        AudioManager.Instance.PlaySound("Battery");
        energy += energyMax * recharge;
        if (energy >= energyMax) { energy = energyMax; ActivateSuper(); }
        Destroy(collider.gameObject);
    }

}
