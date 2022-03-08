using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Range(10, 50)]
    public float strength; // hur högt den ska hoppa - Anton
    private void OnCollisionEnter(Collision collision) // känner av när något hoppar på den - Anton
    {
        if (collision.gameObject.GetComponent<Rigidbody>() != null) // om objektet har en rigidbody - Anton
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>(); // referens till rigidbodyn - Anton
            //rb.velocity += transform.up * strength; // skjuter upp objektet i luften - Anton
            rb.velocity = new Vector3(rb.velocity.x,strength,rb.velocity.z);

            GetComponentInChildren<Animator>().Play("jumppad"); //Animation - Max

            SoundManagerScript.PlaySound("JumpPad");
        }
    }
}
