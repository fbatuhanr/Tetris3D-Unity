using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockManager : MonoBehaviour
{
    [SerializeField] private GameObject [] blocks;
    private void Start()
    {
        SpawnBlock();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SpawnBlock();
    }

    private void SpawnBlock()
    {
        var spawnedBlock = Instantiate(blocks[GetRandomBlockIndex()], Vector3.zero, Quaternion.identity);
        spawnedBlock.transform.SetParent(transform);
    }

    private int GetRandomBlockIndex()
    {
        return Random.Range(0, blocks.Length);
    }
}
