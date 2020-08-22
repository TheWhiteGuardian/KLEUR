using UnityEngine;
using System;
using Kopernicus;
using Kopernicus.Configuration;
using Kopernicus.Configuration.ModLoader;

namespace KLEUR
{
    public sealed class PQSMod_TiledHeightMap : PQSMod
    {
        public MapSO tiledMap;
        public float tiling = 1.0f;
        public float smoothness = 1.0f;
        public float deformationBalance = 1.0f;
        public float deformity = 1.0f;
        
        public override void OnVertexBuildHeight(PQS.VertexBuildData data)
        {
            double scalar = data.vertHeight - sphere.radius;
            scalar *= deformationBalance;
            scalar += sphere.radius;
            scalar *= tiling;
            Vector3d position = data.directionFromCenter * scalar;

            // Now that we have the position, we need to sample the MapSO
            // thrice, once for each plane.
            float x = tiledMap.GetPixelFloat(position.y, position.z);
            float y = tiledMap.GetPixelFloat(position.x, position.z);
            float z = tiledMap.GetPixelFloat(position.x, position.y);

            // Next, we need to process the normal (directionFromCenter).
            var blend = data.directionFromCenter;
            scalar = 1.0 / (blend.x + blend.y + blend.z);
            blend.x = Math.Pow(Math.Abs(blend.x), smoothness) * scalar;
            blend.y = Math.Pow(Math.Abs(blend.y), smoothness) * scalar;
            blend.z = Math.Pow(Math.Abs(blend.z), smoothness) * scalar;

            double height = (blend.x * x) + (blend.y * y) + (blend.z * z);

            data.vertHeight += deformity * height;
        }
    }
}
