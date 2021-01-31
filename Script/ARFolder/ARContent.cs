using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using YoutubePlayer;

public class ARContent : MonoBehaviour
{
    public GameObject MapPointer;
    public GameObject Playbutton;
    public GameObject TextButton;
    public GameObject CurveTV;
    public GameObject TextBoard;
    public TextMeshPro textMeshPro;

    private VideoPlayer videoPlayer;

    private bool MapPointerToggle;
    private bool TextButtonToggle;

    private HistoricStreet street;

    private string videourl;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        
        Playbutton.SetActive(false);
        TextButton.SetActive(false);
        CurveTV.SetActive(false);
        TextBoard.SetActive(false);

        MapPointerToggle = false;
        TextButtonToggle = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpContent(HistoricStreet historicStreet)
    {
        street = historicStreet;

        if (!String.IsNullOrEmpty(street.VideoUrl))
        {
            videourl = street.VideoUrl;
            
            
        }
       
        if (!String.IsNullOrEmpty(street.Textcontent))
        {
            textMeshPro.text = historicStreet.Textcontent;
        }

        
        
    }

    public void MapPointerClicked()
    {
        if (!MapPointerToggle)
        {
            //TODO: Make the Play and Text button visible
            
            Playbutton.SetActive(true);
            TextButton.SetActive(true);
            
            
            MapPointerToggle = true;
        }
        else
        {
          //TODO: Make the buttons invisible
          
          Playbutton.SetActive(false);
          TextButton.SetActive(false);
          
          MapPointerToggle = false;
        }
        
        _ShowAndroidToastMessage("MapPointerClicked is called");
    }

    public void ClearContent()
    {
        Playbutton.SetActive(false);
        TextButton.SetActive(false);

        if (!String.IsNullOrEmpty(street.VideoUrl))
        {

            if (videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
                

            }
            CurveTV.SetActive(false);
           
        }
         
        TextBoard.SetActive(false);
        
    }

    public void PlayButtonClicked()
    {

        if (String.IsNullOrEmpty(street.VideoUrl))
        {
            _ShowAndroidToastMessage("There is not Video for this Historic Street");
        }
        else
        {
            if (videourl.EndsWith(".mp4"))
            {
                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = videourl;
                
                if (!videoPlayer.isPlaying)
                {
                    CurveTV.SetActive(true);
                    videoPlayer.Play();
          
                }
                else
                {
                    videoPlayer.Stop();
                    CurveTV.SetActive(false);
          
                }
            }
            else
            {
                videoPlayer.source = VideoSource.Url;
                videoPlayer.PlayYoutubeVideoAsync(videourl);
            }



           
            
            
        }

        
    }
    
    

    public void TextButtonClicked()
    {
        if (String.IsNullOrEmpty(street.Textcontent))
        {
            _ShowAndroidToastMessage("There is not Info for this Historic Street");
            return;
        }
        
        if (!TextButtonToggle)
        {
            // TODO: Make the TextBoard visible
            TextBoard.SetActive(true);
            TextButtonToggle = true;
        }
        else
        {
            //TODO: Make the TextButton Invisible
            TextBoard.SetActive(false);
            TextButtonToggle = false;
        }
    }
    
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>(
                    "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }


}
