using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private float aheadDistance = 3.0f;
    [SerializeField] private float cameraSpeed = 2.0f;
    private float lookAhead;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }
    private void Update()
    {
            transform.position = new Vector3(player.transform.position.x + lookAhead, player.transform.position.y + 2.3f, transform.position.z);
            lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.transform.localScale.x), Time.deltaTime * cameraSpeed);
    }

}