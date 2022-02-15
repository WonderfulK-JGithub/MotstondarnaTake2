using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Max Script
public class BaseEnemy : MonoBehaviour
{
    [Header("Base Enemy")]

    [SerializeField] float playerVelocityForDeath; //Hur snabbt spelaren ska �ka f�r att d�da fienden - Max
    [SerializeField] float dieForce; //Den force fienden skjuts iv�g fr�n spelaren med n�r den d�r - Max
    [SerializeField] float fadeSpeed; //Hur snabbt fienden fadear iv�g efter d�d - Max

    [SerializeField] bool canDieFromOtherPins = false; //Om de kan d� av att andra fiender �ker in i dem - Max
    [SerializeField] bool canKillPlayer = false; //Basic pins kan inte g�ra skada mot spelaren - Max

    public bool hasDied = false;

    //Referenser
    MeshRenderer rend;
    [HideInInspector] public Rigidbody rb; //�r public f�r att skript som �rver av detta ocks� ska kunna accessa rigidbodyn - Max
    [SerializeField] GameObject deathParticle; //Man d�r Particle som skapas n�r man d�r - Max

    public virtual void Awake()
    {
        rend = GetComponentInChildren<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        OnAnyCollisionEnter(collision.collider, collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        OnAnyCollisionEnter(other);
    }

    //Den h�r funktionen callas b�de om spelaren collidar med en collider eller en trigger - Max
    void OnAnyCollisionEnter(Collider other, Collision collision = null)
    {
        if (hasDied) return;

        BallMovement ball = other.transform.GetComponent<BallMovement>();

        if (ball != null) //Kollar om det �r spelaren man collidat med - Max
        {
            if (ball.currentSpeed.magnitude >= playerVelocityForDeath)
            {
                Die(collision != null ? collision.GetContact(0).point : other.transform.position, ball.currentSpeed);
            }
            else if (ball.currentSpeed.magnitude < playerVelocityForDeath)
            {
                rb.isKinematic = true; //S� att spelaren inte kan putta p� fienden - Max

                //Anv�nder formeln angle = point1 - point2 f�r att ta fram riktningen fr�n spelaren till fienden - Max
                Vector3 dir = collision != null ? collision.GetContact(0).point : other.transform.position - transform.position;
                dir = dir.normalized; //Normalizar vinkeln den s� att jag bara f�r vinkeln av vektorn - Max

                ball.currentSpeed = dir * 5; //L�gger p� en force i direction - Max

                //De flesta k�glor g�r damage till spelaren om man �ker in i dem f�r l�ngsamt - Max
                if (canKillPlayer)
                    other.transform.GetComponent<BallHealth>().TakeDamage(Vector3.zero, 1);
            }
        }
        else //Om man inte collidade med spelaren
        {
            if (canDieFromOtherPins && other.transform.GetComponent<BaseEnemy>()) //Basic k�glor kan d� av varandra n�r de studsar p� varnadra - Max
            {
                Vector3 dir = collision.GetContact(0).point - transform.position; //F�r riktingen mot contact point - Max 
                dir = -dir.normalized;  //Normalizar vinkeln s� att jag bara f�r vinkeln av vektorn - Max

                Die();
            }
        }       
    }

    private void OnCollisionExit(Collision collision)
    {
        //Man ska kunna forts�tta r�ra sig efter man inte l�ngre r�r spelaren
        if (collision.transform.GetComponent<BallMovement>())
            rb.isKinematic = false;
    }

    public virtual void Die(Vector3 contactPoint, Vector3 speed) //D�r och f�r en dieforce (flyger iv�g fr�n spelaren) - Max
    {
        PrepareDeath();

        //Anv�nder formeln angle = point1 - point2 f�r att ta fram riktningen fr�n spelaren till fienden - Max
        Vector3 dir = contactPoint - transform.position;
        dir = speed.normalized; //Anv�nder spelarens riktning f�r att skapa forcen - Max

        GetComponent<Rigidbody>().AddForceAtPosition(dir * dieForce, contactPoint); //L�gger p� en force i riktingen - Max

        StartCoroutine(Fade());
    }

    public void Die() //D�r utan att f� en dieforce - Max
    {
        PrepareDeath();
        StartCoroutine(Fade());
    }

    public void DieNow() //D�r direkt utan att fadea - Max
    {
        PrepareDeath();
        Destroy(gameObject);
    }

    void PrepareDeath()
    {
        hasDied = true; //S� att man inte kan d� flera g�nger - Max

        SoundManagerScript.PlaySound("K�gglaDamage"); //Ljudeffekt - Max
        if(deathParticle != null)
            SpawnParticles(); //Spawnar particles - Max
    }

    void SpawnParticles()
    {
        //Skapar particle - Max
        GameObject newParticle = Instantiate(deathParticle, transform.position, Quaternion.identity);

        //F�rst�r den efter viss tid - Max
        Destroy(newParticle, 1);
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(2); //F�rst v�ntar 2 sekunder - Max

        Color color;
        while (rend.material.color.a > 0) //Uppdaterar materialet varje frame och den f�r mindre och mindre alpha - Max
        {
            color = rend.material.color;
            rend.material.color = new Color(color.r, color.g, color.b, color.a - (Time.deltaTime * fadeSpeed));
            yield return null;
        }

        Destroy(gameObject); //N�r den �r osynlig s� f�rst�rs objektet p� riktigt - Max
    }
}
