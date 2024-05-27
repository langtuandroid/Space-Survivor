using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerMovement : MonoBehaviour
{
    [FoldoutGroup("참조")][SerializeField] private FixedTouchField fixedTouchField;
    [FoldoutGroup("참조")][SerializeField] private FixedJoystick joystick;
    [FoldoutGroup("참조")][SerializeField] private PlayerStat playerStat;
    [FoldoutGroup("참조")][SerializeField] private Transform playerBody;
    [FoldoutGroup("참조")][SerializeField] private Transform virtualTarget;
    [FoldoutGroup("참조")][SerializeField] private Rigidbody2D rigid;


    private Vector2 virtualDir = Vector2.zero;
    private Vector2 moveDir;
    private float angle;
    private bool isMoving = false;


    private void Start()
    {
        StartCoroutine(ChangeVirtualDir());

        IEnumerator ChangeVirtualDir()
        {
            while (true)
            {
                virtualDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

                yield return new WaitForSeconds(Random.Range(5, 20));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerStat.GetPlayerDie())
            return;

        if (GameManager.instance.inMainMenu)
        {
            virtualTarget.RotateAround(transform.position, Vector3.back, Time.deltaTime * 10);

            Vector2 dirToVirtualTarget = Utility.GetDirection(transform.position, virtualTarget.position);

            //rigid.velocity += new Vector2(playerBody.transform.up.x, playerBody.transform.up.y) * Time.deltaTime * playerStat.GetMoveSpeed();
            // rigidbody.velocity = playerBody.transform.up * Time.deltaTime * playerStat.GetMoveSpeed() * 10f;
            //rigid.velocity = Vector2.Lerp(rigid.velocity, new Vector2(playerBody.transform.up.x, playerBody.transform.up.y) * playerStat.GetMoveSpeed() * 0.7f, Time.deltaTime * 3);

            if (Mathf.Abs(virtualDir.normalized.x) > 0 || Mathf.Abs(virtualDir.normalized.y) > 0)
            {
                angle = Mathf.Atan2((virtualDir.normalized.y + playerBody.transform.position.y) - playerBody.transform.position.y,
                (virtualDir.normalized.x + playerBody.transform.position.x) - playerBody.transform.position.x) * Mathf.Rad2Deg;

                isMoving = true;
            }

            if (playerBody != null)
                playerBody.transform.rotation = Quaternion.Lerp(playerBody.transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), playerStat.GetRotationSpeed() * Time.deltaTime * 0.3f);
        }
        else if (fixedTouchField.Pressed)
        {
            //rigid.velocity += new Vector2(playerBody.transform.up.x, playerBody.transform.up.y) * Time.deltaTime * playerStat.GetMoveSpeed() * fixedTouchField.distBetweenJoystickBodyToHandle;
            // rigidbody.velocity = playerBody.transform.up * Time.deltaTime * playerStat.GetMoveSpeed() * 10f;
            //rigid.velocity = Vector2.Lerp(rigid.velocity, new Vector2(playerBody.transform.up.x, playerBody.transform.up.y) * playerStat.GetMoveSpeed() * 0.7f, Time.deltaTime * 3);

            // print(fixedTouchField.distBetweenJoystickBodyToHandle);

            if (Mathf.Abs(fixedTouchField.joystickDir.normalized.x) > 0 || Mathf.Abs(fixedTouchField.joystickDir.normalized.y) > 0)
            {
                angle = Mathf.Atan2((fixedTouchField.joystickDir.normalized.y + playerBody.transform.position.y) - playerBody.transform.position.y,
                (fixedTouchField.joystickDir.normalized.x + playerBody.transform.position.x) - playerBody.transform.position.x) * Mathf.Rad2Deg;

                isMoving = true;
            }

            if (playerBody != null)
                playerBody.transform.rotation = Quaternion.Lerp(playerBody.transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), playerStat.GetRotationSpeed() * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (playerStat.GetPlayerDie())
            return;

        if (GameManager.instance.inMainMenu)
        {
            rigid.velocity = Vector2.Lerp(rigid.velocity, new Vector2(playerBody.transform.up.x, playerBody.transform.up.y) * playerStat.GetMoveSpeed() * 0.6f, Time.deltaTime * 3);

        }
        else if (fixedTouchField.Pressed)
        {
            rigid.velocity = Vector2.Lerp(rigid.velocity, new Vector2(playerBody.transform.up.x, playerBody.transform.up.y) * playerStat.GetMoveSpeed() * 0.6f * fixedTouchField.distBetweenJoystickBodyToHandle, Time.deltaTime * 3);
        }
    }

    public bool getIsMoving()
    {
        return isMoving;
    }

    public void SetPlayerBody(Transform body)
    {
        playerBody = body;
    }
}
