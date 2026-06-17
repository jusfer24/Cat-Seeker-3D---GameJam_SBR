using System.Collections.Generic;
using UnityEngine;

public class EndlessLevelManager : MonoBehaviour
{
    [Header("Configuración del Entorno")]
    [Tooltip("Velocidad a la que se mueven los chunks hacia el jugador (Eje X)")]
    public float velocidadEntorno = 10f;
    [Tooltip("Posición en X negativa donde el chunk desaparece")]
    public float limiteDesaparicionX = -40f;

    [Header("Referencias de Chunks")]
    public GameObject[] prefabsChunks;
    public int chunksIniciales = 6;

    private List<GameObject> poolChunks = new List<GameObject>();
    private Queue<GameObject> chunksActivos = new Queue<GameObject>();
    private Transform ultimoEndPosition;

    void Start()
    {
        GenerarChunkAleatorio(Vector3.zero);

        for (int i = 1; i < chunksIniciales; i++)
        {
            GenerarChunkAleatorio(ultimoEndPosition.position);
        }
    }

    void Update()
    {
        MoverChunks();
        ComprobarLimites();
    }

    void MoverChunks()
    {
        foreach (GameObject chunk in chunksActivos)
        {
            // Se mueve estrictamente hacia la izquierda (X negativo)
            chunk.transform.Translate(Vector3.left * velocidadEntorno * Time.deltaTime, Space.World);
        }
    }

    void ComprobarLimites()
    {
        GameObject chunkMasAntiguo = chunksActivos.Peek();

        if (chunkMasAntiguo.transform.position.x <= limiteDesaparicionX)
        {
            // Lo sacamos de los activos para que deje de moverse
            chunksActivos.Dequeue();

            // Lo apagamos para reciclarlo (Object Pool) sin causar lag
            chunkMasAntiguo.SetActive(false);

            GenerarChunkAleatorio(ultimoEndPosition.position);
        }
    }

    void GenerarChunkAleatorio(Vector3 posicionDeSpawn)
    {
        // SEGURO CONTRA DESVIACIONES: 
        // Forzamos a que Y y Z siempre sean 0. Así aseguramos movimiento estrictamente lineal en X.
        Vector3 posicionCorregida = new Vector3(posicionDeSpawn.x, 0f, 0f);

        int indiceAleatorio = Random.Range(0, prefabsChunks.Length);
        GameObject prefabSeleccionado = prefabsChunks[indiceAleatorio];

        GameObject nuevoChunk = ObtenerDelPool(prefabSeleccionado.name);

        if (nuevoChunk == null)
        {
            nuevoChunk = Instantiate(prefabSeleccionado);
            nuevoChunk.name = prefabSeleccionado.name;
            poolChunks.Add(nuevoChunk);
        }

        // Usamos la posición corregida en lugar de la original
        nuevoChunk.transform.position = posicionCorregida;
        nuevoChunk.SetActive(true);
        chunksActivos.Enqueue(nuevoChunk);

        ChunkData data = nuevoChunk.GetComponent<ChunkData>();
        if (data != null && data.endPosition != null)
        {
            ultimoEndPosition = data.endPosition;
        }
        else
        {
            Debug.LogError($"El chunk {nuevoChunk.name} no tiene ChunkData o falta el EndPosition.");
        }
    }

    GameObject ObtenerDelPool(string nombrePrefab)
    {
        foreach (GameObject chunk in poolChunks)
        {
            if (!chunk.activeInHierarchy && chunk.name == nombrePrefab)
            {
                return chunk;
            }
        }
        return null;
    }
}