using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using UnityEngine;


public class CrossRoad : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 directionToAdd;//will be here mostly for debugging, so we can see it in the inspector
    public Vector3 directionToIngore;
    //this var needs some explanation, when a value is set to +1, ie, (1, 0, 0)
    //we will ignore going in the + x direction
    //when set to -1, we ignore the -x direction
    //examples:
    //ignore = (1, 0, -1) ignore the +x and -z directions
    //ignore = (0, 0, 1) ignore the +z direction
    //ignore = (0, 0, 0) ignore no directions
    //changes depending whether we are at an edge or not
    //this could be changed to store directions to a POI

    private Vector3[] allDirections = new Vector3[4];//4 possible directions
    public Vector3[] possibleDirections = new Vector3[4];
    public int totalPossible = 0;
    public bool onEdge; //this will flag whether the node is on the edge
    private bool infectousZone = false; //keeps track whether this node has a chance of infecting
    private int infectionRate = 10; //default value in case we fail to retrieve it from supervisor
    private int infectionRateOriginal = 10;
    public Supervisor Supervisor;//a reference to our supervisor
    private float infectionStart = 0;
    //someone that steps over it
    //which allows it to make humans double back
    //will be uninitialised by default, so will return false 
    void Start()
    {
        infectionRate = Supervisor.getInfectionRate();
        infectionRateOriginal = infectionRate;
        allDirections[0] = new Vector3(1, 0, 0);//our four starting directions
        allDirections[1] = new Vector3(-1, 0, 0);
        allDirections[2] = new Vector3(0, 0 ,1);
        allDirections[3] = new Vector3(0, 0, -1);
        //now we need to account for the ones that are not possible

        foreach (var element in allDirections)
        {
            if(!(element.x != 0 && element.x == directionToIngore.x))
            {
                if (!(element.z != 0 && element.z == directionToIngore.z))
                {
                    possibleDirections[totalPossible++] = element;//add to list and then increase number of possible directions
                }
            }
        }//this works, granted, but its preferable if human  is responsible for pathfinding
        //however, in order to reduce load times, we will keep it this way
        //generally, number crossroads < number humans, so doing it this way will reduce overhead
        //however, we sacrifice modularisation, humans are completely dependent on the crossroads
        
    }

    // Update is called once per frame
    void Update()
    {
        if (infectousZone)
        {
            if (infectionStart == 0)
            {
                infectionStart = Time.time + 0.5f;//stay infectous for 1/2 a second
                //Debug.Log("Infectous");
                //smaller the gap, the closer the target human, the more realistic
            }
            //would be better to have this tied to a clock, in supervisor
            //still though, that will not fix performance issues
            if (Time.time >= infectionStart)
            {
                infectousZone = false;
                //Debug.Log("No longer infectous");
                infectionRate = infectionRateOriginal; //reset to our original infection rate
            }
        }
    }

    void OnTriggerEnter(Collider coll)
    {
        {
            //some behaviour, explained
            //when the outside of our human hitbox enters the crossroads, it changes direction randomly
            //each crossroad has a set of available directions calculated at game start
            //this is for maximised efficiency
            //however, since human hitboxes are relatively large, this makes them have a slight gap between eachother when walking
            //this is intentional, to simulate both sides of the street.
            if (coll.gameObject.tag.Equals("Hitbox"))//check to see if the object is a hitbox
            {//remember it basically doubles the size

                var otherHuman = coll.GetComponentInParent<Human>();
                directionToAdd = possibleDirections[Random.Range(0, totalPossible)];
                if (otherHuman.direction != -directionToAdd || onEdge)//make sure we arent telling our human to double back, if so
                    //let the next node take care of it
                    otherHuman.direction = directionToAdd;
                //unless of course this node is an edge node
                if (otherHuman.IsInfectious())
                {
                    if (infectousZone)
                    {
                        infectionRate += infectionRateOriginal; //1 infected human = 10%, 2 infected human in .5 seconds, = 20%, etc...
                    }
                    //in order to get our cascading effect, we will make it more and more likely
                    //to infect a human for every infected human that passes by before we reset

                    infectionStart = 0;
                    infectousZone = true;
                    
                }
                else if (otherHuman.IsHealthy() && infectousZone)
                {
                    if(Random.Range(0, 100) <= infectionRate)
                    {
                        otherHuman.InfectionStatus = Human.Status.Exposed;
                        Supervisor.addGraphValues(-1, +1, +0);//update the graph
                        //Debug.Log("infected a human");
                    }
                }
            }
        }
        //the crossRoads class is an integral part of the sim
        //not only do they handle pathfinding, but the infection too
        //we create the illusion that being near an infected person has a chance of infecting a human
        //but rather, it is travelling an area that has been visitied by an infected person not long ago
        //that way, we do not need thousands of rigidbodies 
        //a LOT more efficient, but obviously not v realistic
    }
}