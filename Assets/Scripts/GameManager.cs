using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
	public static GameManager instance;

	[SerializeField] private Score scoreObject;

	private void Start()
	{
		instance = this;
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
		Vector3 spawnPosition = new Vector3(Random.Range(-50f, 50f), 0f, Random.Range(-50f, 50f));
		GameObject obj = PhotonNetwork.Instantiate(scoreObject.name, spawnPosition, Quaternion.identity);
		obj.GetComponent<Score>().SetLevel(Random.Range(1, 7));
	}
}
