using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using System.Linq;
//using UnityEngine.Networking;

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

    public struct location
    {
        public float latitude;
        public float longitude;
    };

    public InputField userLa;
    public InputField userLo;

    public Text[] data = new Text[1];
    public float[] locationCal = new float[5];
    public bool[] locationCheck = new bool[5];

    public bool once = true;

    public static int locationNumber = 0; 
    /* 
     * 0 = defalut 
     * 1 = starbuck 
     * 2 = school 
     * 3 = redmond library 
     * 4 = digipen library
    */
    public static string cuntName;

    //int isUpdate = 0;

    public static location curPlace;
    public static bool unknownPlace;
    location redmondPlace;
    location bigQFCStarbucks;
    location digipen;
    location redmondLibrary;
    location digipenLibrary;

    /*public class IpApiData
    {
        public string country_name;

        public static IpApiData CreateFromJSON(string jsonString)
        {
            return JsonUtility.FromJson<IpApiData>(jsonString);
        }
    }*/

    void Start()
    {
        once = true;
        unknownPlace = false;

        locationNumber = 0;

        //setting latitude and longitude point
        redmondPlace.latitude = 47.68171f;
        redmondPlace.longitude = -122.12920f;

        bigQFCStarbucks.latitude = 47.68106998946979f;
        bigQFCStarbucks.longitude = -122.12582419123895f;
        //47.68106998946979, -122.12582419123895
        digipen.latitude = 47.68906f;
        digipen.longitude = -122.15040f;

        redmondLibrary.latitude = 47.678956999173984f;
        redmondLibrary.longitude = -122.1280648501779f;
        //47.678956999173984, -122.1280648501779
        digipenLibrary.latitude = 47.687963f;
        digipenLibrary.longitude = -122.151153f;
        /////////////////////////////////////

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
            data[0].text = "User has not enabled GPS";
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
            data[0].text = "Unable to determine device location";
        }

        if (maxTime <= 1)
        {
            data[0].text = "Timed out";
            yield break;
        }

        curPlace.latitude = Input.location.lastData.latitude;
        curPlace.longitude = Input.location.lastData.longitude;
        //altitudeCur = Input.location.lastData.altitude;

        //data[0].text = "Latitude : " + latitudeCur.ToString();
        //data[1].text = "Longitude : " + longitudeCur.ToString();
        //data[2].text = "Altitude : " + altitudeCur.ToString();
        //data[3].text = "Done";

        Input.location.Stop();
        
        yield break;
    }
    void Update()
    {
        //StartCoroutine(GPS_manager());
        if(UpdateLocationForDebug.checkUpdate)
        {
            curPlace.latitude = float.Parse(userLa.text);
            curPlace.longitude = float.Parse(userLo.text);

            Debug.Log(userLa.text);
            Debug.Log(userLo.text); 

            UpdateLocationForDebug.checkUpdate = false;
        }

        locationCal[0] = CalculateLocationOffset(redmondPlace.latitude, redmondPlace.longitude); // redmondPlace
        locationCal[1] = CalculateLocationOffset(bigQFCStarbucks.latitude, bigQFCStarbucks.longitude); // starbucks
        locationCal[2] = CalculateLocationOffset(digipen.latitude, digipen.longitude); // digipen
        locationCal[3] = CalculateLocationOffset(redmondLibrary.latitude, redmondLibrary.longitude); // redmond library
        locationCal[4] = CalculateLocationOffset(digipenLibrary.latitude, digipenLibrary.longitude); // digipen library

        if (locationCal[0] < 100.0f)
        {
            locationCheck[0] = true;
        }
        else
        {
            locationCheck[0] = false;
        }
        
        if (locationCal[1] < 100.0f)
        {
            locationCheck[1] = true;
        }
        else
        {
            locationCheck[1] = false;
        }
        
        if (locationCal[2] < 100.0f)
        {
            locationCheck[2] = true;
        }
        else
        {
            locationCheck[2] = false;
        }

        if (locationCal[3] < 100.0f)
        {
            locationCheck[3] = true;
        }
        else
        {
            locationCheck[3] = false;
        }

        if (locationCal[4] < 100.0f)
        {
            locationCheck[4] = true;
        }
        else
        {
            locationCheck[4] = false;
        }

        Debug.Log(locationCal.Min());

        if (locationCal.Min() == locationCal[0] && locationCheck[0] == true)
        {
             data[0].text = "You are at Home";
             locationNumber = 0;
             unknownPlace = false;
        }
        else if (locationCal.Min() == locationCal[1] && locationCheck[1] == true)
        {
            data[0].text = "You are at Starbuck";
            locationNumber = 1;
            unknownPlace = false;
        }
        else if (locationCal.Min() == locationCal[2] && locationCheck[2] == true)
        {
            data[0].text = "You are at Digipen";
            locationNumber = 2;
            unknownPlace = false;
        }
        else if (locationCal.Min() == locationCal[3] && locationCheck[3] == true)
        {
            data[0].text = "You are at Library";
            locationNumber = 3;
            unknownPlace = false;
        }
        else if (locationCal.Min() == locationCal[4] && locationCheck[4] == true)
        {
            data[0].text = "You are at Digipen Library";
            locationNumber = 4;
            unknownPlace = false;
        }
        else
        {
            data[0].text = "We cannot find your location ";
            locationNumber = 0;
            unknownPlace = true;
        }

        data[0].text += "\n" + Locating.countryName + "\n" + Locating.regionName;

    }

    private float CalculateLocationOffset(float latitudeB, float longitudeB)
    {
        //starbucks sample location
        //curPlace.latitude = 47.681426f;
        //curPlace.longitude = -122.126919f;

        //home sample location
        //curPlace.latitude = 47.68191277999324f;
        //curPlace.longitude = -122.13011754289914f;

        //digipen sample location
        //curPlace.latitude = 47.68890776558581f;
        //curPlace.longitude = -122.15030925149208f;


        float firstSinSqure = Mathf.Sin((Mathf.PI * (latitudeB - curPlace.latitude) / 360) * Mathf.Sin((Mathf.PI * (latitudeB - curPlace.latitude)) / 360));
        float firstCosSqure = Mathf.Cos((Mathf.PI * longitudeB) / 180);
        float secondCosSqure = Mathf.Cos((Mathf.PI * curPlace.longitude) / 180);
        float secondSinSqure = Mathf.Sin((Mathf.PI * (longitudeB - curPlace.longitude) / 360) * Mathf.Sin((Mathf.PI * (longitudeB - curPlace.longitude)) / 360));

        float inDistanceSqure = Mathf.Sqrt(firstSinSqure * firstCosSqure * secondCosSqure * secondSinSqure);
        float distance = 127420000000 * Mathf.Asin(inDistanceSqure);

        return distance;
    }
}
