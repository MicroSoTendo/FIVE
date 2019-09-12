﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.CameraSystem
{
    public class CameraPosition
    {
        static CameraPosition instance;
        public Transform position;
        public static CameraPosition Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CameraPosition();
                }

                return instance;
            }
        }
    }

}