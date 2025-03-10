using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxis("Vertical");
        rb.AddForce(new Vector3 (h, 0, v) * Time.deltaTime * 2f, ForceMode.Impulse);

        if(Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
    }
}
