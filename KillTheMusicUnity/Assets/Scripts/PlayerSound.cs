using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private Rigidbody rb;
    public AudioSource rollingSound;

    // Use this for initialization
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.magnitude > 0.01f)
        {
            float pitch = Mathf.Log10(rb.velocity.magnitude + 1f);
            float volume = Mathf.Log10(rb.velocity.magnitude + 1.3f);
            pitch = Mathf.Clamp(pitch, 0.9f, 1f);
            rollingSound.pitch = pitch;
            rollingSound.volume = volume;
        }
        else
        {
            rollingSound.pitch = 0f;
            rollingSound.volume = 0f;
        }
    }
}
