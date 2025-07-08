#region

using UnityEngine;

#endregion

public class GameManager : MonoBehaviour
{
    [SerializeField] private ShroomLauncher shroomLauncher;

    private void Start()
    {
        Next();
    }

    public void ResetState()
    {
        shroomLauncher.ResetState();
    }

    public void Next()
    {
        shroomLauncher.NextShroom();
    }
}