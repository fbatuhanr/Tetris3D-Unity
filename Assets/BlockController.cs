using System;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private BlockManager _blockManager;
    
    public bool controllerIsActive;
    [SerializeField] private bool rotateOffsetRequired;

    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material hideRotateMaterial;

    public enum movabilityDirections
    {
        left,
        right,
        bottom,
        rotate
    }

    private void Start()
    { 
        _blockManager = transform.parent.GetComponent<BlockManager>();
        
        controllerIsActive = true;
    }

    private void Update()
    {
        if (controllerIsActive)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if(CheckMovability(movabilityDirections.left))
                    transform.Translate(Vector3.left, Space.World);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if(CheckMovability(movabilityDirections.right))
                    transform.Translate(Vector3.right, Space.World);
            }
            else if(Input.GetKeyDown(KeyCode.Space))
            {
                var ghostRotateBlock = Instantiate(gameObject, transform.position, transform.rotation);
                Destroy(ghostRotateBlock.GetComponent<BlockController>());
                
                foreach (Transform child in ghostRotateBlock.transform)
                    child.GetComponent<Renderer>().material = hideRotateMaterial;

                if (isRotatable(ghostRotateBlock.transform))
                {
                    transform.eulerAngles += Vector3.forward * 90;
                }
                Destroy(ghostRotateBlock, 3f);
            }
        }
        
        if (CheckMovability(movabilityDirections.bottom))
            transform.Translate(Vector3.down * Time.deltaTime * _blockManager.blockSpeed, Space.World);
    }

    private void FixedUpdate()
    {
    }

    private void SetMaterial(Material mat)
    {
        foreach (Transform child in transform)
            child.GetComponent<Renderer>().material = mat;
    }
    

    private bool CheckMovability (Enum movDirection)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
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
                controllerIsActive = false;
                return false;
            }
            
            
            for (int j = 0; j < transform.parent.childCount-1; j++)
            {
                for (int k = 0; k < transform.parent.GetChild(j).childCount; k++)
                {
                    var blockDifferenceX = transform.GetChild(i).position.x - transform.parent.GetChild(j).GetChild(k).position.x;
                    var blockDifferenceY = Math.Abs(transform.GetChild(i).position.y - transform.parent.GetChild(j).GetChild(k).position.y);
                    
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
                            blockDifferenceX = Math.Abs(blockDifferenceX);
                            blockDiffNotEnough = blockDifferenceX <= 0;
                            break;
                    }
                    
                    if (blockDiffNotEnough && blockDifferenceY <= 1)
                    {
                        //if(movDirection == (Enum)movabilityDirections.left || movDirection == (Enum)movabilityDirections.right)
                        controllerIsActive = false;
                        return false;
                    }
                }
            }
            
        }
        controllerIsActive = true;
        return true;
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
            
            if (leftGameBorderDiff <= 0 || rightGameBorderDiff <= 0 || bottomGameBorderDiff <= 0)
            {
                Debug.Log("false: rotate border diff <= 0");
                return false;
            }
            
            
            for (int j = 0; j < transform.parent.childCount-1; j++)
            {
                for (int k = 0; k < transform.parent.GetChild(j).childCount; k++)
                {
                    var blockDifferenceX = Math.Abs(ghostObject.GetChild(i).position.x - transform.parent.GetChild(j).GetChild(k).position.x);
                    var blockDifferenceY = Math.Abs(ghostObject.GetChild(i).position.y - transform.parent.GetChild(j).GetChild(k).position.y);

                    Debug.Log("Block Diff X: "+blockDifferenceX + " ve Block Diff Y: " +blockDifferenceY);
                    if (blockDifferenceX < 0 || blockDifferenceY < 0)
                    {
                        Debug.Log("false: block diff <= 0");
                        return false;
                    }
                }
            }
        }
        

        return true;
    }
}
