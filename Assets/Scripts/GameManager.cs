using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
	public static GameManager instance;

	public List<Score> poolScoreObjectList;
	public List<PlayerController> poolPlayerObjectList;

	[SerializeField] private GameObject restartPanel;

	private void Start()
	{
		instance = this;

		poolScoreObjectList = new List<Score>();
		poolPlayerObjectList = new List<PlayerController>();
	}

	public void StartGame()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			StartCoroutine(SpawnScoreObjectCo());
		}
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
			StartCoroutine(SpawnScoreObjectCo());
	}

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

	private void SpawnScoreObject()
	{
		Vector3 spawnPosition = new Vector3(Random.Range(-45f, 45f), 0f, Random.Range(-45f, 45f));

		Score scoreObject;

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

	public void SpawnPlayerObject()
	{
        // 플레이어 스폰
        Vector3 spawnPosition = new Vector3(Random.Range(-45f, 45f), 0f, Random.Range(-45f, 45f));

        PlayerController player;

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

	public void GameOver()
	{
		if(restartPanel != null)
			restartPanel.gameObject.SetActive(true);
	}
}
