using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{ 
	[Header("Main Movement Options")]
    public float speed = 10f;
    public Rigidbody rb;
    public Transform cam;
    public float turnSmoothing = 0.1f;

    [Header("Jump Controls")]
    public float jumpForce = 400f;
    public float jumpPointRad = 0.05f;
    public Transform feet;
    public LayerMask whatIsGround;

    [Header("Particle System Settings")]
    public GameObject jumpPS;
    
    private Vector3 movement;
    private float _speed;
    private float _turnSmoothvel;
    private bool jumping = false;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
        if(Input.GetButtonDown("Sprint")){
        	speed *= 1.5f;
        }
        if(Input.GetButtonUp("Sprint")){
        	speed = _speed;
        }

        if(Input.GetButtonDown("Jump")){
        	Jump();
        }
    }

    void Jump(){
    	if(Physics.OverlapSphere(feet.position, jumpPointRad, whatIsGround).Length >= 1){
    		jumping = true;
    	}
    }

    void FixedUpdate(){
    	if(movement.magnitude >= 0.1f){
    		float targetAngle = Mathf.Atan2(movement.x, movement.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
    		float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothvel, turnSmoothing);
    		transform.rotation = Quaternion.Euler(0f, angle, 0f);

    		Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
           	rb.MovePosition(rb.position + (moveDirection.normalized) * speed * Time.fixedDeltaTime);
    	}

    	if(jumping){
    		rb.AddForce(transform.up * jumpForce * Time.deltaTime, ForceMode.Impulse);
    		GameObject ps = Instantiate(jumpPS, feet.position, Quaternion.identity);
    		Destroy(ps, 0.5f);
    		jumping = false;
    	}
    }

    void OnCollisionEnter(Collision other){
        rb.freezeRotation = true;
    }
}
