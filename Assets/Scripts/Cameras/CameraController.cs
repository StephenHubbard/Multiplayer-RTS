using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform = null;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;
    [SerializeField] private Vector2 zoomLimits = Vector2.zero;

    private Vector2 previousInput;
    private Vector2 previousZoomInput;

    private Controls controls;

    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);

        controls = new Controls();

        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;

        controls.Player.Zoom.performed += SetPreviousZoomInput;
        controls.Player.Zoom.canceled += SetPreviousZoomInput;

        controls.Enable();
    }

    public void SetPlayerCameraStartPos()
    {
        UnitBase unitBase = FindObjectOfType<UnitBase>();

        playerCameraTransform.position = new Vector3(unitBase.transform.position.x, 20, unitBase.transform.position.z - 20);
    }

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority || !Application.isFocused) { return; }

        UpdateCameraPosition();

        ZoomInAndOut();
    }

    private void ZoomInAndOut()
    {
        if (previousZoomInput == Vector2.zero) { return; }

        Vector3 pos = playerCameraTransform.position;

        pos += new Vector3(0f, -previousZoomInput.y, previousZoomInput.y) * .5f * Time.deltaTime;

        pos.y = Mathf.Clamp(pos.y, zoomLimits.x, zoomLimits.y);

        playerCameraTransform.position = pos;

    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = playerCameraTransform.position;

        if (previousInput == Vector2.zero)
        {
            Vector3 cursorMovement = Vector3.zero;

            Vector2 cursorPosition = Mouse.current.position.ReadValue();

            if (cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;
            }
            else if (cursorPosition.y <= screenBorderThickness)
            {
                cursorMovement.z -= 1;
            }
            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            pos += cursorMovement.normalized * speed * Time.deltaTime;
        }
        else
        {
            pos += new Vector3(previousInput.x, 0f, previousInput.y) * speed * Time.deltaTime;
        }

        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

        playerCameraTransform.position = pos;
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previousInput = ctx.ReadValue<Vector2>();
    }

    private void SetPreviousZoomInput(InputAction.CallbackContext ctx)
    {
        previousZoomInput = ctx.ReadValue<Vector2>();
    }
}
