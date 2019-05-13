using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("Key Config")]
    public KeyCode cameraMovFwd = KeyCode.W;
    public KeyCode cameraMovBck = KeyCode.S;
    public KeyCode cameraMovLft = KeyCode.A;
    public KeyCode cameraMovRgt = KeyCode.D;
    public KeyCode cameraRotLft = KeyCode.Q;
    public KeyCode cameraRotRgt = KeyCode.E;
    public KeyCode mouseSelect = KeyCode.Mouse0;
    public KeyCode mouseCommand = KeyCode.Mouse1;

    public Vector3 CameraMovementAxes()
    {
        float moveZ = 0;
        float moveX = 0;
        float moveY = 0;
        if (KeyPress(cameraMovFwd)) moveZ += 1;
        if (KeyPress(cameraMovBck)) moveZ -= 1;
        if (KeyPress(cameraMovLft)) moveX -= 1;
        if (KeyPress(cameraMovRgt)) moveX += 1;
        return new Vector3(moveX, moveY, moveZ).normalized;
    }

    public Vector3 CameraRotationAxes()
    {
        float rotX = 0;
        float rotY = 0;
        if (KeyPress(cameraRotLft)) rotY -= 1;
        if (KeyPress(cameraRotRgt)) rotY += 1;
        return new Vector3(rotX, rotY, 0).normalized;
    }

    public bool KeyPress(KeyCode key)
    {
        return Input.GetKey(key);
    }

    public bool KeyDown(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    public bool KeyUp(KeyCode key)
    {
        return Input.GetKeyUp(key);
    }
}
