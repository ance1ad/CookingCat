using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour {
    [SerializeField] private Transform _playerPoint;
    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private float rotationSpeed = 7f;

    private void LateUpdate() {
        if (_playerPoint == null) return;

        // 1. ������� ����������� �� ������
        Vector3 direction = _playerPoint.transform.position - transform.position;
        direction.y = 0; // ������� ������ �� ��������� (���� �� ������� ������)

        // 2. ������� ������� �������
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 3. ������ ������������� ����� ������� � ������� ���������
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // 4. ��������� �������� (��������)
        transform.Rotate(rotationOffset, Space.Self);
    }
}
