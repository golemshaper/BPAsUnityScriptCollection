using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /*
     * Add any sort of "gloabal" system object references here
     */
    public static GameManager instance;
    public GameObject Player;
    private System.Random rand = new System.Random();
    public System.Random RandomMain()
    {
        return rand;
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
