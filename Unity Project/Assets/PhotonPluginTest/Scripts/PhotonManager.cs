using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonManager : Photon.PunBehaviour
{
    public static PhotonManager instance;

    //로컬 사용자
    public static GameObject localPlayer;
    // 캐릭터 진입 지점
    GameObject defaultSpawnPoint;

    void Awake()
    {
        if(instance != null)
        {
            DestroyImmediate( gameObject );
            return;
        }

        DontDestroyOnLoad( gameObject );
        instance = this;

        //PUN의 맵 로딩 자동 동기화를 사용할 수 있고, 맵 로딩 초기화 시 발생할 수 있는 네트워크 문제를 피할 수 있습니다.
        PhotonNetwork.automaticallySyncScene = true;
    }

    void Start()
    {
        Connect();
        // InvokeRepeating("UpdateStatus", 2, 1);
    }

    void Connect()
    {
        //Photon Cloud/Server와의 연결 시작 시 게임 버전은 문자열 모드로 설정
        PhotonNetwork.ConnectUsingSettings( "PUN_PhotonPlugin_1.0" );
    }

    void UpdateStatus()
    {
        string status = PhotonNetwork.connectionStateDetailed.ToString();
        Debug.Log( status );
    }

    public void JoinGameRoom()
    {
        //게임룸과 명칭 설정 시 사용자 인원수 최대 2명
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;

        int IdNum = Random.Range( 1, 9999 );
        string userId = "Player" + IdNum.ToString() ;
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = userId;
        PhotonNetwork.playerName = userId;
        Debug.LogFormat("My Name : {0}", userId);

        //        options.Plugins = ;
        PhotonNetwork.JoinOrCreateRoom( "RoomPlugins", options, TypedLobby.Default );
    }

    public override void OnConnectedToMaster()
    {
        //마스터 서버에 연결 후, 버튼(UI)으로 기타 후속 동작을 표시하거나 실행할 수 있습니다.
        Debug.Log( "Master Server에 접속하였습니다" );
    }

    public override void OnJoinedRoom()
    {
        Debug.Log( "룸에 진입했습니다" );

        //마스터 클라이언트일 경우, 즉시 구축 및 초기화하고 게임 화면 로딩 가능
        if(PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel( "GameRoomScene" );
        }
    }

    //게임 화면 로딩
    void OnLevelWasLoaded( int levelNumber )
    {
        if(!PhotonNetwork.inRoom)
            return;

        Debug.Log( "게임 화면 로딩/게임룸내" );
    }

    public override void OnPhotonPlayerConnected( PhotonPlayer newPlayer )
    {
        Debug.Log( "Other Player Entered." );
    }
}