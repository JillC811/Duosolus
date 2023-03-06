using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUndoable
{
    public void undo(Vector3 coord);
}
