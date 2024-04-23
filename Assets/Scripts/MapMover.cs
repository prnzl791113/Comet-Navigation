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

public class CompassWorldPose : MonoBehaviour
{
    [SerializeField] public ARCameraManager _arCameraManager;

    [SerializeField] public XROrigin _xrOrigin;
    [SerializeField] public UnityEngine.UI.Image _compassImage;
    [SerializeField] private int currLevel;
    private ARWorldPositioningCameraHelper _cameraHelper;

    private ARWorldPositioningObjectHelper _objectPosHelper;

    public GameObject cube;

    public double currlat;
    public double currlon;
    public double destlat;
    public double destlon;
    public int destLevel;
    private string json;


    private List<Vector2> coordinates;
    List<GameObject> markers2 = new List<GameObject>();

    public void Start()
    {

        Color newColor = _compassImage.color;
        newColor.a = 0;
        _compassImage.color = newColor;
        currLevel = currLevel != 0 ? currLevel : 0;
        destLevel = destLevel != 0 ? destLevel : 0;
        coordinates = new List<Vector2>();
        StartCoroutine(GetCoordinates(OnCoordinatesReceived));
        placeSpheres();
        

        /* cube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Quaternion rot = Quaternion.LookRotation(Vector3.up, Vector3.up);
            cube.GetComponent<Renderer>().material.color = Color.blue;
            cube.transform.localScale = new Vector3(2, 2, 2);
            Debug.Log("preparing to add object");
            _objectPosHelper.AddOrUpdateObject(gameObject: cube, latitude: 32.98454, longitude: -96.75154, altitude: 0, rotationXYZToEUN: rot);
            Debug.Log("added object");*/
    }
    private void Update()
    {

        // Quaternion rot = Quaternion.LookRotation(Vector3.up, Vector3.up);   
        // _objectPosHelper.AddOrUpdateObject(gameObject: cube, latitude: 32.98454, longitude: -96.75154, altitude: _cameraHelper.Altitude, rotationXYZToEUN: rot);
        //Debug.Log(_cameraHelper.Altitude);
        float heading = _cameraHelper.TrueHeading;
        //_compassImage.rectTransform.rotation = Quaternion.Euler(0, 0, heading);

        //_coordinatesText.text = "Latitude: " + _cameraHelper.Latitude + "\nLongitude: " + _cameraHelper.Longitude;
        currlat = _cameraHelper.Latitude;
        currlon = _cameraHelper.Longitude;
    }


    IEnumerator GetCoordinates(Action onReceived)
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"https://api.concept3d.com/wayfinding/?map=1772&v2=true&toLat={destlat}&toLng={destlon}&toLevel={destLevel}&currentLevel=2&stamp=MEnBfBYK&fromLevel={currLevel}&fromLat={currlat}&fromLng={currlon}&key=0001085cc708b9cef47080f064612ca5"))
        {
            yield return www.Send();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Got text!");
                // Show results as text
                json = www.downloadHandler.text;
                //Debug.Log(json);
                _cameraHelper = _arCameraManager.GetComponent<ARWorldPositioningCameraHelper>();
                _objectPosHelper = _xrOrigin.GetComponent<ARWorldPositioningObjectHelper>();

                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);
                myDeserializedClass.fullPath.ForEach(delegate (List<float> coord)
                {
                    Debug.LogFormat("{0}, {1}", coord.ElementAt(0), coord.ElementAt(1));
                    coordinates.Add(new Vector2(coord.ElementAt(1), coord.ElementAt(0)));
                });

                Debug.Log("finished getting coordinates");
                Debug.Log(coordinates);
                onReceived?.Invoke();

            }
        }
    }
    void OnCoordinatesReceived()
    {
        placeSpheres();
    }
    void placeSpheres()
    {
        
        foreach(Vector2 coord in coordinates)
        {
            Debug.Log("Reached here");
            //GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //Quaternion rot = Quaternion.LookRotation(Vector3.up, Vector3.up);
            //newCube.GetComponent<Renderer>().material.color = Color.blue;
            //newCube.transform.localScale = new Vector3(2, 2, 2);
            //markers2.Add(newCube);
            _objectPosHelper.AddOrUpdateObject(gameObject: cube, latitude: coord.x, longitude: coord.y, altitude: _cameraHelper.Altitude, rotationXYZToEUN: Quaternion.identity);
            Debug.LogFormat("adding cube at {0}, {1}", coord.x, coord.y);

        }
        
    }
}

public class Root
{
    public List<List<float>> fullPath { get; set; }
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
    public int level { get; set; }
    public List<List<double>> route { get; set; }
    public List<List<double>> routeOther { get; set; }
    public string type { get; set; }
}