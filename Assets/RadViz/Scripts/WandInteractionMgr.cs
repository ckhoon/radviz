using Avpl;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WandInteractionMgr : MonoBehaviour {

	private static int DRAG_DELAY = 50;

	public Avpl.InputKey button_select;
	public Avpl.InputKey button_menu;
	public Avpl.InputKey key_hideColor;
	public Avpl.InputKey key_scaleUp;
	public Avpl.InputKey key_scaleDown;
	public GameObject plotter;
	public GameObject plotterEada;
	public GameObject MenuCanvas;
	public GameObject txtDetailCanvas;
	public GameObject vrWand;
	public Text txtDetails;


	private int btnPressed = 0;
	private bool dragged = false;
	private float mag;

	GameObject dragObject;
	GameObject hitObject;

	private UITarget[] allTargets;

	void Update () {
		if ( !dragged )
		{
			bool dragHit = false;
			if ( button_select.IsPressed() )
			{
				dragHit = selectBtnNoDrag();
			}
			else if ( button_menu.IsToggled() )
			{
				MenuCanvas.SetActive(true);
				MenuCanvas.transform.rotation = Avpl.AvplStatic.wandRay.transform.rotation;
				MenuCanvas.transform.position = Avpl.AvplStatic.GetRay().origin + Avpl.AvplStatic.GetRay().direction * 0.5f;
			}

			if ( key_hideColor.IsToggled() )
			{
				if ( plotter.activeSelf )
					plotter.GetComponent<RadVizPlotter>().ToggleColor();
				if ( plotterEada.activeSelf )
					plotterEada.GetComponent<EadaRadViz>().ToggleColor();
			}

			if ( !dragHit )
			{
				if ( hitObject != null )
				{
					if ( plotterEada.activeSelf )
						plotterEada.GetComponent<EadaRadViz>().ToggleFilter(hitObject.transform.parent.transform.GetSiblingIndex());
					hitObject = null;
					btnPressed = 0;
				}
			}
		}
		else
		{
			if ( button_select.IsToggled() )
			{
				Debug.Log("End drag " + hitObject.name);
				vrWand.GetComponent<VRInteractionNavigationWandJoystick>().enabled = true;
				hitObject.transform.parent.Find("Canvas Label").transform.position = Avpl.AvplStatic.GetRay().origin + Avpl.AvplStatic.GetRay().direction * mag + new Vector3(0,0.3f,0);
				hitObject = null;
				btnPressed = 0;
				dragged = false;
			}
			else
			{
				if ( MiddleVR.VRDeviceMgr.GetWandVerticalAxisValue() > 0.1 || MiddleVR.VRDeviceMgr.GetWandVerticalAxisValue() < -0.1 )
					mag += MiddleVR.VRDeviceMgr.GetWandVerticalAxisValue()/50.0f;
				hitObject.transform.parent.transform.position = Avpl.AvplStatic.GetRay().origin + Avpl.AvplStatic.GetRay().direction * mag;
				hitObject.transform.parent.transform.rotation = Avpl.AvplStatic.wandRay.transform.rotation;
				hitObject.transform.parent.Find("Canvas Label").transform.position = Avpl.AvplStatic.GetRay().origin + Avpl.AvplStatic.GetRay().direction * 0.5f;
			}
		}
	}

	private bool selectBtnNoDrag()
	{
		allTargets = MenuCanvas.GetComponentsInChildren<UITarget>();
		List<UITarget> RaycastHits = new List<UITarget>();
		//This loop performs the UI "raycast" using the ray given from GetSelectionRay();
		foreach ( UITarget target in allTargets )
		{
			//Skip objects that are inactive
			if ( !target.gameObject.activeInHierarchy )
				continue;

			Vector3 hitPos;
			if ( RayIntersectsRectTransform(target.RectTransform, Avpl.AvplStatic.GetRay(), out hitPos) )
			{
				RaycastHits.Add(target);
			}
		}

		RaycastHit hit;
		if ( Physics.Raycast(Avpl.AvplStatic.GetRay(), out hit) )
		{
			if ( hitObject == hit.collider.gameObject )
			{
				btnPressed++;
				if ( btnPressed > DRAG_DELAY )
				{
					dragged = true;
					mag = ( AvplStatic.wandRay.transform.position - hitObject.transform.position ).magnitude;
					vrWand.GetComponent<VRInteractionNavigationWandJoystick>().enabled = false;
					Debug.Log("Start drag " + hitObject.name);
				}
				return true;
			}
			else
			{
				if ( RaycastHits.Count != 0 )
				{
					RaycastHits = RaycastHits.OrderByDescending(x => x.Graphic.depth).ToList();
					if (
						( Avpl.AvplStatic.wandRay.transform.position.z - RaycastHits[0].transform.position.z ) >
						 ( Avpl.AvplStatic.wandRay.transform.position.z - hit.collider.gameObject.transform.position.z )
					)
					{
						if ( plotterEada.activeSelf )
							plotterEada.GetComponent<EadaRadViz>().LoadFilter(RaycastHits[0].MenuTag);
						//Debug.Log(RaycastHits[0].MenuTag);
						return false;
					}
				}

				if ( hit.collider.gameObject.tag == "AnchorPoint" || hit.collider.gameObject.tag == "OriginPoint" )
				{
					hitObject = hit.collider.gameObject;
					btnPressed = 0;
					return true;
				}
				else if ( hit.collider.gameObject.tag == "DataPoint" )
				{
					txtDetailCanvas.SetActive(true);
					txtDetailCanvas.transform.rotation = Avpl.AvplStatic.wandRay.transform.rotation;
					txtDetailCanvas.transform.position = Avpl.AvplStatic.GetRay().origin + Avpl.AvplStatic.GetRay().direction * 0.5f;
					txtDetails.text = hit.collider.gameObject.GetComponentInChildren<PointData>().details;
					if ( plotterEada.activeSelf )
					{
						//plotterEada.GetComponent<EadaRadViz>().DrawLinks(hit.collider.gameObject.GetComponentInChildren<PointData>().index);
						plotterEada.GetComponent<EadaRadViz>().DrawLinks(hit.collider.gameObject.GetComponentInChildren<PointData>().index, hit.collider.gameObject.GetComponentInChildren<PointData>().fData, hit.collider.gameObject.transform.parent.gameObject);
						plotterEada.GetComponent<EadaRadViz>().ToggleAlpha(hit.collider.gameObject.GetComponentInChildren<PointData>().details);
					}
					if ( plotter.activeSelf )
						plotter.GetComponent<RadVizPlotter>().DrawLinks(hit.collider.gameObject.GetComponentInChildren<PointData>().index);
				}
			}
		}
		else
		{
			if ( RaycastHits.Count != 0 )
			{
				RaycastHits = RaycastHits.OrderByDescending(x => x.Graphic.depth).ToList();
				//Debug.Log(RaycastHits[0].MenuTag);
				if ( plotterEada.activeSelf )
					plotterEada.GetComponent<EadaRadViz>().LoadFilter(RaycastHits[0].MenuTag);
				return false;
			}

			MenuCanvas.SetActive(false);
			txtDetailCanvas.SetActive(false);
			if ( plotterEada.activeSelf )
			{
				plotterEada.GetComponent<EadaRadViz>().ClearLinks();
				plotterEada.GetComponent<EadaRadViz>().ClearAlpha();
			}
			if ( plotter.activeSelf )
				plotter.GetComponent<RadVizPlotter>().ClearLinks();
		}

		return false;
	}

	public static bool RayIntersectsRectTransform(RectTransform rectTransform, Ray ray, out Vector3 worldPos)
	{
		Vector3[] corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);
		Plane plane = new Plane(corners[0], corners[1], corners[2]);

		float enter;
		if ( !plane.Raycast(ray, out enter) )
		{
			worldPos = Vector3.zero;
			return false;
		}

		Vector3 intersection = ray.GetPoint(enter);

		Vector3 BottomEdge = corners[3] - corners[0];
		Vector3 LeftEdge = corners[1] - corners[0];
		float BottomDot = Vector3.Dot(intersection - corners[0], BottomEdge);
		float LeftDot = Vector3.Dot(intersection - corners[0], LeftEdge);
		if ( BottomDot < BottomEdge.sqrMagnitude && // Can use sqrMag because BottomEdge is not normalized
			LeftDot < LeftEdge.sqrMagnitude &&
				BottomDot >= 0 &&
				LeftDot >= 0 )
		{
			worldPos = corners[0] + LeftDot * LeftEdge / LeftEdge.sqrMagnitude + BottomDot * BottomEdge / BottomEdge.sqrMagnitude;
			return true;
		}
		else
		{
			worldPos = Vector3.zero;
			return false;
		}
	}


}
