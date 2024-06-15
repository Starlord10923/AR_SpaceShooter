using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class bl_Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Settings")]
    [SerializeField, Range(1, 15)] private float Radio = 5; // the ratio of the circumference of the joystick
    [SerializeField, Range(0.01f, 1)] private float SmoothTime = 0.5f; // return to default position speed
    [SerializeField, Range(0.5f, 4)] private float OnPressScale = 1.5f; // return to default position speed
    public Color NormalColor = new Color(1, 1, 1, 1);
    public Color PressColor = new Color(1, 1, 1, 1);
    [SerializeField, Range(0.1f, 5)] private float Duration = 1;

    [Header("Reference")]
    [SerializeField] private RectTransform StickRect; // The middle joystick UI
    [SerializeField] private RectTransform CenterReference;

    // Privates
    private Vector3 originalSize;
    private Vector3 DeathArea;
    private Vector3 currentVelocity;
    private bool isFree = false;
    private int lastId = -2;
    private Image stickImage;
    private Image backImage;
    private Canvas m_Canvas;
    private float diff;
    private Vector3 PressScaleVector;
    private bool IsDoubleClicked = false;

    // Double click settings
    private const float DOUBLE_CLICK_THRESHOLD = 0.5f;
    private float lastClickTime = 0f;

    public bool doubleClicked
    {
        get { return IsDoubleClicked; }
        private set { IsDoubleClicked = value; }
    }

    void Start()
    {
        originalSize = StickRect.localScale;
        if (StickRect == null)
        {
            Debug.LogError("Please add the stick for joystick work!.");
            this.enabled = false;
            return;
        }

        if (transform.root.GetComponent<Canvas>() != null)
        {
            m_Canvas = transform.root.GetComponent<Canvas>();
        }
        else if (transform.root.GetComponentInChildren<Canvas>() != null)
        {
            m_Canvas = transform.root.GetComponentInChildren<Canvas>();
        }
        else
        {
            Debug.LogError("Required at least one canvas for joystick work.!");
            this.enabled = false;
            return;
        }

        // Get the default area of joystick
        DeathArea = CenterReference.position;
        diff = CenterReference.position.magnitude;
        PressScaleVector = new Vector3(OnPressScale, OnPressScale, OnPressScale);
        if (GetComponent<Image>() != null)
        {
            backImage = GetComponent<Image>();
            stickImage = StickRect.GetComponent<Image>();
            backImage.CrossFadeColor(NormalColor, 0.1f, true, true);
            stickImage.CrossFadeColor(NormalColor, 0.1f, true, true);
        }
    }

    void Update()
    {
        DeathArea = CenterReference.position;
        // If this not free (not touched) then not need continue
        if (!isFree)
            return;

        // Return to default position with a smooth movement
        StickRect.position = Vector3.SmoothDamp(StickRect.position, DeathArea, ref currentVelocity, smoothTime);
        // When is in default position, we not need continue update this
        if (Vector3.Distance(StickRect.position, DeathArea) < .1f)
        {
            isFree = false;
            StickRect.position = DeathArea;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (lastId == -2)
        {
            lastId = data.pointerId;
            StopAllCoroutines();
            StartCoroutine(ScaleJoystick(true));
            OnDrag(data);
            if (backImage != null)
            {
                backImage.CrossFadeColor(PressColor, Duration, true, true);
                stickImage.CrossFadeColor(PressColor, Duration, true, true);
            }
        }

        // Double click detection
        if (Time.time - lastClickTime <= DOUBLE_CLICK_THRESHOLD)
        {
            doubleClicked = true;
        }
        lastClickTime = Time.time;
    }

    public void OnDrag(PointerEventData data)
    {
        if (data.pointerId == lastId)
        {
            isFree = false;
            Vector3 position = bl_JoystickUtils.TouchPosition(m_Canvas, GetTouchID);
            if (Vector2.Distance(DeathArea, position) < Radio)
            {
                StickRect.position = position;
            }
            else
            {
                StickRect.position = DeathArea + (position - DeathArea).normalized * Radio;
            }
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        isFree = true;
        currentVelocity = Vector3.zero;
        if (data.pointerId == lastId)
        {
            lastId = -2;
            StopAllCoroutines();
            StartCoroutine(ScaleJoystick(false));
            if (backImage != null)
            {
                backImage.CrossFadeColor(NormalColor, Duration, true, true);
                stickImage.CrossFadeColor(NormalColor, Duration, true, true);
            }
        }

        // Double click event handling
        if (doubleClicked)
        {
            // Trigger double click event
            OnDoubleClick?.Invoke();
            doubleClicked = false;
        }
    }

    IEnumerator ScaleJoystick(bool increase)
    {
        float time = 0;

        while (time < Duration)
        {
            Vector3 v = StickRect.localScale;
            if (increase)
            {
                v = Vector3.Lerp(StickRect.localScale, PressScaleVector, (time / Duration));
            }
            else
            {
                v = Vector3.Lerp(StickRect.localScale, originalSize, (time / Duration));
            }
            StickRect.localScale = v;
            time += Time.deltaTime;
            yield return null;
        }
    }

    public int GetTouchID
    {
        get
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.touches[i].fingerId == lastId)
                {
                    return i;
                }
            }
            return -1;
        }
    }

    private float smoothTime { get { return (1 - (SmoothTime)); } }

    public float Horizontal
    {
        get
        {
            return (StickRect.position.x - DeathArea.x) / Radio;
        }
    }

    public float Vertical
    {
        get
        {
            return (StickRect.position.y - DeathArea.y) / Radio;
        }
    }

    // Event declaration for double click
    public delegate void DoubleClickAction();
    public static event DoubleClickAction OnDoubleClick;
}
