using System;
using System.Collections.Generic;
using Clicker;
using DG.Tweening;
using Game.Scripts;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DebugMenu : MonoBehaviour
{
    public Image backgroundImage;
    public Transform cameraHandle;
    public List<Gradient> backgroundColors;
    public List<CameraState> cameraStates;
    public List<Color> craftingTableColors;
    public MeshRenderer table1Renderer;
    public MeshRenderer table2Renderer;
    public MeshRenderer table3Renderer;
    public MeshRenderer table4Renderer;
    public Transform window;
    private int lastGradientIndex = -1;
    private int lastTableColorIndex = -1;
    private Material clonedMaterial;
    private bool isWindowActive;
    public List<Color> bgColors;

    private void Start()
    {
        clonedMaterial = backgroundImage.material = Instantiate(backgroundImage.material);
    }

    public void DebugClicked()
    {
        isWindowActive = !isWindowActive;
        window.gameObject.SetActive(isWindowActive);
    }

    public void RandomBackgroundClicked()
    {
        // var randomGradient = PickRandomGradient();
        var color = bgColors[Random.Range(0, bgColors.Count)];
        clonedMaterial.SetColor("_TopLeftColor", color);
        clonedMaterial.SetColor("_TopRightColor", color);
        clonedMaterial.SetColor("_BottomLeftColor", color);
        clonedMaterial.SetColor("_BottomRightColor", color);
    }

    public void ChangeCourierClicked()
    {
        ClickerManager.Instance.RandomCourierColorClicked();
    }

    public void ChangeHouseClicked()
    {
        HouseManager.Instance.RandomColorClicked();
    }

    public void CamClicked(int index)
    {
        SetCameraState(index);
    }

    public void RandomTableColorClicked()
    {
        var randomColor = PickRandomTableColor();
        table1Renderer.material.color =
            table2Renderer.material.color =
                table3Renderer.material.color =
                    table4Renderer.material.color = randomColor;
    }

    private void SetCameraState(int index)
    {
        var state = cameraStates[index];
        transform.DORewind();
        cameraHandle.DOLocalMove(state.position, 1f);
        cameraHandle.DOLocalRotateQuaternion(Quaternion.Euler(state.rotation), 1f);
    }

    private Color PickRandomTableColor()
    {
        var index = Random.Range(0, craftingTableColors.Count);
        while (index == lastTableColorIndex)
        {
            index = Random.Range(0, craftingTableColors.Count);
        }

        lastTableColorIndex = index;

        return craftingTableColors[index];
    }

    private Gradient PickRandomGradient()
    {
        var index = Random.Range(0, backgroundColors.Count);
        while (index == lastGradientIndex)
        {
            index = Random.Range(0, backgroundColors.Count);
        }

        lastGradientIndex = index;

        return backgroundColors[index];
    }

    [Serializable]
    public class Gradient
    {
        public Color from;
        public Color to;
    }

    [Serializable]
    public class CameraState
    {
        public Vector3 position;
        public Vector3 rotation;
    }
}