using System;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class CameraController : MonoBehaviour {
    float cameraScale;
    Vector3 cameraOffSet = new Vector3(0,0,-1);
    const int followSpeed = 5;

    [SerializeField] GameObject target;
    Rigidbody2D _targetRb;
    void Start() {
        _targetRb = target.GetComponent<Rigidbody2D>();
    }
    void Update() {
        Follow();
    }

    void Follow() {
        transform.position = Vector3.Lerp(transform.position, target.transform.position+cameraOffSet, followSpeed*Time.deltaTime);
    }
}
