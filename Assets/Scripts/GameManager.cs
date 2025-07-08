#region

using UnityEngine;

#endregion

/// <summary>
/// A single entry point for the game.
/// It is usually good practice to have this kind of single entry point for easier debugging and extension.
/// In this game however it is not very useful because it is so minimalistic (yet)
/// </summary>
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