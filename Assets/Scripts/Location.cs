using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Niantic.Experimental.Lightship.AR.WorldPositioning;
using UnityEngine.XR.ARFoundation;
using System.Net;
using System;


public class Location : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ARCameraManager _arCameraManager;
    [SerializeField] private UnityEngine.UI.Image compassImage;

    [SerializeField] private TextMeshProUGUI _coordinatesText;
    [SerializeField] public TMPro.TMP_InputField latlon;
    [SerializeField] public Button sub;
    public double destlat;
    public double destlon;
    private ARWorldPositioningCameraHelper _cameraHelper;
    private float dis;
    private string apiurl;
    private Vector2 devicePosition;
    private string utdmapapi;
    Gyroscope m_Gyro;

    void Start()
    {
        _cameraHelper = _arCameraManager.GetComponent<ARWorldPositioningCameraHelper>();
        sub.onClick.AddListener(locGetter);
        compassImage.gameObject.SetActive(false);
        destlat = 0;
        destlon = 0;
        dis = 0;
        Input.location.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the direction to the destination
        Vector2 destination = new Vector2((float)destlat, (float)destlon);
        devicePosition = new Vector2((float)_cameraHelper.Latitude, (float)_cameraHelper.Longitude);
        Vector2 directionToDestination = (destination - devicePosition);

        // Calculate the angle between the device's forward direction and the direction to the destination
        float angle = Mathf.Atan2(directionToDestination.y, directionToDestination.x) * Mathf.Rad2Deg;

        float heading = _cameraHelper.TrueHeading;

        Vector3 rotationRate = Input.gyro.rotationRateUnbiased;

        // Calculate the angles along X, Y, and Z axes
        float angleX = rotationRate.x * Mathf.Rad2Deg;
        float angleY = rotationRate.y * Mathf.Rad2Deg;
        float angleZ = rotationRate.z * Mathf.Rad2Deg;


        angle = angle + angleX;
        compassImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        //compassImage.transform.rota(destination);

        // Update the coordinates text
        _coordinatesText.text = "Latitude: " + _cameraHelper.Latitude + "\nLongitude: " + _cameraHelper.Longitude;

    }


    void locGetter()
    {
        string[] vals = latlon.text.Split(",");

        Debug.Log("Reacehed heere");
        if (double.TryParse(vals[0], out destlat) && double.TryParse(vals[1], out destlon))
        {
            Debug.Log("Correct loc");
            compassImage.gameObject.SetActive(true);

        }
        else
        {
            Debug.Log("Weong ");
        }
        Debug.Log("Reacehed heere 2");
        apiurl = string.Format("https://api.openrouteservice.org/v2/directions/walking-foot?api_key=5b3ce3597851110001cf6248f4413af4ad3a425fad1005afdba293d2&start={0},{1}&end={2},{3}", devicePosition.x, devicePosition.y, destlat, destlon);
        //utdmapapi = string.Format("https://api.concept3d.com/wayfinding/?map=1772&v2=true&toLat={0}&toLng={1}&toLevel=0&currentLevel=0&stamp=MEkLV4YJ&fromLevel=2&fromLat={2}&fromLng={3}&key=0001085cc708b9cef47080f064612ca5", destlat, destlon, devicePosition.x, devicePosition.y);

        StartCoroutine(apicall());
        Debug.Log("The first api call ends here");
        //StartCoroutine(routesGetter());
    }

    IEnumerator apicall()
    {
        using (var webRequest = UnityEngine.Networking.UnityWebRequest.Get(apiurl))
        {
            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check if there were any errors during the request
            if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                // Print the response data to the console
                Debug.Log("API Response: " + webRequest.downloadHandler.text);

                // You can parse the response data here and use it as needed
                // For example, if the API returns JSON data, you can use JsonUtility to deserialize it
            }
            Debug.Log("This is the first api call that gets walking direction ig??");
        }
    }


    //IEnumerator routesGetter()
    //{
    //    using (var webRequest = UnityEngine.Networking.UnityWebRequest.Get(utdmapapi))
    //    {
    //        // Send the request and wait for a response
    //        yield return webRequest.SendWebRequest();

    //        // Check if there were any errors during the request
    //        if (webRequest.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
    //        {
    //            Debug.LogError("Error: " + webRequest.error);
    //        }
    //        else
    //        {
    //            // Print the response data to the console
    //            Debug.Log("API Response: " + webRequest.result);
    //            string jsonResponse = webRequest.downloadHandler.text;
    //            Debug.Log("API Response: " + jsonResponse);

    //            // Deserialize the JSON response
    //            ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonResponse);

    //            // Access the fullPath array
    //            float[][] fullPath = responseData.fullPath;
    //            foreach(var v in fullPath)
    //            {
    //                Debug.Log(v+"This is one");
    //            }


    //            // You can parse the response data here and use it as needed
    //            // For example, if the API returns JSON data, you can use JsonUtility to deserialize it
    //        }
    //    }
    //}
}
