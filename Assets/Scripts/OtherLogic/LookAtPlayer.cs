using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour {
    [SerializeField] private Transform head;           // Ссылка на кость головы
    [SerializeField] private Transform player;         // Ссылка на игрока
    [SerializeField, Range(0.1f, 10f)] private float rotationSpeed = 2f;
    [SerializeField] private Vector3 rotationOffset;   // Смещение в градусах, если голова "кривая"

    void Update()
    {
        if (player == null || head == null)
            return;

        // Направление от головы к игроку
        Vector3 direction = player.position - head.position;

        // Если игрок слишком близко — не крутим головой
        if (direction.sqrMagnitude < 0.001f)
            return;

        // Рассчитываем желаемый поворот
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Добавляем смещение
        lookRotation *= Quaternion.Euler(rotationOffset);

        // Плавно поворачиваем голову
        head.rotation = Quaternion.Slerp(
            head.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );
    }
}
