using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EadaRadViz : MonoBehaviour {

	public GameObject PointPrefab;
	public GameObject AnchorPrefab;
	public GameObject LinePrefab;
	public GameObject PointsHolder;
	public GameObject AnchorsHolder;
	public GameObject LinesHolder;
	public string inputfile;
	public string mapfile;
	public float ptScale;
	public int explodeScale = 5;
	public int spaceScale = 5;


	private List<Vector3> Anchors = new List<Vector3>();
	private List<Vector3> oldAnchors = new List<Vector3>();

	private Dictionary<string, Color> ListDataColor = new Dictionary<string, Color>{
		{ "1", new Color(1, 0.1f, 0.1f) },
		{ "2", new Color(0.1f, 1f, 0.1f) },
		{ "4", new Color(0.1f, 0.1f, 1f) },
		{ "3", new Color(0.9f, 0.1f, 0.5f) },
		{ "5", new Color(0.5f, 0.9f, 0.1f) },
		{ "6", new Color(0.1f, 0.5f, 0.9f) }
	};

	private static Color defaultColor = new Color(0.8f, 0.2f, 0.2f, 0.8f);

	private Vector3 Origin;
	private static int COL_NUM = 12;
	private static float LINE_WIDTH = 0.08f;

	private List<string> colName;
	private List<string> listDetails;
	private List<string> listClassification;
	private List<bool> listFilter;
	private List<List<float>> data;
	private List<List<float>> dataNorm = new List<List<float>>();
	private bool hideColor = true;

	// Use this for initialization
	void Start()
	{
		Origin = new Vector3(spaceScale / 2.0f, spaceScale / 2.0f, spaceScale / 2.0f);
		colName = LoadEadaData.ReadHeader(inputfile, COL_NUM);
		data = LoadEadaData.ReadData(inputfile, COL_NUM);
		listDetails = LoadEadaData.ReadDetails(inputfile, 1);
		listClassification = LoadEadaData.ReadDetails(inputfile, 10);
		listFilter = LoadEadaData.ReadFilter(mapfile, 7);
		normalise();

		GameObject dataPoint = Instantiate(
			PointPrefab,
			Origin, Quaternion.identity
		);
		dataPoint.GetComponent<Renderer>().material.color = new Color(0, 0, 0);

		loadAnchors();
		drawPoints();
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

		if ( redraw )
		{
			for ( int i = 0; i < anchorPts.Length; i++ )
				oldAnchors[i] = anchorPts[i].transform.position;

			redrawPoints();
		}
	}

	public void ToggleColor()
	{
		hideColor = !hideColor;
		if ( !hideColor )
		{
			Debug.Log("i am here - " + PointsHolder.transform.childCount);
			for ( int i = 0; i < PointsHolder.transform.childCount; i++ )
			{
				if ( !PointsHolder.transform.GetChild(i).GetComponent<PointData>() )
					continue;
				//Debug.Log("string is " + PointsHolder.transform.GetChild(i).GetComponent<PointData>().classification);
				//Debug.Log("color is " + ListDataColor[PointsHolder.transform.GetChild(i).GetComponent<PointData>().classification]);
				PointsHolder.transform.GetChild(i).GetComponent<Renderer>().material.color = ListDataColor[PointsHolder.transform.GetChild(i).GetComponent<PointData>().classification];
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
		}
	}

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

	public void ClearLinks()
	{
		for ( int i = LinesHolder.transform.childCount - 1; i >= 0; i-- )
		{
			Destroy(LinesHolder.transform.GetChild(i).gameObject);
		}
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

	private void generateAnchors()
	{
		float x = 0;
		float y = 0;
		float z = 0;

		x = 0f;
		y = 0;
		for ( int row = 0; row < 7; row++ )
		{
			z = 0;
			for ( int col = 0; col < 8; col++ )
			{
				Vector3 v = new Vector3(x * spaceScale, y * spaceScale, z * spaceScale);
				Anchors.Add(v);
				z += 0.125f;
			}
			y += 0.167f;
		}

		z = 1;
		y = 0;
		for ( int row = 0; row < 7; row++ )
		{
			x = 0f;
			for ( int col = 0; col < 8; col++ )
			{
				Vector3 v = new Vector3(x * spaceScale, y * spaceScale, z * spaceScale);
				Anchors.Add(v);
				x += 0.125f;
			}
			y += 0.167f;
		}

		x = 1f;
		y = 0;
		for ( int row = 0; row < 7; row++ )
		{
			z = 0;
			for ( int col = 0; col < 9; col++ )
			{
				Vector3 v = new Vector3(x * spaceScale, y * spaceScale, z * spaceScale);
				Anchors.Add(v);
				z += 0.125f;
			}
			y += 0.167f;
		}
	}

	
	private void drawPoints()
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
					v[j] = dataNorm[i][j] * ( anchorPts[j].transform.position - Origin );
					pos += v[j];
					nPos++;
				}
			}
			pos /= nPos;
			pos *= explodeScale;
			pos += Origin;
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
					v[j] = dataNorm[i][j] * ( anchorPts[j].transform.position - Origin );
					pos += v[j];
					nPos++;
				}
			}
			pos /= nPos;
			pos *= explodeScale;
			pos += Origin;
			PointsHolder.transform.GetChild(i).transform.position = pos;
		}
	}



	private void normalise()
	{
		Debug.Log("start norm");
		List<float> max, min;
		Debug.Log("get max");
		max = getMax();
		Debug.Log("get min");
		min = getMin();

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
		Debug.Log("end norm");
		Debug.Log("norm count -" + dataNorm.Count);
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
		//Debug.Log("max count -" + max.Count);
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
		//Debug.Log("min count -" + min.Count);
		return min;
	}
	
	void clearPoints()
	{
		for ( int i = PointsHolder.transform.childCount - 1; i >= 0; i-- )
		{
			Destroy(PointsHolder.transform.GetChild(i).gameObject);
		}
	}
	
}
