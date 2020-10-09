using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayerController : MonoBehaviour
{
     
     public enum AILevel
     {
         easy, normal, hard
     };
    Rigidbody rigid;  
    Quaternion rotation;
    Animator    _animator ;
    float smoothTime = 1;
    Vector3 targetPosition;
    public UIController gameUI;

    Plate [] plateList;
    public GameObject objEnvironment;
    public AILevel cpuLvl;
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    { 
        //Init 

        rotation = Quaternion.LookRotation(new Vector3(0, 0, 0)); 
        rigid = GetComponent<Rigidbody>();  
        _animator = GetComponent<Animator>();
        targetPosition = transform.position;
        rigid.useGravity = false;

        
        //Get Plates from Environment Object.

        plateList = objEnvironment.GetComponentsInChildren<Plate>();
        for(int i = 0 ; i <plateList.Length ; i ++)
        {
            plateList[i].plateNo = i;
        }
    }

    // Update is called once per frame
    
    void Update()
    { 
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 3.0f * Time.deltaTime );         
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    } 
    
    public void SetStarted()
    {        
        _animator.SetTrigger("Jumping");
        rigid.useGravity = true;
        rigid.velocity = new Vector3( 0, 10, 0);
    }

    private void OnTriggerEnter(Collider other)
    { 

        //Check if enters to plate

        if( other.tag == "Plate" || other.tag == "BreakPlate") //Spring Plate
        { 
            rigid.velocity = new Vector3( 0, 10, 0); // Add Force to Rigid to Jump
            int nextNo = other.GetComponent<Plate>().plateNo + 1;  //Get Next Plate Number 
            _animator.SetTrigger("Ground"); 

            if(nextNo < plateList.Length)
            {
                //Automatically Rotate to Next Plate
                FaceToNextPlate(plateList[nextNo].gameObject.transform);  


                int rand = 0;

                // Set Jumping Chance wiht AI Level

                rand = UnityEngine.Random.Range(1, 3 + cpuLvl - AILevel.easy); 

                
                if(rand == 1)
                {
                    targetPosition = plateList[nextNo].gameObject.transform.position;
                    targetPosition += new Vector3(0, 0.5f, 0);
                }
            }

        }
        
        else if(other.tag == "End") // End Plate
        {
            rigid.isKinematic = true;
            _animator.SetTrigger("Land"); //Land
            gameUI.AIFinished( );
        }
    }


    void ResetGround()
    {
        _animator.ResetTrigger("Ground"); // Reset Animation
    }

    void FaceToNextPlate(Transform target)
    {   
        var lookPos = target.position - transform.position;
        lookPos.y = 0;
        rotation = Quaternion.LookRotation(lookPos);
    }
}
