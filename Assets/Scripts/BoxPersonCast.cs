using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPersonCast : MonoBehaviour
{
    // Tamanho do BoxCast
    public Vector3 boxSize = new Vector3(1f, 1f, 1f);

    // Direção e distância do BoxCast
    public Vector3 direction = Vector3.forward;
    public float distance = 5f;

    // Camada a ser verificada
    public LayerMask layerMask;

    MoveControl moveControl;

    private void Start()
    {
        moveControl = GetComponent<MoveControl>();
    }
    void Update()
    {
        // Realiza o BoxCast
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, boxSize / 2, transform.TransformDirection(direction), out hit, Quaternion.identity, distance, layerMask))
        {
            Debug.Log("Objeto detectado: " + hit.collider.name);
            if (hit.collider.gameObject.layer == 6) {
                MoveNPC moveNPC = hit.collider.transform.parent.GetComponent<MoveNPC>();
                if (!moveNPC._onlist)
                {
                    moveNPC._onlist = true;
                    moveControl._enemy.Add(hit.collider.transform.parent);
                }
            }
        }
    }

    // Método para visualizar o BoxCast com Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Ponto de origem do BoxCast
        Vector3 origin = transform.position;

        // Desenha o cubo na posição do BoxCast
        Gizmos.DrawWireCube(origin, boxSize);
    }
}
