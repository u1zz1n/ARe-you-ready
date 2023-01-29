using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
public class GPS : MonoBehaviour
{
    /*public Text gpsOut;
    public bool isUpdating;
    private void Update()
    {
        if (!isUpdating)
        {
            StartCoroutine(GetLocation());
            isUpdating = !isUpdating;
        }
    }
    IEnumerator GetLocation()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield return new WaitForSeconds(10);

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 10;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait < 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            gpsOut.text = "Timed out";
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            gpsOut.text = "Unable to determine device location";
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            gpsOut.text = "Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + 100f + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp;
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop service if there is no need to query location updates continuously
        isUpdating = !isUpdating;
        Input.location.Stop();
    }*/

    public Text[] data = new Text[4];
    //public float maxTime = 100.0f;

    float latitudeCur;
    float longitudeCur;
    float altitudeCur;

    void Start()
    {
        StartCoroutine(GPS_manager());
    }

    private IEnumerator GPS_manager()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);

            while(!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
            }
        }

        if (!Input.location.isEnabledByUser)
        {
            data[3].text = "User has not enabled GPS";
            yield break;
        }

        Input.location.Start();

        int maxTime = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxTime > 0)
        {
            yield return new WaitForSeconds(1);
            maxTime--;
        }

        if (Input.location.status == LocationServiceStatus.Failed || Input.location.status == LocationServiceStatus.Stopped)
        {
            data[3].text = "Unable to determine device location";
        }

        if (maxTime <= 1)
        {
            data[3].text = "Timed out";
            yield break;
        }

        //latitudeCur = Input.location.lastData.latitude;
        //longitudeCur = Input.location.lastData.longitude;
        //altitudeCur = Input.location.lastData.altitude;

        data[0].text = "Latitude : " + latitudeCur.ToString();
        data[1].text = "Longitude : " + longitudeCur.ToString();
        data[2].text = "Altitude : " + altitudeCur.ToString();
        data[3].text = "Done";

        Input.location.Stop();
        
        yield break;
    }
    void Update()
    {
        //StartCoroutine(GPS_manager());

        //home = 47.681774 -122.129486


        //home = 47.682021 -122.131673
        //home = 47.682042 -122.127673
        //home = 47.681565 -122.127662
        //home = 47.681500 -122.131734

        float starL = 47.681042f;
        float starLo = -122.2125707f;

        latitudeCur = 47.680373f;
        longitudeCur = -122.125507f;

        if (CalculateLocationOffset(starL, starLo) < 100.0f) //nono
        {
            data[0].text = CalculateLocationOffset(starL, starLo).ToString(); //0.03002862
        }
        else
        {
            data[0].text = "NOnono";
        }

        latitudeCur = 47.681056f;
        longitudeCur = -122.125705f;

        if (CalculateLocationOffset(starL, starLo) < 100.0f) //yesyes
        {
            data[1].text = CalculateLocationOffset(starL, starLo).ToString(); //0.0006848066
        }
        else
        {
            data[1].text = "NOnono";
        }

        latitudeCur = 47.681236f;
        longitudeCur = -122.125888f;

        if (CalculateLocationOffset(starL, starLo) < 100.0f) //yesyes
        {
            data[2].text = CalculateLocationOffset(starL, starLo).ToString(); //0.00871902
        }
        else
        {
            data[2].text = "NOnono";
        }
    }

    private float CalculateLocationOffset(float latitudeB, float longitudeB)
    {
        float firstSinSqure = Mathf.Sin((Mathf.PI * (latitudeB - latitudeCur)/ 360) * Mathf.Sin((Mathf.PI * (latitudeB - latitudeCur)) / 360));
        float firstCosSqure = Mathf.Cos((Mathf.PI * longitudeB) / 180);
        float secondCosSqure = Mathf.Cos((Mathf.PI * longitudeCur) / 180);
        float secondSinSqure = Mathf.Sin((Mathf.PI * (longitudeB - longitudeCur) / 360) * Mathf.Sin((Mathf.PI * (longitudeB - longitudeCur)) / 360));

        float inDistanceSqure = Mathf.Sqrt(firstSinSqure * firstCosSqure * secondCosSqure * secondSinSqure);
        float distance = 127420000000 * Mathf.Asin(inDistanceSqure);

        return distance;
    }
}
