using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Location : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI info;
    string last;

    void Start()
    {
        if (!Input.location.isEnabledByUser)
        {
            return;
        }
        last = "";

        Input.location.Start(1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
        info.text = Input.location.lastData.latitude.ToString()+Input.location.lastData.longitude;
        ExecuteAfterDelay(5);
    }
    IEnumerator ExecuteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Code to execute after the delay
        Debug.Log("Code executed after 5 seconds.");
    }
}
