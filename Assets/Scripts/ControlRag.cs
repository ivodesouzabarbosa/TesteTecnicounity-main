using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ControlRag : MonoBehaviour
{
    Animator _animator;
    MoveNPC _moveNPC;
    NavMeshAgent _agent;

    public Transform target;
    public GameControl control;
    public Rigidbody[] ragdollBodies;

    public GameObject _cool;

   // public Transform target;          // O objeto alvo a ser seguido
    public float followSpeed = 5f;    // Velocidade de aproximação
    public float inertiaFactor = 0.1f; // Quanto maior o valor, maior o efeito de inércia (desaceleração)

    private Vector3 velocity = Vector3.zero; // Para armazenar a velocidade do movimento (usado no SmoothDamp)

    public Transform obj;

    public bool objCheck;




    void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _moveNPC = GetComponent<MoveNPC>();
        control = Camera.main.GetComponent<GameControl>();
        target = control._playerControl.transform;
    }

    private void Update()
    {
        if (objCheck)
        {
            Vector3 targetPosition = target.GetComponent<MoveControl>()._objCarg.transform.position;

            // Utilizando SmoothDamp para suavizar o movimento do objeto, criando o efeito de inércia
            obj.transform.position = Vector3.SmoothDamp(target.GetComponent<MoveControl>()._objCarg.transform.position, targetPosition, ref velocity, inertiaFactor);


        }

    }



    public void RagOn(bool value)
    {
        //  Debug.Log("ww");
        if (value)
        {
            _animator.enabled = true;

            _moveNPC.enabled = true;
           
            ActivateRagdoll(false);
            //  _agent.enabled=true;
        }
        else
        {
            ActivateRagdoll(true);
            _moveNPC.enabled = false;
            control._playerControl._car = true;
            _cool.SetActive(false);
            objCheck = true;

        }
    }

    void ActivateRagdoll(bool value)
    {
        _animator.enabled = !value;
        SetRagdollState(value);
    }

    void SetRagdollState(bool state)
    {
        foreach (Rigidbody rb in ragdollBodies)
        {
              //rb.isKinematic = state; // Ativa/desativa a física
              rb.useGravity = !state;
        }
    }

    
}
