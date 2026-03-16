using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField]
    private AudioClip chestThud;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            AudioManager.Instance.PlaySoundEffect(chestThud);
            rb.isKinematic = true; 
        }
    }
}
