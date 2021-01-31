using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ARContentPopUpScript : MonoBehaviour
{
    private ARContent arContent;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(ARContent ARContent)
    {
        arContent = ARContent;
    }

    public void InfoButton()
    {
        arContent.TextButtonClicked();
        Destroy(gameObject);
    }

    public void VideoButton()
    {
        arContent.PlayButtonClicked();
        Destroy(gameObject);
    }

    public void ClearButton()
    {
        arContent.ClearContent();
        Destroy(gameObject);
    }
}
