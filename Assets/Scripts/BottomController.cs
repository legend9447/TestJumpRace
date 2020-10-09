using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomController : MonoBehaviour
{
    public GameObject objVfx;
    // Start is called before the first frame update
 
    private void OnTriggerEnter(Collider other)
    {
        //Check if player is entered to bottom ( Once player falls down)
        if(other.tag == "Player")
        {
            if(other.gameObject.GetComponent<PlayerController>()) //Check if it's really player component
            {
                Camera.main.transform.SetParent(null); 

                //Spawn Destroy Effect
                GameObject go = Instantiate(objVfx, other.transform.position + new Vector3(0, 1, 0),   Quaternion.Euler(-90, 0, 0)) as GameObject ;  

                Destroy ( other.gameObject ); 
            }           
        }
    }
}
