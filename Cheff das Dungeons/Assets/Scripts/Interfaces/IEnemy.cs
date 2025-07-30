using UnityEngine;

public interface IEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void levarDano(int dano);

    public void GetKnockedback(Transform playerTransform, float knockedbackForce, float stunTime);

    public void generateDrop();
}
