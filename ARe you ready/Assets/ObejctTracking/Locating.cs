using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using UnityEngine.Networking;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public class LocationInfo
{
	public string city;
	public string region;
	public string country_name;

	public static LocationInfo CreateFromJSON(string jsonString)
	{
		return JsonUtility.FromJson<LocationInfo>(jsonString);
	}
}

public enum ADDRESSFAM
{
	IPv4, IPv6
};

public class Locating : MonoBehaviour
{
	public static string cityName;
	public static string regionName;
	public static string countryName;

	// Start is called before the first frame update
	void Start()
    {
		cityName = " ";
		regionName = " ";
		countryName = " ";

		StartCoroutine(SetCountry());
	}

	public static string GetIP(ADDRESSFAM Addfam)
	{
		//Return null if ADDRESSFAM is Ipv6 but Os does not support it
		if (Addfam == ADDRESSFAM.IPv6 && !Socket.OSSupportsIPv6)
		{
			return null;
		}

		string output = "0.0.0.0";

		foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
		{
			#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
			NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
			NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

			if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
			#endif
			{
				foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
				{
					//IPv4
					if (Addfam == ADDRESSFAM.IPv4)
					{
						if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
						{
							if (ip.Address.ToString() != "127.0.0.1")
								output = ip.Address.ToString();
						}
					}

					//IPv6
					else if (Addfam == ADDRESSFAM.IPv6)
					{
						if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
						{
							if (ip.Address.ToString() != "127.0.0.1")
								output = ip.Address.ToString();
						}
					}
				}
			}
		}
		return output;
	}

	IEnumerator SetCountry()
	{
		//string ip = "73.42.171.127";//new System.Net.WebClient().DownloadString("https://api.ipify.org");
		//string ip = new WebClient().DownloadString("https://icanhazip.com/");
		//string ip = GetIP(ADDRESSFAM.IPv6); //Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();

		//WebClient client = new WebClient();

		string IP_ADDRESS_API = GetIP(ADDRESSFAM.IPv6); //"https://api.ipify.org";
		//const string IP_LOCATION_API = "https://ipapi.co/json";

		UnityWebRequest www = UnityWebRequest.Get(IP_ADDRESS_API);
		//UnityWebRequest www_city = UnityWebRequest.Get(IP_LOCATION_API);

		string ip = www.downloadHandler.text.Trim();
		string uri = $"https://ipapi.co/{ip}/json/";

		using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
		{
			yield return webRequest.SendWebRequest();

			string[] pages = uri.Split('/');
			int page = pages.Length - 1;

			LocationInfo locations = LocationInfo.CreateFromJSON(webRequest.downloadHandler.text);

			cityName = locations.city;
			regionName = locations.region;
			countryName = locations.country_name;

			Debug.Log("ip is " + IP_ADDRESS_API);
			Debug.Log("country is " + locations.country_name);
			Debug.Log("region is " + locations.region);
			Debug.Log("country is " + locations.city);
		}

		//string locationJson = www_city.downloadHandler.text;
		//Location location = JsonUtility.FromJson<Location>(locationJson);
		//Debug.Log("City: " + location.city);

		//ShowAndroidToastMessage("ip is " + IP_ADDRESS_API);

	}

	// Update is called once per frame
	void Update()
    {
		//GPS.data[0].text += "\n" + countryName;// + "\n" + regionName;
	}

	/// <summary>
	/// Show an Android toast message.
	/// </summary>
	/// <param name="message">Message string to show in the toast.</param>
	private static void ShowAndroidToastMessage(string message)
	{
#if UNITY_ANDROID
		using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		var unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		if (unityActivity == null) return;
		var toastClass = new AndroidJavaClass("android.widget.Toast");
		unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
		{
			// Last parameter = length. Toast.LENGTH_LONG = 1
			using var toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText",
				unityActivity, message, 1);
			toastObject.Call("show");
		}));
#endif
	}
}
