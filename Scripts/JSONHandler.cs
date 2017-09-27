/* 
 JSONHandler.cs

adapted by NovaVR for the CMS Collaboration CMS.VR project
http://novavrllc.com

original script igJSONHandler.cs by Thomas McCauley
thomas.mccauley@cern.ch

*/


using UnityEngine;
using System;
using JsonFx.Json;
using System.Collections.Generic;

public class JSONHandler {
	
	private string eventJSON;

	private Dictionary<string,object> eventObjects; // These include the Types, Collections, Associations

	private Dictionary<string,object> typeObjects; 
	private Dictionary<string,object> collectionObjects;
	private Dictionary<string,object> associationObjects;
	
	// This is not used right now but contains the information of the 
	// types: the first string is the collection name, the second string
	// is the name of the variable in the collection, and the int
	// is the index of the property in the collection
	private Dictionary<string, Dictionary<string, int> > Types;
	
	public JSONHandler(string jsonString) {
		eventJSON = jsonString;
		processJSON();
	}
	
	private void outputKeys(Dictionary<string,object>.KeyCollection kc) {
		foreach(string k in kc) {
			//Debug.Log("key: " + k);
		}
	}
		
	
	private void processJSON() {
		
		// Fill the dictionaries for Types, 
		// Collections, and Associations
		
		JsonReader reader = new JsonReader(eventJSON);
		
		eventObjects = (Dictionary<string,object>) reader.Deserialize();
		//Debug.Log ("Here are the Event keys:");
		outputKeys(eventObjects.Keys);
		
		Types = new Dictionary<string, Dictionary<string,int> >();
		
		if ( eventObjects.ContainsKey("Types") ) {
			
			typeObjects = (Dictionary<string,object>) eventObjects["Types"];
			//outputKeys(typeObjects.Keys);
		}
		else {
			Debug.Log("No Types in event");
		}
		
		if ( eventObjects.ContainsKey("Collections") ) {
		
			collectionObjects = (Dictionary<string,object>) eventObjects["Collections"];
			//Debug.Log ("Here are the Collection keys:");
			//outputKeys(collectionObjects.Keys);
		}
		else {
			Debug.Log("No Collections in event");
		}
		
		if ( eventObjects.ContainsKey("Associations") ) {
			
			associationObjects = (Dictionary<string,object>) eventObjects["Associations"];
			//Debug.Log ("Here are the Association keys:");
			//outputKeys(associationObjects.Keys);
		}
		
		else {
			Debug.Log("No Associations in event");
		}
	}
	
	public bool HasType(string type_key) {
		return typeObjects.ContainsKey(type_key);
	}
	
	public bool HasCollection(string collection_key) {
		return collectionObjects.ContainsKey(collection_key);	
	}
	
	public bool HasAssociation(string association_key) {
		return associationObjects.ContainsKey(association_key);
	}
	
	public void ProcessType(string type_key) {
		
		// Here we fill the Types dict for use when we want to 
		// fetch collection properties
		
		if ( HasType(type_key) ) {
			
			Debug.Log("Has " + type_key);
			String[][] type = (String[][]) typeObjects[type_key];
			Dictionary<string,int> type_info = new Dictionary<string,int>();
			
			Debug.Log(type_key);
			
			for ( int i = 0; i < type.Length; i++ ) {
				type_info.Add(type[i][0],i);
				
				Debug.Log(type[i][0] + "  " + i);
			}
			
			Types.Add(type_key, type_info);
		}
		
		else {
			Debug.Log("No Type of name: " + type_key + " in Types");
		}
	}
	
	public object[][] GetCollection(string collection_key) {
		if (HasCollection(collection_key)) {
			Type t = collectionObjects [collection_key].GetType ();
			if (t != typeof(System.Object[])) { // should not be a single array
				object[][] collection = (object[][]) collectionObjects[collection_key];
				return collection;
			} else
				return null;
		}
		else {
			Debug.Log("No Collection of name: " + collection_key + " in Collections");
			return null;
		}
	}

	public object[] GetCollectionSingleArray(string collection_key) {
		if (HasCollection(collection_key)) {
			object[] collection = (object[]) collectionObjects[collection_key];
			if (collection.Length > 0) { // should not be empty array
				return collection;
			} else
				return null;
		}
		else {
			Debug.Log("No Collection of name: " + collection_key + " in Collections");
			return null;
		}
	}
	
	public object[] GetAssociation(string association_key) {
		if ( HasAssociation(association_key) ) {
			object[] association = (object[]) associationObjects[association_key];
			return association;
		}
		else {
			Debug.Log("No Association of name: " + association_key + " in Associations");
			return null;
		}
	}
}



