using System;
using System.Collections;
using System.Reflection;
using DG.Tweening;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private BlockManager _blockManager;
    
    public bool userCanControl, blockCanFall;
    [SerializeField] private bool rotatable;
    [SerializeField] private bool rotateOffsetRequired;

    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material hideRotateMaterial;

    public enum movabilityDirections
    {
        left,
        right,
        bottom
    }

    private float blockInitSpeed;
    private void Start()
    { 
        _blockManager = transform.parent.GetComponent<BlockManager>();
        userCanControl = true;
        blockCanFall = true;

        blockInitSpeed = _blockManager.blockSpeed;
        // StartCoroutine(BlockFall());
    }

    private void Update()
    {
        if (userCanControl) UserInput();
        if (blockCanFall) BlockFall();
    }

    private void UserInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if(CheckMovability(movabilityDirections.left))
                transform.Translate(Vector3.left, Space.World);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(CheckMovability(movabilityDirections.right))
                transform.Translate(Vector3.right, Space.World);
        }
        else if(Input.GetKeyDown(KeyCode.Space) && rotatable)
        {
            var ghostRotateBlock = Instantiate(gameObject, transform.position, transform.rotation);
            Destroy(ghostRotateBlock.GetComponent<BlockController>());
                
            foreach (Transform child in ghostRotateBlock.transform)
                child.GetComponent<Renderer>().material = hideRotateMaterial;

            if (isRotatable(ghostRotateBlock.transform))
            {
                transform.eulerAngles += Vector3.forward * 90;
                if (rotateOffsetRequired)
                    transform.position += transform.position.x < 0 ? Vector3.left * 0.5f : Vector3.right * 0.5f;
            }
            Destroy(ghostRotateBlock, 3f);
        }
        
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            _blockManager.blockSpeed = blockInitSpeed * 3f;
        }
        else
        {
            _blockManager.blockSpeed = blockInitSpeed;
        }
    }
    private void BlockFall()
    { 
        if (CheckMovability(movabilityDirections.bottom))
            transform.Translate(Vector3.down * Time.deltaTime * _blockManager.blockSpeed, Space.World);
    }

    // private IEnumerator BlockFall()
    // {
    //     while (controllerIsActive)
    //     {
    //         if (CheckMovability(movabilityDirections.bottom))
    //             transform.Translate(Vector3.down, Space.World);
    //         
    //         yield return new WaitForSeconds(1/_blockManager.blockSpeed);
    //     }
    // }

    private void SetMaterial(Material mat)
    {
        foreach (Transform child in transform)
            child.GetComponent<Renderer>().material = mat;
    }
    

    private bool CheckMovability (Enum movDirection)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.parent.childCount-1; j++)
            {
                for (int k = 0; k < transform.parent.GetChild(j).childCount; k++)
                {
                    var blockDifferenceX = transform.GetChild(i).position.x - transform.parent.GetChild(j).GetChild(k).position.x;
                    blockDifferenceX = Mathf.Round(blockDifferenceX * 10) / 10;
                    
                    var blockDifferenceY = Math.Abs(transform.GetChild(i).position.y - transform.parent.GetChild(j).GetChild(k).position.y);
                    blockDifferenceY = Mathf.Round(blockDifferenceY * 10) / 10;
                    
                    var blockDiffNotEnough=false;
                    switch (movDirection) //switch case for active block object interaction with other blocks
                    {
                        case movabilityDirections.left:
                            blockDiffNotEnough = (blockDifferenceX <= 1 && blockDifferenceX > -1);
                            break;
                        case movabilityDirections.right:
                            blockDiffNotEnough = (blockDifferenceX < 1 && blockDifferenceX >= -1);
                            break;
                        case movabilityDirections.bottom:
                            blockDifferenceX = Mathf.Abs(blockDifferenceX);
                            blockDiffNotEnough = blockDifferenceX <= 0;
                            break;
                    }
                    
                    if (blockDiffNotEnough && blockDifferenceY <= 1)
                    {
                        
                        var currPosY = Mathf.Round(transform.position.y*2)/2;
                        transform.position = new Vector3(transform.position.x, currPosY, transform.position.z);
                        
                        if (movDirection.Equals(movabilityDirections.bottom))
                        {
                            Debug.Log("here 1");
                            blockCanFall = false;
                            BlockFallCompleted();
                        }
                        userCanControl = false;
                        
                        return false;
                    }
                }
            }
            
            var difference = 0f;
            switch (movDirection) //switch case for active block object interaction with game borders
            {
                case movabilityDirections.left:
                    difference = Math.Abs(transform.GetChild(i).position.x - _blockManager.leftGameBorder.position.x);
                    
                    break;
                case movabilityDirections.right:
                    difference = Math.Abs(transform.GetChild(i).position.x - _blockManager.rightGameBorder.position.x);
                    
                    break;
                case movabilityDirections.bottom:
                    difference = Math.Abs(transform.GetChild(i).position.y - _blockManager.bottomGameBorder.position.y);
                    
                    break;
            }
            if (difference <= 1)
            {
                var currPosY = Mathf.Round(transform.position.y*2)/2;
                transform.position = new Vector3(transform.position.x, currPosY, transform.position.z);
                
                
                if (movDirection.Equals(movabilityDirections.bottom))
                {
                    Debug.Log("here 2");
                    blockCanFall = false;
                    BlockFallCompleted();
                }
                
                userCanControl = false;
                
                return false;
            }
            
        }
        userCanControl = true;
        return true;
    }

    private void BlockFallCompleted()
    {
        _blockManager.blockSpeed = blockInitSpeed;

        transform.DOShakeScale(0.5f).SetEase(Ease.InOutElastic);
        
        SetMaterial(originalMaterial);

        _blockManager.CheckBlockLine();
        
        if (transform.position.y >= _blockManager.topGameBorder.position.y)
        {
            Debug.Log("Game Over!");
            Time.timeScale = 0;
            return;
        }
        
        
        _blockManager.SpawnBlock();
    }


    private bool isRotatable(Transform ghostObject)
    {
        ghostObject.eulerAngles += Vector3.forward * 90;
        for (int i = 0; i < ghostObject.childCount; i++)
        {
            var leftGameBorderDiff =
                Math.Abs(ghostObject.GetChild(i).position.x - _blockManager.leftGameBorder.position.x);
            var rightGameBorderDiff =
                Math.Abs(ghostObject.GetChild(i).position.x - _blockManager.rightGameBorder.position.x);
            var bottomGameBorderDiff =
                Math.Abs(ghostObject.GetChild(i).position.y - _blockManager.bottomGameBorder.position.y);
            
            if (leftGameBorderDiff <= 1 || rightGameBorderDiff <= 1 || bottomGameBorderDiff <= 1)
            {
                //Debug.Log("false: rotate border diff <= 0");
                return false;
            }

            Debug.Log("Active Obj: " + ghostObject.GetChild(i).name);
            for (int j = 0; j < transform.parent.childCount-1; j++)
            {
                for (int k = 0; k < transform.parent.GetChild(j).childCount; k++)
                {
                    var blockDifferenceX = Math.Abs(ghostObject.GetChild(i).position.x - transform.parent.GetChild(j).GetChild(k).position.x);
                    //blockDifferenceX = Mathf.Round(blockDifferenceX * 10) / 10;
                    var blockDifferenceY = Math.Abs(ghostObject.GetChild(i).position.y - transform.parent.GetChild(j).GetChild(k).position.y);
                    //blockDifferenceY = Mathf.Round(blockDifferenceY * 10) / 10;
                    
                    Debug.Log("Selected Obj: " + transform.parent.GetChild(j).GetChild(k).name);
                    Debug.Log("Block Diff X: "+blockDifferenceX + " ve Block Diff Y: " +blockDifferenceY);

                    if (blockDifferenceX < 0 || blockDifferenceY < 0)
                    {
                        //Debug.Log("false: block diff <= 0");
                        return false;
                    }
                }
            }
        }
        

        return true;
    }
}
