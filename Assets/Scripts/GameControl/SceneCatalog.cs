﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneCatalog", menuName = "SceneCatalog")]
public class SceneCatalog : ScriptableObject {

	private static SceneCatalog myInstance;
	public static SceneCatalog sceneCatalog { get {
			if (myInstance == null) {
				myInstance = Resources.Load("SceneCatalog") as SceneCatalog;
			}
			return myInstance;
		}
	}

	public List<SceneData> scenes = new List<SceneData>();

	private Dictionary<string, SceneData> mySceneMap = null;

	private Dictionary<string, SceneData> sceneMap {
		get {
			if (mySceneMap == null) {
				mySceneMap = new Dictionary<string, SceneData>();
				foreach (SceneData data in scenes) {
					mySceneMap.Add(data.path, data);
				}
			}
			return mySceneMap;
		}
	}

	public void addScene(string path) {
		if (!sceneMap.ContainsKey(path)) {
			SceneData data = new SceneData(path, GlobalConfigData.getDefault());
			scenes.Add(data);
			sceneMap.Add(path, data);
		} else {
			if (GlobalConfig.globalConfig != null) {
				sceneMap[path] = new SceneData(path, GlobalConfig.globalConfig.configuration);
			}
		}
	}

	public SceneData? getSceneData(string path) {
		if (sceneMap.ContainsKey(path))
			return sceneMap[path];
		return null;
	}
}
