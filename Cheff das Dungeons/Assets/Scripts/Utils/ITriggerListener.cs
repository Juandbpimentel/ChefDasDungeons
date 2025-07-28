using UnityEngine;

/// <summary>
/// Define um "contrato" para qualquer componente que queira "ouvir"
/// os eventos de um gatilho filho (como o Custom2DTrigger).
/// </summary>
public interface ITriggerListener
{
    void OnChildTriggerEnter2D(GameObject triggerObject, Collider2D other);

    void OnChildTriggerExit2D(GameObject triggerObject, Collider2D other);
}

