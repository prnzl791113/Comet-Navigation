using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;

public class APIButtons : MonoBehaviour
{
    // Start is called before the first frame update
    public Button submit;
    public TMP_InputField locationField;
    public GameObject emptyTextField;
    public TextMeshProUGUI loc;


    void Start()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Input location not enabled");
            return;
        }
        submit.onClick.AddListener(getInputLoc);
        Input.location.Start(1f, 1f);

    }

    // Update is called once per frame

    void getInputLoc()
    {
        var g = locationField.text;
        if (g == null) Pause5Seconds();
        loc.text = Input.location.lastData.latitude.ToString() + Input.location.lastData.longitude.ToString() + "and" + locationField.text;
        //Now do an api call to find that path to the destination from current location
    }

    static async void GetDirections(dynamic origin,string destination)
    {
        string Api_key = "";
        string apicall = "https://maps.googleapis.com/maps/api/directions/json\n  ?destination=Montreal\n  &origin=Toronto\n  &key=YOUR_API_KEY";
    }
    IEnumerator Pause5Seconds()
    {
        emptyTextField.SetActive(true);
        yield return new WaitForSeconds(5);

        emptyTextField.SetActive(false);
    }


}
