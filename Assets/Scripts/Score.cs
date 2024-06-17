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
		
		// 활성화 될 때 풀링 리스트에 있다면 삭제
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

		// 비활성화 될 때 풀링 리스트에 없으면 추가
		if(GameManager.instance.poolScoreObjectList.Contains(this) == false)
			GameManager.instance.poolScoreObjectList.Add(this);
    }

	// Score Object가 생성 될 때 레벨에 맞는 Material과 Scale 설정 함수
    [PunRPC]
	public void SetLevel(int level)
	{
		Level = level;
		transform.localScale = Vector3.one * Level;
		meshRenderer.material = matList[Level - 1];
	}
}
