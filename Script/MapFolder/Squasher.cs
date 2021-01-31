/**
 * Copyright 2020 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using UnityEngine;

namespace Google.Maps.Demos.Zoinkies {
    /// <summary>
    /// Modifies the vertical scaling of a GameObject based on the proximity of a target object.
    /// </summary>
    public class Squasher : MonoBehaviour {
        /// <summary>
        /// The Transform (GameObject) whose position controls the degree of squashing.
        /// </summary>
        public Transform Target;

        /// <summary>
        /// The distance outside which no squashing is applied.
        /// </summary>
        public float Far = 200.0f;

        /// <summary>
        /// The distance within which <see cref="MaximumSquashing"/> is applied.
        /// </summary>
        public float Near = 20.0f;

        /// <summary>
        /// The maximum amount of squashing that will be applied, where 1.0 is no squashing and 0.0 is
        /// 100% squashing to 0 height.
        /// </summary>
        public float MaximumSquashing = 0.1f;

        /// <summary>
        /// Linearly interpolate the squashing factor based on the proximity of <see cref="Target"/>
        /// </summary>
        private void Update() {
            float dist = (Target.transform.position - transform.position).magnitude;

            float normalized = (dist - Near) / (Far - Near);
            float scale = Mathf.Lerp(MaximumSquashing, 1.0f, Mathf.Clamp(normalized, 0, 1));
            transform.localScale = new Vector3(1, scale, 1);
            
            
        }
    }
}
