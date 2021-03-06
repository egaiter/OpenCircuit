﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GlobalConfig : NetworkBehaviour {

	public bool localPlayerDead;

    public GameObject playerPrefab;

	[SyncVar]
	public GlobalConfigData configuration = GlobalConfigData.getDefault();

    [SyncVar]
    public bool gameStarted;

	public GameMode gamemode;
	public CameraManager cameraManager;

    public int robotControllers;

    void Start() {
        myGlobalConfig = this;
        configuration = Menu.menu.serverConfig;
        gamemode = getGameMode(configuration.gameMode);
        gamemode.initialize();
        gamemode.enabled = true;

        gameStarted = true;
    }

    private static GlobalConfig myGlobalConfig;
    public static GlobalConfig globalConfig {
        get {
            return myGlobalConfig;
        }
    }

    [Server]
    public void winGame() {
	    //NetworkController.networkController.serverClearPlayers();
        RpcWinGame();
    }

    [Server]
    public void loseGame() {
        //NetworkController.networkController.serverClearPlayers();
        RpcLoseGame();
    }

    [ClientRpc]
    private void RpcLoseGame() {
		clearLocalPlayers();
	    Menu.menu.lose();
    }

    [ClientRpc]
    private void RpcWinGame() {
	    clearLocalPlayers();
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
    public void spawnPlayerForConnection(NetworkConnection connection) {
        Transform startPos = NetworkManager.singleton.GetStartPosition();
        NetworkController.networkController.serverAddPlayer(playerPrefab, startPos.position, startPos.rotation,
            connection);
    }

	private void clearLocalPlayers() {
		List<short> playerIds = new List<short>();
		foreach (var player in ClientScene.localPlayers) {
			playerIds.Add(player.playerControllerId);
		}
		foreach (short Id in playerIds) {
			ClientScene.RemovePlayer(Id);
		}

		cameraManager.switchCamera();
	}

    private GameMode getGameMode(GameMode.GameModes gameType) {
        //TODO: Do this better
        GameMode mode = null;
        switch (gameType) {
            case GameMode.GameModes.BASES:
                mode = GetComponent<Bases>();
                break;
            case GameMode.GameModes.SPAWNER_HUNT:
                mode = GetComponent<SpawnerHunt>();
                break;
        }
        return mode;
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