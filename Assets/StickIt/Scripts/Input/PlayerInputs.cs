// GENERATED AUTOMATICALLY FROM 'Assets/StickIt/Scripts/Input/PlayerInputs.inputactions'

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
                    ""name"": ""Direction"",
                    ""type"": ""Value"",
                    ""id"": ""b16cc044-ba91-485f-a9b1-3a9a667baf49"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MousePosition"",
                    ""type"": ""Value"",
                    ""id"": ""d6731089-5380-4b24-a009-c1343ba1c4d1"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Quit"",
                    ""type"": ""Button"",
                    ""id"": ""b0ad7773-f602-4180-bfda-02abba2ebfd3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Test"",
                    ""type"": ""Button"",
                    ""id"": ""b9b77ec6-9fc8-4ad8-a2f8-fcf304d35387"",
                    ""expectedControlType"": ""Button"",
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
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""93e22bf6-7b1a-4099-9889-61237ed77478"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e669e173-b6da-40ec-ab8c-39008d1f44c4"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Direction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d1ce3d64-5943-4623-b6bb-3335016c4fde"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""MousePosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3dc3c78b-c892-4289-b41b-e31cc1b813fd"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad;Keyboard"",
                    ""action"": ""Quit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f1c24fb3-58ad-419d-8e3a-cad8c4b9f8de"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Test"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // NormalInputs
        m_NormalInputs = asset.FindActionMap("NormalInputs", throwIfNotFound: true);
        m_NormalInputs_Jump = m_NormalInputs.FindAction("Jump", throwIfNotFound: true);
        m_NormalInputs_Direction = m_NormalInputs.FindAction("Direction", throwIfNotFound: true);
        m_NormalInputs_MousePosition = m_NormalInputs.FindAction("MousePosition", throwIfNotFound: true);
        m_NormalInputs_Quit = m_NormalInputs.FindAction("Quit", throwIfNotFound: true);
        m_NormalInputs_Test = m_NormalInputs.FindAction("Test", throwIfNotFound: true);
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
    private readonly InputAction m_NormalInputs_Direction;
    private readonly InputAction m_NormalInputs_MousePosition;
    private readonly InputAction m_NormalInputs_Quit;
    private readonly InputAction m_NormalInputs_Test;
    public struct NormalInputsActions
    {
        private @PlayerInputs m_Wrapper;
        public NormalInputsActions(@PlayerInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_NormalInputs_Jump;
        public InputAction @Direction => m_Wrapper.m_NormalInputs_Direction;
        public InputAction @MousePosition => m_Wrapper.m_NormalInputs_MousePosition;
        public InputAction @Quit => m_Wrapper.m_NormalInputs_Quit;
        public InputAction @Test => m_Wrapper.m_NormalInputs_Test;
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
                @Direction.started -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnDirection;
                @Direction.performed -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnDirection;
                @Direction.canceled -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnDirection;
                @MousePosition.started -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnMousePosition;
                @MousePosition.performed -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnMousePosition;
                @MousePosition.canceled -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnMousePosition;
                @Quit.started -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnQuit;
                @Quit.performed -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnQuit;
                @Quit.canceled -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnQuit;
                @Test.started -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnTest;
                @Test.performed -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnTest;
                @Test.canceled -= m_Wrapper.m_NormalInputsActionsCallbackInterface.OnTest;
            }
            m_Wrapper.m_NormalInputsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Direction.started += instance.OnDirection;
                @Direction.performed += instance.OnDirection;
                @Direction.canceled += instance.OnDirection;
                @MousePosition.started += instance.OnMousePosition;
                @MousePosition.performed += instance.OnMousePosition;
                @MousePosition.canceled += instance.OnMousePosition;
                @Quit.started += instance.OnQuit;
                @Quit.performed += instance.OnQuit;
                @Quit.canceled += instance.OnQuit;
                @Test.started += instance.OnTest;
                @Test.performed += instance.OnTest;
                @Test.canceled += instance.OnTest;
            }
        }
    }
    public NormalInputsActions @NormalInputs => new NormalInputsActions(this);
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface INormalInputsActions
    {
        void OnJump(InputAction.CallbackContext context);
        void OnDirection(InputAction.CallbackContext context);
        void OnMousePosition(InputAction.CallbackContext context);
        void OnQuit(InputAction.CallbackContext context);
        void OnTest(InputAction.CallbackContext context);
    }
}
