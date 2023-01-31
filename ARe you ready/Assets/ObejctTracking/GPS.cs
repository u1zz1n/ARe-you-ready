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

    public Text[] data = new Text[4];
    public float[] locationCal = new float[3];
    public bool[] locationCheck = new bool[3];

    public bool once = true;

    float latitudeCur;
    float longitudeCur;
    float altitudeCur;

    public static int locationNumber = 0; // 0 = defalut, 1 = starbuck, 2 = school
    public static string cuntName;

    //int isUpdate = 0;

    location redmondPlace;
    location bigQFCStarbucks;
    location digipen;

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

        locationNumber = 0;

        //setting latitude and longitude point
        redmondPlace.latitude = 47.68171f;
        redmondPlace.longitude = -122.12920f;

        bigQFCStarbucks.latitude = 47.68111f;
        bigQFCStarbucks.longitude = -122.12596f;

        digipen.latitude = 47.68906f;
        digipen.longitude = -122.15040f;
        /////////////////////////////////////

        StartCoroutine(GPS_manager());
        //StartCoroutine(SetCountry());
    }

    /*public static IEnumerator SetCountry()
    {
        string ip = new System.Net.WebClient().DownloadString("https://api.ipify.org");
        string uri = $"https://ipapi.co/{ip}/json/";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            IpApiData ipApiData = IpApiData.CreateFromJSON(webRequest.downloadHandler.text);

            Debug.Log(ipApiData.country_name);
            cuntName = ipApiData.country_name;
        }
    }*/

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

        latitudeCur = Input.location.lastData.latitude;
        longitudeCur = Input.location.lastData.longitude;
        altitudeCur = Input.location.lastData.altitude;

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

       locationCal[0] = CalculateLocationOffset(redmondPlace.latitude, redmondPlace.longitude); // redmondPlace
       locationCal[1] = CalculateLocationOffset(bigQFCStarbucks.latitude, bigQFCStarbucks.longitude); // starbucks
       locationCal[2] = CalculateLocationOffset(digipen.latitude, digipen.longitude); // digipen

       if (locationCal[0] < 100.0f)
       {
           data[0].text = "It is home!: " + locationCal[0].ToString();
           locationCheck[0] = true;
       }
       else
       {
           data[0].text = "It is not home! : " + locationCal[0].ToString();
           locationCheck[0] = false;
       }

       if (locationCal[1] < 100.0f)
       {
           data[1].text = "It is starbucks!: " + locationCal[1].ToString();
           locationCheck[1] = true;
       }
       else
       {
           data[1].text = "It is not starbucks! : " + locationCal[1].ToString();
           locationCheck[1] = false;
       }

       if (locationCal[2] < 100.0f)
       {
           data[2].text = "It is digipen!: " + locationCal[2].ToString();
           locationCheck[2] = true;
       }
       else
       {
           data[2].text = "It is not digipen! : " + locationCal[2].ToString();
           locationCheck[2] = false;
       }

       if (locationCal.Min() == locationCal[0] && locationCheck[0] == true)
       {
            data[0].text = "lilly lilly Home"; //+ systemLocale.getCountry();
            locationNumber = 1;
       }
       else if (locationCal.Min() == locationCal[1] && locationCheck[0] == true)
       {
           data[1].text = "lilly lilly Starbuck";
           locationNumber = 2;
       }
       else if (locationCal.Min() == locationCal[2] && locationCheck[0] == true)
       {
           data[2].text = "lilly lilly Digipen";
           locationNumber = 3;
       }
       else
       {
           data[3].text = "No Place ";
           locationNumber = 0;
       }
       
        //cal check code
        /*float starL = 47.681042f;
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
        }*/
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
