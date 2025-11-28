using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowData : MonoBehaviour {
    public Vector3 position { get; private set; }

    private void Awake() {
        position = transform.localPosition;
    }

    public void ResetPosition() {
        transform.localPosition = position;
    }
}
