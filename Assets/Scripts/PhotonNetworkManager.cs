using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotonNetworkManager : MonoBehaviourPunCallbacks
{
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

	public void OnClick()
	{
		// 플레이어 스폰
		Vector3 spawnPosition = new Vector3(Random.Range(-50f, 50f), 0f, Random.Range(-50f, 50f));
		GameObject player = PhotonNetwork.Instantiate("PlayerPrefab", spawnPosition, Quaternion.identity, 0);
		GameManager.instance.StartGame();
		panel.SetActive(false);
	}
}
