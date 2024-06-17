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

        if(GameManager.instance.poolPlayerObjectList.Contains(this) == true)
            GameManager.instance.poolPlayerObjectList.Remove(this);

        if (photonView.IsMine)
        {
            CameraFollow.target = transform;
            photonView.RPC("SetScale", RpcTarget.AllBuffered, MyScale);
        }
    }

	private void Update()
    {
        if (photonView.IsMine)
        {
            HandleTouchInput();
            MovePlayer();
        }
    }

	private void OnCollisionEnter(Collision collision)
	{
        if (collision.transform.CompareTag("Score") && enterCollision == false)
        {
            enterCollision = true;

            Score score = collision.transform.GetComponent<Score>();

            float newScale = transform.localScale.x + ((float)score.Level / 10);

            score.gameObject.SetActive(false);
            photonView.RPC("SetScale", RpcTarget.AllBuffered, newScale);

            StartCoroutine(ResetEnterCollision());
        }
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

        if(GameManager.instance.poolPlayerObjectList.Contains(this) == false)
            GameManager.instance.poolPlayerObjectList.Add(this);

        if (photonView.IsMine)
        {
            CameraFollow.target = null;
            photonView.RPC("SetScale", RpcTarget.AllBuffered, 1f);
            GameManager.instance.GameOver();
        }
    }

    private IEnumerator ResetEnterCollision()
    {
        yield return new WaitForSeconds(0.2f);
        enterCollision = false;
    }

    [PunRPC]
    private void SetScale(float scale)
	{
        Debug.Log("Big");
        MyScale = scale;
        transform.localScale = Vector3.one * MyScale;
    }

    public void ActiveOrder(bool active)
    {
       if(photonView.IsMine) 
        {
            photonView.RPC("SetActive", RpcTarget.AllBuffered, active);
        }
    }

    [PunRPC]
    private void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

	private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                touchCurrentPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isTouching = false;
            }
        }
        else
        {
            isTouching = false;
        }
    }

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