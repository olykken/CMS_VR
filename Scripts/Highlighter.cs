using UnityEngine;
using TubeRender;
using CMSdraw;

public class Highlighter : MonoBehaviour {

    public Material highlightMaterial;
    public NewSteamVR_LaserPointer laserPointer;

    private Material electronMat;
    private Material trackMat;
    private Material jetMat;
    private Material metMat;
    private Material muonMat;
    private Material rechitMat;

    private MeshRenderer objectMesh;
    private Rechit rechitToHighlight;
    private TubeRenderer meshToHighlight;
    private string lastObj;

    private void Start()
    {
        highlightMaterial = Resources.Load("Materials/Highlight", typeof(Material)) as Material;
        trackMat = Resources.Load("Materials/TrackMat", typeof(Material)) as Material;
        jetMat = Resources.Load("Materials/JetMat", typeof(Material)) as Material;
        metMat = Resources.Load("Materials/MetMat", typeof(Material)) as Material;
        muonMat = Resources.Load("Materials/Emissive_red", typeof(Material)) as Material;
        rechitMat = Resources.Load("Materials/Emissive_yellow", typeof(Material)) as Material;
        electronMat = Resources.Load("Materials/ElectronMat", typeof(Material)) as Material;

        laserPointer = GameObject.Find("Hand2").GetComponent<NewSteamVR_LaserPointer>();
        laserPointer.PointerIn -= HandlePointerIn;
        laserPointer.PointerIn += HandlePointerIn;
        laserPointer.PointerOut -= HandlePointerOut;
        laserPointer.PointerOut += HandlePointerOut;
    }

    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        meshToHighlight = e.target.GetComponent<TubeRenderer>();
        if(meshToHighlight != null)
        {
            objectMesh = meshToHighlight.gameObject.GetComponentInChildren<MeshRenderer>();
            objectMesh.material = highlightMaterial;
            lastObj = objectMesh.gameObject.name;
        }

        rechitToHighlight = e.target.GetComponent<Rechit>();
        if(rechitToHighlight != null)
        {
            objectMesh = rechitToHighlight.gameObject.GetComponentInChildren<MeshRenderer>();
            objectMesh.material = highlightMaterial;
            lastObj = objectMesh.gameObject.name;
        }
    }

    private void HandlePointerOut(object sender, PointerEventArgs e)
    {
        if(objectMesh!=null)
        {
            if (objectMesh.gameObject.name.Contains("electron"))
            {
                objectMesh.material = electronMat;
            }
            else if (objectMesh.gameObject.name.Contains("track"))
            {
                objectMesh.material = trackMat;
            }
            else if (objectMesh.gameObject.name.Contains("jet"))
            {
                objectMesh.material = jetMat;
            }
            else if (objectMesh.gameObject.name.Contains("MET"))
            {
                objectMesh.material = metMat;
            }
            else if (objectMesh.gameObject.name.Contains("muon"))
            {
                objectMesh.material = muonMat;
            }
            else if (objectMesh.gameObject.name.Contains("rechit"))
            {
                objectMesh.material = rechitMat;
            }
        }
    }
}
