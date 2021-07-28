using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Valve.Newtonsoft.Json;
using Valve.Newtonsoft.Json.Linq;
using Valve.VR;
using UnityEditor;
/// <summary>
///Auto Binder de trackers,
///
/// Utilisation :
///1) Copier C:\Program Files(x86)\Steam\config\steamvr.vrsettings et mettre son extension en.json(Todo le trouver automatiquement)
///2) Reference le fichier steamvr dans le script
///3) indiquer les TrackedObject des tracker et leurs role souhaité
///
/// Du a un bug dans le plugin d'OpenVR, fait regulierement crash Unity donc code commenté par sécurite
/// 
/// private OpenVR test;
/// 
/// </summary>
[CustomEditor(typeof(AutoBinder))]
public class AutoBinderEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        /*
        if (GUILayout.Button("Generate"))
        {
            AutoBinder a = target as AutoBinder;
            a.BindTrackers();
        }
        */
    }

}
public class AutoBinder : MonoBehaviour
{

    public enum TrackerRole { TrackerRole_None,TrackerRole_Waist, TrackerRole_RightFoot, TrackerRole_LeftFoot }

    [System.Serializable]
    public class TrackerConfig {
        public TrackerRole role;
        public SteamVR_TrackedObject obj;
        [Header("Will generate")]
        public string serialNumber;
        public uint id;
    }

    //File under Steam/config/vrconfig.json
    public string steamvrPath = "C:\\Program Files(x86)\\Steam\\config\\steamvr.vrsettings";
    public List<TrackerConfig> trackers;

    //steamvrconfig :
    //role -> manufacturer -> 

    //unityconfig :
    //manufacturer-> id

    //trackerconfig :
    //role->id

    //dictionnaire des roles ecris dans steamvr.json
    Dictionary<TrackerRole, List<string>> roleToSerial = new Dictionary<TrackerRole, List<string>>();
    //dictionnaire des numSerie to num trackerIndex
    Dictionary<string, uint> serialToId = new Dictionary<string, uint>();

    private TrackerRole ParseTrackerRole(string name)
    {
        return (TrackerRole)Enum.Parse(typeof(TrackerRole), name, true); ;
    }

    void setTrackedObjects() { 
    
        foreach (var v in trackers)
        {
            List<string> steamvr_roles = roleToSerial[v.role];
            foreach (string r in steamvr_roles)
            {
                if (serialToId.ContainsKey(r))
                {
                    v.serialNumber = r;
                    v.id = serialToId[v.serialNumber];

                    SteamVR_TrackedObject to = v.obj;
                    to.SetDeviceIndex((int)v.id);
                }

            }
        }

    }

    private void ParseSteamVRJson()
    {

        string text = "";
        try
        {
            text = System.IO.File.ReadAllText(steamvrPath);
        }
        catch(Exception e)
        {
            Debug.LogError("Fichier steamvr.vrsettings introuvable, verifiez le chemin");
            return;
        }
        JObject jsonfile = JObject.Parse(text);

        // get JSON result objects into a list
        var subjson = jsonfile["trackers"];
        Dictionary<string, string> trackersDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(subjson.ToString());

        foreach (KeyValuePair<string, string> kvp in trackersDict)
        {
            TrackerRole t = ParseTrackerRole(kvp.Value);
            if (roleToSerial.ContainsKey(t))
            {
                roleToSerial[t].Add(kvp.Key.Substring(25));
            }
            else
            {
                roleToSerial.Add(t, new List<string>{
                    kvp.Key.Substring(25)
                });
            }
        }
    }

    private void Start()
    {
        BindTrackers();
    }

    public void BindTrackers() {

        Debug.Log("Debut binding trackers !");
        if (!ListDevices())
            return;
        ParseSteamVRJson();
        setTrackedObjects();

        Debug.Log("Binding trackers Terminée !!");
    }
    bool ListDevices()
    {
        serialToId.Clear();

        var error = ETrackedPropertyError.TrackedProp_Success;
        for (uint i = 0; i < 16; i++)
        {
            var result = new System.Text.StringBuilder((int)64);
            OpenVR.System.GetStringTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_RenderModelName_String, result, 64, ref error);

            var model = new System.Text.StringBuilder((int)64);
            OpenVR.System.GetStringTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_SerialNumber_String, model, 64, ref error);
            if (result.ToString().Contains("tracker") && model.Length > 0)
            {
                serialToId.Add(model.ToString(), i);
            }
        }
        return true;

    }
    

}

