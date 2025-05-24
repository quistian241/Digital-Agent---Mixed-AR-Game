using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Oculus.Interaction.Throw;
using TMPro;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;

public class spatialAnchors : MonoBehaviour
{
    public GameObject desk;
    public GameObject wall;
    public GameObject leftHand;
    public GameObject rightHand;
    private List<OVRSpatialAnchor> anchorInstances = new();
    private List<System.Guid> anchorUuids = new();
    List<OVRSpatialAnchor.UnboundAnchor> unboundAnchors = new();

    // Floor, Right, Front, Left, Back
    Quaternion[] roomRotations = {Quaternion.identity,Quaternion.Euler(0,0,90),Quaternion.Euler(-90,0,0),Quaternion.Euler(0,0,-90),Quaternion.Euler(90,0,0)};
    int idx = 0;

    private async void SetupAnchorAsync(OVRSpatialAnchor anchor, string id, bool saveAnchor)
    {
        if (!await anchor.WhenLocalizedAsync())
        {
            Debug.LogError("Unable to create anchor");
            Destroy(anchor.gameObject);
            return;
        }

        anchorInstances.Add(anchor);

        if (saveAnchor && (await anchor.SaveAnchorAsync()).Success)
        {
            PlayerPrefs.SetString(id, anchor.Uuid.ToString());
        }
    }

    async void LoadAnchorByUuid(IEnumerable<System.Guid> uuids)
    {
        var result = await OVRSpatialAnchor.LoadUnboundAnchorsAsync(uuids, unboundAnchors);
        if (result.Success)
        {
            Debug.Log("Anchors loaded successfully");
            for (int i = 0; i < result.Value.Count; i++)
            {
                OVRSpatialAnchor.UnboundAnchor unboundAnchor = result.Value[i];
                unboundAnchor.LocalizeAsync().ContinueWith((success, anchor) =>
                {
                    if (success)
                    {
                        GameObject spatialAnchor = Instantiate(wall, new Vector3(0, 0, 0), Quaternion.identity);
                        spatialAnchor.GetComponent<MeshRenderer>().enabled = false;

                        unboundAnchor.BindTo(spatialAnchor.AddComponent<OVRSpatialAnchor>());
                    }
                    else
                    {
                        Debug.LogError($"Localization failed for anchor {unboundAnchor.Uuid}");
                    }
                }, unboundAnchor);
            }
        }
        else
        {
            Debug.LogError($"Load failed with error {result.Status}");
        }
    }

    void loadUuids()
    {
        for (int i = 0; i < roomRotations.Length; i++)
        {
            if (PlayerPrefs.HasKey(i.ToString())) {
                anchorUuids.Add(new System.Guid(PlayerPrefs.GetString(i.ToString())));
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        // //Floor
        // GameObject anchor = Instantiate(obj, new Vector3(0, -160, 0), Quaternion.identity);
        // SetupAnchorAsync(anchor.AddComponent<OVRSpatialAnchor>(), false);
        // //Forward Wall
        // anchor = Instantiate(obj, new Vector3(0, 0, 640), Quaternion.Euler(-90,0,0));
        // SetupAnchorAsync(anchor.AddComponent<OVRSpatialAnchor>(), false);
        // //Backward Wall
        // anchor = Instantiate(obj, new Vector3(0, 0, -640), Quaternion.Euler(90,0,0));
        // SetupAnchorAsync(anchor.AddComponent<OVRSpatialAnchor>(), false);
        // //Right Wall
        // anchor = Instantiate(obj, new Vector3(320, 0, 0), Quaternion.Euler(0,0,90));
        // SetupAnchorAsync(anchor.AddComponent<OVRSpatialAnchor>(), false);
        // //Left Wall
        // anchor = Instantiate(obj, new Vector3(-320, 0, 0), Quaternion.Euler(0,0,-90));
        // SetupAnchorAsync(anchor.AddComponent<OVRSpatialAnchor>(), false);
        loadUuids();
        if (anchorUuids.Count != 0)
        {
            LoadAnchorByUuid(anchorUuids);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            GameObject anchor = Instantiate(wall, leftHand.transform.position, Quaternion.LookRotation(leftHand.transform.forward, -leftHand.transform.up));
            SetupAnchorAsync(anchor.AddComponent<OVRSpatialAnchor>(), idx.ToString(), true);
            idx++;
        }
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            GameObject anchor = Instantiate(desk, rightHand.transform.position, Quaternion.LookRotation(rightHand.transform.forward, -rightHand.transform.up));
            //SetupAnchorAsync(anchor.AddComponent<OVRSpatialAnchor>(), idx, true);
            //idx++;
        }
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
