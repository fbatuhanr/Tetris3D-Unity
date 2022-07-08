using System;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private BlockManager _blockManager;
    
    public bool controllerIsActive;
    private float speed = 1;
    
    public enum movabilityDirections
    {
        left,
        right,
        bottom
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
                    transform.Translate(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if(CheckMovability(movabilityDirections.right))
                    transform.Translate(Vector3.right);
            }
            
            if(Input.GetKeyDown(KeyCode.Space))
            {
                _blockManager.blockSpeed *= 2;
            }
        }
    }

    private void FixedUpdate()
    {
        if (CheckMovability(movabilityDirections.bottom))
            transform.Translate(Vector3.down * Time.fixedDeltaTime * _blockManager.blockSpeed);
    }
    

    private bool CheckMovability (Enum movDirection)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var difference=0f;
            switch (movDirection)
            {
                case movabilityDirections.left:
                    
                    difference = Math.Abs(
                        transform.GetChild(i).position.x - 
                        _blockManager.leftGameBorder.position.x);
                    Debug.Log("Left Border Diff: "+difference);

                    if (difference <= 1)
                    {
                        controllerIsActive = false;
                        return false;
                    }
                    
                    // childCount-1 meaning not check last one (active object)
                    for (int j = 0; j < transform.parent.childCount-1; j++)
                    {
                        Debug.Log(transform.parent.GetChild(j).name + ": ");
                        for (int k = 0; k < transform.parent.GetChild(j).childCount; k++)
                        {
                            var blockDifferenceX =
                                transform.GetChild(i).position.x -
                                transform.parent.GetChild(j).GetChild(k).position.x;

                            var blockDifferenceY = Math.Abs(
                                transform.GetChild(i).position.y -
                                transform.parent.GetChild(j).GetChild(k).position.y);
                            
                            Debug.Log(
                                "Aktif:"+transform.GetChild(i).name + ", Pos X: " + transform.GetChild(i).position.x 
                                + " ve " + 
                                "Seçilen: "+transform.parent.GetChild(j).GetChild(k).name + ", Seçilen Pos X: " + transform.parent.GetChild(j).GetChild(k).position.x
                                + ", X Fark: "+blockDifferenceX
                                + " ve Y Fark: " + blockDifferenceY
                                );
                
                            //blockDifferenceX < 1
                            if ((blockDifferenceX <= 1 && blockDifferenceX > -1) && blockDifferenceY <= 1)
                            {
                                controllerIsActive = false;
                                return false;
                            }
                        }
                    }
                    
                    break;
                case movabilityDirections.right:
                    
                    difference = Math.Abs(
                        transform.GetChild(i).position.x - 
                        _blockManager.rightGameBorder.position.x);
                    Debug.Log("Right Border Diff: "+difference);
                    
                    if (difference <= 1)
                    {
                        controllerIsActive = false;
                        return false;
                    }
                    
                    
                    // childCount-1 meaning not check last one (active object)
                    for (int j = 0; j < transform.parent.childCount-1; j++)
                    {
                        Debug.Log(transform.parent.GetChild(j).name + ": ");
                        for (int k = 0; k < transform.parent.GetChild(j).childCount; k++)
                        {
                            var blockDifferenceX =
                                transform.GetChild(i).position.x -
                                transform.parent.GetChild(j).GetChild(k).position.x;
                            
                            var blockDifferenceY = Math.Abs(
                                transform.GetChild(i).position.y -
                                transform.parent.GetChild(j).GetChild(k).position.y);
                            
                            Debug.Log(
                                "Aktif:"+transform.GetChild(i).name + ", Pos X: " + transform.GetChild(i).position.x 
                                + " ve " + 
                                "Seçilen: "+transform.parent.GetChild(j).GetChild(k).name + ", Seçilen Pos X: " + transform.parent.GetChild(j).GetChild(k).position.x
                                + ", X Fark: "+blockDifferenceX
                                + " ve Y Fark: " + blockDifferenceY
                            );

                            if ((blockDifferenceX < 1 && blockDifferenceX >= -1) && blockDifferenceY <= 1)
                            {
                                controllerIsActive = false;
                                return false;
                            }
                        }
                    }
                    
                    break;
                case movabilityDirections.bottom:
                    
                    difference = Math.Abs(
                        transform.GetChild(i).position.y - 
                        _blockManager.bottomGameBorder.position.y);
                    
                    if (difference <= 1)
                    {
                        controllerIsActive = false;
                        return false;
                    }
                    
                    // childCount-1 meaning not check last one (active object)
                    for (int j = 0; j < transform.parent.childCount-1; j++)
                    {
                        for (int k = 0; k < transform.parent.GetChild(j).childCount; k++)
                        {
                            var blockDifferenceX = Math.Abs(
                                transform.GetChild(i).position.x -
                                transform.parent.GetChild(j).GetChild(k).position.x
                            );
                            
                            var blockDifferenceY = Math.Abs(
                                transform.GetChild(i).position.y -
                                transform.parent.GetChild(j).GetChild(k).position.y);
                            
                            //Debug.Log("X: "+ blockDifferenceX + " Y:" + blockDifferenceY);

                            if (blockDifferenceX <= 0 && blockDifferenceY <= 1)
                            {
                                controllerIsActive = false;
                                return false;
                            }
                        }
                    }
                    break;
            }
        }
        controllerIsActive = true;
        return true;
    }
}
