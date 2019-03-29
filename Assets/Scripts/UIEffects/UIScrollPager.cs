using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScrollPager : MonoBehaviour {

	public int nPages;

	public float thresholdSpeedForNextPage;

	public float pageWidth;

	public float acceleration;

	public UITargetAnimation dotsTargetAnim;

	public const float Threshold = 1.0f;

	public float releaseSpeedFactor = 0.2f;

	public float maxInertialSpeed = 400.0f;

	public float escapeVelocity = 4000.0f;

	// all private below
	public float targetOffset;
	public float targetSpeed;
	public float offset;
	public float touchOffset;
	public float releaseSpeed;

	public bool picked = false;
	public bool calculateMotion = false;
	public float pickOffset;
	public float prevTouchOffset = 0.0f;
	public float currentPoint = 0.0f;
	public float inertiaSpeed = 0.0f;

	int consumePage = -1;

	public int page = 0;

	public float totalOffset;

	// Use this for initialization
	void Start () {
		targetOffset = 0.0f;
		consumePage = -1;
	}

	public void initialize() {
		targetOffset = 0.0f;
		consumePage = -1;
	}

	public void deinitialize() {
		// nuttin here
	}
	
	// Update is called once per frame
	void Update () {
		
		if(calculateMotion || (Mathf.Abs(inertiaSpeed) > Threshold)) {
			if (picked) {
				prevTouchOffset = currentPoint;
				currentPoint = (Input.mousePosition.x / ((float)Screen.width)) * pageWidth;
				touchOffset = currentPoint - pickOffset;
				if (totalOffset > 0.0f) { // softlimit {
					if (touchOffset < 0.0f) {
						touchOffset = -Mathf.Pow (-touchOffset, 0.66f);
					}
					else touchOffset = Mathf.Pow (touchOffset, 0.66f);
				}
				if (totalOffset < (-pageWidth * (nPages - 1))) {
					if (touchOffset < 0.0f) {
						touchOffset = -Mathf.Pow (-touchOffset, 0.66f);
					}
					else touchOffset = Mathf.Pow (touchOffset, 0.66f);
				}

			} else {
				

				targetSpeed = (targetOffset - offset) * acceleration;
				offset += inertiaSpeed * Time.deltaTime;
				if (Mathf.Abs(offset - targetOffset) < Threshold) {
					calculateMotion = false;
					offset = targetOffset;
				}

				float inertiaAcceleration = (targetSpeed - inertiaSpeed) * acceleration * 2;
				float differenceBefore = inertiaAcceleration;
				inertiaSpeed += inertiaAcceleration * Time.deltaTime;
				float differenceAfter = (targetSpeed - inertiaSpeed);
				// two ways to kill difference:
				if ((differenceAfter * differenceBefore) < 0.0f) { // a: avoid overoscillations
					inertiaSpeed = targetSpeed;
				}
				if (Mathf.Abs(inertiaSpeed - targetSpeed) < Threshold) { // b: damp
					inertiaSpeed = 0.0f;
				}

			}
			totalOffset = offset + touchOffset;
//			if (totalOffset > 0.0f) // softlimit
//				totalOffset = Mathf.Pow (totalOffset, 0.66f);
//			if (totalOffset < (-pageWidth * (nPages - 1))) {
//				float excess = -(totalOffset - (-pageWidth * (nPages - 1)));
//				excess = Mathf.Pow (excess, 0.66f);
//				totalOffset = -excess + (-pageWidth * (nPages - 1));
//			}
			this.transform.localPosition = new Vector3 (totalOffset, 0, 0);
			updatePageTarget ();
		}

	}

	public void pick() {
		picked = true;
		pickOffset = (Input.mousePosition.x / ((float)Screen.width)) * pageWidth;
		prevTouchOffset = pickOffset;
		calculateMotion = true;
	}

	private void updatePageTarget() {
		
		float minDelta = Mathf.Abs(offset);
		int minPage = 0;
		for (int i = 1; i < nPages; ++i) {
			float curOffset = -pageWidth * i;
			float delta = Mathf.Abs (curOffset - offset);
			if (delta < minDelta) {
				minDelta = delta;
				minPage = i;
			}
		}
		dotsTargetAnim.setFrame (minPage);
		if (minPage == consumePage)
			consumePage = -1;

		if (consumePage == -1)
			page = minPage;
		else
			page = consumePage;

		targetOffset = -page * pageWidth;
	}

	public void release() {

		float deltaOffset = (currentPoint - prevTouchOffset);
		inertiaSpeed = (deltaOffset / Time.deltaTime) * releaseSpeedFactor;
		if (Mathf.Abs (inertiaSpeed) > maxInertialSpeed) {
			if (inertiaSpeed > 0.0f)
				inertiaSpeed = maxInertialSpeed;
			else
				inertiaSpeed = -maxInertialSpeed;
		}
		releaseSpeed = inertiaSpeed;
		if (Mathf.Abs(releaseSpeed) > thresholdSpeedForNextPage) {
			if (releaseSpeed > 0) { // prev page
				if(page > 0) consumePage = page-1;
			} else { // next page
				if(page < (nPages-1)) consumePage = page + 1;
			}
		}
		picked = false;
		offset += touchOffset;
		touchOffset = 0.0f;

	}
}
