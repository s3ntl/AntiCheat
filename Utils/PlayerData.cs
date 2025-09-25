using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AntiCheat.Utils
{
    public struct PlayerData
    {
        // transform
        public Vector3 position;
        public Quaternion quaternion;

        // rigidbody
        public Vector3 velocity;

        // костыль
        public bool isFlying;
    }
}
