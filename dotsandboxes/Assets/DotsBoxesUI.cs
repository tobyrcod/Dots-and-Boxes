using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

//Dots UI
public class DotsBoxesUI : MonoBehaviour
{
    private DotsBoxes dotsBoxes;

    public int width, height;
    public Vector2 dotOrigin;
    public float dotCellSize;
    public float dotOffset;

    [Space]

    [SerializeField] private Color playerOneColor;
    [SerializeField] private Color playerTwoColor;

    private Grid dotsPositions;
    private Transform[,] dotsUI;

    [Space]

    [SerializeField] private Transform dotParent;
    [SerializeField] private GameObject dotPrefab;

    [Space]

    [SerializeField] private Transform lineParent;
    [SerializeField] private GameObject LinePrefab;

    [Space]

    [SerializeField] private Transform boxParent;
    [SerializeField] private GameObject boxPrefab;

    [Space]

    [SerializeField] private Pen Pen;

    [Space]

    [SerializeField] TextMeshProUGUI playerOneScoreText;
    [SerializeField] TextMeshProUGUI playerTwoScoreText;

    [Space]

    [SerializeField] private Color HoverColor;

    DotsBoxes.Line hoverLine;
    GameObject hoverLineObject;

    private void Awake() {
        dotsPositions = new Grid(dotCellSize, dotOrigin);
        dotsUI = new Transform[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                dotsUI[x, y] = newDotPrefab(x, y);
            }
        }

        dotsBoxes = new DotsBoxes(width, height);
        dotsBoxes.OnAddLineEvent += OnAddLine;
        dotsBoxes.OnBoxMadeEvent += OnBoxMade;
        dotsBoxes.OnTurnOverEvent += OnTurnOver;
        dotsBoxes.OnScoreChangedEvent += OnScoreChanged;
    }

    private void OnScoreChanged(bool isPlayerOneTurn) {
        if (isPlayerOneTurn) {
            playerOneScoreText.text = dotsBoxes.playerOneScore.ToString();
        }
        else {
            playerTwoScoreText.text = dotsBoxes.playerTwoScore.ToString();
        }
    }

    private void OnTurnOver() {
        Pen.ChangePenSprite(dotsBoxes.isPlayerOneTurn);
    }

    private void OnBoxMade(Vector2Int bottomLeft) {
        Vector2 bottomLeftPos = dotsPositions.GetWorldPosition(bottomLeft.x, bottomLeft.y);
        Vector2 boxCenterPos = bottomLeftPos + new Vector2(dotCellSize, dotCellSize) * 0.5f;

        SpriteRenderer boxIcon = Instantiate(boxPrefab, boxParent).GetComponent<SpriteRenderer>();
        boxIcon.transform.position = boxCenterPos;
        if (dotsBoxes.isPlayerOneTurn) {
            //If it is now player Ones turn, as you get a new turn for making a box, it is player 1 who made the box
            boxIcon.color = playerOneColor;
        }
        else {
            boxIcon.color = playerTwoColor;
        }
    }

    private void Update() {

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Pen.transform.position = mousePos;

        Vector2 mousePosRelativeToDotOrigin = mousePos - dotOrigin;

        //Check for lines
        bool hoverLineFound = false;
        #region HorizontalLines
        int potentialY = -1;
        for (int y = 0; y < height; y++) {
            float yMax = (y * dotCellSize) + dotOffset;
            float yMin = (y * dotCellSize) - dotOffset;

            //If mouse y position is between yMax ans yMin, it is in the region
            if (mousePosRelativeToDotOrigin.y < yMax && mousePosRelativeToDotOrigin.y > yMin) {
                potentialY = y;
            }
        }

        if (potentialY != -1) {
            for (int x = 0; x < width - 1; x++) {
                float xMax = ((x + 1) * dotCellSize) - dotOffset;
                float xMin = (x * dotCellSize) + dotOffset;

                if (mousePosRelativeToDotOrigin.x < xMax && mousePosRelativeToDotOrigin.x > xMin) {
                    DotsBoxes.Line newHoverLine = new DotsBoxes.Line(true, x, potentialY);
                    if (!dotsBoxes.DoesLineExist(newHoverLine)) {
                        if (hoverLine == null || (hoverLine.x != newHoverLine.x && hoverLine.y != newHoverLine.y)) {
                            //New Hoverline.
                            Destroy(hoverLineObject);

                            hoverLine = newHoverLine;
                            hoverLineObject = DrawHoverLine(hoverLine);
                        }
                    }

                    hoverLineFound = true;
                }
            }
        }
        #endregion
        #region VerticalLines
        if (!hoverLineFound) {
            //If No Horizontal Line was placed, check for Vertical Lines

            int potentialX = -1;
            for (int x = 0; x < width; x++) {
                float xMax = (x * dotCellSize) + dotOffset;
                float xMin = (x * dotCellSize) - dotOffset;

                //If mouse y position is between yMax ans yMin, it is in the region
                if (mousePosRelativeToDotOrigin.x < xMax && mousePosRelativeToDotOrigin.x > xMin) {
                    potentialX = x;
                }
            }

            if (potentialX != -1) {
                for (int y = 0; y < height - 1; y++) {
                    float yMax = ((y + 1) * dotCellSize) - dotOffset;
                    float yMin = (y * dotCellSize) + dotOffset;

                    if (mousePosRelativeToDotOrigin.y < yMax && mousePosRelativeToDotOrigin.y > yMin) {
                        DotsBoxes.Line newHoverLine = new DotsBoxes.Line(false, potentialX, y);
                        if (!dotsBoxes.DoesLineExist(newHoverLine)) {
                            if (hoverLine == null || (hoverLine.x != newHoverLine.x && hoverLine.y != newHoverLine.y)) {
                                //New Hoverline.
                                Destroy(hoverLineObject);

                                hoverLine = newHoverLine;
                                hoverLineObject = DrawHoverLine(hoverLine);
                            }
                        }

                        hoverLineFound = true;
                    }
                }
            }
        }
        #endregion
        if (!hoverLineFound) {
            hoverLine = null;
            Destroy(hoverLineObject);
            hoverLineObject = null;
        }

        if (Input.GetMouseButtonDown(0)) {
            if (hoverLine != null) {
                if (dotsBoxes.TryAddLine(hoverLine)) {
                    Destroy(hoverLineObject);
                    hoverLineObject = null;
                }
            }
        }    
    }

    private Transform newDotPrefab(int x, int y) {
        Transform dot = Instantiate(dotPrefab, dotParent).transform;
        dot.position = dotsPositions.GetWorldPosition(x, y);

        return dot;
    }

    private void OnAddLine(DotsBoxes.Line line) {

        LineRenderer lr = Instantiate(LinePrefab, lineParent).GetComponent<LineRenderer>();
        lr.positionCount = 2;

        if (dotsBoxes.isPlayerOneTurn) {
            lr.startColor = playerOneColor;
            lr.endColor = playerOneColor;         
        }
        else {
            lr.startColor = playerTwoColor;
            lr.endColor = playerTwoColor;
        }

        if (!line.horizontal) {
            ////Not horizontal => must be vertical             
            lr.SetPosition(0, dotsPositions.GetWorldPosition(line.x, line.y));
            lr.SetPosition(1, dotsPositions.GetWorldPosition(line.x, line.y + 1));
        }
        else {
            lr.SetPosition(0, dotsPositions.GetWorldPosition(line.x, line.y));
            lr.SetPosition(1, dotsPositions.GetWorldPosition(line.x + 1, line.y));
        }
    }

    private GameObject DrawHoverLine(DotsBoxes.Line hoverLine) {
        LineRenderer lr = Instantiate(LinePrefab, lineParent).GetComponent<LineRenderer>();

        if (dotsBoxes.isPlayerOneTurn) {
            lr.startColor = new Color(playerOneColor.r, playerOneColor.g, playerOneColor.b, 0.7f);
            lr.endColor = new Color(playerOneColor.r, playerOneColor.g, playerOneColor.b, 0.7f);
        }
        else {
            lr.startColor = new Color(playerTwoColor.r, playerTwoColor.g, playerTwoColor.b, 0.7f);
            lr.endColor = new Color(playerTwoColor.r, playerTwoColor.g, playerTwoColor.b, 0.7f);
        }

        lr.positionCount = 2;
        if (!hoverLine.horizontal) {
            ////Not horizontal => must be vertical             
            lr.SetPosition(0, dotsPositions.GetWorldPosition(hoverLine.x, hoverLine.y));
            lr.SetPosition(1, dotsPositions.GetWorldPosition(hoverLine.x, hoverLine.y + 1));
        }
        else {
            lr.SetPosition(0, dotsPositions.GetWorldPosition(hoverLine.x, hoverLine.y));
            lr.SetPosition(1, dotsPositions.GetWorldPosition(hoverLine.x + 1, hoverLine.y));
        }

        return lr.gameObject;
    }
}
