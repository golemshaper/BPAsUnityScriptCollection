﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NaiveECS
{
    public class NaiveEntity : MonoBehaviour
    {
        public int id;
        //NO! Bad:
        public Transform myTransform;
    }
}