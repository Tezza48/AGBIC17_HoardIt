using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using HoardIt;
using UnityInput = UnityEngine.Input;

namespace HoardIt.Core
{
    internal struct ArcadeInput
    {
        //struct State
        //{
        //    bool[] moveStates;
        //    bool[] buttonStates;
        //}

        KeyCode[] m_MoveDirections;
        KeyCode[] m_Buttons;

        public KeyCode[] Buttons { get { return m_Buttons; } }

        //State m_LastState;
        //State m_CurrentState;

        public ArcadeInput(KeyCode up, KeyCode right, KeyCode down, KeyCode left, KeyCode one, KeyCode two)
        {
            m_MoveDirections = new KeyCode[4] { up, right, down, left };
            m_Buttons = new KeyCode[2] { one, two };

            //m_LastState = new State();
            //m_CurrentState = new State();
        }

        public int getVertical()
        {
            int value;
            value = UnityInput.GetKey(m_MoveDirections[0]) ? 1 : 0;
            value -= UnityInput.GetKey(m_MoveDirections[2]) ? 1 : 0;
            return value;
        }

        public int getHorizontal()
        {
            int value;
            value = UnityInput.GetKey(m_MoveDirections[1]) ? 1 : 0;
            value -= UnityInput.GetKey(m_MoveDirections[3]) ? 1 : 0;
            return value;
        }

    }
    static class Input
    {
        private static ArcadeInput[] m_PlayerInputs = new ArcadeInput[4]
        {
            new ArcadeInput(KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.Period, KeyCode.Slash),
            new ArcadeInput(KeyCode.W, KeyCode.D, KeyCode.S, KeyCode.A, KeyCode.BackQuote, KeyCode.Alpha1),
            new ArcadeInput(KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.G, KeyCode.H),
            new ArcadeInput(KeyCode.Keypad8, KeyCode.Keypad6, KeyCode.Keypad5, KeyCode.Keypad4, KeyCode.Keypad1, KeyCode.Keypad2)
        };

        public static int getVertical(int playerIndex)
        {
            return m_PlayerInputs[playerIndex].getVertical();
        }

        public static int getHorizontal(int playerIndex)
        {
            return m_PlayerInputs[playerIndex].getHorizontal();
        }

        public static bool getButtonDown(int playerIndex, int button)
        {
            if (button < 0 || button >= 2)
                throw new Exception("getButtonDown: Button must be 0 or 1, you entered: " + button);
            else
                return UnityInput.GetKeyDown(m_PlayerInputs[playerIndex].Buttons[button]);
        }

        public static bool getButtonUp(int playerIndex, int button)
        {
            if (button < 0 && button >= 2)
                throw new Exception("getButtonDown: Button must be 0 or 1, you entered: " + button);
            else
                return UnityInput.GetKeyUp(m_PlayerInputs[playerIndex].Buttons[button]);
        }

        public static bool getButton(int playerIndex, int button)
        {
            if (button < 0 && button >= 2)
                throw new Exception("getButtonDown: Button must be 0 or 1, you entered: " + button);
            else
                return UnityInput.GetKey(m_PlayerInputs[playerIndex].Buttons[button]);
        }
    }
}
