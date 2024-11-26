using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class CameraPostProcessor : MonoBehaviour
{
    [SerializeField]
    protected List<Shader> _effectsStack;

    protected Camera _camera;

    protected List<Material> _effectMaterials;
    protected CommandBuffer _commandBuffer;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _effectMaterials = new List<Material>();

        _commandBuffer = new CommandBuffer();
        foreach(Shader effect in _effectsStack)
        {
            Material effectMaterial = new Material(effect);
            _effectMaterials.Add(effectMaterial);

            _commandBuffer.Blit(_camera.targetTexture, _camera.targetTexture, effectMaterial);
        }

        _camera.AddCommandBuffer(CameraEvent.AfterEverything, _commandBuffer);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
    }

    private void OnDestroy()
    {
        _camera.RemoveCommandBuffer(CameraEvent.AfterEverything, _commandBuffer);
        _commandBuffer.Release();
    }
}
