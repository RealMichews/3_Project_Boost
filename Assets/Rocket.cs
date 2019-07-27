using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 1000f;

    [SerializeField] AudioClip thrustSound;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip deathSound;

    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem winParticles;
    [SerializeField] ParticleSystem deathParticles;

    static private int currentLevel = 0;
    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrust();
            RespondToRotate();
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; } //Ignore Collisions when dead

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                LoadNextLevel();
                break;
            default:
                ExecuteDeath();
                break;
        }
    }

    private void LoadNextLevel()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(winSound);
        winParticles.Play();
        Invoke("LoadNextScene", 1f); //parameterise time
    }

    private void ExecuteDeath()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke("ResetScene", 1f); //parameterise time
    }

    private void ResetScene()
    {
        SceneManager.LoadScene(currentLevel);
    }

    private void LoadNextScene()
    {
        currentLevel++;
        SceneManager.LoadScene(currentLevel);
    }

    private void RespondToThrust()
    {
        if (Input.GetKey(KeyCode.Space)) // can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            thrustParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(thrustSound);
        }
        if (!thrustParticles.isPlaying)
        {
            thrustParticles.Play();
        }
        
    }

    private void RespondToRotate()
    {
        rigidBody.freezeRotation = true; //take manual control of rotation
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; // return automatic control
    }
}
