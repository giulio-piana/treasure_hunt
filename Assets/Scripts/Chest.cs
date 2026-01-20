using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

 public enum ChestState
{
    Closed,     
    Opening,    
    Opened      
}
public class Chest
{
    public int Index { get; private set; }
    public bool IsWinning { get; private set; }
    public FSM<Chest,ChestState> StateMachine { get; private set; }
    
     private CancellationTokenSource currentOpeningCTS;

    public Chest(int index, bool isWinning)
    {
        Index = index;
        IsWinning = isWinning;

        StateMachine = new FSM<Chest, ChestState>(this);
        StateMachine.AddState(ChestState.Closed, null);
        StateMachine.AddState(ChestState.Opening, null);
        StateMachine.AddState(ChestState.Opened, null);
        StateMachine.ChangeState(ChestState.Closed);
    }

    public async Task<bool> OpenAsync(float duration, CancellationToken externalCancellationToken = default)
    {
        
        if (StateMachine.CurrentState != ChestState.Closed)
        {
            Debug.LogWarning($"Chest {Index} cannot be opened - current state: {StateMachine.CurrentState }");
            return false;
        }
       
        currentOpeningCTS = new CancellationTokenSource();
        
        using (var linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(currentOpeningCTS.Token, externalCancellationToken))
        {
            try
            {              
                StateMachine.ChangeState(ChestState.Opening);                
                await Task.Delay(TimeSpan.FromSeconds(duration), linkedCTS.Token);
                StateMachine.ChangeState(ChestState.Opened);
                
                return true;
            }
            catch (OperationCanceledException)
            {
                StateMachine.ChangeState(ChestState.Closed);
                
                Debug.Log($"Chest {Index} opening was cancelled");
                return false;
            }
            finally
            {                
                currentOpeningCTS?.Dispose();
                currentOpeningCTS = null;
            }
        }
    }

    public void CancelOpening()
    {
        if ( StateMachine.CurrentState == ChestState.Opening && currentOpeningCTS != null)
        {
            currentOpeningCTS.Cancel();
        }
    }

    public void Reset(bool isWinning)
    {        
        CancelOpening();
        
        IsWinning = isWinning;

        StateMachine.ChangeState(ChestState.Closed);
    }
}



