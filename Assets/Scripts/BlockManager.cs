using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance;
    
    [Header("The Game Blocks:")] 
    [SerializeField] private GameObject [] blockObjects;

    public float blockSpeed;

    [Header("The Game Borders:")] 
    public Transform leftGameBorder;
    public Transform topGameBorder;
    public Transform rightGameBorder;
    public Transform bottomGameBorder;

    private int leftRightGameBorderDiff;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        leftRightGameBorderDiff = (int)(Mathf.Abs(leftGameBorder.position.x) + Mathf.Abs(rightGameBorder.position.x)) - 1;
        
        if(UIController.Instance.IsTutorialComplete) 
            SpawnBlock();
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

        if (blok.Count >= leftRightGameBorderDiff)
        {
            for (int i = 0; i < blok.Count; i++)
            {
                Debug.Log("Y: " + blok[i].transform.position.y);
                Destroy(blok[i]);
            }
            
            var uniqY = blok.GroupBy(x => x.transform.position.y).Select(x => x.First()).ToList();
            FallRemovedLineUpperBlocks(uniqY);
        }
    }

    private void FallRemovedLineUpperBlocks(List<GameObject> fallStartPosY)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.GetChild(i).childCount; j++)
            {

                for (int k = 0; k < fallStartPosY.Count; k++)
                {
                    if (transform.GetChild(i).GetChild(j).position.y > fallStartPosY[k].transform.position.y)
                    {
                        transform.GetChild(i).GetChild(j)
                            .DOMoveY(transform.GetChild(i).GetChild(j).position.y - fallStartPosY.Count, 1f)
                            .SetEase(Ease.InOutElastic)
                            .OnComplete(() =>
                            {
                                // for (int k = 0; k < transform.GetChild(i).childCount; k++)
                                // {
                                //     if (transform.GetChild(i).GetChild(k).position.y <= bottomGameBorder.position.y)
                                //     {
                                //         Destroy(transform.GetChild(i).GetChild(k).gameObject);
                                //     }
                                // }
                            });
                        //break;
                    }
                    
                }
                
            }
        }
    }
}
