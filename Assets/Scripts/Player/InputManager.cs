using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputHandler _ih;

    [Header("Settings")]
    public int cameraGridClamp = 5;
    public float camMovementSpd = 35;
    public float camRotationSpd = 150;

    [Header("Key Inputs")]
    public Vector3 camMovementAxes;
    public Vector3 camRotationAxes;
    public bool mouseSelectPress;
    public bool mouseSelectDown;
    public bool mouseSelectUp;
    public bool mouseCommandPress;
    public bool mouseCommandDown;
    public bool mouseCommandUp;

    // Start is called before the first frame update
    void Start()
    {
        _ih = GetComponent<InputHandler>();
        if (!_ih) throw new MissingComponentException("Missing component: InputHandler");
    }

    // Update is called once per frame
    void Update()
    {
        camMovementAxes = _ih.CameraMovementAxes();
        camRotationAxes = _ih.CameraRotationAxes();

        mouseSelectPress = _ih.KeyPress(_ih.mouseSelect);
        mouseSelectDown = _ih.KeyDown(_ih.mouseSelect);
        mouseSelectUp = _ih.KeyUp(_ih.mouseSelect);

        mouseCommandPress = _ih.KeyPress(_ih.mouseCommand);
        mouseCommandDown = _ih.KeyDown(_ih.mouseCommand);
        mouseCommandUp = _ih.KeyUp(_ih.mouseCommand);
    }
}
