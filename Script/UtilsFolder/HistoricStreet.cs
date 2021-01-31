using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoricStreet 
{
    public string locationName;
    public double latitude;
    public double longitude;
    public string videoUrl;
    public string textcontent;
    public string key;
 
 

    public HistoricStreet(string name, double lat, double lng, string textcon)
    {

        locationName = name;
        latitude = lat;
        longitude = lng;
        textcontent = textcon;

    }
 
    public HistoricStreet(string name, double lat, double lng, string videolink, string textcon)
    {
        locationName = name;
        latitude = lat;
        longitude = lng;
        videoUrl = videolink;
        textcontent = textcon;
    }

    public HistoricStreet(string name, double lat, double lng, string videolink, string textcon , string pushkey )
    {
        locationName = name;
        latitude = lat;
        longitude = lng;
        videoUrl = videolink;
        textcontent = textcon;
        key = pushkey;

    }



    public string LocationName
    {
        get => locationName;
        set => locationName = value;
    }

    public double Latitude
    {
        get => latitude;
        set => latitude = value;
    }

    public double Longitude
    {
        get => longitude;
        set => longitude = value;
    }

    public string VideoUrl
    {
        get => videoUrl;
        set => videoUrl = value;
    }

    public string Textcontent
    {
        get => textcontent;
        set => textcontent = value;
    }

    public string Key
    {
        get => key;
        set => key = value;
    }

}
