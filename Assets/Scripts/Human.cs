using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.Apple;
using UnityEngine.Serialization;
using static UnityEngine.GraphicsBuffer;


public class Human : MonoBehaviour
{
    // Start is called before the first frame update
    public float Speed = 1.0f;//our speed, set default value in case supervisor fails to allocate it
    public Material HealthyMaterial;//material to convey that we are healthy
    public Material InfectiousMaterial;//material to convey that we are infectous
    public Material ExposedMaterial;//material to convey that we are exposed
    public Material ImmuneMaterial;
    public Supervisor Supervisor;//a reference to our supervisor
    public Vector3 direction;
    private static int incubationTime;
    private static int recoveryTime;
    Status m_InfectionStatus;//the humans current status
    private float incubationStart = 0;
    private float infectionStart = 0;


    //status code adapted from the store pandemic simulator
    //found at https://blog.unity.com/industry/exploring-new-ways-to-simulate-the-coronavirus-spread
    //by James Fort, Adam Crespi, Chris Elion, Rambod Kermanizadeh, Priyesh Wani and Danny Lange
    public enum Status//need others to check the status, mainly supervisor
    {
        Healthy,
        Infectious,
        Exposed,
        Immune
    }

    public Status InfectionStatus
    {
        get => m_InfectionStatus;
        set { SetStatus(value); }
    }

    public Supervisor theSupervisor
    {
        set { Supervisor = value; }
    }

    void SetStatus(Status s)
    {
        Material m = null;
        switch (s)
        {
            //update how we look depending on whether we are infectous, exposed or healthy
            case Status.Healthy://mental note: if it breaks, remember to link these in the unity inspector
                m = HealthyMaterial;
                break;
            case Status.Infectious:
                m = InfectiousMaterial;
                break;
            case Status.Exposed:
                m = ExposedMaterial;
                break;
            case Status.Immune:
                m = ImmuneMaterial;
                break;
        }
        m_InfectionStatus = s;

        var renderer = this.GetComponentInChildren<Renderer>();//incredibly annoying, but
        //capsule is not seen as part of human, but rather its child
        //two hours down the drain
        renderer.material = m;


    }


    public bool IsHealthy()
    {
        return m_InfectionStatus == Status.Healthy;
    }

    public bool IsInfectious()
    {
        return m_InfectionStatus == Status.Infectious;
    }

    public bool IsExposed()
    {
        return m_InfectionStatus == Status.Exposed;
    }

    public bool IsImmune()
    {
        return m_InfectionStatus == Status.Immune;
    }
    //end of the adapted code
    

    void Start()
    {
        direction = new Vector3 (0, 0, 1);//give us a default speed, will move up slightly and hit a crossroads
        incubationTime = Supervisor.getIncubationTime();
        recoveryTime = Supervisor.getRecoveryTime();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Speed * direction;
        //here we run into a problem:
        //movement is based on units/frame, not units/second
        //therefore, if framerate changes, so will the human's speed
        //also prevents us from properly implementing a pause system
        //using a units/second approach leads to some humans "stepping over" crossroads
        //(one big step/second vs 1 tiny step per frame)
        //hard to remedy
        //could limit to less than 1/unit per second speed to ensure contact but that leads to v slow humans
        //which in turn breaks the illusion of the infection model
        //another disavantage to the crossroads approach

        // Check if the position of the cube and sphere are approximately equal.
        if (IsExposed())
        {
            if (incubationStart == 0)
            {
                incubationStart = Time.time + incubationTime;
            }
            //would be better to have this tied to a clock, in supervisor
            //still though, that will not fix performance issues
            if (Time.time >= incubationStart)
            {
                SetStatus(Status.Infectious);
                Supervisor.addGraphValues(0, -1, 1);
            }
        }
        else if(IsInfectious() && recoveryTime != 0)//if recovery time is enabled, human will immunise after a little bit
        {
            if (infectionStart == 0)
            {
                infectionStart = Time.time + recoveryTime;
            }
            //would be better to have this tied to a clock, in supervisor
            //still though, that will not fix performance issues
            if (Time.time >= infectionStart)
            {
                SetStatus(Status.Immune);
                Supervisor.addGraphValues(1, 0, -1);
            }
        }
        
    }


    public Vector3 getPosition()//returns the current position of the shopper
    {
        return transform.position; 
    }
}