using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour// av K-J
{
    public static CameraController current;

    [SerializeField] float mouseSence = 3f;
    [SerializeField] BallMovement target;//referense till spelaren som kameran ska kolla p�
    [SerializeField] float maxDistanceFromTarget = 5f;//hur l�ngt kameran ska kolla ifr�n spelaren
    [SerializeField] float smoothTime;
    [SerializeField] float smoothSpeed;
    [SerializeField] LayerMask collisionLayers;
    [SerializeField] float theGaming;
    [Header("ScreenShake")]
    [SerializeField] float screenShakeTime;
    [SerializeField] float screenShakeMagnitude;

    Vector3 currentRotation;
    Vector3 smoothVelocity = Vector3.zero;

    float rotationX;
    float rotationY;
    float distanceFromTarget;
    float shakeTimer;
    float shakePower;
    float powerReduction;

    bool firstPerson;//om man �r i firstperson mode

    void Awake()
    {
        current = this;

        Cursor.lockState = CursorLockMode.Locked;
        distanceFromTarget = maxDistanceFromTarget;
    }
    void Update()
    {
        if (Pause.gamePaused) return;
        if(!firstPerson)
        {
            
            float mouseX = Input.GetAxis("Mouse X");//skaffar mouse drag input
            float mouseY = Input.GetAxis("Mouse Y");

            rotationX += mouseX * mouseSence;
            rotationY += mouseY * -mouseSence;

            rotationY = Mathf.Clamp(rotationY, 0, 80f);//begr�nsar hur mycket man kan rotera kameran upp och ner

            Vector3 nextRotation = new Vector3(rotationX, rotationY);//rotationen kameran ska g� mot
            currentRotation = Vector3.SmoothDamp(currentRotation, nextRotation, ref smoothVelocity, smoothTime);//i v�rt fall har vi ingen "smooth" camera s� vi beh�ver egentligen inte detta
            target.UpdateRotation(new Vector3(0f, rotationX, 0f));//�ndrar rotationen p� ett antal saker baserat p� kamerans rotation

            if(Input.GetMouseButtonDown(1))//omg firstperson mode
            {
                firstPerson = true;
                transform.SetParent(target.transform);
                transform.localPosition = Vector3.zero;
            }

            


        }
        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                firstPerson = false;
                transform.parent = null;
            }
        }
        
        
    }

    void FixedUpdate()
    {
        if(!firstPerson)
        {
            transform.localEulerAngles = new Vector3(currentRotation.y, currentRotation.x, 0f);//�ndrar rotationen p� transformen

            //kameran raycastar fr�n spelaren f�r att kolla s� det inte �r ett objekt mellan den och spearen (eller att kameran �r i ett objekt)
            if (Physics.Raycast(target.transform.position, transform.forward * -1f, out RaycastHit hit, maxDistanceFromTarget, collisionLayers))
            {
                distanceFromTarget = Mathf.Lerp(distanceFromTarget, hit.distance, smoothSpeed) - theGaming;//igen, jag beh�ver inte lerp f�r smootheSpeed �r p� 1 men det f�r vara kvar �nd�
            }
            else
            {
                distanceFromTarget = Mathf.Lerp(distanceFromTarget, maxDistanceFromTarget, smoothSpeed) - theGaming;
            }

            transform.position = target.transform.position - transform.forward * distanceFromTarget;//�ndrar kamerans position

            if(shakeTimer > 0f)
            {
                shakeTimer -= Time.fixedDeltaTime;

                Vector3 shakePos = new Vector3();
                shakePos.x = Random.Range(-shakePower, shakePower);
                shakePos.y = Random.Range(-shakePower, shakePower);

                Vector3 truePos = shakePos.x * transform.right + shakePos.y * transform.up;

                transform.position += truePos;

                shakePower -= powerReduction * Time.fixedDeltaTime;
            }
        }
        else
        {
            transform.localEulerAngles = new Vector3(target.transform.localEulerAngles.x, target.transform.localEulerAngles.y, target.transform.localEulerAngles.z);//�ndrar rotationen p� transformen
        }
    }

    [ContextMenu("ScreenShake")]
    public void ScreenShake()
    {
        shakeTimer = screenShakeTime;
        shakePower = screenShakeMagnitude;
        powerReduction = shakePower / shakeTimer;
    }
}