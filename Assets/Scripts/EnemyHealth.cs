
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Sistema de vida")]
    [Min(1)]
    public int vidaMaxima = 1;

    [Header("Puntuación")]
    [Min(0)]
    public int puntosAlDestruir = 100;

    private int vidaActual;
    private bool destruido = false;

    // Permite consultar la vida actual desde otros scripts.
    public int VidaActual
    {
        get { return vidaActual; }
    }

    // Permite consultar el porcentaje de vida restante.
    // 1 = 100%, 0.5 = 50%, 0 = 0%.
    public float PorcentajeVida
    {
        get
        {
            if (vidaMaxima <= 0)
            {
                return 0f;
            }

            return Mathf.Clamp01(
                (float)vidaActual / vidaMaxima
            );
        }
    }

    private void Awake()
    {
        vidaActual = Mathf.Max(1, vidaMaxima);
    }

    public bool RecibirDanio(int cantidad)
    {
        if (destruido)
        {
            return false;
        }

        vidaActual -= Mathf.Max(0, cantidad);

        if (vidaActual <= 0)
        {
            vidaActual = 0;
            destruido = true;

            return true;
        }

        return false;
    }
}
