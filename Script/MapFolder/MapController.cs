using System.Collections;
using System.Collections.Generic;
using Google.Maps;
using Google.Maps.Coord;
using Google.Maps.Demos.Zoinkies;
using Google.Maps.Event;
using Google.Maps.Examples.Shared;
using Google.Maps.Feature.Style;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    /// <summary>
    /// Call the GameObjects that are important
    /// </summary>
    public GameObject MapPointer;
    public GameObject MyLocationMaker;
    public GameObject MainCamera;
    public Camera CameraInstance;

    

    /// <summary>
    /// MapsService
    ///
    /// Uses:
    /// 1. To Render or Spawn the Google Map
    /// 2. Helps to convert from Map Coordinate to Vector3
    /// </summary>
    private MapsService mapsService;

    // HistoricStreet variable
    private HistoricStreet historicStreet;

    // latLng variables
    private LatLng PreviousLocation;

    // GameObjectOption variable
    private GameObjectOptions mapObjectOptions;

    // Primitive datatype varibales
    private float usersRadius = 15f;
    
    // List of Vecotr3s
    public static List<Vector3> listVectors;
    private List<Vector3> listofSpawnedPosition;


    // List of Historic Location
    private List<HistoricStreet> listHistoricLocations;
    public static List<HistoricStreet> staticHistoricList;
    private List<HistoricStreet> listSpawnHistoricLocations;
    private List<GameObject> SpawnedMapPointers;
    
    // Map Style Materials
    public Material ExtrudedStructureRoofMaterial;
    public Material ExtrudedStructureWallMaterial;
    public Material ModeledStructureMaterial;
    public Material RegionMaterial;
    public Material SegmentMaterial;
    public Material SegmentBorderMaterial;
    public Material AreaWaterMaterial;
    public Material LineWaterMaterial;


    
    /// <summary>
    ///     Distance inside which buildings will be completely squashed (<see cref="MaximumSquash" />)
    /// </summary>
    public float SquashNear = 20;

    /// <summary>
    ///     Distance outside which buildings will not be squashed.
    /// </summary>
    public float SquashFar = 100;

    /// <summary>
    ///     The vertical scaling factor applied at maximum squashing.
    /// </summary>
    public float MaximumSquash = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        requestLocationPermission();
        InitStyleOption();
        
        
        staticHistoricList = new List<HistoricStreet>();
        listSpawnHistoricLocations = new List<HistoricStreet>();
        
        
        listVectors = new List<Vector3>();
        listofSpawnedPosition = new List<Vector3>();
        SpawnedMapPointers = new List<GameObject>();
        
        StartCoroutine(Follow());
        
       
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mapsService == null)
        {
            return;
        }
        
        ObjectTrackUserLocation();
        ProcessTouch();
    }
    
    
   
    
    

    private IEnumerator Follow()
    {
        // If location is allowed by the user, start the location service and compass, otherwise abort
        // the coroutine.


        #if PLATFORM_IOS
                // The location permissions request in IOS does not seem to get invoked until it is called for
                // in the code. It happens at runtime so if the code is not trying to access the location
                // right away, it will not pop up the permissions dialog.
                Input.location.Start();
        #endif

        while (!Input.location.isEnabledByUser)
        {
            //Debug.Log("Waiting for location services to become enabled..");
            yield return new WaitForSeconds(1f);
        }

        _ShowAndroidToastMessage("Location services is enabled.");

        /*Start the Location service*/
        #if !PLATFORM_IOS

        Input.location.Start();

        #endif

        Input.compass.enabled = true;

        // Wait for the location service to start.
        while (true)
        {
            if (Input.location.status == LocationServiceStatus.Initializing)
            {

                // Starting, just wait.
                yield return new WaitForSeconds(1f);

            }
            else if (Input.location.status == LocationServiceStatus.Failed)
            {

                // Failed, abort the coroutine.
                //Debug.LogError("Location Services failed to start.");
                _ShowAndroidToastMessage("Location Service failed to start.");

                yield break;

            }
            else if (Input.location.status == LocationServiceStatus.Running)
            {

                // Started, continue the coroutine.
                break;
            }
        }

        // Get the MapsService component and load it at the device location.
        PreviousLocation =
            new LatLng(Input.location.lastData.latitude, Input.location.lastData.longitude);

        mapsService = GetComponent<MapsService>();
        
         /*Get the static HistoricStreet from MainScript class
           . Check whether it is null
           . if it is null set the MapsService InitFloatingOrigin to the Users Location
           . if it is not null set the MapService InitFloatingOrgin to the set location
          */

         historicStreet = MainScript.selectedLocation;

         if (historicStreet == null)
         {
             mapsService.InitFloatingOrigin(PreviousLocation);
             
         }
         else
         {
           LatLng selectedLocation = new LatLng(historicStreet.Latitude, historicStreet.Longitude);
           mapsService.InitFloatingOrigin(selectedLocation);
           MainCamera.GetComponent<ObjectFollower>().ShouldFollow(false);
         }

         
        //_ShowAndroidToastMessage("MapsService initialised and InitFloatingOrigin called");
        
        // Register a listener to be notified when the map is loaded.
        mapsService.Events.MapEvents.Loaded.AddListener(OnLoaded);

        // Load map with default options.
        LoadMap();
        
      
    }
     
     /// <summary>
     /// Example of OnLoaded event listener.
     /// </summary>
     /// <remarks>
     /// The communication between the game and the MapsSDK is done through APIs and event listeners.
     /// </remarks>
     public void OnLoaded(MapLoadedArgs args)
     {
         // The Map is loaded - you can start/resume gameplay from that point.
         // The new geometry is added under the GameObject that has MapsService as a component.
         
        //_ShowAndroidToastMessage("Map loaded about to spawn cube");
        StartCoroutine(GetLocation());
        
        // Apply a post-creation listener that adds the squashing MonoBehaviour
        // to each building.
        mapsService.Events.ExtrudedStructureEvents.DidCreate.AddListener(
            e => { AddSquasher(e.GameObject); });

        // Apply a post-creation listener that adds the squashing MonoBehaviour
        // to each building.
        mapsService.Events.ModeledStructureEvents.DidCreate.AddListener(
            e => { AddSquasher(e.GameObject); });

     }

     private LatLng currentLocation;
     void ObjectTrackUserLocation()
     { 
         currentLocation = new LatLng(Input.location.lastData.latitude, Input.location.lastData.longitude);
         Vector3 currentposition = mapsService.Coords.FromLatLngToVector3(currentLocation);
         Vector3 targetPosition = new Vector3(currentposition.x,MyLocationMaker.transform.position.y,currentposition.z);

         MyLocationMaker.transform.position =
             Vector3.Lerp(MyLocationMaker.transform.position, targetPosition, Time.deltaTime * 5);
        
        
         // Only move the map location if the device has moved more than 2 meters.
         if (Vector3.Distance(Vector3.zero, currentposition) > 500f)
         {

             mapsService.MoveFloatingOrigin(currentLocation, new[] {MyLocationMaker.gameObject});
             LoadMap();
             PreviousLocation = currentLocation;
         }


     }
    
    private IEnumerator GetLocation()
    {

        listHistoricLocations = MainScript.ListHistoricLocations;
        
        
        yield return new WaitForSeconds(1f);

        // This to prevent the MapPointer from spawning excess
        // it checks the list to see if there are any Gameobject and then it destroys it and clears the list;
        
        if (SpawnedMapPointers.Count > 0)
        {
            foreach (var pointer in SpawnedMapPointers)
            {
                Destroy(pointer);
                
            }
            
            SpawnedMapPointers.Clear();
        }

        SpawnHistoricCubes();
        
        /*LoadHistoricLocation load = new LoadHistoricLocation();
         
        yield return new WaitUntil(() => load != null);
        listHistoricLocations = load.GetHistoricLocations();
        SpawnHistoricCubes();*/

        //toastClass._ShowAndroidToastMessage("Cube spawned");
    }

    

    void SpawnHistoricCubes()
    {
        foreach (var historic in listHistoricLocations)
        {
            Vector3 worldposition = mapsService.Coords.FromLatLngToVector3(new LatLng(historic.latitude,historic.longitude));

            GameObject pointer = Instantiate(MapPointer, worldposition, Quaternion.identity);

            listofSpawnedPosition.Add(worldposition);
            
            SpawnedMapPointers.Add(pointer);
        }
    }

    
   
    
    

    
    
    void SubtractHistoricVectors()
    {
        Vector3 cylinderPostion = MyLocationMaker.transform.position;

        for (int i = 0; i < listofSpawnedPosition.Count; i++)
        {
            Vector3 position = listofSpawnedPosition[i];
             
            // check if the cubes are within the user radius of 200f
            float distance = Vector3.Distance(cylinderPostion, position);
             
            if (distance <= usersRadius)
            {
                //find the relative vector of the cube from the user mark or location
                Vector3 difference = position - cylinderPostion;
                 
                listVectors.Add(difference);
                staticHistoricList.Add(listHistoricLocations[i]);
                 
            }

        }

    }

    public void OnWillCreateExtrudedStructure(WillCreateExtrudedStructureArgs args) {
        if (!enabled) {
            return;
        }

        
        ExtrudedStructureStyle.Builder builder = args.Style.AsBuilder();
        builder.ApplyFixedHeight = false;
        //builder.FixedHeight = 0;
         

        args.Style = builder.Build();
         
         
    }

    public void OnDidCreateExtrudedStructure(DidCreateExtrudedStructureArgs args)
    {
        AddSquasher(args.GameObject);
        
    }

    void LoadMap()
     {
         mapsService.MakeMapLoadRegion().
             AddViewport(Camera.main, 10000f).
             Load(mapObjectOptions);
         
         
         
         //mapsService.LoadMap(ExampleDefaults.DefaultBounds,ExampleDefaults.DefaultGameObjectOptions);
         //_ShowAndroidToastMessage("LoadMap called and executed");
         //mapsService.LoadMap(ExampleDefaults.DefaultBounds,ExampleDefaults.DefaultGameObjectOptions);
         //mapsService.LoadMap(mapsService.GetPreviewBounds(), mapsService.MaybeGetGameObjectOptions());
     }

    
     // Setup the Map Style using the GameObjectOption -- (Design the Map)
     void InitStyleOption()
     {
         mapObjectOptions = ExampleDefaults.DefaultGameObjectOptions;
         
         mapObjectOptions.ExtrudedStructureStyle = new ExtrudedStructureStyle.Builder
         {
             
             RoofMaterial = ExtrudedStructureRoofMaterial,
             WallMaterial = ExtrudedStructureWallMaterial

         }.Build();
         
         mapObjectOptions.ModeledStructureStyle = new ModeledStructureStyle.Builder
         {
             Material = ModeledStructureMaterial
         }.Build();
         
         mapObjectOptions.RegionStyle = new RegionStyle.Builder
         {
             FillMaterial = RegionMaterial
         }.Build();
         
         mapObjectOptions.SegmentStyle = new SegmentStyle.Builder
         {
             Material = SegmentMaterial
             //BorderMaterial = SegmentBorderMaterial
         }.Build();
         
         mapObjectOptions.AreaWaterStyle = new AreaWaterStyle.Builder
         {
             FillMaterial = AreaWaterMaterial
         }.Build();
         
         mapObjectOptions.LineWaterStyle = new LineWaterStyle.Builder
         {
             Material = LineWaterMaterial,
             BorderMaterial = LineWaterMaterial
         }.Build();
         
         
         
         
     }
     
     /// <summary>
     /// Button Event Handler Function
     /// </summary>
     /// <param name="message"></param>
     
     public void ARClick()
     {
         //SubtractVectors();
         
         
         SubtractHistoricVectors();
         SceneManager.LoadScene(ConstantString.ARSCENE);
     }

     public void BackButton()
     {
         SceneManager.LoadScene(ConstantString.MAINSCENE);
     }

     public void MyLocation()
     {
         MainCamera.GetComponent<ObjectFollower>().ShouldFollow(true);
         mapsService.MoveFloatingOrigin(currentLocation, new[] {MyLocationMaker.gameObject});
         LoadMap();
         
     }
     
     void ProcessTouch()
     {
         if ((Input.touchCount > 0 ) && (Input.GetTouch(0).phase == TouchPhase.Began))
         {
             Ray raycast = CameraInstance.ScreenPointToRay(Input.GetTouch(0).position); 
             //Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            
             RaycastHit raycastHit;

             if (Physics.Raycast(raycast, out raycastHit))
             {
                 // something hit
                 
                 
                 
                 if (raycastHit.collider.CompareTag("CameraNavBG"))
                 {
                     
                     
                     MainCamera.GetComponent<ObjectFollower>().ShouldFollow(false);
                     //raycastHit.collider.GetComponent<SpawnCubeScript>().PlayOut();
                 }
             }
         }
     }

     public void JoyStickControllerClicked()
     {
         MainCamera.GetComponent<ObjectFollower>().ShouldFollow(false);
        _ShowAndroidToastMessage("Navigation Controller Activated");

        
     }

     // Android Toast or Dialog function
     public static void _ShowAndroidToastMessage(string message)
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
     
     void requestLocationPermission()
     {
         if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
         {
             Permission.RequestUserPermission(Permission.Camera);
         }
     }
     
     private void AddSquasher(GameObject target)
     {
         Squasher squasher = target.AddComponent<Squasher>();
         squasher.Target = MapPointer.transform;
         squasher.Near = SquashNear;
         squasher.Far = SquashFar;
         squasher.MaximumSquashing = MaximumSquash;
         
     }

}
