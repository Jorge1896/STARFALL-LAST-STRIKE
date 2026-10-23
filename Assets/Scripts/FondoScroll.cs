
using UnityEngine;

public class FondoScroll : MonoBehaviour
{
    [Header("Fondos del nivel")]
    public Transform fondo1;
    public Transform fondo2;

    [Header("Configuración")]
    public float velocidad = 2f;

    [Header("Zona 1 - Bases enemigas")]
    public Sprite mapa1;
    public Sprite mapa2;

    [Header("Zona 2 - Industrial")]
    public Sprite mapa3;
    public Sprite mapa4;

    [Header("Zona 3 - Montañas")]
    public Sprite mapa5;
    public Sprite mapa6;

    [Header("Zona 4 - Orbital")]
    public Sprite mapa7;
    public Sprite mapa8;

    [Header("Jefe final")]
    public Sprite mapa9;

    [Header("Puntos para cambiar de zona")]
    public int puntosZona2 = 5000;
    public int puntosZona3 = 15000;
    public int puntosZona4 = 30000;
    public int puntosJefeFinal = 40000;

    private float alturaFondo;
    private SpriteRenderer renderer1;
    private SpriteRenderer renderer2;

    private int zonaActual = 1;
    private int siguienteMapa = 0;

    private void Start()
    {
        if (fondo1 == null || fondo2 == null)
        {
            Debug.LogError(
                "FondoScroll: asigna Fondo1 y Fondo2."
            );

            enabled = false;
            return;
        }

        renderer1 = fondo1.GetComponent<SpriteRenderer>();
        renderer2 = fondo2.GetComponent<SpriteRenderer>();

        if (renderer1 == null || renderer2 == null)
        {
            Debug.LogError(
                "FondoScroll: ambos fondos necesitan SpriteRenderer."
            );

            enabled = false;
            return;
        }

        if (renderer1.sprite == null || renderer2.sprite == null)
        {
            Debug.LogError(
                "FondoScroll: ambos fondos necesitan un sprite inicial."
            );

            enabled = false;
            return;
        }

        alturaFondo = renderer1.bounds.size.y;

        if (alturaFondo <= 0f)
        {
            Debug.LogError(
                "FondoScroll: la altura del fondo no es válida."
            );

            enabled = false;
            return;
        }

        fondo2.position = new Vector3(
            fondo1.position.x,
            fondo1.position.y + alturaFondo,
            fondo1.position.z
        );

        zonaActual = ObtenerZonaPorPuntos();
        siguienteMapa = 0;
    }

    private void Update()
    {
        int nuevaZona = ObtenerZonaPorPuntos();

        if (nuevaZona != zonaActual)
        {
            zonaActual = nuevaZona;
            siguienteMapa = 0;

            Debug.Log(
                "FondoScroll: transición a zona " + zonaActual
            );
        }

        float movimiento = velocidad * Time.deltaTime;

        fondo1.position += Vector3.down * movimiento;
        fondo2.position += Vector3.down * movimiento;

        if (fondo1.position.y <= -alturaFondo)
        {
            ReubicarFondo(fondo1, fondo2, renderer1);
        }

        if (fondo2.position.y <= -alturaFondo)
        {
            ReubicarFondo(fondo2, fondo1, renderer2);
        }
    }

    private int ObtenerZonaPorPuntos()
    {
        if (GameManager.instancia == null)
        {
            return 1;
        }

        int puntos = GameManager.instancia.puntuacion;

        if (puntos >= puntosJefeFinal)
        {
            return 5;
        }

        if (puntos >= puntosZona4)
        {
            return 4;
        }

        if (puntos >= puntosZona3)
        {
            return 3;
        }

        if (puntos >= puntosZona2)
        {
            return 2;
        }

        return 1;
    }

    private void ReubicarFondo(
        Transform fondoMover,
        Transform fondoReferencia,
        SpriteRenderer rendererMover
    )
    {
        fondoMover.position = new Vector3(
            fondoReferencia.position.x,
            fondoReferencia.position.y + alturaFondo,
            fondoMover.position.z
        );

        Sprite nuevoMapa = ObtenerSiguienteMapa();

        if (nuevoMapa != null)
        {
            rendererMover.sprite = nuevoMapa;
        }
    }

    private Sprite ObtenerSiguienteMapa()
    {
        Sprite primero = null;
        Sprite segundo = null;

        switch (zonaActual)
        {
            case 1:
                primero = mapa1;
                segundo = mapa2;
                break;

            case 2:
                primero = mapa3;
                segundo = mapa4;
                break;

            case 3:
                primero = mapa5;
                segundo = mapa6;
                break;

            case 4:
                primero = mapa7;
                segundo = mapa8;
                break;

            case 5:
                return mapa9;
        }

        Sprite elegido = siguienteMapa == 0
            ? primero
            : segundo;

        siguienteMapa = 1 - siguienteMapa;

        return elegido;
    }
}
