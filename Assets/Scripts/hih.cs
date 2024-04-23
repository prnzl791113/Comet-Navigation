//Initial Worldpose shown in demo

using System;
using System.Xml.Serialization;
using Unity.XR.CoreUtils;
using Niantic.Experimental.Lightship.AR.WorldPositioning;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Networking;
using System.Collections;

namespace WorldPoseSamples
{
    public class CompassWorldPose : MonoBehaviour
    {
        [SerializeField] private ARCameraManager _arCameraManager;

        [SerializeField] private XROrigin _xrOrigin;
        [SerializeField] private UnityEngine.UI.Image _compassImage;

        [SerializeField] private UnityEngine.UI.Text _coordinatesText;

        private ARWorldPositioningCameraHelper _cameraHelper;

        private ARWorldPositioningObjectHelper _objectPosHelper;

        GameObject cube;

        GameObject[] markers;

        Vector2[] coordinates;

        public void Start()
        {

            Color newColor = _compassImage.color;
            newColor.a = 0;
            _compassImage.color = newColor;
            coordinates = new Vector2[30];
            markers = new GameObject[30];
            coordinates[0] = new Vector2(-96.7508326f, 32.9859696f);
            coordinates[1] = new Vector2(-96.7508845f, 32.9860584f);
            coordinates[2] = new Vector2(-96.7508818f, 32.9861432f);
            coordinates[3] = new Vector2(-96.750901f, 32.9861454f);
            coordinates[4] = new Vector2(-96.75090149045f, 32.986174125453f);
            coordinates[5] = new Vector2(-96.7509525f, 32.9861719f);
            coordinates[6] = new Vector2(-96.750946417451f, 32.986351300349f);
            coordinates[7] = new Vector2(-96.750946417451f, 32.98644635594f);
            coordinates[8] = new Vector2(-96.750945746899f, 32.986571784167f);
            coordinates[9] = new Vector2(-96.7509457f, 32.986637324576186f);
            coordinates[10] = new Vector2(-96.7509451f, 32.9866983f);
            coordinates[11] = new Vector2(-96.7509451f, 32.9868103f);
            coordinates[12] = new Vector2(-96.750944405794f, 32.986867636588f);
            coordinates[13] = new Vector2(-96.750951781869f, 32.987139302481f);
            coordinates[14] = new Vector2(-96.750911548734f, 32.987138740027f);
            coordinates[15] = new Vector2(-96.750910207629f, 32.98729960184f);
            coordinates[16] = new Vector2(-96.750945076346f, 32.987360909237f);
            coordinates[17] = new Vector2(-96.7494404f, 32.9854924f);
            coordinates[18] = new Vector2(-96.750945076346f, 32.987418841876f);
            coordinates[19] = new Vector2(-96.749195605516f, 32.985555979144f);
            coordinates[20] = new Vector2(-96.749078258872f, 32.985561603789f);
            coordinates[20] = new Vector2(-96.748972982168f, 32.985627974569f);

            _cameraHelper = _arCameraManager.GetComponent<ARWorldPositioningCameraHelper>();
            _objectPosHelper = _xrOrigin.GetComponent<ARWorldPositioningObjectHelper>();

            StartCoroutine(GetText());

            int index = 0;
            foreach (Vector2 coord in coordinates)
            {
                if (coord != null)
                {
                    GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Quaternion rot = Quaternion.LookRotation(Vector3.up, Vector3.up);
                    newCube.GetComponent<Renderer>().material.color = Color.blue;
                    newCube.transform.localScale = new Vector3(2, 2, 2);
                    markers[index] = newCube;
                    //  Debug.Log("preparing to add object");
                    _objectPosHelper.AddOrUpdateObject(gameObject: markers[index], latitude: coord.y, longitude: coord.x, altitude: 0, rotationXYZToEUN: rot);
                    // Debug.Log("added object");
                    index += 1;
                }
            }

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

            _coordinatesText.text = "Latitude: " + _cameraHelper.Latitude + "\nLongitude: " + _cameraHelper.Longitude;
        }

        public string json;

        IEnumerator GetText()
        {
            using (UnityWebRequest www = UnityWebRequest.Get("https://api.concept3d.com/wayfinding/?map=1772&v2=true&toLat=32.989246&toLng=-96.750488&toLevel=1&currentLevel=0&stamp=MDzXlttL&fromLevel=0&fromLat=32.98803408650673&fromLng=-96.74849102745358&key=0001085cc708b9cef47080f064612ca5"))
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
                    Debug.Log(json);

                    RouteInfo route = RouteInfo.CreateFromJSON(json);
                    Debug.Log(route.fullPath);
                }
            }
        }
    }

    [System.Serializable]
    public class RouteInfo
    {
        public float[][] fullPath;

        public static RouteInfo CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<RouteInfo>(jsonString);
        }
    }
}