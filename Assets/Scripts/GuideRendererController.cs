using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideRendererController : MonoBehaviour
{
    public Material mat01;
    public Material mat02;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        When player is entered to Plate shadow must be green
        */

       if(other.tag == "Plate" || other.tag == "BreakPlate" || other.tag == "JumpPlate" )
       {
           GetComponent<MeshRenderer>().material = mat01;    
            
       }
    }

    private void OnTriggerExit(Collider other)
    {
        
        /*
        When player is exited from plate shadow must be red
        */
        
       if(other.tag == "Plate" || other.tag == "BreakPlate" || other.tag == "JumpPlate" )
       {
           GetComponent<MeshRenderer>().material = mat02;
           
       }
    }


}
