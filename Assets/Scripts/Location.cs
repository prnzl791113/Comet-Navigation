using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Niantic.Experimental.Lightship.AR.WorldPositioning;
using UnityEngine.XR.ARFoundation;
using System.Net;

public class Location : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ARCameraManager _arCameraManager;
    [SerializeField] private UnityEngine.UI.Image compassImage;

    [SerializeField] private TextMeshProUGUI _coordinatesText;
    [SerializeField] public TMPro.TMP_InputField latlon;
    [SerializeField] public Button sub;
    private double destlat;
    private double destlon;
    private ARWorldPositioningCameraHelper _cameraHelper;
    private float dis;
    void Start()
    {
        _cameraHelper = _arCameraManager.GetComponent<ARWorldPositioningCameraHelper>();
        sub.onClick.AddListener(locGetter);
        compassImage.gameObject.SetActive(false);
        destlat = 0;
        destlon = 0;
        dis = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the direction to the destination
        Vector2 destination = new Vector2((float)destlat, (float)destlon);
        Vector2 devicePosition = new Vector2((float)_cameraHelper.Latitude, (float)_cameraHelper.Longitude);
        Vector2 directionToDestination = (destination - devicePosition);

        // Calculate the angle between the device's forward direction and the direction to the destination
        float angle = Mathf.Atan2(directionToDestination.y, directionToDestination.x) * Mathf.Rad2Deg;
        float heading = _cameraHelper.TrueHeading;
        // Rotate the compass image to point towards the destination
        compassImage.rectTransform.rotation = Quaternion.Euler(0, 0, heading);


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

    }
}
