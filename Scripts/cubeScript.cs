//Created by Joseph Lykken and Oliver Lykken
//
using UnityEngine;
using System.Collections;

public class cubeScript : MonoBehaviour {

	public string event_name;
	public string xml_path;
    public string xml_name;
	///////////////////////////////////
	///  for the histogram
	/////////////////////////////////////
	public GameObject[] events2;
	public Object[] event_folders2;
	public GameObject[,,] myCubes;
	public GameObject cube;
	int eventNumber2 = 0;
	int num_events2 = 55498;
	public ItemContainer ic2;
	int nev = 0;
	public int NXcubes = 30;
	public int NYcubes = 30;
	public int NZcubes = 20;
    float cubesize = 0.002f;
    float MET = 0;
	float HT = 0;
    float LHT = 0;
	float MT = 0;
    float Mll = 0;
    int njets = 0;
	float ptjetmax = 0;
	float ptlmax = 0;
	float etalmax;
	float philmax = 0;
	float phijetmax = 0;
	float phimet = 0;
	float dr = 0;
	float dg = 0;
	float db = 0;
	float da = 0;
	float maxMT = 120f;
    float minMT = 0f;
    float maxHT = 400f;
    float minHT = 0f;
	float maxLHT = 7f;
    float minLHT = 3f;
	float maxeta = 3f;
	float mineta = -3f;

	float metmin = 0f;
	float metmax = 0f;
	float metstep = 1f;
    int imin = 0;
    int istep = 100;
    int imax = 100;

    float colorstep = 0.20f;
	float scalestep = 0.01f;
	float transstep = 0.04f;
	bool changeable;
	bool addmode = false;

    bool go = false;
    // Use this for initialization
    void Start()
    {
        Debug.Log("Initializing histogram cubes");
		myCubes = new GameObject[NXcubes, NYcubes, NZcubes];
		for (int z = 0; z < NZcubes; z++)
		{
			for (int y = 0; y < NYcubes; y++)
			{
				for (int x = 0; x < NXcubes; x++)
				{
                    //Debug.Log(x + ", " + y + ", " + z);
                    //myCubes[x,y,z] = Instantiate(cube, new Vector3(0.25f*x + 35, 0.125f*y + 1, 0.25f*z - 5), Quaternion.identity) as GameObject;
                    myCubes[x, y, z] = Instantiate(cube, new Vector3(0.15f * x + 40.1f, 0.15f * y + 1.89f, 0.25f * z + 8.7f), Quaternion.identity) as GameObject;
                    myCubes[x,y,z].GetComponent<MeshRenderer>().sharedMaterial.shader = Shader.Find("Transparent/Diffuse");
					myCubes[x,y,z].GetComponent<MeshRenderer>().sharedMaterial.color = new Color(0, 0, 1, 0f);
					myCubes[x,y,z].GetComponent<Transform>().localScale = new Vector3(cubesize, cubesize, cubesize);
                    myCubes[x, y, z].GetComponent<Rigidbody>().useGravity = false;
				}
			}
		}

		//Gets the events and the related data from the xml files
		event_folders2 = Resources.LoadAll("histo");
		//num_events2 = event_folders2.Length;
		events2 = new GameObject[num_events2];
		Debug.Log("Found " + num_events2 + " events");
		ic2 = new ItemContainer();
		//Begin loop over events
		//Gets the relevant data for each event
	    xml_name = "histo_30K_MET";
		xml_path = "histo/" + xml_name;
        Debug.Log("using xml_path = " + xml_path);
		ic2 = ItemContainer.Load(xml_path);

		//Grab the data that you want to use for the histogram
		foreach (Item item in ic2.items)
		    {
            MT = item.MT;
            MET = item.MET;
            HT = item.HT;
            LHT = item.LHT;
            etalmax = item.etalmax;
            Mll = item.Mll;
			/// Fill the histogram  
			int MT_index = Mathf.Min(NXcubes-1,Mathf.FloorToInt(NXcubes*MT/maxMT));
			int HT_index = Mathf.Min(NYcubes-1,Mathf.FloorToInt(NYcubes*(LHT - minLHT)/(maxLHT - minLHT)));
			int eta_index = Mathf.Min(NZcubes-1,Mathf.FloorToInt(NZcubes*(etalmax- mineta) / (maxeta - mineta)));
			MT_index = Mathf.Max(0, MT_index);
			HT_index = Mathf.Max(0, HT_index);
			eta_index = Mathf.Max(0, eta_index);
			float cubeScale = myCubes[MT_index, HT_index, eta_index].GetComponent<Transform>().localScale.magnitude;
			Color cubeColor = myCubes[MT_index, HT_index, eta_index].GetComponent<MeshRenderer>().material.color;
			if (cubeColor.a < 1)
				da = transstep;
			if (cubeColor.r < colorstep && cubeColor.g < 1f && cubeColor.b >= colorstep)
			{
				dr = 0;
				dg = colorstep;
				db = 0;
			}
			else if(cubeColor.r < colorstep && cubeColor.g > (1 - colorstep) && cubeColor.b >= colorstep)
			{
				dr = 0;
				dg = 0;
				db = -colorstep;
			}
			else if (cubeColor.r < 1f && cubeColor.g >= (1 - colorstep) && cubeColor.b < colorstep)
			{
				dr = colorstep;
				dg = 0;
				db = 0;
			}
			else if (cubeColor.r > (1 - colorstep) && cubeColor.g >= colorstep && cubeColor.b <= colorstep)
			{
				dr = 0;
				dg = -colorstep;
				db = 0;
			}
			else
			{
				dr = 0;
				dg = 0;
				db = 0;
				//da = 0;
			}
			//if (nev < 1)
			//{
				//Debug.Log("cube index = " + MT_index + ", " + HT_index + ", " + eta_index);
				//Debug.Log("MT = " + MT + "    HT = " + HT + "   etalmax = " + etalmax);
				//Debug.Log(" old Color = (" + cubeColor.r + ", " + cubeColor.g + ", " + cubeColor.b + ", " + cubeColor.a  + ")");
				//Debug.Log(" new Color = (" + (cubeColor.r + dr) + ", " + (cubeColor.g + dg) + ", " + (cubeColor.b + db) + ", " + (cubeColor.a + 0.1f) + ")");
				myCubes[MT_index, HT_index, eta_index].GetComponent<MeshRenderer>().sharedMaterial.color = new Color(cubeColor.r + dr, cubeColor.g + dg, cubeColor.b + db, cubeColor.a + da);
				//Debug.Log("old Scale = " + myCubes[MT_index, HT_index, eta_index].GetComponent<Transform>().localScale);
				if (cubeScale < 1)
					myCubes[MT_index, HT_index, eta_index].GetComponent<Transform>().localScale += new Vector3(scalestep, scalestep, scalestep);
				//Debug.Log("new Scale = " + myCubes[MT_index, HT_index, eta_index].GetComponent<Transform>().localScale);
			//}

		}
        //RebuildHistogram();
    }

    public void FixedUpdate()
    {
         RebuildHistogram();
    }

    public void RebuildHistogram()
    {
        if (addmode)
        {
            //metmin += metstep;
            //metmax = metmin + Mathf.Abs(metstep);
            imin += istep;
            imax += istep;

            //BuildUp(imin, imax);
            //Grab the data that you want to use for the histogram
            for (int i = imin; i < imax && i < ic2.items.Count; i++)
                {
                    Item item = ic2.items[i];
                    MT = item.MT;
                    MET = item.MET;
                    //HT = item.HT;
                    LHT = item.LHT;
                    etalmax = item.etalmax;
                //Mll = item.Mll;
                /// Fill the histogram 
                int MT_index = Mathf.Min(NXcubes - 1, Mathf.FloorToInt(NXcubes * MT / maxMT));
                //int HT_index = Mathf.Min(NYcubes - 1, Mathf.FloorToInt(NYcubes * HT / maxHT));
                int HT_index = Mathf.Min(NYcubes - 1, Mathf.FloorToInt(NYcubes * (LHT - minLHT) / (maxLHT - minLHT)));
                int eta_index = Mathf.Min(NZcubes - 1, Mathf.FloorToInt(NZcubes * (etalmax - mineta) / (maxeta - mineta)));
                    MT_index = Mathf.Max(0, MT_index);
                    HT_index = Mathf.Max(0, HT_index);
                    eta_index = Mathf.Max(0, eta_index);
                    float cubeScale = myCubes[MT_index, HT_index, eta_index].GetComponent<Transform>().localScale.magnitude;
                    Color cubeColor = myCubes[MT_index, HT_index, eta_index].GetComponent<MeshRenderer>().material.color;
                    if (cubeColor.a < 1)
                        da = transstep;
                    if (cubeColor.r < colorstep && cubeColor.g < 1f && cubeColor.b >= colorstep)
                    {
                        dr = 0;
                        dg = colorstep;
                        db = 0;
                    }
                    else if (cubeColor.r < colorstep && cubeColor.g > (1 - colorstep) && cubeColor.b >= colorstep)
                    {
                        dr = 0;
                        dg = 0;
                        db = -colorstep;
                    }
                    else if (cubeColor.r < 1f && cubeColor.g >= (1 - colorstep) && cubeColor.b < colorstep)
                    {
                        dr = colorstep;
                        dg = 0;
                        db = 0;
                    }
                    else if (cubeColor.r > (1 - colorstep) && cubeColor.g >= colorstep && cubeColor.b <= colorstep)
                    {
                        dr = 0;
                        dg = -colorstep;
                        db = 0;
                    }
                    else
                    {
                        dr = 0;
                        dg = 0;
                        db = 0;
                        //da = 0;
                    }
                    myCubes[MT_index, HT_index, eta_index].GetComponent<MeshRenderer>().material.color = new Color(cubeColor.r + dr, cubeColor.g + dg, cubeColor.b + db, cubeColor.a + da);
                    if (cubeScale < 1)
                        myCubes[MT_index, HT_index, eta_index].GetComponent<Transform>().localScale += new Vector3(scalestep, scalestep, scalestep);
                }
            ///  end loop over events
            if (imax > ic2.items.Count)
            {
                addmode = false;
                imin = 0;
                imax = imin + istep;
            }
        }
        else
        {
            //BuildDown(imin, imax);
            //metmin += metstep;
            //metmax = metmin + Mathf.Abs(metstep);
            imin += istep;
            imax += istep;
            //Grab the data that you want to use for the histogram
            for (int i = imin; i < imax && i < ic2.items.Count; i++)
            {
                Item item = ic2.items[i];
                MT = item.MT;
                MET = item.MET;
                //HT = item.HT;
                LHT = item.LHT;
                etalmax = item.etalmax;
                //Mll = item.Mll;
                /// Fill the histogram 
                int MT_index = Mathf.Min(NXcubes - 1, Mathf.FloorToInt(NXcubes * MT / maxMT));
                //int HT_index = Mathf.Min(NYcubes - 1, Mathf.FloorToInt(NYcubes * HT / maxHT));
                int HT_index = Mathf.Min(NYcubes - 1, Mathf.FloorToInt(NYcubes * (LHT - minLHT) / (maxLHT - minLHT)));
                int eta_index = Mathf.Min(NZcubes - 1, Mathf.FloorToInt(NZcubes * (etalmax - mineta) / (maxeta - mineta)));
                MT_index = Mathf.Max(0, MT_index);
                HT_index = Mathf.Max(0, HT_index);
                eta_index = Mathf.Max(0, eta_index);
                float cubeScale = myCubes[MT_index, HT_index, eta_index].GetComponent<Transform>().localScale.magnitude;
                Color cubeColor = myCubes[MT_index, HT_index, eta_index].GetComponent<MeshRenderer>().material.color;
                if (cubeColor.a > 0)
                    da = -transstep;
                if (cubeColor.b < colorstep && cubeColor.g < 1f && cubeColor.r >= colorstep)
                {
                    db = 0;
                    dg = colorstep;
                    dr = 0;
                }
                else if (cubeColor.b < colorstep && cubeColor.g > (1 - colorstep) && cubeColor.r >= colorstep)
                {
                    db = 0;
                    dg = 0;
                    dr = -colorstep;
                }
                else if (cubeColor.b < 1f && cubeColor.g >= (1 - colorstep) && cubeColor.r < colorstep)
                {
                    db = colorstep;
                    dg = 0;
                    dr = 0;
                }
                else if (cubeColor.b > (1 - colorstep) && cubeColor.g >= colorstep && cubeColor.r <= colorstep)
                {
                    db = 0;
                    dg = -colorstep;
                    dr = 0;
                }
                else
                {
                    dr = 0;
                    dg = 0;
                    db = 0;
                    //da = 0;
                }
                myCubes[MT_index, HT_index, eta_index].GetComponent<MeshRenderer>().material.color = new Color(cubeColor.r + dr, cubeColor.g + dg, cubeColor.b + db, cubeColor.a + da);
                if (cubeScale > 0)
                    myCubes[MT_index, HT_index, eta_index].GetComponent<Transform>().localScale += new Vector3(-scalestep, -scalestep, -scalestep);
            }
            ///  end loop over events
            if (imax > ic2.items.Count)
                    {
                        addmode = true;
                        imin = 0;
                        imax = imin + istep;
                    }
                }
            }
}
