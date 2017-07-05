using UnityEngine;
using System.Collections;
 
public class Player : MonoBehaviour {
 
        public int turnSpeed;
        public int moveSpeed;
        public int jumpForce;
       
        bool onGround;
       
        // Use this for initialization
        void Start () {
       
        }
       
        // Update is called once per frame
        void FixedUpdate () {
       
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
               
                transform.Rotate( 0, h * turnSpeed * Time.deltaTime, 0 );
                Vector3 moveAmount = transform.forward * v * moveSpeed;
                rigidbody.MovePosition( transform.position + moveAmount * Time.deltaTime );
                rigidbody.velocity = moveAmount + Vector3.Scale(rigidbody.velocity, new Vector3(0,1,0));       
               
                if (onGround && Input.GetKey(KeyCode.Space))
                {
                        rigidbody.AddForce( transform.up * jumpForce, ForceMode.Impulse );     
                        onGround = false;
                }
        }
       
        void OnCollisionEnter() {
       
                onGround = true;
               
        }
       
}