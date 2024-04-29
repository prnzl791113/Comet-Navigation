using System;
using System.Xml.Serialization;
using Unity.XR.CoreUtils;
using Niantic.Experimental.Lightship.AR.WorldPositioning;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.UI;

public class MapMover : MonoBehaviour
{
    [SerializeField] public ARCameraManager _arCameraManager;

    [SerializeField] public XROrigin _xrOrigin;
    [SerializeField] public UnityEngine.UI.Image _compassImage;
    [SerializeField] private int currLevel=1;
    [SerializeField] private TMPro.TMP_InputField inpfield;
    [SerializeField] private TMPro.TextMeshProUGUI coordinatesText;

    private ARWorldPositioningCameraHelper _cameraHelper;
    [SerializeField] private Button sub;

    private ARWorldPositioningObjectHelper _objectPosHelper;
    private List<Datum> possiblePlaces = new List<Datum>();
    [SerializeField] public GameObject cube;

     public GameObject demoObj;
     

    private double currlat = 32.986313;
    private double currlon = -96.748009;
    private double destlat;
    private double destlon;
    private int destLevel;
    private string json;


    private List<Vector2> coordinates;
    List<GameObject> markers2 = new List<GameObject>();

    public void Start()
    {

        Color newColor = _compassImage.color;
        newColor.a = 0;
        _compassImage.color = newColor;
        coordinates = new List<Vector2>();
        sub.onClick.AddListener(doerListner);
        _cameraHelper = _arCameraManager.GetComponent<ARWorldPositioningCameraHelper>();
        _objectPosHelper = _xrOrigin.GetComponent<ARWorldPositioningObjectHelper>();

             demoObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Quaternion rot = Quaternion.LookRotation(Vector3.up, Vector3.up);
            demoObj.GetComponent<Renderer>().material.color = Color.blue;
            demoObj.transform.localScale = new Vector3(2, 2, 2);
            Debug.Log("preparing to add object");
            _objectPosHelper.AddOrUpdateObject(gameObject: demoObj, latitude: 32.98961f, longitude: -96.7489f, altitude: 1, rotationXYZToEUN: rot);
            Debug.Log("added object");
    }
    void doerListner()
    {
        StartCoroutine(doer());
    }

    IEnumerator doer()
    {
        yield return StartCoroutine(autocomplete());
        yield return StartCoroutine(GetCoordinates(OnCoordinatesReceived));
    }

    private void Update()
    {

        // Quaternion rot = Quaternion.LookRotation(Vector3.up, Vector3.up);   
        // _objectPosHelper.AddOrUpdateObject(gameObject: cube, latitude: 32.98454, longitude: -96.75154, altitude: _cameraHelper.Altitude, rotationXYZToEUN: rot);
        //Debug.Log(_cameraHelper.Altitude);
        //float heading = _cameraHelper.TrueHeading;
        //_compassImage.rectTransform.rotation = Quaternion.Euler(0, 0, heading);

        coordinatesText.text = "Latitude: " + _cameraHelper.Latitude + "\nLongitude: " + _cameraHelper.Longitude;

        //NEED TO ENABLE THINGS WHEN DEMOING
        currlat = _cameraHelper.Latitude;
        currlon = _cameraHelper.Longitude;
    }


    IEnumerator GetCoordinates(Action onReceived)
    {
        Debug.LogFormat("Making a request from {0}", $"https://api.concept3d.com/wayfinding/?map=1772&v2=true&toLat={destlat}&toLng={destlon}&toLevel={destLevel}&currentLevel=0&stamp=MEnBfBYK&fromLevel={currLevel}&fromLat={currlat}&fromLng={currlon}&key=0001085cc708b9cef47080f064612ca5");
        using (UnityWebRequest www = UnityWebRequest.Get($"https://api.concept3d.com/wayfinding/?map=1772&v2=true&toLat={destlat}&toLng={destlon}&toLevel={destLevel}&currentLevel=0&stamp=MEnBfBYK&fromLevel={currLevel}&fromLat={currlat}&fromLng={currlon}&key=0001085cc708b9cef47080f064612ca5"))
        {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Got text in route call!");
                // Show results as text
                json = www.downloadHandler.text;
                Debug.Log(json);

                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);
                Debug.Log("Route call: Finished deserializing");
                myDeserializedClass.fullPath.ForEach(delegate (List<double> coord)
                {
                    Debug.LogFormat("Received new coordinate {0}, {1}", coord.ElementAt(0), coord.ElementAt(1));
                    coordinates.Add(new Vector2((float)coord.ElementAt(1), (float)coord.ElementAt(0)));
                });

                Debug.Log("finished getting coordinates");
                //Debug.Log(string.Join(", ", coordinates));
                onReceived?.Invoke();

            }
        }
    }
    void OnCoordinatesReceived()
    {
        Debug.Log("should be placing spheres");
        placeSpheres();
    }

    IEnumerator autocomplete()
    {
        string[] str = inpfield.text.Split();
        Debug.Log("reached to autocomplete");
        string requrl;
        Debug.Log("The text in the textfield is" + str[0]);
        if (str.Length == 1)
        {
             requrl = $"https://api.concept3d.com/search?map=1772&q={str[0]}&ppage=5&key=0001085cc708b9cef47080f064612ca5";

        }
        else
        {
            requrl = $"https://api.concept3d.com/search?map=1772&q={str[0]}%20{str[1]}&ppage=5&key=0001085cc708b9cef47080f064612ca5";

        }
        Debug.Log(requrl+ " this is what the requrl become");
        using (UnityWebRequest www = UnityWebRequest.Get(requrl))
        {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Got text in autocomplete!");
                json = www.downloadHandler.text;
                Debug.Log(json);
    
                Example myDeserializedClass = JsonConvert.DeserializeObject<Example>(json);
                Debug.Log("this is reached too");
                foreach (Datum d in myDeserializedClass.data)
                {
                    possiblePlaces.Add(d);
                }

                destlat = myDeserializedClass.data[0].lat;
                destlon = myDeserializedClass.data[0].lng;
                destLevel = myDeserializedClass.data[0].level.ElementAt(0);

                Debug.Log("finished doing the autocomplete");
                Debug.Log(destLevel);
            }
        }
    }

    void placeSpheres()
    {
        Debug.Log("Looping through coordinates");
       // Debug.Log(coordinates);
        
        foreach(Vector2 coord in coordinates)
        {
            //Debug.Log("Reached here");
            GameObject newSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Quaternion rot = Quaternion.LookRotation(Vector3.up, Vector3.up);
            newSphere.GetComponent<Renderer>().material.color = Color.blue;
            newSphere.transform.localScale = new Vector3(3, 3, 3);
            markers2.Add(newSphere);
            _objectPosHelper.AddOrUpdateObject(gameObject: markers2.Last(), latitude: coord.x, longitude: coord.y, altitude: 0, rotationXYZToEUN: rot);
            Debug.LogFormat("adding sphere at {0}, {1}", coord.x, coord.y);

        }
        Debug.Log("Done placing spheres");
        
    }
}

public class Root
    {
        public List<List<double>> fullPath { get; set; }
        public string formattedDuration { get; set; }
        public double distance { get; set; }
        public List<Route> route { get; set; }
        public object parking { get; set; }
        public string status { get; set; }
        public string stamp { get; set; }
        public List<List<double>> bbox { get; set; }
    }

    public class Route
    {
        public string action { get; set; }
        public double angle { get; set; }
        public string code { get; set; }
        public List<List<double>> bbox { get; set; }
        public double distance { get; set; }
        public object level { get; set; }
        public List<List<double>> route { get; set; }
        public List<List<double>> routeOther { get; set; }
        public string type { get; set; }
        public int? fromLevel { get; set; }
        public int? toLevel { get; set; }
    }


public class Media
{
    public string url;
}

public class Datum
{
    public string name;
    public string description;
    public string categoryName;
    public int id;
    public int mapId;
    public int mrkId;
    public int catId;
    public double lat;
    public double lng;
    public string type;
    public double score;
    public Media? media;
    public List<int> level;
}

public class Example
{
    public int resultsPerPage;
    public int? currentPage;
    public int totalFound;
    public List<Datum> data;
}
