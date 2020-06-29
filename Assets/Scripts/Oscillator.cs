using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {
    [SerializeField] Vector3 movementVector;
    [SerializeField] float period = 2.0f;

    [Range(0,1)] [SerializeField] 
    float movementFactor;

    Vector3 startingPosition;
    const float tau = (float)Math.PI * 2;

    void Start() {
        startingPosition = transform.position;
    }

    void FixedUpdate() {
        if (period <= Mathf.Epsilon) return;

        float cycles = Time.time / period;
        float rawSinWave = (float)Math.Sin(cycles * tau);

        movementFactor = rawSinWave / 2.0f + 0.5f;

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
    }
}
