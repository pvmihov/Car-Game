using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DriftScoreRender : MonoBehaviour {

    [SerializeField] CarController carController;
    [SerializeField] DriftScorer driftScorer;
    [SerializeField] TMP_Text TMPObject;
    [SerializeField] float verticalOffset;
    [SerializeField] float horizontalOffset;

    // Start is called before the first frame update
    void Start() {
        transform.position = carController.transform.position + Vector3.up * verticalOffset + Vector3.right * horizontalOffset;
    }

    // Update is called once per frame
    void Update() {
        transform.position = carController.transform.position + Vector3.up * verticalOffset + Vector3.right * horizontalOffset;
        int score = driftScorer.GetCarScore();
        if (score <= 0) {
            TMPObject.enabled = false;
        } else {
            TMPObject.enabled = true;
            TMPObject.text = score.ToString();
        }
    }
}