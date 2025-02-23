using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelParticleHandler : MonoBehaviour {
    float particleEmissionRate = 0;

    CarController carController;
    ParticleSystem particleSystemSmoke;
    ParticleSystem.EmissionModule particleSystemEmmisionModule;

    void Awake() {
        carController = GetComponentInParent<CarController>();
        particleSystemSmoke = GetComponentInParent<ParticleSystem>();

        particleSystemEmmisionModule = particleSystemSmoke.emission;



        particleSystemEmmisionModule.rateOverTime = 0;
    }

    void Update() {
        particleEmissionRate = Mathf.Lerp(particleEmissionRate, 0, Time.deltaTime * 5);
        particleSystemEmmisionModule.rateOverTime = particleEmissionRate;
        if (carController.IsSkidding()) {
            particleEmissionRate = carController.GetLateralVelocity().magnitude * 2;
        }
    }
}