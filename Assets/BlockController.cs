using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    private bool isActiveBlock;
    
    private Transform gameBorders;
    private Transform leftGameBorder, rightGameBorder, bottomGameBorder;

    private void Start()
    {
        isActiveBlock = true;
        
        gameBorders = GameObject.Find("Game Borders").transform;
        
        leftGameBorder = gameBorders.GetChild(3).transform;
        rightGameBorder = gameBorders.GetChild(1).transform;
        bottomGameBorder = gameBorders.GetChild(2).transform;
    }

    private float speed = 1;
    private void Update()
    {
        if (isActiveBlock)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if(CheckMovability(0))
                    transform.Translate(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if(CheckMovability(1))
                    transform.Translate(Vector3.right);
            }
            
            if(Input.GetKeyDown(KeyCode.Space))
            {
                speed *= 2;
            }
        }
    }

    private void FixedUpdate()
    {
        if (CheckMovability(2))
            transform.Translate(Vector3.down * Time.fixedDeltaTime * speed);
        else
            isActiveBlock = false;
    }

    private bool CheckMovability (int lr)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var difference=0f;
            switch (lr)
            {
                case 0:
                    difference = Math.Abs(transform.GetChild(i).position.x - leftGameBorder.position.x);
                    //Debug.Log("left " + transform.GetChild(i).name+ ": " + difference);
                    if (difference <= 1)
                        return false;
                    break;
                case 1:
                    difference = Math.Abs(transform.GetChild(i).position.x - rightGameBorder.position.x);
                    //Debug.Log("right " + transform.GetChild(i).name+ ": " + difference);
                    if (difference <= 1)
                        return false;
                    break;
                case 2:
                    difference = Math.Abs(transform.GetChild(i).position.y - bottomGameBorder.position.y);
                    //Debug.Log("bottom " + transform.GetChild(i).name+ ": " + difference);
                    if (difference <= 1)
                        return false;
                    
                    
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
                            
                            Debug.Log("X: "+ blockDifferenceX + " Y:" + blockDifferenceY);
                            
                            if (blockDifferenceX <= 0 && blockDifferenceY <= 1)
                                return false;
                        }
            
                    }
                    
                    
                    break;
            }
        }


        return true;
    }
}
