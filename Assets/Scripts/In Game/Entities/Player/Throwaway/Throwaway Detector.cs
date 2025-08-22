using UnityEngine;

public class ThrowawayDetector : MonoBehaviour
{
    [SerializeField] PlayerInventory inv;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Throwaway") {
            var throwaway = other.gameObject.GetComponent<PlayerThrow>();

            if (!throwaway.firstGrab) {
                throwaway.firstGrab = true;
                return;
            }
            
            inv.GetWeapon(throwaway.thrownWeapon, other.gameObject);
        }
    }
}
