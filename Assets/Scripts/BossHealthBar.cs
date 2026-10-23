
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [Header("Referencias")]
    public EnemyHealth vidaJefe;
    public Image barraRelleno;

    private void Start()
    {
        ActualizarBarra();
    }

    private void Update()
    {
        ActualizarBarra();
    }

    private void ActualizarBarra()
    {
        if (vidaJefe == null || barraRelleno == null)
        {
            return;
        }

        barraRelleno.fillAmount = vidaJefe.PorcentajeVida;
    }
}
