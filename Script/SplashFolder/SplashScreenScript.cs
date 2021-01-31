using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using UnityEngine.Android;
using Firebase.Unity.Editor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreenScript : MonoBehaviour
{
    public Text dots;
    private FirebaseApp auth;
    private FirebaseUser currentUser;
    private DatabaseReference dataReference;
    
    List<HistoricStreet> historicStreetList;

    // Start is called before the first frame update
    void Start()
    {
        
        _ShowAndroidToastMessage($"Setup Firebase {ConstantString.DATABASE_URL} , {ConstantString.HISTORIC_LOCATION}");

        historicStreetList = new List<HistoricStreet>();
        
        /*FirebaseApp.CheckDependenciesAsync().ContinueWith(task =>
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        });*/
        
        
        
        // Set up the Editor before calling into the realtime database
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(ConstantString.DATABASE_URL);
        
        dataReference = FirebaseDatabase.DefaultInstance.RootReference.Child(ConstantString.HISTORIC_LOCATION);
        
       _ShowAndroidToastMessage($"After Database Reference");

        StartCoroutine(AnonyLogin());
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(DotTextAnimate());
        
       
    }

    IEnumerator DotTextAnimate()
    {
        dots.text = ".";
        yield return new WaitForSeconds(1f);
        dots.text = "..";
        yield return new WaitForSeconds(1f);
        dots.text = "...";
        yield return new WaitForSeconds(1f);
        dots.text = "....";
        yield return new WaitForSeconds(1f);
        dots.text = ".....";
        yield return new WaitForSeconds(1f);
        dots.text = "......";
        yield return new WaitForSeconds(1f);
        dots.text = ".......";
        

    }

    IEnumerator WaitLoad()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(ConstantString.MAINSCENE);
    }


    IEnumerator AnonyLogin()
    {
        var auth = FirebaseAuth.DefaultInstance;
        _ShowAndroidToastMessage("AnonyLogin called");
        var registerTask = auth.SignInAnonymouslyAsync();
        
        yield return new WaitUntil(() => registerTask.IsCompleted);
        
        
        _ShowAndroidToastMessage("AnonyLogin completed");
        
        if (registerTask.Exception != null)
        {
            _ShowAndroidToastMessage($"Failed to Login user with {registerTask.Exception}");
        }
        else
        {
            _ShowAndroidToastMessage($"Successfully logged in User {registerTask.Result.UserId}");
            StartCoroutine(LoadLocationIntoFirebase());

        }
    }
    
    IEnumerator LoadLocationIntoFirebase()
    {
        
        _ShowAndroidToastMessage("LoadLocation called");
        
        var snapshot = LoadData(dataReference, 0, 0);

        yield return new WaitUntil(() => snapshot.IsCompleted);

        DataSnapshot datasnap = snapshot.Result;
        
        if (!datasnap.Exists || !datasnap.HasChildren)
        {
            _ShowAndroidToastMessage("DataSnap does not exist");
            
            AddHistoricLocations("Samuel E",6.494617835034915, 3.316794096653674,"Samuel Ekonola street\n\n is quite",0);
        AddHistoricLocations("Ekonola",6.4945798588446895, 3.31779187839149,"Take it\n\n or leave it",1);
        AddHistoricLocations("Sam Eku", 6.494218751743211, 3.3176832489289394, "Sam is\n\n Eku now",0);
        AddHistoricLocations("SSSEEE",6.494859350232242, 3.3163776837130476,"Take out \n\nof street",1);
        AddHistoricLocations("Grandmate",6.49435566600017, 3.3168685279463794,"Take the grand \n\nout of the mate",0);
        AddHistoricLocations("Samuel",6.494307696047648, 3.3168115310105244,"Samuel\n\n is good",1);
        AddHistoricLocations("Kunola",6.494341008515165, 3.316885291751043,"Take about\n\n class",1);
        AddHistoricLocations("Ekun Ekun",6.4943616622439215, 3.31686651628982,"Take twice\n\n and more",1);
        AddHistoricLocations("Street Sam",6.494362328493222, 3.3168504230373435,"Last part\n\n of the street",1);
        AddHistoricLocations("Sam Sam",6.494372988481906, 3.3167853794752493,"Legacy for \n\n all who seeks it",0);
        AddHistoricLocations("Street street",6.4943276835284145, 3.316765262909653,"Today\n\n we experience History\n\nhere",1);
        AddHistoricLocations("Today Street",6.493988895621053, 3.3173486433215458,"Free the Homer \n\nand goner\n\nand all who is here",1);
        AddHistoricLocations("Big Sam",6.493988895621053, 3.3173486433215458,"Big Sam is here and truly here.\n\nSo get ready",0);
        AddHistoricLocations("Street War",6.494374987229755, 3.3168450586198506,"Forget me and them\n\nLet focus on you",1);
        AddHistoricLocations("Wall Street ",6.494575195098559, 3.3166351757950707,"We live here and\n\nand there is nothing else to say",0);
        AddHistoricLocations("Sam Naija",6.494457602134095, 3.316756545731228,"The last one and\n\nthe very last one",1);
        AddHistoricLocations("Outdoor Ekuno",6.494540883273943, 3.3166794322297757,"Ekuno and the\n\n beauty",1); 
        AddHistoricLocations("Ekunola street",6.494697451783252, 3.3165272168633444,"Ekunola street\n\n right now",0); 
        AddHistoricLocations("Sam Outdoor",6.49477540289341, 3.316451444466265,"Outdoor street\n\n for all",1);

        StartCoroutine(AddLocationToBase());
        }

        StartCoroutine(WaitLoad());


    }
    
    void AddHistoricLocations(string name, double lat, double lng, string textcontent, int videoindex)
    {

        //StartCoroutine(AddLocationToBase(name, lat, lng, textcontent));
        
        historicStreetList.Add(new HistoricStreet(name,lat,lng,textcontent));

    }

    IEnumerator AddLocationToBase()
    {

        foreach (var street in historicStreetList)
        {
            DatabaseReference locationRef = dataReference.Push();
        
            string key = locationRef.Key;
        
            HistoricStreet historicLocation = new HistoricStreet(street.LocationName,street.Latitude,street.Longitude,ConstantString.VIDEO_URL,street.Textcontent,key);
        
            string json = JsonUtility.ToJson(historicLocation);
        
            var taskstore = locationRef.SetRawJsonValueAsync(json);
        
            yield return new WaitUntil(() => taskstore.IsCompleted);

            if (taskstore.Exception != null) 
            { 
                _ShowAndroidToastMessage($"Something wrong happened {taskstore.Exception.Message}");
            }
            else 
            { 
                _ShowAndroidToastMessage("Successfully uploaded Historic Location");
                
            }




            
        }

        

    }
    
    public async Task<DataSnapshot> LoadData(DatabaseReference reference, int start , int end)
    {
        var dataSnapshot = await reference.GetValueAsync();
        
        /*if (!dataSnapshot.Exists)
        {
            return null;
        }*/

        return dataSnapshot;

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
