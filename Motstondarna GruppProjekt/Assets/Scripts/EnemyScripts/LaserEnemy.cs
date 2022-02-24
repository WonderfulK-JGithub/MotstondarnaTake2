using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Max Script
public class LaserEnemy : MonoBehaviour
{
    [SerializeField] GameObject[] activeLasers = new GameObject[2];     //De lasrarna som är igång - Max

    [SerializeField] float laserRotateSpeed; //Hur snabbt fienden roterar när lasrarna är på - Max

    [Header("Parameters")]

    [SerializeField] float laserMaxDistance; //Hur lång som lasern kan vara - Max

    [SerializeField] float laserStartRotation; //Så att lasern börjar med att kolla ner i marken så man inte instant dör - Max

    [SerializeField] float laserActivationRotationSpeed; //Hur snabbt lasern roterar på x-axeln alltså vinklar sig upp eller ner - Max

    [SerializeField] float laserAttackActivateRadius; //Radius för när lasern ska användas - Max

    [SerializeField] LayerMask laserMask; //Vad laserns ska collidea med

    [SerializeField] float laserFireTime;
    [SerializeField] float laserCooldown;

    public bool lasersOn = false;
    bool alerted = false;

    float fireTimer;
    float coolDownTimer;

    [Header("References")]

    [SerializeField] GameObject laserObject;
    [SerializeField] GameObject[] laserDust;

    //Lasrarna ska komma ut ur ögonen - Max
    [SerializeField] Transform[] eyes = new Transform[2];

    //För att inte behöva göra en raycast per ögon så finns det en annan transform som används för raycast origin - Max
    [SerializeField] Transform laserOrigin; //Är också parent till eyes

    WanderingEnemy wanderingScript;
    Transform player;

    Animator anim;

    AudioSource currentSound;

    private void Awake()
    {
        wanderingScript = GetComponent<WanderingEnemy>();
        player = FindObjectOfType<BallMovement>().transform;
        anim = GetComponentInChildren<Animator>();

        laserDust[0].SetActive(false);
        laserDust[1].SetActive(false);
    }

    private void Update()
    {
        coolDownTimer -= Time.deltaTime;

        //Kollar distance till spelaren och stänger av eller sätter på lasrar - Max
        if (wanderingScript.isChasingPlayer && !wanderingScript.overrideChasing && Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(player.position.x, 0, player.position.z)) < laserAttackActivateRadius)
        {
            if(coolDownTimer <= 0f)
            {
                wanderingScript.overrideChasing = true;
                TurnOnLasers();
            }
            
        }
        else if (wanderingScript.overrideChasing && Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(player.position.x, 0, player.position.z)) > laserAttackActivateRadius + 4)
        {
            wanderingScript.overrideChasing = false;
            TurnOffLasers();
        }

        if(alerted || lasersOn)
        {
            //När den lasrar ska den rotera mot spelaren - Max
            RotateTowardsPlayer();
        }

        if (lasersOn)
        {
            fireTimer -= Time.deltaTime;

            //Har fienden dött eller slutat chasea spelaren, eller att fireTimern har gått ut, ska lasrarna stängas av - Max
            if (!wanderingScript.isChasingPlayer || wanderingScript.hasDied || fireTimer <= 0f)
            {
                wanderingScript.overrideChasing = false;
                TurnOffLasers();
            }

            //Logic för lasrarna:

            for (int i = 0; i < eyes.Length; i++)
            {
                RaycastHit hit;

                //En Raycast för varje öga
                if (Physics.Raycast(eyes[i].position, eyes[i].forward, out hit, laserMaxDistance, laserMask))
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        //Gör Damage
                        FindObjectOfType<BallHealth>().TakeDamage(/*eyes[i].forward * 10*/ Vector3.zero, 1);
                        wanderingScript.overrideChasing = false;
                        TurnOffLasers();
                    }

                    //Laserns storlek ska ändras så den slutar där raycasten träffar - Max
                    UpdateLaserScale(hit.distance, i);

                    laserDust[i].transform.position = hit.point;
                }
                else
                {
                    UpdateLaserScale(laserMaxDistance, i);

                    laserDust[i].transform.position = new Vector3(69f, 420f, 1337f);
                }
            }

            //Använder formeln angle = point1 - point2 för att ta fram riktningen från fienden till spelaren - Max
            Vector3 dir = player.position - laserOrigin.position;

            //Normalizar till en riktning - Max
            dir = dir.normalized;

            //Gör om till en rotation - Max
            Quaternion dirQ = Quaternion.LookRotation(dir);

            //kollar om spelaren är över fiendens ögon - Max
            bool playerAbove = player.position.y > laserOrigin.position.y;

            //Lerpar rotationen så att den roterar lasrarna upp och ner så att den siktar mot spelaren, annars kunde man stå under lasern när den gick rakt ut - Max
            laserOrigin.localEulerAngles = new Vector3(Mathf.Clamp(Mathf.Lerp(laserOrigin.eulerAngles.x, playerAbove ? 0 : dirQ.eulerAngles.x, laserActivationRotationSpeed * Time.deltaTime), 1, 72), 0, 0);

            //laserOrigin.localEulerAngles = new Vector3(Mathf.Lerp(laserOrigin.eulerAngles.x, 0, laserActivationRotationSpeed * Time.deltaTime), 0, 0);

        }
    }

    void RotateTowardsPlayer()
    {
        //Roterar med en constant speed - Max
        Vector3 lookAt = transform.InverseTransformPoint(player.position);
        lookAt.y = 0;
        lookAt = transform.TransformPoint(lookAt);

        Quaternion rotation = transform.rotation;
        transform.LookAt(lookAt, transform.up);
        Quaternion lookRotation = transform.rotation;
        transform.rotation = Quaternion.RotateTowards(rotation, lookRotation, laserRotateSpeed * Time.deltaTime);
    }

    void UpdateLaserScale(float length, int id)
    {
        //Scalear lasern till length - Max
        activeLasers[id].transform.localScale = new Vector3(1, 1, length) * (1 / 0.75f); //(1 / 0.75) är för att kompensera för fiendens scale - Max
    }

    //För att uppdatera scale för båda lasrarna samtidigt
    void UpdateLaserScale(float length)
    {
        for (int i = 0; i <= 1; i++)
        {
            activeLasers[i].transform.localScale = new Vector3(1, 1, length) * (1 / 0.75f);
        }
    }

    public void TurnOnLasers()
    {

        anim.Play("LaserAlerted");
        alerted = true; //Så att den börajr rotera mot spelaren i animationen - Max

        //Använder en IEnumerator så att animationen kan spelas innan lasern sätts igång - Max
        StartCoroutine(nameof(tilLasersOn));

        fireTimer = laserFireTime;
    }

    IEnumerator tilLasersOn()
    {
        yield return new WaitForSeconds(1.28333f / 2);

        //Så att den inte börjar lasra när den är död - Max
        if (wanderingScript.hasDied) yield break;

        wanderingScript.StartChasing();
        lasersOn = true;
        alerted = false;

        //Ljudeffekt
        if(currentSound != null)Destroy(currentSound.gameObject);
        currentSound = AdvancedAudioManager.current.PlayLoopedSound(AdvancedAudioManager.current.audioClips[(int)AUDIO.LASER]);

        //Spawnar lasrar på båda ögonen - Max
        for (int i = 0; i <= 1; i++)
        {
            activeLasers[i] = Instantiate(laserObject, eyes[i]);
            laserDust[i].SetActive(true);
        }

        //Roterar lasrarna neråt i marken så man inte dör direkt - Max
        laserOrigin.rotation = Quaternion.Euler(laserStartRotation, 0, 0);
    }

    public void TurnOffLasers()
    {

        lasersOn = false;

        coolDownTimer = laserCooldown;

        //Förstör lasrarna för båda ögonen - Max
        for (int i = 1; i >= 0; i--)
        {
            Destroy(activeLasers[i]);
            laserDust[i].SetActive(false);
        }

        if(currentSound != null)
        {
            Destroy(currentSound.gameObject);
            currentSound = null;
        }

        anim.Play("Walking");
    }
}
