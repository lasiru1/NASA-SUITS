﻿using System;
using Assets.Scripts;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class NavigationService : MonoBehaviour {

    public static List<Node> nodeMap = new List<Node>();
    public static List<Node> route = new List<Node>();

    private static NavigationService _Instance;
  
    public static NavigationService Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<NavigationService>();
            }
            return _Instance;
        }
    }
  
	Vector3 navOrigin;
	Quaternion navRotation;

	public bool calibrated = false;
    public string modelName;

    public delegate void MapLoadedEvent();
    public event MapLoadedEvent MapLoaded;

    void Awake() {
        _Instance = this;
    }

	// Use this for initialization
	void Start () {
        LoadFromJson("/NodeMaps/ISS.json");

        route = GetRoute(GetNodeByID(1), GetNodeByID(7));

        LogRoute(route);
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void SetOrigin(Vector3 origin, Quaternion rotation) {
		navOrigin = origin;
		navRotation = rotation;
		calibrated = true;
		Debug.Log("Nav Origin Set");
	}

    public List<Node> GetRoute(Node source, Node destination)
    {
        Pathfinder pathFinder = new Pathfinder(nodeMap, source, destination);
        pathFinder.Execute();

        return pathFinder.GetShortestPath();
    }

    public Node GetNodeByID(int id)
    {
        return nodeMap.Find(x => x.id.Equals(id));
    }

    void LogNodes()
    {
        foreach (Node node in nodeMap)
        {
            Debug.Log(node.id + " (" + node.position.x + ", " + node.position.y + ", " + node.position.z + ")");
        }
    }

    public void LogRoute(List<Node> route)
    {
        string output = "Route: ";

        for (int i = 0; i < route.Count; i++)
        {
            output += route[i].id;

            if (i < route.Count - 1) output += " -> ";
        }

        Debug.Log(output);
    }

    void LoadFromJson(string directory)
    {
        string filePath = Application.streamingAssetsPath + directory;

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            NodeMap jsonNodeMap = JsonUtility.FromJson<NodeMap>(jsonContent);

            modelName = jsonNodeMap.modelname;
            nodeMap = ConvertedJsonNodeMap(jsonNodeMap);

            if (MapLoaded != null) MapLoaded();
        }
        else
        {
            Debug.LogError("Cannot load data from " + filePath);
        }
    }

    public List<Node> ConvertedJsonNodeMap(NodeMap jsonNodeMap)
    {
        List<Node> nodeMap = new List<Node>();

        foreach (JsonNode jsonNode in jsonNodeMap.nodes)
        {
            Node node = new Node();

            node.id = jsonNode.id;
            node.position = new Vector3(jsonNode.position.x, jsonNode.position.y, jsonNode.position.z);
            node.adjacentNodeIDs = jsonNode.adjacentNodeIDs;

            nodeMap.Add(node);
        }

        return nodeMap;
    }
    
    public Node getNearestNode(Vector3 userPosition)
    {
        float minDist = float.MaxValue;
        Node nearestNode = new Node();
        foreach(Node node in nodeMap)
        {
            float dist = Mathf.Sqrt(Mathf.Pow(node.position.x - userPosition.x, 2) +
                                    Mathf.Pow(node.position.y - userPosition.y, 2) +
                                    Mathf.Pow(node.position.z - userPosition.z, 2));
            if(dist < minDist)
            {
                minDist = dist;
                nearestNode = node;
            }
        }
        return nearestNode;
    }

    [Serializable]
    public class NodeMap
    {
        public string title;
        public string modelname;
        public List<JsonNode> nodes;
    }

    [Serializable]
    public class JsonNode
    {
        public int id;
        public Positition position;
        public List<int> adjacentNodeIDs;

    }

    [Serializable]
    public class Positition
    {
        public float x;
        public float y;
        public float z;
    }
}
