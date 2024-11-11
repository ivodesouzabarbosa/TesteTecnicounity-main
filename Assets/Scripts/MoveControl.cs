using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveControl : MonoBehaviour
{
    public CharacterController _characterController;
    public Animator _anim;

    [Header("Motion Settings")]
    [SerializeField] float _moveSpeed = 5f;         // Velocidade de movimento do jogador
    [SerializeField] float _rotationSpeed = 720f;   // Velocidade de rotação
    [SerializeField] float _gravityValue = -9.81f;  // Valor da gravidade

    [SerializeField] private Vector3 moveDirection;        // Direção do movimento
    private Vector3 _playerVelocity;      // Velocidade do jogador para cálculo de gravidade
    public bool _isPunching;             // Para verificar se o soco está sendo realizado
    private bool _isGrounded;
    GameControl gameControl;// Verifica se o personagem está no chão
    public Transform target;

    public Transform _enemyP;
    public List<Transform> _enemy = new List<Transform>();

    public Transform _npc;

    public Transform _objCarg;

    public bool _car;
    public bool _checkP;









    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        gameControl = Camera.main.GetComponent<GameControl>();
        gameControl._playerControl = GetComponent<MoveControl>();

    }

    void Update()
    {
        if (_car)
        {
         //   _npc.transform.position = _objCarg.transform.position;
            _npc.localEulerAngles = transform.eulerAngles;
        }


        Move();// Aplica o movimento
        ApplyGravity();        // Aplica a gravidade
              
     

    }

    

    void DetectPunchInput()
    {
        // Detecta o toque na tela ou o pressionamento da tecla "E"
        if (Touchscreen.current.primaryTouch.press.isPressed || Keyboard.current.eKey.isPressed)
        {
            OnPunch(); // Chama o método de soco
        }
    }


    public void SetDir(InputAction.CallbackContext value)
    {
        moveDirection.x = value.ReadValue<Vector3>().x;
        moveDirection.z = value.ReadValue<Vector3>().y;
    }
    public void OnPunch(InputAction.CallbackContext value)
    {
        OnPunch();
    }

    void OnPunch()
    {
        if (!_isPunching) // Previne soco contínuo
        {
            _isPunching = true;
            _anim.SetBool("Punch", _isPunching); // Aciona o trigger de soco
            Invoke("ResetPunch", 0.5f); // Reseta o estado de soco após 0.5s (tempo da animação)

            if(_enemyP != null && _checkP)
            {
                _enemyP.GetComponent<ControlRag>().RagOn(false);
            }
        }
    }

    void ResetPunch()
    {
        _isPunching = false; // Reseta a variável de soco
        _anim.SetBool("Punch", _isPunching);
    }

    void ApplyGravity()
    {
        // Aplica a gravidade se o personagem não estiver no chão
        if (!_isGrounded)
        {
            _playerVelocity.y += _gravityValue * Time.deltaTime;
        }
        else if (_isGrounded && _playerVelocity.y < 0)
        {
            // Se está no chão e a velocidade vertical é menor que zero, reseta a velocidade vertical
            _playerVelocity.y = 0f;
        }

        // Aplica a movimentação final considerando a velocidade vertical
        _characterController.Move(_playerVelocity * Time.deltaTime);
    }

    public void Move()
    {

        float speed = moveDirection.magnitude * _moveSpeed;  // Calcula a velocidade total


        _anim.SetFloat("Speed", speed);  // Atualiza o parâmetro "Speed" no Animator
        if (moveDirection.magnitude > 0)
        {
            // Aplica a rotação suave na direção do movimento
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _rotationSpeed * Time.deltaTime);

            // Movimento baseado no CharacterController
            _characterController.Move(moveDirection * _moveSpeed * Time.deltaTime);
        }
     


    }

    // Verifica se o personagem está no chão
    void CheckGroundStatus()
    {
        float groundCheckDistance = 0.1f;
        _isGrounded = _characterController.isGrounded || Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }



    void EnemyDect()
    {
        Transform closestObject = FindClosestObject();
        if (closestObject != null)
        {
           // Debug.Log("Objeto mais próximo: " + closestObject.name);
            _enemyP = closestObject.transform;
        }
    }

    Transform FindClosestObject()
    {
        Transform closest = null;
        float closestDistance = Mathf.Infinity; // Começa com uma distância infinita para garantir que qualquer outra será menor

        foreach (Transform obj in _enemy)
        {
            // Calcula a distância entre o objeto atual e o objeto da lista
            float distance = Vector3.Distance(transform.position, obj.position);

            // Se a distância for menor que a menor distância registrada, atualiza o objeto mais próximo
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = obj;
            }
        }

        return closest;
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 6)
        {
            _enemyP = other.transform.parent;
            _checkP = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            _enemyP = null;
            _checkP =false;
        }
    }

}
