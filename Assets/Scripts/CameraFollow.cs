using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject observable;
    [SerializeField] float aheadSpeed;
    [SerializeField] float followDamping;

    Rigidbody2D observableRigidBody;

    private void Start() {
        observableRigidBody = observable.GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (observableRigidBody == null)
            return;
        
        Vector2 vct2 = observableRigidBody.velocity * aheadSpeed;
        Vector3 targetPosition = observable.transform.position + new Vector3(vct2.x,vct2.y,transform.position.z) ;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followDamping * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }
}
