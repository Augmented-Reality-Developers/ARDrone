using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    public DroneController _drone;
    public Button flyButton;
    public Button landButton;

    // Moving Controls
    public GameObject controls;


    // AR
    public ARRaycastManager rayManager;
    public ARPlaneManager planeManager;
    List<ARRaycastHit> hitResult = new List<ARRaycastHit>();


    // Drone 
    public GameObject _Drone;


    struct DroneAnimationControls {
        public bool _moving;
        public bool _interpolatingAsc;
        public bool _interpolatingDsc;
        public float _axis;
        public float _direction;
    }

    DroneAnimationControls _movingLeft;
    DroneAnimationControls _movingBack;

    void Start() {
        flyButton.onClick.AddListener(OnClickFlyButton);
        landButton.onClick.AddListener(OnClickLandButton);
    }

    void UpdateDroneControls(ref DroneAnimationControls control)
    {
        if (control._moving || control._interpolatingAsc || control._interpolatingDsc) {
            if (control._interpolatingAsc)
            {
                control._axis += 0.5f;
                if (control._axis > 1.0f) {
                    control._interpolatingAsc = false;
                    control._interpolatingDsc = true;
                    control._axis = 1.0f;
                }
            }
            else if (!control._moving)
            {
                control._axis -= 0.5f;
                if (control._axis < 0.0f)
                {
                    control._axis = 0.0f;
                    control._interpolatingDsc = false;
                }
            }
        }
    }

    // Update is called once per frame
    public void FixedUpdate()  {
        
        float x_speed = Input.GetAxis("Horizontal");
        float z_speed = Input.GetAxis("Vertical");
      
        UpdateDroneControls(ref _movingLeft);
        UpdateDroneControls(ref _movingBack);

        _drone.Move(_movingLeft._axis * _movingLeft._direction , _movingBack._axis * _movingBack._direction);

        if (_drone.IsIdle())
        {
            UpdateAR();
        }
    }

    public void OnClickFlyButton() {
        if (_drone.IsIdle()) {
            _drone.TakeOff();
            flyButton.gameObject.SetActive(false);
            controls.SetActive(true);
        }
    }

    public void OnClickLandButton() {
        if (_drone.IsFlying()) {
            _drone.Land();
            flyButton.gameObject.SetActive(true);
            controls.SetActive(false);
        }
    }
    public void OnClickLeftButton() {
        _movingLeft._moving = true;
        _movingLeft._interpolatingAsc = true;
        _movingLeft._direction = -1.0f;
    }

    public void OnReleasedLeftButton() {
        _movingLeft._moving = false;
    }

    public void OnClickRightButton() {
        _movingLeft._moving = true;
        _movingLeft._interpolatingAsc = true;
        _movingLeft._direction = 1.0f;
    }
    public void OnReleaseRightButton() {
        _movingLeft._moving = false;
    }

    public void OnClickBackButton() {
        _movingBack._moving = true;
        _movingBack._interpolatingAsc = true;
        _movingBack._direction = -1.0f;
    }

    public void OnReleaseBackButton()
    {
        _movingBack._moving = false;
    }

    public void OnClickForwardButton()
    {
        _movingBack._moving = true;
        _movingBack._interpolatingAsc = true;
        _movingBack._direction = 1.0f;
    }
    public void OnReleaseForwardButton() {
        _movingBack._moving = false;
    }


    public void UpdateAR() {
        Vector2 positionScreen = Camera.main.ViewportToScreenPoint(new Vector2(0.5f,0.5f));
        rayManager.Raycast(positionScreen, hitResult, TrackableType.PlaneWithinBounds);
        if (hitResult.Count > 0) {
            if (planeManager.GetPlane(hitResult[0].trackableId).alignment == PlaneAlignment.HorizontalUp)
            {
                Pose pose = hitResult[0].pose;
                _Drone.transform.position = pose.position;
                _Drone.SetActive(true);
            }
        }
    }
}
