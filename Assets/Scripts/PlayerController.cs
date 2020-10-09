using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    bool bMousePressed ;
    Vector3 vMousePos;
    Quaternion rotation ;
    Rigidbody rigid; 
    Animator _animator;
    Plate [] plateList;

    public UIController gameUI;

    public GameObject objEnvironment;
    

    bool bStarted;
    bool bJumped;


    int iCongrated;
    int prePlateNo;
    

    // Start is called before the first frame update
    void Start()
    {

        //Init

        prePlateNo = -1;
        iCongrated = 0 ;
        bMousePressed = false;   
        rotation = Quaternion.LookRotation(new Vector3(0, 0, 0)); 
        rigid = GetComponent<Rigidbody>(); 
        rigid.useGravity = false;
        _animator = GetComponent<Animator>();
        
        bStarted = false;
        bJumped = true;

        
        //Get Plates from Environment Object.

        plateList = objEnvironment.GetComponentsInChildren<Plate>();
        for(int i = 0 ; i <plateList.Length ; i ++)
        {
            plateList[i].plateNo = i;
        }

    }

    public void SetStarted()
    {
        bStarted = true;
        _animator.SetTrigger("Jumping");
        rigid.useGravity = true;
        rigid.velocity = new Vector3( 0, 10, 0);
    }



    // Update is called once per frame
    void Update()
    {
        if( bStarted == false )
                return;

        if (Input.GetMouseButtonDown(0) )
        {
            bMousePressed = true;
            vMousePos = Input.mousePosition;
        }

        if(Input.GetMouseButtonUp(0))
        {
            bMousePressed = false;
        }

        if(bMousePressed == true)
        { 
             Rigidbody rigid = GetComponent<Rigidbody>();  
             transform.position += transform.forward * 0.2f;

             float angY = Input.mousePosition.x - vMousePos.x ;
             transform.eulerAngles  += new Vector3( 0, angY * 0.01f , 0);
             rotation = transform.rotation;
        } 
        
         transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 3.0f * Time.deltaTime ); 

         if(rigid.velocity.y < 0 && bJumped == true)
         {
             Debug.Log("Falling");
             bJumped = false; 
            rigid.velocity = new Vector3( 0, -8, 0);
         }
    }

    public void BreakAction(GameObject objToBreak)
    {
        
        foreach( Transform trans in objToBreak.transform)
        {
            if(trans.gameObject.GetComponent<MeshRenderer>())
            {
                  trans.gameObject.AddComponent<TriangleExplosion>(); 
                  StartCoroutine(trans.gameObject.GetComponent<TriangleExplosion>().SplitMesh(true));
            }
        }
        
      
    }

    private void OnTriggerEnter(Collider other)
    { 
        
        //Check if Game is started 
        if( bStarted == false )
                return;
        
        //Check if enters to plate

        if(other.tag == "Plate" || other.tag == "BreakPlate" ) //Spring Plate
        { 
            int curPlateNo = other.GetComponent<Plate>().plateNo ;
            rigid.velocity = new Vector3( 0, 10, 0); // Add Force to Rigid to Jump
            bJumped = true;
            int nextNo = curPlateNo + 1;  //Get Next Plate Number

            _animator.SetTrigger("Ground");

            other.GetComponent<Plate>().ShowParticle();

            if( curPlateNo - prePlateNo > 1 ) //Check if it's long jump
            {
                gameUI.ShowLongUI();                
            }
            else if( curPlateNo > iCongrated ) 
            {
                float fDistance = Vector3.Distance(transform.position, plateList[curPlateNo].transform.position);                

                if(fDistance < 0.5f) // Check if it's perfect jump
                {
                    gameUI.ShowPerfectUI();
                }
                else if(fDistance < 1.0f) // Check if it's good jump
                {
                    gameUI.ShowGoodUI();
                }
                
                iCongrated = curPlateNo;

            }

            if(nextNo < plateList.Length) // Check if there is a next plate
            {
                //Automatically rotate to next plate
                FaceToNextPlate(plateList[nextNo].gameObject.transform);  
            }

            prePlateNo = curPlateNo;

            if( other.tag == "BreakPlate" ) //Check if BreakPlate
            {
                //Break Plate Action
                other.gameObject.AddComponent<TriangleExplosion>(); 
                StartCoroutine(other.gameObject.GetComponent<TriangleExplosion>().SplitMesh(true));               
                BreakAction(other.gameObject);
            }
            
        }
        else if(other.tag == "End") //Check if it's the end of plate
        {
            rigid.isKinematic = true;
            transform.position += new Vector3(0, 0.5f, 0);
            _animator.SetTrigger("Land");
            gameUI.GameFinished();
        }
        else if(other.tag == "JumpPlate") //Check if it's jump plate which is located at the bottom
        { 
            rigid.velocity = new Vector3( 0, 45, 0); // Higher jump
            bJumped = true;  
              Destroy(other.gameObject, 1.0f);
            _animator.SetTrigger("Ground"); 
        }   
        
    }
 

    void OnDestroy()
    {
        gameUI.GameFaild();
    }


    void ResetGround()
    {
        _animator.ResetTrigger("Ground");
    }

    void FaceToNextPlate(Transform target)
    {   
        var lookPos = target.position - transform.position;
        lookPos.y = 0;
        rotation = Quaternion.LookRotation(lookPos);
    }
}
