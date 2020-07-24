using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public enum DroneState {
        DRONE_STATE_IDLE,
        DRONE_STATE_START_TAKINGOFF,
        DRONE_STATE_TAKINGOFF,
        DRONE_MOVING_UP,
        DRONE_FLYING,
        DRONE_STATE_START_LANGIN,
        DRONE_STATE_LANDING,
        DRONE_STATE_LANDED,
        DRONE_STATE_WAIT_ENDING_STOP,
    }

    DroneState _state;

    Animator _anim;
    Vector3 _speed = new Vector3(0f,0f,0f);
    
    public float speed_multiplayer;

    public bool IsIdle() {
        return (_state == DroneState.DRONE_STATE_IDLE);
    }

    public bool IsFlying()
    {
        return (_state == DroneState.DRONE_FLYING);
    }
    public void TakeOff()
    {
        _state = DroneState.DRONE_STATE_START_TAKINGOFF;
    }
    public void Land()
    {
        _state = DroneState.DRONE_STATE_START_LANGIN;
    }

    void Start()
    {
        _anim = GetComponent<Animator>();
    }


    // Move the AirDrone
    public void Move(float speed_x, float speed_z)
    {
        _speed.x = speed_x;
        _speed.z = speed_z;
        UpdateDronePosition();
    }

    void UpdateDronePosition()
    {
        switch (_state)
        {
            case DroneState.DRONE_STATE_IDLE:
                break;

            case DroneState.DRONE_STATE_START_TAKINGOFF:
                _anim.SetBool("TakeOff", true);
                _state = DroneState.DRONE_STATE_TAKINGOFF;
                break;

            case DroneState.DRONE_STATE_TAKINGOFF:
                if (_anim.GetBool("TakeOff") == false) {
                    _state = DroneState.DRONE_MOVING_UP;
                }
                break;

            case DroneState.DRONE_MOVING_UP:
                if (_anim.GetBool("MoveUp") == false) {
                    _state = DroneState.DRONE_FLYING;
                }
                break;

            case DroneState.DRONE_FLYING:
                float deltaTime = Time.deltaTime;
                float angle_Z = -30.0f * _speed.x * 60.0f * deltaTime;
                float angle_X = 30.0f * _speed.z * 60.0f * deltaTime;

                Vector3 rotation = transform.eulerAngles;
                transform.localPosition += _speed * speed_multiplayer * deltaTime;
                transform.localRotation = Quaternion.Euler(angle_X, rotation.y, angle_Z);
                break;

            case DroneState.DRONE_STATE_START_LANGIN:
                _anim.SetBool("MoveDown",true);
                _state = DroneState.DRONE_STATE_LANDING;
                break;


            case DroneState.DRONE_STATE_LANDING:
                if (_anim.GetBool("MoveDown") == false)
                {
                    _state = DroneState.DRONE_STATE_LANDED;
                }
                break;


            case DroneState.DRONE_STATE_LANDED:
                _anim.SetBool("Land", true);
                _state = DroneState.DRONE_STATE_WAIT_ENDING_STOP;
                break;

            case DroneState.DRONE_STATE_WAIT_ENDING_STOP:
                if (_anim.GetBool("Land") == false)
                {
                    _state = DroneState.DRONE_STATE_IDLE;
                }
                break;
        }        
    }
}
