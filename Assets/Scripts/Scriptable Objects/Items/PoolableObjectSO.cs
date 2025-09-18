
using UnityEngine;

[CreateAssetMenu(fileName = "NewPoolableObject", menuName = "Juego/Tipo de Objeto para Pool")]
public class PoolableObjectSO : ScriptableObject
{
    [Header("Datos para la Pool")]
    public GameObject prefab;
    public int poolSize = 10; 

   
}