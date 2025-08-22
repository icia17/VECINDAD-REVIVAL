using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerLOSController : MonoBehaviour
{
    List<GameObject> wolves;
    List<GameObject> seenWolves = new List<GameObject>();
    public GameObject closestWolf;
    public LayerMask wallLayer;
    PlayerController player;

    private void Start() {
        player = GetComponent<PlayerController>();   
    }

    private void FixedUpdate() {
        if (player.isChosen) { return; }

        wolves = GameObject.FindGameObjectsWithTag("Wolf").ToList();

        closestWolf = null;

        if (wolves.Count > 0) {
            LOSWolves();
            ClosestWolf();
        }
    }   

    private void LOSWolves() {
        seenWolves.Clear();

        foreach (var wolf in wolves) {
            Vector2 direction = wolf.transform.position - transform.position;
            float distance = direction.magnitude;

            RaycastHit2D[] rays = Physics2D.RaycastAll(transform.position, direction, distance);

            bool wolfSeen = true;

            foreach (var ray in rays) {
                if (ray.collider.CompareTag("Wall")) { 
                    wolfSeen = false;
                    break;
            }
        }

            if (wolfSeen) {
                seenWolves.Add(wolf);
            }
        }
    }

    private void ClosestWolf() {
        if (seenWolves.ToArray().Length == 0) { return; }

        closestWolf = seenWolves[0];

        foreach (var wolf in seenWolves) {
            if (Vector2.Distance(transform.position, wolf.transform.position) < Vector2.Distance(transform.position, closestWolf.transform.position)) {
                closestWolf = wolf;
            }
        }
    }
}
