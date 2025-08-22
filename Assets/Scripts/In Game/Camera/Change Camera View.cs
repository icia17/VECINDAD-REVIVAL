using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ChangeCameraView : MonoBehaviour
{
    [Header("Chosen Follower Object")]
    [SerializeField] GameObject chosenFollow;
    
    CinemachineVirtualCamera cam;

    private void Start() {
        cam = GetComponent<CinemachineVirtualCamera>();    
    }

    private void FixedUpdate() {
        cam.Follow = chosenFollow.transform;
    }
}
