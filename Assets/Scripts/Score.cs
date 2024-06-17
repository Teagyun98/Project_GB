using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviourPunCallbacks
{
	public int Level { get; private set; }
	private MeshRenderer meshRenderer;
	[SerializeField] private List<Material> matList;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
	}

    public override void OnEnable()
    {
		base.OnEnable();

        if(GameManager.instance.poolScoreObjectList.Contains(this) == true)
			GameManager.instance.poolScoreObjectList.Remove(this);
    }

    private void Start()
	{
		if(photonView.IsMine)
			photonView.RPC("SetLevel", RpcTarget.AllBuffered, Level);
	}

    public override void OnDisable()
    {
        base.OnDisable();

		if(GameManager.instance.poolScoreObjectList.Contains(this) == false)
			GameManager.instance.poolScoreObjectList.Add(this);
    }

    [PunRPC]
	public void SetLevel(int level)
	{
		Level = level;
		transform.localScale = Vector3.one * Level;
		meshRenderer.material = matList[Level - 1];
	}

	public void Destroy()
	{
		PhotonNetwork.Destroy(gameObject);
	}
}
