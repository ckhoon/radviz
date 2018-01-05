using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadVizPlotter : MonoBehaviour {

	public GameObject PointPrefab;
	public GameObject AnchorPrefab;
	public GameObject LinePrefab;
	public GameObject PointsHolder;
	public GameObject AnchorsHolder;
	public GameObject LinesHolder;
	public GameObject OriginPt;
	public float ptScale;

	private static int spaceScale = 2;

	private Dictionary<string, Vector3> ListAnchors;

	private Vector3[] Anchors = new[] {
		new Vector3(0f,1f,0f) * spaceScale,
		new Vector3(0f,0f,1f) * spaceScale,
		new Vector3(1f,1f,1f) * spaceScale,
		new Vector3(1f,0f,0f) * spaceScale
	};

	private Vector3[] oldAnchors = new[] {
		new Vector3(),
		new Vector3(),
		new Vector3(),
		new Vector3()
	};

	private Vector3 OldOrigin = new Vector3(spaceScale/2, spaceScale/2, spaceScale/2);

	private string[] colName = new string[] {
		"Sepal Length",
		"Sepal Width",
		"Petal Length",
		"Petal Width"
	};

	private Dictionary<string, Color> ListDataColor = new Dictionary<string, Color>{
		{ "setosa", new Color(1, 0.1f, 0.1f) },
		{ "versicolor", new Color(0.1f, 1f, 0.1f) },
		{ "virginica", new Color(0.1f, 0.1f, 1f) },
	};

	private string[] classification = new string[] {
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"setosa",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"versicolor",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica",
		"virginica"
	};

	private float[][] data = new float[][] {
		new float[]{0.222222222f,0.625f,0.06779661f,0.041666667f},
		new float[]{0.166666667f,0.416666667f,0.06779661f,0.041666667f},
		new float[]{0.111111111f,0.5f,0.050847458f,0.041666667f},
		new float[]{0.083333333f,0.458333333f,0.084745763f,0.041666667f},
		new float[]{0.194444444f,0.666666667f,0.06779661f,0.041666667f},
		new float[]{0.305555556f,0.791666667f,0.118644068f,0.125f},
		new float[]{0.083333333f,0.583333333f,0.06779661f,0.083333333f},
		new float[]{0.194444444f,0.583333333f,0.084745763f,0.041666667f},
		new float[]{0.027777778f,0.375f,0.06779661f,0.041666667f},
		new float[]{0.166666667f,0.458333333f,0.084745763f,0f},
		new float[]{0.305555556f,0.708333333f,0.084745763f,0.041666667f},
		new float[]{0.138888889f,0.583333333f,0.101694915f,0.041666667f},
		new float[]{0.138888889f,0.416666667f,0.06779661f,0f},
		new float[]{0f,0.416666667f,0.016949153f,0f},
		new float[]{0.416666667f,0.833333333f,0.033898305f,0.041666667f},
		new float[]{0.388888889f,1f,0.084745763f,0.125f},
		new float[]{0.305555556f,0.791666667f,0.050847458f,0.125f},
		new float[]{0.222222222f,0.625f,0.06779661f,0.083333333f},
		new float[]{0.388888889f,0.75f,0.118644068f,0.083333333f},
		new float[]{0.222222222f,0.75f,0.084745763f,0.083333333f},
		new float[]{0.305555556f,0.583333333f,0.118644068f,0.041666667f},
		new float[]{0.222222222f,0.708333333f,0.084745763f,0.125f},
		new float[]{0.083333333f,0.666666667f,0f,0.041666667f},
		new float[]{0.222222222f,0.541666667f,0.118644068f,0.166666667f},
		new float[]{0.138888889f,0.583333333f,0.152542373f,0.041666667f},
		new float[]{0.194444444f,0.416666667f,0.101694915f,0.041666667f},
		new float[]{0.194444444f,0.583333333f,0.101694915f,0.125f},
		new float[]{0.25f,0.625f,0.084745763f,0.041666667f},
		new float[]{0.25f,0.583333333f,0.06779661f,0.041666667f},
		new float[]{0.111111111f,0.5f,0.101694915f,0.041666667f},
		new float[]{0.138888889f,0.458333333f,0.101694915f,0.041666667f},
		new float[]{0.305555556f,0.583333333f,0.084745763f,0.125f},
		new float[]{0.25f,0.875f,0.084745763f,0f},
		new float[]{0.333333333f,0.916666667f,0.06779661f,0.041666667f},
		new float[]{0.166666667f,0.458333333f,0.084745763f,0.041666667f},
		new float[]{0.194444444f,0.5f,0.033898305f,0.041666667f},
		new float[]{0.333333333f,0.625f,0.050847458f,0.041666667f},
		new float[]{0.166666667f,0.666666667f,0.06779661f,0f},
		new float[]{0.027777778f,0.416666667f,0.050847458f,0.041666667f},
		new float[]{0.222222222f,0.583333333f,0.084745763f,0.041666667f},
		new float[]{0.194444444f,0.625f,0.050847458f,0.083333333f},
		new float[]{0.055555556f,0.125f,0.050847458f,0.083333333f},
		new float[]{0.027777778f,0.5f,0.050847458f,0.041666667f},
		new float[]{0.194444444f,0.625f,0.101694915f,0.208333333f},
		new float[]{0.222222222f,0.75f,0.152542373f,0.125f},
		new float[]{0.138888889f,0.416666667f,0.06779661f,0.083333333f},
		new float[]{0.222222222f,0.75f,0.101694915f,0.041666667f},
		new float[]{0.083333333f,0.5f,0.06779661f,0.041666667f},
		new float[]{0.277777778f,0.708333333f,0.084745763f,0.041666667f},
		new float[]{0.194444444f,0.541666667f,0.06779661f,0.041666667f},
		new float[]{0.75f,0.5f,0.627118644f,0.541666667f},
		new float[]{0.583333333f,0.5f,0.593220339f,0.583333333f},
		new float[]{0.722222222f,0.458333333f,0.661016949f,0.583333333f},
		new float[]{0.333333333f,0.125f,0.508474576f,0.5f},
		new float[]{0.611111111f,0.333333333f,0.610169492f,0.583333333f},
		new float[]{0.388888889f,0.333333333f,0.593220339f,0.5f},
		new float[]{0.555555556f,0.541666667f,0.627118644f,0.625f},
		new float[]{0.166666667f,0.166666667f,0.389830508f,0.375f},
		new float[]{0.638888889f,0.375f,0.610169492f,0.5f},
		new float[]{0.25f,0.291666667f,0.491525424f,0.541666667f},
		new float[]{0.194444444f,0f,0.423728814f,0.375f},
		new float[]{0.444444444f,0.416666667f,0.542372881f,0.583333333f},
		new float[]{0.472222222f,0.083333333f,0.508474576f,0.375f},
		new float[]{0.5f,0.375f,0.627118644f,0.541666667f},
		new float[]{0.361111111f,0.375f,0.440677966f,0.5f},
		new float[]{0.666666667f,0.458333333f,0.576271186f,0.541666667f},
		new float[]{0.361111111f,0.416666667f,0.593220339f,0.583333333f},
		new float[]{0.416666667f,0.291666667f,0.525423729f,0.375f},
		new float[]{0.527777778f,0.083333333f,0.593220339f,0.583333333f},
		new float[]{0.361111111f,0.208333333f,0.491525424f,0.416666667f},
		new float[]{0.444444444f,0.5f,0.644067797f,0.708333333f},
		new float[]{0.5f,0.333333333f,0.508474576f,0.5f},
		new float[]{0.555555556f,0.208333333f,0.661016949f,0.583333333f},
		new float[]{0.5f,0.333333333f,0.627118644f,0.458333333f},
		new float[]{0.583333333f,0.375f,0.559322034f,0.5f},
		new float[]{0.638888889f,0.416666667f,0.576271186f,0.541666667f},
		new float[]{0.694444444f,0.333333333f,0.644067797f,0.541666667f},
		new float[]{0.666666667f,0.416666667f,0.677966102f,0.666666667f},
		new float[]{0.472222222f,0.375f,0.593220339f,0.583333333f},
		new float[]{0.388888889f,0.25f,0.423728814f,0.375f},
		new float[]{0.333333333f,0.166666667f,0.474576271f,0.416666667f},
		new float[]{0.333333333f,0.166666667f,0.457627119f,0.375f},
		new float[]{0.416666667f,0.291666667f,0.491525424f,0.458333333f},
		new float[]{0.472222222f,0.291666667f,0.694915254f,0.625f},
		new float[]{0.305555556f,0.416666667f,0.593220339f,0.583333333f},
		new float[]{0.472222222f,0.583333333f,0.593220339f,0.625f},
		new float[]{0.666666667f,0.458333333f,0.627118644f,0.583333333f},
		new float[]{0.555555556f,0.125f,0.576271186f,0.5f},
		new float[]{0.361111111f,0.416666667f,0.525423729f,0.5f},
		new float[]{0.333333333f,0.208333333f,0.508474576f,0.5f},
		new float[]{0.333333333f,0.25f,0.576271186f,0.458333333f},
		new float[]{0.5f,0.416666667f,0.610169492f,0.541666667f},
		new float[]{0.416666667f,0.25f,0.508474576f,0.458333333f},
		new float[]{0.194444444f,0.125f,0.389830508f,0.375f},
		new float[]{0.361111111f,0.291666667f,0.542372881f,0.5f},
		new float[]{0.388888889f,0.416666667f,0.542372881f,0.458333333f},
		new float[]{0.388888889f,0.375f,0.542372881f,0.5f},
		new float[]{0.527777778f,0.375f,0.559322034f,0.5f},
		new float[]{0.222222222f,0.208333333f,0.338983051f,0.416666667f},
		new float[]{0.388888889f,0.333333333f,0.525423729f,0.5f},
		new float[]{0.555555556f,0.541666667f,0.847457627f,1f},
		new float[]{0.416666667f,0.291666667f,0.694915254f,0.75f},
		new float[]{0.777777778f,0.416666667f,0.830508475f,0.833333333f},
		new float[]{0.555555556f,0.375f,0.779661017f,0.708333333f},
		new float[]{0.611111111f,0.416666667f,0.813559322f,0.875f},
		new float[]{0.916666667f,0.416666667f,0.949152542f,0.833333333f},
		new float[]{0.166666667f,0.208333333f,0.593220339f,0.666666667f},
		new float[]{0.833333333f,0.375f,0.898305085f,0.708333333f},
		new float[]{0.666666667f,0.208333333f,0.813559322f,0.708333333f},
		new float[]{0.805555556f,0.666666667f,0.86440678f,1f},
		new float[]{0.611111111f,0.5f,0.694915254f,0.791666667f},
		new float[]{0.583333333f,0.291666667f,0.728813559f,0.75f},
		new float[]{0.694444444f,0.416666667f,0.762711864f,0.833333333f},
		new float[]{0.388888889f,0.208333333f,0.677966102f,0.791666667f},
		new float[]{0.416666667f,0.333333333f,0.694915254f,0.958333333f},
		new float[]{0.583333333f,0.5f,0.728813559f,0.916666667f},
		new float[]{0.611111111f,0.416666667f,0.762711864f,0.708333333f},
		new float[]{0.944444444f,0.75f,0.966101695f,0.875f},
		new float[]{0.944444444f,0.25f,1f,0.916666667f},
		new float[]{0.472222222f,0.083333333f,0.677966102f,0.583333333f},
		new float[]{0.722222222f,0.5f,0.796610169f,0.916666667f},
		new float[]{0.361111111f,0.333333333f,0.661016949f,0.791666667f},
		new float[]{0.944444444f,0.333333333f,0.966101695f,0.791666667f},
		new float[]{0.555555556f,0.291666667f,0.661016949f,0.708333333f},
		new float[]{0.666666667f,0.541666667f,0.796610169f,0.833333333f},
		new float[]{0.805555556f,0.5f,0.847457627f,0.708333333f},
		new float[]{0.527777778f,0.333333333f,0.644067797f,0.708333333f},
		new float[]{0.5f,0.416666667f,0.661016949f,0.708333333f},
		new float[]{0.583333333f,0.333333333f,0.779661017f,0.833333333f},
		new float[]{0.805555556f,0.416666667f,0.813559322f,0.625f},
		new float[]{0.861111111f,0.333333333f,0.86440678f,0.75f},
		new float[]{1f,0.75f,0.915254237f,0.791666667f},
		new float[]{0.583333333f,0.333333333f,0.779661017f,0.875f},
		new float[]{0.555555556f,0.333333333f,0.694915254f,0.583333333f},
		new float[]{0.5f,0.25f,0.779661017f,0.541666667f},
		new float[]{0.944444444f,0.416666667f,0.86440678f,0.916666667f},
		new float[]{0.555555556f,0.583333333f,0.779661017f,0.958333333f},
		new float[]{0.583333333f,0.458333333f,0.762711864f,0.708333333f},
		new float[]{0.472222222f,0.416666667f,0.644067797f,0.708333333f},
		new float[]{0.722222222f,0.458333333f,0.745762712f,0.833333333f},
		new float[]{0.666666667f,0.458333333f,0.779661017f,0.958333333f},
		new float[]{0.722222222f,0.458333333f,0.694915254f,0.916666667f},
		new float[]{0.416666667f,0.291666667f,0.694915254f,0.75f},
		new float[]{0.694444444f,0.5f,0.830508475f,0.916666667f},
		new float[]{0.666666667f,0.541666667f,0.796610169f,1f},
		new float[]{0.666666667f,0.416666667f,0.711864407f,0.916666667f},
		new float[]{0.555555556f,0.208333333f,0.677966102f,0.75f},
		new float[]{0.611111111f,0.416666667f,0.711864407f,0.791666667f},
		new float[]{0.527777778f,0.583333333f,0.745762712f,0.916666667f},
		new float[]{0.444444444f,0.416666667f,0.694915254f,0.708333333f},
	};

	private float LINE_WIDTH = 0.05f;
	private Color defaultColor = new Color(0.8f, 0.2f, 0.2f, 0.8f);
	private bool hideColor = true;

	// Use this for initialization
	void Start () {

		//GameObject dataPoint = Instantiate(
		//	PointPrefab,
		//	Origin, Quaternion.identity
		//);
		//dataPoint.GetComponent<Renderer>().material.color = new Color(0, 0, 0);
		
		OriginPt.transform.position = new Vector3(spaceScale / 2, spaceScale / 2, spaceScale / 2);

		loadAnchors();
		drawPoints();
	}

	// Update is called once per frame
	void Update()
	{
		bool redraw = false;

		GameObject[] anchorPts =  GameObject.FindGameObjectsWithTag("AnchorPoint");

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
			//clearPoints();
			redrawPoints();
		}
	}

	public void ToggleColor()
	{
		hideColor = !hideColor;
		if ( !hideColor )
		{
			//Debug.Log("i am here - " + PointsHolder.transform.childCount);
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
			//Debug.Log("i am there");
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

		for ( int j = 0; j < data[index].Length; j++ )
		{
			if ( data[index][j] != 0 )
			{
				GameObject link = Instantiate(
					LinePrefab,
					Vector3.zero,
					Quaternion.identity
				);
				link.GetComponent<LineRenderer>().SetPosition(0, anchorPts[j].transform.position);
				link.GetComponent<LineRenderer>().SetPosition(1, PointsHolder.transform.GetChild(index).transform.position);
				link.GetComponent<LineRenderer>().startWidth = data[index][j] * LINE_WIDTH;
				link.GetComponent<LineRenderer>().endWidth = data[index][j] * LINE_WIDTH;
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

	private void loadAnchors() {
		for ( int i = 0; i < colName.Length; i++ )
		{
			GameObject dataPoint = Instantiate(
				AnchorPrefab,
				new Vector3(Anchors[i].x, Anchors[i].y, Anchors[i].z),
				Quaternion.identity
			);
			dataPoint.transform.parent = AnchorsHolder.transform;
			dataPoint.GetComponentInChildren<Text>().text = colName[i];
			//oldAnchors[i] = Anchors[i];
		}
	}

	private void drawPoints()
	{
		GameObject[] anchorPts = GameObject.FindGameObjectsWithTag("AnchorPoint");

		for ( int i = 0; i < data.Length; i++ )
		{
			Vector3[] v = new Vector3[anchorPts.Length];
			Vector3 pos = new Vector3(0,0,0);
			for ( int j = 0; j < anchorPts.Length; j++ )
			{
				v[j] = data[i][j] * ( anchorPts[j].transform.position - OriginPt.transform.position );
				pos += v[j];
			}
			pos /= anchorPts.Length;
			pos += OriginPt.transform.position;
			GameObject dataPoint = Instantiate(
				PointPrefab,
				new Vector3(pos.x, pos.y, pos.z),
				Quaternion.identity
			);
			dataPoint.transform.parent = PointsHolder.transform;
			dataPoint.GetComponent<Renderer>().material.color = defaultColor;
			dataPoint.transform.localScale = new Vector3(ptScale, ptScale, ptScale);
			dataPoint.GetComponentInChildren<PointData>().classification = classification[i];
			dataPoint.GetComponentInChildren<PointData>().details = classification[i];
			dataPoint.GetComponentInChildren<PointData>().index = i;
		}
	}

	private void redrawPoints()
	{
		GameObject[] anchorPts = GameObject.FindGameObjectsWithTag("AnchorPoint");

		for ( int i = 0; i < data.Length; i++ )
		{
			Vector3[] v = new Vector3[anchorPts.Length];
			Vector3 pos = new Vector3(0, 0, 0);
			for ( int j = 0; j < anchorPts.Length; j++ )
			{
				v[j] = data[i][j] * ( anchorPts[j].transform.position - OriginPt.transform.position );
				pos += v[j];
			}
			pos /= anchorPts.Length;
			pos += OriginPt.transform.position;

			PointsHolder.transform.GetChild(i).transform.position = pos;
		}
	}

	void clearPoints()
	{
		for ( int i = PointsHolder.transform.childCount - 1; i >= 0; i-- )
		{
			Destroy(PointsHolder.transform.GetChild(i).gameObject);
		}
	}
}
