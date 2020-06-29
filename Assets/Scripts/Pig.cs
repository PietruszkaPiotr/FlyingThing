using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pig : MonoBehaviour {

    [SerializeField] float rotationSpeed = 150.0f;
    [SerializeField] float thrustSpeed = 150.0f;
    [SerializeField] float levelLoadDelay = 2.0f;

    [SerializeField] AudioClip engineAudio;
    [SerializeField] AudioClip deadAudio;
    [SerializeField] AudioClip finishAudio;

    [SerializeField] ParticleSystem engineParticles;
    [SerializeField] ParticleSystem deadParticles;
    [SerializeField] ParticleSystem finishParticles;

    Rigidbody rb;
    AudioSource audioSource;

    bool alive = true;

    bool enabledCollisions = true;

    void Start() {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (alive) {
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild) {
            RespondToDebugKeys();
        }
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        }
        else {
            audioSource.Stop();
            engineParticles.Stop();
        }
    }

    private void ApplyThrust() {
        if (!audioSource.isPlaying) {
            audioSource.PlayOneShot(engineAudio);
            engineParticles.Play();
        }
        rb.AddRelativeForce(Vector3.up * thrustSpeed * Time.deltaTime);
    }

    private void RespondToRotateInput() {
        rb.angularVelocity = Vector3.zero;

        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(-Vector3.forward * rotationSpeed * Time.deltaTime);
        }
    }

    private void RespondToDebugKeys() {
        if (Input.GetKeyDown(KeyCode.N)) {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.B)) {
            enabledCollisions = !enabledCollisions;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if(!alive || !enabledCollisions) {
            return;
        }
        switch (collision.gameObject.tag) {
            case "Safe":
                break;
            case "Finish":
                ApplySound(finishAudio);
                alive = false;
                finishParticles.Play();
                Invoke("LoadNextScene", levelLoadDelay);
                break;
            default:
                ApplySound(deadAudio);
                alive = false;
                deadParticles.Play();
                Invoke("ResetLevel", levelLoadDelay);
                break;
        }
    }

    private void ApplySound(AudioClip ac) {
        if (audioSource.isPlaying) {
            audioSource.Stop();
        }
        audioSource.PlayOneShot(ac);
    }

    private void LoadNextScene() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex == SceneManager.sceneCountInBuildSettings - 1)
            SceneManager.LoadScene(0);
        else
            SceneManager.LoadScene(currentSceneIndex + 1);
    }

    private void ResetLevel() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
