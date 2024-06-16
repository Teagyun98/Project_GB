using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public float speed = 5f;
    public float MyScale { get; private set; } = 1f;
    private Vector2 touchStartPos;
    private Vector2 touchCurrentPos;
    private bool isTouching = false;

	private void Start()
	{
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
		if(collision.transform.CompareTag("Score"))
		{
            Score score = collision.transform.GetComponent<Score>();

            float newScale = transform.localScale.x + ((float)score.Level / 10);

            photonView.RPC("SetScale", RpcTarget.AllBuffered, newScale);
            score.Destroy();
		}
	}

    [PunRPC]
    private void SetScale(float scale)
	{
        MyScale = scale;
        transform.localScale = Vector3.one * MyScale;
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
            Vector3 move = new Vector3(touchDelta.x, 0, touchDelta.y).normalized;

            transform.Translate(move * speed * Time.deltaTime, Space.World);
        }
    }
}