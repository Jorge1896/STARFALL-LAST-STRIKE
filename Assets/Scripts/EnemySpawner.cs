
using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs de enemigos")]
    public GameObject prefabEnemigo;
    public GameObject prefabEnemigo2;
    public GameObject prefabJefeFinal;

    [Header("Puntos necesarios para cada fase")]
    public int puntosFase2 = 5000;
    public int puntosFase3 = 15000;
    public int puntosFase35 = 30000;
    public int puntosJefeFinal = 40000;

    [Header("Frecuencia de aparicion (segundos)")]
    public float tiempoFase1 = 2f;
    public float tiempoFase2 = 1.5f;
    public float tiempoFase3 = 0.8f;
    public float tiempoFase35 = 0.55f;

    [Header("Probabilidad del enemigo 2")]
    [Range(0f, 100f)]
    public float probabilidadEnemigo2 = 40f;

    [Header("Probabilidad del enemigo 2 - Fase 3.5")]
    [Range(0f, 100f)]
    public float probabilidadEnemigo2Fase35 = 55f;

    [Header("Posicion de aparicion")]
    public float margenHorizontal = 0.5f;
    public float margenSuperior = 1f;

    [Header("Aparicion del jefe final")]
    [Min(0f)]
    public float margenSuperiorJefe = 2f;

    [Header("Interfaz del jefe final")]
    public GameObject interfazJefe;
    public BossHealthBar barraVidaJefe;

    private Camera camaraPrincipal;

    // 1, 2, 3, 35 (fase 3.5) y 4 (jefe)
    private int faseActual = 1;
    private bool jefeGenerado = false;

    private void Start()
    {
        camaraPrincipal = Camera.main;

        if (interfazJefe != null)
        {
            interfazJefe.SetActive(false);
        }

        if (prefabEnemigo == null || camaraPrincipal == null)
        {
            Debug.LogError(
                "EnemySpawner: falta asignar el enemigo 1 o la camara principal."
            );

            enabled = false;
            return;
        }

        StartCoroutine(GenerarEnemigos());
    }

    private void Update()
    {
        if (GameManager.instancia == null)
        {
            return;
        }

        int puntos = GameManager.instancia.puntuacion;

        int nuevaFase = 1;

        if (puntos >= puntosJefeFinal)
        {
            nuevaFase = 4;
        }
        else if (puntos >= puntosFase35)
        {
            nuevaFase = 35;
        }
        else if (puntos >= puntosFase3)
        {
            nuevaFase = 3;
        }
        else if (puntos >= puntosFase2)
        {
            nuevaFase = 2;
        }

        if (nuevaFase != faseActual)
        {
            faseActual = nuevaFase;

            string nombreFase = faseActual == 35
                ? "3.5"
                : faseActual.ToString();

            Debug.Log(
                "STARFALL: Fase " + nombreFase + " desbloqueada."
            );
        }

        if (faseActual == 4 && !jefeGenerado)
        {
            GenerarJefeFinal();
        }
    }

    private IEnumerator GenerarEnemigos()
    {
        while (true)
        {
            if (faseActual == 4)
            {
                yield break;
            }

            GenerarEnemigoNormal();

            float tiempoEspera = tiempoFase1;

            if (faseActual == 2)
            {
                tiempoEspera = tiempoFase2;
            }
            else if (faseActual == 3)
            {
                tiempoEspera = tiempoFase3;
            }
            else if (faseActual == 35)
            {
                tiempoEspera = tiempoFase35;
            }

            yield return new WaitForSeconds(
                Mathf.Max(0.1f, tiempoEspera)
            );
        }
    }

    private void GenerarEnemigoNormal()
    {
        GameObject enemigoSeleccionado = prefabEnemigo;

        if (faseActual >= 2 && prefabEnemigo2 != null)
        {
            float probabilidadActual = probabilidadEnemigo2;

            if (faseActual == 35)
            {
                probabilidadActual = probabilidadEnemigo2Fase35;
            }

            float numeroAleatorio = Random.Range(0f, 100f);

            if (numeroAleatorio < probabilidadActual)
            {
                enemigoSeleccionado = prefabEnemigo2;
            }
        }

        Vector3 posicion = ObtenerPosicionAparicion();

        Instantiate(
            enemigoSeleccionado,
            posicion,
            Quaternion.identity
        );
    }

    private void GenerarJefeFinal()
    {
        jefeGenerado = true;

        if (prefabJefeFinal == null)
        {
            Debug.LogWarning(
                "EnemySpawner: falta asignar el prefab del jefe final."
            );
            return;
        }

        float mitadAlto = camaraPrincipal.orthographicSize;

        float limiteSuperior =
            camaraPrincipal.transform.position.y + mitadAlto;

        float mitadAltoJefe = 0f;

        SpriteRenderer spriteJefe =
            prefabJefeFinal.GetComponent<SpriteRenderer>();

        if (spriteJefe != null && spriteJefe.sprite != null)
        {
            mitadAltoJefe =
                spriteJefe.sprite.bounds.extents.y *
                Mathf.Abs(prefabJefeFinal.transform.localScale.y);
        }

        Vector3 posicionJefe = new Vector3(
            camaraPrincipal.transform.position.x,
            limiteSuperior + mitadAltoJefe + margenSuperiorJefe,
            0f
        );

        GameObject jefe = Instantiate(
            prefabJefeFinal,
            posicionJefe,
            Quaternion.identity
        );

        EnemyHealth vidaJefe =
            jefe.GetComponent<EnemyHealth>();

        if (barraVidaJefe != null)
        {
            barraVidaJefe.vidaJefe = vidaJefe;
        }

        if (interfazJefe != null)
        {
            interfazJefe.SetActive(true);
        }

        Debug.Log("¡DREADNOUGHT HA ENTRADO EN COMBATE!");
    }

    private Vector3 ObtenerPosicionAparicion()
    {
        float mitadAlto = camaraPrincipal.orthographicSize;
        float mitadAncho = mitadAlto * camaraPrincipal.aspect;

        Vector3 centro = camaraPrincipal.transform.position;

        float limiteIzquierdo =
            centro.x - mitadAncho + margenHorizontal;

        float limiteDerecho =
            centro.x + mitadAncho - margenHorizontal;

        float posicionX = centro.x;

        if (limiteIzquierdo < limiteDerecho)
        {
            posicionX = Random.Range(
                limiteIzquierdo,
                limiteDerecho
            );
        }
        else
        {
            Debug.LogWarning(
                "EnemySpawner: el margen horizontal es demasiado grande."
            );
        }

        float posicionY =
            centro.y + mitadAlto + margenSuperior;

        return new Vector3(
            posicionX,
            posicionY,
            0f
        );
    }
}
