using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MuzzleFlash : MonoBehaviour
{
    private Light2D muzzleFlash;

    void Awake()
    {
        if (!TryGetComponent(out muzzleFlash))
        {
            Logger.Log("Light2D Source not found!");
        }
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
