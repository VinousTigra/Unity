using System;

public static class GameEvents
{
    public static event Action<bool> FridgeStateChanged;

    public static event Action<StoveState> StoveStateChanged;
    public static event Action StoveReady;

    public static event Action Stirred;
    public static event Action SaltApplied;

    public static event Action FryingStarted;
    public static event Action FryingStopped;

    public static event Action DishBurned;
    public static event Action CraftFailed;

    public static event Action<DishResult> DishServed;

    public static event Action<string> StatusMessage;

    public static void RaiseFridgeStateChanged(bool isOpen)
        => FridgeStateChanged?.Invoke(isOpen);

    public static void RaiseStoveStateChanged(StoveState state)
        => StoveStateChanged?.Invoke(state);

    public static void RaiseStoveReady()
        => StoveReady?.Invoke();

    public static void RaiseStirred()
        => Stirred?.Invoke();

    public static void RaiseSaltApplied()
        => SaltApplied?.Invoke();

    public static void RaiseFryingStarted()
        => FryingStarted?.Invoke();

    public static void RaiseFryingStopped()
        => FryingStopped?.Invoke();

    public static void RaiseDishBurned()
        => DishBurned?.Invoke();

    public static void RaiseCraftFailed()
        => CraftFailed?.Invoke();

    public static void RaiseDishServed(DishResult result)
        => DishServed?.Invoke(result);

    public static void ShowStatus(string message)
        => StatusMessage?.Invoke(message);
}