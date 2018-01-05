using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EadaRadViz : MonoBehaviour {

	public GameObject PointPrefab;
	public GameObject PointPrefab2015;
	public GameObject AnchorPrefab;
	public GameObject LinePrefab;
	public GameObject YearLinePrefab;
	public GameObject PointsHolder;
	public GameObject PointsHolder2015;
	public GameObject AnchorsHolder;
	public GameObject LinesHolder;
	public GameObject YearLinksHolder;
	public GameObject OriginPt;

	public string inputfile;
	public string inputfile2015;
	public string mapfile;
	public float ptScale;
	public int explodeScale = 5;
	public int spaceScale = 5;
	public float LINE_WIDTH = 0.04f;

	private List<Vector3> Anchors = new List<Vector3>();
	private List<Vector3> oldAnchors = new List<Vector3>();

	private Dictionary<string, Color> ListDataColor = new Dictionary<string, Color>{
		{ "1", new Color(1f, 0.1f, 0.1f) },
		{ "2", new Color(0.1f, 1f, 0.1f) },
		{ "4", new Color(0.1f, 0.1f, 1f) },
		{ "3", new Color(1f, 0.05f, 0.85f) },
		{ "5", new Color(0.85f, 0.05f, 1f) },
		{ "6", new Color(0.05f, 0.85f, 1f) }
	};

	private static Color defaultColor = new Color(0.8f, 0.2f, 0.2f, 0.8f);
	private static Color defaultColor2015 = new Color(0.2f, 0.8f, 0.2f, 0.8f);

	private Vector3 OldOrigin;
	private static int COL_NUM = 12;
	//private static float LINE_WIDTH = 0.08f;

	private List<string> colName;
	private List<string> listDetails;
	private List<string> listClassification;
	private List<string> listDetails2015;
	private List<string> listClassification2015;
	private List<bool> listFilter;
	private List<bool> listFilterAll = new List<bool>();
	private List<bool> listFilterNone = new List<bool>();
	private List<bool> listFilterParticipant = new List<bool>();
	private List<bool> listFilterStaff = new List<bool>();
	private List<bool> listFilterRevenue = new List<bool>();
	private List<bool> listFilterMen = new List<bool>();
	private List<bool> listFilterWomen = new List<bool>();
	private List<bool> listFilterCoed = new List<bool>();
	private List<bool> listFilterTotal = new List<bool>();
	private List<List<float>> data;
	private List<List<float>> dataNorm = new List<List<float>>();
	private List<List<float>> data2015;
	private List<List<float>> dataNorm2015 = new List<List<float>>();
	private bool hideColor = true;

	// Use this for initialization
	void Start()
	{
		//Origin = new Vector3(spaceScale / 2.0f, spaceScale / 4.0f, spaceScale / 2.0f);
		colName = LoadEadaData.ReadHeader(inputfile, COL_NUM);
		data = LoadEadaData.ReadData(inputfile, COL_NUM);
		listDetails = LoadEadaData.ReadDetails(inputfile, 1);
		listClassification = LoadEadaData.ReadDetails(inputfile, 10);
		data2015 = LoadEadaData.ReadData(inputfile2015, COL_NUM);
		listDetails2015 = LoadEadaData.ReadDetails(inputfile2015, 1);
		listClassification2015 = LoadEadaData.ReadDetails(inputfile2015, 10 );
		listFilterParticipant = LoadEadaData.ReadFilter(mapfile, 7);
		listFilterStaff = LoadEadaData.ReadFilter(mapfile, 8);
		listFilterRevenue = LoadEadaData.ReadFilter(mapfile, 9);
		listFilterMen = LoadEadaData.ReadFilter(mapfile, 10);
		listFilterWomen = LoadEadaData.ReadFilter(mapfile, 11);
		listFilterCoed = LoadEadaData.ReadFilter(mapfile, 12);
		listFilterTotal = LoadEadaData.ReadFilter(mapfile, 13);

		for ( int i = 0; i < colName.Count; i++ )
		{
			listFilterAll.Add(true);
			listFilterNone.Add(false);
		}
		listFilter = listFilterAll;
		normalise();

		//GameObject dataPoint = Instantiate(
		//	PointPrefab,
		//	Origin, Quaternion.identity
		//);
		//dataPoint.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
		//dataPoint.transform.localScale = new Vector3(ptScale, ptScale, ptScale);
		OriginPt.transform.position = new Vector3(spaceScale / 2.0f, spaceScale / 4.0f, spaceScale / 2.0f);
		OldOrigin = new Vector3(spaceScale / 2.0f, spaceScale / 4.0f, spaceScale / 2.0f);

		loadAnchors();
		drawPoints();
		drawPoints(dataNorm2015, PointsHolder2015, PointPrefab2015, listDetails2015, listClassification2015);
		DrawYearLinks();
	}

	// Update is called once per frame
	void Update()
	{
		bool redraw = false;

		GameObject[] anchorPts = GameObject.FindGameObjectsWithTag("AnchorPoint");

		for ( int i = 0; i < anchorPts.Length; i++ )
		{
			if ( oldAnchors[i] != anchorPts[i].transform.position )
			{
				redraw = true;
				break;
			}
		}

		if ( OldOrigin != OriginPt.transform.position )
			redraw = true;

		if ( redraw )
		{
			for ( int i = 0; i < anchorPts.Length; i++ )
				oldAnchors[i] = anchorPts[i].transform.position;

			OldOrigin = OriginPt.transform.position;
			if ( PointsHolder.activeSelf )
				redrawPoints();
			if ( PointsHolder2015.activeSelf )
				redrawPoints(dataNorm2015, PointsHolder2015);
			DrawYearLinks();
		}
	}

	public void ToggleColor()
	{
		hideColor = !hideColor;
		if ( !hideColor )
		{
			Debug.Log("i am here - " + PointsHolder.transform.childCount);
			Debug.Log("i am here - " + PointsHolder2015.transform.childCount);
			for ( int i = 0; i < PointsHolder.transform.childCount; i++ )
			{
				if ( !PointsHolder.transform.GetChild(i).GetComponent<PointData>() )
					continue;
				PointsHolder.transform.GetChild(i).GetComponent<Renderer>().material.color = ListDataColor[PointsHolder.transform.GetChild(i).GetComponent<PointData>().classification];
			}
			for ( int i = 0; i < PointsHolder2015.transform.childCount; i++ )
			{
				if ( !PointsHolder2015.transform.GetChild(i).GetComponent<PointData>() )
					continue;
				PointsHolder2015.transform.GetChild(i).GetComponent<Renderer>().material.color = ListDataColor[PointsHolder2015.transform.GetChild(i).GetComponent<PointData>().classification];
			}
		}
		else
		{
			Debug.Log("i am there");
			for ( int i = 0; i < PointsHolder.transform.childCount; i++ )
			{
				if ( !PointsHolder.transform.GetChild(i).GetComponent<PointData>() )
					continue;
				PointsHolder.transform.GetChild(i).GetComponent<Renderer>().material.color = defaultColor;
			}
			for ( int i = 0; i < PointsHolder2015.transform.childCount; i++ )
			{
				if ( !PointsHolder2015.transform.GetChild(i).GetComponent<PointData>() )
					continue;
				PointsHolder2015.transform.GetChild(i).GetComponent<Renderer>().material.color = defaultColor2015;
			}
		}
	}

	/*
	public void DrawLinks(int index)
	{
		for ( int i = LinesHolder.transform.childCount - 1; i >= 0; i-- )
		{
			Destroy(LinesHolder.transform.GetChild(i).gameObject);
		}

		GameObject[] anchorPts = GameObject.FindGameObjectsWithTag("AnchorPoint");

		for ( int j = 0; j < dataNorm[index].Count; j++ )
		{
			if ( dataNorm[index][j] != 0 )
			{
				if ( !listFilter[j] )
					continue;

				GameObject link = Instantiate(
					LinePrefab,
					Vector3.zero,
					Quaternion.identity
				);
				link.GetComponent<LineRenderer>().SetPosition(0, anchorPts[j].transform.position);
				link.GetComponent<LineRenderer>().SetPosition(1, PointsHolder.transform.GetChild(index).transform.position);
				link.GetComponent<LineRenderer>().startWidth = dataNorm[index][j]* LINE_WIDTH;
				link.GetComponent<LineRenderer>().endWidth = dataNorm[index][j] * LINE_WIDTH;
				link.transform.parent = LinesHolder.transform;
				//anchorPts[j].transform.position
				//PointsHolder.transform.GetChild(index).transform.position;
			}
		}
	}
	*/

	public void DrawLinks(int index, List<List<float>> fData, GameObject parentHolder)
	{
		for ( int i = LinesHolder.transform.childCount - 1; i >= 0; i-- )
		{
			Destroy(LinesHolder.transform.GetChild(i).gameObject);
		}

		GameObject[] anchorPts = GameObject.FindGameObjectsWithTag("AnchorPoint");

		//List<List<float>> fData;

		//Debug.Log("index - " + index + " count - " + fData[index].Count);

		for ( int j = 0; j < fData[index].Count; j++ )
		{
			if ( fData[index][j] != 0 )
			{
				if ( !listFilter[j] )
					continue;

				GameObject link = Instantiate(
					LinePrefab,
					Vector3.zero,
					Quaternion.identity
				);
				link.GetComponent<LineRenderer>().SetPosition(0, anchorPts[j].transform.position);
				link.GetComponent<LineRenderer>().SetPosition(1, parentHolder.transform.GetChild(index).transform.position);
				link.GetComponent<LineRenderer>().startWidth = fData[index][j] * LINE_WIDTH;
				link.GetComponent<LineRenderer>().endWidth = fData[index][j] * LINE_WIDTH;
				link.transform.parent = LinesHolder.transform;
				//anchorPts[j].transform.position
				//PointsHolder.transform.GetChild(index).transform.position;
			}
		}
	}

	public void ClearLinks()
	{
		for ( int i = LinesHolder.transform.childCount - 1; i >= 0; i-- )
		{
			Destroy(LinesHolder.transform.GetChild(i).gameObject);
		}
	}

	public void DrawYearLinks()
	{
		for ( int i = YearLinksHolder.transform.childCount - 1; i >= 0; i-- )
		{
			Destroy(YearLinksHolder.transform.GetChild(i).gameObject);
		}

		if ( !PointsHolder.activeSelf || !PointsHolder2015.activeSelf )
		{
			return;
		}

		int lastJ = 0;

		for ( int i = 0; i < listDetails.Count; i++ )
		{
			for ( int j = lastJ; j < listDetails2015.Count; j++ )
			{
				if ( listDetails[i].Equals(listDetails2015[j]) )
				{
					//Debug.Log(listDetails[i] + " -- " + listDetails2015[j] + " *** " + i + " <-> " + j);
					GameObject link = Instantiate(
						YearLinePrefab,
						Vector3.zero,
						Quaternion.identity
					);
					link.GetComponent<LineRenderer>().SetPosition(0, PointsHolder.transform.GetChild(i).transform.position);
					link.GetComponent<LineRenderer>().SetPosition(1, PointsHolder2015.transform.GetChild(j).transform.position);
					link.transform.parent = YearLinksHolder.transform;
					lastJ = j;
					break;
				}
			}
		}
	}

	public void LoadFilter(string menuTag)
	{
		switch ( menuTag ) {
			case "all":
				listFilter = listFilterAll;
				break;
			case "none":
				listFilter = listFilterNone;
				break;
			case "participant":
				listFilter = listFilterParticipant;
				break;
			case "staff":
				listFilter = listFilterStaff;
				break;
			case "revenue":
				listFilter = listFilterRevenue;
				break;
			case "men":
				listFilter = listFilterMen;
				break;
			case "women":
				listFilter = listFilterWomen;
				break;
			case "coed":
				listFilter = listFilterCoed;
				break;
			case "total":
				listFilter = listFilterTotal;
				break;
			case "2016":
				PointsHolder.SetActive(true);
				PointsHolder2015.SetActive(false);
				break;
			case "2015":
				PointsHolder.SetActive(false);
				PointsHolder2015.SetActive(true);
				break;
			case "both":
				PointsHolder.SetActive(true);
				PointsHolder2015.SetActive(true);
				break;
			case "cd1":
				data.Clear();
				data2015.Clear();
				data = LoadEadaData.ReadData(inputfile, COL_NUM, 10, new List<string>() { "1" });
				listDetails = LoadEadaData.ReadDetails(inputfile, 1, 10, new List<string>() { "1" });
				listClassification = LoadEadaData.ReadDetails(inputfile, 10, 10, new List<string>() { "1" });
				data2015 = LoadEadaData.ReadData(inputfile2015, COL_NUM, 10, new List<string>() { "1" });
				listDetails2015 = LoadEadaData.ReadDetails(inputfile2015, 1, 10, new List<string>() { "1" });
				listClassification2015 = LoadEadaData.ReadDetails(inputfile2015, 10, 10, new List<string>() { "1" });
				normalise();
				drawPoints();
				drawPoints(dataNorm2015, PointsHolder2015, PointPrefab2015, listDetails2015, listClassification2015);
				break;
			case "cd2":
				data.Clear();
				data2015.Clear();
				data = LoadEadaData.ReadData(inputfile, COL_NUM, 10, new List<string>() { "2" });
				listDetails = LoadEadaData.ReadDetails(inputfile, 1, 10, new List<string>() { "2" });
				listClassification = LoadEadaData.ReadDetails(inputfile, 10, 10, new List<string>() { "2" });
				data2015 = LoadEadaData.ReadData(inputfile2015, COL_NUM, 10, new List<string>() { "2" });
				listDetails2015 = LoadEadaData.ReadDetails(inputfile2015, 1, 10, new List<string>() { "2" });
				listClassification2015 = LoadEadaData.ReadDetails(inputfile2015, 10, 10, new List<string>() { "2" });
				normalise();
				drawPoints();
				drawPoints(dataNorm2015, PointsHolder2015, PointPrefab2015, listDetails2015, listClassification2015);
				break;
			case "cd3":
				data.Clear();
				data2015.Clear();
				data = LoadEadaData.ReadData(inputfile, COL_NUM, 10, new List<string>() { "3" });
				listDetails = LoadEadaData.ReadDetails(inputfile, 1, 10, new List<string>() { "3" });
				listClassification = LoadEadaData.ReadDetails(inputfile, 10, 10, new List<string>() { "3" });
				data2015 = LoadEadaData.ReadData(inputfile2015, COL_NUM, 10, new List<string>() { "3" });
				listDetails2015 = LoadEadaData.ReadDetails(inputfile2015, 1, 10, new List<string>() { "3" });
				listClassification2015 = LoadEadaData.ReadDetails(inputfile2015, 10, 10, new List<string>() { "3" });
				normalise();
				drawPoints();
				drawPoints(dataNorm2015, PointsHolder2015, PointPrefab2015, listDetails2015, listClassification2015);
				break;
			case "cd4":
				data.Clear();
				data2015.Clear();
				data = LoadEadaData.ReadData(inputfile, COL_NUM, 10, new List<string>() { "4" });
				listDetails = LoadEadaData.ReadDetails(inputfile, 1, 10, new List<string>() { "4" });
				listClassification = LoadEadaData.ReadDetails(inputfile, 10, 10, new List<string>() { "4" });
				data2015 = LoadEadaData.ReadData(inputfile2015, COL_NUM, 10, new List<string>() { "4" });
				listDetails2015 = LoadEadaData.ReadDetails(inputfile2015, 1, 10, new List<string>() { "4" });
				listClassification2015 = LoadEadaData.ReadDetails(inputfile2015, 10, 10, new List<string>() { "4" });
				normalise();
				drawPoints();
				drawPoints(dataNorm2015, PointsHolder2015, PointPrefab2015, listDetails2015, listClassification2015);
				break;
			case "cd5":
				data.Clear();
				data2015.Clear();
				data = LoadEadaData.ReadData(inputfile, COL_NUM, 10, new List<string>() { "5" });
				listDetails = LoadEadaData.ReadDetails(inputfile, 1, 10, new List<string>() { "5" });
				listClassification = LoadEadaData.ReadDetails(inputfile, 10, 10, new List<string>() { "5" });
				data2015 = LoadEadaData.ReadData(inputfile2015, COL_NUM, 10, new List<string>() { "5" });
				listDetails2015 = LoadEadaData.ReadDetails(inputfile2015, 1, 10, new List<string>() { "5" });
				listClassification2015 = LoadEadaData.ReadDetails(inputfile2015, 10, 10, new List<string>() { "5" });
				normalise();
				drawPoints();
				drawPoints(dataNorm2015, PointsHolder2015, PointPrefab2015, listDetails2015, listClassification2015);
				break;
			case "cd6":
				data.Clear();
				data2015.Clear();
				data = LoadEadaData.ReadData(inputfile, COL_NUM);
				listDetails = LoadEadaData.ReadDetails(inputfile, 1);
				listClassification = LoadEadaData.ReadDetails(inputfile, 10);
				data2015 = LoadEadaData.ReadData(inputfile2015, COL_NUM);
				listDetails2015 = LoadEadaData.ReadDetails(inputfile2015, 1);
				listClassification2015 = LoadEadaData.ReadDetails(inputfile2015, 10);
				normalise();
				drawPoints();
				drawPoints(dataNorm2015, PointsHolder2015, PointPrefab2015, listDetails2015, listClassification2015);
				break;
			default:
				Debug.Log("should not be here");
				break;
		}
		reloadAnchors();
		if (PointsHolder.activeSelf)
			redrawPoints();
		if ( PointsHolder2015.activeSelf )
			redrawPoints(dataNorm2015, PointsHolder2015);
		DrawYearLinks();
	}

	public void ToggleFilter(int index)
	{
		//Debug.Log("index - " + index);
		listFilter[index] = !listFilter[index];
		reloadAnchors();
		if ( PointsHolder.activeSelf )
			redrawPoints();
		if ( PointsHolder2015.activeSelf )
			redrawPoints(dataNorm2015, PointsHolder2015);
		DrawYearLinks();
	}


	private void loadAnchors()
	{
		generateAnchors();
		Debug.Log(" col number - " + colName.Count + " -- anchor number -" + Anchors.Count + " -- filter number -" + listFilter.Count);
		for ( int i = 0; i < colName.Count; i++ )
		{
			GameObject dataPoint = Instantiate(
				AnchorPrefab,
				new Vector3(Anchors[i].x, Anchors[i].y, Anchors[i].z),
				Quaternion.identity
			);
			dataPoint.transform.parent = AnchorsHolder.transform;
			dataPoint.GetComponentInChildren<Text>().text = colName[i];
			Vector3 pos = new Vector3();
			pos = dataPoint.transform.position;
			oldAnchors.Add(pos);
			if ( !listFilter[i] )
			{
				Renderer[] Rs = dataPoint.GetComponentsInChildren<Renderer>();
				foreach ( Renderer r in Rs )
				{
					Color c = r.material.color;
					c.a = 0.1f;
					r.material.color = c;
				}
			}
		}
	}

	private void reloadAnchors()
	{
		GameObject[] anchorPts = GameObject.FindGameObjectsWithTag("AnchorPoint");

		for(int i =0; i<anchorPts.Length; i++ )
		{
			Color c =  anchorPts[i].GetComponentInChildren<Renderer>().material.color;
			if ( !listFilter[i] )
			{
				c.a = 0.1f;
			}
			else
			{
				c.a = 0.8f;
			}
			anchorPts[i].GetComponentInChildren<Renderer>().material.color = c;
		}
	}

	private void generateAnchors()
	{
		float x = 0;
		float y = 0;
		float z = 0;

		x = 0f;
		y = 0;
		for ( int row = 0; row < 6; row++ )
		{
			z = 0;
			for ( int col = 0; col < 9; col++ )
			{
				Vector3 v = new Vector3(x * spaceScale, y * spaceScale, z * spaceScale);
				Anchors.Add(v);
				z += 0.112f;
			}
			y += 0.112f;
		}

		z = 1;
		y = 0;
		for ( int row = 0; row < 6; row++ )
		{
			x = 0f;
			for ( int col = 0; col < 9; col++ )
			{
				Vector3 v = new Vector3(x * spaceScale, y * spaceScale, z * spaceScale);
				Anchors.Add(v);
				x += 0.112f;
			}
			y += 0.112f;
		}

		x = 1f;
		y = 0;
		for ( int row = 0; row < 6; row++ )
		{
			z = 0;
			for ( int col = 0; col < 10; col++ )
			{
				Vector3 v = new Vector3(x * spaceScale, y * spaceScale, z * spaceScale);
				Anchors.Add(v);
				z += 0.112f;
			}
			y += 0.112f;
		}
	}

	private void drawPoints()
	{
		clearPoints(PointsHolder);
		GameObject[] anchorPts = GameObject.FindGameObjectsWithTag("AnchorPoint");

		for ( int i = 0; i < dataNorm.Count; i++ )
		{
			Vector3[] v = new Vector3[anchorPts.Length];
			Vector3 pos = new Vector3(0, 0, 0);
			int nPos = 0;
			for ( int j = 0; j < dataNorm[i].Count; j++ )
			{
				if ( !listFilter[j] )
					continue;

				if ( dataNorm[i][j] != 0 )
				{
					v[j] = dataNorm[i][j] * ( anchorPts[j].transform.position - OriginPt.transform.position );
					pos += v[j];
					nPos++;
				}
			}
			if ( nPos > 0 )
			{
				pos /= nPos;
				pos *= explodeScale;
			}
			pos += OriginPt.transform.position;
			GameObject dataPoint = Instantiate(
				PointPrefab,
				new Vector3(pos.x, pos.y, pos.z),
				Quaternion.identity
			);
			dataPoint.transform.parent = PointsHolder.transform;
			dataPoint.GetComponent<Renderer>().material.color = defaultColor;
			dataPoint.transform.localScale = new Vector3(ptScale, ptScale, ptScale);
			dataPoint.GetComponentInChildren<PointData>().details = listDetails[i];
			dataPoint.GetComponentInChildren<PointData>().classification = listClassification[i];
			dataPoint.GetComponentInChildren<PointData>().index = i;
			dataPoint.GetComponentInChildren<PointData>().fData = dataNorm;
		}
	}

	private void drawPoints(List<List<float>> fData, GameObject holder, GameObject prefab, List<string> lDetails, List<string> lClassification)
	{
		clearPoints(holder);
		GameObject[] anchorPts = GameObject.FindGameObjectsWithTag("AnchorPoint");

		for ( int i = 0; i < fData.Count; i++ )
		{
			Vector3[] v = new Vector3[anchorPts.Length];
			Vector3 pos = new Vector3(0, 0, 0);
			int nPos = 0;
			for ( int j = 0; j < fData[i].Count; j++ )
			{
				if ( !listFilter[j] )
					continue;

				if ( fData[i][j] != 0 )
				{
					v[j] = fData[i][j] * ( anchorPts[j].transform.position - OriginPt.transform.position );
					pos += v[j];
					nPos++;
				}
			}
			if ( nPos > 0 )
			{
				pos /= nPos;
				pos *= explodeScale;
			}
			pos += OriginPt.transform.position;
			GameObject dataPoint = Instantiate(
				prefab,
				new Vector3(pos.x, pos.y, pos.z),
				Quaternion.identity
			);
			dataPoint.transform.parent = holder.transform;
			dataPoint.GetComponent<Renderer>().material.color = defaultColor;
			dataPoint.transform.localScale = new Vector3(ptScale, ptScale, ptScale);
			dataPoint.GetComponentInChildren<PointData>().details = lDetails[i];
			dataPoint.GetComponentInChildren<PointData>().classification = lClassification[i];
			dataPoint.GetComponentInChildren<PointData>().index = i;
			dataPoint.GetComponentInChildren<PointData>().fData = fData;
		}
	}

	private void redrawPoints()
	{
		GameObject[] anchorPts = GameObject.FindGameObjectsWithTag("AnchorPoint");

		for ( int i = 0; i < dataNorm.Count; i++ )
		{
			Vector3[] v = new Vector3[anchorPts.Length];
			Vector3 pos = new Vector3(0, 0, 0);
			int nPos = 0;
			for ( int j = 0; j < dataNorm[i].Count; j++ )
			{
				if ( !listFilter[j] )
					continue;

				if ( dataNorm[i][j] != 0 )
				{
					v[j] = dataNorm[i][j] * ( anchorPts[j].transform.position - OriginPt.transform.position );
					pos += v[j];
					nPos++;
				}
			}
			if ( nPos > 0 )
			{
				pos /= nPos;
				pos *= explodeScale;
			}
			pos += OriginPt.transform.position;
			PointsHolder.transform.GetChild(i).transform.position = pos;
		}
	}

	private void redrawPoints(List<List<float>> fData, GameObject holder)
	{
		GameObject[] anchorPts = GameObject.FindGameObjectsWithTag("AnchorPoint");

		for ( int i = 0; i < fData.Count; i++ )
		{
			Vector3[] v = new Vector3[anchorPts.Length];
			Vector3 pos = new Vector3(0, 0, 0);
			int nPos = 0;
			for ( int j = 0; j < fData[i].Count; j++ )
			{
				if ( !listFilter[j] )
					continue;

				if ( fData[i][j] != 0 )
				{
					v[j] = fData[i][j] * ( anchorPts[j].transform.position - OriginPt.transform.position );
					pos += v[j];
					nPos++;
				}
			}
			if ( nPos > 0 )
			{
				pos /= nPos;
				pos *= explodeScale;
			}
			pos += OriginPt.transform.position;
			holder.transform.GetChild(i).transform.position = pos;
		}
	}


	private void normalise()
	{
		List<float> max, min;
		List<float> max2015, min2015;
		max = getMax();
		min = getMin();
		max2015 = getMax(data2015);
		min2015 = getMin(data2015);
		dataNorm.Clear();
		dataNorm2015.Clear();

		for ( int i = 0; i < data.Count; i++ )
		{
			List<float> sublistData = new List<float>();
			for ( int j = 0; j < colName.Count; j++ )
			{
				float range = max[j] - min[j];
				if ( range != 0 )
				{
					float norm = ( data[i][j] - min[j] ) / ( max[j] - min[j] );
					sublistData.Add(norm);
				}
			}
			dataNorm.Add(sublistData);
		}

		for ( int i = 0; i < data2015.Count; i++ )
		{
			List<float> sublistData = new List<float>();
			for ( int j = 0; j < colName.Count; j++ )
			{
				float range = max2015[j] - min2015[j];
				if ( range != 0 )
				{
					float norm = ( data2015[i][j] - min2015[j] ) / ( max2015[j] - min2015[j] );
					sublistData.Add(norm);
				}
			}
			dataNorm2015.Add(sublistData);
		}


	}

	List<float> getMax()
	{
		List<float> max = new List<float>();

		for ( int i = 0; i < colName.Count; i++ )
		{
			float value = data[0][i];
			for ( int j = 1; j < data.Count; j++ )
			{
				if ( value < data[j][i] )
				{
					value = data[j][i];
				}
			}
			max.Add(value);
		}
		return max;
	}

	List<float> getMax(List<List<float>> fData)
	{
		List<float> max = new List<float>();

		for ( int i = 0; i < colName.Count; i++ )
		{
			float value = fData[0][i];
			for ( int j = 1; j < fData.Count; j++ )
			{
				if ( value < fData[j][i] )
				{
					value = fData[j][i];
				}
			}
			max.Add(value);
		}
		return max;
	}

	List<float> getMin()
	{
		List<float> min = new List<float>();

		for ( int i = 0; i < colName.Count; i++ )
		{
			float value = data[0][i];
			for ( int j = 1; j < data.Count; j++ )
			{
				if ( value > data[j][i] )
					value = data[j][i];
			}
			min.Add(value);
		}
		return min;
	}

	List<float> getMin(List<List<float>> fData)
	{
		List<float> min = new List<float>();

		for ( int i = 0; i < colName.Count; i++ )
		{
			float value = fData[0][i];
			for ( int j = 1; j < fData.Count; j++ )
			{
				if ( value > fData[j][i] )
					value = fData[j][i];
			}
			min.Add(value);
		}
		return min;
	}

	void clearPoints(GameObject holder)
	{
		for ( int i = holder.transform.childCount - 1; i >= 0; i-- )
		{
			Destroy(holder.transform.GetChild(i).gameObject);
		}
	}
	
}
