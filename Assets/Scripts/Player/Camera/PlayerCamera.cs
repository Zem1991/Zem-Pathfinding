using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private PlayerController _pController;
    //private GameObject _telepointer;

    public new Camera camera { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _pController = GetComponentInParent<PlayerController>();
        if (!_pController) throw new MissingComponentException("Missing component: PlayerController");
        //_telepointer = GetComponentInChildren<SphereCollider>().gameObject;

        camera = GetComponentInChildren<Camera>();
        if (!camera) throw new MissingComponentException("Missing component: Camera");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveCamera(Vector3 movement)
    {
        Transform targetTransform = _pController.transform;

        Vector3 rotatedMovement = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * movement;
        targetTransform.Translate(rotatedMovement * Time.deltaTime, Space.World);
    }

    public void RotateCamera(Vector3 rotation)
    {
        Transform targetTransform = _pController.transform;

        float angleY = rotation.y * Time.deltaTime;
        targetTransform.RotateAround(targetTransform.position, Vector3.up, angleY);
    }
}
