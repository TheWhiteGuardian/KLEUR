using UnityEngine;
using System;
using Kopernicus.Configuration.ModLoader;
using Kopernicus.ConfigParser.Attributes;
using Kopernicus.ConfigParser.Enumerations;
using Kopernicus.ConfigParser.BuiltinTypeParsers;
using Kopernicus.Configuration.Parsing;

namespace KLEUR
{
    public sealed class PQSMod_TiledHeightMap : PQSMod
    {
        public MapSO tiledMap;
        public float tiling = 1.0f;
        public float smoothness = 1.0f;
        public float deformationBalance = 1.0f;
        public float deformity = 1.0f;
        public bool scaleByRadius = false;
        
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
            blend.x = Math.Pow(Math.Abs(blend.x), smoothness);
            blend.y = Math.Pow(Math.Abs(blend.y), smoothness);
            blend.z = Math.Pow(Math.Abs(blend.z), smoothness);
            blend /= (blend.x + blend.y + blend.z);

            double height = (blend.x * x) + (blend.y * y) + (blend.z * z);

            if (scaleByRadius)
            {
                height *= sphere.radius;
            }

            data.vertHeight += deformity * height;
        }
    }

    [RequireConfigType(ConfigType.Node)]
    public sealed class TiledHeightMap : ModLoader<PQSMod_TiledHeightMap>
    {
        [ParserTarget("map")]
        public MapSOParserGreyScale<MapSO> Map
        {
            get => Mod.tiledMap;
            set => Mod.tiledMap = value;
        }

        [ParserTarget("tiling")]
        public NumericParser<float> Tiling
        {
            get => Mod.tiling;
            set => Mod.tiling = value;
        }

        [ParserTarget("smoothness")]
        public NumericParser<float> Smoothness
        {
            get => Mod.smoothness;
            set => Mod.smoothness = value;
        }

        [ParserTarget("balance")]
        public NumericParser<float> Balance
        {
            get => Mod.deformationBalance;
            set => Mod.deformationBalance = Mathf.Clamp01(value);
        }

        [ParserTarget("deformity")]
        public NumericParser<float> Deformity
        {
            get => Mod.deformity;
            set => Mod.deformity = value;
        }

        [ParserTarget("scaleByRadius")]
        public NumericParser<bool> ScaleByRadius
        {
            get => Mod.scaleByRadius;
            set => Mod.scaleByRadius = value;
        }
    }
}
