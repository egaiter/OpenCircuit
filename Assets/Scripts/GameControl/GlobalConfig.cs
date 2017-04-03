﻿using UnityEngine;
using UnityEngine.Networking;

public class GlobalConfig : NetworkBehaviour {

    public GameObject playerPrefab;

	[SyncVar]
	public GlobalConfigData configuration = GlobalConfigData.getDefault();
	public CentralRobotController centralRobotController;
    public int frozenPlayers = 0;

    [SyncVar]
    public bool gameStarted;

	public GameMode gamemode;

    void Start() {
        myGlobalConfig = this;
        globalConfig.configuration = Menu.menu.serverConfig;

        gameStarted = true;
        gamemode = GameMode.constructGameMode(gameObject, configuration.gameMode);
    }

    private static GlobalConfig myGlobalConfig;
    public static GlobalConfig globalConfig {
        get {
            return myGlobalConfig;
        }
    }

    [Server]
    public void winGame() {
        RpcWinGame();
    }

    [Server]
    public void loseGame() {
        NetworkController.networkController.serverClearPlayers();
        RpcLoseGame();
    }

    [ClientRpc]
    private void RpcLoseGame() {
        Menu.menu.lose();
    }

    [ClientRpc]
    private void RpcWinGame() {
        Menu.menu.win();
    }

	[Server]
	public int getMaxRobots() {
		//TODO: I imagine this will have to change to support spectators -Brian
		return NetworkServer.connections.Count * configuration.robotsPerPlayer;
	}

	[Server]
	public float getDelay() {
		return 1f/(NetworkServer.connections.Count * configuration.spawnRateIncreasePerPlayer + configuration.robotSpawnRatePerSecond); 
	}

	[Server]
	public CentralRobotController getCRC() {
		return centralRobotController;
	}

    [Server]
    public void spawnPlayerForConnection(NetworkConnection connection) {
        Transform startPos = NetworkManager.singleton.GetStartPosition();
        NetworkController.networkController.serverAddPlayer(playerPrefab, startPos.position, startPos.rotation,
            connection);
    }
}

[System.Serializable]
public struct GlobalConfigData {
	public int robotsPerPlayer;
    public float robotSpawnRatePerSecond;
	public float spawnRateIncreasePerPlayer;
    public GameMode.GameModes gameMode;

	public static GlobalConfigData getDefault() {
		GlobalConfigData data = new GlobalConfigData();
		data.robotsPerPlayer = 3;
		data.robotSpawnRatePerSecond = 1f;
		data.spawnRateIncreasePerPlayer = 0.1f;
	    data.gameMode = GameMode.GameModes.BASES;
		return data;
	}
}