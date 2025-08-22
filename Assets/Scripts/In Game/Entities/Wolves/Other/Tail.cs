using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    public int length;
    public LineRenderer lineRend;
    public Vector3[] segmentPoses;
    public Vector3[] segmentV;

    public Transform targetDir;
    public float targetDist;
    public float smoothSpeed;
    public float trailSpeed;

    public float wiggleSpeed;
    public float wiggleMagnitude;
    public Transform wiggleDir;

    private void Start() {
        lineRend.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];

        for (int i = 0; i < segmentPoses.Length; i++) {
            segmentPoses[i] = targetDir.position;
            segmentV[i] = targetDir.position;
        }

        StartCoroutine(ShowTail());
    }

    private void FixedUpdate() {
        wiggleDir.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude);

        segmentPoses[0] = targetDir.position;

        for (int i = 1; i < segmentPoses.Length; i++) {
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i - 1] + targetDir.right * targetDist, ref segmentV[i], smoothSpeed + i / trailSpeed);
        }

        lineRend.SetPositions(segmentPoses);
    }

    private IEnumerator ShowTail() {
        yield return new WaitForSeconds(1);

        lineRend.sortingOrder = 8;
    }
}
