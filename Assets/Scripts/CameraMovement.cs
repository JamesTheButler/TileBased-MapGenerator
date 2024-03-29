﻿using System;
using UnityEngine;

/// <summary>
/// Handles camera movement.
/// </summary>
public class CameraMovement : MonoBehaviour {
    public bool isCamLocked;
    public bool allowMouseMovement;
    public bool allowKeyMovement;

    public float cameraMoveSpeed;
    public float zoomSpeed;
    public float minZoom;
    public float maxZoom;
    public float moveAreaWidth;

    private Vector3 startPosition;
    private Vector2 screenSize;

    private Camera cam;

    //TODO make zoom function change with camera being ortho/perspective

    private void Start() {
        screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        startPosition = transform.position;
    }

    private void Awake() {
        cam = GetComponent<Camera>();
        MapGenerator.OnMapGenerationStarted += map => {
            SetCamera(new Vector3(map.size.x / 2f, map.size.y / 2f, transform.position.z), Math.Max(map.size.x, map.size.y) * 0.55f);
        };
    }

    private void SetCamera(Vector3 position, float size) {
        Debug.Log("OnMapGenerationStarted");
        transform.position = position;
        cam.orthographicSize = size;
        cameraMoveSpeed = size ;
    }

    private void Update() {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenRes = new Vector2(cam.pixelWidth, cam.pixelHeight);
        // toggle cam movement
        if (Input.GetKeyDown(KeyCode.C)) {
            ToggleIsCamLocked();
        }

        if (allowMouseMovement) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                cam.transform.position = startPosition;
            }
        }
        if (!isCamLocked && allowKeyMovement) {
            MoveCameraWithKeys(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
        if (!isCamLocked && allowMouseMovement) {
            if (IsMouseOnScreenEdge()) {
                MoveCameraWithMouse(mousePos, screenRes);
            }
        }

        ApplyZoom();
    }

    private void MoveCameraWithKeys(float h, float v) {
        Vector3 direction = new Vector3(h, v, 0);
        cam.transform.position += direction * cameraMoveSpeed * Time.deltaTime;
    }

    private void ApplyZoom() {
        if (cam.orthographic)
            cam.orthographicSize += -Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        else
            transform.position += new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * zoomSpeed);
    }

    /// <summary>
    /// Toggles if the camera is allowed to move.
    /// </summary>
    private void ToggleIsCamLocked() {
        isCamLocked = !isCamLocked;
    }

    private bool IsMouseOnScreenEdge() {
        Vector2 min = new Vector2(moveAreaWidth, moveAreaWidth);
        Vector2 max = new Vector2(cam.pixelWidth - moveAreaWidth, cam.pixelHeight - moveAreaWidth);
        Vector2 mousePos = Input.mousePosition;
        if (mousePos.x > max.x || mousePos.x < min.x || mousePos.y > max.y || mousePos.y < min.y)
            return true;
        return false;
    }

    /// <summary>
    /// Moves camera when mouse gets close to the edge of a screen.
    /// </summary>
    private void MoveCameraWithMouse(Vector2 mousePos, Vector2 screenRes) {
        Vector3 direction = new Vector3();
        if (screenRes.x - mousePos.x < moveAreaWidth)
            direction.x = 1;
        else if (mousePos.x < moveAreaWidth)
            direction.x = -1;

        if (screenRes.y - mousePos.y < moveAreaWidth)
            direction.y = 1;
        else if (mousePos.y < moveAreaWidth)
            direction.y = -1;
        Vector3.Normalize(direction);
        cam.transform.position += direction * cameraMoveSpeed * Time.deltaTime;
    }
}
