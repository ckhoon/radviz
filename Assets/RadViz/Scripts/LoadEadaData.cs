using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Taken from here: https://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/
// Comments
// Code parses a CSV, converting values into ints or floats if able, and returning a List<Dictionary<string, object>>.

public class LoadEadaData
{
	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))"; // Define delimiters, regular expression craziness
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r"; // Define line delimiters, regular experession craziness
	static char[] TRIM_CHARS = { '\"' };

	private static int PERCENT = 100;
	private static float PERCENT_REC = 100 / PERCENT;

	public static List<string> ReadHeader(string file, int colStart = 0)
	{
		List<string> header = new List<string>();
		TextAsset data = Resources.Load(file) as TextAsset; //Loads the TextAsset named in the file argument of the function
		var lines = Regex.Split(data.text, LINE_SPLIT_RE); // Split data.text into lines using LINE_SPLIT_RE characters
		string[] values = Regex.Split(lines[0], SPLIT_RE); //Split header (element 0)
		for ( var j = colStart; j < values.Length; j++ )
		{
			header.Add(values[j]);
		}

		return header;
	}

	public static List<List<float>> ReadData(string file, int colStart=0)
	{
		List<List<float>> listData = new List<List<float>>();
		TextAsset txtData = Resources.Load(file) as TextAsset; //Loads the TextAsset named in the file argument of the function
		var lines = Regex.Split(txtData.text, LINE_SPLIT_RE); // Split data.text into lines using LINE_SPLIT_RE characters
		for ( var i = 1; i < lines.Length; i += (int)PERCENT_REC )
		{
			var values = Regex.Split(lines[i], SPLIT_RE); //Split lines according to SPLIT_RE, store in var (usually string array)
			if ( values.Length == 0 || values[0] == "" )
				continue; // Skip to end of loop (continue) if value is 0 length OR first value is empty

			// Loops through every value
			List<float> sublistData = new List<float>();
			for ( var j = colStart; j < values.Length; j++ )
			{
				string value = values[j]; // Set local variable value
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", ""); // Trim characters
				float f = new float(); // Create float, to hold value if float
				if ( !float.TryParse(value, out f) )
				{
					Debug.Log("error reading data file, receive -> " + value);
					return null;
				}
				sublistData.Add(f);
			}
			listData.Add(sublistData);
		}
		return listData; //Return list
	}

	public static List<List<float>> ReadData(string file, int colStart = 0, int colSel = 0, List<string> strSel = null)
	{
		List<List<float>> listData = new List<List<float>>();
		TextAsset txtData = Resources.Load(file) as TextAsset; //Loads the TextAsset named in the file argument of the function
		var lines = Regex.Split(txtData.text, LINE_SPLIT_RE); // Split data.text into lines using LINE_SPLIT_RE characters
		for ( var i = 1; i < lines.Length; i += (int)PERCENT_REC )
		{
			var values = Regex.Split(lines[i], SPLIT_RE); //Split lines according to SPLIT_RE, store in var (usually string array)
			if ( values.Length == 0 || values[0] == "" )
				continue; // Skip to end of loop (continue) if value is 0 length OR first value is empty

			if ( strSel.Contains(values[colSel]) )
			{
				// Loops through every value
				List<float> sublistData = new List<float>();
				for ( var j = colStart; j < values.Length; j++ )
				{
					string value = values[j]; // Set local variable value
					value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", ""); // Trim characters
					float f = new float(); // Create float, to hold value if float
					if ( !float.TryParse(value, out f) )
					{
						Debug.Log("error reading data file, receive -> " + value);
						return null;
					}
					sublistData.Add(f);
				}
				listData.Add(sublistData);
			}

		}
		return listData; //Return list
	}

	public static List<string> ReadDetails(string file, int colStart = 0)
	{
		List<string> listData = new List<string>();
		TextAsset txtData = Resources.Load(file) as TextAsset; //Loads the TextAsset named in the file argument of the function
		var lines = Regex.Split(txtData.text, LINE_SPLIT_RE); // Split data.text into lines using LINE_SPLIT_RE characters
		for ( var i = 1; i < lines.Length; i += (int)PERCENT_REC )
		{
			var values = Regex.Split(lines[i], SPLIT_RE); //Split lines according to SPLIT_RE, store in var (usually string array)
			if ( values.Length == 0 || values[0] == "" )
				continue; // Skip to end of loop (continue) if value is 0 length OR first value is empty

			listData.Add(values[colStart]);
		}
		return listData; //Return list
	}

	public static List<string> ReadDetails(string file, int colStart = 0, int colSel = 0, List<string> strSel = null)
	{
		List<string> listData = new List<string>();
		TextAsset txtData = Resources.Load(file) as TextAsset; //Loads the TextAsset named in the file argument of the function
		var lines = Regex.Split(txtData.text, LINE_SPLIT_RE); // Split data.text into lines using LINE_SPLIT_RE characters
		for ( var i = 1; i < lines.Length; i += (int)PERCENT_REC )
		{
			var values = Regex.Split(lines[i], SPLIT_RE); //Split lines according to SPLIT_RE, store in var (usually string array)
			if ( values.Length == 0 || values[0] == "" )
				continue; // Skip to end of loop (continue) if value is 0 length OR first value is empty

			if ( strSel.Contains(values[colSel]) )
				listData.Add(values[colStart]);
		}
		return listData; //Return list
	}


	public static List<bool> ReadFilter(string file, int colStart = 0)
	{
		List<bool> listFilter = new List<bool>();
		TextAsset txtData = Resources.Load(file) as TextAsset; //Loads the TextAsset named in the file argument of the function
		var lines = Regex.Split(txtData.text, LINE_SPLIT_RE); // Split data.text into lines using LINE_SPLIT_RE characters
		for ( var i = 12; i < lines.Length; i ++ )
		{
			var values = Regex.Split(lines[i], SPLIT_RE); //Split lines according to SPLIT_RE, store in var (usually string array)
			if ( values.Length == 0 || values[0] == "" )
				continue; // Skip to end of loop (continue) if value is 0 length OR first value is empty
			bool b = new bool();
			b = !values[colStart].Equals("");
			listFilter.Add(b);
		}
		return listFilter;
	}

	public static List<Dictionary<string, object>> Read(string file) //Declare method
	{
		//Debug.Log("CSVReader is reading " + file); // Print filename, make sure parsed correctly

		var list = new List<Dictionary<string, object>>(); //declare dictionary list

		TextAsset data = Resources.Load(file) as TextAsset; //Loads the TextAsset named in the file argument of the function

		// Debug.Log("Data loaded:" + data); // Print raw data, make sure parsed correctly

		var lines = Regex.Split(data.text, LINE_SPLIT_RE); // Split data.text into lines using LINE_SPLIT_RE characters

		if ( lines.Length <= 1 )
			return list; //Check that there is more than one line

		var header = Regex.Split(lines[0], SPLIT_RE); //Split header (element 0)

		// Loops through lines
		for ( var i = 1; i < lines.Length; i += (int)PERCENT_REC )
		{
			var values = Regex.Split(lines[i], SPLIT_RE); //Split lines according to SPLIT_RE, store in var (usually string array)
			if ( values.Length == 0 || values[0] == "" )
				continue; // Skip to end of loop (continue) if value is 0 length OR first value is empty

			var entry = new Dictionary<string, object>(); // Creates dictionary object

			// Loops through every value
			for ( var j = 0; j < header.Length && j < values.Length; j++ )
			{
				string value = values[j]; // Set local variable value
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", ""); // Trim characters
				object finalvalue = value; //set final value

				int n; // Create int, to hold value if int

				float f; // Create float, to hold value if float

				// If-else to attempt to parse value into int or float
				if ( int.TryParse(value, out n) )
				{
					finalvalue = n;
				}
				else if ( float.TryParse(value, out f) )
				{
					finalvalue = f;
				}
				entry[header[j]] = finalvalue;
			}
			list.Add(entry); // Add Dictionary ("entry" variable) to list
		}
		return list; //Return list
	}
}