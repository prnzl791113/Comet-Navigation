using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Niantic.Experimental.Lightship.AR.WorldPositioning;
using UnityEngine.XR.ARFoundation;

public class Location : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private ARCameraManager _arCameraManager;
    [SerializeField] private UnityEngine.UI.Image _compassImage;

    [SerializeField] private TextMeshProUGUI _coordinatesText;

    private ARWorldPositioningCameraHelper _cameraHelper;

    void Start()
    {
        _cameraHelper = _arCameraManager.GetComponent<ARWorldPositioningCameraHelper>();
    }

    // Update is called once per frame
    void Update()
    {
        float heading = _cameraHelper.TrueHeading;
        _compassImage.rectTransform.rotation = Quaternion.Euler(0, 0, heading);

        _coordinatesText.text = "Latitude: " + _cameraHelper.Latitude + "\nLongitude: " + _cameraHelper.Longitude;
    }
}
