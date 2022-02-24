using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Max Script
public class RocketEnemy : MonoBehaviour
{
    bool rocketOn = false; //Faktiskt åker med raketet - Max
    bool alerted = false; //Att fienden bara är alerted - Max

    [Header("Parameters")]
    [SerializeField] float rocketRotatingSpeed; //Hur snabbt fienden roterar när den åker med raketen - Max
    [SerializeField] float rocketSpeed; //Hur snabbt fienden åker med raketen - Max
    [SerializeField] float rocketRotSpeedWhileActivating; //Hur snabbt den roterar när den blir alerted och kastar sig på marken för att börja åka - Max

    [SerializeField] float rocketActivateRadius; //Radius för att den ska aktiveras - Max
    [SerializeField] float rocketExplosionTime; //Hur länge den åker innan automatisk explosion - Max

    [SerializeField] float rocketExplodeRadius; //Radius för att kolla hur nära spelaren ska vara för att den ska explodera - Max

    [SerializeField] float knockBackForce; //Explosionens knockback på spelaren - Max

    [Header("References")]
    Transform player;
    [SerializeField] ParticleSystem rocketParticles; //Partiklarna från raketen
    [SerializeField] GameObject explosion; //Explosionen
    WanderingEnemy wanderingScript;
    Rigidbody rb;

    [SerializeField] PhysicMaterial physMat;

    Animator anim;

    //privates
    Quaternion? lastRot = null; //Rotationen av rocketen förra framenen - Max

    private void Awake()
    {
        wanderingScript = GetComponent<WanderingEnemy>();
        player = FindObjectOfType<BallMovement>().transform;
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        rocketParticles.Stop(); //Så att inte raketen är på när den bara går runt - Max
    }

    private void Update()
    {
        if (wanderingScript.isChasingPlayer && !wanderingScript.overrideChasing && Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(player.position.x, 0, player.position.z)) < rocketActivateRadius)
        {
            StartRocket(); //När spelaren är tillräckligt nära så ska den starta raketen - Max
        }

        if (rocketOn && !wanderingScript.hasDied)
        {
            RocketInUpdate();
        }
        else if(alerted)
        {
            //Roterar fienden mot spelaren medan den gör startanimationen, annars kan den åka åt fel håll - Max
            RotateTowardsPlayer(rocketRotSpeedWhileActivating);
        }

        //Om tillräckligt nära spelaren så ska den explodera - Max
        if (rocketOn && Vector3.Distance(transform.position, player.position) < rocketExplodeRadius)
        {
            Explode();
        }
    }

    void StartRocket()
    {
        //Ser till så att det inte blir konstigt med wanderingscript - Max
        wanderingScript.overrideChasing = true;

        GetComponent<MeshCollider>().material = physMat;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        anim.Play("RocketStart");
        alerted = true; //istället för att starta raketen direkt så alertar vi den - Max
        Invoke(nameof(StartMovingRocket), 1.05f);
        StartCoroutine(nameof(tilExplode)); //Tills den exploderar automatiskt - Max
    }

    void StartMovingRocket()
    {
        //Raketen sätts faktiskt igång - Max
        rocketOn = true;
        rocketParticles.Play();
        SoundManagerScript.PlaySound("RocketFiende");
    }

    void RocketInUpdate()
    {
        RotateTowardsPlayer();

        //Rör sig framåt
        Vector3 newVel = transform.forward * rocketSpeed;
        rb.velocity = new Vector3(newVel.x, rb.velocity.y, newVel.z);
    }

    void RotateTowardsPlayer()
    {
        RotateTowardsPlayer(rocketRotatingSpeed);
    }

    void RotateTowardsPlayer(float rotSpeed)
    {
        if(lastRot != null)
            transform.rotation = (Quaternion)lastRot; //Gör så att rotationen inte kan ändras mellan frames av okänd anledning som gjorde att den snurrade konstigt - Max

        //Roterar
        Vector3 lookAt = transform.InverseTransformPoint(player.position);
        lookAt.y = 0;
        lookAt = transform.TransformPoint(lookAt);

        Quaternion rotation = transform.rotation;
        transform.LookAt(lookAt, transform.up);
        Quaternion lookRotation = transform.rotation;
        transform.rotation = Quaternion.RotateTowards(rotation, lookRotation, rotSpeed * Time.deltaTime);
        lastRot = Quaternion.RotateTowards(rotation, lookRotation, rotSpeed * Time.deltaTime); ;
    }

    //Exploderar automatiskt efter ett tag - Max
    IEnumerator tilExplode()
    {
        yield return new WaitForSeconds(rocketExplosionTime);
        Explode();
    }

    void Explode()
    {
        SoundManagerScript.PlaySound("Explosion"); //Spelar explosion-ljud - Max

        //Kollar om tillräckligt nära spelaren för att skada den - Max
        if (Vector3.Distance(transform.position, player.position) < rocketExplodeRadius * 1.2f)
        {
            //får fram vector riktning och längd
            Vector3 dir = player.position - transform.position;

            //Normalizar så att det är bara riktning
            dir = dir.normalized;

            //Ger knockback till spelaren, 
            //TakeDamage har en knockback variabel men den kan inte göra på y-axeln, 
            //så därför gör jag det på rigidbodyns velocity istället - Max
            player.GetComponent<Rigidbody>().velocity = new Vector3(0,20,0) + dir * knockBackForce;

            //Skadar spelaren - Max
            FindObjectOfType<BallHealth>().TakeDamage(Vector3.zero, 1);
        }

        Transform newExplosion = Instantiate(explosion, transform.position, Quaternion.identity).transform;
        Destroy(newExplosion.GetChild(0).gameObject, 0.2f);
        Destroy(newExplosion.gameObject, 1);

        //Ska dö direkt efter explosion - Max
        wanderingScript.DieNow();
    }
}
