using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
	// photon 서버 연결 상태를 알려주고 게임을 시작할 수 있게 해주는 패널
	[SerializeField] private GameObject panel;
	[SerializeField] private TextMeshProUGUI log;
	[SerializeField] private Button startBtn;

	private void Start()
	{
		// 포톤 서버에 연결
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		log.text = "Connected to Master";
		// 로비에 연결
		PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		log.text = "Joined Lobby";
		// 방에 접속 혹은 방 생성
		PhotonNetwork.JoinOrCreateRoom("Room1", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
	}

	public override void OnJoinedRoom()
	{
		log.text = "Joined Room";
		startBtn.gameObject.SetActive(true);
	}

	// 플레이어를 생성하고 게임시작 함수를 실행시킨다.
	public void OnClick()
	{
		GameManager.instance.SpawnPlayerObject();
        GameManager.instance.StartGame();
		panel.SetActive(false);
	}
}
