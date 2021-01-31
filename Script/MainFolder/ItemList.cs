using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemList : MonoBehaviour
{
    public Text LocationName;
    public Text Coordinate;
    private HistoricStreet location;
    private MainScript mainScript;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(HistoricStreet historicLocation, MainScript mainSceneScript)
    {
        location = historicLocation;
        mainScript = mainSceneScript;

        LocationName.text = location.LocationName;
        Coordinate.text = $"{location.latitude} , {location.longitude}";

    }

    public void handleClick()
    {
        mainScript.ItemClickHandle(location);
    }
}
