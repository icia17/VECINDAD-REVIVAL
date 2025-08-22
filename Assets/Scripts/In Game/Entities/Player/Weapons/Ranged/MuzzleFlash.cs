using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MuzzleFlash : MonoBehaviour
{
    Light2D muzzleFlash;

    void Awake()
    {
        muzzleFlash = GetComponent<Light2D>();
    }

    private void FixedUpdate() {
        if (muzzleFlash.intensity > 0) {
            muzzleFlash.intensity -= 55 * Time.deltaTime;
        } else {
            muzzleFlash.intensity = 0;
        }
    }

    public void ZeroIntensity() {
        muzzleFlash.intensity = 0;
    }
}
