using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Graph_Window : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    public Material HealthyMaterial;//material to convey that we are healthy
    public int healthyPeople, exposedPeople, infectousPeople = 0;
    public float yAdd;
    private int timeCount = 0;
    public int frameInterval;
    //private int debugInt = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(frameInterval == 0)
        {
            frameInterval = 5;//supervisor should set the value to something reasonable
        }//but if not, we set it to a default
        graphContainer = GetComponent<RectTransform>();
        yAdd = 160.0f / this.GetComponentInParent<Supervisor>().getHumans();
    }

    // Update is called once per frame
    void Update()
    {
        drawGraph(frameInterval);
    }

    private void drawGraph(int frameInterval)
    {

        // we update the graph 1/frameInterval frames
        if (timeCount % frameInterval == 0)
        {
            float xPos = timeCount / frameInterval;
            CreateCircle(new Vector2(xPos, exposedPeople * yAdd), //our exposed people
            new Color32(255, 255, 0, 100));//our max height is 160, 160/400 = 0.4
            //draw this first, since we want infectous people to be drawn at the front

            CreateCircle(new Vector2(xPos, infectousPeople * yAdd), //our infectous people
            new Color32(255, 0, 0, 100));//our max height is 160, 160/400 = 0.4

            CreateCircle(new Vector2(xPos, healthyPeople * yAdd), //our healthy people
            new Color32(0, 0, 0, 100));//our max height is 160, 160/400 = 0.4
        }

        timeCount += 1;
        if (timeCount > 160 * frameInterval)
        {
            timeCount = 0;
            DestroyAll("circle");
        }
    }

    private void DestroyAll(string tag)
    {
        GameObject[] circles = GameObject.FindGameObjectsWithTag(tag);
        foreach(GameObject circle in circles)
            Destroy(circle);
    }
    private void CreateCircle (Vector2 position, Color32 color)
    {
        GameObject circle = new GameObject("circle", typeof(Image));
        circle.tag = "circle";
        circle.transform.SetParent(graphContainer, false);
        circle.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = circle.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = new Vector2(5, 5);//size of circle
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        circle.GetComponent<Image>().color = color;//make a new color
    }
}
