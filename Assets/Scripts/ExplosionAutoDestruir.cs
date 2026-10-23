
using UnityEngine;

public class ExplosionAutoDestruir : MonoBehaviour
{
    [Header("Duración de la explosión")]
    public float tiempoDeVida = 0.55f;

    private void Start()
    {
        // Destruir el objeto automáticamente
        // cuando termine la animación.
        Destroy(gameObject, tiempoDeVida);
    }
}
