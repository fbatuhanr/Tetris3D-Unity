using System.Collections.Generic;
using System.Linq;
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

    private int leftRightGameBorderDiff;
    
    private void Start()
    {
        leftRightGameBorderDiff = (int)(Mathf.Abs(leftGameBorder.position.x) + Mathf.Abs(rightGameBorder.position.x)) - 1;
        SpawnBlock();
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.R)) SpawnBlock();
    }

    public void SpawnBlock()
    {
        var spawnedBlock = Instantiate(blockObjects[GetRandomBlockIndex()], Vector3.up*10, Quaternion.identity);
        spawnedBlock.transform.SetParent(transform);
    }

    private int GetRandomBlockIndex()
    {
        return Random.Range(0, blockObjects.Length);
    }

    public void CheckBlockLine()
    {
        List<GameObject> blocks = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.GetChild(i).childCount; j++)
            {
                blocks.Add(transform.GetChild(i).GetChild(j).gameObject);
            }
        }

        Debug.Log("left right border diff: " + leftRightGameBorderDiff);
        
        var blok = blocks.GroupBy(x => x.transform.position.y)        // Group by name
            .Where(g => g.Count() >= leftRightGameBorderDiff)   // Select only groups having duplicates
            .SelectMany(g => g).ToList();
        
        
        for (int i = 0; i < blok.Count; i++)
        {
            Debug.Log("Y: " + blok[i].transform.position.y);
            Destroy(blok[i]);
        }
        
        if (blok.Count >= leftRightGameBorderDiff)
        {
            for (int i = 0; i < blok.Count; i++)
            {
                Debug.Log("Y: " + blok[i].transform.position.y);
                Destroy(blok[i]);
            }
            FallRemovedLineUpperBlocks(blok[0].transform.position.y);
        }
    }

    private void FallRemovedLineUpperBlocks(float fallStartPosY)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.GetChild(i).childCount; j++)
            {
                if(transform.GetChild(i).GetChild(j).position.y > fallStartPosY)
                    transform.GetChild(i).GetChild(j).position += Vector3.down;
            }
        }
    }
}
