using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    public static UndoManager instance;

    public PlayerMovement player; 
    public AIMovement ai;    

        [System.Serializable] 
    public struct GameState
    {
        public Node playerNode;
        public Node aiNode;
    }     
    private Stack<GameState> history = new Stack<GameState>();
    private int undosRemaining = 3;

    private void Awake() => instance = this;

    void Start()
    {
        // Record the starting positions immediately so we can undo back to the start
        RecordState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            PerformUndo();
        }
    }

    public void RecordState()
    {
        GameState newState = new GameState {
            playerNode = player.currentNode,
            aiNode = ai.currentNode
        };
        history.Push(newState);
        Debug.Log("State Recorded. Stack size: " + history.Count);
    }

    public void PerformUndo()
    {
        // We need history.Count > 1 because the bottom of the stack is our current position
        if (undosRemaining > 0 && history.Count > 1)
        {
            // 1. Discard the "Current" state we are standing in
            history.Pop();
            
            // 2. Peek at the "Previous" state to see where to go
            GameState previous = history.Peek();

            // 3. Teleport characters
            player.currentNode = previous.playerNode;
            player.transform.position = previous.playerNode.transform.position;

            ai.currentNode = previous.aiNode;
            ai.transform.position = previous.aiNode.transform.position;
            
            // 4. Clean up AI path so it doesn't try to finish its old movement
            ai.path.Clear();

            undosRemaining--;
            Debug.Log($"Undo successful! Undos left: {undosRemaining}. Stack size: {history.Count}");
        }
        else
        {
            Debug.Log("Cannot undo: No charges left or at the very beginning of the level!");
        }
    }

    public void ResetUndoCount()
    {
        undosRemaining = 3;
        history.Clear();
        RecordState(); // Re-record the "New" starting position after a reset
    }
}