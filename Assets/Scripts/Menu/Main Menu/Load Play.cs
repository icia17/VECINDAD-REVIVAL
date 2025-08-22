using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPlay : MonoBehaviour
{
    public SceneLoader sceneLoader;
    
    public void ToGame() {
        sceneLoader.ActivateScene();
    }
}
