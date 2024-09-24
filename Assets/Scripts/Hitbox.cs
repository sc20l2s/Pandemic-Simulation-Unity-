using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    //UNUSED, KEPT HERE FOR REFERENCE
    private bool triggerOnce = true;
    private int count = 0;
    private int infectRate = 10; //default value in case we fail to retrieve it from supervisor
    private Human script;
    // Start is called before the first frame update
    void Start()
    {
        //infectRate = this.GetComponentInParent<Supervisor>().getInfectionRate();
        //script = this.GetComponentInParent<Human>();
        //get the heavy stuff out of the way on startup
    }

    // Update is called once per frame
    void Update()
    {
        if(!triggerOnce)
        {
            count += 1;
            if (count == 10)
            {
                triggerOnce = true;//restrict hits to 1/10 frames 
                count = 0;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //trying to avoid having a rigidbody
        if (this.GetComponentInParent<Human>().IsInfectious() && triggerOnce)
        {
            if (other.gameObject.CompareTag("Hitbox"))
            {
                var otherHuman = other.gameObject.GetComponentInParent<Human>();
                triggerOnce = false;
                if (otherHuman.IsHealthy())
                {
                    if (Random.Range(0, 100) <= infectRate)
                    {
                        otherHuman.InfectionStatus = Human.Status.Exposed;
                        //need to update the graph
                        //iunfortunately, still inefficient
                    }
                }
            }
        }
    }


}
