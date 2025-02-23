using UnityEngine;

public class CarController : MonoBehaviour {
    [Header("Car Settings")]
    public float accelerationFactor = 50f; // Acceleration force
    public float maxSpeed = 20f; // Maximum forward speed
    [SerializeField] private float turnFactor = 3.5f; // Steering sensitivity
    [SerializeField] private float driftFactor = 0.9f; // Rear tire grip loss during drift
    [SerializeField] private float angularFriction = 0.95f; // Angular momentum decay rate
    [SerializeField] private float angularForceFactor = 1.5f; // Drift-induced angular force
    [SerializeField] private float angularStabilization = 0.2f; // Stabilization over time
    [SerializeField] float skidThreshold = 1.5f;


    private float accelerationInput;
    private float steeringInput;

    private float rotationAngle;

    private Rigidbody2D carRigidbody2D;
    private AudioSource engineAudioSource;

    private void Awake() {
        carRigidbody2D = GetComponent<Rigidbody2D>();
        carRigidbody2D.inertia = 0.5f; // Reduced inertia for responsive angular control
        AudioManager.Instance.PlayLoopingSound("Engine");
        engineAudioSource = AudioManager.Instance.GetAudioSource("Engine");
    }

    private void FixedUpdate() {
        ApplyEngineForce();
        ApplySteering();
        SimulateDrift();
        UpdateEngineSound();
    }

    private void OnDestroy() {
        AudioManager.Instance.StopLoopingSound("Engine");
    }

    private void ApplyEngineForce() {
        // Forward velocity relative to the car's direction
        float velocityVsUp = Vector2.Dot(transform.up, carRigidbody2D.velocity);

        // Limit forward speed
        if (velocityVsUp > maxSpeed && accelerationInput > 0)
            return;

        // Limit reverse speed
        if (velocityVsUp < -maxSpeed * 0.5f && accelerationInput < 0)
            return;

        // Apply engine force in the car's forward direction (instant response to input)
        Vector2 engineForce = transform.up * accelerationInput * accelerationFactor;
        carRigidbody2D.AddForce(engineForce, ForceMode2D.Force);
    }

    private void ApplySteering() {
        // Reduce steering effect at high speeds
        float speedFactor = Mathf.Clamp01(carRigidbody2D.velocity.magnitude / maxSpeed);

        // Calculate steering input and apply angular velocity
        float steering = -steeringInput * turnFactor * speedFactor;
        carRigidbody2D.angularVelocity = steering;
    }

    private void SimulateDrift() {
        // Separate velocity into forward and lateral components
        Vector2 forwardVelocity = transform.up * Vector2.Dot(carRigidbody2D.velocity, transform.up);
        Vector2 lateralVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);

        // Reduce lateral velocity (simulate rear tires losing grip)
        lateralVelocity *= driftFactor;

        // Combine forward and lateral velocities back into the car's velocity
        carRigidbody2D.velocity = forwardVelocity + lateralVelocity;

        // Preserve angular momentum after steering input stops
        if (steeringInput == 0) {
            // Angular velocity decays gradually
            carRigidbody2D.angularVelocity *= angularFriction;
        } else {
            // Add angular force only when actively steering
            float driftForce = Vector2.Dot(carRigidbody2D.velocity, transform.right);
            carRigidbody2D.angularVelocity += driftForce * angularForceFactor;
        }

        // Stabilize angular velocity over time to prevent infinite spinning
        carRigidbody2D.angularVelocity *= (1f - angularStabilization * Time.fixedDeltaTime);
    }

    public bool IsSkidding() {
        Vector2 lateralVelocity = transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);
        return lateralVelocity.magnitude > skidThreshold;
    }

    public Vector3 GetLateralVelocity() {
        return transform.right * Vector2.Dot(carRigidbody2D.velocity, transform.right);
    }

    public void SetInputVector(Vector2 inputVector) {
        accelerationInput = inputVector.y;
        steeringInput = inputVector.x;
        if (accelerationInput < 0) steeringInput *= -1;
    }

    void UpdateEngineSound() {
        float velocityMagnitude = carRigidbody2D.velocity.magnitude;
        float desiredEngineVolume = velocityMagnitude * 0.01f;
        desiredEngineVolume = Mathf.Clamp(desiredEngineVolume, 0.2f, 1.0f);

        engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, desiredEngineVolume, Time.deltaTime * 10);

        float desiredEnginePitch = velocityMagnitude * 0.04f;
        desiredEnginePitch = Mathf.Clamp(desiredEnginePitch, 0.5f, 2.0f);

        engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, desiredEnginePitch, Time.deltaTime * 1.5f);
    }

}