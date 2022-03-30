using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStart : MonoBehaviour
{
    [SerializeField] BoxCollider spawnArea;
    public Vector3 GetRandomSpawnPos()
    {
        Vector3 spawnExtend = spawnArea.bounds.size;
        Vector3 spawnAreaCorner = spawnArea.bounds.center - spawnArea.bounds.extents;
        Vector3 spawnPos = spawnAreaCorner + spawnExtend.normalized * Random.Range(0, spawnExtend.magnitude);
        return spawnPos;
    }
}
