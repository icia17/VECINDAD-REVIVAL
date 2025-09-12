public interface IGrabber
{
    public void ForceRelease(bool preventGrab);

    public bool IsGrabbing();
}