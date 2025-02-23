using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputHandler : MonoBehaviour {
    [SerializeField] CarController carController;
    void Awake() {
        carController = GetComponent<CarController>();
    }

    float lsty = 0f;

    // Update is called once per frame
    void Update() {
        Vector2 inputVector = Vector2.zero;
        //Debug.Log(Input.GetAxis("Vertical"));
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");
        //Debug.Log(inputVector);

        if (lsty==0 && Input.GetAxis("Vertical")!=0 && lsty!=Input.GetAxis("Vertical")) {
            AudioManager.Instance.PlaySound("Gaz");
        }

        lsty = Input.GetAxis("Vertical");

        carController.SetInputVector(inputVector);
    }
}