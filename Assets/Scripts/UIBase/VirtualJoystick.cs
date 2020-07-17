//----------------------------------------------
//            ColaFramework
// Copyright © 2018-2049 ColaFramework 马三小伙儿
//----------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
using System;
#endif

[AddComponentMenu("UI/VirtualJoystick"), RequireComponent(typeof(RectTransform))]
public class VirtualJoystick : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    bool _isRelocation;
    /// <summary>
    /// 点击时是否设置摇杆位置到点击位置
    /// </summary>
    public bool IsRelocation
    {
        get { return _isRelocation; }
        set
        {
            _isRelocation = value;
        }
    }

    [SerializeField]
    bool _isAutoHide;
    /// <summary>
    /// 不操作时是否隐藏
    /// </summary>
    public bool IsAutoHide
    {
        get { return _isAutoHide; }
        set
        {
            _isAutoHide = value;
        }
    }

    [SerializeField]
    RectTransform _joystickGroup = null;
    public RectTransform JoystickGroup
    {
        get { return _joystickGroup; }
        set
        {
            _joystickGroup = value;
        }
    }


    [SerializeField, Tooltip("The child graphic that will be moved around")]
    RectTransform _joystickGraphic;
    public RectTransform JoystickGraphic
    {
        get { return _joystickGraphic; }
        set
        {
            _joystickGraphic = value;
            UpdateJoystickGraphic();
        }
    }

    [SerializeField]
    RectTransform _joystickBackUI = null;
    public RectTransform JoystickBack
    {
        get { return _joystickBackUI; }
        set
        {
            _joystickBackUI = value;
        }
    }

    [SerializeField]        //to display in inspector
    Vector2 _axis;

    [SerializeField, Tooltip("How fast the joystick will go back to the center")]
    float _spring = 25;
    public float Spring
    {
        get { return _spring; }
        set { _spring = value; }
    }

    [SerializeField, Tooltip("How close to the center that the axis will be output as 0")]
    float _deadZone = .1f;
    public float DeadZone
    {
        get { return _deadZone; }
        set { _deadZone = value; }
    }

    [Tooltip("Customize the output that is sent in OnValueChange")]
    public AnimationCurve outputCurve = new AnimationCurve(new Keyframe(0, 0, 1, 1), new Keyframe(1, 1, 1, 1));

    public JoystickMoveEvent onValueChange;

    public JoystickPressEvent onPress;
    public bool IsPressed
    {
        get { return _IsControlled; }
    }

    public Vector2 JoystickAxis
    {
        get
        {
            Vector2 outputPoint = _axis.magnitude > _deadZone ? _axis : Vector2.zero;
            float magnitude = outputPoint.magnitude;

            outputPoint *= outputCurve.Evaluate(magnitude);

            return outputPoint;
        }
        set { SetAxis(value); }
    }

    RectTransform _rectTransform;
    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = (JoystickGroup ?? transform as RectTransform);

            return _rectTransform;
        }
    }

    enum ControlType
    {
        ByDrag,
        ByHotkey,
    }
    bool _isDragging;
    int _draggingPointerId;
    bool _isControlledByHotkey;
    bool _IsControlled
    {
        get { return _isDragging || _isControlledByHotkey; }
    }

    [HideInInspector]
    bool dontCallEvent;

    KeyCode _hotkeyLeft = KeyCode.None;
    KeyCode _hotkeyUp = KeyCode.None;
    KeyCode _hotkeyRight = KeyCode.None;
    KeyCode _hotkeyDown = KeyCode.None;
    EventModifiers _hotkeyModifier = EventModifiers.None;

    public void SetHotkey(KeyCode left, KeyCode up, KeyCode right, KeyCode down, EventModifiers modifier)
    {
        _hotkeyLeft = left;
        _hotkeyUp = up;
        _hotkeyRight = right;
        _hotkeyDown = down;
        _hotkeyModifier = modifier;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsActive())
            return;

        if (_isDragging || _isControlledByHotkey)
            return;
        Vector3 pos = eventData.position;
        if (IsRelocation && JoystickGroup != null)
        {

            if (eventData.pressEventCamera != null)
            {
                pos = eventData.pressEventCamera.ScreenToWorldPoint(eventData.position);
                JoystickGroup.localPosition = JoystickGroup.parent.InverseTransformPoint(pos);
            }
            else
            {
                JoystickGroup.localPosition = JoystickGroup.parent.InverseTransformPoint(eventData.position);
            }
        }

        EventSystem.current.SetSelectedGameObject(gameObject, eventData);

        Vector2 newAxis = rectTransform.InverseTransformPoint(pos);
        newAxis.x /= rectTransform.sizeDelta.x * .5f;
        newAxis.y /= rectTransform.sizeDelta.y * .5f;

        SetAxis(newAxis);

        _draggingPointerId = eventData.pointerId;
        dontCallEvent = true;
        ProcessBeginControl(ControlType.ByDrag);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isDragging || _isControlledByHotkey)
            return;

        if (eventData.pointerId != _draggingPointerId)
            return;

        ProcessEndControl();
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (!IsActive())
            return;

        eventData.useDragThreshold = false;
    }

    private void ProcessBeginControl(ControlType controlType)
    {
        if (controlType == ControlType.ByDrag)
            _isDragging = true;
        else if (controlType == ControlType.ByHotkey)
        {
            _isControlledByHotkey = true;
        }

        onPress.Invoke(true);
        if (_isAutoHide && JoystickGroup != null)
        {
            JoystickGroup.gameObject.SetActive(true);
            JoystickBack.gameObject.SetActive(false);
        }
    }

    private void ProcessEndControl()
    {
        _isDragging = false;
        _isControlledByHotkey = false;

        onPress.Invoke(false);
        if (_isAutoHide && JoystickGroup != null)
        {
            JoystickGroup.gameObject.SetActive(false);
            JoystickGroup.gameObject.transform.localPosition = JoystickBack.gameObject.transform.localPosition;
            JoystickBack.gameObject.SetActive(true);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging || _isControlledByHotkey)
            return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(JoystickGroup ?? rectTransform, eventData.position, eventData.pressEventCamera, out _axis);

        // by jp 删除轻操作的慢速移动，统一移动
        //_axis.x /= rectTransform.sizeDelta.x * .5f;
        //_axis.y /= rectTransform.sizeDelta.y * .5f;

        SetAxis(_axis);

        dontCallEvent = true;
    }

    void OnDeselect()
    {
        _isDragging = false;
        _isControlledByHotkey = false;
    }

    bool CheckKeyMofidier(EventModifiers modifier)
    {
        if (modifier == EventModifiers.None)
            return true;

        if ((modifier & EventModifiers.Control) != 0)
            if (!(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
                return false;

        if ((modifier & EventModifiers.Alt) != 0)
            if (!(Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
                return false;

        if ((modifier & EventModifiers.Shift) != 0)
            if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                return false;

        return true;
    }

    void UpdateHotkey()
    {
        if (_isDragging)
            return;

        if (CheckKeyMofidier(_hotkeyModifier))
        {
            bool bLeft = _hotkeyLeft != KeyCode.None && Input.GetKey(_hotkeyLeft);
            bool bUp = _hotkeyUp != KeyCode.None && Input.GetKey(_hotkeyUp);
            bool bRight = _hotkeyRight != KeyCode.None && Input.GetKey(_hotkeyRight);
            bool bDown = _hotkeyDown != KeyCode.None && Input.GetKey(_hotkeyDown);

            bool bAnykey = bLeft || bUp || bRight || bDown;
            if (_isControlledByHotkey)
            {
                if (!bAnykey)
                {
                    ProcessEndControl();
                    dontCallEvent = true;
                    SetAxis(new Vector2(0, 0));
                }
            }
            else
            {
                if (bAnykey)
                    ProcessBeginControl(ControlType.ByHotkey);
            }

            if (_isControlledByHotkey)
            {
                int dx = (bLeft ? -1 : 0) + (bRight ? 1 : 0);
                int dy = (bUp ? 1 : 0) + (bDown ? -1 : 0);
                SetAxis(new Vector2(dx, dy));
                dontCallEvent = true;
            }
        }
    }

    void Update()
    {
        SetHotkey(KeyCode.A, KeyCode.W, KeyCode.D, KeyCode.S, 0);
        UpdateHotkey();

        if (_IsControlled)
            if (!dontCallEvent)
                if (onValueChange != null) onValueChange.Invoke(JoystickAxis);
    }

    void LateUpdate()
    {
        if (!_IsControlled)
            if (_axis != Vector2.zero)
            {
                Vector2 newAxis = _axis - (_axis * Time.unscaledDeltaTime * _spring);

                if (newAxis.sqrMagnitude <= .0001f)
                    newAxis = Vector2.zero;

                SetAxis(newAxis);
            }

        dontCallEvent = false;
    }

    void OnEnable()
    {
        ProcessEndControl();
    }
    void OnDisable()
    {
        ProcessEndControl();
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        UpdateJoystickGraphic();
    }
#endif

    public void SetAxis(Vector2 axis)
    {
        _axis = Vector2.ClampMagnitude(axis, 1);

        Vector2 outputPoint = _axis.magnitude > _deadZone ? _axis : Vector2.zero;
        float magnitude = outputPoint.magnitude;

        outputPoint *= outputCurve.Evaluate(magnitude);

        if (!dontCallEvent)
            if (onValueChange != null)
                onValueChange.Invoke(outputPoint);

        UpdateJoystickGraphic();
    }

    void UpdateJoystickGraphic()
    {
        if (_joystickGraphic)
            _joystickGraphic.localPosition = _axis * Mathf.Max(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y) * .5f;
    }

    [System.Serializable]
    public class JoystickMoveEvent : UnityEvent<Vector2> { }

    [System.Serializable]
    public class JoystickPressEvent : UnityEvent<bool> { }
}