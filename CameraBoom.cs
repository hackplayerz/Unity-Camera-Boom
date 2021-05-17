using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraBoom : MonoBehaviour
{
    [Header("Camera setting")]
    [SerializeField] 
    private new Camera camera;
    [SerializeField, Range(0f, 100f)]
    [Tooltip("Between camera to target length.")]
    private float length = 10;
    
    [Header("Camera Pitch")]
    [SerializeField] 
    private float pitch = 0;
    [SerializeField, Range(0, 90)]
    private float pitchMax = 80;
    [SerializeField, Range(-90, 0)]
    private float pitchMin = -20;
    
    [Header("Zoom In")]
    [SerializeField, Range(0, 100f)]
    private float maxZoom = 30;
    [SerializeField, Range(0, 100f)]
    private float minZoom = 10;
    [SerializeField, Range(0, 100f)]
    private float zoomSpeed = 10;


    private float _currentPitchAngle;
    private const float _MAX_DISTANCE = 5f;
    private Vector3 _cameraPosition;
    private float _currentZoom;

    private void Start()
    {
        _currentZoom = length;
    }

    private void OnDrawGizmos()
    {
        var position = transform.position;

        Gizmos.color = Color.cyan;

        Gizmos.DrawSphere(position, 0.1f);
        Gizmos.DrawSphere(_cameraPosition, 0.1f);

        Gizmos.color = Color.red;

        Gizmos.DrawLine(position, _cameraPosition);
    }

    private void Update()
    {
        Zoom();
        Pitching();
        Boom();
    }
    
    /// <summary>
    /// Zoom in and out.
    /// </summary>
    private void Zoom()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        _currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        _currentZoom = Mathf.Clamp(_currentZoom, minZoom, maxZoom);
#endif
    }
    
    /// <summary>
    /// Camera boom.
    /// It's like spring arm.
    /// </summary>
    private void Boom()
    {
        var transformCache = transform;
        var position = transformCache.position;

        //(transformCache = transform).localRotation = Quaternion.Euler(pitch, 0, 0);

        var direction = -transformCache.forward;
        var maxCameraDistance = _currentZoom / 100f * _MAX_DISTANCE;

        var ray = new Ray(position, direction);
        var blocked = Physics.SphereCast(ray, 0.1f, out var hit, maxCameraDistance);

        _cameraPosition = blocked ? hit.point : position + direction * maxCameraDistance;

        var cameraTransformCache = camera.transform;

        cameraTransformCache.position = _cameraPosition;
        cameraTransformCache.LookAt(position);
    }

    private void Pitching()
    {
        _currentPitchAngle = ClampAngle(_currentPitchAngle, pitchMin, pitchMax);
        Quaternion rot = Quaternion.Euler(_currentPitchAngle,0,0);
        transform.localRotation = rot;
    }
    
    /// <summary>
    /// Set camera pitch value.
    /// </summary>
    /// <param name="value">Set pitch value.</param>
    public void AddPitch(float value)
    {
        _currentPitchAngle += value;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if ( angle < -360 )
            angle += 360;
        if ( angle > 360 )
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
