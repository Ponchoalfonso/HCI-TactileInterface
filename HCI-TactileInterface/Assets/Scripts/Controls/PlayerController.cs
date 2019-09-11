using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum Direction { Up, Down, Left, Right };
public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    private Vector2 firstPressPos = new Vector2(0, 0);
    private Animator animator;
    public Camera camera;
    public Text goalText;
    // private GestureListener gestureListener;

    public float speed = 1;
    public bool moving
    {
        get { return rb.velocity.magnitude > 0; }
    }

    // Start is called before the first frame update
    void Start()
    {
        this.rb = this.GetComponent<Rigidbody2D>();
        this.animator = this.GetComponentInChildren<Animator>();
        // gestureListener = Camera.main.GetComponent<GestureListener>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.GetInput();
        camera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
    }

    void GetInput()
    {
        if (!moving)
        {
            this.animator.gameObject.transform.localScale = new Vector3(1, 1, 1);
            this.animator.SetInteger("state", 0);
        }

        // Keyboard or controller inputss
        if (Input.GetAxis("Horizontal") > 0)
            this.Move(Direction.Right);
        else if (Input.GetAxis("Horizontal") < 0)
            this.Move(Direction.Left);
        else if (Input.GetAxis("Vertical") > 0)
            this.Move(Direction.Up);
        else if (Input.GetAxis("Vertical") < 0)
            this.Move(Direction.Down);

        // Touch control
        this.Swipe();

        // Don't proccess kinect if there is no instance or if character is moving
        /*KinectManager kinectManager = KinectManager.Instance;
        if (!this.moving && (!kinectManager || !kinectManager.IsInitialized() || !kinectManager.IsUserDetected()))
            return;
        if (gestureListener)
            this.SwipeKinect();
        */
    }

    public void Swipe()
    {
        Vector2 secondPressPos;
        Vector2 currentSwipe;
        float narrow = 0.5f;

        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                firstPressPos = new Vector2(t.position.x, t.position.y);
            } if (t.phase == TouchPhase.Ended)
            {
                secondPressPos = new Vector2(t.position.x, t.position.y);
                currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
                currentSwipe.Normalize();
                // Swipe up
                if (currentSwipe.y > 0 && currentSwipe.x > -narrow && currentSwipe.x < narrow)
                    this.Move(Direction.Up);
                // Swipe down;
                else if (currentSwipe.y < 0 && currentSwipe.x > -narrow && currentSwipe.x < narrow)
                    this.Move(Direction.Down);
                // Swipe left
                else if (currentSwipe.x < 0 && currentSwipe.y > -narrow && currentSwipe.y < narrow)
                    this.Move(Direction.Left);
                // Swipe right
                else if (currentSwipe.x > 0 && currentSwipe.y > -narrow && currentSwipe.y < narrow)
                    this.Move(Direction.Right);
            }
        }
    }

    /*
    void SwipeKinect()
    {
        if (gestureListener.IsSwipeLeft())
            this.Move(Direction.Left);
        else if (gestureListener.IsSwipeRight())
            this.Move(Direction.Right);
        else if (gestureListener.IsSwipeUp())
            this.Move(Direction.Up);
        else if (gestureListener.IsSwipeDown())
            this.Move(Direction.Down);
    }
    */

    void Move(Direction direction)
    {
        float f = speed * 100;
        if (!moving)
        {
            if (direction.Equals(Direction.Right))
            {
                this.rb.AddForce(new Vector2(f, 0));
                this.animator.SetInteger("state", 1);
                this.animator.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (direction.Equals(Direction.Left))
            {
                this.rb.AddForce(new Vector2(-f, 0));
                this.animator.SetInteger("state", 1);
                this.animator.gameObject.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (direction.Equals(Direction.Up))
            {
                this.rb.AddForce(new Vector2(0, f));
                this.animator.SetInteger("state", 2);
                this.animator.gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (direction.Equals(Direction.Down))
            {
                this.rb.AddForce(new Vector2(0, -f));
                this.animator.SetInteger("state", 2);
                this.animator.gameObject.transform.localScale = new Vector3(1, -1, 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            this.goalText.gameObject.SetActive(true);
            this.rb.Sleep();
        }
    }
}
