using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _ShowAndroidToastMessage("THis is the Test Script");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonClick()
    {
        _ShowAndroidToastMessage("test Button is clicked");
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
