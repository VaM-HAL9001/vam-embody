﻿using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class MotionControllerWithCustomPossessPoint
{
    public string name;
    public Transform offsetTransform;
    public Transform possessPointTransform;
    public Rigidbody customRigidbody;
    public Transform currentMotionControl { get; private set; }
    public string mappedControllerName { get; set; }
    private Func<Transform> _getMotionControl;
    private LineRenderer _lineRenderer;
    private Vector3 _baseOffset;
    private Vector3 _customOffset;
    private Vector3 _offsetRotation;
    private Vector3 _pointRotation;
    private bool _showLine;

    public Vector3 baseOffset
    {
        get { return _baseOffset; }
        set { _baseOffset = value; SyncOffset(); }
    }

    public Vector3 customOffset
    {
        get { return _customOffset; }
        set { _customOffset = value; SyncOffset(); }
    }

    public Vector3 offsetRotation
    {
        get { return _offsetRotation; }
        set { _offsetRotation = value; SyncOffset(); }
    }

    public Vector3 possessPointRotation
    {
        get { return _pointRotation; }
        set { _pointRotation = value; SyncOffset(); }
    }

    public Vector3 combinedOffset => _baseOffset + _customOffset;

    public bool showLine
    {
        get { return _showLine; }
        set { _showLine = value; UpdateLineRenderer(); }
    }

    private void SyncOffset()
    {
        if (currentMotionControl == null) return;
        offsetTransform.localPosition = combinedOffset;
        offsetTransform.localEulerAngles = _offsetRotation;
        possessPointTransform.localPosition = Quaternion.Euler(possessPointRotation) * combinedOffset - combinedOffset;
        possessPointTransform.localEulerAngles = possessPointRotation;
        UpdateLineRenderer();
    }

    public bool Connect()
    {
        currentMotionControl = _getMotionControl();
        if (currentMotionControl == null) return false;
        offsetTransform.SetParent(currentMotionControl, false);
        SyncOffset();
        return true;
    }


    private void UpdateLineRenderer()
    {
        if (currentMotionControl == null) return;

        if (!_showLine)
        {
            if(!ReferenceEquals(_lineRenderer, null))
                Object.Destroy(_lineRenderer);
        }

        if(ReferenceEquals(_lineRenderer, null))
            _lineRenderer = VisualCuesHelper.CreateLine(possessPointTransform.gameObject, new Color(0.8f, 0.8f, 0.5f, 0.8f), 0.002f, 3, false, true);

        _lineRenderer.SetPositions(new[]
        {
            Vector3.zero,
            offsetTransform.InverseTransformPoint(currentMotionControl.position),
            possessPointTransform.InverseTransformPoint(currentMotionControl.position),
        });
    }

    public static MotionControllerWithCustomPossessPoint Create(string motionControlName, Func<Transform> getMotionControl)
    {
        var offsetPointGameObject = new GameObject($"EmbodyPossessOffsetPoint_{motionControlName}");
        var possessPointGameObject = new GameObject($"EmbodyPossessPoint_{motionControlName}");

        var rb = possessPointGameObject.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.None;
        rb.isKinematic = true;

        possessPointGameObject.transform.SetParent(offsetPointGameObject.transform, false);

        return new MotionControllerWithCustomPossessPoint
        {
            name = motionControlName,
            _getMotionControl = getMotionControl,
            offsetTransform = offsetPointGameObject.transform,
            possessPointTransform = possessPointGameObject.transform,
            customRigidbody = rb,
        };
    }
}