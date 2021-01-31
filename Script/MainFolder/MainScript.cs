using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;

public class MainScript : MonoBehaviour
{
    public static HistoricStreet selectedLocation;
    public GameObject Listitem;
    public GameObject ContentContainer;

    private List<HistoricStreet> listOfHistoricLocatios;
    private bool loadlock = true;
    
   
    public static List<HistoricStreet> ListHistoricLocations;

    private DatabaseReference dataReference;
    
    // Start is called before the first frame update
    void Start()
    {
        // Request Location Permission
        requestLocationPermission();
        
        ListHistoricLocations = new List<HistoricStreet>();
        LoadFromFirebase();
    }

    // Update is called once per frame
    void Update()
    {
        //LoadList();
    }



    void LoadList()
    {

        listOfHistoricLocatios = LoadHistoricStreets.ListHistoricLocations;

        if (listOfHistoricLocatios != null && loadlock)
        {
            foreach (var location in listOfHistoricLocatios)
            {
                GameObject historicList = Instantiate(Listitem);
                historicList.transform.SetParent(ContentContainer.transform, false);
                
                historicList.GetComponent<ItemList>().SetUp(location,this);
                
                
            }

            loadlock = false;

        }

    }
    
    void LoadFromFirebase()
    {
        
        // Set up the Editor before calling into the realtime database
        
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(ConstantString.DATABASE_URL);
        
        dataReference = FirebaseDatabase.DefaultInstance.RootReference.Child(ConstantString.HISTORIC_LOCATION);


        StartCoroutine(LoadLocations());


    }
    
    IEnumerator LoadLocations()
    {
        var data = LoadDataSnap(dataReference);
        
        yield return  new WaitUntil(() => data.IsCompleted);

        DataSnapshot dataSnapshot = data.Result;

        foreach (var datasnap in dataSnapshot.Children)
        {
            HistoricStreet historicLocation = JsonUtility.FromJson<HistoricStreet>(datasnap.GetRawJsonValue());
            AddHistoricLoation(historicLocation);
        }
        
        _ShowAndroidToastMessage("About to SpawnListItems");
        
        SpawnListItems();
    }

    
    private async Task<DataSnapshot> LoadDataSnap(DatabaseReference reference)
    {
        var datasnap = await reference.GetValueAsync();

        if (!datasnap.Exists)
        {
            return null;
        }

        return datasnap;
    }

    
    void AddHistoricLoation(HistoricStreet historicLocation)
    {
        ListHistoricLocations.Add(historicLocation);
        
    }

    void SpawnListItems()
    {
        ListHistoricLocations.Reverse();
        foreach (var location in ListHistoricLocations)
        {
            GameObject historicList = Instantiate(Listitem);
            historicList.transform.SetParent(ContentContainer.transform, false);
                
            historicList.GetComponent<ItemList>().SetUp(location,this);
                
                
        }
        
        _ShowAndroidToastMessage("It has Spawned List Items");
    }


    void requestLocationPermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
    }

    public void ItemClickHandle(HistoricStreet clickedLocation)
    {
        selectedLocation = clickedLocation;
        SceneManager.LoadScene(ConstantString.MAPSCENE);
    }

    public void AddLocationButton()
    {
        SceneManager.LoadScene(ConstantString.ADDLOCATIONSCENE);
    }

    public void MyLocationGoogleMapButton()
    {
        selectedLocation = null;
        SceneManager.LoadScene(ConstantString.MAPSCENE);
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
