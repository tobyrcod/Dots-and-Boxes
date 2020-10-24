using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotsBoxes {
    bool[,] horizontalLines;
    bool[,] verticalLines;

    public bool isPlayerOneTurn { get; private set; }
    public int playerOneScore { get; private set; }
    public int playerTwoScore { get; private set; }

    public Action<Line> OnAddLineEvent;
    public Action<Vector2Int> OnBoxMadeEvent;
    public Action OnTurnOverEvent;
    public Action<bool> OnScoreChangedEvent;

    public DotsBoxes(int width, int height) {
        //When Grid is 4x4
        horizontalLines = new bool[width - 1, height]; // Horizontal: 3x4
        verticalLines = new bool[width, height - 1]; // Vertical: 4x3

        isPlayerOneTurn = true;

        playerOneScore = 0;
        playerTwoScore = 0;
    }

    public bool TryAddLine(Line line) {
        if (DoesLineExist(line)) 
            return false;      

        if (line.horizontal) {
            horizontalLines[line.x, line.y] = true;
        }
        else {
            verticalLines[line.x, line.y] = true;
        }         

        if (OnAddLineEvent != null) {
            OnAddLineEvent.Invoke(line);
        }

        //Once we place the new Line, check to see if that line completed a box.
        CheckForNewBox(line, out bool wasBoxMade);
        if (!wasBoxMade) {
            //If no box was made, change whos turn it is
            isPlayerOneTurn = !isPlayerOneTurn;
        }
        //If a box is made, the person that makes a box gets another turn, so dont change the turn variable. 


        if (OnTurnOverEvent != null) {
            OnTurnOverEvent.Invoke();
        }

        return true;
    }

    private void CheckForNewBox(Line newLine, out bool wasBoxMade) {

        //Assume no box is made
        wasBoxMade = false;

        //First Check for Vertical Box
        if (!newLine.horizontal) {
            //Check for box on the left
            if (newLine.x > 0 && newLine.y < horizontalLines.GetLength(1) - 1) {
                //Ensures there is a left column to check
                if (verticalLines[newLine.x - 1, newLine.y]
                    && horizontalLines[newLine.x - 1, newLine.y + 1]
                    && horizontalLines[newLine.x - 1, newLine.y]) {
                    //This line made a new Box on the Left

                    wasBoxMade = true;
                    AddPlayerScore(1);

                    if (OnBoxMadeEvent != null) {
                        Vector2Int bottomLeft = new Vector2Int(newLine.x - 1, newLine.y);
                        OnBoxMadeEvent.Invoke(bottomLeft);
                    }
                }
            }

            //Check for box on the right
            if (newLine.x < verticalLines.GetLength(0) - 1 && newLine.y < horizontalLines.GetLength(1) - 1) {
                if (verticalLines[newLine.x + 1, newLine.y]
                    && horizontalLines[newLine.x, newLine.y + 1]
                    && horizontalLines[newLine.x, newLine.y]) {
                    //This line made a new Box on the Right

                    wasBoxMade = true;
                    AddPlayerScore(1);

                    if (OnBoxMadeEvent != null) {
                        Vector2Int bottomLeft = new Vector2Int(newLine.x, newLine.y);
                        OnBoxMadeEvent.Invoke(bottomLeft);
                    }
                }
            }
        }
        else {
            //Check for Horizontal Line Box 

            //Check for box above the line
            if (newLine.y < horizontalLines.GetLength(1) - 1 && newLine.x < verticalLines.GetLength(0) - 1) {
                if (horizontalLines[newLine.x, newLine.y + 1]
                    && verticalLines[newLine.x, newLine.y]
                    && verticalLines[newLine.x + 1, newLine.y]) {
                    //This line made a new Box above the line

                    wasBoxMade = true;
                    AddPlayerScore(1);

                    if (OnBoxMadeEvent != null) {
                        Vector2Int bottomLeft = new Vector2Int(newLine.x, newLine.y);
                        OnBoxMadeEvent.Invoke(bottomLeft);
                    }
                }
            }

            //Check for box below the line
            if (newLine.y > 0 && newLine.x < verticalLines.GetLength(0) - 1) {
                if (horizontalLines[newLine.x, newLine.y - 1]
                    && verticalLines[newLine.x, newLine.y - 1]
                    && verticalLines[newLine.x + 1, newLine.y - 1]) {
                    //This line made a new Box below the line

                    wasBoxMade = true;
                    AddPlayerScore(1);

                    if (OnBoxMadeEvent != null) {
                        Vector2Int bottomLeft = new Vector2Int(newLine.x, newLine.y - 1);
                        OnBoxMadeEvent.Invoke(bottomLeft);
                    }
                }
            }
        }
    }

    public void AddPlayerScore(int score) {
        if (isPlayerOneTurn) {
            playerOneScore += score;           
        }
        else {
            playerTwoScore += score;
        }

        OnScoreChangedEvent(isPlayerOneTurn);
    }

    public bool DoesLineExist(Line line) {
        if (line.horizontal) {
            return horizontalLines[line.x, line.y];
        }
        else {
            return verticalLines[line.x, line.y];
        }
    }

    public class Line
    {
        public bool horizontal;
        public int x, y;

        public Line(bool horizontal, int x, int y) {
            this.horizontal = horizontal;
            this.x = x;
            this.y = y;
        }
    }
}
