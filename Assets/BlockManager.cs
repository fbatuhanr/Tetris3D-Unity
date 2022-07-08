using UnityEngine;
using Random = UnityEngine.Random;

public class BlockManager : MonoBehaviour
{
    [Header("The Game Blocks:")] 
    [SerializeField] private GameObject [] blockObjects;

    public float blockSpeed;

    [Header("The Game Borders:")] 
    public Transform leftGameBorder;
    public Transform rightGameBorder;
    public Transform bottomGameBorder;
    
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
        var spawnedBlock = Instantiate(blockObjects[GetRandomBlockIndex()], Vector3.up*10, Quaternion.identity);
        spawnedBlock.transform.SetParent(transform);
    }

    private int GetRandomBlockIndex()
    {
        return Random.Range(0, blockObjects.Length);
    }
}
