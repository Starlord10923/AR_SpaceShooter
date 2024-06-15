using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bl_Joystick joystick;
    public float speed = 2f;
    public float rotationSpeed = 1.2f;
    public float maxDistanceFromOrigin = 20f;
    public Transform xrOrigin;
    public Transform mainCamera;
    public GameObject AttackEffect;
    public GameObject AttackParticle;

    private Rigidbody rb;
    private Vector3 initialPositionRelativeToOrigin;
    private Quaternion initialRotationRelativeToOrigin;

    private bool isReturningToOrigin = false;
    private bool isAttacking = false;
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private float returnDuration = 1.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        initialPositionRelativeToOrigin = xrOrigin.InverseTransformPoint(transform.position);
        initialRotationRelativeToOrigin = Quaternion.Inverse(xrOrigin.rotation) * transform.rotation;

        bl_Joystick.OnDoubleClick += ReturnToOrigin;

    }

    void OnDestroy()
    {
        bl_Joystick.OnDoubleClick -= ReturnToOrigin;
    }

    void Update()
    {
        if (!isReturningToOrigin)
        {
            Move();
        }
    }

    public void Move()
    {
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        Vector3 movement = new Vector3(h, 0f, v).normalized;

        Vector3 worldMovementDirection = xrOrigin.TransformDirection(movement);

        if (worldMovementDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(worldMovementDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        rb.velocity = worldMovementDirection * speed;

        Vector3 positionFromOrigin = transform.position - mainCamera.position;
        if (positionFromOrigin.magnitude > maxDistanceFromOrigin)
        {
            Vector3 clampedPosition = Vector3.ClampMagnitude(positionFromOrigin, maxDistanceFromOrigin);
            transform.position = mainCamera.position + clampedPosition;
        }
    }

    private float time;
    public void Attack()
    {
        GameObject attackParticle = Instantiate(AttackParticle, transform.position + transform.forward * 0.6f, transform.rotation);
        BattleUnit battleUnit = GetComponent<BattleUnit>();

        if (isAttacking){
            time = 2.5f;
        }else{
            StartCoroutine(AttackCoroutine());
        }
    }

    public IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        AttackEffect.SetActive(true);

        time = 2.5f;
        while(time>=0f){
            time -= Time.deltaTime;
            yield return null;
        }

        AttackEffect.SetActive(false);
        isAttacking = false;
    }

    public void ReturnToOrigin()
    {
        if (!isReturningToOrigin)
        {
            targetPosition = xrOrigin.TransformPoint(initialPositionRelativeToOrigin);
            targetRotation = xrOrigin.rotation * initialRotationRelativeToOrigin;

            StartCoroutine(ReturnToOriginCoroutine());
        }
    }

    IEnumerator ReturnToOriginCoroutine()
    {
        isReturningToOrigin = true;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        float elapsedTime = 0f;

        while (elapsedTime < returnDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / returnDuration);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / returnDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        isReturningToOrigin = false;
    }
}
