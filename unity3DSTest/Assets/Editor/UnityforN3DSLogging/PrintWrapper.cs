﻿using UnityEngine;

namespace N3DSLogReceiver
{

    public abstract class PrintWrapper
    {
        public void Display()
        {
            Rect rect = new Rect(10, 10, 300, 40);
            GUI.Label(rect, status);
        }

        public string Status
        {
            set
            {
                status = value;
                Debug.Log(status);
            }
        }
        private string status;
    }

}
