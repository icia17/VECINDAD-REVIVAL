
using UnityEngine;

[CreateAssetMenu(fileName = "NewPoolableObject", menuName = "Pooling/Poolable Object")]
public class PoolableObjectSO : ScriptableObject
{
    [Header("Datos para la Pool")]
    public GameObject prefab;
    public int poolSize = 10; 
}