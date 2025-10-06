using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject Target;
    [SerializeField] private GameObject Cursor;
    [SerializeField] private GameObject Ball;
    [SerializeField] private GameObject CameraArm;
    [SerializeField] private ChangeSnowProperties SnowField;

    [Header("Parameters")]
    [SerializeField] private float CursorInterpSpeed = 70f;
    [Range(0f, 1f)]
    [SerializeField] private float EdgeMoveDistance = 0.1f;

    private PlayerMovement m_PlayerMovement;
    private Camera m_AttachedCamera;
    private Transform m_targetTransformCache;

    private float m_ZoomValue = 0.5f;
    private float m_CameraArmLength = 30f;

    [Header("Camera")]
    [SerializeField] private AnimationCurve ZoomCurve;
    [SerializeField] private float CameraFollowSpeed;
    [SerializeField] private Vector2 ZoomLerp = new Vector2(15, 100);
    [SerializeField] private Vector2 AngleLerp = new Vector2(-35, -50);
    [SerializeField] private Vector2 FowLerp = new Vector2(20, 15);
    [SerializeField] private Vector2 SizeLerp = new Vector2(2, 50);

    [Header("Cursor")]
    [SerializeField] private Vector3 CursorSize = new Vector3(0.4f, 1.0f, 0.4f);

    private GameObject m_HoverObject;

    public Vector3 ShootTarget
    {
        get
        {
            if (m_HoverObject)
            {
                return m_HoverObject.transform.position;
            }
            return Cursor.transform.position;
        }
    }

    public GameObject ShooterObject => Ball;


    void Awake()
    {
        GameObject targetCacheGO = new GameObject("TargetCache");
        m_targetTransformCache = targetCacheGO.transform;
        targetCacheGO.hideFlags = HideFlags.HideInHierarchy;
    }

    void Start()
    {
        m_PlayerMovement = GetComponent<PlayerMovement>();

        Target.GetComponent<InteractableHover>().HoverEvent += OnInteractableHover;
        m_AttachedCamera = Camera.main;
        if (m_AttachedCamera != null)
        {
            InvokeRepeating("MoveTracking", 0f, 1f / 60f);
        }
    }

    void Update()
    {
        UpdateCursorPosition();
        UpdateCameraMovement();
        UpdateCameraRotation();

        UpdateZoom();
        UpdateDOF();
    }

    void UpdateCameraMovement()
    {
        Vector3 playerPosition = transform.position;
        playerPosition.y = Ball.transform.position.y;

        Vector3 edgeMoveInput = EdgeMove();
        Vector3 followMoveInput = CameraFollow();

        float edgeWeight = EdgeInputWeight(playerPosition);
        m_PlayerMovement.AddMovementInput(followMoveInput + edgeMoveInput * edgeWeight);

        transform.position = playerPosition;
    }

    void UpdateCameraRotation()
    {
        if (Keyboard.current.qKey.isPressed)
        {
            m_PlayerMovement.AddRotationInput(new Vector3(0f, 1f, 0f));
        }
        if (Keyboard.current.eKey.isPressed)
        {
            m_PlayerMovement.AddRotationInput(new Vector3(0f, -1f, 0f));
        }   
    }

    float EdgeInputWeight(Vector3 cameraPosition)
    {
        Vector3 offset = cameraPosition - Ball.transform.position;
        return 1 - Mathf.Clamp01(offset.magnitude / (Ball.transform.localScale.x * 5f));
    }

    Vector3 ClampCameraOffset(Vector3 cameraPosition)
    {
        float maxDistance = Ball.transform.localScale.x * 10f;
        Vector3 offset = cameraPosition - Ball.transform.position;
        if (offset.magnitude > maxDistance)
        {
            return Ball.transform.position + offset.normalized * maxDistance;
        }
        return cameraPosition;
    }


    void MoveTracking()
    {
        Vector3? mouseWorldPos = ProjectMouseOnGroundPlane();
        if (mouseWorldPos.HasValue)
        {
            Target.transform.position = mouseWorldPos.Value;
        }
    }

    Vector3? ProjectMouseOnGroundPlane()
    {
        Ray ray = m_AttachedCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        float height = GetProjectPlanePoint();
        Plane plane = new Plane(Vector3.up, new Vector3(0, height, 0));

        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        return null;
    }

    void UpdateCursorPosition()
    {
        if (m_HoverObject == null)
        {
            m_targetTransformCache.position = Target.transform.position + new Vector3(0f, 0.3f, 0f);
            m_targetTransformCache.rotation = Target.transform.rotation;
            m_targetTransformCache.localScale = CursorSize;
        }
        else
        {
            Vector3 hoverBounds = m_HoverObject.GetComponent<Collider>().bounds.size * 0.15f;
            Vector3 newPosition = m_HoverObject.transform.position;
            Vector3 scaleDelta = Vector3.one;
            scaleDelta.x = scaleDelta.z = Mathf.Sin(Time.time * 5.0f) * 0.10f;
            Vector3 newlocalScale = new Vector3(hoverBounds.x, 1f, hoverBounds.z) + scaleDelta;

            newPosition.y = Target.transform.position.y + 0.3f;
            m_targetTransformCache.position = newPosition;
            m_targetTransformCache.rotation = Quaternion.Euler(0f, 0f, 0f);
            m_targetTransformCache.localScale = newlocalScale;
            

        }
        (Vector3 newPos, Quaternion newRot, Vector3 newScale) = TransformInterpTo(Cursor.transform, m_targetTransformCache, Time.deltaTime, CursorInterpSpeed);
        Cursor.transform.position = newPos;
        Cursor.transform.rotation = newRot;
        Cursor.transform.localScale = newScale;
        

    }

    float GetProjectPlanePoint()
    {
        return SnowField.NewHeight * SnowField.transform.localScale.y + 0.5f;
    }

    Vector3 CameraFollow()
    { 
        Vector3 moveDirection = Ball.transform.position - transform.position;
        moveDirection.y = 0;
        if (moveDirection.magnitude < Ball.transform.localScale.x * 2) return new Vector3(0f, 0f, 0f);
        return moveDirection.normalized;
    }

    Vector3 EdgeMove()
    {
        if (EdgeMoveDistance == 0) return Vector3.zero;

        float EdgeMoveDistanceAbsolute = EdgeMoveDistance * Screen.height;

        float screenHalfWidth = Screen.width / 2f;
        float screenHalfHeight = Screen.height / 2f;

        Vector2 screenCenter = new Vector2(screenHalfWidth, screenHalfHeight);
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 mouseDirection = mousePos - screenCenter;

        float x = Mathf.Abs(mouseDirection.x) - (screenHalfWidth - EdgeMoveDistanceAbsolute);
        float y = Mathf.Abs(mouseDirection.y) - (screenHalfHeight - EdgeMoveDistanceAbsolute);

        x = Mathf.Max(x, 0);
        y = Mathf.Max(y, 0);
        x /= EdgeMoveDistanceAbsolute;
        y /= EdgeMoveDistanceAbsolute;
        x *= Mathf.Sign(mouseDirection.x);
        y *= Mathf.Sign(mouseDirection.y) * -1;

        Vector3 direction = new Vector3(y, 0, x);

        return transform.TransformDirection(direction);
    }


    void UpdateZoom()
    {
        float zoomDirection = Mathf.Max(Ball.transform.localScale.x - SizeLerp.x, 0) / (SizeLerp.y - SizeLerp.x);

        m_ZoomValue = Mathf.Clamp(zoomDirection, 0f, 1f);

        float zoomAlpha = ZoomCurve.Evaluate(m_ZoomValue);

        m_CameraArmLength = Mathf.Lerp(ZoomLerp.x, ZoomLerp.y, zoomAlpha);
        m_AttachedCamera.transform.localPosition = new Vector3(0f, m_CameraArmLength, 0f);

        Vector3 angles = CameraArm.transform.localEulerAngles;
        angles.z = Mathf.Lerp(AngleLerp.x, AngleLerp.y, zoomAlpha);
        CameraArm.transform.localEulerAngles = angles;

        UpdateDOF();
        m_AttachedCamera.fieldOfView = Mathf.Lerp(FowLerp.x, FowLerp.y, zoomAlpha);
    }

    void UpdateDOF()
    {

    }

    void OnInteractableHover(GameObject hoverObject)
    {
        if (m_HoverObject == null)
        {
            m_HoverObject = hoverObject;
            InvokeRepeating("ClosestHoverCheck", 0f, 0.01f);
        }
    }

    void ClosestHoverCheck()
    {
        GameObject newHover = null;
        Collider[] hits = Physics.OverlapSphere(Target.transform.position, Target.GetComponent<InteractableHover>().Radius, LayerMask.GetMask("Interactable"), QueryTriggerInteraction.Collide );
        if (hits.Length == 0)
        {
            CancelInvoke("ClosestHoverCheck");
            m_HoverObject = null;
            return;
        }
        for (int i = 0; i < hits.Length; i++)
        {
            if (i == 0)
            {
                newHover = hits[i].gameObject;
            }
            else
            {
                Collider hit = hits[i];
                if ((hit.transform.position - Target.transform.position).magnitude < (Target.transform.position - newHover.transform.position).magnitude)
                {
                    newHover = hit.gameObject;
                }
            }
        }

        if (m_HoverObject != newHover)
        {
            m_HoverObject = newHover;
        }
    }

    (Vector3 position, Quaternion rotation, Vector3 scale) TransformInterpTo(Transform current, Transform target, float deltaTime, float interpSpeed)
    {
        if (interpSpeed <= 0f)
        {
            return (target.position, target.rotation, target.localScale);
        }
        float alpha = Mathf.Clamp(deltaTime * interpSpeed, 0f, 1f);
        return TransformLerp(current, target, alpha);
    }

    (Vector3 position, Quaternion rotation, Vector3 scale) TransformLerp(Transform a, Transform b, float alpha)
    {
        alpha = Mathf.Clamp01(alpha);
        Vector3 newPos = Vector3.Lerp(a.position, b.position, alpha);
        Quaternion newRot = Quaternion.Slerp(a.rotation, b.rotation, alpha);
        Vector3 newScale = Vector3.Lerp(a.localScale, b.localScale, alpha);

        return (newPos, newRot, newScale);
    }

    Vector3 VInterpTo(Vector3 current, Vector3 target, float deltaTime, float interpSpeed)
    {
        if (interpSpeed <= 0f)
        {
            return target;
        }
        float alpha = Mathf.Clamp(deltaTime * interpSpeed, 0f, 1f);
        return Vector3.Lerp(current, target, alpha);
    }

}
