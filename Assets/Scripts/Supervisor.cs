using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Supervisor : MonoBehaviour
{
    private HashSet<Human> allHumans;
    private HashSet<CrossRoad> allNodes;
    
    public GameObject humanPrefab;
    public GameObject graphReference;
    public GameObject crossRoadsPrefab;
    public GameObject building;
    
    //TODO:
    //add a menu to modify values on simulation startup
    //plane? w text? how to add slider?
    //modify world size in menu
    //guy mentiones something abouit social distancing, how to implement?
    //the following numbers will need to be set in a menu before the game is run
    public static int TOTALHUMANS = 2000;//set it for one to test pathfinding
    public static int WORLDRADIUS = 100;//unused
    public static int TOTALCROSSROADS = 20;//diameter of grid of crossroads
    public static int crossRoadsGap = 10;
    public static int incubationTime = 10;//arbitrary number, in seconds
    public static int infectionRate = 4;//in percent
    public static int recoveryTime = 15;//how long an infected human takes to immunize, in seconds, disabled if 0
    //given default values for the first time the app is opened


    //down here we will keep our default values, immutable, for the reset button
    private const int humanDefault = 2000;
    private const int crossRoadsDefault = 20;
    private const int incubationTimeDefault = 10;
    private const int infectionRateDefault = 4;
    private const int recoveryTimeDefault = 15;
    //end of variable declaration

    // Start is called before the first frame update
    void Start()
    {
        allNodes = new HashSet<CrossRoad>();

        //now to instantiate our grid of nodes, will be used for pathfinding
        for (int i = 0; i < TOTALCROSSROADS; i++)
        {
            for (int j = 0; j < TOTALCROSSROADS; j++)
            {
                var position = new Vector3(j * crossRoadsGap, 0, i * crossRoadsGap);
                var toIgnore = new Vector3(0, 0, 0);
                var newGridObject = Instantiate(crossRoadsPrefab);//instantiate object
                var newGridNode = newGridObject.GetComponent<CrossRoad>();

                for(int k = 2; k < crossRoadsGap-2; k+=4)
                {
                    for (int m = 2; m < crossRoadsGap - 2; m += 4)
                    {
                        var buildPos = new Vector3(j * crossRoadsGap + k, Random.Range(0,5), i * crossRoadsGap + m);
                        //the random y offset will give us buildings of varying height
                        var newBuildObject = Instantiate(building);
                        //var buildingScript = newBuildObject.GetComponent<Decor>(); used to have a decor script
                        //removed for efficiency
                        addBuilding(newBuildObject, buildPos);
                        //a bit skewed
                    }
                }

                //check whether we are on one of our edges
                if(j == 0)
                {
                    toIgnore.x = -1;
                    newGridNode.onEdge = true;
                }
                else if (j== TOTALCROSSROADS-1)
                {
                    toIgnore.x = 1;
                    newGridNode.onEdge = true;
                }

                if (i == 0)
                {
                    toIgnore.z = -1;
                    newGridNode.onEdge = true;
                }
                else if (i == TOTALCROSSROADS - 1)
                {
                    toIgnore.z = 1;
                    newGridNode.onEdge = true;
                }
                //if it is the infectous person, put them in the center
                addNode(newGridNode, position, toIgnore);
                
                allNodes.Add(newGridNode);
                
            }
        }

        //the building objects are a created as a child of the floor gameobject
        StaticBatchingUtility.Combine(GameObject.Find("Floor"));//since they are static, enable batching for all of them
        //optimises performance in that area
        //alternative is drawmeshinstantiated, but we have far too many buildings for that


        allHumans = new HashSet<Human>();//gotta instantiate the list

        //populate the scene
        for (int i = 0; i < TOTALHUMANS; i++)
        {
            var newHumanObject = Instantiate(humanPrefab);//instantiate object
            var newHuman = newHumanObject.GetComponent<Human>();//make a human
            var spawnLocation = new Vector3(Random.Range(0, TOTALCROSSROADS -1)*crossRoadsGap, 0, Random.Range(0, TOTALCROSSROADS - 1) * crossRoadsGap);//set a random location for them to spawn
            float speed = Random.Range(1, 5) * 0.1f;
            //inconsitencies in speed and hitbox contact lead to humans escaping
            //smaller speed is, less humans escape, i imagine some of them step over the hitbox
            //we subtract a tiny amount from the z direction as humans will walk upwards by default 
            //then the proper pathfinding will commence, not ideal way of doing it
            //but if we start on a crossroads, then no direction is assigned
            //probably due to the starting order of the program


            if (i == 0)//one infectous person, this needs to change
            {
                SpawnHuman(newHuman, speed, new Vector3((TOTALCROSSROADS-2) * crossRoadsGap, 0, (TOTALCROSSROADS-2) * crossRoadsGap), Human.Status.Infectious);//spawn human into the world
                //place infected human at the top, where no other humans have spawned

                allHumans.Add(newHuman);
            }
            else
            {
                SpawnHuman(newHuman, speed, spawnLocation, Human.Status.Healthy);//spawn human into the world
                allHumans.Add(newHuman);
            }

        }
        //CREATE THE HUMANS SECOND, so when they trigger the node area, the nodes are intialised

        graphReference = GameObject.FindGameObjectWithTag("Graph");
        addGraphValues(TOTALHUMANS - 1, 0, 1);//add our number of healthy people - the infected
        //per value increase

        //now to add our buildings

    }

    public int getHumans()
    {
        return TOTALHUMANS;
    }

    public void addGraphValues(int healthy, int exposed, int infectous)
    {//add/subtract value to number of healthy people, exposed people and infectous people
        //remember, you need to get the script component to access the functions
        graphReference.GetComponent<Graph_Window>().healthyPeople += healthy;
        graphReference.GetComponent<Graph_Window>().exposedPeople += exposed;
        graphReference.GetComponent<Graph_Window>().infectousPeople += infectous;
    }

    private void SpawnHuman(Human s, float speed, Vector3 position, Human.Status status)
    {
        s.Supervisor = this;
        s.Speed = speed;//slight variations in speed
        s.transform.position = position;
        s.InfectionStatus = status;
        s.transform.parent = GameObject.Find("Level").transform;
    }

    private void addNode(CrossRoad c, Vector3 position, Vector3 direction)
    {
        c.Supervisor = this;
        c.transform.position = position;
        c.directionToIngore = direction;
        c.transform.parent = GameObject.Find("Level").transform;
    }

    private void addBuilding(GameObject building, Vector3 pos)
    {
        building.transform.position = pos;
        building.transform.parent = GameObject.Find("Floor").transform;
    }
    public int getIncubationTime()
    {
        return incubationTime;
    }

    public int getInfectionRate()
    {
        return infectionRate;
    }
    
    public int getRecoveryTime()
    {
        return recoveryTime;
    }

    public Vector3 getRandomLocation()//unused, kept in here because it may be useful in a future version
    {
        var Location = new Vector3(Random.Range(-WORLDRADIUS, WORLDRADIUS), 0,
            Random.Range(-WORLDRADIUS, WORLDRADIUS));//set a random location on our world
        return Location;
    }

    public DateTime clock()
    {
        DateTime now = DateTime.Now;
        return now;
        //need to find a way of using this
        //might be able to increase performance
    }




    //following methods are called by our sliders and buttons
    public void setHuman(float i)
    {
        TOTALHUMANS = (int) i;
    }
    public void setBlocks(float i)
    {
        TOTALCROSSROADS = (int)i;
    }
    public void setRate(float i)
    {
        infectionRate = (int)i;
    }
    public void setInc(float i)
    {
        incubationTime = (int)i;
    }
    public void setRec(float i)
    {
        recoveryTime = (int)i;
    }

    //reset variables to default
    public void resetToDefault()
    {
        TOTALCROSSROADS = crossRoadsDefault;
        TOTALHUMANS = humanDefault;
        infectionRate = infectionRateDefault;
        incubationTime = incubationTimeDefault;
        recoveryTime = recoveryTimeDefault;
    }
}
