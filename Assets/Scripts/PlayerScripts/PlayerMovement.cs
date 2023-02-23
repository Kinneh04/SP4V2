using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f;
    float OGmoveSpeed;
    public PlayerProperties playerProperties;
    private float turnSpeedX;
    private float turnSpeedY;
    public float jumpHeight = 2f;
    public float swayAmount = 0.5f;
    private Rigidbody rb;
    private float Xrotation = 0f;
    private float Yrotation = 0f;
    private bool isGrounded;
    public bool isMovementEnabled;
    public PhotonView pv;
    public bool canLookAround;
    public InventoryManager IM;
    public GameObject Torso;
    public bool playingSprintAnim = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        OGmoveSpeed = moveSpeed;
        turnSpeedX = 180f * playerProperties.XSensitivity;
        turnSpeedY = 180f * playerProperties.YSensitivity;
    }

    public void UnlockCursor()
    {
        canLookAround = false;
        Cursor.lockState = CursorLockMode.None;
        isMovementEnabled = false;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        rb.Sleep();
    }

    public void LockCursor()
    {
        canLookAround = true;
        Cursor.lockState = CursorLockMode.Locked;
        isMovementEnabled = true;
    }

    void Update()
    {
        if (pv.IsMine && !playerProperties.isSleeping)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (canLookAround)
            {
                Xrotation += Input.GetAxis("Mouse X") * turnSpeedX * Time.deltaTime;
                Yrotation += Input.GetAxis("Mouse Y") * turnSpeedY * Time.deltaTime;

                Yrotation = Mathf.Clamp(Yrotation, -60f, 60f);
                Torso.transform.rotation = Quaternion.Euler(0, 0, 0);
                Xrotation %= 360;
                transform.rotation = Quaternion.Euler(-Yrotation, Xrotation, 0f);
            }
            if (Mathf.Approximately(horizontal, 0f) && Mathf.Approximately(vertical, 0f) && isGrounded || !isMovementEnabled && isGrounded || GetComponent<ChatManager>().isTyping && isGrounded)
            {
                // If no movement keys are being pressed, set velocity to zero
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
                rb.Sleep();
            }
            else
            {

                rb.constraints = RigidbodyConstraints.None;
                Vector3 movement = transform.forward * vertical + transform.right * horizontal;
                movement.y = 0;
                rb.MovePosition(transform.position + movement.normalized * moveSpeed * Time.deltaTime);
                float sway = Mathf.Sin(Time.time * moveSpeed / 5) * swayAmount;
                transform.rotation *= Quaternion.Euler(0f, sway, 0f);

                if (Input.GetButtonDown("Jump") && isGrounded)
                {
                    rb.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
                    isGrounded = false;
                }
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    moveSpeed = sprintSpeed;
                    swayAmount = 0.5f;
                   if(IM.InventoryList[IM.EquippedSlot].itemType == ItemInfo.ItemType.Ranged && !playingSprintAnim)
                    {
                        pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanSprintWeapon");
                        playingSprintAnim = true;
                    }
                   
                    // print("SPRINTING!");
                }
                else
                {
                    moveSpeed = OGmoveSpeed;
                    swayAmount = 0.0f;

                    if(playingSprintAnim)
                    {
                        playingSprintAnim = false;
                        pv.RPC("PlayServerSideAnimation", RpcTarget.All, pv.ViewID, "PBeanIdle");
                    }
                }
            }
            isGrounded = Physics.Raycast(Torso.transform.position, Vector3.down, 0.4f);

           
            



          
        }
    }
}
