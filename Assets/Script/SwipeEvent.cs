using UnityEngine;

public class SwipeEvent : MonoBehaviour {
	
	[SerializeField] private float minimumSwipeDistanceY;
	[SerializeField] private float minimumSwipeDistanceX;

	public GameObject whale1; 
	public GameObject whale2;
	public GameObject whale3;
	public GameObject swipeUI;
	Animator animWhale1;
	Animator animWhale2;
	Animator animWhale3;

	bool IsplayingAnim;


	private Touch t = default(Touch);
	private Vector3 startPosition = Vector3.zero;

	private void Start ()
	{
		animWhale1 = whale1.GetComponent<Animator> ();
		animWhale2 = whale2.GetComponent<Animator> ();
		animWhale3 = whale3.GetComponent<Animator> ();
		IsplayingAnim = false;

	}

	private void Update () {
		if (Input.touches.Length > 0) {

			t = Input.touches[0];

			switch (t.phase) {

			case TouchPhase.Began:
				startPosition = t.position;
				return;
			case TouchPhase.Ended:
				Vector3 positionDelta = (Vector3) t.position - startPosition;
				if (Mathf.Abs(positionDelta.y) > minimumSwipeDistanceY) {
					if (positionDelta.y > 0 && IsplayingAnim == false) {
						
						Debug.Log("UP SWIPE!!!");
						animWhale1.SetTrigger ("Start");
						animWhale2.SetTrigger ("Start");
						animWhale3.SetTrigger ("Start");
						swipeUI.SetActive (false);

					} else {
						Debug.Log("DOWN SWIPE!!!");
					}
				}
				if (Mathf.Abs(positionDelta.x) > minimumSwipeDistanceX) {
					if (positionDelta.x > 0) {
						Debug.Log("SWIPE RIGHT");
					} else {
						Debug.Log("SWIPE LEFT");
					}
				}
				return;
			default:
				return;
			}
		}
	}


	public void IsPlayEnable ()
	{
		IsplayingAnim = true;
	}

	public void IsPlayDisable ()
	{
		IsplayingAnim = false;
	}

}