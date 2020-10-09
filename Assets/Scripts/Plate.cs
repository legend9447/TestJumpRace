using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public int plateNo;
    public ParticleSystem novaEffect;
    // Start is called before the first frame update 

    public void ShowParticle() //Public method which is called by playercontroller
    {
        novaEffect.Play();
    }
 
}
