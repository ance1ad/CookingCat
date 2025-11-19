using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipable {
    public void Apply(PlayerData data);
    public void Remove(PlayerData data);
}
