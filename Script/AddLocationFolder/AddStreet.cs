using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AddStreet : MonoBehaviour
{

    public InputField Name;
    public InputField Lat;
    public InputField Lng;
    public InputField VideoUrl;
    public InputField Info;

    private DatabaseReference dataReference;

    // Start is called before the first frame update
    void Start()
    {
        
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(ConstantString.DATABASE_URL);
        dataReference = FirebaseDatabase.DefaultInstance.RootReference.Child(ConstantString.HISTORIC_LOCATION);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator AddToFirebase(string name, double lat, double lng, string videourl, string textcontent)
    {
        DatabaseReference pushReference = dataReference.Push();

        string key = pushReference.Key;
        
        HistoricStreet historicStreet = new HistoricStreet(name, lat, lng, videourl, textcontent, key);
        
        string json = JsonUtility.ToJson(historicStreet);

        var pushtask = pushReference.SetRawJsonValueAsync(json);
        
        yield return new WaitUntil(() => pushtask.IsCompleted);

        if (pushtask.Exception != null)
        {
            CodelabUtils._ShowAndroidToastMessage("Something went wrong while uploading data");
        }
        else
        {
            CodelabUtils._ShowAndroidToastMessage("Data Successfully uploaded");
            SceneManager.LoadScene(ConstantString.MAINSCENE, LoadSceneMode.Additive);
        }

    }

    public void BackButton()
    {
        SceneManager.LoadScene(ConstantString.MAINSCENE);
    }

    public void SaveButton()
    {
        if (String.IsNullOrEmpty(Name.text) || String.IsNullOrEmpty(Lat.text) || String.IsNullOrEmpty(Lng.text) || String.IsNullOrEmpty(Info.text))
        {
            CodelabUtils._ShowAndroidToastMessage("Please fill-in the missing details");
        }
        else
        {
            StartCoroutine(AddToFirebase(Name.text, Double.Parse(Lat.text),Double.Parse(Lng.text), VideoUrl.text, Info.text ));
        }
    }
}
