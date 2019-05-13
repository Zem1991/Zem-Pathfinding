using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GridGenerator _grid;
    private InputManager _im;
    private PlayerCamera _pCamera;
    private PlayerMouse _pMouse;

    public int currentFloor = 0;

    // Start is called before the first frame update
    void Start()
    {
        _grid = GridGenerator.Singleton;
        if (!_grid) throw new MissingComponentException("Missing component: GridGenerator");

        _im = FindObjectOfType<InputManager>();
        if (!_im) throw new MissingComponentException("Missing component: InputManager");
        _pCamera = GetComponentInChildren<PlayerCamera>();
        if (!_pCamera) throw new MissingComponentException("Missing component: PlayerCamera");
        _pMouse = GetComponentInChildren<PlayerMouse>();
        if (!_pMouse) throw new MissingComponentException("Missing component: PlayerMouse");
    }

    // Update is called once per frame
    void Update()
    {
        CameraControl();
        MouseControl();
    }

    private void CameraControl()
    {
        _pCamera.MoveCamera(_im.camMovementAxes * _im.camMovementSpd);
        _pCamera.RotateCamera(_im.camRotationAxes * _im.camRotationSpd);

        Vector3 innerLimits = _grid.nodeSize * _im.cameraGridClamp;
        transform.position = _grid.ClampWorldPosToGrid(transform.position, innerLimits);
    }

    private void MouseControl()
    {
        _pMouse.RefreshData(_pCamera.camera, currentFloor, _im.mouseSelectPress, _im.mouseSelectUp);
    }
}
