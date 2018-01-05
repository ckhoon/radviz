using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointData : MonoBehaviour {
	public string classification
	{
		get; set;
	}
	public string details
	{
		get; set;
	}
	public int index
	{
		get; set;
	}
	public List<List<float>> fData
	{
		get; set;
	}
}