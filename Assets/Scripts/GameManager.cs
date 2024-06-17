using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
	// 다른 객체가 GameManager에 접근하기 쉽도록 전역 변수 선언
	public static GameManager instance;

	// Score와 Player 오브젝트를 풀링할 때 사용하기 위한 비활성화된 오브젝트를 담을 수 있는 리스트 선언
	public List<Score> poolScoreObjectList;
	public List<PlayerController> poolPlayerObjectList;

	// 재시작 패널
	[SerializeField] private GameObject restartPanel;
	// 플레이어 이동시 중심이 되는 오브젝트
	public GameObject controller;

	// 전역변수 instance와 리스트 초기화
	private void Start()
	{
		instance = this;

		poolScoreObjectList = new List<Score>();
		poolPlayerObjectList = new List<PlayerController>();
	}

	// 게임 시작시 MasterClient일 경우 10개의 Score Object를 생성하고 주기적으로 Score Object를 생성하는 코루틴 실행
	public void StartGame()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			for (int i = 0; i < 10; i++)
				SpawnScoreObject();

			StartCoroutine(SpawnScoreObjectCo());
		}
	}

	// MasterClient 플레이어가 게임을 나가면 다른 플레이어가 MasterClient가 되고 Score Object를 생성하는 코르틴을 실행
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
			StartCoroutine(SpawnScoreObjectCo());
	}

	// 5초 마다 최대 20개가 될 때 까지 Score Object를 생성하는 코루틴
	private IEnumerator SpawnScoreObjectCo()
	{
		while(true)
		{
			yield return new WaitForSeconds(5f);

			if (PhotonNetwork.IsMasterClient)
				if (GameObject.FindGameObjectsWithTag("Score").Length < 20)
				{
					SpawnScoreObject();
				}
		}
	}

	// 특정 범위 내에 Score Object를 생성하는 함수
	private void SpawnScoreObject()
	{
		Vector3 spawnPosition = new Vector3(Random.Range(-45f, 45f), 0f, Random.Range(-45f, 45f));

		Score scoreObject;
		
		// 오브젝트 풀링
		if (poolScoreObjectList.Count > 0)
		{
			scoreObject = poolScoreObjectList[0];
			scoreObject.transform.position = spawnPosition;
			scoreObject.gameObject.SetActive(true);
		}
		else
		{
			GameObject newScoreObject = PhotonNetwork.Instantiate("ScoreObject", spawnPosition, Quaternion.identity);
            scoreObject = newScoreObject.GetComponent<Score>();
        }

        scoreObject.SetLevel(Random.Range(1, 7));
	}

	// 특정 범위 내에 플레이어를 생성하는 함수
	public void SpawnPlayerObject()
	{
        // 플레이어 스폰
        Vector3 spawnPosition = new Vector3(Random.Range(-45f, 45f), 0f, Random.Range(-45f, 45f));

        PlayerController player;

		// 오브젝트 풀링
        if (poolPlayerObjectList.Count > 0)
        {
            player = poolPlayerObjectList[0];
            player.transform.position = spawnPosition;
			player.ActiveOrder(true);
        }
        else
        {
            PhotonNetwork.Instantiate("PlayerPrefab", spawnPosition, Quaternion.identity, 0);
        }
    }

	// 게임오버시 재시작 패널을 활성화 해주는 함수
	public void GameOver()
	{
		if(restartPanel != null)
			restartPanel.gameObject.SetActive(true);
	}
}
