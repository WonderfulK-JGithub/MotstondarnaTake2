using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BallMovement : MonoBehaviour //av K-J (utom d�r det st�r max)
{
    [Header("X-Z Movement")]
    [SerializeField] float topSpeed;//h�gsta hastigheten
    [SerializeField] float acceleration;//hur snabbt man accelererar
    [SerializeField] float extraAccelerationFactor;//hur mycket g�nger mer man ska accelerera n�r man tv�rsv�nger
    [SerializeField] Transform orientationTransform;//en transform som anv�nds f�r att kalkulera vilket h�ll bollen ska �ka baserat p� kamerans rotation
    [SerializeField] Slider speedBar;//en bar som visar hur snabbt man �ker
    [SerializeField] Image fillImage;
    [SerializeField] Gradient speedColors;//r�d / gul = inte tillr�ckligt snabbt f�r att d�da k�glor, gr�n = tillr�ckligt snabbt

    float inputX;
    float inputZ;

    Vector3 targetSpeed;//hastigheten som klotet ska accelerera mot


    [Header("Jumping")]
    [SerializeField] float jumpStrength;//hur h�gt man hoppar
    [SerializeField] float jumpHoldTime;//hur l�nge man kan holla in knappen f�r ett h�gre hopp
    [SerializeField] float extraGravityFactor;//hur mycket g�nger mer man ska ha gravitation n�r man inte h�ller in hoppknappen
    [SerializeField] float terminalVelocity;//Den snabbaste hastighet klotet kan falla
    [SerializeField] LayerMask groundLayers;//vilka layers som r�knas som ground
    [SerializeField] LayerMask slipparyLayer;//vilka layers som r�knas som slippary

    float holdTimer;

    
    [Header("Dashing")]
    [SerializeField] float dashTime;//hur l�nge man �r fast i dashen
    [SerializeField] float dashHoldTime;
    [SerializeField] GameObject dashTrail;//referense till trailobjektet
    [SerializeField] Color dashBarColor;


    float dashTimer;

    [Header("Other")]
    [SerializeField] ParticleSystem chargeParticle;//referense till partiklesystemet som s�tts p� n�r man h�ller in dashknappen
    [SerializeField] float slideExtraGravity;//hur mycket g�nger mer gravitation man ska ha n�r man �ker i en slide(r�nna)
    [SerializeField] bool inHub;
    [SerializeField] GameObject ballUI;

    bool onGround;
    bool onSlippary;
    bool isJumping;
    bool canDash;

    bool noInput;

    public bool aboveKillSpeed = false; //F�r att se om spelaren �ker tillr�ckligt snabbt f�r att d�da en simple bowling pin - Max

    public Vector3 currentSpeed;

    [HideInInspector] public Rigidbody rb;
    public PlayerState state;

    

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();


        if(inHub)
        {
            state = PlayerState.Off;
            ballUI.SetActive(false);

        }

        
        
    }


    public virtual void Update()
    {
        if (Pause.gamePaused) return;
        switch (state)
        {
            case PlayerState.Free:
                #region

                //h�mtar inputs f�r vilket h�ll man ska g�
                inputX = Input.GetAxisRaw("Horizontal");
                inputZ = Input.GetAxisRaw("Vertical");

                noInput = inputX == 0 && inputZ == 0;//variabel som kollar om man tryckt �t n�got h�ll alls

                if(inputX != 0 && inputZ != 0)
                {
                    inputX *= Mathf.Sqrt(0.5f);
                    inputZ *= Mathf.Sqrt(0.5f);
                }
                
                //Den speed som bollen ska accelerera mot
                targetSpeed = new Vector3(inputX * topSpeed * orientationTransform.right.x + inputZ * topSpeed * orientationTransform.forward.x, 0f, inputX * topSpeed * orientationTransform.right.z + inputZ * topSpeed * orientationTransform.forward.z);


                //Hopp
                if (Input.GetButtonDown("Jump") && onGround)
                {
                    rb.velocity = new Vector3(rb.velocity.x, jumpStrength, rb.velocity.z);//ger en y velocity
                    isJumping = true;
                    holdTimer = jumpHoldTime;
                }

                //Om man sl�pper hoppknappen, eller att jumptimern g�r ut, blir isJumping false
                if (isJumping && !onGround)
                {
                    if (Input.GetButton("Jump"))
                    {
                        holdTimer -= Time.deltaTime;
                        if (holdTimer <= 0)
                        {
                            isJumping = false;
                        }
                    }
                    else
                    {
                        isJumping = false;
                    }
                }

                //Dash
                if(Input.GetButtonDown("Fire1") && canDash)
                {
                    state = PlayerState.ChargeDash;//�ndrar state
                    chargeParticle.Play();//S�tter ig�ng particles
                    canDash = false;//g�r att man inte kan dasha (f�r�ns variabeln blir true igen)
                    dashTimer = dashHoldTime;

                    rb.useGravity = false;//st�nger av gravitation

                    dashTrail.transform.position = transform.position;
                    dashTrail.SetActive(false);
                }

                #endregion
                break;
            case PlayerState.ChargeDash:
                #region

                currentSpeed = Vector3.Lerp(currentSpeed, Vector3.zero, 0.05f);//�ndrar bollens hastighet l�ngsamt till 0

                //kollar om man sl�pper dashKnappen
                if(!Input.GetButton("Fire1"))
                {
                    SoundManagerScript.PlaySound("Dash");

                    chargeParticle.Stop();//St�nger av particle systemet
                    state = PlayerState.Dash;//�ndrar state

                    rb.velocity = orientationTransform.forward * topSpeed;//�ndrar hastigheten (i den riktingen kameran kollar)
                    currentSpeed = rb.velocity;
                    dashTimer = dashTime;

                    dashTrail.SetActive(true);
                }
                else
                {
                    dashTimer -= Time.deltaTime;
                    if(dashTimer <= 0)
                    {
                        state = PlayerState.Free;
                        chargeParticle.Stop();
                    }
                }
                #endregion
                break;
            case PlayerState.Dash:
                #region
                dashTimer -= Time.deltaTime;

                if(dashTimer <= 0f)//n�r dashtimern �r slut �r man tillbaks till vanligt movement
                {
                    state = PlayerState.Free;
                    rb.useGravity = true;
                }

                #endregion
                break;
        }
    }

    
    void FixedUpdate()
    {
        switch(state)
        {
            case PlayerState.Free:
                #region
                if (Physics.Raycast(transform.position, Vector3.down, 0.60f, groundLayers))
                {
                    onGround = true;
                    canDash = true;
                }//kollar om man �r p� ground
                else
                {
                    onGround = false;
                }
                if (Physics.Raycast(transform.position, Vector3.down, 0.52f, slipparyLayer))
                {
                    onSlippary = true;
                }//kollar om man �r p� slippary
                else
                {
                    onSlippary = false;
                }


                //�r vinkeln mellan vectorerna av den nuvarnade hastigheten och target hastigheten mer �n 90 grader r�knar jag det som tv�rsv�gning
                //det ska �ven bli extra acceleration om spelaren �r �ver top hastigheten
                float accModifier = 1f;
                if(currentSpeed.magnitude > topSpeed)
                {
                    accModifier = extraAccelerationFactor * 2f;
                }
                else if(Vector3.Angle(targetSpeed.normalized, currentSpeed.normalized) > 90f)
                {
                    accModifier = extraAccelerationFactor;
                }

                if ((onGround && !onSlippary) || !noInput)//�r man i luften eller p� halt golv OCH inte trycker �t n�got h�ll beh�ller man den hastighet man hade
                {

                    currentSpeed = Vector3.MoveTowards(new Vector3(rb.velocity.x, 0f, rb.velocity.z), targetSpeed, acceleration * Time.fixedDeltaTime * accModifier);


                    rb.velocity = new Vector3(currentSpeed.x, rb.velocity.y, currentSpeed.z);
                }

                if (!onGround && !isJumping)//l�gger till extra gravitation f�r ett b�ttre hopp
                {
                    rb.velocity += new Vector3(0f, Physics.gravity.y * (extraGravityFactor - 1) * Time.fixedDeltaTime, 0f);
                }

                float yVelocityClamped = Mathf.Clamp(rb.velocity.y, terminalVelocity, 69420f);//clampar y s� att man inte g�r �ver terminal velocity
                rb.velocity = new Vector3(rb.velocity.x, yVelocityClamped, rb.velocity.z);

                chargeParticle.transform.position = transform.position;//g�r att charge particlen f�ljer med spelaren

                #endregion
                break;
            case PlayerState.ChargeDash:
                #region
                rb.velocity = new Vector3(currentSpeed.x, currentSpeed.y, currentSpeed.z);
                chargeParticle.transform.position = transform.position;
                #endregion
                break;
            case PlayerState.Renn:
            case PlayerState.End:
                #region
                rb.velocity += new Vector3(0f, Physics.gravity.y * (slideExtraGravity - 1) * Time.fixedDeltaTime, 0f);
                #endregion
                break;
            case PlayerState.Dash:
                #region
                dashTrail.transform.position = transform.position;
                break;
            #endregion
            case PlayerState.Hub:
                #region
                rb.velocity += new Vector3(0f, Physics.gravity.y * (slideExtraGravity - 1) * Time.fixedDeltaTime, 0f);
                currentSpeed = rb.velocity;
                #endregion
                break;
            case PlayerState.Off:
                #region
                rb.velocity = Vector3.zero;
                rb.useGravity = false;
                #endregion
                break;
            case PlayerState.UnControllable:
                #region
                if (Physics.Raycast(transform.position, Vector3.down, 0.52f, groundLayers))
                {
                    state = PlayerState.Free;
                }

                currentSpeed = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.useGravity = true;
                rb.velocity += new Vector3(0f, Physics.gravity.y * (extraGravityFactor - 1) * Time.fixedDeltaTime, 0f);

                #endregion
                break;
        }

        rb.angularVelocity = new Vector3(rb.velocity.z,0f,-rb.velocity.x);//�ndrar rotationSpeeden baserat p� hastigheten bollen har

        float _value = new Vector3(rb.velocity.x,0f,rb.velocity.z).magnitude / (topSpeed * 0.95f);//_value �r mellan 0 och 1 och �r hur mycket procent av topspeed man har

        if(state == PlayerState.ChargeDash)
        {
            speedBar.value = dashTimer / dashHoldTime;

            fillImage.color = dashBarColor;
        }
        else
        {
            speedBar.value = Mathf.Lerp(speedBar.value, _value, 0.3f);//�ndrar valuen p� slidern s� att baren fylls upp

            fillImage.color = speedColors.Evaluate(_value);//�ndar f�rgen p� baren baserat p� om man har tillr�ckligt med fart(f�r att d�da k�glor) med hj�lp av en gradient
        }

        //Kollar bollens hastighet och �ndrar vissa bowling pins till 
        //triggers s� att det ser b�ttre ut n�r man f�r en strike - Max 
        if (new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude >= 8)
        {
            if (!aboveKillSpeed) //boolen beh�vs s� det h�r inte g�rs varje frame - Max
            {
                aboveKillSpeed = true;
                ChangeBowlingPinLayers(9);
            }
        }
        else
        {
            if (aboveKillSpeed)
            {
                aboveKillSpeed = false;
                ChangeBowlingPinLayers(0);
            }
        }

    }

    void ChangeBowlingPinLayers(int layer)
    {
        //FindGameObjectsWithTag �r tydligen b�ttre f�r performance �n FindObjectsOfType
        //och eftersom det kanske finns v�ldigt m�nga pins p� banan s� anv�nder jag det - Max
        GameObject[] allSimpleBowlingPins = GameObject.FindGameObjectsWithTag("SimpleBowlingPin");

        //�ndrar lagret p� varenda bowling pin den hittar - Max
        for (int i = 0; i < allSimpleBowlingPins.Length; i++)
        {
            allSimpleBowlingPins[i].layer = layer;
        }
    }

    public void UpdateRotation(Vector3 rotation)
    {
        orientationTransform.eulerAngles = rotation;
        chargeParticle.transform.eulerAngles = rotation;
        if(state == PlayerState.ChargeDash)dashTrail.transform.eulerAngles = rotation;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("R�nna") && state == PlayerState.Free)
        {
            state = PlayerState.Renn;
            currentSpeed = Vector3.zero;
        }
        else if(other.gameObject.CompareTag("Coin"))
        {

            other.gameObject.GetComponent<CollectableCoin>().CollectCoin();

            SoundManagerScript.PlaySound("Coins");
        }
        else if(other.gameObject.CompareTag("Finish"))
        {
            transform.position = other.transform.position;
            rb.velocity = Vector3.right * 2f;
            state = PlayerState.End;
            rb.useGravity = true;
            chargeParticle.Stop();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("R�nna") && state == PlayerState.Free)
        {
            state = PlayerState.Renn;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("R�nna"))
        {
            state = PlayerState.Free;
            currentSpeed = rb.velocity;
            currentSpeed.y = 0f;
        }
    }

    public void BossDamaged(Vector3 bossKnockback)
    {
        state = PlayerState.UnControllable;

        rb.velocity = bossKnockback;
    }
    
}





public enum PlayerState
{
    Free,
    ChargeDash,
    Dash,
    Renn,
    Off,
    Hub,
    End,
    UnControllable,
}