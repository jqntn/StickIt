// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/PlayerInputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInputs : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputs"",
    ""maps"": [
        {
            ""name"": ""NormalInputs"",
            ""id"": ""55b1d386-ead8-4d59-b3b6-320d9db689de"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""c1a302f3-670e-4b70-8fe3-42f056ed090b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""31ad1e37-f4f5-4479-9e8c-fc8a429f2821"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""52281ed5-70c9-4610-801d-a6958c8f28ff"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d1ce3d64-5943-4623-b6bb-3335016c4fde"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // NormalInputs
        m_NormalInputs = asset.FindActionMap("NormalInputs", throwIfNotFound: true);
        m_NormalInputs_Jump = m_NormalInputs.FindAction("Jump", throwIfNotFound: true);
        m_NormalInputs_MousePosition = m_NormalInputs.FindAction("MousePosition", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // NormalInputs
    private readonly InputActionMap m_NormalInputs;
    private INormalInputsActions m_NormalInputsActionsCallbackInterface;
    private readonly InputAction m_NormalInputs_Jump;
    private readonly InputAction m_NormalInputs_MousePosition;
    public struct NormalInputsActions
    {
        private @PlayerInputs m_Wrapper;
        public NormalInputsActions(@PlayerInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_NormalInputs_Jump;
        public InputAction @MousePosition => m_Wrapper.m_NormalInputs_MousePosition;
        public InputActionMap Get() { return m_Wrapper.m_NormalInputs; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(NormalInputsActions set) { return set.Get(); }
        public void SetCallbacks(INormalInputsActions instance)
        {
            if (m_Wrapper.m_NormalInputsActionsCallbackInterface != null)
            {
                @Jump.started -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnJump;
                @MousePosition.started -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnMousePosition;
            }
            m_Wrapper.m_NormalInputsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
            }
        }
    }
    public NormalInputsActions @NormalInputs => new NormalInputsActions(this);
    public interface INormalInputsActions
    {
        void OnJump(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
    }
}
