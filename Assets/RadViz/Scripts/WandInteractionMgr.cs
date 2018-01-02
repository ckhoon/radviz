using Avpl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WandInteractionMgr : MonoBehaviour {

	private static int DRAG_DELAY = 50;

	public Avpl.InputKey button_select;
	public Avpl.InputKey key_hideColor;
	public GameObject plotter;
	public GameObject plotterEada;
	public GameObject txtDetailCanvas;
	public GameObject vrWand;
	public Text txtDetails;


	private int btnPressed = 0;
	private bool dragged = false;
	private float mag;

	GameObject dragObject;
	GameObject hitObject;

	// Update is called once per frame
	void Update () {
		if ( !dragged )
		{
			if ( button_select.IsPressed() )
			{
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
							//vrWand.GetComponent<VRInteractionNavigationWandJoystick>().gameObject.SetActive(false);
							//Debug.Log("active " + vrWand.GetComponent<VRInteractionNavigationWandJoystick>().gameObject.activeSelf);
							Debug.Log("Start drag " + hitObject.name);
						}
						return;
					}
					else
					{
						//Debug.Log(hit.collider.gameObject.tag);
						if ( hit.collider.gameObject.tag == "AnchorPoint" )
						{
							hitObject = hit.collider.gameObject;
							btnPressed = 0;
							return;
						}
						else if ( hit.collider.gameObject.tag == "DataPoint" )
						{
							txtDetailCanvas.transform.rotation = Avpl.AvplStatic.wandRay.transform.rotation;
							txtDetailCanvas.transform.position = Avpl.AvplStatic.GetRay().origin + Avpl.AvplStatic.GetRay().direction * 0.5f;
							txtDetails.text = hit.collider.gameObject.GetComponentInChildren<PointData>().details;
							if ( plotterEada.activeSelf )
								plotterEada.GetComponent<EadaRadViz>().DrawLinks(hit.collider.gameObject.GetComponentInChildren<PointData>().index);
							if ( plotter.activeSelf )
								plotter.GetComponent<RadVizPlotter>().DrawLinks(hit.collider.gameObject.GetComponentInChildren<PointData>().index);

						}
					}
				}
				else
				{
					if ( plotterEada.activeSelf )
						plotterEada.GetComponent<EadaRadViz>().ClearLinks();
					if ( plotter.activeSelf )
						plotter.GetComponent<RadVizPlotter>().ClearLinks();
				}
			}
			hitObject = null;
			btnPressed = 0;
		}
		else
		{
			if ( button_select.IsToggled() )
			{
				Debug.Log("End drag " + hitObject.name);
				vrWand.GetComponent<VRInteractionNavigationWandJoystick>().enabled = true;
				//vrWand.GetComponent<VRInteractionNavigationWandJoystick>().gameObject.SetActive(true);
				//Debug.Log("active " + vrWand.GetComponent<VRInteractionNavigationWandJoystick>().gameObject.activeSelf);
				hitObject.transform.parent.Find("Canvas Label").transform.position = Avpl.AvplStatic.GetRay().origin + Avpl.AvplStatic.GetRay().direction * mag;
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
				//Debug.Log(hitObject.transform.parent.Find("Canvas Label"));
				//hitObject.transform.position = Avpl.AvplStatic.GetRay().origin + Avpl.AvplStatic.GetRay().direction * mag;
			}
		}

		if ( key_hideColor.IsToggled() )
		{
			if(plotter.activeSelf)
				plotter.GetComponent<RadVizPlotter>().ToggleColor();
			if( plotterEada.activeSelf )
				plotterEada.GetComponent<EadaRadViz>().ToggleColor();
		}
	}
}
