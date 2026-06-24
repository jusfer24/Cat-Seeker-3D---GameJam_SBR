using UnityEngine;

public class MovimientoHelloKitty : MonoBehaviour
{
    public float velocidad = 5f;
    public Animator animator_h;

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            animator_h.SetBool("estaCaminando", true);
            transform.Translate(Vector3.right * velocidad * Time.deltaTime);
        }
        else
        {
            animator_h.SetBool("estaCaminando", false);
        }
    }
}