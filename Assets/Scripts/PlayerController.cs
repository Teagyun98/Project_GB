using Photon.Pun;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public float speed = 5f;
    public float MyScale { get; private set; } = 1f;
    private Vector2 touchStartPos;
    private Vector2 touchCurrentPos;
    private bool isTouching = false;
    private bool enterCollision = false;

    public override void OnEnable()
    {
        base.OnEnable();

        // 오브젝트가 활성화 되었을 때 풀링 목록에 자신이 있을 경우 삭제
        if(GameManager.instance.poolPlayerObjectList.Contains(this) == true)
            GameManager.instance.poolPlayerObjectList.Remove(this);

        if (photonView.IsMine)
        {
            // 카메라의 타겟 설정
            CameraFollow.target = transform;
            // 서버 안의 자신의 크기 값 설정
            photonView.RPC("SetScale", RpcTarget.AllBuffered, MyScale);
        }
    }

	private void Update()
    {
        if (photonView.IsMine)
        {
            // 플레이어 이동
            HandleTouchInput();
            MovePlayer();
        }
    }

	private void OnCollisionEnter(Collision collision)
	{
        // Score Object와 충돌 했을 경우 Score Object를 비활성화 하고 가지고 있는 Level값의 10% 만큼 플레이어가 커진다.
        if (collision.transform.CompareTag("Score") && enterCollision == false)
        {
            enterCollision = true;

            Score score = collision.transform.GetComponent<Score>();

            float newScale = transform.localScale.x + ((float)score.Level / 10);

            score.ActiveOrder(false);
            photonView.RPC("SetScale", RpcTarget.AllBuffered, newScale);

            StartCoroutine(ResetEnterCollision());
        }
        // 플레이어와 충돌했을 경우 더 작은 플레이어가 비활성화 되며 비활성화된 플레이어의 크기의 10% 만큼 큰 플레이어의 크기가 커진다.
        else if (collision.transform.CompareTag("Player") && enterCollision == false && collision.transform.localScale.x < transform.localScale.x)
        {
            enterCollision = true;

            float newScale = transform.localScale.x + (collision.transform.localScale.x / 10);
            collision.transform.GetComponent<PlayerController>().ActiveOrder(false);

            StartCoroutine(ResetEnterCollision());
        }
	}

    public override void OnDisable()
    {
        base.OnDisable();

        // 오브젝트가 비활성화 될 때 풀링 리스트에 자신이 없다면 추가한다.
        if(GameManager.instance.poolPlayerObjectList.Contains(this) == false)
            GameManager.instance.poolPlayerObjectList.Add(this);

        if (photonView.IsMine)
        {
            // 카메라 타겟 초기화
            CameraFollow.target = null;
            // 플레이어 Scale 초기화
            photonView.RPC("SetScale", RpcTarget.AllBuffered, 1f);
            // 재시작 패널 활성화
            GameManager.instance.GameOver();
        }
    }

    // 한번 충돌이 일어나면 0.2초간 충돌이 일어나지 않게 해주는 코루틴
    private IEnumerator ResetEnterCollision()
    {
        yield return new WaitForSeconds(0.2f);
        enterCollision = false;
    }

    // 플레이어의 Scale 조절 함수
    [PunRPC]
    private void SetScale(float scale)
	{
        MyScale = scale;
        transform.localScale = Vector3.one * MyScale;
    }

    // 오브젝트 활성화 명령이 PhotonView에도 적용될 수 있도록 해주는 함수
    public void ActiveOrder(bool active)
    {
       if(photonView.IsMine) 
        {
            photonView.RPC("SetActive", RpcTarget.AllBuffered, active);
        }
    }

    // 오브젝트 활성화 함수
    [PunRPC]
    private void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    // 화면을 터치하면 조이스틱 처럼 처음 터치한 곳을 중심으로 드래그한 방향으로 이동 거리를 계산하는 함수
	private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;

                GameManager.instance.controller.SetActive(true);
                GameManager.instance.controller.transform.position = touchStartPos;

                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                touchCurrentPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                GameManager.instance.controller.SetActive(false);
                isTouching = false;
            }
        }
        else
        {
            GameManager.instance.controller.SetActive(false);
            isTouching = false;
        }
    }

    // 플레이어를 이동시키는 함수
    private void MovePlayer()
    {
        if (isTouching)
        {
            Vector2 touchDelta = touchCurrentPos - touchStartPos;
            Vector3 move = new Vector3(touchDelta.x, 0, touchDelta.y).normalized * speed * Time.deltaTime;
            Vector3 newPosition = transform.position + move;

            // 이동 범위를 제한
            newPosition.x = Mathf.Clamp(newPosition.x, -49.5f, 49.5f);
            newPosition.z = Mathf.Clamp(newPosition.z, -49.5f, 49.5f);

            transform.position = newPosition;
        }
    }
}