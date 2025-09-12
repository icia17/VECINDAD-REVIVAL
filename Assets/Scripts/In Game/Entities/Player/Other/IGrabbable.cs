using System;
using UnityEngine;

public interface IGrabbable
{
    public void Grab(IGrabber grabber);

    public void Hold(Vector3 grabberPosition);
    
    public void Release();

    public void Squash();

    public void Grip(float damage);

    public void EnableGrab(bool enable);

    public bool CanGrab();

    public bool IsGrabbed();
}
