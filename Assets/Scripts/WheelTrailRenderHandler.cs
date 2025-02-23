using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrailRenderHandler : MonoBehaviour {
    CarController carController;
    TrailRenderer trailRenderer;

    void Awake() {
        carController = GetComponentInParent<CarController>();
        trailRenderer = GetComponent<TrailRenderer>();

        trailRenderer.emitting = false;
    }

    void Update() {
        if (carController.IsSkidding()) {
            trailRenderer.emitting = true;
        } else {
            trailRenderer.emitting = false;
        }
    }
}